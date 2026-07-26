// mirror of your GematriaValues table (final forms included)
const G = {'א':1,'ב':2,'ג':3,'ד':4,'ה':5,'ו':6,'ז':7,'ח':8,'ט':9,
  'י':10,'כ':20,'ך':20,'ל':30,'מ':40,'ם':40,'נ':50,'ן':50,'ס':60,
  'ע':70,'פ':80,'ף':80,'צ':90,'ץ':90,'ק':100,'ר':200,'ש':300,'ת':400};

const input   = document.getElementById('hebrewInput');
const valueEl = document.getElementById('valueNumber');
const stage   = document.getElementById('stage');

// sum only Hebrew letters U+05D0–U+05EA, like Normalize() does
function computeGematria(text) {
  let total = 0;
  for (const ch of text) if (G[ch]) total += G[ch];
  return total;
}

function refreshValue() {
  valueEl.textContent = computeGematria(input.value);
  valueEl.classList.add('bump');
  setTimeout(() => valueEl.classList.remove('bump'), 150);
}
input.addEventListener('input', refreshValue);

// --- keyboard toggle ---
const kbToggle = document.getElementById('kbToggle');
const keyboard = document.getElementById('keyboard');
kbToggle.addEventListener('click', () => {
  const open = keyboard.classList.toggle('open');
  kbToggle.setAttribute('aria-pressed', String(open));
});

// --- on-screen keys type into the field ---
keyboard.querySelectorAll('.key').forEach(key => {
  key.addEventListener('click', () => {
    if (key.dataset.action === 'backspace') {
      input.value = input.value.slice(0, -1);
    } else {
      input.value += key.dataset.char;
    }
    input.focus();
    refreshValue();
  });
});

// --- "All books" master toggle ---
const allBooks = document.getElementById('allBooks');
const bookCbs  = document.querySelectorAll('.book-cb');
allBooks.addEventListener('change', () => {
  bookCbs.forEach(cb => cb.checked = allBooks.checked);
  if (stage.dataset.state === 'results') runSearch();
});
bookCbs.forEach(cb => cb.addEventListener('change', () => {
  allBooks.checked = [...bookCbs].every(c => c.checked);
}));

// --- search ---
const searchBtn = document.getElementById('searchBtn');
const clearBtn  = document.getElementById('clearBtn');
const exportBtn = document.getElementById('exportBtn');
const counting  = document.getElementById('counting');
const HEB = 'אבגדהוזחטיכלמנסעפצקרשת';

const API_BASE = '';

let lastRequest = null; // remembered so Export can re-use it

function buildRequest() {
  const bookKeys = [...bookCbs]
    .filter(cb => cb.checked)
    .map(cb => cb.dataset.key);
  return {
    HebrewInput: input.value,
    BookKeys: bookKeys,
    NoCrossEtnachta: document.getElementById('noEtnachta').checked
  };
}

// Wrap words [startIndex, startIndex + wordCount) in a highlight span.
// Splits on the same separators as TanachLoader (space, maqqaf, hyphen),
// keeping the separators so the verse renders unchanged.
function highlightMatch(verseText, startIndex, wordCount) {
  const parts = verseText.split(/([ ־-])/);
  let wordIdx = 0;
  let out = '';
  let open = false;
  for (const part of parts) {
    if (part === '') continue;
    if (/^[ ־-]$/.test(part)) {
      out += part;
      continue;
    }
    if (wordIdx === startIndex) { out += '<span class="match">'; open = true; }
    out += part;
    wordIdx++;
    if (open && wordIdx === startIndex + wordCount) { out += '</span>'; open = false; }
  }
  if (open) out += '</span>';
  return out;
}

function renderResults(data) {
  const resultsState = document.querySelector('.state-results');

  document.getElementById('matchCount').innerHTML = `<strong>${data.length}</strong> match${data.length === 1 ? '' : 'es'}`;
  document.getElementById('resultsMeta').style.display = '';

  resultsState.querySelectorAll('.result-card').forEach(c => c.remove());

  data.forEach(r => {
    const card = document.createElement('div');
    card.className = 'result-card';

    const highlighted = highlightMatch(r.verseText, r.startWordIndex, r.wordCount);

    card.innerHTML = `
      <div class="result-head">
        <span class="result-ref">${r.bookName} ${r.chapter}:${r.verseNumber}</span>
        <span class="result-badge">${computeGematria(r.matchedText)}</span>
      </div>
      <p class="result-verse" dir="rtl">${highlighted}</p>`;
    resultsState.appendChild(card);
  });
}

async function runSearch() {
  const req = buildRequest();
  if (!req.HebrewInput.trim() || req.BookKeys.length === 0) return;

  lastRequest = req;
  stage.dataset.state = 'searching';
  searchBtn.disabled = true;
  exportBtn.style.display = 'none';
  clearBtn.style.display = 'none';

  const tick = setInterval(() => {
    counting.textContent = HEB[Math.floor(Math.random() * HEB.length)];
  }, 70);

  try {
    const response = await fetch(`${API_BASE}/api/search`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(req)
    });

    clearInterval(tick);

    if (!response.ok) {
      stage.dataset.state = 'none';
      return;
    }

    const results = await response.json();
    if (results.length === 0) {
      stage.dataset.state = 'none';
    } else {
      renderResults(results);
      stage.dataset.state = 'results';
      exportBtn.style.display = '';
      clearBtn.style.display = '';
    }
  } catch {
    clearInterval(tick);
    stage.dataset.state = 'none';
  } finally {
    searchBtn.disabled = false;
  }
}

searchBtn.addEventListener('click', runSearch);

bookCbs.forEach(cb => cb.addEventListener('change', () => {
  if (stage.dataset.state === 'results') runSearch();
}));

document.getElementById('noEtnachta').addEventListener('change', () => {
  if (stage.dataset.state === 'results') runSearch();
});

clearBtn.addEventListener('click', () => {
  stage.dataset.state = 'empty';
  clearBtn.style.display = 'none';
  exportBtn.style.display = 'none';
  document.getElementById('resultsMeta').style.display = 'none';
  lastRequest = null;
  document.querySelector('.state-results').querySelectorAll('.result-card').forEach(c => c.remove());
});

exportBtn.addEventListener('click', async () => {
  if (!lastRequest) return;

  const response = await fetch(`${API_BASE}/api/search/export`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(lastRequest)
  });

  if (!response.ok) return;

  const disposition = response.headers.get('Content-Disposition') || '';
  const encodedMatch = disposition.match(/filename\*=UTF-8''([^;]+)/);
  const fileName = encodedMatch ? decodeURIComponent(encodedMatch[1]) : 'gematria-results.xlsx';

  const blob = await response.blob();
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = fileName;
  a.click();
  URL.revokeObjectURL(url);
});
