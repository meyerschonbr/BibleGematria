# BibleGematria

Search the Hebrew Bible (Tanach) for words and phrases whose gematria value matches a number you enter. Type a Hebrew word or phrase, choose which books to search, and get back every matching verse with the match highlighted — plus an Excel export of the results.

Gematria assigns each Hebrew letter a numeric value (א=1, ב=2, ... ת=400) and sums the letters in a word or phrase. This project searches all 39 books of the Tanach for single words and multi-word phrases whose total matches a target value.

This repo contains the same search engine built twice, on purpose: once as a .NET desktop/web app, and once as a plain, dependency-free JavaScript static site. See [Why two implementations?](#why-two-implementations) below.

## Live demo

The static JS version is deployed via GitHub Pages: **https://meyerschonbr.github.io/BibleGematria/**

## Projects

| Project | What it is |
|---|---|
| `BibleGematria.Core` | Shared .NET class library — gematria calculator, Sefaria JSON loader, phrase search, book catalog, CSV export |
| `BibleGematria.Wpf` | Windows desktop app (WPF), publishes as a self-contained `.exe` |
| `BibleGematria.Api` | ASP.NET Core Web API + static frontend host — serves the search API and the `wwwroot` web UI from one process |
| `BibleGematria.Web` | Source copy of the web frontend (HTML/CSS/JS) that gets deployed into `BibleGematria.Api/wwwroot` |
| `BibleGematria.Tests` | xUnit tests for `BibleGematria.Core` |
| `BibleGematria.JS` | Fully independent, strictly-JavaScript static rewrite — no backend, no build step, runs entirely in the browser |
| `Data/` | 39 Sefaria-format JSON files (the full Hebrew Bible text), shared by the Wpf and Api projects |

## Running the .NET version

**Desktop app:**
```powershell
dotnet run --project BibleGematria.Wpf
```

**Web app (API + frontend together):**
```powershell
dotnet run --project BibleGematria.Api
```
Then open the URL it prints (e.g. `http://localhost:5000`) in a browser. The API serves both `POST /api/search` (find matches) and `POST /api/search/export` (download an `.xlsx` of the results) alongside the static frontend from `wwwroot/`.

**Tests:**
```powershell
dotnet test
```

## Running the JS version

`BibleGematria.JS/` is a fully static site — no server, no build step, no `npm install`. Because it uses native ES module `import`/`export`, it must be served over `http://` rather than opened directly as a `file://` path (browsers block module loading from `file://`). Any static file server works, for example:

```powershell
cd BibleGematria.JS
python -m http.server 8000
```
then open `http://localhost:8000`. VS Code's Live Server extension works too — right-click `index.html` → "Open with Live Server".

## Why two implementations?

The .NET version came first, built to learn ASP.NET Core, dependency injection, and web API design. Once it was working end-to-end, the entire search engine was deliberately rewritten from scratch in plain JavaScript — as a hands-on exercise to learn JavaScript itself, not because the app needed it. Every module in `BibleGematria.JS/` mirrors a specific C# file one-to-one:

| C# (`BibleGematria.Core`) | JavaScript (`BibleGematria.JS`) |
|---|---|
| `GematriaCalculator.cs` | `gematria.js` |
| `BibleBook.cs` (`BibleBookCatalog`) | `books.js` |
| `TanachLoader.cs` | `loader.js` |
| `SearchService.cs` | `search.js` |
| `XlsxExporter.cs` | `xlsx.js` |

The JS version has no server at all — it fetches the Tanach JSON files directly in the browser, runs the same gematria math and phrase-matching logic client-side, and builds the `.xlsx` export in-browser using [SheetJS](https://sheetjs.com/) (vendored locally in `xlsx.full.min.js`). That's what makes it deployable as a plain static site (GitHub Pages, Netlify, or any static host) with no backend to run or pay for.

## Data source

Bible text is in [Sefaria](https://www.sefaria.org/)'s JSON export format (one file per book: `title`, `heTitle`, and a nested `text` array of chapters → verses).

## Known limitations

- Gematria here is standard/Mispar Hechrachi — final-form letters (ך ם ן ף ץ) use the same value as their regular form, not the larger "Mispar Gadol" alternative values.
- Phrase search has a configurable max phrase length (default 15–20 words) to bound the search space per verse.
- The "don't cross etnachta" search mode restricts phrase matches to not span a verse's etnachta (a cantillation mark indicating a major clause break), for users who want phrases confined to one half of a verse.
