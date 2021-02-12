using AngleSharp;
using AngleSharp.Html.Dom;
using Api.Interfaces;
using Runner.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Runner
{
    public class Runner : IScraper
    {
        //private static readonly string _url = "https://www.marktplaats.nl/l/watersport-en-boten/surfen-golfsurfen/f/fish/8354/#distanceMeters:25000|postcode:2563GP|searchInTitleAndDescription:true";
        //private static readonly string _fileName = _url.GetStableHashCode().ToString();
        private static readonly string _path = Path.Combine(Directory.GetCurrentDirectory(), "Imports");
        
        public static async Task RunAsync(string queryUrl)
        {
            SetDirectory();
            var data = await GetDataAsync(queryUrl);
            var items = await GetNewItemsAsync(data);
            var cleanedItems = CompareWithLastImport(items);
            EmailClient.Send(cleanedItems);
        }

        private static void SetDirectory()
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            Directory.SetCurrentDirectory(_path);
        }

        private static List<Advertisement> CompareWithLastImport(List<Advertisement> items)
        {
            var itemHashDictionary = new Dictionary<int, Advertisement>();
            foreach (var item in items)
            {
                itemHashDictionary.Add(item.GetHashCode(), item);
            }

            var currentImport = itemHashDictionary.Keys;
            var lastImport = GetLastImportFromDisk();
            var newItemsHashes = itemHashDictionary.Keys.Except(lastImport);

            var newItems = itemHashDictionary.Where(x => newItemsHashes.Contains(x.Key)).Select(x => x.Value).ToList();
            SaveNewItemsToDisk(newItems);

            return newItems;
        }

        private static void SaveNewItemsToDisk(List<Advertisement> newItems)
        {
            if (newItems.Count == 0)
            {
                return;
            }

            File.AppendAllLines(_fileName, newItems.Select(x => x.GetHashCode().ToString()));
        }

        private static List<int> GetLastImportFromDisk()
        {
            var lastImport = new List<int>();
            
            if (!File.Exists(_fileName))
            {
                return lastImport;
            }

            var lines = File.ReadLines(_fileName);
            foreach (var line in lines)
            {
                lastImport.Add(int.Parse(line));
            }
            return lastImport;
        }

        private static async Task<List<Advertisement>> GetNewItemsAsync(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var document = await GetDomDocumentAsync(data);
            return GetItems(document);
        }

        private static List<Advertisement> GetItems(AngleSharp.Dom.IDocument document)
        {
            try
            {
                var listings = document.QuerySelector("ul.mp-Listings").QuerySelectorAll("li");

                var result = new List<Advertisement>();
                foreach (var item in listings)
                {

                    var anchor = item.QuerySelector("a") as IHtmlAnchorElement;
                    var content = item.QuerySelector("div.mp-Listing-group").QuerySelector("div.mp-Listing-group--title-description-attributes");
                    var title = content.GetElementsByClassName("mp-Listing-title").First();
                    var description = content.GetElementsByClassName("mp-Listing-description").First();

                    var attributes = item.QuerySelector("div.mp-Listing-group--price-date-feature");
                    var price = attributes.GetElementsByClassName("mp-Listing-price").First();

                    var img = anchor.QuerySelector("img") as IHtmlImageElement;

                    result.Add(
                        new Advertisement
                        {
                            Title = title?.TextContent,
                            Description = description?.TextContent,
                            Price = price?.TextContent,
                            Url = anchor?.Href,
                            ImgSource = img?.Source
                        });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ScrapeException("Something went wrong while scraping the page", _url ,ex);
            }
        }

        private static async Task<AngleSharp.Dom.IDocument> GetDomDocumentAsync(string data)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(x => x.Content(data));
            return document;
        }

        private static async Task<string> GetDataAsync(string queryUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(queryUrl);
            using var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"Er is iets mis gegaan {response.StatusCode}");
            }

            using var stream = response.GetResponseStream();

            var readStream = string.IsNullOrWhiteSpace(response.CharacterSet)
                ? new StreamReader(stream)
                : new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet));

            return await readStream.ReadToEndAsync();

        }
    }
}
