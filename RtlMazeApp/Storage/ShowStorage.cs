namespace RtlMazeApp.Storage
{
    internal class ShowStorage
    {
        static ShowStorage()
        {
            Store = new ShowMemoryStore();
        }

        public static IShowStore Store { get; }
    }
}