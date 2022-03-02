using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

namespace Beautify
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        // Set the initial page size and page index for salons
        private int pageSizeSalons = 9;
        private int pageIndexSalons = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Register event handler for salons pager user control event
            this.uclPagerSalons.PaginationLinkClicked += new EventHandler(salonsPaginationLink_Click);

            if (Page.IsPostBack)
            {
                // Get the client ID of the control that caused the PostBack
                string postBackControlClientID = Request.Form["__EVENTTARGET"];


                // If the control is a member of uclPagerSalons, then we know that it was one of the link buttons in uclPagerSalons that caused the post back
                // So we proceed to handle that action
                // We are doing this check to ensure that the default page links 1 to 7 is not set for uclPagerSalons whenever a postback occurs from another control on this page
                if (postBackControlClientID.Contains("uclPagerSalons"))
                {
                    // We are replacing the empty space in " - " . This is because we removed it in default.aspx. Note that cities are stored as CityName - CountryCode
                    string city = Request.QueryString["city"].ToString().Replace("-", " - ");
                    //During all postbacks - Add the pagination links to the page
                    int tableDataCount = PagingDatabase.GetSalonsCount(city,selLocationInCity.Value);

                    string defaultInitialValueForHiddenControl = "Blank Value";
                    int indexFromPreviousDataRetrieval = 1;
                    if (!String.Equals(hdnCurrentIndexSalons.Value, defaultInitialValueForHiddenControl))
                    {
                        indexFromPreviousDataRetrieval = Convert.ToInt32(hdnCurrentIndexSalons.Value, CultureInfo.InvariantCulture);
                    }

                    //Set property of user control
                    uclPagerSalons.PreviousIndex = indexFromPreviousDataRetrieval;

                    //Call method in user control
                    uclPagerSalons.PreAddAllLinks(tableDataCount, pageSizeSalons, indexFromPreviousDataRetrieval);
                }                
            }


            if (!Page.IsPostBack)
            {
                if (Request.QueryString["city"] != null)
                {
                    // We are replacing the empty space in " - " . This is because we removed it in default.aspx. Note that cities are stored as CityName - CountryCode
                    string city = Request.QueryString["city"].ToString().Replace("-", " - ");

                    // Load the locations in the specified city
                    LoadLocationsInCity(city);

                    // Load the salons, set the first page as current index and then paginate
                    pageIndexSalons = 1;
                    BindSalonsDataAndProcessPagination();
                }
                else
                {
                    // Take the user to the home page
                    Response.Redirect("Default.aspx");
                }
            }
        }

        

        
        

        protected void salonsPaginationLink_Click(object sender, EventArgs e)
        {
            //When link is clicked, set the pageIndex from user control property
            pageIndexSalons = uclPagerSalons.CurrentClickedIndex;
            // Set the page size from the select
            pageSizeSalons = int.Parse(selPageSize.Value);
            BindSalonsDataAndProcessPagination();
        }


        private void BindSalonsDataAndProcessPagination()
        {

            string orderBy = selOrderBy.Value;
            // We are replacing the empty space in " - " . This is because we removed it in default.aspx. Note that cities are stored as CityName - CountryCode
            string city = Request.QueryString["city"].ToString().Replace("-", " - ");
            List<Salon> searchResult = PagingDatabase.GetSalonsData(city, selLocationInCity.Value, orderBy, pageSizeSalons, pageIndexSalons).ToList();

            StringBuilder strSalons = new StringBuilder();
            int numberOfSalons = 0;
            // Add all salons found
            for (int i = 0; i < searchResult.Count; i++)
            {
                
                // Add the current salon.
               
                    strSalons.Append("<div class='col-md-4' data-toggle='animation-appear' data-animation-class='animation-fadeInQuick' data-element-offset='-100'>" +
                                "<div class='store-item'>" +

                                    "<div class='store-item-image' align='center'>" +
                                        "<a href='ChooseServices.aspx?salon=" + searchResult[i].username + "'>" +
                                            "<img src='" + searchResult[i].imageUrl + "' alt='' class='img-circle' height='300' width='300'>" +
                                        "</a>" +
                                    "</div>" +
                                    "<input id='input-2c' class='rating' min='0' max='5' step='0.1' data-size='xs'" +
               "data-symbol='&#xf005;' data-glyphicon='false' data-rating-class='rating-fa' value='" + Math.Round(Convert.ToDecimal(searchResult[i].averageRating), 1) + "' disabled='disabled'> " +
               "<label>" + FormatTextSingularAndPlural(searchResult[i].numberOfVotes, "Vote", "", " ") + "</label>" +
                                    "<div class='store-item-info clearfix'>" +
                                        "<span class='store-item-price themed-color-dark pull-right' style='font-size:small;'>" + FormatTextSingularAndPlural(searchResult[i].numberOfBookings, "Booking", "", " ") + "</span>" +
                                        "<a href='ChooseServices.aspx?salon=" + searchResult[i].username + "'><strong style='font-size:large;'>" + searchResult[i].salonName + "</strong></a><br>" +
                        // Here, we are displaying locations as: "specifiedCity and x NumberOfOtherCitiesInCSV"
                                        "<small><i class='fa fa-map-marker fa-2x text-muted'></i> <span style='font-size:small'>" + city + FormatTextSingularAndPlural(GetNumberOfCommaSeparatedValues(searchResult[i].locations) - 1, "City", " and ", " Other ") + "</span></small>" +
                                    "</div>" +
                                "</div>" +
                            "</div>");
                
                
                
            }

            
            

            // Display the salons
            divSalons.InnerHtml = strSalons.ToString();
            

            //Index
            hdnCurrentIndexSalons.Value = pageIndexSalons.ToString(CultureInfo.InvariantCulture);

            //Get total number of records
            int tableDataCount = PagingDatabase.GetSalonsCount(city,selLocationInCity.Value);

            uclPagerSalons.AddPageLinks(tableDataCount, pageSizeSalons, pageIndexSalons);

            // Compute the number of salons
            numberOfSalons = tableDataCount;
            // Display the search decsription
            if (selLocationInCity.Value.Equals("All"))
            {
                // A description for search for salons in all locations in the specified city
                lblSearchDecription.InnerHtml = "<strong>" + FormatTextSingularAndPlural(numberOfSalons, "salon", "", " </strong> ") + " found in " + city;
            }
            else
            {
                // A description for search for salons in a selected location in the specified city
                lblSearchDecription.InnerHtml = "<strong>" + FormatTextSingularAndPlural(numberOfSalons, "salon", "", " </strong> ") + " found in " + selLocationInCity.Value;
            }
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
            // We are using this if Block specifically for the Locations section for each salon on this page
            // This is because we want to show "CitName - CountryCode" for a salon that has only 1 city location
            // This will ensure that we do not display something like "Lagos - NG and 0 Other City"
            if (singularText.Equals("City") && quantity == 0)
            {
                return "";
            }
            
            // 
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

        protected void lnkRefreshSalons_Click(object sender, EventArgs e)
        {
            //When the refresh button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for services again
            pageIndexSalons = 1;
            pageSizeSalons = int.Parse(selPageSize.Value);
            BindSalonsDataAndProcessPagination();
        }

        private void LoadLocationsInCity(string cityName)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT Locations from Cities WHERE CityName = @CityName";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            da.SelectCommand.Parameters.AddWithValue("@CityName", cityName);
            dt = new DataTable();
            da.Fill(dt);
            // This will hold the Csv of the locations in the specified city
            string locationsInCityCsv = "";
            if (dt.Rows.Count != 0)
            {
                locationsInCityCsv = dt.Rows[0]["Locations"].ToString();
            }
            

            // Split the Csv into a string array
            string[] locationsInCity = locationsInCityCsv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            // Add another option All
            selLocationInCity.Items.Add("All");

            // Loop through the locations and add each one to the select
            foreach (string location in locationsInCity)
            {
                // Add each location to the dropdown
                selLocationInCity.Items.Add(location);
            }

            
            // Select 'All' as the default option
            selLocationInCity.Items.FindByValue("All").Selected = true;
            
            da.Dispose();
            dt.Clear();
            conn.Close();
        }
    }
}