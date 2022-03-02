using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Beautify
{
    public class Earning
    {
        public string bookingID { get; set; }
        public string clientName { get; set; }
        public string salonEmail { get; set; }
        public double serviceCost { get; set; }
        public int serviceCommissionPercent { get; set; }
        public double serviceCommission { get; set; }
        public double finalSalonEarning { get; set; }
        public string bankName { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string earningPaymentStatus { get; set; }
        public string datePaid { get; set; }
        public string numericalDatePaid { get; set; }
    }
}