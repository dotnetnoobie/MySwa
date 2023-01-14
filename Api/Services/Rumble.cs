using AngleSharp.Dom;
using BlazorApp.Shared;
using System.Net;
using System.Xml.Linq;

namespace Api.Services
{
    public interface IRumble
    {
        IEnumerable<XElement> CreateElements(IEnumerable<Item> items);
        Task<Channel> Get(string channel);
        //Task<Entries> GetHtml(string channel);
    }

    public class Rumble : IRumble
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Rumble(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Channel> Get(string query)
        {
            var entries = await GetHtml(query);

            var items = new List<Item>();

            foreach (var entry in entries.items)
            {
                var item = new Item();

                item.Title = WebUtility.HtmlDecode(entry.GetElementsByClassName("video-item--title")?.FirstOrDefault()?.Html() ?? string.Empty);
                item.Url = WebUtility.UrlDecode("https://rumble.com/" + entry.GetElementsByClassName("video-item--a")?.FirstOrDefault()?.GetAttribute("href"));
                item.img = WebUtility.UrlDecode(entry.GetElementsByClassName("video-item--img")?.FirstOrDefault()?.GetAttribute("src") ?? string.Empty);

                var dt = entry.GetElementsByClassName("video-item--meta video-item--time")?.FirstOrDefault()?.GetAttribute("datetime");

                item.PubDate = DateTimeOffset.Parse(dt);

                items.Add(item);
            }

            var channel = new Channel();
            channel.Title = query;
            channel.Link = query;
            channel.Items = items;
            channel.Type = entries.type;

            return channel;
        }

        public async Task<Entries> GetHtml(string channel)
        {
            var type = RumbleType.Channel;

            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync($"https://rumble.com/c/{channel}");

            if (response.StatusCode == System.Net.HttpStatusCode.Gone || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                type = RumbleType.User;
                response = await client.GetAsync($"https://rumble.com/user/{channel}");
            }

            var html = await response.Content.ReadAsStringAsync();

            var parser = new AngleSharp.Html.Parser.HtmlParser();
            var doc = parser.ParseDocument(html);

            var entries = doc.GetElementsByClassName("video-listing-entry");

            return new Entries(type, entries);
        }

        public IEnumerable<XElement> CreateElements(IEnumerable<Item> items)
        {
            List<XElement> list = new List<XElement>();

            foreach (var item in items)
            {

                XElement itemElement = new XElement("item",
                                         new XElement("id", item.Url),
                                         new XElement("title", item.Title),
                                         new XElement("link", item.Url),
                                         new XElement("image", item.img),
                                         new XElement("pubDate", item.PubDate)
                );

                list.Add(itemElement);
            }
            return list;
        }

        public record Entries(RumbleType type, IHtmlCollection<IElement> items);

    }

}
