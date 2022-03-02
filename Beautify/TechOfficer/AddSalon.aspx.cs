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

namespace Beautify.Admin
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
        {
            // Add new salon account to Salon Role
            Roles.AddUserToRole(CreateUserWizard1.UserName, "Salon");

            // Update the new salon's membership record
            Membership.UpdateUser(Membership.GetUser(CreateUserWizard1.UserName));

            // Add salon record to Salons table
            AddSalon(CreateUserWizard1.UserName, CreateUserWizard1.Email);
        }

        private void AddSalon(string username, string email)
        {
            // Get the date that this salon is added. That is today's date
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
            string dateRegistered = day + "-" + AppHelper.GetMonthName(int.Parse(month)) + "-" + DateTime.Now.Year + "  " +
                hour + ":" + minute + " " + DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);

            string numericalDateRegistered = DateTime.Now.Year + "-" + month + "-" + day;

            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            conn = new SqlConnection(connString);
            //conn.Open();
            SqlCommand command;
            string sqlString;
            command = new SqlCommand();
            sqlString = @"INSERT INTO Salons Values(@Username,@Email, @SalonName," +
                " @Locations, @DateRegistered, @NumericalDateRegistered, @ImageUrl, @BookingStatus, @RentStatus, @About, @OpeningTime, @ClosingTime, @DisabledDays, @BankName, @AccountName, @AccountNumber, @PhoneNumber, @LocationsInCities)"; //the sql string        
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@SalonName", "");
            command.Parameters.AddWithValue("@Locations", "Lagos - NG,");
            command.Parameters.AddWithValue("@DateRegistered", dateRegistered);
            command.Parameters.AddWithValue("@NumericalDateRegistered", numericalDateRegistered);
            command.Parameters.AddWithValue("@ImageUrl", "content/backend/img/placeholders/avatars/avatar2.jpg");
            command.Parameters.AddWithValue("@BookingStatus", "ENABLED");
            command.Parameters.AddWithValue("@RentStatus", "ACTIVE");
            command.Parameters.AddWithValue("@About", "");
            command.Parameters.AddWithValue("@OpeningTime", "8:00am");
            command.Parameters.AddWithValue("@ClosingTime", "06:00pm");
            command.Parameters.AddWithValue("@DisabledDays", "");
            command.Parameters.AddWithValue("@BankName", "");
            command.Parameters.AddWithValue("@AccountName", "");
            // Encrypt a dummy account number
            command.Parameters.AddWithValue("@AccountNumber", MyAppSecurity.Encrypt("1234567890", MyAppSecurity.GetPasswordBytes()));
            command.Parameters.AddWithValue("@PhoneNumber", "");
            command.Parameters.AddWithValue("@LocationsInCities", ",");
            conn.Open();
            command.Connection = conn; //Assign connection of the command
            command.CommandText = sqlString; //Assign the command text of the command
            command.ExecuteNonQuery(); //Execute the query
            conn.Close();
        }
    }
}