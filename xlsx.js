'use strict';

function buildRows(results, searchedText, gematriaValue) {
    const rows = [
        ['Searched Text', searchedText],
        ['Gematria Value', gematriaValue],
        ['Book', 'Chapter', 'Verse', 'Match', 'Word Count', 'Full Verse']
    ];
    for (const r of results) {
        rows.push([r.bookName, r.chapter, r.verseNumber, r.matchedText, r.wordCount, r.verseText]);
    }
    return rows;
}

const MAX_COLUMN_WIDTH = 90;

function computeColumnWidths(rows) {
    const colCount = Math.max(...rows.map(row => row.length));
    const widths = new Array(colCount).fill(0);

    for (const row of rows) {
        for (let col = 0; col < row.length; col++){
            const cellText = String(row[col] ?? '');
            widths[col] = Math.max(widths[col], cellText.length);
        }
    }
    return widths.map(w => ({ wch: Math.min(w, MAX_COLUMN_WIDTH) + 2 }));
}

function buildWorkbook(results, searchedText, gematriaValue) {
    const rows = buildRows(results, searchedText, gematriaValue);

    const worksheet = XLSX.utils.aoa_to_sheet(rows);
    worksheet['!cols'] = computeColumnWidths(rows);

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Results');

    return workbook;
}

const INVALID_FILENAME_CHARS = /[\\/:*?"<>|]/g;

function buildExportFileName(searchedText) {
  const firstThreeWords = searchedText
    .split(' ')
    .filter(word => word.length > 0)
    .slice(0, 3)
    .map(word => word.replace(INVALID_FILENAME_CHARS, ''));

  const namePart = firstThreeWords.join('-');

  return namePart.length === 0
    ? 'gematria-results.xlsx'
    : `gematria-results-${namePart}.xlsx`;
}

function exportToXlsx(results, searchedText, gematriaValue) {
    const workbook = buildWorkbook(results, searchedText, gematriaValue);
    const bytes = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const fileName = buildExportFileName(searchedText);
  
    return { bytes, fileName };
  }
  
  export { exportToXlsx };
  