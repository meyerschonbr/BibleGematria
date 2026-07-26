'use strict';

const DEFAULT_MAX_PHRASE_LENGTH = 20;

function findPhraseMatches(verses, target, maxPhraseLength = DEFAULT_MAX_PHRASE_LENGTH) {
    const results = [];

    for (const verse of verses) {
        const n = verse.words.length;
        if (n === 0) continue;

        const prefix = new Array(n + 1).fill(0);
        for (let i = 0; i < n; i++){
            prefix[i + 1] = prefix[i] + verse.words[i].gematriaValue;
        }
        for (let start = 0; start < n; start++){
            const maxLen = Math.min(maxPhraseLength, n - start)
            for (let len = 1; len <= maxLen; len++){
                const endExclusive = start + len;
                const sum = prefix[endExclusive] - prefix[start];
                if (sum === target) {
                    const phrase = verse.words
                        .slice(start, endExclusive)
                        .map(w => w.text)
                        .join(' ');

                    results.push({
                        bookName: verse.bookName,
                        chapter: verse.chapter,
                        verseNumber: verse.verseNumber,
                        verseText: verse.fullText,
                        matchedText: phrase,
                        startWordIndex: start,
                        wordCount: len
                    });
                }
            }
        }
    }
    return results;
}
function findPhraseMatchesNoEtnachta(verses, target, maxPhraseLength = DEFAULT_MAX_PHRASE_LENGTH) {
    const results = [];

    for (const verse of verses) {
        const n = verse.words.length;
        if (n === 0) continue;

        const prefix = new Array(n + 1).fill(0);
        for (let i = 0; i < n; i++){
            prefix[i + 1] = prefix[i] + verse.words[i].gematriaValue;
        }
        for (let start = 0; start < n; start++){
            const maxLen = Math.min(maxPhraseLength, n - start)
            for (let len = 2; len <= maxLen; len++){
                const crossesBoundary = verse.words
                    .slice(start, start + len - 1)
                    .some(w => w.hasEtnachta);
                if (crossesBoundary) continue;

                const endExclusive = start + len;
                const sum = prefix[endExclusive] - prefix[start];
                if (sum === target) {
                    const phrase = verse.words
                        .slice(start, endExclusive)
                        .map(w => w.text)
                        .join(' ');

                    results.push({
                        bookName: verse.bookName,
                        chapter: verse.chapter,
                        verseNumber: verse.verseNumber,
                        verseText: verse.fullText,
                        matchedText: phrase,
                        startWordIndex: start,
                        wordCount: len
                    });
                }
            }
        }
    }
    return results;
}

export { findPhraseMatches, findPhraseMatchesNoEtnachta };
