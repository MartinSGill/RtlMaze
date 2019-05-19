namespace RtlMazeApp.Storage
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Model;

    internal interface IShowStore
    {
        int Count { get; }
        int LastId { get; }
        bool TryAdd(int id, SimpleShow show);
        IEnumerable<SimpleShow> GetPage(int page);
    }

    internal class ShowMemoryStore : IShowStore
    {
        private readonly ConcurrentDictionary<int, SimpleShow> _shows;

        public ShowMemoryStore()
        {
            _shows = new ConcurrentDictionary<int, SimpleShow>();
        }

        public int Count => _shows.Count;

        public bool TryAdd(int id, SimpleShow show)
        {
            return _shows.TryAdd(id, show);
        }

        public IEnumerable<SimpleShow> GetPage(int page)
        {
            var low = page * 250;
            var high = (page + 1) * 250;
            return _shows.Where(x =>
            {
                var (key, _) = x;
                return key >= low && key < high;
            }).Select(x => x.Value).ToList();
        }

        public int LastId => _shows.Keys.OrderBy(x => x).LastOrDefault();
    }
}