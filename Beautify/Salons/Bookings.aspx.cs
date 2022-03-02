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
    public partial class WebForm4 : System.Web.UI.Page
    {
        // Set the initial page size and page index for services
        private int pageSizeBookings = 20;
        private int pageIndexBookings = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Register event handler for bookings pager user control event
            this.uclPagerBookings.PaginationLinkClicked += new EventHandler(bookingsPaginationLink_Click);

            if (Page.IsPostBack)
            {
                // Get the client ID of the control that caused the PostBack
                string postBackControlClientID = Request.Form["__EVENTTARGET"];


                // If the control is a member of uclPagerBookings, then we know that it was one of the link buttons in uclPagerBookings that caused the post back
                // So we proceed to handle that action
                // We are doing this check to ensure that the default page links 1 to 7 is not set for uclPagerBookings whenever a postback occurs from another control on this page
                if (postBackControlClientID.Contains("uclPagerBookings"))
                {
                    //During all postbacks - Add the pagination links to the page
                    int tableDataCount = PagingDatabase.GetBookingsCount(Membership.GetUser().Email, selBookingStatus.Value);

                    string defaultInitialValueForHiddenControl = "Blank Value";
                    int indexFromPreviousDataRetrieval = 1;
                    if (!String.Equals(hdnCurrentIndexBookings.Value, defaultInitialValueForHiddenControl))
                    {
                        indexFromPreviousDataRetrieval = Convert.ToInt32(hdnCurrentIndexBookings.Value, CultureInfo.InvariantCulture);
                    }

                    //Set property of user control
                    uclPagerBookings.PreviousIndex = indexFromPreviousDataRetrieval;

                    //Call method in user control
                    uclPagerBookings.PreAddAllLinks(tableDataCount, pageSizeBookings, indexFromPreviousDataRetrieval);
                }

            }

            if (!Page.IsPostBack)
            {
                // Load the bookings, set the first page as current index and then paginate
                pageIndexBookings = 1;
                BindBookingsDataAndProcessPagination("PageFirstLoad");
            }
        }

        private void BindBookingsDataAndProcessPagination(string caller)
        {

            string status = selBookingStatus.Value;
            List<BookingInfo> searchResult = new List<BookingInfo>();

            // At this point, we show all services
            searchResult = PagingDatabase.GetBookingsData(Membership.GetUser().Email, status, pageSizeBookings, pageIndexBookings).ToList();
            


            StringBuilder strBookings = new StringBuilder();

            strBookings.Append("<table class='table table-bordered table-striped table-vcenter'>" +
                                "<thead>" +
                                    "<tr>" +
                                        "<th class='text-center' style='width: 70px;'>Booking ID</th>" +
                                        "<th>Client Name</th>" +
                                        "<th class='text-right hidden-xs'>Cost</th>" +
                                        "<th class='hidden-xs'>Status</th>" +
                                        "<th class='hidden-xs text-center'>Booked</th>" +
                                        "<th class='text-center'>Action</th>" +
                                    "</tr>" +
                                "</thead>" +
                                "<tbody>");


            // Add all services found
            for (int i = 0; i < searchResult.Count; i++)
            {
                // Append the bookings
                strBookings.Append("<tr>" +
                                        "<td class='text-center'><a href='ViewBooking.aspx?id=" + searchResult[i].bookingID + "'>" + searchResult[i].bookingID + "</a></td>" +
                                        "<td><a href='ViewBooking.aspx?id=" + searchResult[i].bookingID + "'>" + searchResult[i].clientName + "</a></td>" +
                                        "<td class='text-right hidden-xs'><strong>" + AppHelper.GetCurrencySymbol() + " " + searchResult[i].subTotal.ToString("N2") + "</strong></td>");
                // Style the status of each booking
                switch (searchResult[i].status.ToUpper())
                {
                    case "ATTENDED":
                        strBookings.Append("<td class='hidden-xs'>" +
                                            "<span class='label label-success'>ATTENDED</span>" +
                                        "</td>");
                        break;
                    case "UNATTENDED":
                        strBookings.Append("<td class='hidden-xs'>" +
                                            "<span class='label label-warning'>UNATTENDED</span>" +
                                        "</td>");
                        break;
                    case "CANCELLED":
                        strBookings.Append("<td class='hidden-xs'>" +
                                            "<span class='label label-danger'>CANCELLED</span>" +
                                        "</td>");
                        break;
                    case "PENDING":
                        strBookings.Append("<td class='hidden-xs'>" +
                                            "<span class='label label-primary'>PENDING</span>" +
                                        "</td>");
                        break;

                }
                
                strBookings.Append("<td class='hidden-xs text-center'>" + searchResult[i].clientChoiceDate + "</td>" +
                                        "<td class='text-center'>" +
                                            "<div class='btn-group btn-group-xs'>" +
                                                "<a href='ViewBooking.aspx?id=" + searchResult[i].bookingID + "' data-toggle='tooltip' title='View' class='btn btn-default'><i class='fa fa-pencil'></i></a>" +
                                                "</div>" +
                                        "</td>" +
                                    "</tr>");

            }

            strBookings.Append("</tbody>" +
                            "</table>");
            divServices.InnerHtml = strBookings.ToString();

            //Index
            hdnCurrentIndexBookings.Value = pageIndexBookings.ToString(CultureInfo.InvariantCulture);

            int tableDataCount = 0;
            //Get total number of records
            tableDataCount = PagingDatabase.GetBookingsCount(Membership.GetUser().Email, status);
            

            // Only compute the bookings count if this method is being called by PageFirstLoad
            if (caller.Equals("PageFirstLoad"))
            {
                // Show the number of attended, cancelled, pending, unattended, and all bookings
                lblAttendedBookingsCount.InnerText = double.Parse(PagingDatabase.GetBookingsCount(Membership.GetUser().Email, "ATTENDED").ToString()).ToString("N0");
                lblCancelledBookingsCount.InnerText = double.Parse(PagingDatabase.GetBookingsCount(Membership.GetUser().Email, "CANCELLED").ToString()).ToString("N0");
                lblPendingBookingsCount.InnerText = double.Parse(PagingDatabase.GetBookingsCount(Membership.GetUser().Email, "PENDING").ToString()).ToString("N0");
                lblUnattendedBookingsCount.InnerText = double.Parse(PagingDatabase.GetBookingsCount(Membership.GetUser().Email, "UNATTENDED").ToString()).ToString("N0");
                lblAllBookingsCount.InnerText = double.Parse(tableDataCount.ToString()).ToString("N0");
            }


            uclPagerBookings.AddPageLinks(tableDataCount, pageSizeBookings, pageIndexBookings);

            // Show an appropriate description for the search
            lblSearchDescription.InnerHtml = "<strong>" + selBookingStatus.Value + "</strong> Bookings";
            

        }

        protected void bookingsPaginationLink_Click(object sender, EventArgs e)
        {
            //When link is clicked, set the pageIndex from user control property
            pageIndexBookings = uclPagerBookings.CurrentClickedIndex;
            // Set the page size from the select
            pageSizeBookings = int.Parse(selBookingsPageSize.Value);

            BindBookingsDataAndProcessPagination("PaginationLink");
            
        }

        protected void lnkAttendedBookings_Click(object sender, EventArgs e)
        {
            //When the 'Attended Bookings' link button is clicked, we should set the first page as the page index, 
            // set the page size that the user has chosen and then search for attended bookings
            pageIndexBookings = 1;
            pageSizeBookings = int.Parse(selBookingsPageSize.Value);
            // Select option 'ATTENDED' from the category dropdown
            selBookingStatus.Value = "ATTENDED";
            BindBookingsDataAndProcessPagination("AttendedBookings");
        }

        protected void lnkCancelledBookings_Click(object sender, EventArgs e)
        {
            //When the 'Cancelled Bookings' link button is clicked, we should set the first page as the page index, 
            // set the page size that the user has chosen and then search for cancelled bookings
            pageIndexBookings = 1;
            pageSizeBookings = int.Parse(selBookingsPageSize.Value);
            // Select option 'CANCELLED' from the category dropdown
            selBookingStatus.Value = "CANCELLED";
            BindBookingsDataAndProcessPagination("CancelledBookings");
        }

        protected void lnkPendingBookings_Click(object sender, EventArgs e)
        {
            //When the 'Pending Bookings' link button is clicked, we should set the first page as the page index, 
            // set the page size that the user has chosen and then search for pending bookings
            pageIndexBookings = 1;
            pageSizeBookings = int.Parse(selBookingsPageSize.Value);
            // Select option 'PENDING' from the category dropdown
            selBookingStatus.Value = "PENDING";
            BindBookingsDataAndProcessPagination("PendingBookings");
        }

        protected void lnkUnattendedBookings_Click(object sender, EventArgs e)
        {
            //When the 'Unattended Bookings' link button is clicked, we should set the first page as the page index, 
            // set the page size that the user has chosen and then search for unattended bookings
            pageIndexBookings = 1;
            pageSizeBookings = int.Parse(selBookingsPageSize.Value);
            // Select option 'UNATTENDED' from the category dropdown
            selBookingStatus.Value = "UNATTENDED";
            BindBookingsDataAndProcessPagination("UnattendedBookings");
        }

        protected void lnkAllBookings_Click(object sender, EventArgs e)
        {
            //When the 'All Bookings' link button is clicked, we should set the first page as the page index, 
            // set the page size that the user has chosen and then search for all bookings
            pageIndexBookings = 1;
            pageSizeBookings = int.Parse(selBookingsPageSize.Value);
            // Select option 'All' from the category dropdown
            selBookingStatus.Value = "All";
            BindBookingsDataAndProcessPagination("AllBookings");
        }

        protected void lnkRefreshBookings_Click(object sender, EventArgs e)
        {
            //When the refresh button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for bookings again
            pageIndexBookings = 1;
            pageSizeBookings = int.Parse(selBookingsPageSize.Value);
            BindBookingsDataAndProcessPagination("RefreshButton");
        }
    }
}