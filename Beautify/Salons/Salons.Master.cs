using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;

namespace Beautify.Salons
{
    public partial class Salons : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblUsername.InnerText = Membership.GetUser().UserName;
                imgSidebarPhoto.Src = GetSalonImageUrl(Membership.GetUser().UserName);
            }
        }

        private string GetSalonImageUrl(string username)
        {
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStrBeautify"].ConnectionString;
            SqlConnection conn;
            string selectString = @"SELECT ImageUrl FROM Salons WHERE Username = @Username";
            SqlDataAdapter da;
            DataTable dt;
            conn = new SqlConnection(connString);
            conn.Open();
            da = new SqlDataAdapter(selectString, conn);
            // Add the username parameter
            da.SelectCommand.Parameters.AddWithValue("@Username", username);
            dt = new DataTable();
            da.Fill(dt);
            string imageUrl = "";
            // Ensure a record is returned before attempting to read
            if (dt.Rows.Count != 0)
            {
                // Fetch the image url
                imageUrl = "../" + dt.Rows[0]["ImageUrl"].ToString();                
            }
            da.Dispose();
            dt.Clear();
            conn.Close();
            // Return the image url
            return imageUrl;
        }
    }
}