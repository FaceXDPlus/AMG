//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Text.RegularExpressions;

namespace MaterialUI
{
    public abstract class VectorImageParserFontAwesome : VectorImageFontParser
    {
        protected override string GetIconFontLicenseUrl()
        {
            return "https://github.com/FortAwesome/Font-Awesome/blob/master/README.md?raw=true";
        }

        protected override string GetIconFontDataUrl()
        {
            return "https://raw.githubusercontent.com/FortAwesome/Font-Awesome/master/advanced-options/metadata/icons.json";
        }

        public override string GetWebsite()
        {
            return "https://fontawesome.com/";
        }

        protected abstract string styleToMatch { get; }

        protected override VectorImageSet GenerateIconSet(string fontDataContent)
        {
            VectorImageSet vectorImageSet = new VectorImageSet();

            Regex getIconData = new Regex("\".*?\":.*?(}.*?}.*?},)", RegexOptions.Singleline);
            Regex getStyles = new Regex("(?<=\"styles\":\\s\\[).*?(?=\\])", RegexOptions.Singleline);
            Regex getUnicode = new Regex("(?<=\"unicode\":\\s\").*?(?=\")", RegexOptions.Singleline);
            Regex getLabel = new Regex("(?<=\"label\":\\s\").*?(?=\")", RegexOptions.Singleline);

            foreach (Match iconData in getIconData.Matches(fontDataContent))
            {
                bool hasStyle = false;

                foreach (Match style in getStyles.Matches(iconData.Value))
                {
                    if (style.Value.Contains(styleToMatch))
                    {
                        hasStyle = true;
                        break;
                    }
                }

                if (hasStyle)
                {
                    vectorImageSet.iconGlyphList.Add(new Glyph()
                    {
                        name = getLabel.Match(iconData.Value).Value,
                        unicode = getUnicode.Match(iconData.Value).Value
                    });
                }
            }

            return vectorImageSet;
        }

        protected override string ExtractLicense(string fontDataLicenseContent)
        {
            return fontDataLicenseContent;
        }
    }
}
