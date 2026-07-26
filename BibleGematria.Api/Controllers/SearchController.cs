using BibleGematria.Core;
using BibleGematria.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BibleGematria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly TanachRepository _repository;

        public SearchController(TanachRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public ActionResult<List<MatchResult>> Search([FromBody] SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.HebrewInput))
            {
                return BadRequest("Hebrew input is required.");
            }
            if (request.BookKeys.Count == 0)
            {
                return BadRequest("At least one book must be selected.");
            }
            var verses = _repository.GetBooks(request.BookKeys);

            int target = GematriaCalculator.Compute(request.HebrewInput);

            var service = new SearchService(verses) { MaxPhraseLength = 15 };

            var results = request.NoCrossEtnachta ? service.FindPhraseMatchesNoBoundary(target) : service.FindPhraseMatches(target);

            return Ok(results);
        }
        [HttpPost("export")]
        public ActionResult ExportXlsx([FromBody] SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.HebrewInput))
            {
                return BadRequest("Hebrew input is required.");
            }
            if (request.BookKeys.Count == 0)
            {
                return BadRequest("At least one book must be selected.");
            }
            var verses = _repository.GetBooks(request.BookKeys);

            int target = GematriaCalculator.Compute(request.HebrewInput);

            var service = new SearchService(verses) { MaxPhraseLength = 15 };

            var results = request.NoCrossEtnachta ? service.FindPhraseMatchesNoBoundary(target) : service.FindPhraseMatches(target);

            byte[] bytes = XlsxExporter.Export(results, request.HebrewInput, target);
            string fileName = BuildExportFileName(request.HebrewInput);

            // Hebrew filenames need percent-encoding — HTTP headers are ASCII-only,
            // and the plain filename= fallback can't carry non-ASCII text at all.
            string encodedFileName = Uri.EscapeDataString(fileName);
            Response.Headers["Content-Disposition"] = $"attachment; filename*=UTF-8''{encodedFileName}";

            return new FileContentResult(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        private static string BuildExportFileName(string hebrewInput)
        {
            string[] invalidChars = System.IO.Path.GetInvalidFileNameChars().Select(c => c.ToString()).ToArray();

            string[] firstThreeWords = hebrewInput
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(3)
                .Select(word => invalidChars.Aggregate(word, (current, ch) => current.Replace(ch, "")))
                .ToArray();

            string namePart = string.Join("-", firstThreeWords);

            return string.IsNullOrEmpty(namePart)
                ? "gematria-results.xlsx"
                : $"gematria-results-{namePart}.xlsx";
        }
    }

    public class SearchRequest
    {
        public string HebrewInput { get; set; } = String.Empty;
        public List<string> BookKeys { get; set; } = new();
        public bool NoCrossEtnachta { get; set; }
    }
}
