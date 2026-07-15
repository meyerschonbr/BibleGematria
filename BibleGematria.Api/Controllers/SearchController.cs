using BibleGematria.Core;
using BibleGematria.Core.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
    public class SearchRequest
    {
        public string HebrewInput { get; set; } = String.Empty;
        public List<string> BookKeys { get; set; } = new();
        public bool NoCrossEtnachta { get; set; }
    }
}
