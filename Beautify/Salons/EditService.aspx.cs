using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Security;

namespace Beautify.Salons
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Load the service categories
                LoadServiceCategories();

                // Load the pricing info
                LoadPricingInfo();

                // Set the Add button as visible initially
                lnkAdd.Visible = true;
                lnkUpdate.Visible = false;

                // Hide the service ID div
                divServiceID.Visible = false;

                if (Request.QueryString["id"] != null)
                {
                    try
                    {
                        int serviceID = int.Parse(Request.QueryString["id"].ToString());
                        // Load the service using the specified id
                        LoadService(serviceID, Membership.GetUser().Email);

                        // Hide the Add button since this is an update
                        lnkAdd.Visible = false;
                        lnkUpdate.Visible = true;

                        // Hide the reset button
                        btnReset.Visible = false;
                    }
                    catch
                    {
                        // If an error occurs, take the user to the services page.
                        Response.Redirect("Services.aspx");
                    }
                }
            }

            //add our javascript function to the fileupload control
            FileUpload1.Attributes.Add("onchange", "return checkFileExtension(this);");
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
            // Add an empty value  to the select
            selServiceCategory.Items.Add("");
            
            // Add all categories to the dropdown
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Add each category
                selServiceCategory.Items.Add(dt.Rows[i]["CategoryName"].ToString());
                
            }
            // Disable the empty value option 
            selServiceCategory.Items.FindByValue("").Value = "";
            selServiceCategory.Items.FindByText("").Attributes["disabled"] = "disabled";

            // Make a default selection
            selServiceCategory.Items[1].Selected = true;

            da.Dispose();
            dt.Clear();
            conn.Close();
        }

        
        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //create an array of valid file extensions
            string[] validExt = { "jpg", "jpeg", "png" };

            //set args to invalid
            args.IsValid = false;

            //get file name
            string fileName = FileUpload1.PostedFile.FileName;

            //check to make sure the string is not empty
            if (!String.IsNullOrEmpty(fileName))
            {
                //get file extension
                string fileExt = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                //check if the current extension matches any valid extensions
                for (int i = 0; i < validExt.Length; i++)
                {
                    if (fileExt == validExt[i]) //if we find a match
                        args.IsValid = true;    //validate it
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Append the date and time to the filename
                string photoName = DateTime.Now.Day.ToString() +
                    DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() +
                    DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() +
                    DateTime.Now.Millisecond.ToString() + "_" + FileUpload1.PostedFile.FileName;
                string fullPhotoPath = "content/uploads/images/services/" + Membership.GetUser().UserName + "_" + photoName;
                // Save the file on the server
                FileUpload1.SaveAs(Server.MapPath("~/" + fullPhotoPath));
                imgProfile.Src = "../" + fullPhotoPath; // Display the image

                divUploadMessage.InnerHtml = "<div class='alert alert-warning'><b>Note!</b> " +
                                    "Your image has been uploaded but it will only be saved when you click the 'Add/Save' button below</div>";
            }
        }

        private void AddService(string salonEmail, string serviceCategory, string serviceName, string shortDescription, string fullDescription, string imageUrl, double serviceCost, bool serviceStatus)
        {
            // Get the date that this service is added. That is today's date
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
            string dateAdded = day + "-" + GetMonthName(int.Parse(month)) + "-" + DateTime.Now.Year + "  " +
                hour + ":" + minute + " " + DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            //conn.Open();
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"INSERT INTO ServicePricing Values(@SalonEmail,@ServiceCategory, @ServiceName," +
                " @ShortDescription, @FullDescription, @ImageUrl, @ServiceCost, @ServiceStatus, @DateAdded, @DateUpdated)"; //the sql string        
            command.Parameters.AddWithValue("@SalonEmail", salonEmail);
            command.Parameters.AddWithValue("@ServiceCategory", serviceCategory);
            command.Parameters.AddWithValue("@ServiceName", serviceName);
            command.Parameters.AddWithValue("@ShortDescription", shortDescription);
            command.Parameters.AddWithValue("@FullDescription", fullDescription);
            // Note that we are taking substring from position 3 because we appended ../ to the image src before
            command.Parameters.AddWithValue("@ImageUrl", imageUrl.Substring(3));
            command.Parameters.AddWithValue("@ServiceCost", Math.Round(serviceCost, 0));
            command.Parameters.AddWithValue("@ServiceStatus", serviceStatus);
            // Note that since this is a new service, dateAdded = dateUpdated
            command.Parameters.AddWithValue("@DateAdded", dateAdded);
            command.Parameters.AddWithValue("@DateUpdated", dateAdded);
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
        }

        private void UpdateService(int serviceID, string salonEmail, string serviceCategory, string serviceName, string shortDescription, string fullDescription, string imageUrl, double serviceCost, bool serviceStatus)
        {
            // Get the date that this service is updated. That is today's date
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
            string dateUpdated = day + "-" + GetMonthName(int.Parse(month)) + "-" + DateTime.Now.Year + "  " +
                hour + ":" + minute + " " + DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"UPDATE ServicePricing SET ServiceCategory = @ServiceCategory, ServiceName = @ServiceName, ShortDescription = @ShortDescription, FullDescription = @FullDescription," +
                " ImageUrl = @ImageUrl, ServiceCost = @ServiceCost, ServiceStatus = @ServiceStatus, DateUpdated = @DateUpdated " + 
                "WHERE ServiceID = @ServiceID AND SalonEmail = @SalonEmail"; //the sql string  
            command.Parameters.AddWithValue("@ServiceID", serviceID);
            command.Parameters.AddWithValue("@SalonEmail", salonEmail);
            command.Parameters.AddWithValue("@ServiceCategory", serviceCategory);
            command.Parameters.AddWithValue("@ServiceName", serviceName);
            command.Parameters.AddWithValue("@ShortDescription", shortDescription);
            command.Parameters.AddWithValue("@FullDescription", fullDescription);
            // Note that we are taking substring from position 3 because we appended ../ to the image src before
            command.Parameters.AddWithValue("@ImageUrl", imageUrl.Substring(3));
            command.Parameters.AddWithValue("@ServiceCost", Math.Round(serviceCost, 0));
            command.Parameters.AddWithValue("@ServiceStatus", serviceStatus);
            command.Parameters.AddWithValue("@DateUpdated", dateUpdated);
            
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
        }

        private void LoadService(int serviceID, string salonEmail)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT * FROM ServicePricing WHERE ServiceID = @ServiceID AND SalonEmail = @SalonEmail";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the parameters
            da.SelectCommand.Parameters.AddWithValue("@ServiceID", serviceID);
            da.SelectCommand.Parameters.AddWithValue("@SalonEmail", salonEmail);
            dt = new DataTable();
            da.Fill(dt);
            // Ensure a record is returned before attempting to read
            if (dt.Rows.Count != 0)
            {
                // Display details of the service
                imgProfile.Src = "../" + dt.Rows[0]["ImageUrl"].ToString();
                lblServiceID.Text = dt.Rows[0]["ServiceID"].ToString();
                txtServiceName.Text = dt.Rows[0]["ServiceName"].ToString();
                txtShortDescription.Value = dt.Rows[0]["ShortDescription"].ToString();
                txtFullDescription.Value = dt.Rows[0]["FullDescription"].ToString();
                selServiceCategory.Value = dt.Rows[0]["ServiceCategory"].ToString();
                txtServiceCost.Text = Math.Round(double.Parse(dt.Rows[0]["ServiceCost"].ToString()),0).ToString();

                // Make the appropriate status selection
                if (Convert.ToBoolean(dt.Rows[0]["ServiceStatus"].ToString()) == true)
                {
                    chkServiceStatus.Checked = true;
                }
                else
                {
                    chkServiceStatus.Checked = false;
                }

            }
            else
            {
                // Take the user to the services page if the service is not found
                Response.Redirect("Services.aspx");
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
        }

        public String GetMonthName(int month)
        {
            string monthName = "";
            switch (month)
            {
                case 1:
                    monthName = "JAN";
                    break;
                case 2:
                    monthName = "FEB";
                    break;
                case 3:
                    monthName = "MAR";
                    break;
                case 4:
                    monthName = "APR";
                    break;
                case 5:
                    monthName = "MAY";
                    break;
                case 6:
                    monthName = "JUN";
                    break;
                case 7:
                    monthName = "JUL";
                    break;
                case 8:
                    monthName = "AUG";
                    break;
                case 9:
                    monthName = "SEP";
                    break;
                case 10:
                    monthName = "OCT";
                    break;
                case 11:
                    monthName = "NOV";
                    break;
                case 12:
                    monthName = "DEC";
                    break;
            }
            return monthName; // Return the month name
        }

        protected void lnkAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool serviceStatus;
                if (chkServiceStatus.Checked == true)
                {
                    serviceStatus = true;
                }
                else
                {
                    serviceStatus = false;
                }
                // Add the service
                AddService(Membership.GetUser().Email, selServiceCategory.Value, txtServiceName.Text, txtShortDescription.Value, txtFullDescription.Value, imgProfile.Src, double.Parse(txtServiceCost.Text), serviceStatus);

                // Show a success message
                divMessage.InnerHtml = "<div class='alert alert-success'>" +
                            "<h4><i class='fa fa-check-circle'></i> Added</h4> The new service was added successfully" +
                            "</div>";

                // Clear the controls
                imgProfile.Src = "../content/uploads/images/services/default.jpg";
                txtServiceName.Text = "";
                txtShortDescription.Value = "";
                txtFullDescription.Value = "";
                txtServiceCost.Text = "";
            }
        }

        protected void lnkUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool serviceStatus;
                if (chkServiceStatus.Checked == true)
                {
                    serviceStatus = true;
                }
                else
                {
                    serviceStatus = false;
                }
                // Update the service
                UpdateService(int.Parse(lblServiceID.Text), Membership.GetUser().Email, selServiceCategory.Value, txtServiceName.Text, txtShortDescription.Value, txtFullDescription.Value, imgProfile.Src, double.Parse(txtServiceCost.Text), serviceStatus);

                // Show a success message
                divMessage.InnerHtml = "<div class='alert alert-success'>" +
                            "<h4><i class='fa fa-check-circle'></i> Updated</h4> Your changes were saved successfully" +
                            "</div>";
            }
        }

        
        private void LoadPricingInfo()
        {
            // Fetch the service commission that will be taken by Beautify
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT HandlingFeePercentage FROM PlatformCharges WHERE SettingName = 'SalonServiceCommission'";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            dt = new DataTable();
            da.Fill(dt);
            int serviceCommissionPercentage = 0;
            if (dt.Rows.Count != 0)
            {
                serviceCommissionPercentage = int.Parse(dt.Rows[0]["HandlingFeePercentage"].ToString());
            }
            divPricingInfo.InnerText = "Remember to include your transportation cost in your pricing. Also note that Beautify is entitled to " + serviceCommissionPercentage + "% commission on every service rendered.";
            da.Dispose();
            dt.Clear();
            conn.Close();
        }
        
    }
}