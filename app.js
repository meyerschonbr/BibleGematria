'use strict';

import { compute } from './gematria.js';
import { getByKey } from './books.js';
import { loadBook } from './loader.js';
import { findPhraseMatches, findPhraseMatchesNoEtnachta } from './search.js';
import { exportToXlsx } from './xlsx.js';

const input   = document.getElementById('hebrewInput');
const valueEl = document.getElementById('valueNumber');
const stage = document.getElementById('stage');
const bookCache = new Map();  //bookKey -> Promise<Verse[]>

function getBookVerses(bookKey) {
  if (!bookCache.has(bookKey)) {
    const book = getByKey(bookKey);
    const promise = loadBook(`./data/${book.fileName}`);
    bookCache.set(bookKey, promise);
  }
  return bookCache.get(bookKey);
}

// sum only Hebrew letters U+05D0–U+05EA
function computeGematria(text) {
  return compute(text);
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

let lastSearch = null;  // last search params, so re-searching (e.g. checkbox change) is possible
let lastResults = null; // last match results, so Export can build the xlsx without re-searching

function buildSearchParams() {
  const bookKeys = [...bookCbs]
    .filter(cb => cb.checked)
    .map(cb => cb.dataset.key);
  return {
    hebrewInput: input.value,
    bookKeys,
    noCrossEtnachta: document.getElementById('noEtnachta').checked
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
  const params = buildSearchParams();
  if (!params.hebrewInput.trim() || params.bookKeys.length === 0) return;

  lastSearch = params;
  stage.dataset.state = 'searching';
  searchBtn.disabled = true;
  exportBtn.style.display = 'none';
  clearBtn.style.display = 'none';

  const tick = setInterval(() => {
    counting.textContent = HEB[Math.floor(Math.random() * HEB.length)];
  }, 70);

  try {
    const bookVerseLists = await Promise.all(
      params.bookKeys.map(key => getBookVerses(key))
    );
    const verses = bookVerseLists.flat();

    const target = compute(params.hebrewInput);
    const results = params.noCrossEtnachta
      ? findPhraseMatchesNoEtnachta(verses, target)
      : findPhraseMatches(verses, target);

    clearInterval(tick);

    if (results.length === 0) {
      lastResults = null;
      stage.dataset.state = 'none';
    } else {
      lastResults = results;
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
  lastSearch = null;
  lastResults = null;
  document.querySelector('.state-results').querySelectorAll('.result-card').forEach(c => c.remove());
});

exportBtn.addEventListener('click', () => {
  if (!lastSearch || !lastResults) return;

  const target = compute(lastSearch.hebrewInput);
  const { bytes, fileName } = exportToXlsx(lastResults, lastSearch.hebrewInput, target);

  const blob = new Blob([bytes], {
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
  });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = fileName;
  a.click();
  URL.revokeObjectURL(url);
});
