using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Beautify
{
    public partial class WebForm9 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Login1_LoggedIn(object sender, EventArgs e)
        {
            // Fetch the roles that the logged on use belongs to
            string[] userRoles = Roles.GetRolesForUser(Login1.UserName);

            // We are switching only the role at position 0 (the first role)
            // because our application allows a user to belong to only 1 role
            switch (userRoles[0])
            {
                case "Salon":
                    Response.Redirect("~/Salons/Default.aspx");
                    break;
                case "Tech Officer":
                    Response.Redirect("~/TechOfficer/AddSalon.aspx");
                    break;
                case "Administrator":
                    Response.Redirect("~/Admin/Default.aspx");
                    break;
            }
        }
    }
}