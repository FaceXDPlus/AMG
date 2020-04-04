//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace MaterialUI
{
    public class VectorImageParserIcoMoon : VectorImageFontParser
    {
        protected override string GetIconFontUrl()
        {
            return "https://github.com/Keyamoon/IcoMoon-Free/blob/master/Font/IcoMoon-Free.ttf?raw=true";
        }

        protected override string GetIconFontLicenseUrl()
        {
            return "https://github.com/Keyamoon/IcoMoon-Free/blob/master/License.txt?raw=true";
        }

        protected override string GetIconFontDataUrl()
        {
            return "https://github.com/Keyamoon/IcoMoon-Free/raw/master/Font/selection.json?raw=true";
        }

        public override string GetWebsite()
        {
            return "https://icomoon.io/#preview-free";
        }

        public override string GetFontName()
        {
            return "IcoMoon";
        }

        protected override VectorImageSet GenerateIconSet(string fontDataContent)
        {
            return GenerateSpecificIconSet(fontDataContent);
        }

        public static VectorImageSet GenerateSpecificIconSet(string fontDataContent)
        {
            VectorImageSet vectorImageSet = new VectorImageSet();

            string[] sections = fontDataContent.Split(new[] { "iconIdx" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < sections.Length; i++)
            {
                string[] linesInSection = sections[i].Replace("\t", "").Replace(" ", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (linesInSection.Length == 0) continue;

                Glyph currentGlyph = new Glyph();
                bool glyphHasName = false;
                bool glyphHasCode = false;

                for (int j = 0; j < linesInSection.Length; j++)
                {
                    if (linesInSection[j].StartsWith("\"code\""))
                    {
                        string[] sectionsInLineInSection = linesInSection[j].Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                        if (sectionsInLineInSection.Length < 2) continue;

                        currentGlyph.unicode = sectionsInLineInSection[1].Replace(",", "");

                        if (string.IsNullOrEmpty(currentGlyph.unicode)) continue;
                        
                        currentGlyph.unicode = int.Parse(currentGlyph.unicode).ToString("X");
                        glyphHasCode = true;
                    }
                    if (linesInSection[j].StartsWith("\"name\""))
                    {
                        currentGlyph.name = linesInSection[j].Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("\"", "");
                        if(currentGlyph.name.StartsWith("uni")) continue;
                        glyphHasName = true;
                    }

                    if (glyphHasName && glyphHasCode)
                    {
                        break;
                    }
                }
                if (glyphHasName && glyphHasCode)
                {
                    vectorImageSet.iconGlyphList.Add(currentGlyph);
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
