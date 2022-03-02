using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Beautify
{
    public partial class WebForm4 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*if (!Page.IsPostBack)
            {
                try
                {
                    // Check whether the user has a cart
                    if (Request.QueryString["cart"] != null)
                    {
                        List<CartItem> deserializedCart = (List<CartItem>)JsonConvert.DeserializeObject(Request["cart"], typeof(List<CartItem>));
                        // Load the shopping cart
                        LoadShoppingCart(deserializedCart);
                    }
                    else
                    {
                        // The user does not have a cart so let us take her to the home page
                        Response.Redirect("Default.aspx");
                    }
                }
                catch
                {
                    // Take the user to the home page if an error occurs
                    Response.Redirect("Default.aspx");
                }
            }*/

            /*if (!Page.IsPostBack)
            {
                if (Request.QueryString["salon"] != null)
                {
                    // The salon query parameter was supplied
                    // Don't do anything
                }
                else
                {
                    // The salon query parameter was not supplied
                    // Take the user to the home page
                    Response.Redirect("Default.aspx");
                }

            }*/
        }

        /*private void LoadShoppingCart(List<CartItem> shoppingCartItems)
        {
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
                                                    "<img src='" + dt.Rows[0]["ImageUrl"].ToString() + "' alt='' style='width: 180px;'>" +
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
                                    "<td class='text-right h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + handlingAmountDue.ToString("N0") +"</strong></td>" +
                                "</tr>" +
                                "<tr class='active'>" +
                                    "<td colspan='4' class='text-right text-uppercase h4'><strong>Total Amount Due</strong></td>" +
                                    "<td class='text-right text-success h4'><strong>" + AppHelper.GetCurrencySymbol() + " " + totalDue.ToString("N0") + "</strong></td>" +
                                "</tr>" +
                            "</tbody>" +
                        "</table>");
            // Display the shopping cart
            divShoppingCart.InnerHtml = strShoppingCart.ToString();
            lblSalonName.InnerText = salonName;

            // Clear the list of read cart items
            readCartItems.Clear();

            // Hide the Checkout button if the total amount due = 0
            if (totalDue == 0)
            {
                //lnkCheckout.Visible = false;
            }
        }*/


    }
}