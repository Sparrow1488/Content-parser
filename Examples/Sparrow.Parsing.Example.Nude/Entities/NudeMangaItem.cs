using System.Collections.Generic;

namespace Sparrow.Parsing.Example.Nude.Entities
{
    internal class NudeMangaItem
    {
        public NudePreview Preview { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
