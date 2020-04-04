//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;

namespace MaterialUI
{
    public class VectorImageParserFontAwesomeSolid : VectorImageParserFontAwesome
    {
        protected override string GetIconFontUrl()
        {
            return "https://github.com/FortAwesome/Font-Awesome/blob/master/web-fonts-with-css/webfonts/fa-solid-900.ttf?raw=true";
        }

        public override string GetFontName()
        {
            return "FontAwesomeSolid";
        }

        protected override string styleToMatch
        {
            get { return "solid"; }
        }
    }
}
