using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace DashboardApplication
{
    public partial class Login : System.Web.UI.Page
    {

        string loginconnstring = ConfigurationManager.ConnectionStrings["LoginConnString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnlogin_click(object sender, EventArgs e)
        {
            int chkisexist = 0;
            int validuser = 0;
            int updateexpiry = 0;
            string username = txtUsername.Text;
            string password = hdpass.Value;

            chkisexist = ISEXIST(username);

            if (chkisexist == 0)
            {
                lblMsg.Text = "User Not Exist!";
            }
            else
            {
                try
                {
                    string encryptpassword = EncryptDashboardPassword(password);
                    validuser = CheckLogin(username, encryptpassword);

                    if (validuser > 0)
                    {
                        updateexpiry = UpdateExpiryTime(username);

                        if (updateexpiry > 0)
                        {
                            Session["UserID"] = Convert.ToString(username);
                            Response.Redirect("~/FifthDashBoard.aspx", false);
                        }
                        else
                        {
                            lblMsg.Text = "Failed to update";
                        }
                    }
                    else
                    {
                        lblMsg.Text = "Invalid Password";
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Text = ex.Message;
                }
            }

        }

        public int CheckLogin(string username, string password)
        {
            int cnt = 0;
            SqlCommand command = new SqlCommand();

            try
            {
                string strQuery = "Select Count('1') From tblDashBoardLogin with (nolock) Where UserName = @UserName And Password = @Password";
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@UserName", username);
                    command.Parameters.AddWithValue("@Password", password);
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

        public int ISEXIST(string UserName)
        {
            int cnt = 0;
            SqlCommand command = new SqlCommand();

            try
            {
                string strQuery = "Select Count('1') From tblDashBoardLogin with (nolock) Where UserName = @UserName";
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

        public string EncryptDashboardPassword(string password)
        {
            string encryptkey = string.Empty;

            encryptkey = ConfigurationManager.AppSettings["EncryptDecryptKey"].ToString();

            string EncryptionKey = encryptkey;
            byte[] clearBytes = Encoding.Unicode.GetBytes(password);

            using (Aes encryptor = Aes.Create())
            {

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);


                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    password = Convert.ToBase64String(ms.ToArray());
                }
            }
            return password;
        }

        public int UpdateExpiryTime(string username)
        {
            int expirttime = Convert.ToInt32(ConfigurationManager.AppSettings["ExpiryTime"]);
            SqlCommand command = new SqlCommand();
            int cnt = 0;
            try
            {
                string strQuery = "Update tblDashBoardLogin SET ExpiryTime =  DATEADD(minute, " + expirttime + ", GetDate()) Where UserName = @UserName";
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@UserName", username);
                }

                using (SqlConnection connection = new SqlConnection(loginconnstring))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    cnt = command.ExecuteNonQuery();
                    command.Connection.Close();
                    return cnt;
                }
            }
            catch (Exception ex)
            {
                return cnt = 0;
            }
        }

    }
}