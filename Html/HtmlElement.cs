using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Html
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement()
        {
            Classes = new List<string>();
            Attributes = new List<string>();
            Children = new List<HtmlElement>();

        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                HtmlElement element = queue.Dequeue();
                yield return element; 

                foreach (HtmlElement child in element.Children)
                {
                    queue.Enqueue(child); 
                }
            }
        }
      

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement element = this;
            while (element != null)
            {
                yield return element;
                element = element.Parent;
            }
        }

        public HashSet<HtmlElement> FindMatches(Selector selector)
        {
            HashSet<HtmlElement> HashElements = new HashSet<HtmlElement>();
            FindMatchesHelper(selector, this, HashElements);
            return HashElements;
        }
        private bool Match(HtmlElement element, Selector selector)
        {
           return
            (selector.TagName == null || element.Name == selector.TagName) &&
            (selector.Id == null || element.Id == selector.Id) &&
            (selector.Classes.Count() == 0 || (element.Classes != null && selector.Classes.Any(cls => element.Classes.Contains(cls))));
        }
        private void FindMatchesHelper(Selector selector, HtmlElement element, HashSet<HtmlElement> hashElements)
        {
            if (selector == null || element == null)
                return;

            
            foreach (HtmlElement child in element.Descendants())
            {
                if (child != null)
                {
                    if (Match(child, selector)&& child != element)
                    {
                        if (selector.Child == null)
                        {
                            hashElements.Add(child);
                        }
                        else
                        {
                            FindMatchesHelper(selector.Child, child, hashElements);
                        }
                    }
                }
            }
        }



        public override string ToString()
        {
            string element = "";
            element += this.Name != null ? $"Name ={this.Name} " : "";
            element += this.Id != null ? $"Id ={this.Id} " : "";
            element += this.Classes.Count > 0? $"Classes = {string.Join(", ", this.Classes.Select(a => a.ToString()))} " : "";
            element += this.Parent != null &&this.Parent.Name!=null? $"ParentName ={this.Parent.Name} " : "";

            return element;

        }
    }

}


