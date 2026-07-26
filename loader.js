'use strict';
import { normalize, compute } from "./gematria.js";

const SPLIT_CHARS = [' ', '\u05BE', '-'];
const ETNACHTA_CHAR = '\u0591';

function splitIntoWords(verseText) {
    const rawWords = verseText.split(/[ \u05BE-]/).filter(w => w.length > 0);

    const words = [];
    for (let i = 0; i < rawWords.length; i++){
        const word = rawWords[i];
        const clean = normalize(word);

        words.push({
            text: word,
            cleanText: clean,
            gematriaValue: compute(clean),
            wordIndex: i,
            hasEtnachta: word.includes(ETNACHTA_CHAR)
        });
    }
    return words;
}

function normalizeVerseText(text) {
    if (!text || !text.trim()) return '';

    const cleaned = text.replace('(פ)', '').replace('(ס)', '');
    const parts = cleaned
        .split(' ')
        .filter(t => t.length > 0 && t !== 'פ' && t !== 'ס' && t !== '(פ)' && t !== '(ס)');
    return parts.join(' ').trim();
}

async function loadBook(url) {
    const response = await fetch(url);
    if (!response.ok) {
        throw new Error(`Failed to load ${url}: ${response.status}`);
    }

    const rawBook = await response.json();
    if (!rawBook || !rawBook.text) return [];

    const verses = [];

    for (let i = 0; i < rawBook.text.length; i++){
        const chapter = rawBook.text[i];
        for (let j = 0; j < chapter.length; j++){
            const verseText = normalizeVerseText(chapter[j]);
            verses.push({
                bookName: rawBook.title,
                chapter: i + 1,
                verseNumber: j + 1,
                fullText: verseText,
                words: splitIntoWords(verseText)
            });
        }
    }
    return verses;
}

export { loadBook };