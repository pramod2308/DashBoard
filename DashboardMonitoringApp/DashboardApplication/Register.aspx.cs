using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace DashboardApplication
{
    public partial class Resigter : System.Web.UI.Page
    {
        string loginconnstring = ConfigurationManager.ConnectionStrings["LoginConnString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {            
            int chkisexist = 0;
            SqlCommand command = new SqlCommand();

            chkisexist = ISEXIST(txtUserName.Text);

            if (chkisexist > 0)
            {
                lblmsg.Text = "User Already Exist!";
            }
            else
            {
                try
                {
                    string encryptpassword = EncryptDashboardPassword(txtPassword.Text);
                    string strQuery = "Insert Into tblDashBoardLogin (UserName,Password,CreatedDate) Values (@UserName,@Password,@CreatedDate)";
                    using (command = new SqlCommand(strQuery.ToString()))
                    {
                        command.Parameters.AddWithValue("@UserName", txtUserName.Text);
                        command.Parameters.AddWithValue("@Password", encryptpassword);
                        command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    }

                    using (SqlConnection connection = new SqlConnection(loginconnstring))
                    {
                        command.Connection = connection;
                        command.Connection.Open();
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 200;
                        SqlDataAdapter adt = new SqlDataAdapter(command);
                        command.ExecuteNonQuery();
                        command.Connection.Close();
                        lblmsg.Text = "Add User Succesfully";
                    }
                }
                catch (Exception ex)
                {
                    lblmsg.Text = ex.Message;
                }
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

        protected void btndecrypt_click(object sender, EventArgs e)
        {
            string EncryptionKey = ConfigurationManager.AppSettings["EncryptDecryptKey"].ToString();
            byte[] cipherBytes = Convert.FromBase64String(txtencpassword.Text);



            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    lbldecryptpassword.Text = "Your Password is " + Encoding.Unicode.GetString(ms.ToArray());
                }
            }           
        }
    }
}