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
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Load all cities where Beautify is available
                LoadCities();

                // Load the profile of the salon
                LoadSalonProfile(Membership.GetUser().UserName);
            }

            //add our javascript function to the fileupload control
            FileUpload1.Attributes.Add("onchange", "return checkFileExtension(this);");
        }

        private void LoadSalonProfile(string username)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT * FROM Salons WHERE Username = @Username";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the username parameter
            da.SelectCommand.Parameters.AddWithValue("@Username", username);
            dt = new DataTable();
            da.Fill(dt);
            // Ensure a record is returned before attempting to read
            if (dt.Rows.Count != 0)
            {
                // Salon Tab
                lblSalonUsername.Text = dt.Rows[0]["Username"].ToString();
                lblSalonEmail.Text = dt.Rows[0]["Email"].ToString();
                imgProfile.Src = "../" + dt.Rows[0]["ImageUrl"].ToString();
                txtSalonName.Text = dt.Rows[0]["SalonName"].ToString();
                txtAboutSalon.InnerText = dt.Rows[0]["About"].ToString();
                txtSalonPhoneNumber.Text = dt.Rows[0]["PhoneNumber"].ToString();
                divBookingLink.InnerHtml = "<div class='alert alert-success'>" +
                        "<h4><i class='fa fa-check-circle'></i> Share this link with your clients</h4> <a href='" + "http://beautify.com.ng/book/" + dt.Rows[0]["Username"].ToString() + "'>" + "http://beautify.com.ng/book/" + dt.Rows[0]["Username"].ToString() + "</a>" +
                        "</div>";

                // Booking Tab
                string bookingStatus = dt.Rows[0]["BookingStatus"].ToString();
                if (bookingStatus.Equals("ENABLED"))
                {
                    chkBookingStatus.Checked = true;
                }
                else
                {
                    chkBookingStatus.Checked = false;
                }

                txtSalonOpeningTime.Text = dt.Rows[0]["OpeningTime"].ToString();
                txtSalonClosingTime.Text = dt.Rows[0]["ClosingTime"].ToString();

                // Fetch the comma separated values of the salon's disabled days
                string disabledDaysCsv = dt.Rows[0]["DisabledDays"].ToString();
                string[] disabledDaysArray = disabledDaysCsv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                // Loop through all the days that this salon does not operate
                foreach (string day in disabledDaysArray)
                {
                    // Select each of the days that this salon does not operate
                    selDisabledDays.Items.FindByValue(day).Selected = true;
                }

                // Fetch the comma separated values of the salon's locations
                string salonLocationsCsv = dt.Rows[0]["Locations"].ToString();
                string[] salonLocationsArray = salonLocationsCsv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                // Loop through all the locations where this salon is available
                foreach (string location in salonLocationsArray)
                {
                    // Select each of the cities where this salon is available
                    selSalonLocations.Items.FindByValue(location).Selected = true;
                }

                // Load locations in the cities where this salon is available
                LoadLocationsInCities(salonLocationsArray);
                string locationsInCitiesCsv = dt.Rows[0]["LocationsInCities"].ToString();
                string[] locationsInCitiesArray = locationsInCitiesCsv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                // Loop through all locations in the cities where this salon is available and then select each location
                foreach (string location in locationsInCitiesArray)
                {
                    // Select each location in the city where this salon is available
                    selSalonLocationsInCities.Items.FindByValue(location).Selected = true;
                }

                // Bank Tab
                txtBankName.Text = dt.Rows[0]["BankName"].ToString();
                txtBankAccountName.Text = dt.Rows[0]["AccountName"].ToString();
                // Decrypt the account number
                txtBankAccountNumber.Text = MyAppSecurity.Decrypt(dt.Rows[0]["AccountNumber"].ToString(), MyAppSecurity.GetPasswordBytes());
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
        }

        /// <summary>
        /// Loads all cities where Beautify is available from the database
        /// </summary>
        private void LoadCities()
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT CityName FROM Cities ORDER BY CityName ASC";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            dt = new DataTable();
            da.Fill(dt);
            // Add all cities to the dropdown
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Add the current city.
                selSalonLocations.Items.Add(dt.Rows[i]["CityName"].ToString());
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
        }

        private void LoadLocationsInCities(string[] cities)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            // Create an SQL select string to search for all cities in the specified cities array
            string selectString = @"SELECT Locations FROM Cities WHERE CityName LIKE ";
            foreach (string city in cities)
            {
                selectString += "'" + city + "' OR CityName LIKE ";
            }

            // Remove the last occurrence of " OR CityName LIKE "
            selectString = selectString.Substring(0, selectString.Length - 18);

            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            dt = new DataTable();
            da.Fill(dt);
            StringBuilder strLocationsCsv = new StringBuilder();

            // Loop through all locations and append them to the string builder
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strLocationsCsv.Append(dt.Rows[i]["Locations"].ToString());
            }

            // Now, create a locations array from the locations Csv
            string[] locationsInCities = strLocationsCsv.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            
            // Loop through all locations in the array and add each one to the select
            foreach (string location in locationsInCities)
            {
                // Add each location to the dropdown
                ListItem item = new ListItem();
                item.Value = location;
                item.Text = location + "";
                selSalonLocationsInCities.Items.Add(item);
            }

            da.Dispose();
            dt.Clear();
            conn.Close();
        }

        private void UpdateSalonProfile(string username, string salonName, string locations, string imageUrl, string bookingStatus, string about, string openingTime, string closingTime, string disabledDays, string bankName, string accountName, string accountNumber, string phoneNumber, string locationsInCities)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"UPDATE Salons SET SalonName = @SalonName, Locations = @Locations, ImageUrl = @ImageUrl, BookingStatus = @BookingStatus," + 
                " About = @About, OpeningTime = @OpeningTime, ClosingTime = @ClosingTime, DisabledDays = @DisabledDays, BankName = @BankName, " +
                "AccountName = @AccountName, AccountNumber = @AccountNumber, PhoneNumber = @PhoneNumber, LocationsInCities = @LocationsInCities WHERE Username = @Username"; //the sql string        
            command.Parameters.AddWithValue("@SalonName", salonName);
            command.Parameters.AddWithValue("@Locations", locations);
            // Note that we are taking substring from position 3 because we appended ../ to the image src before
            command.Parameters.AddWithValue("@ImageUrl", imageUrl.Substring(3));
            command.Parameters.AddWithValue("@BookingStatus", bookingStatus);
            command.Parameters.AddWithValue("@About", about);
            command.Parameters.AddWithValue("@OpeningTime", openingTime);
            command.Parameters.AddWithValue("@ClosingTime", closingTime);
            command.Parameters.AddWithValue("@DisabledDays", disabledDays);
            command.Parameters.AddWithValue("@BankName", bankName);
            command.Parameters.AddWithValue("@AccountName", accountName);
            command.Parameters.AddWithValue("@AccountNumber", accountNumber);
            command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@LocationsInCities", locationsInCities);

            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Fetch the cities where this salon is available
                StringBuilder salonLocationsCsv = new StringBuilder();
                foreach (ListItem location in selSalonLocations.Items)
                {
                    // Check whether the location is selected
                    // This will ensure that we only add locations that the salon owner has selected
                    if (selSalonLocations.Items.FindByValue(location.Value).Selected == true)
                    {
                        salonLocationsCsv.Append(location + ",");
                    }
                }

                // Fetch the locations in the cities where this salon is available
                StringBuilder salonLocationsInCitiesCsv = new StringBuilder();
                foreach (ListItem location in selSalonLocationsInCities.Items)
                {
                    // Check whether the location is selected
                    // This will ensure that we only add locations that the salon owner has selected
                    if (selSalonLocationsInCities.Items.FindByValue(location.Value).Selected == true)
                    {
                        salonLocationsInCitiesCsv.Append(location + ",");
                    }
                }

                // Fetch the disabled days (no work) specified by the salon owner
                StringBuilder disabledDaysCsv = new StringBuilder();
                foreach (ListItem day in selDisabledDays.Items)
                {
                    // Check whether the day is selected
                    // This will ensure that we only add days that the salon owner has selected
                    if (selDisabledDays.Items.FindByValue(day.Value).Selected == true)
                    {
                        disabledDaysCsv.Append(day + ",");
                    }
                }

                // Fetch the booking status
                string bookingStatus = "";
                if (chkBookingStatus.Checked == true)
                {
                    bookingStatus = "ENABLED";
                }
                else
                {
                    bookingStatus = "DISABLED";
                }

                // Update the salon's profile
                // Note that we are encrypting the account number
                UpdateSalonProfile(Membership.GetUser().UserName, txtSalonName.Text, salonLocationsCsv.ToString(), imgProfile.Src, bookingStatus, txtAboutSalon.Value, txtSalonOpeningTime.Text, txtSalonClosingTime.Text, disabledDaysCsv.ToString(), txtBankName.Text, txtBankAccountName.Text, MyAppSecurity.Encrypt(txtBankAccountNumber.Text, MyAppSecurity.GetPasswordBytes()), txtSalonPhoneNumber.Text, salonLocationsInCitiesCsv.ToString());

                // Show a success message
                divMessage.InnerHtml = "<div class='alert alert-success'>" +
                            "<h4><i class='fa fa-check-circle'></i> Updated</h4> Your changes were saved successfully" +
                            "</div>";
            }
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
                string fullPhotoPath = "content/uploads/images/salons/" + Membership.GetUser().UserName + "_" + photoName;
                // Save the file on the server
                FileUpload1.SaveAs(Server.MapPath("~/" + fullPhotoPath));
                imgProfile.Src = "../" + fullPhotoPath; // Display the image

                divUploadMessage.InnerHtml = "<div class='alert alert-warning'><b>Note!</b> " +
                                    "Your image has been uploaded but it will only be saved when you click the 'Save Changes' button</div>";
            }
        }
    }
}