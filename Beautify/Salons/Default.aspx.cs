using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Beautify.Salons
{
    public partial class WebForm10 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Show the number of attended and pending bookings
                lblAttendedBookingsCount.InnerText = double.Parse(PagingDatabase.GetBookingsCount(Membership.GetUser().Email, "ATTENDED").ToString()).ToString("N0");
                lblPendingBookingsCount.InnerText = double.Parse(PagingDatabase.GetBookingsCount(Membership.GetUser().Email, "PENDING").ToString()).ToString("N0");

                // Show the number of paid and unpaid earnings
                lblPaidEarningsCount.InnerText = double.Parse(PagingDatabase.GetEarningsCount(Membership.GetUser().Email, "PAID").ToString()).ToString("N0");
                lblUnpaidEarningsCount.InnerText = double.Parse(PagingDatabase.GetEarningsCount(Membership.GetUser().Email, "UNPAID").ToString()).ToString("N0");

                // Show the total value of earnings
                lblTotalValueOfPaidEarnings.InnerText = AppHelper.GetCurrencySymbol() + " " + PagingDatabase.GetTotalValueOfEarnings(Membership.GetUser().Email, "PAID").ToString("N0");
                lblTotalValueOfUnpaidEarnings.InnerText = AppHelper.GetCurrencySymbol() + " " + PagingDatabase.GetTotalValueOfEarnings(Membership.GetUser().Email, "UNPAID").ToString("N0");
            }
        }
    }
}