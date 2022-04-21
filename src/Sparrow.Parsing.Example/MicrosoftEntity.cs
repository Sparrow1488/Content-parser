using System;
using System.Collections.Generic;

namespace Sparrow.Parsing.Example
{
    internal class MicrosoftEntity
    {
        public MicrosoftEntity()
        {
            News = new List<MicrosoftNews>();
            Products = new List<MicrosoftProduct>();
        }

        public Guid Guid { get; set; }
        public List<MicrosoftProduct> Products { get; set; }
        public List<MicrosoftNews> News { get; set; }

        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"Total News: {News.Count}\nTotal Products: {Products.Count}\nCreated at: {CreatedAt}";
        }
    }
}
