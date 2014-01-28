using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.Links.Parsers.Css
{
    public static class CssPrefixCompleater
    {
        public static string CompletePrefixes(string css, CssPrefixReplaceOptions options = null)
        {
            CssStyleSheet sheet = CssParser.Parse(css);
            sheet.CompletePrefixes();
            return sheet.ToString();
        }

        public static void CompletePrefixes(this CssStyleSheet sheet, CssPrefixReplaceOptions options = null)
        {
            sheet.Classes.ForEach(c => CompletePrefixes(c));
        }

        public static void CompletePrefixes(this CssClass c, CssPrefixReplaceOptions options = null)
        {
            c.Attributes.ToArray().Where(kvp => PrefixRules.ContainsKey(kvp.Key)).ForEach(kvp =>
            {
                options.VendorPrefexs.ForEach(prefix =>
                {
                    CssClassAttribute atr = PrefixRules[kvp.Key](kvp, prefix, options);
                    if (atr != null)
                        c.Attributes.Add(atr);
                });
            });

            IEnumerable<CssClassAttribute> allAttr = c.Attributes.Distinct(atr => atr.Key + ":" + atr.Value).OrderBy(atr => atr.Key).ToArray();
            c.Attributes.Clear();
            c.Attributes.AddRange(allAttr);
        }

        public static Dictionary<string, Func<CssClassAttribute, string, CssPrefixReplaceOptions, CssClassAttribute>> PrefixRules =
            new Dictionary<string, Func<CssClassAttribute, string, CssPrefixReplaceOptions, CssClassAttribute>>(){
                {"border-radius", BaseReplacementRule},
  /*  {"border-top-left-radius", BorderRadiusReplacementRule},
    {"border-top-right-radius", BorderRadiusReplacementRule},
    {"border-bottom-right-radius", BorderRadiusReplacementRule},
    {"border-bottom-left-radius", BorderRadiusReplacementRule},
    {"border-image", FullReplacementRule},
    {"box-shadow", BaseReplacementRule},
    {"box-sizing", MozReplacementRule},
    {"box-orient", BaseAndIEReplacementRule},
    {"box-direction", BaseAndIEReplacementRule},
    {"box-ordinal-group", BaseAndIEReplacementRule},
    {"box-align", BaseAndIEReplacementRule},
    {"box-flex", BaseAndIEReplacementRule},
    {"box-flex-group", BaseReplacementRule},
    {"box-pack", BaseAndIEReplacementRule},
    {"box-lines", BaseAndIEReplacementRule},
    {"user-select", BaseReplacementRule},
    {"user-modify", BaseReplacementRule},
    {"margin-start", BaseReplacementRule},
    {"margin-end", BaseReplacementRule},
    {"padding-start", BaseReplacementRule},
    {"padding-end", BaseReplacementRule},
    {"column-count", BaseReplacementRule},
    {"column-gap", BaseReplacementRule},
    {"column-rule", BaseReplacementRule},
    {"column-rule-color", BaseReplacementRule},
    {"column-rule-style", BaseReplacementRule},
    {"column-rule-width", BaseReplacementRule},
    {"column-span", WebkitReplacementRule},
    {"column-width", BaseReplacementRule},
    {"columns", WebkitReplacementRule},

    {"background-clip", WebkitReplacementRule},
    {"background-origin", WebkitReplacementRule},
    {"background-size", WebkitReplacementRule},
    {"background-image", GradientReplacementRule},
    {"background", GradientReplacementRule},

    {"text-overflow", OperaAndIEReplacementRule},

    {"transition", TransitionReplacementRule},
    {"transition-delay", BaseAndOperaReplacementRule},
    {"transition-duration", BaseAndOperaReplacementRule},
    {"transition-property", TransitionReplacementRule},
    {"transition-timing-function", BaseAndOperaReplacementRule},
    {"transform", FullReplacementRule},
    {"transform-origin", FullReplacementRule},

    {"display", DisplayReplacementRule},
    {"opacity", OpacityReplacementRule},
    {"appearance", WebkitReplacementRule}*/
        };

        #region replacement rules..

        public static CssClassAttribute BaseReplacementRule(CssClassAttribute atr, string prefix, CssPrefixReplaceOptions options)
        {
            return new CssClassAttribute(prefix + atr.Key, atr.Value);
        }

        public static CssClassAttribute BorderRadiusReplacementRule(CssClassAttribute atr, string prefix, CssPrefixReplaceOptions options)
        {
            if (prefix == "-moz-") // applies only to the -moz- prefix.
            {
                string name = "-moz-" + atr.Key
                    .Replace("top-left-radius", "radius-topleft")
                    .Replace("top-right-radius", "radius-topright")
                    .Replace("bottom-right-radius", "radius-bottomright")
                    .Replace("bottom-left-radius", "radius-bottomleft");
                return new CssClassAttribute(name, atr.Value);
            }
            else return null;
        }

        public static CssClassAttribute DisplayReplacementRule(CssClassAttribute atr, string prefix, CssPrefixReplaceOptions options)
        {
            if (atr.Value.StartsWith("box") || atr.Value.StartsWith("inline-box"))
            {
                return new CssClassAttribute(atr.Key, prefix + atr.Value);
            }
            return null;
        }

        #endregion
    }
}
