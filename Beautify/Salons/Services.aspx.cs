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
    public partial class WebForm2 : System.Web.UI.Page
    {
        // Set the initial page size and page index for services
        private int pageSizeServices = 20;
        private int pageIndexServices = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Register event handler for services pager user control event
            this.uclPagerServices.PaginationLinkClicked += new EventHandler(servicesPaginationLink_Click);

            if (Page.IsPostBack)
            {
                // Get the client ID of the control that caused the PostBack
                string postBackControlClientID = Request.Form["__EVENTTARGET"];


                // If the control is a member of uclPagerServices, then we know that it was one of the link buttons in uclPagerServices that caused the post back
                // So we proceed to handle that action
                // We are doing this check to ensure that the default page links 1 to 7 is not set for uclPagerServices whenever a postback occurs from another control on this page
                if (postBackControlClientID.Contains("uclPagerServices"))
                {
                    //During all postbacks - Add the pagination links to the page
                    int tableDataCount = PagingDatabase.GetServicesCount(Membership.GetUser().Email, selServiceCategory.Value);

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

            }

            if (!Page.IsPostBack)
            {
                // Load all service categories
                LoadServiceCategories();

                // Load the services, set the first page as current index and then paginate
                pageIndexServices = 1;
                BindServicesDataAndProcessPagination("PageFirstLoad");
            }
        }


        protected void servicesPaginationLink_Click(object sender, EventArgs e)
        {
            //When link is clicked, set the pageIndex from user control property
            pageIndexServices = uclPagerServices.CurrentClickedIndex;
            // Set the page size from the select
            pageSizeServices = int.Parse(selServicePageSize.Value);
            
            // This will ensure that the 2nd,3rd, 4th, 5th, ... pages of discontinued services is shown
            if (selShow.Value.Equals("Discontinued"))
            {
                BindServicesDataAndProcessPagination("DiscontinuedServices");
            }
            else
            {
                BindServicesDataAndProcessPagination("PaginationLink");
            }
        }


        private void BindServicesDataAndProcessPagination(string caller)
        {

            string category = selServiceCategory.Value;
            List<Service> searchResult = new List<Service>();

            // Check whether this method was called by the 'Discontinued Services' link button
            if (caller.Equals("DiscontinuedServices"))
            {
                // At this point, we know that we only need to show the discontinued services
                searchResult = PagingDatabase.GetServicesDataWithStatus(Membership.GetUser().Email, category, "False", pageSizeServices, pageIndexServices).ToList();
            }
            else
            {
                // At this point, we show all services
                searchResult = PagingDatabase.GetServicesData(Membership.GetUser().Email, category, pageSizeServices, pageIndexServices).ToList();
            }

            
            StringBuilder strServices = new StringBuilder();

            strServices.Append("<table class='table table-bordered table-striped table-vcenter'>" + 
                                "<thead>" +
                                    "<tr>" +
                                        "<th class='text-center' style='width: 70px;'></th>" +
                                        "<th>Service Name</th>" +
                                        "<th class='text-right hidden-xs'>Price</th>" +
                                        "<th class='hidden-xs'>Status</th>" +
                                        "<th class='hidden-xs text-center'>Added</th>" +
                                        "<th class='text-center'>Action</th>" +
                                    "</tr>" +
                                "</thead>" +
                                "<tbody>");
            

            // Add all services found
            for (int i = 0; i < searchResult.Count; i++)
            {
                // Append the modal for the service
                strServices.Append("<tr>" +
                                        "<td class='text-center'><a href='EditService.aspx?id=" + searchResult[i].serviceID + "'><img height='60' width='60' src='../" + searchResult[i].imageUrl + "' class='img-circle'/></a></td>" +
                                        "<td><a href='EditService.aspx?id=" + searchResult[i].serviceID + "'>" + searchResult[i].serviceName + "</a></td>" +
                                        "<td class='text-right hidden-xs'><strong>" + AppHelper.GetCurrencySymbol() + " " + searchResult[i].serviceCost.ToString("N2") + "</strong></td>");
                // Style the status of each service
                if (searchResult[i].serviceStatus.Equals("True"))
                {
                    strServices.Append("<td class='hidden-xs'>" +
                                            "<span class='label label-success'>Available</span>" +
                                        "</td>");
                }
                else
                {
                    strServices.Append("<td class='hidden-xs'>" +
                                            "<span class='label label-danger'>Discontinued</span>" +
                                        "</td>");
                }
                strServices.Append("<td class='hidden-xs text-center'>" + searchResult[i].dateAdded + "</td>" +
                                        "<td class='text-center'>" +
                                            "<div class='btn-group btn-group-xs'>" +
                                                "<a href='EditService.aspx?id=" + searchResult[i].serviceID + "' data-toggle='tooltip' title='Edit' class='btn btn-default'><i class='fa fa-pencil'></i></a>" +
                                                "</div>" +
                                        "</td>" +
                                    "</tr>");                        

            }

            strServices.Append("</tbody>" +
                            "</table>");
            divServices.InnerHtml = strServices.ToString();

            //Index
            hdnCurrentIndexServices.Value = pageIndexServices.ToString(CultureInfo.InvariantCulture);
            
            int tableDataCount = 0;
            // Check whether this method was called by the 'Discontinued Services' link button
            if (caller.Equals("DiscontinuedServices"))
            {
                tableDataCount = PagingDatabase.GetServicesCountWithStatus(Membership.GetUser().Email, category, "False");
            }
            else
            {
                //Get total number of records
                tableDataCount = PagingDatabase.GetServicesCount(Membership.GetUser().Email, category);
            }

            // Only compute the service count if this method is being called by PageFirstLoad
            if (caller.Equals("PageFirstLoad"))
            {
                // Show the number of dicontinued services and all services
                lblDiscontinuedServicesCount.InnerText = double.Parse(PagingDatabase.GetServicesCountWithStatus(Membership.GetUser().Email, category, "False").ToString()).ToString("N0");
                lblAllServicesCount.InnerText = double.Parse(tableDataCount.ToString()).ToString("N0");
            }
            

            uclPagerServices.AddPageLinks(tableDataCount, pageSizeServices, pageIndexServices);

            // Show an appropriate description for the search
            // Check whether this method was called by the 'Discontinued Services' link button
            if (caller.Equals("DiscontinuedServices"))
            {
                lblSearchDescription.InnerHtml = "<strong>Discontinued</strong> Services";
            }
            else
            {
                lblSearchDescription.InnerHtml = "<strong>" + selServiceCategory.Value + "</strong> Services";
            }

        }

        

        protected void lnkRefreshServices_Click(object sender, EventArgs e)
        {
            //When the refresh button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for services again
            pageIndexServices = 1;
            pageSizeServices = int.Parse(selServicePageSize.Value);
            // Select 'All' from the hidden dropdown
            selShow.Value = "All";
            BindServicesDataAndProcessPagination("RefreshButton");
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

        protected void lnkDiscontinuedServices_Click(object sender, EventArgs e)
        {
            //When the 'Discontinued Services' link button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for discontinued services
            pageIndexServices = 1;
            pageSizeServices = int.Parse(selServicePageSize.Value);
            // Select option 'All' from the category dropdown
            selServiceCategory.Value = "All";
            // Select 'Discontinued' from the hidden dropdown
            selShow.Value = "Discontinued";
            BindServicesDataAndProcessPagination("DiscontinuedServices");
        }

        protected void lnkAllServices_Click(object sender, EventArgs e)
        {
            //When the 'All Services' link button is clicked, we should set the first page as the page index, set the page size that the user has chosen and then search for all services again
            pageIndexServices = 1;
            pageSizeServices = int.Parse(selServicePageSize.Value);
            // Select option 'All' from the category dropdown
            selServiceCategory.Value = "All";
            // Select 'All' from the hidden dropdown
            selShow.Value = "All";
            BindServicesDataAndProcessPagination("AllServices");
        }
    }
}