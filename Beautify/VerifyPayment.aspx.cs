using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Beautify.FidelityPayGateService;
using System.Data;
using System.Data.SqlClient;

namespace Beautify
{
    public partial class WebForm7 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["OrderID"] == null || Request.QueryString["TransactionReference"] == null)
                {
                    // Redirect to the homepage if the OrderID or TransactionReference query string is not specified
                    Response.Redirect("Default.aspx");
                }
            }
        }

        /// <summary>
        /// Updates details of a booking
        /// </summary>
        /// <param name="bookingID">The ID of the booking</param>
        /// <param name="paymentStatus">The payment status</param>
        /// <param name="bookingStatus">The booking status</param>
        private void UpdateBookingInfo(string bookingID, string paymentStatus, string bookingStatus)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"UPDATE Bookings SET PaymentStatus = @PaymentStatus, BookingStatus = @BookingStatus WHERE BookingID = @BookingID"; //the sql string        
            command.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
            command.Parameters.AddWithValue("@BookingStatus", bookingStatus);
            command.Parameters.AddWithValue("@BookingID", bookingID);
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
        }

        /// <summary>
        /// Gets the amount due for an INACTIVE booking. Note that INACTIVE bookings are those bookings where the user decided to make payment with Credit Card but is yet to make the payment
        /// </summary>
        /// <param name="bookingID">The ID of the booking</param>
        /// <returns>Returns the amount due for the booking. That is the total amount due</returns>
        private double GetBookingAmountDue(string bookingID)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            // Note that we are looking for inactive bookings here
            // This will ensure that only those bookings that have not been paid for are loaded
            // Note that INACTIVE bookings are those bookings where the user decided to make payment with Credit Card but is yet to make the payment
            
            string selectString = @"SELECT TotalAmountDue FROM Bookings WHERE BookingID = @BookingID AND PaymentStatus = 'UNPAID' AND BookingStatus = 'INACTIVE'";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            da.SelectCommand.Parameters.AddWithValue("@BookingID", bookingID);
            dt = new DataTable();
            da.Fill(dt);
            double totalAmountDue = 0;
            if (dt.Rows.Count != 0)
            {
                totalAmountDue = double.Parse(dt.Rows[0]["TotalAmountDue"].ToString());
            }
            else
            {
                // At this point, the booking was not found
                // So redirect the user to the Payment Failed page
                Response.Redirect("PaymentFailed.aspx");
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
            // Return the total amount due
            return totalAmountDue;
        }

        protected void btnVerifyMyPayment_Click(object sender, EventArgs e)
        {
            string orderID = Request.QueryString["OrderID"].ToString();
            string merchantID = "06307";
            string result = string.Empty;

            try
            {
                using (TransactionStatusCheckClient client = new TransactionStatusCheckClient())
                {

                    result = client.GetTransactionStatus(orderID, merchantID);

                }
                if (result.Equals("E00&[No Transaction Record]"))
                {
                    // At this point, it means the transaction was not found in Fidelity Bank's database
                    // So we should redirect the user to the Payment Failed page
                    Response.Redirect("PaymentFailed.aspx");
                }
                else
                {
                    // At this point, it means that the transaction exists. So we should check for the status.
                    // Split the returned transaction record
                    string[] transactionDetailsArray = result.Split(new string[] { "&" }, StringSplitOptions.None);
                    // Fetch the bookingID, that is the orderID
                    string bookingID = transactionDetailsArray[1];
                    // Fetch the amount that was paid
                    double amountPaid = double.Parse(transactionDetailsArray[2]);
                    // Fetch the payment channel
                    string paymentChannel = transactionDetailsArray[3];
                    // Fetch the transaction status
                    string transactionStatus = transactionDetailsArray[4];

                    // Check whether the status is successful. Note that "00" means the ransaction was successful. Any other value means the transaction was not successful
                    if (transactionStatus.Equals("00"))
                    {
                        // At this point, the transaction is successful
                        // So we now check whether this is a booking or rent and then update the appropriate table
                        switch (bookingID.Substring(0, 3))
                        {
                            case "BKN":
                                // This is a booking
                                // Now we should verify that the amount paid = cost of the subscription
                                double totalAmountDue = GetBookingAmountDue(bookingID);
                                if (amountPaid == totalAmountDue)
                                {
                                    // At this point, the payment was successful and the user paid the amount due for the booking
                                    // Update the user's booking Status to PENDING since it has not been attended to yet. Also set the payment status to PAID
                                    UpdateBookingInfo(bookingID, "PAID", "PENDING");

                                    // -----------At this point, we are sending SMS to the client, salon and admin-------------

                                    // Fetch the booking info
                                    BookingInfo book = new BookingInfo();
                                    book = GetBookingInfo(bookingID, "PENDING");

                                    // Fetch the salon info
                                    SalonInfo sal = new SalonInfo();
                                    sal = GetSalonInfo(book.salonEmail);

                                    // Fetch Beautify Phone number
                                    string beautifyPhoneNumber = System.Configuration.ConfigurationManager.AppSettings["beautifyPhoneNumber"];

                                    // At this point, we compose the SMS message to send to the client, salon and admin
                                    string messageToClient = @"Your booking details:\n" + sal.salonName + "(" + sal.phoneNumber + ")\nBookingID: " + bookingID + "\nReviewCode: " + book.reviewCode +
                                        "\nServiceDeliveryCode: " + book.serviceDeliveryCode + "\nCost: " + AppHelper.GetCurrencySymbol() + " " + book.subTotal.ToString("N0") + "\nVisit www.beautify.com.ng/book/" + sal.userName + " to view the address of the salon.\nThank you for choosing www.beautify.com.ng \nFor enquiries contact us on " + beautifyPhoneNumber;

                                    string messageToSalon = @"Hi, " + sal.userName + ", you have a new booking from " + book.clientName + "(" + book.clientPhoneNumber + ") Visit www.beautify.com.ng to view. \nFor enquiries contact us on " + beautifyPhoneNumber;

                                    string messageToAdmin = @"A new booking has been placed on the site. Visit www.beautify.com.ng to view";

                                    // Here we are sending the SMS
                                    // Send SMS to client
                                    AppHelper.SendSMS(messageToClient, book.clientPhoneNumber);
                                    // Send SMS to Salon
                                    AppHelper.SendSMS(messageToSalon, sal.phoneNumber);
                                    // Send SMS to Admin
                                    AppHelper.SendSMS(messageToAdmin, beautifyPhoneNumber);

                                    // Take the user to the success page
                                    Response.Redirect("BookingSuccessful.aspx");
                                }
                                else
                                {
                                    // The amount paid != subscription cost
                                    // So we redirect the user to the payment failed page
                                    Response.Redirect("PaymentFailed.aspx");
                                }
                                break;
                            case "RNT":
                                // This is a rent
                                break;
                        }
                        
                    }
                    else
                    {
                        // The transaction was not successful
                        // So we should redirect the user to the Payment Failed page
                        Response.Redirect("PaymentFailed.aspx");
                    }

                }

            }
            catch (Exception ex)
            {
                divMessage.InnerHtml = "<div class='alert alert-danger'>" +
                        "<h4><i class='fa fa-exclamation'></i> Error Verifying Payment</h4> Oops! an error occurred while verifying your payment. Please try again." +
                        "</div>";
            }
        }

        private SalonInfo GetSalonInfo(string salonEmail)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            // Note that we are selecting only the bank account info for the specified salon
            selectString = @"select Username, SalonName, PhoneNumber, BankName, AccountName, AccountNumber from Salons WHERE Email = @Email AND RentStatus = 'ACTIVE'";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameter
            da.SelectCommand.Parameters.AddWithValue("@Email", salonEmail);

            dt = new DataTable();
            da.Fill(dt);
            SalonInfo sal = new SalonInfo();
            // Ensure a record was returned
            if (dt.Rows.Count != 0)
            {
                sal.userName = dt.Rows[0]["Username"].ToString();
                sal.salonName = dt.Rows[0]["SalonName"].ToString();
                sal.phoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                sal.bankName = dt.Rows[0]["BankName"].ToString();
                sal.accountName = dt.Rows[0]["AccountName"].ToString();
                sal.accountNumber = MyAppSecurity.Decrypt(dt.Rows[0]["AccountNumber"].ToString(), MyAppSecurity.GetPasswordBytes());
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the salon info
            return sal;
        }

        private BookingInfo GetBookingInfo(string bookingID, string bookingStatus)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            // Note that we are selecting only PAID bookings for this salon
            selectString = @"select SalonEmail, ClientPhoneNumber, ClientName, SubTotal, ServiceDeliveryCode, ReviewCode from Bookings WHERE BookingID = @BookingID AND PaymentStatus = 'PAID' AND BookingStatus = @BookingStatus ORDER BY NumericalClientChoiceDate DESC";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameter
            da.SelectCommand.Parameters.AddWithValue("@BookingID", bookingID);
            da.SelectCommand.Parameters.AddWithValue("@BookingStatus", bookingStatus);

            dt = new DataTable();
            da.Fill(dt);
            BookingInfo bookInfo = new BookingInfo();
            // Ensure a record was returned
            if (dt.Rows.Count != 0)
            {
                bookInfo.bookingID = bookingID;
                bookInfo.clientPhoneNumber = dt.Rows[0]["ClientPhoneNumber"].ToString();
                bookInfo.clientName = dt.Rows[0]["ClientName"].ToString();
                bookInfo.salonEmail = dt.Rows[0]["SalonEmail"].ToString();
                bookInfo.subTotal = double.Parse(dt.Rows[0]["SubTotal"].ToString());
                // Decrypt the service delivery code
                bookInfo.serviceDeliveryCode = MyAppSecurity.Decrypt(dt.Rows[0]["ServiceDeliveryCode"].ToString(), MyAppSecurity.GetPasswordBytes());
                // Decrypt the review code
                bookInfo.reviewCode = MyAppSecurity.Decrypt(dt.Rows[0]["ReviewCode"].ToString(), MyAppSecurity.GetPasswordBytes());
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the booking info
            return bookInfo;
        }

    }
}