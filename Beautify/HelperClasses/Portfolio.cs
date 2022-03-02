using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Beautify
{
    public class Portfolio
    {
        public int id { get; set; }
        public string salonEmail { get; set; }
        public string serviceCategory { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public string dateAdded { get; set; }
        public string dateUpdated { get; set; }
    }
}