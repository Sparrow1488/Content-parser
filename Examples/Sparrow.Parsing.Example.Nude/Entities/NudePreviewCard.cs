using System.Collections.Generic;

namespace Sparrow.Parsing.Example.Nude.Entities
{
    internal class NudePreviewCard
    {
        public string Title { get; set; }
        public string MainSource { get; set; }
        public string PreviewImage { get; set; }
        public List<string> Tags { get; } = new List<string>();
    }
}
