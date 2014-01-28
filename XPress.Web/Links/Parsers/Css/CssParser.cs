using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XPress.Web.Links.Parsers.Css
{
    /// <summary>
    /// Css parser based on http://blog.dynamicprogrammer.com/2008/01/20/CSSParserClassInNET.aspx
    /// </summary>
    public class CssParser
    {
        public CssParser()
        {
        }

        public static CssStyleSheet Parse(string css)
        {
            string[] parts = css.Split('}');
            return new CssStyleSheet(parts.Select(s =>
            {
                if (CleanUp(s).IndexOf('{') > -1)
                {
                    return ToStyleClass(s);
                }
                return null;
            }).Where(c => c != null).ToArray());
        }

        private static CssClass ToStyleClass(string s)
        {
            CssClass sc = new CssClass();
            string[] parts = s.Split('{');
            string styleName = CleanUp(parts[0]).Trim().ToLower();

            sc.Name = styleName;

            string[] atrs = CleanUp(parts[1]).Replace("}", "").Split(';');
            foreach (string a in atrs)
            {
                if (a.Contains(":"))
                {
                    string _key = a.Split(':')[0].Trim().ToLower();
                    string _val =  a.Split(':')[1].Trim().ToLower();
                    sc.Attributes.Add(new CssClassAttribute(_key, _val));
                }
            }
            return sc;
        }

        private static string CleanUp(string s)
        {
            string temp = s;
            string reg = "(/\\*(.|[\r\n])*?\\*/)|(//.*)";
            Regex r = new Regex(reg);
            temp = r.Replace(temp, "");
            temp = temp.Replace("\r", "").Replace("\n", "");
            return temp;
        }
    }
    
}
