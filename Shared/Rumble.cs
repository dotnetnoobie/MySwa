namespace BlazorApp.Shared
{
    public enum RumbleType
    {
        User,
        Channel
    }

    public class Channel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        // public RumbleType Type { get; set; }
        public string Type { get; set; }
        public IEnumerable<Item> Items { get; set; } = Enumerable.Empty<Item>();
    }

    public class Item
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string img { get; set; }
        public DateTimeOffset PubDate { get; set; }
    }
}
