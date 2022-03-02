using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Beautify
{
    public class Service
    {
        public int serviceID { get; set; }
        public string salonEmail { get; set; }
        public string serviceCategory { get; set; }
        public string serviceName { get; set; }
        public string shortDescription { get; set; }
        public string fullDescription { get; set; }
        public string imageUrl { get; set; }
        public double serviceCost { get; set; }
        public string serviceStatus { get; set; }
        public string dateAdded { get; set; }
        public string dateUpdated { get; set; }
    }
}