using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POEApi.Model
{
    public class ShopEntryInfo
    {

        public int Id { get; private set; }
        public string Buyout { get; set; }
        public string CurrentOffer { get; set; }
        public bool Highlight { get; set; }
        public DateTime LastUpdated { get; set; }

        public ShopEntryInfo(int id, string buyout, string currentOffer, bool highlight, DateTime lastUpdated)
        {
            Id = id;
            Buyout = buyout;
            CurrentOffer = currentOffer;
            Highlight = highlight;
            LastUpdated = lastUpdated;
        }

    }
}
