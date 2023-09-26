using Inzynierka.Models;
using System.Xml.Linq;

namespace Inzynierka.Helpers
{
    public class XMLManager
    {
        //Create styling based on filled form and return whole document.
        //In that case siingular stylings won't be saved
        public static XElement CreateStyling(StylingManager stylings)
        {
            XElement stylingDocument = 
                new XElement("root", new XAttribute("name", stylings.stylingName),
                    new XElement("text-styling"),
                    new XElement("table-styling")
                    );

            XElement? textStylingNode = stylingDocument.Descendants("text-styling")?.FirstOrDefault();
            if(textStylingNode != null)
            {
                for(int i = 0; i < stylings._textStylingKeys.Length; i++)
                {
                    XElement element = new XElement("data",
                        new XAttribute("name", stylings._textStylingKeys[i]),
                        new XAttribute("value", stylings._textStylingValues[i]));
                    //element.Name = stylings._textStylingKeys[i].ToString();
                    textStylingNode.Add(element);
                }
            }

            XElement tableStylingNode = stylingDocument.Element("table-styling");
            if(tableStylingNode != null)
            {
                for (int i = 0; i < stylings._tableStylingKeys.Length; i++)
                {
                    XElement element = new XElement("data",
                        new XAttribute("name", stylings._tableStylingKeys[i]),
                        new XAttribute("value", stylings._tableStylingValues[i]));
                    tableStylingNode.Add(element);
                }
            }
            return stylingDocument;
        }

        //Create styling for single type of object to style
        public static XElement CreateSingleStyling(string[] stylingKeys, string[] stylingValues, string elementName)
        {
            XElement styleDoc = new XElement(elementName);

            if (stylingKeys != null && stylingValues != null && stylingKeys.Length == stylingValues.Length)
            {
                for (int i = 0; i < stylingKeys.Length; i++)
                {
                    XElement element = new XElement("data",
                        new XAttribute("name", stylingKeys[i]),
                        new XAttribute("value", stylingValues[i]));
                    //element.Name = stylings._textStylingKeys[i].ToString();
                    styleDoc.Add(element);
                }
            }
            return styleDoc;
        }

        //Create whole styling by joining it together wit heach other
        public static XElement JoinMulitpleStyles(XElement[] styles, string stylingName = "custom-style")
        {
            XElement stylingDocument = new XElement("root", new XAttribute("name", stylingName));

            foreach(XElement style in styles)
            {
                stylingDocument.Add(style);
            }

            return stylingDocument;
        }

    }
}
