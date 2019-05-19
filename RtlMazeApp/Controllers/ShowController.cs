namespace RtlMazeApp.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Model;
    using Scraper;

    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        // GET api/values/5
        [HttpGet("{page}")]
        public ActionResult<IEnumerable<SimpleShow>> Get(int page)
        {
            return ShowScraper.Instance.GetPage(page).ToList();
        }
    }
}