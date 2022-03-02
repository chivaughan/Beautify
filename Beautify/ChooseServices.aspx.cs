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

namespace Beautify
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        // Note: On this page, we are going to load all available services for a specified salon

        // Set the initial page size and page index for services
        private int pageSizeServices = 6;
        private int pageIndexServices = 1;

        // Set the initial page size and page index for portfolio
        private int pageSizePortfolio = 6;
        private int pageIndexPortfolio = 1;

        // Set the initial page size and page index for reviews
        private int pageSizeReviews = 15;
        private int pageIndexReviews = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Register event handler for services pager user control event
            this.uclPagerServices.PaginationLinkClicked += new EventHandler(servicesPaginationLink_Click);

            //Register event handler for portfolio pager user control event
            this.uclPagerPortfolio.PaginationLinkClicked += new EventHandler(portfolioPaginationLink_Click);

            //Register event handler for reviews pager user control event
            this.uclPagerReviews.PaginationLinkClicked += new EventHandler(reviewsPaginationLink_Click);

            if (Page.IsPostBack)
            {
                // Fetch the specified salon from the query string/ routeTable data value
                string salon = "";
                // This will be executed if the salon query parameter was supplied
                if (Request.QueryString["salon"] != null)
                {
                    salon = Request.QueryString["salon"].ToString();
                }

                // This will be used if the SalonRoute was used to get to this page
                if (Page.RouteData.Values["salonUsername"] != null)
                {
                    salon = Page.RouteData.Values["salonUsername"].ToString();
                }

                // Get the client ID of the control that caused the PostBack
                string postBackControlClientID = Request.Form["__EVENTTARGET"];
                

                // If the control is a member of uclPagerServices, then we know that it was one of the link buttons in uclPagerServices that caused the post back
                // So we proceed to handle that action
                // We are doing this check to ensure that the default page links 1 to 7 is not set for uclPagerServices whenever a postback occurs from another control on this page
                if (postBackControlClientID.Contains("uclPagerServices"))
                {
                    SalonInfo currentSalonInfo = new SalonInfo();
                    currentSalonInfo = GetSalonInfo(salon);
                    //During all postbacks - Add the pagination links to the page
                    int tableDataCount = PagingDatabase.GetServicesCountWithStatus(currentSalonInfo.email, selServiceCategory.Value, "True");

                    string defaultInitialValueForHiddenControl = "Blank Value";
                    int indexFromPreviousDataRetrieval = 1;
                    if (!String.Equals(hdnCurrentIndexServices.Value, defaultInitialValueForHiddenControl))
                    {
                        indexFromPreviousDataRetrieval = Convert.ToInt32(hdnCurrentIndexServices.Value, CultureInfo.InvariantCulture);
                    }

                    //Set property of user control
                    uclPagerServices.PreviousIndex = indexFromPreviousDataRetrieval;

                    //Call method in user control
                    uclPagerServices.PreAddAllLinks(tableDataCount, pageSizeServices, indexFromPreviousDataRetrieval);
                }

                // If the control is a member of uclPagerPortfolio, then we know that it was one of the link buttons in uclPagerPortfolio that caused the post back
                // So we proceed to handle that action
                // We are doing this check to ensure that the default page links 1 to 7 is not set for uclPagerPortfolio whenever a postback occurs from another control on this page
                if (postBackControlClientID.Contains("uclPagerPortfolio"))
                {
                    SalonInfo currentSalonInfo = new SalonInfo();
                    currentSalonInfo = GetSalonInfo(salon);
                    //During all postbacks - Add the pagination links to the page
                    int tableDataCount = PagingDatabase.GetPortfolioCount(currentSalonInfo.email, selPortfolioServiceCategory.Value);

                    string defaultInitialValueForHiddenControl = "Blank Value";
                    int indexFromPreviousDataRetrieval = 1;
                    if (!String.Equals(hdnCurrentIndexPortfolio.Value, defaultInitialValueForHiddenControl))
                    {
                        indexFromPreviousDataRetrieval = Convert.ToInt32(hdnCurrentIndexPortfolio.Value, CultureInfo.InvariantCulture);
                    }

                    //Set property of user control
                    uclPagerPortfolio.PreviousIndex = indexFromPreviousDataRetrieval;

                    //Call method in user control
                    uclPagerPortfolio.PreAddAllLinks(tableDataCount, pageSizePortfolio, indexFromPreviousDataRetrieval);
                }

                // If the control is a member of uclPagerReviews, then we know that it was one of the link buttons in uclPagerReviews that caused the post back
                // So we proceed to handle that action
                // We are doing this check to ensure that the default page links 1 to 7 is not set for uclPagerReviews whenever a postback occurs from another control on this page
                if (postBackControlClientID.Contains("uclPagerReviews"))
                {
                    SalonInfo currentSalonInfo = new SalonInfo();
                    currentSalonInfo = GetSalonInfo(salon);
                    //During all postbacks - Add the pagination links to the page
                    int tableDataCount = PagingDatabase.GetReviewsCount(currentSalonInfo.email);

                    string defaultInitialValueForHiddenControl = "Blank Value";
                    int indexFromPreviousDataRetrieval = 1;
                    if (!String.Equals(hdnCurrentIndexReviews.Value, defaultInitialValueForHiddenControl))
                    {
                        indexFromPreviousDataRetrieval = Convert.ToInt32(hdnCurrentIndexReviews.Value, CultureInfo.InvariantCulture);
                    }

                    //Set property of user control
                    uclPagerReviews.PreviousIndex = indexFromPreviousDataRetrieval;

                    //Call method in user control
                    uclPagerReviews.PreAddAllLinks(tableDataCount, pageSizeReviews, indexFromPreviousDataRetrieval);
                }
            }

            if (!Page.IsPostBack)
            {
                // Here we check whether the user got to this page by entering the SalonRoute. That is beautify.com.ng/book/salonUserName
                // If this is the case, we redirect the user to this page after appending the salon query parameter
                // We are doing this to avoid broken links that was observed if I allow this page to open as beautify.com.ng/book/salonUserName
                if (Page.RouteData.Values["salonUsername"] != null)
                {
                    // Redirect the user to this page and pass the salon query parameter
                    Response.Redirect("../ChooseServices.aspx?salon=" + Page.RouteData.Values["salonUsername"].ToString());
                }

                // Check whether a salon query parameter or the routeTable data value was passed
                if (Request.QueryString["salon"] != null)
                {
                    // Load the available service categories
                    LoadServiceCategories(); 

                    // Fetch the specified salon from the query string
                    string salon = "";
                    // This will be executed if the salon query parameter was supplied
                    if (Request.QueryString["salon"] != null)
                    {
                        salon = Request.QueryString["salon"].ToString();
                    }
                                        
                    // Write the salon username to the hidden field
                    lblSalonUsername.Value = salon;

                    // Load details of the specified salon (in the sidebar)
                    LoadSalonInfo(salon);

                    // Load the services, set the first page as current index and then paginate
                    pageIndexServices = 1;
                    BindServicesDataAndProcessPagination();

                    // Load the portfolio, set the first page as current index and then paginate
                    pageIndexPortfolio = 1;
                    BindPortfolioDataAndProcessPagination();

                    // Load the reviews, set the first page as current index and then paginate
                    pageIndexReviews = 1;
                    BindReviewsDataAndProcessPagination();
                }
                else
                {
                    // Take the user to the home page if no salon query parameter was passed
                    Response.Redirect("~/Default.aspx");
                }
            }
            
        }

        

        /// <summary>
        /// Loads details of a specified salon
        /// </summary>
        /// <param name="salon">The salon's username</param>
        private void LoadSalonInfo(string salon)
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

                // Display the salon name
                lblSalonName.InnerText = dt.Rows[0]["SalonName"].ToString();

                // Set the page title to the salon Name
                Page.Title = dt.Rows[0]["SalonName"].ToString() + " - Beautify: Beauty Salon Home Service for African Cities";

                // Add the current salon.
                strSalonInfo.Append("<div class='text-center'>" +
                                                "<img src='../" + dt.Rows[0]["ImageUrl"].ToString() + "' height='160px' width='160px' alt='' class='img-circle'>" +
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

                
                // Display the salon about
                divAbout.InnerHtml = dt.Rows[0]["About"].ToString();

                
            }
            else
            {
                // The salon was not found
                // Take the user to the home page
                Response.Redirect("~/Default.aspx");
            }
            
            // Display the salon info
            divSalonInfo.InnerHtml = strSalonInfo.ToString();
            
            da.Dispose();
            dt.Clear();
            conn.Close();
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



        protected void servicesPaginationLink_Click(object sender, EventArgs e)
        {
            //When link is clicked, set the pageIndex from user control property
            pageIndexServices = uclPagerServices.CurrentClickedIndex;
            // Set the page size from the select
            pageSizeServices = int.Parse(selServicePageSize.Value);
            BindServicesDataAndProcessPagination();
        }

        protected void portfolioPaginationLink_Click(object sender, EventArgs e)
        {
            //When link is clicked, set the pageIndex from user control property
            pageIndexPortfolio = uclPagerPortfolio.CurrentClickedIndex;
            // Set the page size from the select
            pageSizePortfolio = int.Parse(selPortfolioPageSize.Value);
            BindPortfolioDataAndProcessPagination();
        }

        
        private void BindServicesDataAndProcessPagination()
        {
            // Fetch the specified salon from the query string/ routeTable data value
            string salon = "";
            // This will be executed if the salon query parameter was supplied
            if (Request.QueryString["salon"] != null)
            {
                salon = Request.QueryString["salon"].ToString();
            }

            // This will be used if the SalonRoute was used to get to this page
            if (Page.RouteData.Values["salonUsername"] != null)
            {
                salon = Page.RouteData.Values["salonUsername"].ToString();
            }

            string category = selServiceCategory.Value;
            SalonInfo currentSalonInfo = new SalonInfo();
            currentSalonInfo = GetSalonInfo(salon);
            List<Service> searchResult = PagingDatabase.GetServicesDataWithStatus(currentSalonInfo.email, category, "True", pageSizeServices, pageIndexServices).ToList();

            StringBuilder strServices = new StringBuilder();

            // Display a message if no service is found
            if (searchResult.Count == 0)
            {
                strServices.Append("<div class='alert alert-danger'>" +
                        "<h4><i class='fa fa-times-circle'></i> No Service Found</h4> This salon has not added any service. Kindly Go Back and choose a different salon." +
                        "</div>");
            }

            // Add all services found
            for (int i = 0; i < searchResult.Count; i++)
            {
                // Append the modal for the service
                strServices.Append("<!-- SERVICE MODAL -->" +
                    "<div id='modal-regular-" + searchResult[i].serviceID.ToString() + "' class = 'modal fade' tabindex = '-1' role = 'dialog' aria-hidden = 'true'>" +
                "<div class='modal-dialog'>" +
                       "<div class='modal-content'>" +
                         "<div class='modal-header'>" +
                         "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button> " +
                           "<h4 class='modal-title'>" + searchResult[i].serviceName.ToString() + "</h4> " +
                         "</div> " +
                         "<div style='overflow-y:auto;overflow-x:hidden;height:350px;width:100%;'>" +

                         "<div class='form-horizontal form-bordered'>" +
                         "<br/><br/>" +
                   "<p align='center'>" +
                              "<img class='img-responsive' src='../" + searchResult[i].imageUrl.ToString() + "'/> " +
                              "</p> " +

                              "<div class='form-group'>" +
                          "<label class='col-md-3 control-label'>Name:</label><div class='col-md-9'> <strong><span class='help-block' style='font-size:medium;'>" + searchResult[i].serviceName + "</span></strong> " +
                       "</div></div> " +


                              "<div class='form-group'>" +
                          "<label class='col-md-3 control-label'>Category:</label><div class='col-md-9'> <strong><span class='help-block' style='font-size:medium;'>" + searchResult[i].serviceCategory + "</span></strong>" +
                       "</div></div> " +

                       "<div class='form-group'>" +
                         "<label class='col-md-3 control-label'>Excerpt:</label> <div class='col-md-9'><strong><span class='help-block' style='font-size:medium;'>" + searchResult[i].shortDescription + "</span></strong></div>" +
      "</div> " +

                              "<div class='form-group'>" +
                          "<label class='col-md-3 control-label'>Full Description:</label><div class='col-md-9'> <strong><span class='help-block' style='font-size:medium;'>" + searchResult[i].fullDescription + "</span></strong>" +
                       "</div></div>" +



 "<div class='form-group'>" +
                          "<label class='col-md-3 control-label'>Cost:</label>  <div class='col-md-9'><strong><span class='help-block' style='font-size:medium;color:red;'>" + AppHelper.GetCurrencySymbol() + " " + searchResult[i].serviceCost.ToString("N0") + "</span></strong>" +
      "</div></div>" +

                    "</div>" +

"</div>" +
"<div class='modal-footer'>" +
"<button type='button' class='btn btn-default' data-dismiss='modal'>Close</button>" +

"</div>" +
"</div>" +
"</div>" +
                        "</div>" +
                        "<input type='button' id='btnLaunchModal-" + searchResult[i].serviceID.ToString() + "' style='display:none;' class='btn btn-default btn-sm' data-toggle='modal' data-target='#modal-regular-" + searchResult[i].serviceID.ToString() + "' />" +
                        "<!-- END MODAL -->");

                // Append the service
                strServices.Append("<div class='col-md-6' data-toggle='animation-appear' data-animation-class='animation-fadeInQuick' data-element-offset='-100'>" +
                                "<div class='store-item'>" +

                                    "<div class='store-item-image' align='center'>" +
                                        "<a href='javascript:void(0);' onclick=\"javascript:$('#btnLaunchModal-" + searchResult[i].serviceID.ToString() + "').click();\">" +
                                            "<img src='../" + searchResult[i].imageUrl + "' alt='' class='img-circle' height='300' width='300'>" +
                                        "</a>" +
                                    "</div>" +
                                   "<div class='store-item-info clearfix'>" +
                                        "<span class='store-item-price themed-color-dark pull-right'>" + AppHelper.GetCurrencySymbol() + " " + searchResult[i].serviceCost.ToString("N0") + "</span>" +
                                        "<a href='javascript:void(0);' onclick=\"javascript:$('#btnLaunchModal-" + searchResult[i].serviceID.ToString() + "').click();\"><strong>" + searchResult[i].serviceName + "</strong></a><br>");

                // Change the action performed by AddtoCart and Remove buttons depending on the booking status

                if (currentSalonInfo.bookingStatus.Equals("ENABLED"))
                {
                    strServices.Append("<small id='addItem-" + searchResult[i].serviceID.ToString() + "'><i class='fa fa-shopping-cart text-muted'></i> <a href='javascript:AddItemToCart(" + searchResult[i].serviceID.ToString() + "," + searchResult[i].serviceCost.ToString() + ");' class='text-muted'>Add to cart</a></small>" +
                       "<small hidden='hidden' id='removeItem-" + searchResult[i].serviceID.ToString() + "'><a href='javascript:RemoveItemFromCart(" + searchResult[i].serviceID.ToString() + ");' class='text-danger'><i class='fa fa-times'></i> Remove</a></small>");
                }
                else
                {
                    // The salon is not available for bookingss
                    strServices.Append("<small id='addItem-" + searchResult[i].serviceID.ToString() + "'><i class='fa fa-shopping-cart text-muted'></i> <a href='javascript:NotAvailableForBooking();' class='text-muted'>Add to cart</a></small>" +
                       "<small hidden='hidden' id='removeItem-" + searchResult[i].serviceID.ToString() + "'><a href='javascript:NotAvailableForBooking();' class='text-danger'><i class='fa fa-times'></i> Remove</a></small>");
                }
                strServices.Append("</div>" +
                                "</div>" +
                            "</div>");

            }
            
            
            divServices.InnerHtml = strServices.ToString();

            //Index
            hdnCurrentIndexServices.Value = pageIndexServices.ToString(CultureInfo.InvariantCulture);

            //Get total number of records
            int tableDataCount = PagingDatabase.GetServicesCountWithStatus(currentSalonInfo.email, category, "True");

            uclPagerServices.AddPageLinks(tableDataCount, pageSizeServices, pageIndexServices);


        }

        private void BindPortfolioDataAndProcessPagination()
        {
            // Fetch the specified salon from the query string/ routeTable data value
            string salon = "";
            // This will be executed if the salon query parameter was supplied
            if (Request.QueryString["salon"] != null)
            {
                salon = Request.QueryString["salon"].ToString();
            }

            // This will be used if the SalonRoute was used to get to this page
            if (Page.RouteData.Values["salonUsername"] != null)
            {
                salon = Page.RouteData.Values["salonUsername"].ToString();
            }

            string category = selPortfolioServiceCategory.Value;
            SalonInfo currentSalonInfo = new SalonInfo();
            currentSalonInfo = GetSalonInfo(salon);
            List<Portfolio> searchResult = PagingDatabase.GetPortfolioData(currentSalonInfo.email, category, pageSizePortfolio, pageIndexPortfolio).ToList();

            StringBuilder strPortfolio = new StringBuilder();

            // Display a message if no portfolio item is found
            if (searchResult.Count == 0)
            {
                strPortfolio.Append("<div class='alert alert-danger'>" +
                        "<h4><i class='fa fa-times-circle'></i> Portfolio Empty</h4> This salon has not created a portfolio." +
                        "</div>");
            }

            // Add all portfolio items found
            for (int i = 0; i < searchResult.Count; i++)
            {
                // Append the modal for the portfolio
                strPortfolio.Append("<!-- PORTFOLIO MODAL -->" +
                    "<div id='modal-regular-" + searchResult[i].id.ToString() + "' class = 'modal fade' tabindex = '-1' role = 'dialog' aria-hidden = 'true'>" +
                "<div class='modal-dialog'>" +
                       "<div class='modal-content'>" +
                         "<div class='modal-header'>" +
                         "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>&times;</button> " +
                           "<h4 class='modal-title'>" + searchResult[i].title.ToString() + "</h4> " +
                         "</div> " +
                         "<div style='overflow-y:auto;overflow-x:hidden;height:350px;width:100%;'>" +

                         "<div class='form-horizontal form-bordered'>" +
                         "<br/><br/>" +
                   "<p align='center'>" +
                              "<img class='img-responsive' src='../" + searchResult[i].imageUrl.ToString() + "'/> " +
                              "</p> " +

                              "<div class='form-group'>" +
                          "<label class='col-md-3 control-label'>Title:</label><div class='col-md-9'> <strong><span class='help-block' style='font-size:medium;'>" + searchResult[i].title + "</span></strong> " +
                       "</div></div> " +


                              "<div class='form-group'>" +
                          "<label class='col-md-3 control-label'>Category:</label><div class='col-md-9'> <strong><span class='help-block' style='font-size:medium;'>" + searchResult[i].serviceCategory + "</span></strong>" +
                       "</div></div> " +

                       "<div class='form-group'>" +
                         "<label class='col-md-3 control-label'>Description:</label> <div class='col-md-9'><strong><span class='help-block' style='font-size:medium;'>" + searchResult[i].description + "</span></strong></div>" +
      "</div> " +

                             

                    "</div>" +

"</div>" +
"<div class='modal-footer'>" +
"<button type='button' class='btn btn-default' data-dismiss='modal'>Close</button>" +

"</div>" +
"</div>" +
"</div>" +
                        "</div>" +
                        "<input type='button' id='btnLaunchModal-" + searchResult[i].id.ToString() + "' style='display:none;' class='btn btn-default btn-sm' data-toggle='modal' data-target='#modal-regular-" + searchResult[i].id.ToString() + "' />" +
                        "<!-- END MODAL -->");

                // Append the service
                strPortfolio.Append("<div class='col-md-6' data-toggle='animation-appear' data-animation-class='animation-fadeInQuick' data-element-offset='-100'>" +
                                "<div class='store-item'>" +

                                    "<div class='store-item-image' align='center'>" +
                                        "<a href='javascript:void(0);' onclick=\"javascript:$('#btnLaunchModal-" + searchResult[i].id.ToString() + "').click();\">" +
                                            "<img src='../" + searchResult[i].imageUrl + "' alt='' class='img-circle' height='300' width='300'>" +
                                        "</a>" +
                                    "</div>" +
                                   "<div class='store-item-info clearfix'>" +
                                        "<a href='javascript:void(0);' onclick=\"javascript:$('#btnLaunchModal-" + searchResult[i].id.ToString() + "').click();\"><strong>" + searchResult[i].title + "</strong></a><br>");

                
                strPortfolio.Append("</div>" +
                                "</div>" +
                            "</div>");

            }


            divPortfolio.InnerHtml = strPortfolio.ToString();

            //Index
            hdnCurrentIndexPortfolio.Value = pageIndexPortfolio.ToString(CultureInfo.InvariantCulture);

            //Get total number of records
            int tableDataCount = PagingDatabase.GetPortfolioCount(currentSalonInfo.email, category);

            uclPagerPortfolio.AddPageLinks(tableDataCount, pageSizePortfolio, pageIndexPortfolio);


        }

        private SalonInfo GetSalonInfo(string salonUsername)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString;
            selectString = "SELECT Email, BookingStatus FROM Salons WHERE Username = @Username"; // The sql string
            SalonInfo currentSalonInfo = new SalonInfo();
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the Username parameter
            da.SelectCommand.Parameters.AddWithValue("@Username", salonUsername);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count != 0)
            {
                currentSalonInfo.email = dt.Rows[0]["Email"].ToString();
                currentSalonInfo.bookingStatus = dt.Rows[0]["BookingStatus"].ToString();
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
            // Return the current salon's info
            return currentSalonInfo;
        }

        protected void lnkRefreshServices_Click(object sender, EventArgs e)
        {
            //When the refresh button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for services again
            pageIndexServices = 1;
            pageSizeServices = int.Parse(selServicePageSize.Value);
            BindServicesDataAndProcessPagination();
        }

        protected void lnkRefreshPortfolio_Click(object sender, EventArgs e)
        {
            //When the refresh button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for services again
            pageIndexPortfolio = 1;
            pageSizePortfolio = int.Parse(selPortfolioPageSize.Value);
            BindPortfolioDataAndProcessPagination();
        }

        protected void reviewsPaginationLink_Click(object sender, EventArgs e)
        {
            //When link is clicked, set the pageIndex from user control property
            pageIndexReviews = uclPagerReviews.CurrentClickedIndex;
            BindReviewsDataAndProcessPagination();
        }

        private void BindReviewsDataAndProcessPagination()
        {
            // Fetch the specified salon from the query string/ routeTable data value
            string salon = "";
            // This will be executed if the salon query parameter was supplied
            if (Request.QueryString["salon"] != null)
            {
                salon = Request.QueryString["salon"].ToString();
            }

            // This will be used if the SalonRoute was used to get to this page
            if (Page.RouteData.Values["salonUsername"] != null)
            {
                salon = Page.RouteData.Values["salonUsername"].ToString();
            }

            SalonInfo currentSalonInfo = new SalonInfo();
            currentSalonInfo = GetSalonInfo(salon);
            List<Review> searchResult = PagingDatabase.GetReviewsData(currentSalonInfo.email, pageSizeReviews, pageIndexReviews).ToList();

            StringBuilder strReviews = new StringBuilder();
            strReviews.Append("<ul class='media-list push'>");

            // Display a message if no review is found
            if (searchResult.Count == 0)
            {
                strReviews.Append("<div class='alert alert-danger'>" +
                        "<h4><i class='fa fa-times-circle'></i> No Review Found</h4> This salon has not been reviewed by any client." +
                        "</div>");
            }

            // Add all reviews found
            for (int i = 0; i < searchResult.Count; i++)
            {
                strReviews.Append("<li class='media'>" +
                                                    "<a href='javascript:void(0)' class='pull-left'>" +
                                                        "<img src='../content/frontend/img/placeholders/avatars/avatar.png' alt='Avatar' class='img-circle'>" +
                                                    "</a>" +
                                                    "<div class='media-body'>" +
                                                        "<div class='text-warning pull-right'>" +
                                                            "<input class='rating' min='0' max='5' step='0.1' data-size='xxs' data-symbol='&#xf005;' data-glyphicon='false' " +
                                                    "data-rating-class='rating-fa' value='" + searchResult[i].rating.ToString() + "' disabled='disabled'>" +
                                                        "</div>" +
                                                        "<strong>" + searchResult[i].clientName + "</strong><br>" +
                                                        "<span class='text-muted'><small><em>" + searchResult[i].reviewDate + "</em></small></span>" +
                                                        "<p>" + searchResult[i].comment + "</p>" +
                                                    "</div>" +
                                                "</li>");

            }
            strReviews.Append("</ul>");
            

            // Display the reviews
            divReviews.InnerHtml = strReviews.ToString();

            //Index
            hdnCurrentIndexReviews.Value = pageIndexReviews.ToString(CultureInfo.InvariantCulture);

            //Get total number of records
            int tableDataCount = PagingDatabase.GetReviewsCount(currentSalonInfo.email);

            uclPagerReviews.AddPageLinks(tableDataCount, pageSizeReviews, pageIndexReviews);


        }

        /// <summary>
        /// Loads all service categories from the database
        /// </summary>
        private void LoadServiceCategories()
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT CategoryName FROM ServiceCategories ORDER BY CategoryName ASC";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            dt = new DataTable();
            da.Fill(dt);
            // Add an empty value CATEGORY to the select
            selServiceCategory.Items.Add("CATEGORY");
            selPortfolioServiceCategory.Items.Add("CATEGORY");
            // Add another option All
            selServiceCategory.Items.Add("All");
            selPortfolioServiceCategory.Items.Add("All");

            // Add all categories to the dropdown
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Add each category
                selServiceCategory.Items.Add(dt.Rows[i]["CategoryName"].ToString());
                selPortfolioServiceCategory.Items.Add(dt.Rows[i]["CategoryName"].ToString());
            }
            // Disable the option named CATEGORY
            selServiceCategory.Items.FindByValue("CATEGORY").Value = "";
            selPortfolioServiceCategory.Items.FindByValue("CATEGORY").Value = "";
            selServiceCategory.Items.FindByText("CATEGORY").Attributes["disabled"] = "disabled";
            selPortfolioServiceCategory.Items.FindByText("CATEGORY").Attributes["disabled"] = "disabled";

            // Select 'All' as the default option
            selServiceCategory.Items.FindByValue("All").Selected = true;
            selPortfolioServiceCategory.Items.FindByValue("All").Selected = true;

            da.Dispose();
            dt.Clear();
            conn.Close();
        }
    }
}