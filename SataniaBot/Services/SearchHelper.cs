using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Google.Apis.Services;
using SataniaBot;
using SataniaBot.Services;
using System.Xml;

namespace Satania.Services
{

    public enum RequestHttpMethod
    {
        Get,
        Post
    }

    public static class SearchHelper
    {
        public enum WebsiteType
        {
            Safebooru,
            Gelbooru,
            Rule34,
            Konachan,
            Yandere
        }

        private static DateTime lastRefreshed = DateTime.MinValue;
        private static string token { get; set; } = "";
        private static readonly HttpClient httpClient = new HttpClient();
        private static Random rng = new Random();

        public static async Task<string> PictureSearch(string tag, WebsiteType type)
        {
            tag = tag?.Replace(" ", "_");
            var website = "";
            switch (type)
            {
                case WebsiteType.Safebooru:
                    website = $"https://safebooru.org/index.php?page=dapi&s=post&q=index&limit=100&tags={tag}";
                    break;
                case WebsiteType.Gelbooru:
                    website = $"http://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=100&tags={tag}";
                    break;
                case WebsiteType.Rule34:
                    website = $"https://rule34.xxx/index.php?page=dapi&s=post&q=index&limit=100&tags={tag}";
                    break;
                case WebsiteType.Konachan:
                    website = $"https://konachan.com/post.xml?s=post&q=index&limit=100&tags={tag}";
                    break;
                case WebsiteType.Yandere:
                    website = $"https://yande.re/post.xml?limit=100&tags={tag}";
                    break;
            }
            try
            {
                var toReturn = await Task.Run(async () =>
                {
                    using (var http = new HttpClient())
                    {
                        http.AddFakeHeaders();
                        var data = await http.GetStreamAsync(website).ConfigureAwait(false);
                        var doc = new XmlDocument();
                        doc.Load(data);

                        var node = doc.LastChild.ChildNodes[rng.Next(0, doc.LastChild.ChildNodes.Count)];

                        var url = node.Attributes["file_url"].Value;
                        if (!url.StartsWith("http"))
                            url = "https:" + url;
                        return url;
                    }
                }).ConfigureAwait(false);
                return toReturn;
            }
            catch
            {
                return null;
            }
        }

        public static Task<string> GetE621ImageLink(string tag) => Task.Run(async () =>
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.AddFakeHeaders();
                    var data = await http.GetStreamAsync("http://e621.net/post/index.xml?tags=" + tag).ConfigureAwait(false);
                    var doc = new XmlDocument();
                    doc.Load(data);
                    var nodes = doc.GetElementsByTagName("file_url");

                    var node = nodes[rng.Next(0, nodes.Count)];
                    return node.InnerText;
                }
            }
            catch
            {
                return null;
            }
        });

        public static void AddFakeHeaders(this HttpClient http)
        {
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1");
            http.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        }

        /*
        public static string GoogleSearch(string q)
        {
            
            string apiKey = Configuration.Load().GoogleApiCode;
            string cx = Configuration.Load().GoogleCustomSearch;
            string query = q;

            var svc =
                new Google.Apis.Customsearch.v1.CustomsearchService(new BaseClientService.Initializer
                {
                    ApiKey = apiKey
                });
            var listRequest = svc.Cse.List(query);

            listRequest.Cx = cx;
            var search = listRequest.Execute();

            var results = search.Items[0];

            return ("```\nTitle: " + results.Title + "\n" + "Link: " + results.Link + "\n```");
        }
        */
    }
}
