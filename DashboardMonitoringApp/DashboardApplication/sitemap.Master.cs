using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace DashboardApplication
{
    public partial class sitemap : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["UserID"]) == "")
            {
                Response.Redirect("~/Login.aspx", false);
            }
            else
            {
                int cnt = 0;
                cnt = CheckExpiryTime(Convert.ToString(Session["UserID"]));

                if (cnt == 0)
                {
                    Response.Redirect("~/Login.aspx", false);
                }
            }
        }

        protected void Logout(object sender, EventArgs e)
        {
            // Remove SessionId and Expire Session
            Session.Abandon();            
            Response.Redirect("~/Login.aspx");
        }

        public int CheckExpiryTime(string UserName)
        {
            int cnt = 0;
            SqlCommand command = new SqlCommand();
            string loginconnstring = ConfigurationManager.ConnectionStrings["LoginConnString"].ConnectionString;

            try
            {
                string strQuery = "Select Count(1) From tblDashBoardLogin Where UserName = @UserName And ExpiryTime > GetDate()";
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                }

                using (SqlConnection connection = new SqlConnection(loginconnstring))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    cnt = Convert.ToInt16(command.ExecuteScalar());
                    command.Connection.Close();

                    return cnt;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }     
}