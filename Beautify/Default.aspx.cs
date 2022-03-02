using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace Beautify
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadCities(); // Load the cities

                // Display the number of registered salons
                lblSalonCount.InnerText = "Explore Over " + GetSalonsCount() + " Salons!";
            }
        }

        /// <summary>
        /// Loads all cities from the database
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
            StringBuilder strCities = new StringBuilder();
            strCities.Append("<select id='selCity' class='cs-select cs-skin-border' size='1' >");
            strCities.Append("<option value='' disabled selected>Select a City</option>");
            // Add all cities to the select dropdown
            for (int i=0;i <dt.Rows.Count; i++)
            {
                // Add the current city. Note that we are removing " - " from the city name when assigning it to the value. This is because cities are stored as CityName - CountryCode
                strCities.Append("<option value='" + dt.Rows[i]["CityName"].ToString().Replace(" - ","-") + "'>" + dt.Rows[i]["CityName"].ToString() + "</option>");
            }
            strCities.Append("</select>");
            sectionCities.InnerHtml = strCities.ToString();
            da.Dispose();
            dt.Clear();
            conn.Close();
        }

        private int GetSalonsCount()
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT Email FROM Salons";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            dt = new DataTable();
            da.Fill(dt);
            int numOfSalons = 0;
            // Count the num of salons
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Increment the number of salons by 1
                numOfSalons++;
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
            // Return the number of salons
            return numOfSalons;
        }
    }
}