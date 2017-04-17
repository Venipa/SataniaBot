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

namespace Satania.Services
{

    public enum RequestHttpMethod
    {
        Get,
        Post
    }

    public static class SearchHelper
    {
        private static DateTime lastRefreshed = DateTime.MinValue;
        private static string token { get; set; } = "";
        private static readonly HttpClient httpClient = new HttpClient();
        private static Random rng = new Random();


        public static async Task<Stream> GetResponseStreamAsync(string url,
            IEnumerable<KeyValuePair<string, string>> headers = null, RequestHttpMethod method = RequestHttpMethod.Get)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));
            //if its a post or there are no headers, use static httpclient
            // if there are headers and it's get, it's not threadsafe
            var cl = headers == null || method == RequestHttpMethod.Post ? httpClient : new HttpClient();
            cl.DefaultRequestHeaders.Clear();
            switch (method)
            {
                case RequestHttpMethod.Get:
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            cl.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }
                    return await cl.GetStreamAsync(url).ConfigureAwait(false);
                case RequestHttpMethod.Post:
                    FormUrlEncodedContent formContent = null;
                    if (headers != null)
                    {
                        formContent = new FormUrlEncodedContent(headers);
                    }
                    var message = await cl.PostAsync(url, formContent).ConfigureAwait(false);
                    return await message.Content.ReadAsStreamAsync().ConfigureAwait(false);
                default:
                    throw new NotImplementedException("That type of request is unsupported.");
            }
        }

        public static async Task<string> GetResponseStringAsync(string url,
            IEnumerable<KeyValuePair<string, string>> headers = null,
            RequestHttpMethod method = RequestHttpMethod.Get)
        {

            using (var streamReader = new StreamReader(await GetResponseStreamAsync(url, headers, method).ConfigureAwait(false)))
            {
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        public static async Task<string> GetDanbooruImageLink(string tag)
        {

            if (tag == "loli") //loli doesn't work for some reason atm
                tag = "flat_chest";

            var link = $"http://danbooru.donmai.us/posts?" +
                        $"page={rng.Next(0, 15)}";
            if (!string.IsNullOrWhiteSpace(tag))
                link += $"&tags={tag.Replace(" ", "_")}";

            var webpage = await GetResponseStringAsync(link).ConfigureAwait(false);
            var matches = Regex.Matches(webpage, "data-large-file-url=\"(?<id>.*?)\"");

            if (matches.Count == 0)
                return null;
            return $"http://danbooru.donmai.us" +
                   $"{matches[rng.Next(0, matches.Count)].Groups["id"].Value}";
        }

        public static async Task<string> GetGelbooruImageLink(string tag)
        {
            var headers = new Dictionary<string, string>() {
                {"User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1"},
                {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" },
            };
            var url =
                $"http://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=100&tags={tag.Replace(" ", "_")}";
            var webpage = await GetResponseStringAsync(url, headers).ConfigureAwait(false);
            var matches = Regex.Matches(webpage, "file_url=\"(?<url>.*?)\"");
            if (matches.Count == 0)
                return null;
            var match = matches[rng.Next(0, matches.Count)];
            return matches[rng.Next(0, matches.Count)].Groups["url"].Value;
        }

        public static async Task<string> GetSafebooruImageLink(string tag)
        {
            var url =
            $"http://safebooru.org/index.php?page=dapi&s=post&q=index&limit=100&tags={tag.Replace(" ", "_")}";
            var webpage = await GetResponseStringAsync(url).ConfigureAwait(false);
            var matches = Regex.Matches(webpage, "file_url=\"(?<url>.*?)\"");
            if (matches.Count == 0)
                return null;
            var match = matches[rng.Next(0, matches.Count)];
            return matches[rng.Next(0, matches.Count)].Groups["url"].Value;
        }

        public static async Task<string> GetRule34ImageLink(string tag)
        {
            var url =
            $"http://rule34.xxx/index.php?page=dapi&s=post&q=index&limit=100&tags={tag.Replace(" ", "_")}";
            var webpage = await GetResponseStringAsync(url).ConfigureAwait(false);
            var matches = Regex.Matches(webpage, "file_url=\"(?<url>.*?)\"");
            if (matches.Count == 0)
                return null;
            var match = matches[rng.Next(0, matches.Count)];
            return "http:" + matches[rng.Next(0, matches.Count)].Groups["url"].Value;
        }


        internal static async Task<string> GetE621ImageLink(string tags)
        {
            try
            {
                var headers = new Dictionary<string, string>() {
                    {"User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1"},
                    {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" },
                };
                var data = await GetResponseStreamAsync(
                    "http://e621.net/post/index.xml?tags=" + Uri.EscapeUriString(tags) + "%20order:random&limit=1",
                    headers);
                var doc = XDocument.Load(data);
                return doc.Descendants("file_url").FirstOrDefault().Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in e621 search: \n" + ex);
                return "Error, do you have too many tags?";
            }
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
