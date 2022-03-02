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
    public partial class WebForm9 : System.Web.UI.Page
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

                        // Load details of the earning
                        LoadEarningDetails(Membership.GetUser().Email, bookingID);

                        // Load the booked services
                        LoadBookedServices(bookingID);
                    }
                    catch
                    {
                        // If an error occurs, take the user to the earnings page.
                        Response.Redirect("Earnings.aspx");
                    }
                }
                else
                {
                    // Take the user to the earnings page if the query string parameter is not specified
                    Response.Redirect("Earnings.aspx");
                }
            }
        }

        private void LoadEarningDetails(string salonEmail, string bookingID)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT * FROM SalonEarnings WHERE BookingID = @BookingID AND SalonEmail = @SalonEmail";
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
                lblAccountName.InnerText = dt.Rows[0]["AccountName"].ToString();
                lblAccountNumber.InnerText = MyAppSecurity.Decrypt(dt.Rows[0]["AccountNumber"].ToString(), MyAppSecurity.GetPasswordBytes());
                lblBankName.InnerText = dt.Rows[0]["BankName"].ToString();
                lblClientName.InnerText = dt.Rows[0]["ClientName"].ToString();
                
                string paymentStatus = dt.Rows[0]["EarningPaymentStatus"].ToString();
                // Style the payment status
                switch (paymentStatus.ToUpper())
                {
                    case "PAID":
                        lblEarningPaymentStatus.InnerHtml = "<label class='label label-success'>PAID</label>";
                        lblPaymentStatus.InnerHtml = "<label class='label label-success'>PAID</label>";
                        lblDatePaid.InnerText = dt.Rows[0]["DatePaid"].ToString();
                        break;
                    case "UNPAID":
                        lblEarningPaymentStatus.InnerHtml = "<label class='label label-danger'>UNPAID</label>";
                        lblPaymentStatus.InnerHtml = "<label class='label label-danger'>UNPAID</label>";
                        break;
                }
            }
            else
            {
                // Take the user to the Earnings page if no record was returned
                Response.Redirect("Earnings.aspx");
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
            double subTotal = 0;
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
                subTotal += double.Parse(dt.Rows[i]["TotalCost"].ToString());
            }
            
            // Get the service commission percent for this booking
            double serviceCommissionPercent = GetServiceCommissionPercent(Membership.GetUser().Email, bookingID);
            double serviceCommission = (serviceCommissionPercent / 100) * subTotal;
            double totalAmountEarned = subTotal - serviceCommission;

            strBookedServices.Append("<tr>" +
                                    "<td colspan='4' class='text-right h4'><strong>Sub Total</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + subTotal.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td colspan='4' class='text-right h4'>-<strong> Service Commission(" + serviceCommissionPercent + "%)</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + serviceCommission.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                                "<tr class='active'>" +
                                    "<td colspan='4' class='text-right text-uppercase h4'><strong>Total Amount Earned</strong></td>" +
                                    "<td class='text-right text-success h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + totalAmountEarned.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                            "</tbody>" +
                        "</table>");
            divBookedServices.InnerHtml = strBookedServices.ToString();
            da.Dispose();
            dt.Clear();
            conn.Close();
        }

        private int GetServiceCommissionPercent(string salonEmail, string bookingID)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT ServiceCommissionPercent FROM SalonEarnings WHERE SalonEmail = @SalonEmail AND BookingID = @BookingID";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameters
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);
            da.SelectCommand.Parameters.AddWithValue("@BookingID", bookingID);
            dt = new DataTable();
            da.Fill(dt);
            int serviceCommissionPercent = 0;
            if (dt.Rows.Count != 0)
            {
                serviceCommissionPercent = int.Parse(dt.Rows[0]["ServiceCommissionPercent"].ToString());
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
            // Return the service commission percent
            return serviceCommissionPercent;
        }
    }
}