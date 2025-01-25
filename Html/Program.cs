
using Html;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;


async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

static HtmlElement BuildTree(List<string> html)
{

    HtmlElement root = new HtmlElement();
    root.Name = "html";
    HtmlElement currentTags = root;
    bool withTag = false;
    bool isElement = false;
    bool isInnerHtml = true;
    for (int i = 1; i < html.Count(); i++)
    {
        withTag = false;
        isElement = false;
        isInnerHtml = true;
        string line = html[i];
        string tag = line.Contains(' ') ? line.Trim().Substring(0, line.IndexOf(' ') + 1) : line;
        if (tag == "/html")
        {
            return root.Children.FirstOrDefault();
        }
        else if (tag.StartsWith('/'))
        {
            if (currentTags.Parent != null)
                currentTags = currentTags.Parent;
            isInnerHtml = false;
        }
        else if (HtmlHelper.HtmlWithOutTags.Contains(tag.Trim()))
        {
            isElement = true;
        }
        else if (HtmlHelper.HtmlWithTags.Contains(tag.Trim()))
        {
            withTag = true;
            isElement = true;
        }
        
        if(isElement) 
        {
            HtmlElement element = new HtmlElement();
            element.Name = tag.Trim();
            element.Parent = currentTags;
            currentTags.Children.Add(element);
            List<string> attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(html[i]).Cast<Match>().Select(match => match.Value).ToList(); ;
            foreach (string item in attributes)
            {
                string[] s = item.Split("=");
                string attribute = s[0].Trim();
                string value = s[1].Trim('"', '\'');
                switch (attribute)
                {
                    case "id":
                        element.Id = value;
                        element.Attributes.Add("id");
                        break;
                    case "class":
                        element.Classes = value.Split(" ").ToList();
                        element.Attributes.Add("class");
                        break;
                    default:
                        element.Attributes.Add(item);
                        break;
                }
            }
            if (withTag)
            {
                currentTags = element;
            } 
        }
        else if (isInnerHtml)
        {
            currentTags.InnerHtml += " " + tag;
        }


    }
    return root.Children.FirstOrDefault();
}
static List<string> ExtractHtmlLines(string html )
{
    var cleanHtml = new Regex("\\s").Replace(html, " ");
    var lines = Regex.Matches(cleanHtml, @"<\/?([A-Za-z][A-Za-z0-9]*)\b[^>]*>|([^<]+)");
    List<string> htmlLines = new List<string>();
    foreach (var line in lines)
    {
        string l = line.ToString();
        if (!string.IsNullOrWhiteSpace(l))
        {
            l = l.Replace('<', ' ');
            l = l.Replace('>', ' ');
            l = l.Trim();
            htmlLines.Add(l);
        }
    }
    return htmlLines;
}
//-----------main------------------
Console.WriteLine("enter URL");
string URL = Console.ReadLine();
string html = await Load(URL);
List<string> htmlLines= ExtractHtmlLines(html);
HtmlElement root= BuildTree(htmlLines);
//------------------------
Console.WriteLine("enter selector");
string sel=Console.ReadLine();
Selector selector = Selector.DecodingAQuery(sel);
HashSet<HtmlElement> HashElements = root.FindMatches(selector);
foreach (var element in HashElements)
{
    Console.WriteLine(element);
}