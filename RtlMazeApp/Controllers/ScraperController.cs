namespace RtlMazeApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Model;
    using Scraper;

    [Route("api/[controller]")]
    [ApiController]
    public class ScraperController : ControllerBase
    {
        // GET api/scraper
        [HttpGet]
        public ActionResult<ScraperStatus> Get()
        {
            return ShowScraper.GetStatus();
        }
    }
}