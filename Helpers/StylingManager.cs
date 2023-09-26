using System.Collections.Generic;

namespace Inzynierka.Helpers
{
    public class StylingManager
    {
        public string[] _tableStylingKeys { get; set; }
        public string[] _textStylingKeys { get; set; }
        public string[] _tableStylingValues { get; set; }
        public string[] _textStylingValues { get; set; }
        public string[] _fontFamilies { get; set; }
        public string[] _tableBorders { get; set; }
        public string stylingName { get; set; }

        public StylingManager()
        {
            InitTextStyling();
            InitTableStyling();
            InitFontFamilies();
            InitTableBorders();
            _tableStylingValues = new string[_tableStylingKeys.Length];
            _textStylingValues = new string[_textStylingKeys.Length];
        }

        private void InitTableBorders()
        {
            _tableBorders = new string[]
            {
                "dotted",
                "dashed", 
                "solid", 
                "double", 
                "grove", 
                "ridge"
            };
        }

        public void InitTextStyling()
        {
            //fill it with stylings froms css files
            _textStylingKeys = new string[]
            {
                "color",
                "font-family",
                "font-size",
                "font-weight"
            };
        }

        public void InitTableStyling()
        {
            //fill it with stylings froms css files
            _tableStylingKeys = new string[]
            {
                "color",
                "font-family",
                "font-size",
                "font-weight",
                "border",
                "border-color",
                "border-style",
            };
        }

        public void InitFontFamilies()
        {
            _fontFamilies = new string[]
            {
                "georgia",
                "sans-serif",
                "serif",
                "cursive",
                "stystem-ui",
                "monospace",
                "fantasy"
            };
        }
    }

    public enum Stylings
    {
        TableStyling,
        TextStyling
    }
}
