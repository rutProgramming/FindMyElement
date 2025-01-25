using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Html
{
    internal class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public static string[] HtmlWithTags { get;private set; }
        public static string[] HtmlWithOutTags { get; private set; }

        private HtmlHelper()
        {
            try
            {
                var contextHtmlWithTags = File.ReadAllText("JSON Files/HtmlTags.json");
                var contextHtmlWithOutTags = File.ReadAllText("JSON Files/HtmlVoidTags.json");
                HtmlWithTags = JsonSerializer.Deserialize<string[]>(contextHtmlWithTags);
                HtmlWithOutTags = JsonSerializer.Deserialize<string[]>(contextHtmlWithOutTags);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading HTML tags: {ex.Message}");
            }
        }
    }
}

