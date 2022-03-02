using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Globalization;
using System.Web.Security;

namespace Beautify.Salons
{
    public partial class WebForm6 : System.Web.UI.Page
    {
        // Set the initial page size and page index for earnings
        private int pageSizeEarnings = 20;
        private int pageIndexEarnings = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Register event handler for bookings pager user control event
            this.uclPagerEarnings.PaginationLinkClicked += new EventHandler(earningsPaginationLink_Click);

            if (Page.IsPostBack)
            {
                // Get the client ID of the control that caused the PostBack
                string postBackControlClientID = Request.Form["__EVENTTARGET"];


                // If the control is a member of uclPagerEarnings, then we know that it was one of the link buttons in uclPagerEarnings that caused the post back
                // So we proceed to handle that action
                // We are doing this check to ensure that the default page links 1 to 7 is not set for uclPagerEarnings whenever a postback occurs from another control on this page
                if (postBackControlClientID.Contains("uclPagerEarnings"))
                {
                    //During all postbacks - Add the pagination links to the page
                    int tableDataCount = PagingDatabase.GetEarningsCount(Membership.GetUser().Email, selEarningStatus.Value);

                    string defaultInitialValueForHiddenControl = "Blank Value";
                    int indexFromPreviousDataRetrieval = 1;
                    if (!String.Equals(hdnCurrentIndexEarnings.Value, defaultInitialValueForHiddenControl))
                    {
                        indexFromPreviousDataRetrieval = Convert.ToInt32(hdnCurrentIndexEarnings.Value, CultureInfo.InvariantCulture);
                    }

                    //Set property of user control
                    uclPagerEarnings.PreviousIndex = indexFromPreviousDataRetrieval;

                    //Call method in user control
                    uclPagerEarnings.PreAddAllLinks(tableDataCount, pageSizeEarnings, indexFromPreviousDataRetrieval);
                }

            }

            if (!Page.IsPostBack)
            {
                // Load the earnings, set the first page as current index and then paginate
                pageIndexEarnings = 1;
                BindEarningsDataAndProcessPagination("PageFirstLoad");
            }
        }

        private void BindEarningsDataAndProcessPagination(string caller)
        {

            string status = selEarningStatus.Value;
            List<Earning> searchResult = new List<Earning>();

            // At this point, we show all services
            searchResult = PagingDatabase.GetEarningsData(Membership.GetUser().Email, status, pageSizeEarnings, pageIndexEarnings).ToList();



            StringBuilder strEarnings = new StringBuilder();

            strEarnings.Append("<table class='table table-bordered table-striped table-vcenter'>" +
                                "<thead>" +
                                    "<tr>" +
                                        "<th class='text-center' style='width: 70px;'>Booking ID</th>" +
                                        "<th>Client Name</th>" +
                                        "<th class='text-right hidden-xs'>Amount Earned</th>" +
                                        "<th class='hidden-xs'>Status</th>" +
                                        "<th class='hidden-xs text-center'>Date Paid</th>" +
                                        "<th class='text-center'>Action</th>" +
                                    "</tr>" +
                                "</thead>" +
                                "<tbody>");


            // Add all earnings found
            for (int i = 0; i < searchResult.Count; i++)
            {
                // Append the earnings
                strEarnings.Append("<tr>" +
                                        "<td class='text-center'><a href='ViewEarning.aspx?id=" + searchResult[i].bookingID + "'>" + searchResult[i].bookingID + "</a></td>" +
                                        "<td><a href='ViewEarning.aspx?id=" + searchResult[i].bookingID + "'>" + searchResult[i].clientName + "</a></td>" +
                                        "<td class='text-right hidden-xs'><strong>" + AppHelper.GetCurrencySymbol() + " " + searchResult[i].finalSalonEarning.ToString("N0") + "</strong></td>");
                // Style the status of each earning
                switch (searchResult[i].earningPaymentStatus.ToUpper())
                {
                    case "PAID":
                        strEarnings.Append("<td class='hidden-xs'>" +
                                            "<span class='label label-success'>PAID</span>" +
                                        "</td>");
                        break;
                    case "UNPAID":
                        strEarnings.Append("<td class='hidden-xs'>" +
                                            "<span class='label label-danger'>UNPAID</span>" +
                                        "</td>");
                        break;
                    
                }

                strEarnings.Append("<td class='hidden-xs text-center'>" + searchResult[i].datePaid + "</td>" +
                                        "<td class='text-center'>" +
                                            "<div class='btn-group btn-group-xs'>" +
                                                "<a href='ViewEarning.aspx?id=" + searchResult[i].bookingID + "' data-toggle='tooltip' title='View' class='btn btn-default'><i class='fa fa-pencil'></i></a>" +
                                                "</div>" +
                                        "</td>" +
                                    "</tr>");

            }

            strEarnings.Append("</tbody>" +
                            "</table>");
            divEarnings.InnerHtml = strEarnings.ToString();

            //Index
            hdnCurrentIndexEarnings.Value = pageIndexEarnings.ToString(CultureInfo.InvariantCulture);

            int tableDataCount = 0;
            //Get total number of records
            tableDataCount = PagingDatabase.GetEarningsCount(Membership.GetUser().Email, status);


            // Only compute the bookings count if this method is being called by PageFirstLoad
            if (caller.Equals("PageFirstLoad"))
            {
                // Show the number of paid, unpaid, and all earnings
                lblPaidEarningsCount.InnerText = double.Parse(PagingDatabase.GetEarningsCount(Membership.GetUser().Email, "PAID").ToString()).ToString("N0");
                lblUnpaidEarningsCount.InnerText = double.Parse(PagingDatabase.GetEarningsCount(Membership.GetUser().Email, "UNPAID").ToString()).ToString("N0");
                lblAllEarningsCount.InnerText = double.Parse(tableDataCount.ToString()).ToString("N0");

                // Show the total value of earnings
                lblTotalValueOfPaidEarnings.InnerText = AppHelper.GetCurrencySymbol() + " " + PagingDatabase.GetTotalValueOfEarnings(Membership.GetUser().Email, "PAID").ToString("N0");
                lblTotalValueOfUnpaidEarnings.InnerText = AppHelper.GetCurrencySymbol() + " " + PagingDatabase.GetTotalValueOfEarnings(Membership.GetUser().Email, "UNPAID").ToString("N0");
                lblTotalValueOfAllEarnings.InnerText = AppHelper.GetCurrencySymbol() + " " + PagingDatabase.GetTotalValueOfEarnings(Membership.GetUser().Email, "All").ToString("N0");
            }


            uclPagerEarnings.AddPageLinks(tableDataCount, pageSizeEarnings, pageIndexEarnings);

            // Show an appropriate description for the search
            lblSearchDescription.InnerHtml = "<strong>" + selEarningStatus.Value + "</strong> Earnings";


        }

        protected void earningsPaginationLink_Click(object sender, EventArgs e)
        {
            //When link is clicked, set the pageIndex from user control property
            pageIndexEarnings = uclPagerEarnings.CurrentClickedIndex;
            // Set the page size from the select
            pageSizeEarnings = int.Parse(selEarningsPageSize.Value);

            BindEarningsDataAndProcessPagination("PaginationLink");

        }

        protected void lnkPaidEarnings_Click(object sender, EventArgs e)
        {
            //When the 'Paid Earnings' link button is clicked, we should set the first page as the page index, 
            // set the page size that the user has chosen and then search for paid earnings
            pageIndexEarnings = 1;
            pageSizeEarnings = int.Parse(selEarningsPageSize.Value);
            // Select option 'PAID' from the category dropdown
            selEarningStatus.Value = "PAID";
            BindEarningsDataAndProcessPagination("PaidEarnings");
        }

        protected void lnkUnpaidEarnings_Click(object sender, EventArgs e)
        {
            //When the 'Unpaid Earnings' link button is clicked, we should set the first page as the page index, 
            // set the page size that the user has chosen and then search for unpaid earnings
            pageIndexEarnings = 1;
            pageSizeEarnings = int.Parse(selEarningsPageSize.Value);
            // Select option 'UNPAID' from the category dropdown
            selEarningStatus.Value = "UNPAID";
            BindEarningsDataAndProcessPagination("UnpaidEarnings");
        }

        protected void lnkAllEarnings_Click(object sender, EventArgs e)
        {
            //When the 'All Earnings' link button is clicked, we should set the first page as the page index, 
            // set the page size that the user has chosen and then search for all earnings
            pageIndexEarnings = 1;
            pageSizeEarnings = int.Parse(selEarningsPageSize.Value);
            // Select option 'All' from the category dropdown
            selEarningStatus.Value = "All";
            BindEarningsDataAndProcessPagination("AllEarnings");
        }

        protected void lnkRefreshEarnings_Click(object sender, EventArgs e)
        {
            //When the refresh button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for earnings again
            pageIndexEarnings = 1;
            pageSizeEarnings = int.Parse(selEarningsPageSize.Value);
            BindEarningsDataAndProcessPagination("RefreshButton");
        }
    }
}