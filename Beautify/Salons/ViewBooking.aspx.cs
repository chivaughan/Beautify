using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Security;

namespace Beautify.Salons
{
    public partial class WebForm5 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    try
                    {
                        string bookingID = Request.QueryString["id"].ToString();

                        // Load details od the booking
                        LoadBookingDetails(Membership.GetUser().Email, bookingID);

                        // Load the booked services
                        LoadBookedServices(bookingID);
                    }
                    catch
                    {
                        // If an error occurs, take the user to the bookings page.
                        Response.Redirect("Bookings.aspx");
                    }
                }
                else
                {
                    // Take the user to the bookings page if the query string parameter is not specified
                    Response.Redirect("Bookings.aspx");
                }
            }
        }

        private void LoadBookingDetails(string salonEmail, string bookingID)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT BookingID, ClientPhoneNumber, ClientName, ClientEmail, ClientChoiceDate, ClientChoiceTime, City, ClientChoiceLocation, OtherNotes, SubTotal, BookingStatus FROM Bookings WHERE BookingID = @BookingID AND SalonEmail = @SalonEmail AND PaymentStatus = 'PAID'";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameters
            da.SelectCommand.Parameters.AddWithValue("@BookingID", bookingID);
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);
            dt = new DataTable();
            da.Fill(dt);
            
            // Ensure a record is returned before attempting to read
            if (dt.Rows.Count != 0)
            {
                lblBookingID.InnerText = dt.Rows[0]["BookingID"].ToString();
                lblClientName.InnerText = dt.Rows[0]["ClientName"].ToString();
                lblPhoneNumber.InnerText = dt.Rows[0]["ClientPhoneNumber"].ToString();
                lblEmail.InnerText = dt.Rows[0]["ClientEmail"].ToString();
                lblClientChoiceDate.InnerText = dt.Rows[0]["ClientChoiceDate"].ToString();
                lblClientChoiceTime.InnerText = dt.Rows[0]["ClientChoiceTime"].ToString();
                lblCity.InnerText = dt.Rows[0]["City"].ToString();
                lblClientChoiceLocation.InnerText = dt.Rows[0]["ClientChoiceLocation"].ToString();
                lblOtherNotes.InnerText = dt.Rows[0]["OtherNotes"].ToString();
                
                string bookingStatus = dt.Rows[0]["BookingStatus"].ToString();
                // Style the booking status
                switch (bookingStatus.ToUpper())
                {
                    case "ATTENDED":
                        lblBookingStatus.InnerHtml = "<label class='label label-success'>ATTENDED</label>";
                        // Hide the service delivery div
                        divServiceDeliveryCode.Visible = false;      
                        break;
                    case "UNATTENDED":
                        lblBookingStatus.InnerHtml = "<label class='label label-warning'>UNATTENDED</label>";
                        // Hide the service delivery div
                        divServiceDeliveryCode.Visible = false;
                        break;
                    case "CANCELLED":
                        lblBookingStatus.InnerHtml = "<label class='label label-danger'>CANCELLED</label>";
                        // Hide the service delivery div
                        divServiceDeliveryCode.Visible = false;
                        break;
                    case "PENDING":
                        lblBookingStatus.InnerHtml = "<label class='label label-primary'>PENDING</label>";
                        // Hide the service delivery div
                        divServiceDeliveryCode.Visible = true;
                        break;
                }
            }
            else
            {
                // Take the user to the Bookings page if no record was returned
                Response.Redirect("Bookings.aspx");
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
        }

        private void LoadBookedServices(string bookingID)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT * FROM BookingsDetails WHERE BookingID = @BookingID";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameters
            da.SelectCommand.Parameters.AddWithValue("@BookingID", bookingID);
            dt = new DataTable();
            da.Fill(dt);
            StringBuilder strBookedServices = new StringBuilder();
            strBookedServices.Append("<table class='table table-bordered table-vcenter'>" +
                            "<thead>" +
                                "<tr>" +
                                    "<th colspan='2'>Service</th>" +
                                    "<th class='text-center'>QTY</th>" +
                                    "<th class='text-right'>Unit Price</th>" +
                                    "<th class='text-right'>Price</th>" +
                                "</tr>" +
                            "</thead>" +
                            "<tbody>");
            double totalAmountPaid = 0;
            // Loop through all booked services
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Append each service to the string builder
                strBookedServices.Append("<tr>" +
                                    "<td style='width: 200px;'>" +
                                        "<img src='../" + dt.Rows[i]["ImageUrl"].ToString() + "' alt='' style='width: 180px;'>" +
                                    "</td>" +
                                    "<td>" +
                                        "<strong>" + dt.Rows[i]["ServiceName"].ToString() + "</strong><br>" +
                                        dt.Rows[i]["ShortDescription"].ToString() + "<br>" +
                                        "<strong class='text-success'>" + dt.Rows[i]["ServiceCategory"].ToString() + "</strong>" +
                                    "</td>" +
                                    "<td class='text-center'>" +
                                    "<span class='label label-success'><strong>" + dt.Rows[i]["Quantity"].ToString() + "</strong></span>" +
                                    "</td>" +
                    // Note that here, we are computing quantity * unitPrice
                                    "<td class='text-right'>" + AppHelper.GetCurrencySymbol() + " " + double.Parse(dt.Rows[i]["UnitCost"].ToString()).ToString("N0") + "</td>" +
                                    "<td class='text-right'><strong>" + AppHelper.GetCurrencySymbol() + " " + (double.Parse(dt.Rows[i]["TotalCost"].ToString())).ToString("N0") + "</strong></td>" +
                                "</tr>");
                // Increment the total amount paid
                totalAmountPaid += double.Parse(dt.Rows[i]["TotalCost"].ToString());
            }

            strBookedServices.Append("<tr>" +
                                    "<td colspan='4' class='text-right h4'><strong>Sub Total</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + totalAmountPaid.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                                "<tr class='active'>" +
                                    "<td colspan='4' class='text-right text-uppercase h4'><strong>Total Amount Paid</strong></td>" +
                                    "<td class='text-right text-success h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + totalAmountPaid.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                            "</tbody>" +
                        "</table>");
            divBookedServices.InnerHtml = strBookedServices.ToString();
            da.Dispose();
            dt.Clear();
            conn.Close();
        }
    }
}