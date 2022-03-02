using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

namespace Beautify.BeautifyWebService
{
    /// <summary>
    /// Summary description for BeautifyWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class BeautifyWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        // Updates a cart by increasing / decreasing the quantity of a specified cart item
        [WebMethod]
        public List<CartHtmlTable> UpdateCart(string serializedShoppingCartItems)
        {
            // Deserialize the cart items
            List<CartItem> shoppingCartItems = (List<CartItem>)JsonConvert.DeserializeObject(serializedShoppingCartItems, typeof(List<CartItem>));

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            SqlDataAdapter da;
            DataTable dt;
            string selectString = "";
            StringBuilder strShoppingCart = new StringBuilder();
            strShoppingCart.Append("<table class='table table-bordered table-vcenter'>" +
                            "<thead>" +
                                "<tr>" +
                                    "<th colspan='2'>Service</th>" +
                                    "<th class='text-center'>QTY</th>" +
                                    "<th class='text-right'>Unit Price</th>" +
                                    "<th class='text-right'>Price</th>" +
                                "</tr>" +
                            "</thead>" +
                            "<tbody>");
            int serviceID;
            double subTotal = 0;
            double totalDue = 0;
            double handlingFeePercentage = 0;

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
                        // We are doing this in case the user types in a JSON cart item manually in the cart querystring.
                        if (previousSalonEmail.Equals(dt.Rows[0]["SalonEmail"].ToString()))
                        {
                            // Append each service to the string builder
                            strShoppingCart.Append("<tr>" +
                                                "<td style='width: 200px;'>" +
                                                    "<img src='" + dt.Rows[0]["ImageUrl"].ToString() + "' alt='' class='img-circle' height='90' width='90'>" +
                                                "</td>" +
                                                "<td>" +
                                                    "<strong>" + dt.Rows[0]["ServiceName"].ToString() + "</strong><br>" +
                                                    dt.Rows[0]["ShortDescription"].ToString() + "<br>" +
                                                    "<strong class='text-success'>" + dt.Rows[0]["ServiceCategory"].ToString() + "</strong>" +
                                                "</td>" +
                                                "<td class='text-center'>" +
                                                    "<a href='javascript:void(0)' onclick='ReduceQuantity(" + serviceID + ")' class='btn btn-xs btn-danger' data-toggle='tooltip' title='Remove'><i class='fa fa-minus'></i></a>    " +
                                                "<strong>" + item.quantity + "</strong>" +                                                    
                                                    "    <a href='javascript:void(0)' onclick='IncreaseQuantity(" + serviceID + ")' class='btn btn-xs btn-success' data-toggle='tooltip' title='Add'><i class='fa fa-plus'></i></a>" +
                                                    "</td>" +
                                // Note that here, we are computing quantity * unitPrice
                                                "<td class='text-right'>" + AppHelper.GetCurrencySymbol() + " " + double.Parse(dt.Rows[0]["ServiceCost"].ToString()).ToString("N0") + "</td>" +
                                                "<td class='text-right'><strong>" + AppHelper.GetCurrencySymbol() + " " + (item.quantity * double.Parse(dt.Rows[0]["ServiceCost"].ToString())).ToString("N0") + "</strong></td>" +
                                            "</tr>");
                            // Increment the sub total
                            subTotal += (item.quantity * double.Parse(dt.Rows[0]["ServiceCost"].ToString()));
                            // Fetch the handling fee percentage from the last record
                            handlingFeePercentage = double.Parse(dt.Rows[0]["HandlingFeePercentage"].ToString());
                        }
                    }
                    da.Dispose();
                    dt.Clear();
                    conn.Close();
                }
            }
            // Compute the handling amount due
            double handlingAmountDue = (handlingFeePercentage / 100) * subTotal;
            totalDue = subTotal + handlingAmountDue;
            strShoppingCart.Append("<tr>" +
                                    "<td colspan='4' class='text-right h4'><strong>Sub Total</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + subTotal.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td colspan='4' class='text-right h4'>+ <strong>Convenience Fee (" + handlingFeePercentage + "%)</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + handlingAmountDue.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                                "<tr class='active'>" +
                                    "<td colspan='4' class='text-right text-uppercase h4'><strong>Total Amount Due</strong></td>" +
                                    "<td class='text-right text-success h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + totalDue.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                            "</tbody>" +
                        "</table>");
            
            // Clear the list of read cart items
            readCartItems.Clear();

            // Return the HTML of the cart
            List<CartHtmlTable> recList = new List<CartHtmlTable>();
            CartHtmlTable cartTable = new CartHtmlTable()
            {
                cartTable = strShoppingCart.ToString(),
                totalDue = totalDue,
                // Set the default value for outstanding balance
                outstandingBalance = 0,
                // Return an empty string here, since we are not using this value to make any decision on the calling page
                resultDescription = ""
            };

            // Add the resultRecord to the list
            recList.Add(cartTable);
            return recList;
        }

        [WebMethod]
        public List<CartHtmlTable> LoadCart(string serializedShoppingCartItems)
        {
            // Deserialize the cart items
            List<CartItem> shoppingCartItems = (List<CartItem>)JsonConvert.DeserializeObject(serializedShoppingCartItems, typeof(List<CartItem>));

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            SqlDataAdapter da;
            DataTable dt;
            string selectString = "";
            StringBuilder strShoppingCart = new StringBuilder();
            strShoppingCart.Append("<table class='table table-bordered table-vcenter'>" +
                            "<thead>" +
                                "<tr>" +
                                    "<th colspan='2'>Service</th>" +
                                    "<th class='text-center'>QTY</th>" +
                                    "<th class='text-right'>Unit Price</th>" +
                                    "<th class='text-right'>Price</th>" +
                                "</tr>" +
                            "</thead>" +
                            "<tbody>");
            int serviceID;
            double subTotal = 0;
            double totalDue = 0;
            double handlingFeePercentage = 0;

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
                        // We are doing this in case the user types in a JSON cart item manually in the cart querystring.
                        if (previousSalonEmail.Equals(dt.Rows[0]["SalonEmail"].ToString()))
                        {
                            // Append each service to the string builder
                            strShoppingCart.Append("<tr>" +
                                                "<td style='width: 200px;'>" +
                                                    "<img src='" + dt.Rows[0]["ImageUrl"].ToString() + "' alt='' class='img-circle' height='90' width='90'>" +
                                                "</td>" +
                                                "<td>" +
                                                    "<strong>" + dt.Rows[0]["ServiceName"].ToString() + "</strong><br>" +
                                                    dt.Rows[0]["ShortDescription"].ToString() + "<br>" +
                                                    "<strong class='text-success'>" + dt.Rows[0]["ServiceCategory"].ToString() + "</strong>" +
                                                "</td>" +
                                                "<td class='text-center'>" +
                                                    "<a href='javascript:void(0)' onclick='ReduceQuantity(" + serviceID + ")' class='btn btn-xs btn-danger' data-toggle='tooltip' title='Remove'><i class='fa fa-minus'></i></a>    " +
                                                "<strong>" + item.quantity + "</strong>" +
                                                    "    <a href='javascript:void(0)' onclick='IncreaseQuantity(" + serviceID + ")' class='btn btn-xs btn-success' data-toggle='tooltip' title='Add'><i class='fa fa-plus'></i></a>" +
                                                    "</td>" +
                                // Note that here, we are computing quantity * unitPrice
                                                "<td class='text-right'>" + AppHelper.GetCurrencySymbol() + " " + double.Parse(dt.Rows[0]["ServiceCost"].ToString()).ToString("N0") + "</td>" +
                                                "<td class='text-right'><strong>" + AppHelper.GetCurrencySymbol() + " " + (item.quantity * double.Parse(dt.Rows[0]["ServiceCost"].ToString())).ToString("N0") + "</strong></td>" +
                                            "</tr>");
                            // Increment the sub total
                            subTotal += (item.quantity * double.Parse(dt.Rows[0]["ServiceCost"].ToString()));
                            // Fetch the handling fee percentage from the last record
                            handlingFeePercentage = double.Parse(dt.Rows[0]["HandlingFeePercentage"].ToString());
                        }
                    }
                    da.Dispose();
                    dt.Clear();
                    conn.Close();
                }
            }
            // Compute the handling amount due
            double handlingAmountDue = (handlingFeePercentage / 100) * subTotal;
            totalDue = subTotal + handlingAmountDue;
            strShoppingCart.Append("<tr>" +
                                    "<td colspan='4' class='text-right h4'><strong>Sub Total</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + subTotal.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td colspan='4' class='text-right h4'>+ <strong>Convenience Fee (" + handlingFeePercentage + "%)</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + handlingAmountDue.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                                "<tr class='active'>" +
                                    "<td colspan='4' class='text-right text-uppercase h4'><strong>Total Amount Due</strong></td>" +
                                    "<td class='text-right text-success h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + totalDue.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                            "</tbody>" +
                        "</table>");

            // Clear the list of read cart items
            readCartItems.Clear();

            // Return the HTML of the cart
            List<CartHtmlTable> recList = new List<CartHtmlTable>();
            CartHtmlTable cartTable = new CartHtmlTable()
            {
                cartTable = strShoppingCart.ToString(),
                totalDue = totalDue,
                // Set the default value of outstanding balance
                outstandingBalance = 0,
                // Return an empty string here, since we are not using this value to make any decision on the calling page
                resultDescription = ""
            };

            // Add the resultRecord to the list
            recList.Add(cartTable);
            return recList;
        }

        [WebMethod]
        public List<CartHtmlTable> LoadCartForCheckout(string serializedShoppingCartItems)
        {
            // Deserialize the cart items
            List<CartItem> shoppingCartItems = (List<CartItem>)JsonConvert.DeserializeObject(serializedShoppingCartItems, typeof(List<CartItem>));

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            SqlDataAdapter da;
            DataTable dt;
            string selectString = "";
            StringBuilder strShoppingCart = new StringBuilder();
            strShoppingCart.Append("<table class='table table-bordered table-vcenter'>" +
                            "<thead>" +
                                "<tr>" +
                                    "<th colspan='2'>Service</th>" +
                                    "<th class='text-center'>QTY</th>" +
                                    "<th class='text-right'>Unit Price</th>" +
                                    "<th class='text-right'>Price</th>" +
                                "</tr>" +
                            "</thead>" +
                            "<tbody>");
            int serviceID;
            double subTotal = 0;
            double totalDue = 0;
            double handlingFeePercentage = 0;
            double outstandingBalance = 0;

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
                        // We are doing this in case the user types in a JSON cart item manually in the cart querystring.
                        if (previousSalonEmail.Equals(dt.Rows[0]["SalonEmail"].ToString()))
                        {
                            // Append each service to the string builder
                            strShoppingCart.Append("<tr>" +
                                                "<td style='width: 200px;'>" +
                                                    "<img src='" + dt.Rows[0]["ImageUrl"].ToString() + "' alt='' class='img-circle' height='90' width='90'>" +
                                                "</td>" +
                                                "<td>" +
                                                    "<strong>" + dt.Rows[0]["ServiceName"].ToString() + "</strong><br>" +
                                                    dt.Rows[0]["ShortDescription"].ToString() + "<br>" +
                                                    "<strong class='text-success'>" + dt.Rows[0]["ServiceCategory"].ToString() + "</strong>" +
                                                "</td>" +
                                                "<td class='text-center'>" +
                                                "<span class='label label-success'><strong>" + item.quantity + "</strong></span>" +
                                                    "</td>" +
                                // Note that here, we are computing quantity * unitPrice
                                                "<td class='text-right'>" + AppHelper.GetCurrencySymbol() + " " + double.Parse(dt.Rows[0]["ServiceCost"].ToString()).ToString("N0") + "</td>" +
                                                "<td class='text-right'><strong>" + AppHelper.GetCurrencySymbol() + " " + (item.quantity * double.Parse(dt.Rows[0]["ServiceCost"].ToString())).ToString("N0") + "</strong></td>" +
                                            "</tr>");
                            // Increment the sub total
                            subTotal += (item.quantity * double.Parse(dt.Rows[0]["ServiceCost"].ToString()));
                            // Fetch the handling fee percentage from the last record
                            handlingFeePercentage = double.Parse(dt.Rows[0]["HandlingFeePercentage"].ToString());
                        }
                    }
                    da.Dispose();
                    dt.Clear();
                    conn.Close();
                }
            }
            // Compute the handling amount due
            double handlingAmountDue = (handlingFeePercentage / 100) * subTotal;
            totalDue = subTotal + handlingAmountDue;
            strShoppingCart.Append("<tr>" +
                                    "<td colspan='4' class='text-right h4'><strong>Sub Total</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + subTotal.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td colspan='4' class='text-right h4'>+ <strong>Convenience Fee (" + handlingFeePercentage + "%)</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + handlingAmountDue.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td colspan='4' class='text-right h4'>- <strong>Outstanding Balance</strong></td>" +
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + outstandingBalance.ToString("N2") + "</strong></td>" +
                                "</tr>" +
                                "<tr class='active'>" +
                                    "<td colspan='4' class='text-right text-uppercase h4'><strong>Total Amount Due</strong></td>" +
                                    "<td class='text-right text-success h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + totalDue.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                            "</tbody>" +
                        "</table>");

            // Clear the list of read cart items
            readCartItems.Clear();

            // Return the HTML of the cart
            List<CartHtmlTable> recList = new List<CartHtmlTable>();
            CartHtmlTable cartTable = new CartHtmlTable()
            {
                cartTable = strShoppingCart.ToString(),
                totalDue = totalDue,
                outstandingBalance = outstandingBalance,
                // Return an empty string here, since we are not using this value to make any decision on the calling page
                resultDescription = ""
            };

            // Add the resultRecord to the list
            recList.Add(cartTable);
            return recList;
        }

        /*[WebMethod]
        public void SendSMS(string message)
        {
            AppHelper.SendSMS(message + Environment.NewLine + "another line","08065891644");
        }*/

        [WebMethod]
        public List<CartHtmlTable> ApplyOutstandingBalance(string serializedShoppingCartItems, string outstanding_phoneNumber, string outstanding_bookingID, string outstanding_unlockCode, string booking_name, string booking_email, string booking_phoneNumber, string booking_choiceDate, string booking_choiceTime, string booking_city, string booking_address, string booking_otherNotes)
        {
            // Fetch the outstanding balance for the specified wallet details
            double outstandingBalance = GetOutstandingBalance(outstanding_phoneNumber, outstanding_bookingID, outstanding_unlockCode);
            double totalDue = 0;
            StringBuilder strShoppingCart = new StringBuilder();
                
            // Check whether the user has an outstanding balance
            if (outstandingBalance != 0)
            {
                // At this point, we know that the user has outsanding balance. So we recompute the cart for checkout

                // Deserialize the cart items
                List<CartItem> shoppingCartItems = (List<CartItem>)JsonConvert.DeserializeObject(serializedShoppingCartItems, typeof(List<CartItem>));

                string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
                SqlConnection conn;
                SqlDataAdapter da;
                DataTable dt;
                string selectString = "";
                strShoppingCart.Append("<table class='table table-bordered table-vcenter'>" +
                                "<thead>" +
                                    "<tr>" +
                                        "<th colspan='2'>Service</th>" +
                                        "<th class='text-center'>QTY</th>" +
                                        "<th class='text-right'>Unit Price</th>" +
                                        "<th class='text-right'>Price</th>" +
                                    "</tr>" +
                                "</thead>" +
                                "<tbody>");
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
                                // Append each service to the string builder
                                strShoppingCart.Append("<tr>" +
                                                    "<td style='width: 200px;'>" +
                                                        "<img src='" + dt.Rows[0]["ImageUrl"].ToString() + "' alt='' style='width: 180px;'>" +
                                                    "</td>" +
                                                    "<td>" +
                                                        "<strong>" + dt.Rows[0]["ServiceName"].ToString() + "</strong><br>" +
                                                        dt.Rows[0]["ShortDescription"].ToString() + "<br>" +
                                                        "<strong class='text-success'>" + dt.Rows[0]["ServiceCategory"].ToString() + "</strong>" +
                                                    "</td>" +
                                                    "<td class='text-center'>" +
                                                    "<span class='label label-success'><strong>" + item.quantity + "</strong></span>" +
                                                        "</td>" +
                                    // Note that here, we are computing quantity * unitPrice
                                                    "<td class='text-right'>" + AppHelper.GetCurrencySymbol() + " " + double.Parse(dt.Rows[0]["ServiceCost"].ToString()).ToString("N0") + "</td>" +
                                                    "<td class='text-right'><strong>" + AppHelper.GetCurrencySymbol() + " " + (item.quantity * double.Parse(dt.Rows[0]["ServiceCost"].ToString())).ToString("N0") + "</strong></td>" +
                                                "</tr>");
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
                strShoppingCart.Append("<tr>" +
                                        "<td colspan='4' class='text-right h4'><strong>Sub Total</strong></td>" +
                                        "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + subTotal.ToString("N0") + "</strong></td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td colspan='4' class='text-right h4'>+ <strong>Convenience Fee (" + handlingFeePercentage + "%)</strong></td>" +
                                        "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + handlingAmountDue.ToString("N0") + "</strong></td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td colspan='4' class='text-right h4'>- <strong>Outstanding Balance</strong></td>" +
                                        "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + outstandingBalance.ToString("N2") + "</strong></td>" +
                                    "</tr>" +
                                    "<tr class='active'>" +
                                        "<td colspan='4' class='text-right text-uppercase h4'><strong>Total Amount Due</strong></td>" +
                                        "<td class='text-right text-success h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + totalDue.ToString("N0") + "</strong></td>" +
                                    "</tr>" +
                                "</tbody>" +
                            "</table>");

                // Clear the list of read cart items
                readCartItems.Clear();

                if (totalDue <= 0)
                {
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
                    RegisterBooking(bookingID, booking_name, booking_email, booking_phoneNumber, clientChoiceDate, booking_choiceDate, booking_choiceTime, booking_city, booking_address, booking_otherNotes, previousSalonEmail, subTotal, handlingAmountDue, totalBookingCost, "PAID", "WALLET", "PENDING",MyAppSecurity.Encrypt(serviceDeliveryCode, MyAppSecurity.GetPasswordBytes()),MyAppSecurity.Encrypt(reviewCode, MyAppSecurity.GetPasswordBytes()),MyAppSecurity.HashMD5(serviceDeliveryCode),MyAppSecurity.HashMD5(reviewCode));

                    // Save the details of each booked service
                    foreach (BookedService bookedService in listOfBookedServices)
                    {
                        SaveBookingDetails(bookingID, bookedService.serviceID, bookedService.serviceCategory, bookedService.serviceName, bookedService.shortDescription, bookedService.fullDescription, bookedService.imageUrl, bookedService.unitCost, bookedService.quantity);
                    }

                    // Fetch Beautify Phone number
                    string beautifyPhoneNumber = System.Configuration.ConfigurationManager.AppSettings["beautifyPhoneNumber"];
                    
                    // Fetch the salon info
                    SalonInfo sal = new SalonInfo();
                    sal = GetSalonInfo(previousSalonEmail);

                    // At this point, we compose the SMS message to send to the client, salon and admin
                    string messageToClient = @"Your booking details:\n" + sal.salonName + "(" +sal.phoneNumber + ")\nBookingID: " + bookingID + "\nReviewCode: " + reviewCode +
                        "\nServiceDeliveryCode: " + serviceDeliveryCode + "\nCost: " + AppHelper.GetCurrencySymbol() + " " + subTotal.ToString("N0") + "\nVisit www.beautify.com.ng/book/" + sal.userName + " to view the address of the salon.\nThank you for choosing www.beautify.com.ng \nFor enquiries contact us on " + beautifyPhoneNumber;

                    string messageToSalon = @"Hi, " + sal.userName + ", you have a new booking from " + booking_name + "(" + booking_phoneNumber + ") Visit www.beautify.com.ng to view. \nFor enquiries contact us on " + beautifyPhoneNumber;

                    string messageToAdmin = @"A new booking has been placed on the site. Visit www.beautify.com.ng to view";

                    // Here we are sending the SMS
                    // Send SMS to client
                    AppHelper.SendSMS(messageToClient, booking_phoneNumber);
                    // Send SMS to Salon
                    AppHelper.SendSMS(messageToSalon, sal.phoneNumber);
                    // Send SMS to Admin
                    AppHelper.SendSMS(messageToAdmin, beautifyPhoneNumber);

                    // Return the HTML of the cart
                    List<CartHtmlTable> recList = new List<CartHtmlTable>();
                    CartHtmlTable cartTable = new CartHtmlTable()
                    {
                        cartTable = strShoppingCart.ToString(),
                        totalDue = totalDue,
                        outstandingBalance = outstandingBalance,
                        resultDescription = "Payment Cleared"
                    };

                    // Add the resultRecord to the list
                    recList.Add(cartTable);
                    return recList;
                }
                else
                {
                    // At this point, we know that the user has some outstanding balance but still needs to make additional payment for the items in the cart
                    
                    // Return the HTML of the cart
                    List<CartHtmlTable> recList = new List<CartHtmlTable>();
                    CartHtmlTable cartTable = new CartHtmlTable()
                    {
                        cartTable = strShoppingCart.ToString(),
                        totalDue = totalDue,
                        outstandingBalance = outstandingBalance,
                        resultDescription = "Needs To Make Additional Payment"
                    };

                    // Add the resultRecord to the list
                    recList.Add(cartTable);
                    return recList;
                }
            }
            else
            {
                // At this point, it means the user does not have any outstanding balance. So we don't need to recompute the cart 
                // Return the an empty string HTML of the cart since we don't need to update the cart
                List<CartHtmlTable> recList = new List<CartHtmlTable>();
                CartHtmlTable cartTable = new CartHtmlTable()
                {
                    cartTable = strShoppingCart.ToString(),
                    totalDue = totalDue,                    
                    outstandingBalance = outstandingBalance,
                    resultDescription = "No Outstanding Balance"
                };

                // Add the resultRecord to the list
                recList.Add(cartTable);
                return recList;
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
            command.Parameters.AddWithValue("@NumericalTransactionDate",numericalTransactionDate);
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

        
        
        /// <summary>
        /// Loads details of a specified salon
        /// </summary>
        /// <param name="salon">The salon</param>
        [WebMethod]
        public List<SalonInfoForCheckout> LoadSalonInfoForCheckout(string salon)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            string searchDescription = "";
            // A city was specified. So we load salons in that city. Note that only salons with ACTIVE rent will be returned
            selectString = @";with SalonInfo as" +
                " (select Salons.*, isnull(sum(RatingsAndReviews.Rating),0) as TotalRating, COUNT(RatingsAndReviews.Rating) as NumberOfVotes from " +
                "salons left JOIN RatingsAndReviews ON RatingsAndReviews.SalonEmail = Salons.Email where Salons.Username = @Username " +
                "AND Salons.RentStatus = 'ACTIVE' group by salons.Username, salons.BookingStatus, salons.DateRegistered, salons.Email, salons.ImageUrl," +
                " salons.Locations, salons.NumericalDateRegistered, salons.RentStatus, salons.SalonName, RatingsAndReviews.Rating, salons.About, salons.OpeningTime, salons.ClosingTime, salons.DisabledDays, salons.BankName, salons.AccountName, salons.AccountNumber, salons.PhoneNumber, salons.LocationsInCities) select SalonInfo.Username, SalonInfo.About, SalonInfo.OpeningTime, SalonInfo.ClosingTime, SalonInfo.DisabledDays, " +
                "SalonInfo.Email, SalonInfo.SalonName, SalonInfo.BookingStatus, SalonInfo.DateRegistered, SalonInfo.ImageUrl, SalonInfo.Locations, " +
                "SalonInfo.NumericalDateRegistered, SalonInfo.RentStatus, sum(totalrating) as TotalRating, sum(NumberOfVotes) as NumberOfVotes from " +
                "SalonInfo group by SalonInfo.Username, SalonInfo.BookingStatus, SalonInfo.DateRegistered, SalonInfo.Email, SalonInfo.ImageUrl, SalonInfo.Locations, " +
                "SalonInfo.NumericalDateRegistered, SalonInfo.RentStatus,SalonInfo.SalonName, SalonInfo.About, SalonInfo.OpeningTime, SalonInfo.ClosingTime, SalonInfo.DisabledDays";
            // A description of the search
            searchDescription = " found in " + salon;


            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the Username parameter
            da.SelectCommand.Parameters.AddWithValue("@Username", salon);
            dt = new DataTable();
            da.Fill(dt);
            // Set the default opening and closing times
            string openingTime = "7:00am";
            string closingTime = "5:00pm";
            string locations = "";
            string disabledDays = "";

            StringBuilder strSalonInfo = new StringBuilder();

            // Ensure a salon is found before reading values
            if (dt.Rows.Count != 0)
            {
                // Compute the average rating for the current salon
                double averageRating = 0;
                // We should only recompute the average if the NumberOfVotes != 0 to avoid a division by 0 error
                if (int.Parse(dt.Rows[0]["NumberOfVotes"].ToString()) != 0)
                {
                    averageRating = double.Parse(dt.Rows[0]["TotalRating"].ToString()) / int.Parse(dt.Rows[0]["NumberOfVotes"].ToString());
                }

                
                
                // Add the current salon.
                strSalonInfo.Append("<div class='text-center'>" +
                                                "<img src='" + dt.Rows[0]["ImageUrl"].ToString() + "' height='160px' width='160px' alt='' class='img-circle'>" +
                    "<h1 class='animation-slideDown'><strong>" + dt.Rows[0]["SalonName"].ToString() + "</strong></h1>" +
                                                "<input class='rating' min='0' max='5' step='0.1' data-size='xs' data-symbol='&#xf005;' data-glyphicon='false' " +
                                                    "data-rating-class='rating-fa' value='" + Math.Round(Convert.ToDecimal(averageRating), 1) + "' disabled='disabled'>" +
               "<label>" + FormatTextSingularAndPlural(int.Parse(dt.Rows[0]["NumberOfVotes"].ToString()), "Vote", "", " ") + "</label>" +
                   "</div>" +
                                           "<table class='table table-borderless table-striped table-vcenter'>" +
                                                "<tbody>" +
                                                    "<tr>" +
                                                        "<td style='width: 120px;'><strong>Available in</strong></td>" +
                    // Here we are giving a space after each item in the location CSV
                                                        "<td>" + dt.Rows[0]["Locations"].ToString().Replace(",", ", ") + "</td>" +
                                                    "</tr>" +
                                                    "<tr>" +
                                                        "<td><strong>Working Days</strong></td>" +
                                                        "<td>Everyday" + FormatCsvToShowPreceedingText(" except ", dt.Rows[0]["DisabledDays"].ToString()) + "</td>" +
                                                    "</tr>" +
                                                    "<tr>" +
                                                        "<td style='width: 120px;'><strong>Working Hours</strong></td>" +
                                                        "<td>" + dt.Rows[0]["OpeningTime"].ToString() + " to " + dt.Rows[0]["ClosingTime"].ToString() + "</td>" +
                                                    "</tr>" +
                                                "<tr>" +
                                                        "<td><strong>Attended Bookings</strong></td>" +
                                                        "<td>" + FormatTextSingularAndPlural(GetNumberOfBookings(dt.Rows[0]["Email"].ToString()), "Booking", "", " ") + "</td>" +
                                                    "</tr>" +
                                                    "</tbody>" +
                                            "</table>");

                // Style the Booking status based on the salon's setting
                if (dt.Rows[0]["BookingStatus"].ToString().Equals("ENABLED"))
                {
                    strSalonInfo.Append("<div class='alert alert-success'>" +
                        "<h4><i class='fa fa-check-circle'></i> Available for Booking</h4> This salon is currently available for booking." +
                        "</div>");
                }
                else
                {
                    strSalonInfo.Append("<div class='alert alert-danger'>" +
                        "<h4><i class='fa fa-times-circle'></i> Not Available for Booking</h4> This salon is currently not available for booking. Kindly Go Back and choose a different salon." +
                        "</div>");
                }

                // Fetch the opening and closing time for this salon
                openingTime = dt.Rows[0]["OpeningTime"].ToString();
                closingTime = dt.Rows[0]["ClosingTime"].ToString();

                // Fetch the locations where this salon is available
                locations = dt.Rows[0]["Locations"].ToString();

                // Fetch the days that this salon has disabled
                disabledDays = dt.Rows[0]["DisabledDays"].ToString();
            }
            else
            {
                // The salon was not found
                
            }

            
            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the Salon info for checkout
            List<SalonInfoForCheckout> recList = new List<SalonInfoForCheckout>();
            SalonInfoForCheckout salonInfo = new SalonInfoForCheckout()
            {
                salonInfo = strSalonInfo.ToString(),
                openingTime = openingTime,
                closingTime = closingTime,
                locations = locations,
                disabledDays = disabledDays
            };

            // Add the resultRecord to the list
            recList.Add(salonInfo);
            return recList;
        }

        /// <summary>
        /// Returns the number of ATTENDED bookings for a salon
        /// </summary>
        /// <param name="salonEmail">The salon's email adrress</param>
        /// <returns>The number of ATTENDED bookings for a salon</returns>
        private int GetNumberOfBookings(string salonEmail)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT COUNT(BookingID) as NumberOfBookings from Bookings WHERE SalonEmail = @SalonEmail AND BookingStatus = 'ATTENDED'";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);
            dt = new DataTable();
            da.Fill(dt);
            int numberOfBookings = 0;
            if (dt.Rows.Count != 0)
            {
                numberOfBookings = int.Parse(dt.Rows[0]["NumberOfBookings"].ToString());
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
            // Return the number of bookings
            return numberOfBookings;
        }

        private string FormatTextSingularAndPlural(int quantity, string singularText, string leadingText, string betweenText)
        {

            if (quantity <= 1)
            {
                // The quantity is less than or equal to 1 so the singular form of the text should be returned
                return leadingText + quantity.ToString("N0") + betweenText + singularText;
            }
            else
            {
                // The quantity is greater than 1 so the plural form of the text should be returned
                if (singularText.Substring(singularText.Length - 1).Equals("y"))
                {
                    // The singular text ends with y, so let us change it to ies
                    // But first, we will remove the last letter which is y
                    singularText = singularText.Substring(0, singularText.Length - 1);
                    return leadingText + quantity.ToString("N0") + betweenText + singularText + "ies";
                }
                else
                {
                    // The singular form does not end with y, so let us append an s to it
                    return leadingText + quantity.ToString("N0") + betweenText + singularText + "s";
                }
            }
        }

        private int GetNumberOfCommaSeparatedValues(string csv)
        {
            string[] strCsvArray = csv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            // Return the length of the array(That is the number of items)
            return strCsvArray.Length;
        }

        private string FormatCsvToShowPreceedingText(string preceedingText, string csv)
        {
            int numberOfCsvItems = GetNumberOfCommaSeparatedValues(csv);
            // This is the default result. Give a space after each CSV item
            string result = preceedingText + csv.Replace(",", ", ");

            // If the number of csv items ==0, we return only the csv
            if (numberOfCsvItems == 0)
            {
                // Give a space after each CSV item
                result = csv.Replace(",", ", ");
            }
            // Return the result
            return result;
        }

        public class CartHtmlTable
        {
            public string cartTable { get; set; }
            public double totalDue { get; set; }
            public double outstandingBalance { get; set; }
            public string resultDescription { get; set; }
        }

        public class SalonInfoForCheckout
        {
            public string salonInfo { get; set; }
            public string openingTime { get; set; }
            public string closingTime { get; set; }
            public string locations { get; set; }
            public string disabledDays { get; set; }
        }

        

        [WebMethod]
        public List<ReviewStatus> SubmitReview(string bookingID, string reviewCode, double rating, string comment, string UsernameOfSalonCurrentlyBeingViewedByClient)
        {
            // Fetch info of the booked salon
            BookedSalonInfo bookedSalonInfo = GetBookedSalonEmail(bookingID, reviewCode);
            // Check whether the booked salon's email is null or empty
            if (bookedSalonInfo.email == null || bookedSalonInfo.email.Equals(""))
            {
                // At this point it means the salon email is null or empty
                // So this implies that the salon's email was not found either because payment has not been made for the booking or the bookingID specified is incorrect
                // Return the status of the review
                List<ReviewStatus> recList = new List<ReviewStatus>();
                ReviewStatus revStatus = new ReviewStatus()
                {
                    status = "Failed"
                };

                // Add the resultRecord to the list
                recList.Add(revStatus);
                return recList;
            }
            else
            {
                // At this point, we know that the salon's email was fetched. 
                // Now we check whether the booked salon's username is equal to the username of the salon currently being viewed by the client
                // Note: We are not allowing the client to review salon A from salon B's page
                if (bookedSalonInfo.username.Equals(UsernameOfSalonCurrentlyBeingViewedByClient))
                {
                    // At this point, everything is ok. So we can proceed to add the review
                    try
                    {
                        // So we proceed to add the review
                        AddReview(bookingID, bookedSalonInfo.email, rating, comment);
                        // Return the status of the review
                        List<ReviewStatus> recList = new List<ReviewStatus>();
                        ReviewStatus revStatus = new ReviewStatus()
                        {
                            status = "Success"
                        };

                        // Add the resultRecord to the list
                        recList.Add(revStatus);
                        return recList;
                    }
                    catch
                    {
                        // The common error that is likely to occur is an SQL Exception.
                        // An error will occur if the user tries to use a bookingID to review a salon more than once. 
                        // This is because BookingID is the primary field in the ratings table 'RatingsAndReviews'
                        // So we should send back a response to the user
                        // Return the status of the review
                        List<ReviewStatus> recList = new List<ReviewStatus>();
                        ReviewStatus revStatus = new ReviewStatus()
                        {
                            status = "Existing"
                        };

                        // Add the resultRecord to the list
                        recList.Add(revStatus);
                        return recList;
                    }
                }
                else
                {
                    // At this point, it means the username of the booked salon is different from the username of the salon currently being viewed by tthe client
                    // That is, the Booking ID supplied by the client does not belong to the salon he is currently viewing
                    // Note: We are not allowing the client to review salon A from salon B's page
                    // So we should return a Failed status
                    List<ReviewStatus> recList = new List<ReviewStatus>();
                    ReviewStatus revStatus = new ReviewStatus()
                    {
                        status = "Failed"
                    };

                    // Add the resultRecord to the list
                    recList.Add(revStatus);
                    return recList;
                }
                
            }
        }

        private BookedSalonInfo GetBookedSalonEmail(string bookingID, string reviewCode)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            // Note that we are looking for the salon's username and email where payment has been made. 
            // This will ensure that we do not fetch those bookings that have not been paid for
            string selectString = @"select DISTINCT Salons.Username, Bookings.SalonEmail from Salons INNER JOIN Bookings ON Salons.Email = Bookings.SalonEmail WHERE Bookings.BookingID = @BookingID AND Bookings.PaymentStatus = 'PAID' AND Bookings.HashedReviewCode = @HashedReviewCode";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            da.SelectCommand.Parameters.AddWithValue("@BookingID", bookingID);
            // Hash the review code and compare
            da.SelectCommand.Parameters.AddWithValue("@HashedReviewCode", MyAppSecurity.HashMD5(reviewCode));
            dt = new DataTable();
            da.Fill(dt);
            BookedSalonInfo bookedSalonInfo = new BookedSalonInfo();
            if (dt.Rows.Count != 0)
            {
                bookedSalonInfo.email = dt.Rows[0]["SalonEmail"].ToString();
                bookedSalonInfo.username = dt.Rows[0]["Username"].ToString();
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
            // Return the booked salon info
            return bookedSalonInfo; ;
        }

        private void AddReview(string bookingID, string salonEmail, double rating, string comment)
        {
            // Get the review date. That is today's date
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
            string reviewDate = day + "-" + AppHelper.GetMonthName(int.Parse(month)) + "-" + DateTime.Now.Year + "  " +
                hour + ":" + minute + " " + DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);
            string numericalReviewDate = DateTime.Now.Year + "-" + month + "-" + day;

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            //conn.Open();
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"INSERT INTO RatingsAndReviews Values(@BookingID, @SalonEmail, @Rating, @Comment, @ReviewDate, @NumericalReviewDate)"; //the sql string 
            command.Parameters.AddWithValue("@BookingID", bookingID);
            command.Parameters.AddWithValue("@SalonEmail", salonEmail);
            command.Parameters.AddWithValue("@Rating", rating);
            command.Parameters.AddWithValue("@Comment", comment);
            command.Parameters.AddWithValue("@ReviewDate", reviewDate);
            command.Parameters.AddWithValue("@NumericalReviewDate", numericalReviewDate);
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
        }

        public class ReviewStatus
        {
            public string status { get; set; }
        }


        [WebMethod]
        public List<ServiceDeliveryResult> SubmitServiceDeliveryCode(string bookingID, string serviceDeliveryCode)
        {
            // Fetch the booking status
            string bookingStatus = GetBookingStatus(bookingID);

            // Switch the booking status so we can retunr the appropriate result
            switch (bookingStatus.ToUpper())
            {
                case "ATTENDED":
                    // The salon has already marked this booking as attended
                    // Return a response
                    List<ServiceDeliveryResult> attendedRecList = new List<ServiceDeliveryResult>();
                    ServiceDeliveryResult attendedResult = new ServiceDeliveryResult()
                    {
                        result = "ATTENDED"
                    };

                    // Add the resultRecord to the list
                    attendedRecList.Add(attendedResult);
                    return attendedRecList;                                                      
                case "UNATTENDED":
                    // The salon did not attend this booking and should not be allowed to change the status
                    // Return a response
                    List<ServiceDeliveryResult> unattendedRecList = new List<ServiceDeliveryResult>();
                    ServiceDeliveryResult unattendedResult = new ServiceDeliveryResult()
                    {
                        result = "UNATTENDED"
                    };

                    // Add the resultRecord to the list
                    unattendedRecList.Add(unattendedResult);
                    return unattendedRecList;                    
                case "CANCELLED":
                    // The client cancelled the booking and hence, the salon should not be allowed to change the status
                    // Return a response
                    List<ServiceDeliveryResult> cancelledRecList = new List<ServiceDeliveryResult>();
                    ServiceDeliveryResult cancelledResult = new ServiceDeliveryResult()
                    {
                        result = "CANCELLED"
                    };

                    // Add the resultRecord to the list
                    cancelledRecList.Add(cancelledResult);
                    return cancelledRecList;
                case "PENDING":
                    // The salon should be allowed to change the status to ATTENDED
                    // But first, let us validate the service delivery code
                    if (ValidateServiceDeliveryCode(bookingID, serviceDeliveryCode) == true)
                    {
                        // At this point, we know that the specified service delivery code is VALID
                        // So we proceed to update the Booking status to ATTENDED
                        UpdateBookingStatus(bookingID, "ATTENDED");

                        // Record this salon's earning (from this service) in the database
                        RecordSalonEarning(bookingID);

                        // Return a response
                        List<ServiceDeliveryResult> successRecList = new List<ServiceDeliveryResult>();
                        ServiceDeliveryResult successResult = new ServiceDeliveryResult()
                        {
                            result = "SUCCESS"
                        };

                        // Add the resultRecord to the list
                        successRecList.Add(successResult);
                        return successRecList;
                    }
                    else
                    {
                        // At this point, we know that the specified service delivery code is INVALID
                        // Return a response
                        List<ServiceDeliveryResult> failedRecList = new List<ServiceDeliveryResult>();
                        ServiceDeliveryResult failedResult = new ServiceDeliveryResult()
                        {
                            result = "FAILED"
                        };

                        // Add the resultRecord to the list
                        failedRecList.Add(failedResult);
                        return failedRecList;
                    }   
                default:
                    // Return a response
                    List<ServiceDeliveryResult> defaultRecList = new List<ServiceDeliveryResult>();
                    ServiceDeliveryResult defaultResult = new ServiceDeliveryResult()
                    {
                        result = "FAILED"
                    };
                    // Add the resultRecord to the list
                    defaultRecList.Add(defaultResult);
                    return defaultRecList;
            }
        }

        
        private void RecordSalonEarning(string bookingID)
        {
            // Get Booking Info.E.g salonEmail, subTotal
            BookingInfo bookInfo = GetBookingInfo(bookingID, "ATTENDED");

            // Get serviceCommissionPercent
            int serviceCommissionPercent = GetPlatformCommissionPercent("SalonServiceCommission");
            double serviceCommission = (double.Parse(serviceCommissionPercent.ToString()) / 100) * bookInfo.subTotal;

            // Get Salon Account info
            SalonInfo sal = GetSalonInfo(bookInfo.salonEmail);

            // Add the salon's earning to the database
            // Note that the earning status is set to UNPAID because the salon has not yet been paid
            AddSalonEarning(bookingID, bookInfo.clientName, bookInfo.salonEmail, bookInfo.subTotal, serviceCommissionPercent, serviceCommission, sal.bankName, sal.accountName, MyAppSecurity.Encrypt(sal.accountNumber, MyAppSecurity.GetPasswordBytes()), "UNPAID", "", "");

        }

        private void AddSalonEarning(string bookingID, string clientName, string salonEmail, double serviceCost, int serviceCommissionPercent, double serviceCommission, string bankName, string accountName, string accountNumber, string earningStatus, string datePaid, string numericalDatePaid)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            //conn.Open();
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"INSERT INTO SalonEarnings Values(@BookingID,@ClientName, @SalonEmail," +
                " @ServiceCost, @ServiceCommissionPercent, @ServiceCommission, @BankName, @AccountName,@AccountNumber, @EarningPaymentStatus," +
                " @DatePaid, @NumericalDatePaid)"; //the sql string        
            command.Parameters.AddWithValue("@BookingID", bookingID);
            command.Parameters.AddWithValue("@ClientName", clientName);
            command.Parameters.AddWithValue("@SalonEmail", salonEmail);
            command.Parameters.AddWithValue("@ServiceCost", serviceCost);
            command.Parameters.AddWithValue("@ServiceCommissionPercent", serviceCommissionPercent);
            command.Parameters.AddWithValue("@ServiceCommission", serviceCommission);
            command.Parameters.AddWithValue("@BankName", bankName);
            command.Parameters.AddWithValue("@AccountName", accountName);
            command.Parameters.AddWithValue("@AccountNumber", accountNumber);
            command.Parameters.AddWithValue("@EarningPaymentStatus", earningStatus);
            command.Parameters.AddWithValue("@DatePaid", datePaid);
            command.Parameters.AddWithValue("@NumericalDatePaid", numericalDatePaid);
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
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

        private int GetPlatformCommissionPercent(string settingName)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            // Note that we are selecting only HandlingFeePercentage for the specified settingName
            selectString = @"select HandlingFeePercentage from PlatformCharges WHERE SettingName = @SettingName";

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameter
            da.SelectCommand.Parameters.AddWithValue("@SettingName", settingName);

            dt = new DataTable();
            da.Fill(dt);
            int commissionPercent = 0;
            // Ensure a record was returned
            if (dt.Rows.Count != 0)
            {
                commissionPercent = int.Parse(dt.Rows[0]["HandlingFeePercentage"].ToString());
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the commission percent
            return commissionPercent;
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

        private string GetBookingStatus(string bookingID)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT BookingStatus FROM Bookings WHERE BookingID = @BookingID AND PaymentStatus = 'PAID'";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameters
            da.SelectCommand.Parameters.AddWithValue("@BookingID", bookingID);
            dt = new DataTable();
            da.Fill(dt);
            string bookingStatus = "";
            
            // Ensure a record was returned before attempting to read
            if (dt.Rows.Count != 0)
            {
                bookingStatus = dt.Rows[0]["BookingStatus"].ToString();
            }

            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the booking status
            return bookingStatus;
        }

        private bool ValidateServiceDeliveryCode(string bookingID, string serviceDeliveryCode)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT ServiceDeliveryCode FROM Bookings WHERE BookingID = @BookingID AND HashedServiceDeliveryCode = @HashedServiceDeliveryCode AND PaymentStatus = 'PAID'";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameters
            da.SelectCommand.Parameters.AddWithValue("@BookingID", bookingID);
            // Hash the service delivery code (uppercase) and compare
            da.SelectCommand.Parameters.AddWithValue("@HashedServiceDeliveryCode", MyAppSecurity.HashMD5(serviceDeliveryCode.ToUpper()));
            dt = new DataTable();
            da.Fill(dt);
            bool isValidCode = false;
            if (dt.Rows.Count != 0)
            {
                // At this point, it means a record was found.That is, the specified service delivery code is valid for this booking
                isValidCode = true;
            }
            else
            {
                // The specified service delivery code is not valid for this booking
                isValidCode = false;
            }
            da.Dispose();
            dt.Clear();
            conn.Close();

            // Return the status 
            return isValidCode;
        }

        private void UpdateBookingStatus(string bookingID, string status)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            // Update the booking status
            sqlString = @"UPDATE Bookings SET BookingStatus = @BookingStatus WHERE BookingID = @BookingID AND PaymentStatus = 'PAID'";
            // Add the parameters
            command.Parameters.AddWithValue("@BookingID", bookingID);
            command.Parameters.AddWithValue("@BookingStatus", status);
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
        }

        public class ServiceDeliveryResult
        {
            public string result { get; set; }
        }

        #region Resize Image
        /// <summary>
        /// Resizes an image in memory
        /// </summary>
        /// <param name="imagePath">The path of the image to resize</param>
        /// <returns>Returns the image url of the resized image</returns>
        private string ResizeImage(string imagePath)
        {
            string path = Server.MapPath("~/" + imagePath);
            System.Drawing.Image image = System.Drawing.Image.FromFile(path);
            using (System.Drawing.Image thumbnail = image.GetThumbnailImage(720, 480, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    thumbnail.Save(memoryStream, ImageFormat.Png);
                    Byte[] bytes = new Byte[memoryStream.Length];
                    memoryStream.Position = 0;
                    memoryStream.Read(bytes, 0, (int)bytes.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    string imageUrl = "data:image/png;base64," + base64String;
                    // Return the image url of the resized image
                    return imageUrl;
                }
            }
        }

        public bool ThumbnailCallback()
        {
            return false;
        }
        #endregion
    }
}
