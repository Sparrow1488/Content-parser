using System.Collections.Generic;

namespace Sparrow.Parsing.Example.Nude.Entities
{
    internal class NudeMangaItem
    {
        public NudePreviewCard Preview { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; }
    }
}
