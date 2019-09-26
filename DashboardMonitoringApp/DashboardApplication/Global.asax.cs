using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Configuration;
namespace DashboardApplication
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            clsAESEncrytDecrypt objclsConnectionString = new clsAESEncrytDecrypt();
            
            try
            {
                    // getting Configuration Connection String
                    GlobalConnectionString.strFinoPaymentConfigurationConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentConfigurationConnString.ToString()].ConnectionString.ToString();
                    GlobalConnectionString.strFinoPaymentConfigurationConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentConfigurationConnString);

                    // getting Logger Connection String
                    GlobalConnectionString.strFinoPaymentLogConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentLogConnString.ToString()].ConnectionString.ToString();
                    GlobalConnectionString.strFinoPaymentLogConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentLogConnString);

                    // getting Master Connection String
                    GlobalConnectionString.strFinoPaymentMastersConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentMastersConnString.ToString()].ConnectionString.ToString();
                    GlobalConnectionString.strFinoPaymentMastersConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentMastersConnString);

                    //// getting Configuration Connection String
                    //GlobalConnectionString.strFinoPaymentConfigurationConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentConfigurationConnString.ToString()].ConnectionString.ToString();
                    //GlobalConnectionString.strFinoPaymentConfigurationConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentConfigurationConnString);

                    //// getting Logger Connection String
                    //GlobalConnectionString.strFinoPaymentLogConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentLogConnString.ToString()].ConnectionString.ToString();
                    //GlobalConnectionString.strFinoPaymentLogConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentLogConnString);

                    //// getting Master Connection String
                    //GlobalConnectionString.strFinoPaymentMastersConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentMastersConnString.ToString()].ConnectionString.ToString();
                    //GlobalConnectionString.strFinoPaymentMastersConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentMastersConnString);

                    //// getting Configuration Connection String
                    //GlobalConnectionString.strFinoPaymentConfigurationConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentConfigurationConnString.ToString()].ConnectionString.ToString();
                    //GlobalConnectionString.strFinoPaymentConfigurationConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentConfigurationConnString);

                    //// getting Logger Connection String
                    //GlobalConnectionString.strFinoPaymentLogConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentLogConnString.ToString()].ConnectionString.ToString();
                    //GlobalConnectionString.strFinoPaymentLogConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentLogConnString);

                    //// getting Master Connection String
                    //GlobalConnectionString.strFinoPaymentMastersConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentMastersConnString.ToString()].ConnectionString.ToString();
                    //GlobalConnectionString.strFinoPaymentMastersConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentMastersConnString);

                    //// getting Configuration Connection String
                    //GlobalConnectionString.strFinoPaymentConfigurationConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentConfigurationConnString.ToString()].ConnectionString.ToString();
                    //GlobalConnectionString.strFinoPaymentConfigurationConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentConfigurationConnString);

                    //// getting Logger Connection String
                    //GlobalConnectionString.strFinoPaymentLogConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentLogConnString.ToString()].ConnectionString.ToString();
                    //GlobalConnectionString.strFinoPaymentLogConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentLogConnString);

                    //// getting Master Connection String
                    //GlobalConnectionString.strFinoPaymentMastersConnString = ConfigurationManager.ConnectionStrings[ConnStr.FinoPaymentMastersConnString.ToString()].ConnectionString.ToString();
                    //GlobalConnectionString.strFinoPaymentMastersConnString = objclsConnectionString.GetDecryptedConnStringByName(GlobalConnectionString.strFinoPaymentMastersConnString);                    
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
        public enum ConnStr
        {
            FinoPaymentConfigurationConnString,
            FinoPaymentLogConnString, 
            FinoPaymentMastersConnString,
            LogTransactionConnString,
        }
    }
}