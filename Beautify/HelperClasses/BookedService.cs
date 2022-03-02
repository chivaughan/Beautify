using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Beautify
{
    public class BookedService
    {
        public int serviceID { get; set; }
        public string serviceCategory { get; set; }
        public string serviceName { get; set; }
        public string shortDescription { get; set; }
        public string fullDescription { get; set; }
        public string imageUrl { get; set; }
        public double unitCost { get; set; }
        public int quantity { get; set; }
    }
}