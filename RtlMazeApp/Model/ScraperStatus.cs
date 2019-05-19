namespace RtlMazeApp.Model
{
    public class ScraperStatus
    {
        internal ScraperStatus()
        {
        }

        public bool Running { get; internal set; }
        public int PagesProcessed { get; internal set; }
        public int ShowsProcessed { get; internal set; }
    }
}