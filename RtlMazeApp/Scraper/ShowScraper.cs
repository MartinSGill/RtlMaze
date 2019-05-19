namespace RtlMazeApp.Scraper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using Newtonsoft.Json.Linq;
    using Storage;

    internal class ShowScraper
    {
        private const string BaseUrl = "http://api.tvmaze.com";
        private const string ShowIndexUrl = BaseUrl + "/shows?page={0}";
        private const string CastUrl = BaseUrl + "/shows/{0}/cast";

        // Prevent too many tasks from hitting the API at once
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(10, 10);
        private readonly IShowStore _showStore;

        private volatile int _pageCount;
        private volatile bool _running;

        private ShowScraper(IShowStore store)
        {
            _showStore = store;
        }

        public static ShowScraper Instance { get; private set; }

        public static void Initialize(IShowStore store)
        {
            Instance = new ShowScraper(store);
            Task.Run(() => Instance.Scrape());
        }

        public static ScraperStatus GetStatus()
        {
            return new ScraperStatus
            {
                PagesProcessed = Instance._pageCount,
                Running = Instance._running,
                ShowsProcessed = Instance._showStore.Count
            };
        }

        private async Task ProcessIndexPage(HttpResponseMessage indexPage)
        {
            try
            {
                var body = await indexPage.Content.ReadAsStringAsync();
                _pageCount++;
                var json = JArray.Parse(body);
                var tasks = json.OfType<JObject>().Select(show => Task.Run(() => ProcessShow(show))).ToList();
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task ProcessShow(JObject show)
        {
            var id = show["id"].Value<int>();
            var retry = true;
            Semaphore.Wait();
            while (retry)
                try
                {
                    using (var client = new HttpClient())
                    using (var response = await client.GetAsync(string.Format(CastUrl, id)))
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.TooManyRequests:
                                // Rate Limit hit
                                Console.Write("!");
                                await Task.Delay(5000);
                                break;
                            case HttpStatusCode.OK:
                                retry = false;
                                var body = await response.Content.ReadAsStringAsync();
                                var cast = JArray.Parse(body);
                                _showStore.TryAdd(id,
                                    new SimpleShow
                                    {
                                        Id = id,
                                        Name = show["name"].Value<string>(),
                                        Cast = cast.Select(x => new SimpleCast
                                        {
                                            Id = x["person"]["id"].Value<int>(),
                                            Name = x["person"]["name"].Value<string>(),
                                            Birthday = x["person"]["birthday"].Value<DateTime?>()
                                        }).OrderBy(x => x.Birthday).ToList()
                                    });
                                break;
                            default:
                                // unexpected error, abort
                                throw new Exception($"Unexpected Error. Code = {response.StatusCode}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            Console.Write("+");
            Semaphore.Release();
        }

        public async void Scrape()
        {
            _pageCount = 0;
            _running = true;
            var moreData = true;
            uint page = 0;
            var tasks = new List<Task>();

            Console.WriteLine("Beginning Scrape");

            while (moreData)
                using (var client = new HttpClient())
                {
                    Semaphore.Wait();
                    var response = client.GetAsync(string.Format(ShowIndexUrl, page)).Result;
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.TooManyRequests:
                            // Rate Limit hit
                            // Simplistic, maybe a custom scheduler?
                            Console.Write("W");
                            Thread.Sleep(5000);
                            break;
                        case HttpStatusCode.NotFound:
                            moreData = false;
                            Console.WriteLine("");
                            Console.WriteLine($"Index Scrape Complete. {page} pages read.");
                            break;
                        case HttpStatusCode.OK:
                            tasks.Add(Task.Run(() => ProcessIndexPage(response)));
                            Console.Write(".");
                            page++;
                            break;
                        default:
                            // unexpected error, abort
                            throw new Exception($"Unexpected Error. Code = {response.StatusCode}");
                    }

                    Semaphore.Release();
                }

            await Task.WhenAll(tasks.ToArray());
            _running = false;
            Console.WriteLine($"Shows Read: {_showStore.Count}");
            Console.WriteLine("Scrape Complete");
        }

        public IEnumerable<SimpleShow> GetPage(int page)
        {
            return _showStore.GetPage(page);
        }
    }
}