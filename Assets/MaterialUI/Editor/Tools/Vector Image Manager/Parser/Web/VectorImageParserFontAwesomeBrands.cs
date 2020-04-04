//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

namespace MaterialUI
{
    public class VectorImageParserFontAwesomeBrands : VectorImageParserFontAwesome
    {
        protected override string GetIconFontUrl()
        {
            return "https://github.com/FortAwesome/Font-Awesome/blob/master/web-fonts-with-css/webfonts/fa-brands-400.ttf?raw=true";
        }

        public override string GetFontName()
        {
            return "FontAwesomeBrands";
        }

        protected override string styleToMatch
        {
            get { return "brands"; }
        }
    }
}
