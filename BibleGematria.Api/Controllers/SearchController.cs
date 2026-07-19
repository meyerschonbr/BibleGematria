using BibleGematria.Core;
using BibleGematria.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
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
        public ActionResult ExportCsv([FromBody] SearchRequest request)
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

            string csv = CsvExporter.Export(results, request.HebrewInput, target);
            byte[] bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
            return File(bytes, "text/csv", "gematria-results.csv");

        }
    }

    public class SearchRequest
    {
        public string HebrewInput { get; set; } = String.Empty;
        public List<string> BookKeys { get; set; } = new();
        public bool NoCrossEtnachta { get; set; }
    }
}
