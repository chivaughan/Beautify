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
    public partial class WebForm7 : System.Web.UI.Page
    {
        // Set the initial page size and page index for portfolio
        private int pageSizePortfolio = 20;
        private int pageIndexPortfolio = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Register event handler for services pager user control event
            this.uclPagerPortfolio.PaginationLinkClicked += new EventHandler(portfolioPaginationLink_Click);

            if (Page.IsPostBack)
            {
                // Get the client ID of the control that caused the PostBack
                string postBackControlClientID = Request.Form["__EVENTTARGET"];


                // If the control is a member of uclPagerPortfolio, then we know that it was one of the link buttons in uclPagerPortfolio that caused the post back
                // So we proceed to handle that action
                // We are doing this check to ensure that the default page links 1 to 7 is not set for uclPagerPortfolio whenever a postback occurs from another control on this page
                if (postBackControlClientID.Contains("uclPagerPortfolio"))
                {
                    //During all postbacks - Add the pagination links to the page
                    int tableDataCount = PagingDatabase.GetPortfolioCount(Membership.GetUser().Email, selServiceCategory.Value);

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

            }

            if (!Page.IsPostBack)
            {
                // Load all service categories
                LoadServiceCategories();

                // Load the portfolio, set the first page as current index and then paginate
                pageIndexPortfolio = 1;
                BindPortfolioDataAndProcessPagination("PageFirstLoad");
            }
        }


        protected void portfolioPaginationLink_Click(object sender, EventArgs e)
        {
            //When link is clicked, set the pageIndex from user control property
            pageIndexPortfolio = uclPagerPortfolio.CurrentClickedIndex;
            // Set the page size from the select
            pageSizePortfolio = int.Parse(selServicePageSize.Value);

            BindPortfolioDataAndProcessPagination("PaginationLink");
            
        }


        private void BindPortfolioDataAndProcessPagination(string caller)
        {

            string category = selServiceCategory.Value;
            List<Portfolio> searchResult = new List<Portfolio>();

            // Check whether this method was called by the 'Discontinued Services' link button
            searchResult = PagingDatabase.GetPortfolioData(Membership.GetUser().Email, category, pageSizePortfolio, pageIndexPortfolio).ToList();
            


            StringBuilder strPortfolio = new StringBuilder();

            strPortfolio.Append("<table class='table table-bordered table-striped table-vcenter'>" +
                                "<thead>" +
                                    "<tr>" +
                                        "<th class='text-center' style='width: 70px;'></th>" +
                                        "<th>Title</th>" +
                                        "<th class='hidden-xs text-center'>Added</th>" +
                                        "<th class='text-center'>Action</th>" +
                                    "</tr>" +
                                "</thead>" +
                                "<tbody>");


            // Add all services found
            for (int i = 0; i < searchResult.Count; i++)
            {
                // Append the modal for the service
                strPortfolio.Append("<tr>" +
                                        "<td class='text-center'><a href='EditPortfolio.aspx?id=" + searchResult[i].id + "'><img height='60' width='60' src='../" + searchResult[i].imageUrl + "' class='img-circle'/></a></td>" +
                                        "<td><a href='EditPortfolio.aspx?id=" + searchResult[i].id + "'>" + searchResult[i].title + "</a></td>");
                
                strPortfolio.Append("<td class='hidden-xs text-center'>" + searchResult[i].dateAdded + "</td>" +
                                        "<td class='text-center'>" +
                                            "<div class='btn-group btn-group-xs'>" +
                                                "<a href='EditPortfolio.aspx?id=" + searchResult[i].id + "' data-toggle='tooltip' title='Edit' class='btn btn-default'><i class='fa fa-pencil'></i></a>" +
                                                "<a href='EditPortfolio.aspx?id=" + searchResult[i].id + "' data-toggle='tooltip' title='Delete' class='btn btn-danger'><i class='fa fa-times'></i></a>" +
                                        "</div>" +
                                        "</td>" +
                                    "</tr>");

            }

            strPortfolio.Append("</tbody>" +
                            "</table>");
            divPortfolio.InnerHtml = strPortfolio.ToString();

            //Index
            hdnCurrentIndexPortfolio.Value = pageIndexPortfolio.ToString(CultureInfo.InvariantCulture);

            int tableDataCount = 0;
            tableDataCount = PagingDatabase.GetPortfolioCount(Membership.GetUser().Email, category);
            

            // Only compute the portfolio count if this method is being called by PageFirstLoad
            if (caller.Equals("PageFirstLoad"))
            {
                // Show the number of all portfolio items
                lblAllPortfolioCount.InnerText = double.Parse(tableDataCount.ToString()).ToString("N0");
            }


            uclPagerPortfolio.AddPageLinks(tableDataCount, pageSizePortfolio, pageIndexPortfolio);

            // Show an appropriate description for the search
            lblSearchDescription.InnerHtml = "<strong>" + selServiceCategory.Value + "</strong> Portfolio";
            

        }



        protected void lnkRefreshPortfolio_Click(object sender, EventArgs e)
        {
            //When the refresh button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for services again
            pageIndexPortfolio = 1;
            pageSizePortfolio = int.Parse(selServicePageSize.Value);
            BindPortfolioDataAndProcessPagination("RefreshButton");
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
            // Add another option All
            selServiceCategory.Items.Add("All");

            // Add all categories to the dropdown
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Add each category
                selServiceCategory.Items.Add(dt.Rows[i]["CategoryName"].ToString());
            }
            // Disable the option named CATEGORY
            selServiceCategory.Items.FindByValue("CATEGORY").Value = "";
            selServiceCategory.Items.FindByText("CATEGORY").Attributes["disabled"] = "disabled";

            // Select 'All' as the default option
            selServiceCategory.Items.FindByValue("All").Selected = true;

            da.Dispose();
            dt.Clear();
            conn.Close();
        }

        

        protected void lnkAllPortfolio_Click(object sender, EventArgs e)
        {
            //When the 'All Services' link button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for all services again
            pageIndexPortfolio = 1;
            pageSizePortfolio = int.Parse(selServicePageSize.Value);
            // Select option 'All' from the category dropdown
            selServiceCategory.Value = "All";
            BindPortfolioDataAndProcessPagination("AllServices");
        }
    }
}