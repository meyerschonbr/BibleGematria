'use strict';

const BibleSection = Object.freeze({
    TORAH: 'Torah',
    PROPHETS: 'Prophets',
    WRITINGS: 'Writings'
});
const books = [
    { key: 'Genesis',      hebrewName: 'בראשית',        fileName: 'Genesis.json',      section: BibleSection.TORAH },
    { key: 'Exodus',       hebrewName: 'שמות',           fileName: 'Exodus.json',       section: BibleSection.TORAH },
    { key: 'Leviticus',    hebrewName: 'ויקרא',          fileName: 'Leviticus.json',    section: BibleSection.TORAH },
    { key: 'Numbers',      hebrewName: 'במדבר',          fileName: 'Numbers.json',      section: BibleSection.TORAH },
    { key: 'Deuteronomy',  hebrewName: 'דברים',          fileName: 'Deuteronomy.json',  section: BibleSection.TORAH },
  
    { key: 'Joshua',       hebrewName: 'יהושע',          fileName: 'Joshua.json',       section: BibleSection.PROPHETS },
    { key: 'Judges',       hebrewName: 'שופטים',         fileName: 'Judges.json',       section: BibleSection.PROPHETS },
    { key: 'ISamuel',      hebrewName: 'שמואל א',        fileName: 'ISamuel.json',      section: BibleSection.PROPHETS },
    { key: 'IISamuel',     hebrewName: 'שמואל ב',        fileName: 'IISamuel.json',     section: BibleSection.PROPHETS },
    { key: 'IKings',       hebrewName: 'מלכים א',        fileName: 'IKings.json',       section: BibleSection.PROPHETS },
    { key: 'IIKings',      hebrewName: 'מלכים ב',        fileName: 'IIKings.json',      section: BibleSection.PROPHETS },
    { key: 'Isaiah',       hebrewName: 'ישעיהו',         fileName: 'Isaiah.json',       section: BibleSection.PROPHETS },
    { key: 'Jeremiah',     hebrewName: 'ירמיהו',         fileName: 'Jeremiah.json',     section: BibleSection.PROPHETS },
    { key: 'Ezekiel',      hebrewName: 'יחזקאל',         fileName: 'Ezekiel.json',      section: BibleSection.PROPHETS },
    { key: 'Hosea',        hebrewName: 'הושע',           fileName: 'Hosea.json',        section: BibleSection.PROPHETS },
    { key: 'Joel',         hebrewName: 'יואל',           fileName: 'Joel.json',         section: BibleSection.PROPHETS },
    { key: 'Amos',         hebrewName: 'עמוס',           fileName: 'Amos.json',         section: BibleSection.PROPHETS },
    { key: 'Obadiah',      hebrewName: 'עובדיה',         fileName: 'Obadiah.json',      section: BibleSection.PROPHETS },
    { key: 'Jonah',        hebrewName: 'יונה',           fileName: 'Jonah.json',        section: BibleSection.PROPHETS },
    { key: 'Micah',        hebrewName: 'מיכה',           fileName: 'Micah.json',        section: BibleSection.PROPHETS },
    { key: 'Nahum',        hebrewName: 'נחום',           fileName: 'Nahum.json',        section: BibleSection.PROPHETS },
    { key: 'Habakkuk',     hebrewName: 'חבקוק',          fileName: 'Habakkuk.json',     section: BibleSection.PROPHETS },
    { key: 'Zephaniah',    hebrewName: 'צפניה',          fileName: 'Zephaniah.json',    section: BibleSection.PROPHETS },
    { key: 'Haggai',       hebrewName: 'חגי',            fileName: 'Haggai.json',       section: BibleSection.PROPHETS },
    { key: 'Zechariah',    hebrewName: 'זכריה',          fileName: 'Zechariah.json',    section: BibleSection.PROPHETS },
    { key: 'Malachi',      hebrewName: 'מלאכי',          fileName: 'Malachi.json',      section: BibleSection.PROPHETS },
  
    { key: 'Psalms',       hebrewName: 'תהילים',         fileName: 'Psalms.json',       section: BibleSection.WRITINGS },
    { key: 'Proverbs',     hebrewName: 'משלי',           fileName: 'Proverbs.json',     section: BibleSection.WRITINGS },
    { key: 'Job',          hebrewName: 'איוב',           fileName: 'Job.json',          section: BibleSection.WRITINGS },
    { key: 'SongOfSongs',  hebrewName: 'שיר השירים',     fileName: 'SongOfSongs.json',  section: BibleSection.WRITINGS },
    { key: 'Ruth',         hebrewName: 'רות',            fileName: 'Ruth.json',         section: BibleSection.WRITINGS },
    { key: 'Lamentations', hebrewName: 'איכה',           fileName: 'Lamentations.json', section: BibleSection.WRITINGS },
    { key: 'Ecclesiastes', hebrewName: 'קהלת',           fileName: 'Ecclesiastes.json', section: BibleSection.WRITINGS },
    { key: 'Esther',       hebrewName: 'אסתר',           fileName: 'Esther.json',       section: BibleSection.WRITINGS },
    { key: 'Daniel',       hebrewName: 'דניאל',          fileName: 'Daniel.json',       section: BibleSection.WRITINGS },
    { key: 'Ezra',         hebrewName: 'עזרא',           fileName: 'Ezra.json',         section: BibleSection.WRITINGS },
    { key: 'Nehemiah',     hebrewName: 'נחמיה',          fileName: 'Nehemiah.json',     section: BibleSection.WRITINGS },
    { key: 'IChronicles',  hebrewName: 'דברי הימים א',   fileName: 'IChronicles.json',  section: BibleSection.WRITINGS },
    { key: 'IIChronicles', hebrewName: 'דברי הימים ב',   fileName: 'IIChronicles.json', section: BibleSection.WRITINGS }
  ];
  
const booksByKey = new Map(books.map(book => [book.key, book]));

function getByKey(key) {
    return booksByKey.get(key);
}

export { BibleSection, books, getByKey };