using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Collections.Specialized;

namespace Beautify
{
    public partial class WebForm5 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*if (!Page.IsPostBack)
            {
                if (Request.QueryString["salon"] != null)
                {
                    // The salon query parameter was supplied
                    // Use it to load the cities
                    string salonUsername = Request.QueryString["salon"].ToString();
                    LoadCities(salonUsername);
                }
                else
                {
                    // The salon query parameter was not supplied
                    // Take the user to the home page
                    Response.Redirect("Default.aspx");
                }
                
            }*/
        }

        protected void btnMakePayment_Click(object sender, EventArgs e)
        {
            // Make payment
            MakePayment(lblShoppingCart.Value, lblOutstandingPhoneNumber.Value, lblOutstandingBookingID.Value, lblOutstandingUnlockCode.Value, txtClientName.Text, txtClientEmail.Text, txtClientPhoneNumber.Text, cldChoiceDate.Text, txtChoiceTime.Text, hdnChoiceCity.Value, txtChoiceLocation.InnerText, txtOtherNotes.InnerText);
        }

        private void MakePayment(string serializedShoppingCartItems, string outstanding_phoneNumber, string outstanding_bookingID, string outstanding_unlockCode, string booking_name, string booking_email, string booking_phoneNumber, string booking_choiceDate, string booking_choiceTime, string booking_city, string booking_address, string booking_otherNotes)
        {
            // Fetch the outstanding balance for the specified wallet details. We are doing this to reverify the outstanding balance that the user might have applied to his cart
            double outstandingBalance = GetOutstandingBalance(outstanding_phoneNumber, outstanding_bookingID, outstanding_unlockCode);
            double totalDue = 0;
            
                // Deserialize the cart items
                List<CartItem> shoppingCartItems = (List<CartItem>)JsonConvert.DeserializeObject(serializedShoppingCartItems, typeof(List<CartItem>));

                string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
                SqlConnection conn;
                SqlDataAdapter da;
                DataTable dt;
                string selectString = "";
                int serviceID;
                double subTotal = 0;
                double handlingFeePercentage = 0;

                // We create a list of booked services
                List<BookedService> listOfBookedServices = new List<BookedService>();

                // This variable will keep track of the previous salon email. 
                // We will use this variable to ensure that services can only be booked from 1 salon at a time
                string previousSalonEmail = "";
                string salonName = "";
                // Counter to keep track of the number of valid cart items
                int validCartItemCount = 0;
                List<int> readCartItems = new List<int>();

                // Loop through the list of cart items
                foreach (CartItem item in shoppingCartItems)
                {
                    // We should only proceed to fetch a service if the service has not been read before
                    if (!readCartItems.Contains(item.id))
                    {
                        // Fetch the id of each service
                        serviceID = item.id;

                        // Add the serviceID to the list of read cart items
                        readCartItems.Add(serviceID);

                        // The select string to fetch service details and HandlingFee percentage. Note that only salons who have ENABLED booking and have an ACTIVE rent will be returned
                        selectString = @"select ServicePricing.*, PlatformCharges.HandlingFeePercentage, Salons.SalonName from ServicePricing INNER JOIN PlatformCharges ON ServicePricing.ServiceID = @ServiceID AND PlatformCharges.SettingName = 'ClientServiceBooking' INNER JOIN Salons ON ServicePricing.SalonEmail = Salons.Email WHERE Salons.BookingStatus = 'ENABLED' AND Salons.RentStatus = 'ACTIVE'";
                        conn = new SqlConnection(connString);
                        conn.Open();
                        da = new SqlDataAdapter(selectString, conn);
                        // Add the ServiceID parameter
                        da.SelectCommand.Parameters.AddWithValue("@ServiceID", serviceID);
                        dt = new DataTable();
                        da.Fill(dt);
                        // Ensure a row was returned before reading values
                        if (dt.Rows.Count != 0)
                        {
                            // Increment the number of valid cart items by 1
                            validCartItemCount++;

                            // We should assign the previousSalonEmail for the first valid cart item
                            if (validCartItemCount == 1)
                            {
                                // Assign the salon email
                                previousSalonEmail = dt.Rows[0]["SalonEmail"].ToString();
                                salonName = dt.Rows[0]["SalonName"].ToString();
                            }

                            // We should only append a cart item if the previousSalonEmail = currently read SalonEmail
                            // This will ensure that services can only be booked from a single salon at a time
                            if (previousSalonEmail.Equals(dt.Rows[0]["SalonEmail"].ToString()))
                            {
                                // Increment the sub total
                                subTotal += (item.quantity * double.Parse(dt.Rows[0]["ServiceCost"].ToString()));
                                // Fetch the handling fee percentage from the last record
                                handlingFeePercentage = double.Parse(dt.Rows[0]["HandlingFeePercentage"].ToString());

                                // We create a BookedService object
                                BookedService bookedService = new BookedService();
                                bookedService.serviceID = serviceID;
                                bookedService.serviceCategory = dt.Rows[0]["ServiceCategory"].ToString();
                                bookedService.serviceName = dt.Rows[0]["ServiceName"].ToString();
                                bookedService.shortDescription = dt.Rows[0]["ShortDescription"].ToString();
                                bookedService.fullDescription = dt.Rows[0]["FullDescription"].ToString();
                                bookedService.imageUrl = dt.Rows[0]["ImageUrl"].ToString();
                                bookedService.unitCost = double.Parse(dt.Rows[0]["ServiceCost"].ToString());
                                bookedService.quantity = item.quantity;
                                // Add the booked service to the list of booked services
                                listOfBookedServices.Add(bookedService);
                            }
                        }
                        da.Dispose();
                        dt.Clear();
                        conn.Close();
                    }
                }
                // Compute the handling amount due
                double handlingAmountDue = (handlingFeePercentage / 100) * subTotal;
                // Compute the total booking cost
                double totalBookingCost = subTotal + handlingAmountDue;
                // Recompute the total amount due
                totalDue = totalBookingCost - outstandingBalance;
                
                // Clear the list of read cart items
                readCartItems.Clear();

                if (totalDue <= 0)
                {
                    //** Note that this section has already been handled on the client side on this page through an ajax call to the web service
                    //** However, we are just doing this as a precaution
                    // At this point, it means the user's outstanding balance has successfully paid for all items in the cart
                    // So we debit his wallet and register his booking
                    // Debit the client's wallet
                    DebitClientWallet(outstandingBalance, totalBookingCost, outstanding_phoneNumber, outstanding_bookingID, outstanding_unlockCode);

                    // Write Wallet History
                    WriteClientWalletTransactionHistory(outstanding_bookingID, "DEBIT", totalBookingCost, "Paid for a new Booking");

                    // Next thing to do is Register the Booking. But first we should style the date in a more readable way
                    // Here, we want to style the client's choice date to show as: DD-MMM-YYYY. E.g 02-AUG-2015.
                    // This will be much easier to read
                    DateTime fullClientChoiceDate = Convert.ToDateTime(booking_choiceDate);
                    string day = fullClientChoiceDate.Day.ToString();
                    // If the day is not a 2 digit number, add a zero before the day
                    if (day.Length != 2)
                    {
                        day = "0" + day;
                    }
                    string month = fullClientChoiceDate.Month.ToString();
                    // If the month is not a 2 digit number, add a zero before the month
                    if (month.Length != 2)
                    {
                        month = "0" + month;
                    }
                    string clientChoiceDate = day + "-" + AppHelper.GetMonthName(int.Parse(month)) + "-" + fullClientChoiceDate.Year.ToString();

                    // Generate a unique booking ID and append a random number to it
                    Random myRandGen = new Random();
                    int randomNumber = myRandGen.Next(1000, 50000);
                    string bookingID = "BKN-" + MyAppSecurity.GenerateUniqueAlphaNumericString(12) + randomNumber.ToString();

                    // Generate the service delivery code of 12 characters
                    string serviceDeliveryCode = MyAppSecurity.GenerateUniqueAlphaNumericString(12);

                    // Generate the review code which the client can use to review the salon
                    string reviewCode = MyAppSecurity.GenerateUniqueAlphaNumericString(8);


                    // Register the booking. Note that the payment status is PAID because the user paid with his wallet balance. The booking status is PENDING because itb has not been attended yet since it is a new booking
                    RegisterBooking(bookingID, booking_name, booking_email, booking_phoneNumber, clientChoiceDate, booking_choiceDate, booking_choiceTime, booking_city, booking_address, booking_otherNotes, previousSalonEmail, subTotal, handlingAmountDue, totalBookingCost, "PAID", "WALLET", "PENDING", MyAppSecurity.Encrypt(serviceDeliveryCode, MyAppSecurity.GetPasswordBytes()), MyAppSecurity.Encrypt(reviewCode, MyAppSecurity.GetPasswordBytes()), MyAppSecurity.HashMD5(serviceDeliveryCode), MyAppSecurity.HashMD5(reviewCode));


                    // Save the details of each booked service
                    foreach (BookedService bookedService in listOfBookedServices)
                    {
                        SaveBookingDetails(bookingID, bookedService.serviceID, bookedService.serviceCategory, bookedService.serviceName, bookedService.shortDescription, bookedService.fullDescription, bookedService.imageUrl, bookedService.unitCost, bookedService.quantity);
                    }

                    // At this point, we are sending SMS to both the client and salon

                    // The final thing here is to take the user to the success page since he does not need to make additional payment
                    Response.Redirect("BookingSuccessful.aspx");
                }
                else
                {
                    // At this point, we know that the user has still needs to make additional payment for the items in the cart

                    // Let us check if he has any outstanding balance. This will help us know whether to debit his wallet
                    if (outstandingBalance != 0)
                    {
                        // At this point, it means the user has outstanding balance but still needs to make additional payment, so we debit his wallet and write the history
                        // Debit the client's wallet. Notice that here, the user's new wallet balance will become 0 .That is (outstandingBalance - outstandingBalance)
                        // This is because the user's current wallet balance cannot clear his payment. So it will be drained in order to reduce the amount he will pay at the payment gateway
                        DebitClientWallet(outstandingBalance, outstandingBalance, outstanding_phoneNumber, outstanding_bookingID, outstanding_unlockCode);

                        // Write Wallet History
                        WriteClientWalletTransactionHistory(outstanding_bookingID, "DEBIT", outstandingBalance, "Paid for a new Booking");
                    }

                    // Next thing to do is Register the Booking. But first we should style the date in a more readable way
                    // Here, we want to style the client's choice date to show as: DD-MMM-YYYY. E.g 02-AUG-2015.
                    // This will be much easier to read
                    DateTime fullClientChoiceDate = Convert.ToDateTime(booking_choiceDate);
                    string day = fullClientChoiceDate.Day.ToString();
                    // If the day is not a 2 digit number, add a zero before the day
                    if (day.Length != 2)
                    {
                        day = "0" + day;
                    }
                    string month = fullClientChoiceDate.Month.ToString();
                    // If the month is not a 2 digit number, add a zero before the month
                    if (month.Length != 2)
                    {
                        month = "0" + month;
                    }
                    string clientChoiceDate = day + "-" + AppHelper.GetMonthName(int.Parse(month)) + "-" + fullClientChoiceDate.Year.ToString();

                    // Generate a unique booking ID and append a random number to it
                    Random myRandGen = new Random();
                    int randomNumber = myRandGen.Next(1000, 50000);
                    string bookingID = "BKN-" + MyAppSecurity.GenerateUniqueAlphaNumericString(12) + randomNumber.ToString();

                    // Generate the service delivery code of 12 characters
                    string serviceDeliveryCode = MyAppSecurity.GenerateUniqueAlphaNumericString(12);

                    // Generate the review code which the client can use to review the salon
                    string reviewCode = MyAppSecurity.GenerateUniqueAlphaNumericString(8);

                    // Register the booking. Note that the payment status is UNPAID because the user has not made payment yet. The booking status is also INACTIVE because the user has not made payment yet. Also, we have indicated that the user is paying with her credit card
                    RegisterBooking(bookingID, booking_name, booking_email, booking_phoneNumber, clientChoiceDate, booking_choiceDate, booking_choiceTime, booking_city, booking_address, booking_otherNotes, previousSalonEmail, subTotal, handlingAmountDue, outstandingBalance, "UNPAID", "CREDIT CARD", "INACTIVE", MyAppSecurity.Encrypt(serviceDeliveryCode, MyAppSecurity.GetPasswordBytes()), MyAppSecurity.Encrypt(reviewCode, MyAppSecurity.GetPasswordBytes()), MyAppSecurity.HashMD5(serviceDeliveryCode), MyAppSecurity.HashMD5(reviewCode));

                    // Save the details of each booked service
                    foreach (BookedService bookedService in listOfBookedServices)
                    {
                        SaveBookingDetails(bookingID, bookedService.serviceID, bookedService.serviceCategory, bookedService.serviceName, bookedService.shortDescription, bookedService.fullDescription, bookedService.imageUrl, bookedService.unitCost, bookedService.quantity);
                    }

                    // The next thing is to take the user to the payment gateway
                    // Set the post values
                    string paymentGatewayUrl = "https://fidelitypaygate.fidelitybankplc.com/cipg/MerchantServices/MakePayment.aspx";
                    NameValueCollection data = new NameValueCollection();
                    data.Add("mercId", "06307");
                    data.Add("currCode", "566");
                    data.Add("amt", totalDue.ToString());
                    data.Add("orderId", bookingID);
                    data.Add("prod", "Beauty Service Booking: " + salonName);
                    data.Add("email", booking_email);
                    // Redirect the user to the payment page using HTTP POST
                    HttpHelper.RedirectAndPOST(this.Page, paymentGatewayUrl, data);
                }
                
            
            
        }

        private void DebitClientWallet(double currentOutstandingBalance, double debitAmount, string phoneNumber, string bookingID, string unlockCode)
        {
            // Ensure that the debit amount is not greater than the current wallet balance
            if (debitAmount <= currentOutstandingBalance)
            {
                string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
                SqlConnection conn;
                conn = new SqlConnection(connString);
                SqlCommand command;
                string sqlString;
                command = new SqlCommand();
                // Compute the client's new wallet balance
                double newWalletBalance = currentOutstandingBalance - debitAmount;
                // Update the client's wallet balance
                sqlString = @"UPDATE ClientWallet SET Balance = @Balance WHERE BookingID = @BookingID AND ClientPhoneNumber = @ClientPhoneNumber AND HashedUnlockCode = @HashedUnlockCode";
                // Add the parameters
                command.Parameters.AddWithValue("@BookingID", bookingID);
                command.Parameters.AddWithValue("@ClientPhoneNumber", phoneNumber);
                // Hash the unlock code and compare
                command.Parameters.AddWithValue("@HashedUnlockCode", MyAppSecurity.HashMD5(unlockCode));
                command.Parameters.AddWithValue("@Balance", newWalletBalance);
                conn.Open();
                command.Connection = conn; //Assign connection of the command
                command.CommandText = sqlString; //Assign the command text of the command
                command.ExecuteNonQuery(); //Execute the query
                conn.Close();
            }
        }

        private void WriteClientWalletTransactionHistory(string bookingID, string transactionType, double transactionAmount, string comment)
        {
            // This method writes the history of every transaction performed on a Client's Wallet

            // Get the transaction date. That is today's date
            string day = DateTime.Now.Day.ToString();
            // If the day is not a 2 digit number, add a zero before the day
            if (day.Length != 2)
            {
                day = "0" + day;
            }
            string month = DateTime.Now.Month.ToString();
            // If the month is not a 2 digit number, add a zero before the month
            if (month.Length != 2)
            {
                month = "0" + month;
            }
            // If the hour is not a 2 digit number, add a zero before the hour
            string hour = DateTime.Now.Hour.ToString();
            if (hour.Length != 2)
            {
                hour = "0" + hour;
            }
            // If the minute is not a 2 digit number, add a zero before the minute
            string minute = DateTime.Now.Minute.ToString();
            if (minute.Length != 2)
            {
                minute = "0" + minute;
            }
            string transactionDate = day + "-" + AppHelper.GetMonthName(int.Parse(month)) + "-" + DateTime.Now.Year + "  " +
                hour + ":" + minute + " " + DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);
            string numericalTransactionDate = DateTime.Now.Year + "-" + month + "-" + day;

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            //conn.Open();
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"INSERT INTO ClientWalletTransactions Values(@BookingID, @TransactionType, @TransactionAmount, @TransactionDate, @NumericalTransactionDate," +
                " @Comment)"; //the sql string 
            command.Parameters.AddWithValue("@BookingID", bookingID);
            command.Parameters.AddWithValue("@TransactionType", transactionType);
            command.Parameters.AddWithValue("@TransactionAmount", transactionAmount);
            command.Parameters.AddWithValue("@TransactionDate", transactionDate);
            command.Parameters.AddWithValue("@NumericalTransactionDate", numericalTransactionDate);
            command.Parameters.AddWithValue("@Comment", comment);
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
        }

        private void RegisterBooking(string bookingID, string booking_name, string booking_email, string booking_phoneNumber, string booking_choiceDate, string booking_numericalClientChoiceDate, string booking_choiceTime, string booking_city, string booking_address, string booking_otherNotes, string salonEmail, double subTotal, double handlingFee, double walletBalanceUsed, string paymentStatus, string paidWith, string bookingStatus, string serviceDeliveryCode, string reviewCode, string hashedServiceDeliveryCode, string hashedReviewCode)
        {
            // Get the date that this booking is made. That is today's date
            string day = DateTime.Now.Day.ToString();
            // If the day is not a 2 digit number, add a zero before the day
            if (day.Length != 2)
            {
                day = "0" + day;
            }
            string month = DateTime.Now.Month.ToString();
            // If the month is not a 2 digit number, add a zero before the month
            if (month.Length != 2)
            {
                month = "0" + month;
            }
            // If the hour is not a 2 digit number, add a zero before the hour
            string hour = DateTime.Now.Hour.ToString();
            if (hour.Length != 2)
            {
                hour = "0" + hour;
            }
            // If the minute is not a 2 digit number, add a zero before the minute
            string minute = DateTime.Now.Minute.ToString();
            if (minute.Length != 2)
            {
                minute = "0" + minute;
            }
            string dateBooked = day + "-" + AppHelper.GetMonthName(int.Parse(month)) + "-" + DateTime.Now.Year + "  " +
                hour + ":" + minute + " " + DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);
            string numericalDateBooked = DateTime.Now.Year + "-" + month + "-" + day;


            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            //conn.Open();
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"INSERT INTO Bookings Values(@BookingID,@ClientPhoneNumber, @ClientName, @ClientEmail," +
                " @ClientChoiceDate, @NumericalClientChoiceDate, @ClientChoiceTime, @City, @ClientChoiceLocation,@OtherNotes, @SalonEmail," +
                " @DateBooked, @NumericalDateBooked, @SubTotal, @HandlingFee," +
                " @WalletBalanceUsed," +
                " @PaymentStatus, @PaidWith, @ServiceDeliveryCode, @BookingStatus, @ReviewCode, @HashedServiceDeliveryCode, @HashedReviewCode)"; //the sql string        
            command.Parameters.AddWithValue("@BookingID", bookingID);
            command.Parameters.AddWithValue("@ClientPhoneNumber", booking_phoneNumber);
            command.Parameters.AddWithValue("@ClientName", booking_name);
            command.Parameters.AddWithValue("@ClientEmail", booking_email);
            command.Parameters.AddWithValue("@ClientChoiceDate", booking_choiceDate);
            command.Parameters.AddWithValue("@NumericalClientChoiceDate", booking_numericalClientChoiceDate);
            command.Parameters.AddWithValue("@ClientChoiceTime", booking_choiceTime);
            command.Parameters.AddWithValue("@City", booking_city);
            command.Parameters.AddWithValue("@ClientChoiceLocation", booking_address);
            command.Parameters.AddWithValue("@OtherNotes", booking_otherNotes);
            command.Parameters.AddWithValue("@SalonEmail", salonEmail);
            command.Parameters.AddWithValue("@DateBooked", dateBooked);
            command.Parameters.AddWithValue("@NumericalDateBooked", numericalDateBooked);
            command.Parameters.AddWithValue("@SubTotal", subTotal);
            command.Parameters.AddWithValue("@HandlingFee", handlingFee);
            command.Parameters.AddWithValue("@WalletBalanceUsed", walletBalanceUsed);
            command.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
            command.Parameters.AddWithValue("@PaidWith", paidWith);
            command.Parameters.AddWithValue("@ServiceDeliveryCode", serviceDeliveryCode);
            command.Parameters.AddWithValue("@BookingStatus", bookingStatus);
            command.Parameters.AddWithValue("@ReviewCode", reviewCode);
            command.Parameters.AddWithValue("@HashedServiceDeliveryCode", hashedServiceDeliveryCode);
            command.Parameters.AddWithValue("@HashedReviewCode", hashedReviewCode);
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();

        }

        private void SaveBookingDetails(string bookingID, int serviceID, string serviceCategory, string serviceName, string shortDescription, string fullDescription, string imageUrl, double unitCost, int quantity)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            //conn.Open();
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"INSERT INTO BookingsDetails Values(@BookingID,@ServiceID, @ServiceCategory, @ServiceName," +
                " @ShortDescription, @FullDescription, @ImageUrl, @UnitCost, @Quantity)"; //the sql string        
            command.Parameters.AddWithValue("@BookingID", bookingID);
            command.Parameters.AddWithValue("@ServiceID", serviceID);
            command.Parameters.AddWithValue("@ServiceCategory", serviceCategory);
            command.Parameters.AddWithValue("@ServiceName", serviceName);
            command.Parameters.AddWithValue("@ShortDescription", shortDescription);
            command.Parameters.AddWithValue("@FullDescription", fullDescription);
            command.Parameters.AddWithValue("@ImageUrl", imageUrl);
            command.Parameters.AddWithValue("@UnitCost", unitCost);
            command.Parameters.AddWithValue("@Quantity", quantity);
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
        }

        private double GetOutstandingBalance(string phoneNumber, string bookingID, string unlockCode)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            // Load the balance for the specified wallet details
            string selectString = @"SELECT Balance From ClientWallet WHERE BookingID = @BookingID AND ClientPhoneNumber = @ClientPhoneNumber AND HashedUnlockCode = @HashedUnlockCode";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameters
            da.SelectCommand.Parameters.AddWithValue("@BookingID", bookingID);
            da.SelectCommand.Parameters.AddWithValue("@ClientPhoneNumber", phoneNumber);
            // Hash the unlock code and compare
            da.SelectCommand.Parameters.AddWithValue("@HashedUnlockCode", MyAppSecurity.HashMD5(unlockCode));
            dt = new DataTable();
            da.Fill(dt);
            // set the default outstanding balance
            double outstandingBalance = 0;
            // Ensure a row was returned before reading values
            if (dt.Rows.Count != 0)
            {
                // Fetch the outstanding balance
                outstandingBalance = double.Parse(dt.Rows[0]["Balance"].ToString());
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
            // Return the outsanding balance
            return outstandingBalance;
        }

        //private void LoadCities(string salonUsername)
        //{
        //    string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
        //    SqlConnection conn;
        //    string selectString = @"SELECT Locations FROM Salons WHERE Username = @Username";
        //    SqlDataAdapter da;
        //    DataTable dt;
        //    conn = new SqlConnection(connString);
        //    conn.Open();
        //    da = new SqlDataAdapter(selectString, conn);
        //    da.SelectCommand.Parameters.AddWithValue("@Username", salonUsername);
        //    dt = new DataTable();
        //    da.Fill(dt);
        //    string locationsCsv = "";
        //    if (dt.Rows.Count != 0)
        //    {
        //        locationsCsv = dt.Rows[0]["Locations"].ToString();
        //    }

        //    // Split the locationsCsv into an array
        //    string[] locationsArray = locationsCsv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            
        //    //Loop through all locations in the array
        //    foreach (string location in locationsArray)
        //    {
        //        ddlChoiceCity.Items.Add(location);
        //    }

        //    // Make a default selection. Select the first item
        //    ddlChoiceCity.SelectedIndex = 0;

        //    da.Dispose();
        //    dt.Clear();
        //    conn.Close();
        //}

    }
}