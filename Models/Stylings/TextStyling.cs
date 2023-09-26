using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Inzynierka.Models
{
    public class TextStyling
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "xml")]
        public string Values { get; set; }
        public string CreatorUsername { get; set; }
        public string ReferenceToken { get; set; }
    }
}
