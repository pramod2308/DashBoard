using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DashboardApplication
{
    public partial class AEPSFailed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Menu M = (Menu)this.Master.FindControl("NavigationMenu");
                foreach (MenuItem item in M.Items)
                {
                    if (item.Text == "AEPS Failed")//Replace Home with the page name
                    {
                        item.Selected = true;
                    }
                }
            }
        }
    }
}