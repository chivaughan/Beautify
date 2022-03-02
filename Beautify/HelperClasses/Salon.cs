using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Beautify
{
    public class Salon
    {
        
        public string username { get; set; }
        public string email { get; set; }
        public string salonName {get;set;}
        public string imageUrl { get; set; }
        public string locations { get; set; }
        public double totalRating { get; set; }
        public int numberOfVotes { get; set; }
        public double averageRating { get; set; }
        public int numberOfBookings { get; set; }
    }
}