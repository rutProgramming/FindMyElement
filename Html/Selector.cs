using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Html
{
    internal class Selector
    {
        public string Id { get; set; }
        public string TagName { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }
        public Selector()
        {
            Classes = new List<string>();
        }

        public static Selector DecodingAQuery(string query)
        {
            Selector root = new Selector();
            Selector current = root;
            List<string> list = query.Split(' ').ToList();
            bool isValid = false;
            foreach (var item in list)
            {
                Selector selector = new Selector();
                List<string> parts = Regex.Matches(item, @"[^\s.#]+|[.#][^\s.#]+").Cast<Match>().Select(m => m.Value).ToList();

                foreach (var part in parts)
                {
                    if (!string.IsNullOrEmpty(part))
                    {
                        //Selector selector = new Selector();
                        char key = part[0];
                        string value = part.Substring(1);
                        isValid = false;
                        switch (key)
                        {
                            case '#':
                                selector.Id = value;
                                isValid = true;
                                break;
                            case '.':
                                selector.Classes.Add(value);
                                isValid = true;
                                break;

                            default:
                                if (HtmlHelper.HtmlWithOutTags.Contains(part) || HtmlHelper.HtmlWithTags.Contains(part))
                                {
                                    selector.TagName = part;
                                    isValid = true;
                                }
                                break;
                        }
                        if (isValid)
                        {
                            current.Child = selector;
                            selector.Parent = current;
                            current = selector;
                        }
                    }
                }
            }
            return root.Child;
        }

    }
}
