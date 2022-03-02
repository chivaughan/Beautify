using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Beautify
{
    public class Review
    {
        public string bookingID { get; set; }
        public string clientName { get; set; }
        public string salonEmail { get; set; }
        public double rating { get; set; }
        public string comment { get; set; }
        public string reviewDate { get; set; }
        public string numericalReviewDate { get; set; }
    }
}