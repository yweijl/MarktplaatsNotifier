using Shared;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Advertisement : EntityBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        private string _url;
        public string Url { get => _url; set { _url = value.Replace("localhost", "marktplaats.nl").Replace("http", "https"); } }

        public string ImageUrl { get; set; }
        public long QueryId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb
                .AppendLine("<br>")
                .AppendLine("<strong>Item</strong><br>")
                .AppendLine(@$"<img src=""{ImageUrl}""></img><br>")
                .AppendLine("<br>")
                .AppendLine($"Titel: {Title}<br>")
                .AppendLine($"Omschrijving {Description}<br>")
                .AppendLine($"Prijs {Price}<br>")
                .AppendLine($"Link {Url}<br>")
                .AppendLine("<br>");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var hash = Title.GetStableHashCode();
            hash = hash * 23 + Description.GetStableHashCode();
            hash = hash * 23 + Price.GetStableHashCode();
            hash = hash * 23 + ImageUrl.GetStableHashCode();
            hash = hash * 23 + Url.GetStableHashCode();
            return hash;
        }
    }

    public static class AdvertisementExtensions
    {
        public static string CreateBody(this List<Advertisement> Items)
        {
            var sb = new StringBuilder();
            Items.ForEach(x => sb.AppendLine(x.ToString()));
            return sb.ToString();
        }
    }
}
