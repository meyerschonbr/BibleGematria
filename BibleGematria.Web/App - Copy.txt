/* =====================================================================
   DEMO PLUMBING ONLY — not your real search.
   This script exists so the styling and animations react to clicks.
   When you wire up the JavaScript port of your C# core, it replaces the
   fake search here. The mapping:
     • computeGematria()   ↔  GematriaCalculator.Compute
     • the searchBtn flow  ↔  ExecuteSearchAsyncImpl (set state, run, fill)
     • the hard-coded cards in index.html  ↔  MatchResult objects you'll build
   Nothing about the HTML/CSS depends on this file being present.
   ===================================================================== */

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
});
bookCbs.forEach(cb => cb.addEventListener('change', () => {
  allBooks.checked = [...bookCbs].every(c => c.checked);
}));

// --- fake search: empty input → "none", otherwise → "results" ---
const searchBtn = document.getElementById('searchBtn');
const counting  = document.getElementById('counting');
const HEB = 'אבגדהוזחטיכלמנסעפצקרשת';

searchBtn.addEventListener('click', () => {
  const found = computeGematria(input.value) > 0;

  stage.dataset.state = 'searching';
  searchBtn.disabled = true;

  // flick random letters in the counter for that "computing" feel
  const tick = setInterval(() => {
    counting.textContent = HEB[Math.floor(Math.random() * HEB.length)];
  }, 70);

  // it's instant in reality — this delay just lets the animation breathe
  setTimeout(() => {
    clearInterval(tick);
    stage.dataset.state = found ? 'results' : 'none';
    searchBtn.disabled = false;
  }, 850);
});