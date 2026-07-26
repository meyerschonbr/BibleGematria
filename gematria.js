'use strict';

const GEMATRIA_VALUES = {
    'א': 1, 'ב': 2, 'ג': 3, 'ד': 4, 'ה': 5,
    'ו': 6, 'ז': 7, 'ח': 8, 'ט': 9,
    'י': 10,
    'כ': 20, 'ך': 20,
    'ל': 30,
    'מ': 40, 'ם': 40,
    'נ': 50, 'ן': 50,
    'ס': 60, 'ע': 70,
    'פ': 80, 'ף': 80,
    'צ': 90, 'ץ': 90,
    'ק': 100, 'ר': 200, 'ש': 300, 'ת': 400
};
function normalize(input) {
    if (!input) return '';

    let result = '';
    for (const ch of input) {
        if (ch >= '\u05D0' && ch <= '\u05EA') {
            result += ch;
        }
    }
    return result;
}
function compute(input) {
    if (!input) {
        return 0;
    }
    const clean = normalize(input);
    let total = 0;
    for (const ch of clean) {
        if (GEMATRIA_VALUES[ch] !== undefined) {
            total += GEMATRIA_VALUES[ch];
        }
    }
    return total;
}
export { normalize, compute };