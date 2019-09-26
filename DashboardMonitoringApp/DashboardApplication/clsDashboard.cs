using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Web.Configuration;

namespace DashboardApplication
{
    public class clsDashboard
    {
        #region GET DashBoard Data
        public enum param
        {
            TimeInterval
        }
        public string GetDashboard(int DashboardType, string DtTime)
        {
            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string DashboardString = string.Empty;
            try
            {
                strQuery = GetStringBuilder(DashboardType);
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                //using (SqlConnection connection = new SqlConnection(GlobalConnectionString.strFinoPaymentLogConnString))
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["LogTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                DashboardString = JsonConvert.SerializeObject(dt);
                return DashboardString;
            }
            catch (Exception)
            {
                return DashboardString;
            }
            finally
            {
                command = null;
                strQuery = null;
                DashboardString = null;
            }
        }
        #endregion

        #region GET Transactions Data
        public string GetESBData(int ESBType, string Sdattime, string Edattime, string TransactionName, string TType, string appid)
        {
            DateTime SDt;
            DateTime EDt;

            string CustomerAppString = string.Empty;

            if (Sdattime == "" && Edattime == "")
            {
                SDt = DateTime.Now.AddMinutes(-2);
                EDt = DateTime.Now;
                Sdattime = String.Format("{0:yyyy-M-d HH:mm}", SDt);// +":00.000"; 
                Edattime = String.Format("{0:yyyy-M-d HH:mm}", EDt);// +":00.000";   
            }
            else if (Sdattime == "today" && Edattime == "today")
            {
                SDt = DateTime.Now;
                EDt = DateTime.Now;
                Sdattime = String.Format("{0:yyyy-M-d}", SDt) + " " + "00:00:00.000";
                Edattime = String.Format("{0:yyyy-M-d HH:mm}", EDt);

            }
            else
            {
                SDt = DateTime.Now;
                EDt = DateTime.Now;
                Sdattime = String.Format("{0:yyyy-M-d}", SDt) + " " + Sdattime + ":00:00.000";
                Edattime = String.Format("{0:yyyy-M-d}", EDt) + " " + Edattime + ":00:00.000";
            }


            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();

            try
            {
                if (ESBType == 100)
                {
                    strQuery = GetQueryOfCashCollection(TransactionName);
                }
                else
                {
                    strQuery = GetQueryOfESBData(ESBType, TransactionName, TType, appid);
                }
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@STime", Sdattime);
                    command.Parameters.AddWithValue("@ETime", Edattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 300;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    string transtype = dr[1].ToString();

                    if (transtype == null || transtype == "")
                    {
                        dt.Rows[i].Delete();
                    }
                }
                dt.AcceptChanges();

                CustomerAppString = JsonConvert.SerializeObject(dt);
                return CustomerAppString;
            }
            catch (Exception)
            {
                return CustomerAppString;
            }
            finally
            {
                command = null;
                strQuery = null;
                CustomerAppString = null;
            }
        }
        #endregion

        #region GET Failed-Awaiting Data
        public string GetFailedAwaitingData(int ESBType, string DtTime, string TransactionName, string TType)
        {
            DateTime datetime;
            string dattime;
            string CustomerAppString = string.Empty;
            if (DtTime != "")
            {
                if (DtTime == "2")
                {
                    datetime = DateTime.Now.AddMinutes(-2);
                    dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                   
                }
                else if (DtTime == "1")
                {
                    datetime = DateTime.Now.AddMinutes(-60);
                    dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                    
                }
                else
                {
                    datetime = DateTime.Now;
                    dattime = String.Format("{0:yyyy/M/d}", datetime);
                }

                SqlCommand command = new SqlCommand();
                DataSet ds;
                string strQuery = string.Empty;
                DataTable dt = new DataTable();

                try
                {
                    strQuery = GetQueryOfESBFailedAwaiting(ESBType, TransactionName, TType);
                    using (command = new SqlCommand(strQuery.ToString()))
                    {
                        command.Parameters.AddWithValue("@DateTime", dattime);
                    }
                    //using (SqlConnection connection = new SqlConnection(GlobalConnectionString.strESBConnString))
                    using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                    {
                        command.Connection = connection;
                        command.Connection.Open();
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 300;
                        SqlDataAdapter adt = new SqlDataAdapter(command);
                        command.Connection.Close();
                        ds = new DataSet();
                        adt.Fill(ds);

                    }
                    dt = ds.Tables[0];
                    CustomerAppString = JsonConvert.SerializeObject(dt);
                    return CustomerAppString;
                }
                catch (Exception)
                {
                    return CustomerAppString;
                }
                finally
                {
                    command = null;
                    strQuery = null;
                    CustomerAppString = null;
                }

            }
            return CustomerAppString;
        }
        #endregion

        #region GET Service/Node Latency Data
        public string GetLatencyData(string LatencyType, string DtTime)
        {
            DateTime datetime;
            string dattime;
            string LatencyString = string.Empty;

            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                   
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                    
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                strQuery = GetQueryOfLatencyData();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                //using (SqlConnection connection = new SqlConnection(GlobalConnectionString.strFinoPaymentLogConnString))
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["LogTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                dt2 = ds.Tables[1];
                if (LatencyType == "Service")
                {
                    LatencyString = JsonConvert.SerializeObject(dt);
                    return LatencyString;
                }
                else
                {
                    LatencyString = JsonConvert.SerializeObject(dt2);
                    return LatencyString;
                }
            }
            catch (Exception)
            {
                return LatencyString;
            }
            finally
            {
                command = null;
                strQuery = null;
                LatencyString = null;
            }
        }
        #endregion

        #region GET Merchant Transaction Data
        public string GetMerchantData(int MerchantTab, string Sdattime, string Edattime, string UserID)
        {
            DateTime SDt;
            DateTime EDt;

            string MerchantString = string.Empty;

            if (Sdattime == "" && Edattime == "")
            {
                SDt = DateTime.Now.AddMinutes(-2);
                EDt = DateTime.Now;
                Sdattime = String.Format("{0:yyyy-M-d HH:mm}", SDt);// +":00.000"; 
                Edattime = String.Format("{0:yyyy-M-d HH:mm}", EDt);// +":00.000";   
            }
            else if (Sdattime == "today" && Edattime == "today")
            {
                SDt = DateTime.Now;
                EDt = DateTime.Now;
                Sdattime = String.Format("{0:yyyy-M-d}", SDt) + " " + "00:00:00.000";
                Edattime = String.Format("{0:yyyy-M-d HH:mm}", EDt);

            }
            else
            {
                SDt = DateTime.Now;
                EDt = DateTime.Now;
                Sdattime = String.Format("{0:yyyy-M-d}", SDt) + " " + Sdattime + ":00:00.000";
                Edattime = String.Format("{0:yyyy-M-d}", EDt) + " " + Edattime + ":00:00.000";
            }


            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                strQuery = GetQueryOfMerchantData(UserID);
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@STime", Sdattime);
                    command.Parameters.AddWithValue("@ETime", Edattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 300;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                dt2 = ds.Tables[1];
                if (MerchantTab == 1)
                {
                    MerchantString = JsonConvert.SerializeObject(dt);
                    return MerchantString;
                }
                else
                {
                    MerchantString = JsonConvert.SerializeObject(dt2);
                    return MerchantString;
                }

            }
            catch (Exception)
            {
                return MerchantString;
            }
            finally
            {
                command = null;
                strQuery = null;
                MerchantString = null;
            }
        }
        #endregion

        #region GET Logs Data

        public string GetLogsDashboard(string tablename, string RequestId)
        {
            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string DashboardString = string.Empty;
            try
            {
                strQuery = GetLogStringBuilder(tablename, RequestId);
                command = new SqlCommand(strQuery.ToString());

                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["LogTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                DashboardString = JsonConvert.SerializeObject(dt);
                return DashboardString;
            }
            catch (Exception)
            {
                return DashboardString;
            }
            finally
            {
                command = null;
                strQuery = null;
                DashboardString = null;
            }
        }
        #endregion

        #region GET Request Data
        public string GetRequestTypeData(string RequestType)
        {
            string RequestTypeString = string.Empty;
            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();

            try
            {
                strQuery = GetQueryofRequestType(RequestType);
                command = new SqlCommand(strQuery.ToString());

                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];

                RequestTypeString = JsonConvert.SerializeObject(dt);
                return RequestTypeString;
            }
            catch (Exception)
            {
                return RequestTypeString;
            }
            finally
            {
                command = null;
                strQuery = null;
                RequestTypeString = null;
            }
        }
        #endregion

        #region GET ZONE Data
        public string GetESBLatencyData(string DtTime, string LatencyType, string ServiceID, string MethodID)
        {
            DateTime datetime;
            string dattime;
            string ESBLatencyString = string.Empty;

            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                   
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                    
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                strQuery = GetQueryofESBLatencyData(LatencyType, ServiceID, MethodID);
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["LogTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];

                ESBLatencyString = JsonConvert.SerializeObject(dt);
                return ESBLatencyString;
            }
            catch (Exception ex)
            {
                return ESBLatencyString;
            }
            finally
            {
                command = null;
                strQuery = null;
                ESBLatencyString = null;
            }
        }
        #endregion

        #region GET IMPS Outward Data

        public string GetIMPSDashboard(string DtTime)
        {
            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string IMPSString = string.Empty;
            try
            {
                strQuery = GetIMPSStringBuilder();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }

                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                IMPSString = JsonConvert.SerializeObject(dt);
                return IMPSString;
            }
            catch (Exception)
            {
                return IMPSString;
            }
            finally
            {
                command = null;
                strQuery = null;
                IMPSString = null;
            }
        }
        #endregion

        #region GET EKO Data
        public string GetEKOData(string DtTime, string ekostatus, string TxnStatus, string EndDateTime, string clientname)
        {
            //DateTime datetime;
            //string dattime;
            string EKOString = string.Empty;

            //if (DtTime == "2")
            //{
            //    datetime = DateTime.Now.AddMinutes(-2);
            //    dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                   
            //}
            //else if (DtTime == "1")
            //{
            //    datetime = DateTime.Now.AddMinutes(-60);
            //    dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                    
            //}
            //else
            //{
            //    datetime = DateTime.Now;
            //    dattime = String.Format("{0:yyyy/M/d}", datetime);
            //}

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                strQuery = GetQueryofEKOData(ekostatus, TxnStatus, clientname);
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", DtTime);
                    command.Parameters.AddWithValue("@EndDateTime", EndDateTime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];

                EKOString = JsonConvert.SerializeObject(dt);
                return EKOString;
            }
            catch (Exception ex)
            {
                return EKOString;
            }
            finally
            {
                command = null;
                strQuery = null;
                EKOString = null;
            }
        }
        #endregion

        #region GET AEPS Data

        public string GetAEPS_RUPAYTransactions(string tranType, string DtTime)
        {
            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string AEPS_RUPAYString = string.Empty;
            try
            {
                strQuery = Get_AEPS_RUPAYTransactionsStringBuilder(tranType);
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                AEPS_RUPAYString = JsonConvert.SerializeObject(dt);
                return AEPS_RUPAYString;
            }
            catch (Exception)
            {
                return AEPS_RUPAYString;
            }
            finally
            {
                command = null;
                strQuery = null;
                AEPS_RUPAYString = null;
            }
        }
        #endregion

        #region GET AEPS Failed Data
        public string GetAEPSFailedData(string DtTime)
        {
            DateTime datetime;
            string dattime;
            string AEPSString = string.Empty;
            if (DtTime != "")
            {
                if (DtTime == "2")
                {
                    datetime = DateTime.Now.AddMinutes(-2);
                    dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                   
                }
                else if (DtTime == "1")
                {
                    datetime = DateTime.Now.AddMinutes(-60);
                    dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                    
                }
                else
                {
                    datetime = DateTime.Now;
                    dattime = String.Format("{0:yyyy/M/d}", datetime);
                }

                SqlCommand command = new SqlCommand();
                DataSet ds;
                string strQuery = string.Empty;
                DataTable dt = new DataTable();

                try
                {
                    strQuery = GetQueryOfAEPSFailed();
                    using (command = new SqlCommand(strQuery.ToString()))
                    {
                        command.Parameters.AddWithValue("@DateTime", dattime);
                    }
                    //using (SqlConnection connection = new SqlConnection(GlobalConnectionString.strESBConnString))
                    using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                    {
                        command.Connection = connection;
                        command.Connection.Open();
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 300;
                        SqlDataAdapter adt = new SqlDataAdapter(command);
                        command.Connection.Close();
                        ds = new DataSet();
                        adt.Fill(ds);

                    }
                    dt = ds.Tables[0];
                    AEPSString = JsonConvert.SerializeObject(dt);
                    return AEPSString;
                }
                catch (Exception)
                {
                    return AEPSString;
                }
                finally
                {
                    command = null;
                    strQuery = null;
                    AEPSString = null;
                }

            }
            return AEPSString;
        }
        #endregion

        #region GET AEPS API Data
        public string GetAEPSAPIData(string DtTime, string aepsapistatus, string requestflag, string EndDateTime, string clientname, string productcode)
        {
            string AEPSAPIString = string.Empty;

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                strQuery = GetQueryofAEPSAPI(aepsapistatus, requestflag, clientname, productcode);
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", DtTime);
                    command.Parameters.AddWithValue("@EndDateTime", EndDateTime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["PBBC_APIConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];

                AEPSAPIString = JsonConvert.SerializeObject(dt);
                return AEPSAPIString;
            }
            catch (Exception ex)
            {
                return AEPSAPIString;
            }
            finally
            {
                command = null;
                strQuery = null;
                AEPSAPIString = null;
            }
        }
        #endregion

        #region GET AEPS ISSUER Data

        public string GetAEPSISSUERTransactions(string DtTime)
        {
            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string AEPSISSUERString = string.Empty;
            try
            {
                strQuery = Get_AEPSISSUER_TransactionsStringBuilder();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                AEPSISSUERString = JsonConvert.SerializeObject(dt);
                return AEPSISSUERString;
            }
            catch (Exception)
            {
                return AEPSISSUERString;
            }
            finally
            {
                command = null;
                strQuery = null;
                AEPSISSUERString = null;
            }
        }
        #endregion

        #region GET Refire Success-Failed Data

        public string GetRefireSuccFailTransactions(string fromdate, string todate, int refire)
        {
            DateTime datetime;
            string fdate = fromdate;
            string tdate = todate;

            if (fromdate == "" && todate == "")
            {
                datetime = DateTime.Now;
                fdate = String.Format("{0:yyyy/M/d}", datetime);
                tdate = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string refres = string.Empty;
            try
            {
                strQuery = RefireQueryData(refire, fdate, tdate);
                command = new SqlCommand(strQuery.ToString());

                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                //using (SqlConnection connection = new SqlConnection("server=10.15.20.51;Database=PaymentBank;Persist Security Info=True;uid=a_devcomdb_prd;password=Sql@admin;"))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    adt.Fill(dt);
                }
                if (refire == 1)
                {
                    refres = JsonConvert.SerializeObject(dt);
                }
                if (refire == 3)
                {
                    refres = JsonConvert.SerializeObject(dt);
                }
                return refres;
            }
            catch (Exception)
            {
                return refres;
            }
            finally
            {
                command = null;
                strQuery = null;
                refres = null;
            }
        }
        #endregion

        #region AccountOpeningData
        public string GetAccountOpeningData(string DtTime)
        {
            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string AccountOpeningString = string.Empty;
            try
            {
                strQuery = AccountOpeningQuery();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["LogTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                AccountOpeningString = JsonConvert.SerializeObject(dt);
                return AccountOpeningString;
            }
            catch (Exception)
            {
                return AccountOpeningString;
            }
            finally
            {
                command = null;
                strQuery = null;
                AccountOpeningString = null;
            }
        }
        #endregion

        #region GET DMSStoredProcedure

        public string DMSStoredProcedure(string DtTime, string tabname)
        {
            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                // datetime = DateTime.Now.AddMonths(-10);
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string TransactionsString = string.Empty;
            try
            {
                if (tabname == "ACCOUNT_DASHBOARD")
                {
                    strQuery = "SP_ACCOUNT_DASHBOARD";
                }
                else
                {
                    strQuery = "SP_LENDING_DASHBOARD";
                }
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DATE", dattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DMSConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                TransactionsString = JsonConvert.SerializeObject(dt);
                return TransactionsString;
            }
            catch (Exception ex)
            {

                return "3";
            }
            finally
            {
                command = null;
                strQuery = null;
                TransactionsString = null;
            }
        }
        #endregion

        #region GET Suryoday Data

        public string GetSuryodayTransactions(string date, string esbtab)
        {

            SqlCommand command = new SqlCommand();
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string suryodayjsondata = string.Empty;
            try
            {

                DateTime datetime;
                string fdate = date;

                if (date == "")
                {
                    datetime = DateTime.Now;
                    fdate = String.Format("{0:yyyy/M/d}", datetime);
                }

                if (esbtab == "2")
                {
                    strQuery = SuryodaySubQueryData(fdate);
                }
                else
                {
                    strQuery = SuryodayQueryData(fdate);
                }


                command = new SqlCommand(strQuery.ToString());

                //using (SqlConnection connection = new SqlConnection("server=10.15.20.51;Database=ESB_Transactions;Persist Security Info=True;uid=a_devcomdb_prd;password=Sql@admin"))
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    adt.Fill(dt);
                }

                suryodayjsondata = JsonConvert.SerializeObject(dt);

                return suryodayjsondata;
            }
            catch (Exception)
            {
                return suryodayjsondata;
            }
            finally
            {
                command = null;
                strQuery = null;
                suryodayjsondata = null;
            }
        }
        #endregion

        #region ESB_Latency Report Query

        public string GetESB_Latency_DBInsertion(string DtTime)
        {
            // int MinusMinute = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings[param.TimeInterval.ToString()]);
            // DateTime datetime = DateTime.Now.AddMinutes(MinusMinute);
            // string dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";

            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "10")
            {
                datetime = DateTime.Now.AddMinutes(-10);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "15")
            {
                datetime = DateTime.Now.AddMinutes(-15);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "30")
            {
                datetime = DateTime.Now.AddMinutes(-30);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy-MM-dd}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string DashboardString = string.Empty;
            try
            {
                strQuery = GetESB_Latency_DBInsertionQuery();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                // using (SqlConnection connection = new SqlConnection(GlobalConnectionString.strFinoPaymentLogConnString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                DashboardString = JsonConvert.SerializeObject(dt);
                return DashboardString;
            }
            catch (Exception)
            {
                return DashboardString;
            }
            finally
            {
                command = null;
                strQuery = null;
                DashboardString = null;
            }
        }
        #endregion

        #region Get ESB Response Time Query

        public string GetESB_Latency_Report(string Sdattime, string Edattime)
        {
            DateTime SDt;
            DateTime EDt;

            string MerchantString = string.Empty;

            if (Sdattime == "" && Edattime == "")
            {
                SDt = DateTime.Now.AddMinutes(-2);
                EDt = DateTime.Now;
                Sdattime = String.Format("{0:yyyy-M-d HH:mm}", SDt);// +":00.000"; 
                Edattime = String.Format("{0:yyyy-M-d HH:mm}", EDt);// +":00.000";   
            }
            else if (Sdattime == "today" && Edattime == "today")
            {
                SDt = DateTime.Now;
                EDt = DateTime.Now;
                Sdattime = String.Format("{0:yyyy-M-d}", SDt) + " " + "00:00:00.000";
                Edattime = String.Format("{0:yyyy-M-d HH:mm}", EDt);

            }
            else
            {
                SDt = DateTime.Now;
                EDt = DateTime.Now;
                Sdattime = String.Format("{0:yyyy-M-d}", SDt) + " " + Sdattime + ":00:00.000";
                Edattime = String.Format("{0:yyyy-M-d}", EDt) + " " + Edattime + ":00:00.000";
            }


            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string DashboardString = string.Empty;
            try
            {
                strQuery = GetESB_Latency_ReportQuery();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@from", Sdattime);
                    command.Parameters.AddWithValue("@to", Edattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                // using (SqlConnection connection = new SqlConnection(GlobalConnectionString.strFinoPaymentLogConnString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                DashboardString = JsonConvert.SerializeObject(dt);
                return DashboardString;
            }
            catch (Exception)
            {
                return DashboardString;
            }
            finally
            {
                command = null;
                strQuery = null;
                DashboardString = null;
            }
        }
        #endregion

        #region GET ESB NODE and SERVICE WISE Latency Data From DevDB

        public string GetESBNODESERVICE_Latency_Report(string LatencyType, string DtTime)
        {
            DateTime datetime;
            string dattime;
            string LatencyString = string.Empty;

            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "10")
            {
                datetime = DateTime.Now.AddMinutes(-10);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "15")
            {
                datetime = DateTime.Now.AddMinutes(-15);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "30")
            {
                datetime = DateTime.Now.AddMinutes(-30);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy-MM-dd}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            string DashboardString = string.Empty;
            try
            {
                strQuery = GetQueryOfESBLatencyData();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);

                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                // using (SqlConnection connection = new SqlConnection(GlobalConnectionString.strFinoPaymentLogConnString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                dt2 = ds.Tables[1];
                if (LatencyType == "Service")
                {
                    LatencyString = JsonConvert.SerializeObject(dt);
                    return LatencyString;
                }
                else
                {
                    LatencyString = JsonConvert.SerializeObject(dt2);
                    return LatencyString;
                }
            }
            catch (Exception)
            {
                return DashboardString;
            }
            finally
            {
                command = null;
                strQuery = null;
                DashboardString = null;
            }
        }
        #endregion

        #region GET HA-ESB Latency Data From DevDB

        public string Get_HA_ESBLatencyReport(string DtTime)
        {
            DateTime datetime;
            string dattime;
            string LatencyString = string.Empty;

            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "10")
            {
                datetime = DateTime.Now.AddMinutes(-10);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "15")
            {
                datetime = DateTime.Now.AddMinutes(-15);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "30")
            {
                datetime = DateTime.Now.AddMinutes(-30);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy-MM-dd}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            string DashboardString = string.Empty;
            try
            {
                strQuery = GetQueryOfHA_ESBLatency();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);

                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                // using (SqlConnection connection = new SqlConnection(GlobalConnectionString.strFinoPaymentLogConnString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];

                LatencyString = JsonConvert.SerializeObject(dt);
                return LatencyString;
            }
            catch (Exception)
            {
                return DashboardString;
            }
            finally
            {
                command = null;
                strQuery = null;
                DashboardString = null;
            }
        }
        #endregion

        #region GET DMS Data

        public string GetDMSData(string DtTime)
        {
            DateTime datetime;
            string dattime;

            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy-MM-dd HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy-MM-dd}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ESBDataSet = new DataSet();
            DataSet DMSDataSet = new DataSet();
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            string DMSString = string.Empty;
            try
            {
                for (int i = 1; i <= 2; i++)
                {
                    strQuery = GetDMSQuery(i);
                    string[] connstring = { "ESBTransactionConnString", "DMSConnString" };
                    // command = new SqlCommand(strQuery.ToString());
                    using (command = new SqlCommand(strQuery.ToString()))
                    {
                        command.Parameters.AddWithValue("@DateTime", dattime);

                    }
                    using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings[connstring[i - 1]].ConnectionString))
                    {
                        command.Connection = connection;
                        command.Connection.Open();
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 200;
                        SqlDataAdapter adt = new SqlDataAdapter(command);
                        command.Connection.Close();
                        if (i == 1)
                        {
                            ESBDataSet = new DataSet();
                            adt.Fill(ESBDataSet);
                            dt = ESBDataSet.Tables[0];
                        }
                        else
                        {
                            DMSDataSet = new DataSet();
                            adt.Fill(DMSDataSet);
                            dt2 = DMSDataSet.Tables[0];
                        }

                    }
                }

                var data = from esb in dt.AsEnumerable()
                           join dms in dt2.AsEnumerable() on esb.Field<int>("Minute") equals dms.Field<int>("Minute")
                           where esb.Field<DateTime>("DATE") == dms.Field<DateTime>("DATE") && esb.Field<int>("HOUR") == dms.Field<int>("HOUR") && esb.Field<int>("Minute") == dms.Field<int>("Minute")
                           select new { DATE = esb.Field<DateTime>("DATE"), HOUR = esb.Field<int>("HOUR"), Duration = esb.Field<int>("Minute"), ESB = esb.Field<int>("ESB"), DMS = dms.Field<int>("DMS") };



                DMSString = JsonConvert.SerializeObject(data);
                return DMSString;
            }
            catch (Exception)
            {
                return DMSString;
            }
            finally
            {
                command = null;
                strQuery = null;
                DMSString = null;
            }
        }
        #endregion

        #region GET Transactions Data

        public string GetTransactions(int tranType, string DtTime)
        {
            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string TransactionsString = string.Empty;
            try
            {
                strQuery = GetTransactionsStringBuilder(tranType);
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESBTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                TransactionsString = JsonConvert.SerializeObject(dt);
                return TransactionsString;
            }
            catch (Exception)
            {
                return TransactionsString;
            }
            finally
            {
                command = null;
                strQuery = null;
                TransactionsString = null;
            }
        }
        #endregion

        #region GET City  Data
        public string GetCityCashData(string DtTime, string citycashstatus, string EndDateTime, string transactiontype, int projectid, string mobnumber)
        {
            string CityCashString = string.Empty;
            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();

            try
            {
                if (mobnumber == "" || mobnumber == null)
                {
                    strQuery = GetQueryofCityCashData(citycashstatus, transactiontype, projectid);
                }
                else
                {
                    strQuery = GetQueryofCityCashDataByMobileNumber(citycashstatus, transactiontype, mobnumber);
                }
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", DtTime);
                    command.Parameters.AddWithValue("@EndDateTime", EndDateTime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["PBBC_APIConnString"].ConnectionString))
                //using (SqlConnection connection = new SqlConnection("server=10.15.20.132;Database=ESB_Transactions;Persist Security Info=True;uid=a_devcomdb_prd;password=Sql@admin;"))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];

                CityCashString = JsonConvert.SerializeObject(dt);
                return CityCashString;
            }
            catch (Exception ex)
            {
                return CityCashString;
            }
            finally
            {
                command = null;
                strQuery = null;
                CityCashString = null;
            }
        }
        #endregion

        #region GET CMS Data
        public string GetCMSData(string DtTime, string EndDateTime, string cmsstatus, string PartnerID, string productcode)
        {
            string CMSAPIString = string.Empty;
            SqlCommand command = new SqlCommand();
            string strQuery = string.Empty;
            DataTable dt = new DataTable();

            try
            {

                strQuery = GetQueryofCMSAPIData(cmsstatus, PartnerID, productcode);

                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", DtTime);
                    command.Parameters.AddWithValue("@EndDateTime", EndDateTime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["PBBC_APIConnString"].ConnectionString))
                //using (SqlConnection connection = new SqlConnection("server=10.15.20.132;Database=ESB_Transactions;Persist Security Info=True;uid=a_devcomdb_prd;password=Sql@admin;"))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    adt.Fill(dt);

                }

                CMSAPIString = JsonConvert.SerializeObject(dt);
                return CMSAPIString;
            }
            catch (Exception ex)
            {
                return CMSAPIString;
            }
            finally
            {
                command = null;
                strQuery = null;
                CMSAPIString = null;
            }
        }
        #endregion

        #region GET AODashBoard
        public string GetAODashBoardData(string DtTime, string tabname)
        {
            string AODashBoard = string.Empty;
            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            DateTime datetime;

            try
            {
                if (DtTime == "")
                {
                    datetime = DateTime.Now;
                    DtTime = String.Format("{0:yyyy/M/d}", datetime);
                }

                if (tabname == "AODashBoard")
                {
                    strQuery = "AODashBoard";
                    using (command = new SqlCommand(strQuery.ToString()))
                    {
                        command.Parameters.AddWithValue("@SELECTEDDATE", Convert.ToString(DtTime));
                    }
                }
                else if (tabname == "AODashBoard_pending")
                {
                    strQuery = "AODashBoard_pending";
                    command = new SqlCommand(strQuery.ToString());
                }
                else if (tabname == "AODashBoardByChannel")
                {
                    strQuery = "AODashBoardByChannel";
                    using (command = new SqlCommand(strQuery.ToString()))
                    {
                        command.Parameters.AddWithValue("@SELECTEDDATE", Convert.ToString(DtTime));
                    }
                }
                else if (tabname == "AODashBoard_pendingByChannel")
                {
                    strQuery = "AODashBoard_pendingByChannel";
                    command = new SqlCommand(strQuery.ToString());
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DMSConnString"].ConnectionString))
                //using (SqlConnection connection = new SqlConnection("server=10.71.36.52,15952;Database=SERVOSTREAMS_UAT;Persist Security Info=True;uid=a_dms_uat;password=Sql@fino2016;"))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 500;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];

                AODashBoard = JsonConvert.SerializeObject(dt);
                return AODashBoard;
            }
            catch (Exception ex)
            {
                return AODashBoard;
            }
            finally
            {
                command = null;
                strQuery = null;
                AODashBoard = null;
            }
        }
        #endregion

        #region GET Passthrough Data

        public string GetPassthroughData(string DtTime)
        {
            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string PassthroughString = string.Empty;

            try
            {
                strQuery = GetPassthroughStringBuilder();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ESB_NPCIConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    adt.Fill(dt);

                }
                PassthroughString = JsonConvert.SerializeObject(dt);
                return PassthroughString;
            }
            catch (Exception)
            {
                return PassthroughString;
            }
            finally
            {
                command = null;
                strQuery = null;
                PassthroughString = null;
            }
        }
        #endregion

        #region GET AUA ASA Data

        public string GetAUAASATransactions(AUAASARequest objRequest)
        {
            DateTime datetime;
            string dattime;
            string AUAASA = string.Empty;
            string DtTime = Convert.ToString(objRequest.DateTime);
            string RequestType = Convert.ToString(objRequest.RequestType);
            string Instance_Id = Convert.ToString(objRequest.Instance_Id);
            string Authres_err = Convert.ToString(objRequest.Authres_err);
            string Log_Date = Convert.ToString(objRequest.Log_Date);

            if (DtTime != "")
            {
                if (DtTime == "2")
                {
                    datetime = DateTime.Now.AddMinutes(-2);
                    dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                   
                }
                else if (DtTime == "1")
                {
                    datetime = DateTime.Now.AddMinutes(-60);
                    dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime);// +":00.000";                    
                }
                else
                {
                    datetime = DateTime.Now;
                    dattime = String.Format("{0:yyyy/M/d}", datetime);
                }

                SqlCommand command = new SqlCommand();
                string strQuery = string.Empty;
                DataTable dt = new DataTable();

                try
                {
                    strQuery = AUAASAQueryData(RequestType, Instance_Id, Authres_err, Log_Date);
                    using (command = new SqlCommand(strQuery.ToString()))
                    {
                        command.Parameters.AddWithValue("@DateTime", dattime);
                    }
                    using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["FINOPBAUAConnString"].ConnectionString))
                    {
                        command.Connection = connection;
                        command.Connection.Open();
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 300;
                        SqlDataAdapter adt = new SqlDataAdapter(command);
                        command.Connection.Close();
                        adt.Fill(dt);

                    }
                    AUAASA = JsonConvert.SerializeObject(dt);
                    return AUAASA;
                }
                catch (Exception)
                {
                    return AUAASA;
                }
                finally
                {
                    command = null;
                    strQuery = null;
                    AUAASA = null;
                }

            }
            return AUAASA;
        }
        #endregion

        #region GET Cache Failed Data
        public string GetCacheFailedData(string ResponseMessage, string DtTime)
        {
            DateTime datetime;
            string dattime;
            if (DtTime == "2")
            {
                datetime = DateTime.Now.AddMinutes(-2);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else if (DtTime == "1")
            {
                datetime = DateTime.Now.AddMinutes(-60);
                dattime = String.Format("{0:yyyy/M/d HH:mm}", datetime) + ":00.000";
            }
            else
            {
                datetime = DateTime.Now;
                dattime = String.Format("{0:yyyy/M/d}", datetime);
            }

            SqlCommand command = new SqlCommand();
            DataSet ds;
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string DashboardString = string.Empty;
            try
            {
                strQuery = CacheFailedResponseMessageQuery();
                using (command = new SqlCommand(strQuery.ToString()))
                {
                    command.Parameters.AddWithValue("@DateTime", dattime);
                }
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["LogTransactionConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    ds = new DataSet();
                    adt.Fill(ds);

                }
                dt = ds.Tables[0];
                DashboardString = JsonConvert.SerializeObject(dt);
                return DashboardString;
            }
            catch (Exception)
            {
                return DashboardString;
            }
            finally
            {
                command = null;
                strQuery = null;
                DashboardString = null;
            }
        }
        #endregion

        #region GET Cash Bazaar Data
        public string GetCashBazaarData(string cola, string fdate, string tdate)
        {
            SqlCommand command = new SqlCommand();
            string strQuery = string.Empty;
            DataTable dt = new DataTable();
            string CashBazaarString = string.Empty;
            try
            {
                strQuery = COLAQuery(cola, fdate, tdate);
                command = new SqlCommand(strQuery.ToString());

                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["COLAConnString"].ConnectionString))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 200;
                    SqlDataAdapter adt = new SqlDataAdapter(command);
                    command.Connection.Close();
                    adt.Fill(dt);

                }

                CashBazaarString = JsonConvert.SerializeObject(dt);
                return CashBazaarString;
            }
            catch (Exception)
            {
                return CashBazaarString;
            }
            finally
            {
                command = null;
                strQuery = null;
                CashBazaarString = null;
            }
        }
        #endregion

        //Query

        //tab 1-4

        #region DashBoard Query
        private string GetStringBuilder(int DashboardType)
        {
            StringBuilder strQuery;
            try
            {
                if (DashboardType == 1)
                {
                    strQuery = new StringBuilder();
                    strQuery.Append("select Corelation_Request into #temp from Tbl_Corelation_Log tl(nolock) where tl.RequestIn >= @DateTime and Layer_ID = 1 group by Corelation_Request ");

                    strQuery.Append("select mrt.Request_Type,ml.Layer_Name,Count(tl.Request_Flag) as NoOfRequest into #TEMP2 from #temp T2(nolock) ");
                    strQuery.Append("inner join Tbl_Corelation_Log tl with (nolock index = IX_Corelation_Request) on T2.Corelation_Request = tl.Corelation_Request ");
                    strQuery.Append("inner join temp_mstRequestType mrt with (nolock) on tl.Method_ID = mrt.Method_ID and tl.Service_ID = mrt.Service_ID ");
                    strQuery.Append("inner join temp_mstLayer ml(nolock) on tl.Layer_ID = ml.Layer_ID ");
                    strQuery.Append("inner join temp_mstServiceName msn(nolock) on tl.Service_ID = msn.Service_ID ");
                    strQuery.Append("group by mrt.Request_Type,ml.Layer_Name,tl.Request_Flag,tl.Status_Code ");
                    strQuery.Append("having tl.Request_Flag = 1 and tl.Status_Code in (1, 0) ");

                    strQuery.Append("select mrt.Request_Type,ml.Layer_Name,Count(tl.Response_Flag) as NoOfResponse into #TEMP3 from #temp T2(nolock) ");
                    strQuery.Append("inner join Tbl_Corelation_Log tl with (nolock index = IX_Corelation_Request) on T2.Corelation_Request = tl.Corelation_Request ");
                    strQuery.Append("inner join temp_mstRequestType mrt with (nolock) on tl.Method_ID = mrt.Method_ID and tl.Service_ID = mrt.Service_ID ");
                    strQuery.Append("inner join temp_mstLayer ml(nolock) on tl.Layer_ID = ml.Layer_ID ");
                    strQuery.Append("inner join temp_mstServiceName msn(nolock) on tl.Service_ID = msn.Service_ID ");
                    strQuery.Append("group by mrt.Request_Type,ml.Layer_Name,tl.Response_Flag,tl.Status_Code ");
                    strQuery.Append("having tl.Response_Flag = 1 and tl.Status_Code in (1, 0) ");

                    strQuery.Append("select x.Request_Type as [Request_Type]  ");
                    strQuery.Append(",ISNULL(x.UI, 0) as [Request_UI],ISNULL(x.BLL, 0) as [Request_BLL],ISNULL(x.ESB, 0) as [Request_ESB] ");
                    strQuery.Append(",ISNULL(y.ESB, 0) as [Response_ESB],ISNULL(y.BLL, 0) as [Response_BLL],ISNULL(y.UI, 0) as [Response_UI] ");
                    strQuery.Append("from (select Request_Type,UI,BLL,ESB ");
                    strQuery.Append("from (select * from #TEMP2) A pivot(sum(NoOfRequest) for Layer_Name in (UI, BLL, ESB)) as P) as x ");
                    strQuery.Append("inner join (select Request_Type,UI,BLL,ESB ");
                    strQuery.Append("from (select *  from #TEMP3) B pivot(sum(NoOfResponse) for Layer_Name in (UI, BLL, ESB)) as P) as y on x.Request_Type = y.Request_Type order by x.Request_Type ");

                    strQuery.Append("drop table #temp ");
                    strQuery.Append("drop table #temp2 ");
                    strQuery.Append("drop table #temp3 ");
                    return strQuery.ToString();
                }
                else if (DashboardType == 2)
                {
                    strQuery = new StringBuilder();
                    strQuery.Append("select Corelation_Request, RequestIn, RequestOut,Method_ID,Service_ID into #temp from Tbl_Corelation_Log with (nolock) where Layer_ID=1 and Service_ID<>0 and RequestIn >=@DateTime and Status_Code in(1,0)   ");
                    strQuery.Append("Update b Set RequestOut = a.RequestOut from #temp b inner join Tbl_Corelation_Log a with (nolock index = IX_Corelation_Request) on a.Corelation_Request = b.Corelation_Request ");
                    strQuery.Append("Select x.Request_Type as [Request_Type],ISNULL(x.timeslot1,0) as [Response_UI_0_1Sec], ");
                    strQuery.Append("ISNULL(y.timeslot2,0) as [Response_UI_1_3Sec], ISNULL(z.timeslot2,0) as [Response_UI_3_Sec] ");
                    strQuery.Append("from (select mrt.Request_Type, Count(DATEDIFF(ss,tl.RequestIn,tl.RequestOut)) as timeslot1 ");
                    strQuery.Append("from #temp tl (nolock) ");
                    strQuery.Append("inner join temp_mstRequestType mrt (nolock)on tl.Method_ID = mrt.Method_ID and tl.Service_ID = mrt.Service_ID  ");
                    strQuery.Append("where DATEDIFF(ss,tl.RequestIn,tl.RequestOut)<=1 group by mrt.Request_Type) as x ");
                    strQuery.Append("left outer join (select mrt.Request_Type, Count(DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)) as timeslot2 from #temp tl1  ");
                    strQuery.Append("inner join temp_mstRequestType mrt (nolock) on tl1.Method_ID = mrt.Method_ID and tl1.Service_ID = mrt.Service_ID ");
                    strQuery.Append("where DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)>=1 and DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)<=3 ");
                    strQuery.Append("group by mrt.Request_Type ) as y on x.Request_Type=y.Request_Type left outer join ");
                    strQuery.Append("(select mrt.Request_Type,Count(DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)) as timeslot2 from #temp tl1 ");
                    strQuery.Append("inner join temp_mstRequestType mrt (nolock) on tl1.Method_ID = mrt.Method_ID and tl1.Service_ID = mrt.Service_ID ");
                    strQuery.Append("where DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)>3 ");
                    strQuery.Append("group by mrt.Request_Type )as Z on x.Request_Type=Z.Request_Type ");
                    strQuery.Append("where x.Request_Type is not NULL order by x.Request_Type  ");

                    strQuery.Append(" drop table #temp  ");
                    return strQuery.ToString();
                }
                else if (DashboardType == 3)
                {
                    strQuery = new StringBuilder();
                    strQuery.Append("select Request_Type,Layer_Name,Status_Code,Node_IP_Address,count(1) as [Count] ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with(nolocK) ");
                    strQuery.Append("left outer join temp_mstRequestType mstR (nolock) on ");
                    strQuery.Append("x.Method_ID=mstr.Method_ID and x.Service_ID=mstr.Service_ID ");
                    strQuery.Append("left outer join temp_mstLayer L (nolock) on x.Layer_ID=L.Layer_ID ");
                    strQuery.Append("where (RequestIn>=@DateTime or RequestOut>=@DateTime) ");
                    strQuery.Append("and Response_Flag='1' and Status_Code not in(1,0) ");
                    strQuery.Append("group by Request_Type,Layer_Name,Status_Code,Node_IP_Address order by 3 ");
                    return strQuery.ToString();
                }
                else
                {
                    strQuery = new StringBuilder();
                    strQuery.Append("select Corelation_Request, Request_Type, Layer_Name, Response_Message, Node_IP_Address, count(1) as [Count], RequestOut from  ");
                    strQuery.Append("(select Corelation_Request, Request_Type, Layer_Name, Response_Message, Node_IP_Address, RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with (nolock  index=Ix_RequestOut) left outer join temp_mstRequestType mstR on x.Method_ID = mstr.Method_ID and x.Service_ID = mstr.Service_ID left outer join temp_mstLayer L on x.Layer_ID = L.Layer_ID ");
                    strQuery.Append("where RequestOut>=@DateTime and Response_Flag='1' and Response_Message<>'' and Layer_Name='UI' and Response_Message like '%Failed%' ");
                    strQuery.Append("Union all ");
                    strQuery.Append("select Corelation_Request, Request_Type, Layer_Name, Response_Message, Node_IP_Address, RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with (nolock  index=Ix_RequestOut) left outer join temp_mstRequestType mstR on x.Method_ID = mstr.Method_ID and x.Service_ID = mstr.Service_ID left outer join temp_mstLayer L on x.Layer_ID = L.Layer_ID ");
                    strQuery.Append("where RequestOut>=@DateTime and Response_Flag='1' and Response_Message<>'' and Layer_Name='UI'and Response_Message like '%Error%' and Response_Message Not IN ('Remote server return an error')");
                    strQuery.Append("Union all ");
                    strQuery.Append("select Corelation_Request, Request_Type, Layer_Name, Response_Message, Node_IP_Address, RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with (nolock  index=Ix_RequestOut) left outer join temp_mstRequestType mstR on x.Method_ID = mstr.Method_ID and x.Service_ID = mstr.Service_ID left outer join temp_mstLayer L on x.Layer_ID = L.Layer_ID ");
                    strQuery.Append("where RequestOut>=@DateTime and Response_Flag='1' and mstR.Service_ID  not in('3','19') and Layer_Name='UI'and Response_Message like '%Primary%' ");
                    strQuery.Append("Union all  ");
                    strQuery.Append("select Corelation_Request, Request_Type, Layer_Name, Response_Message, Node_IP_Address, RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with (nolock  index=Ix_RequestOut) left outer join temp_mstRequestType mstR on x.Method_ID = mstr.Method_ID and x.Service_ID = mstr.Service_ID left outer join temp_mstLayer L on x.Layer_ID = L.Layer_ID ");
                    strQuery.Append("where RequestOut>=@DateTime and Response_Flag='1' and Response_Message<>'' and Layer_Name='UI' and Response_Message like '%Server%' and Response_Message Not IN ('Remote server return an error')");
                    strQuery.Append("Union all  ");
                    strQuery.Append("select Corelation_Request, Request_Type, Layer_Name, Response_Message, Node_IP_Address, RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with (nolock  index=Ix_RequestOut) left outer join temp_mstRequestType mstR on x.Method_ID = mstr.Method_ID and x.Service_ID = mstr.Service_ID left outer join temp_mstLayer L on x.Layer_ID = L.Layer_ID ");
                    strQuery.Append("where RequestOut>=@DateTime and Response_Flag='1' and Response_Message<>'' and Layer_Name='UI' and Response_Message like '%IMPS Disable%') As temp ");
                    strQuery.Append("group by Corelation_Request,Request_Type,Layer_Name,Response_Message,Node_IP_Address,RequestOut order by RequestOut desc ");
                    return strQuery.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                strQuery = null;
            }
        }
        #endregion

        //tab 5
        #region ESB Transactions Query
        private string GetQueryOfESBData(int ESBTab, string TransactionName, string TType, string appid)
        {
            String qry;
            try
            {
                if (ESBTab == 1)
                {
                    string pcode = WebConfigurationManager.AppSettings["PCODE"];
                    string pcodename = WebConfigurationManager.AppSettings["PCODEName"];

                    string strPCODEIMPS = string.Empty;
                    string strPCODENEFT = string.Empty;
                    string strPCODEIMPS_NEFT = string.Empty;

                    string[] pcodearr = pcode.Split(',');
                    string[] pcodenamearr = pcodename.Split(',');

                    if (pcodearr.Length == pcodenamearr.Length)
                    {
                        for (int k = 0; k < pcodearr.Length; k++)
                        {
                            pcodearr[k] = pcodearr[k].Trim();
                        }

                        for (int m = 0; m < pcodearr.Length; m++)
                        {
                            pcodenamearr[m] = pcodenamearr[m].Trim();
                            strPCODEIMPS += "'" + pcodearr[m] + "IMPS'" + ",";
                            strPCODENEFT += "'" + pcodearr[m] + "NEFT'" + ",";
                            strPCODEIMPS_NEFT += " When R.PCODE = '" + pcodearr[m] + "IMPS' Then '" + pcodenamearr[m] + "IMPS'";
                            strPCODEIMPS_NEFT += " When R.PCODE = '" + pcodearr[m] + "NEFT' Then '" + pcodenamearr[m] + "NEFT'";
                        }
                    }

                    qry = " DECLARE @Localdate DATETIME = convert(VARCHAR(8), GETDATE(), 112) " +
                          " SELECT Trace,X_Correlation_ID,MSGTYPE,PCODE,appid,Amount INTO #TEMP	 " +
                          " FROM dbo.TransactionsRequest R WITH (NOLOCK INDEX = IX_LOCAL_DATE) 	 " +
                          " WHERE r.LOCAL_Date = @Localdate	AND r.LOCAL_TIME BETWEEN @STime	AND @ETime AND R.MSGTYPE = 0 " +

                          " SELECT RESPCODE,Trace,X_Correlation_ID,MSGTYPE,PCODE,Amount,TransResponse_Id INTO #TEMP2 " +
                          " FROM dbo.TransactionsResponse R WITH (NOLOCK INDEX = IX_LOCAL_DATE) " +
                          " WHERE r.LOCAL_Date = @Localdate	AND r.LOCAL_TIME BETWEEN @STime	AND @ETime AND R.MSGTYPE = 0 " +

                          " SELECT Trace,X_Correlation_ID,max(TransResponse_Id) AS TransResponse_Id INTO #temp3 " +
                          " FROM #Temp2	WHERE MSGTYPE = 0 GROUP BY Trace ,X_Correlation_ID " +

                          "select appid, case " +
                          "when R.PCODE ='DMTIMPSP2A' then 'IMPS Walk-In' 	 " +
                          "when R.PCODE in('IMPSFTP2A') then 'IMPS PPI' 	 " +
                          "when R.PCODE in('IMPSBENVC','DMTIMPSBENV','IMPSBENV') then 'Bene verification'	 " +
                          "when R.PCODE IN ('DMTNEFT') then 'Walkin NEFT' 	 " +
                          "when R.PCODE IN ('NEFT','NEFTC') then 'CASA NEFT' 	 " +
                          "when R.PCODE in('DMTBILLPAY1','BILLPAY1') then 'BillPay' 	 " +
                          "when R.PCODE in('DMTTOPUP1','TOPUP1') then 'Recharge' 	 " +
                          "when R.PCODE in('CASHTXFR') then 'Cash-In' 	 " +
                          "when R.PCODE in('P2MPURCH') then 'PPI-Purchase'	 " +
                          "when R.PCODE in('01') then 'Pay To Mobile'	" +
                          "when R.PCODE in('INSGEN12','INSGEN12C') then 'HDFC ERGO Tanstype'	  	 " +
                          "when R.PCODE in('INSGEN22TW','INSGEN22TWC') then 'Two Wheeler'	  	 " +
                          "when R.PCODE in('INSGEN22','INSGEN22C') AND R.Amount in (2950,3540,4130,4720) then 'ICICI LOMBARD(Family Floater)'	 " +
                          "when R.PCODE in('INSGEN22','INSGEN22C') AND R.Amount in (400,475,525,600,750,950)  then 'ICICI LOMBARD(Hospicash)'	 " +
                          "when R.PCODE in('INSGEN42','INSGEN42C') then 'EXIDE'	 " +
                          "when R.PCODE in('INSGEN33','INSGEN33C') then 'Prudential Shubh Raksha'	 " +
                          "when R.PCODE in('INSGEN22VB','INSGEN22VBC') then 'ICICI Lombard(Dengue & Malaria Cover)'	 " +
                          "when R.PCODE in('INSGEN32','INSGEN32C') then 'Prudential'	 " + strPCODEIMPS_NEFT +
                          " end as TransactionType, count(distinct r.trace) as Initiated	 " +
                          " ,SUM(case when RR.RESPCODE='0' then 1 else 0 end) as Success	" +
                          " ,SUM(case when ((RR.RESPCODE not in ('91','8','0','00')  " +
                          " and R.PCODE in (" + strPCODEIMPS + "'DMTIMPSP2A','IMPSBENVC','DMTIMPSBENV','IMPSFTP2A','IMPSBENV')) or  " +
                          " (R.PCODE in('INSGEN12','INSGEN12C','INSGEN22','INSGEN22C','INSGEN42','INSGEN42C','INSGEN32','INSGEN32C') and RR.RESPCODE In ('1','100030')) or  " +
                          " (R.PCODE in(" + strPCODENEFT + "'DMTBILLPAY1','DMTTOPUP1','DMTNEFT','P2MPURCH','TOPUP1','BILLPAY1','CASHTXFR','CASHDOLAC','01') and RR.RESPCODE In ('1','700420')))   then 1 else 0 end) as Failed, " +

                          " SUM(case when ((RR.RESPCODE in ('91','8')  " +
                          " and R.PCODE in (" + strPCODEIMPS + "'DMTIMPSP2A','IMPSBENVC','DMTIMPSBENV','IMPSFTP2A','IMPSBENV')) or  " +
                          " (R.PCODE in('DMTBILLPAY1','DMTTOPUP1','TOPUP1','BILLPAY1','CASHTXFR','01') and RR.RESPCODE in('2','')))   then 1 else 0 end) as Awaiting	 " +
                          " from #TEMP R  with (nolock) " +
                          " left outer merge join  #TEMP3 TT with(nolock) " +
                          " on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                          " inner merge join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                          " where R.MSGTYPE =0 " +
                          " group by  appid,case " +
                          " when R.PCODE ='DMTIMPSP2A' then 'IMPS Walk-In' 	 " +
                          " when R.PCODE in('IMPSFTP2A') then 'IMPS PPI' 	 " +
                          " when R.PCODE in('IMPSBENVC','DMTIMPSBENV','IMPSBENV') then 'Bene verification'	 " +
                          "when R.PCODE IN ('DMTNEFT') then 'Walkin NEFT' 	 " +
                          "when R.PCODE IN ('NEFT','NEFTC') then 'CASA NEFT' 	 " +
                          " when R.PCODE in('DMTBILLPAY1','BILLPAY1') then 'BillPay' 	 " +
                          " when R.PCODE in('DMTTOPUP1','TOPUP1') then 'Recharge' 	 " +
                          " when R.PCODE in('CASHTXFR') then 'Cash-In' 	 " +
                          " when R.PCODE in('P2MPURCH') then 'PPI-Purchase' 	 " +
                          " when R.PCODE in('01') then 'Pay To Mobile'" +
                          " when R.PCODE in('INSGEN12','INSGEN12C') then 'HDFC ERGO Tanstype'	  	 " +
                          "when R.PCODE in('INSGEN22TW','INSGEN22TWC') then 'Two Wheeler'	  	 " +
                          "when R.PCODE in('INSGEN22','INSGEN22C') AND R.Amount in (2950,3540,4130,4720) then 'ICICI LOMBARD(Family Floater)'	 " +
                          "when R.PCODE in('INSGEN22','INSGEN22C') AND R.Amount in (400,475,525,600,750,950)  then 'ICICI LOMBARD(Hospicash)'	 " +
                          "when R.PCODE in('INSGEN42','INSGEN42C') then 'EXIDE'	 " +
                          "when R.PCODE in('INSGEN33','INSGEN33C') then 'Prudential Shubh Raksha'	 " +
                          "when R.PCODE in('INSGEN22VB','INSGEN22VBC') then 'ICICI Lombard(Dengue & Malaria Cover)'	 " +
                          "when R.PCODE in('INSGEN32','INSGEN32C') then 'Prudential'	 " + strPCODEIMPS_NEFT +
                          " end " +
                          " drop table #TEMP	 " +
                          " drop table #TEMP2" +
                          " drop table #TEMP3";

                    return qry;
                }
                else if (ESBTab == 2 && TransactionName == "IMPS Walk-In" && TType == "Failed")
                {
                    qry = "	select * into #TEMP	" +
                          "	from dbo.TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                          "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                          "	and r.LOCAL_TIME between @STime and @ETime" +
                          "	and R.MSGTYPE =0	" +

                          "	select * into #TEMP2	" +
                          "	from dbo.TransactionsResponse R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                          "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                          "	and r.LOCAL_TIME between @STime and @ETime" +
                          "	and R.MSGTYPE =0	" +

                          "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                          "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

                          "   select Isnull(MI.BankName,'Manual Posting') as Bankname,count(distinct R.Trace) as [Count] " +
                          "   from  #TEMP R  with (nolock) " +
                          "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                          "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                          "   left outer join  mstIFSCMapping MI With(nolock) on left(BeneIFSC,4)=left(MI.IFSC,4) " +
                          "   where R.MSGTYPE =0  and RR.RESPCODE not in ('91','8','0','00') and R.PCODE in ('DMTIMPSP2A') AND appid='" + appid + "' group by MI.BankName" +
                          "	drop table #TEMP	" +
                          "	drop table #TEMP2	" +
                          "	drop table #TEMP3";
                    return qry;
                }
                else if (ESBTab == 3 && TransactionName == "IMPS Walk-In" && TType == "Awaiting")
                {
                    qry = "	select * into #TEMP	" +
                    "	from dbo.TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                    "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                    "	and r.LOCAL_TIME between @STime and @ETime" +
                    "	and R.MSGTYPE =0	" +

                    "	select * into #TEMP2	" +
                    "	from dbo.TransactionsResponse R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                    "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                    "	and r.LOCAL_TIME between @STime and @ETime" +
                    "	and R.MSGTYPE =0	" +

                    "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                    "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

                    "   select Isnull(MI.BankName,'Manual Posting') as Bankname,count(distinct R.Trace) as [Count] " +
                    "   from  #TEMP R  with (nolock) " +
                    "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                    "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                    "   left outer join  mstIFSCMapping MI With(nolock) on left(BeneIFSC,4)=left(MI.IFSC,4) " +
                    "   where R.MSGTYPE =0  and RR.RESPCODE in ('91','8') and R.PCODE in ('DMTIMPSP2A') AND appid='" + appid + "' group by MI.BankName" +
                    "	drop table #TEMP	" +
                    "	drop table #TEMP2	" +
                    "	drop table #TEMP3";
                    return qry;
                }
                else if (ESBTab == 2 && TransactionName == "Bene verification" && TType == "Failed")
                {
                    qry = "	select * into #TEMP	" +
                    "	from dbo.TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                    "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                    "	and r.LOCAL_TIME between @STime and @ETime" +
                    "	and R.MSGTYPE =0	" +

                    "	select * into #TEMP2	" +
                    "	from dbo.TransactionsResponse R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                    "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                    "	and r.LOCAL_TIME between @STime and @ETime" +
                    "	and R.MSGTYPE =0	" +

                    "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                    "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

                    "   select Isnull(MI.BankName,'Manual Posting') as Bankname,count(distinct R.Trace) as [Count] " +
                    "   from  #TEMP R  with (nolock) " +
                    "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                    "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                    "   left outer join mstIFSCMapping MI With(nolock) on left(BeneIFSC,4)=left(MI.IFSC,4) " +
                    "   where R.MSGTYPE =0  and RR.RESPCODE not in ('91','8','0','00') and R.PCODE in ('IMPSBENVC','DMTIMPSBENV','IMPSBENV') AND appid='" + appid + "' group by MI.BankName" +
                    "	drop table #TEMP	" +
                    "	drop table #TEMP2	" +
                    "	drop table #TEMP3";
                    return qry;
                }
                else if (ESBTab == 3 && TransactionName == "Bene verification" && TType == "Awaiting")
                {
                    qry = "	select * into #TEMP	" +
                    "	from dbo.TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                    "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                    "	and r.LOCAL_TIME between @STime and @ETime" +
                    "	and R.MSGTYPE =0	" +

                    "	select * into #TEMP2	" +
                    "	from dbo.TransactionsResponse R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                    "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                    "	and r.LOCAL_TIME between @STime and @ETime" +
                    "	and R.MSGTYPE =0	" +

                    "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                    "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

                    "   select Isnull(MI.BankName,'Manual Posting') as Bankname,count(distinct R.Trace) as [Count] " +
                    "   from  #TEMP R  with (nolock) " +
                    "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                    "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                    "   left outer join mstIFSCMapping MI With(nolock) on left(BeneIFSC,4)=left(MI.IFSC,4) " +
                    "   where R.MSGTYPE =0  and RR.RESPCODE in ('91','8') and R.PCODE in ('IMPSBENVC','DMTIMPSBENV') AND appid='" + appid + "' group by MI.BankName" +
                    "	drop table #TEMP	" +
                    "	drop table #TEMP2	" +
                    "	drop table #TEMP3";
                    return qry;
                }
                else
                {
                    qry = "	select * into #TEMP	" +
                    "	from dbo.TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                    "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                    "	and r.LOCAL_TIME between @STime and @ETime" +
                    "	and R.MSGTYPE =0	" +

                    "	select * into #TEMP2	" +
                    "	from dbo.TransactionsResponse R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                    "	where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                    "	and r.LOCAL_TIME between @STime and @ETime" +
                    "	and R.MSGTYPE =0	" +

                    "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                    "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

                    "   select Isnull(MI.BankName,'Manual Posting') as Bankname,count(distinct R.Trace) as [Count] " +
                    "   from  #TEMP R  with (nolock) " +
                    "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                    "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                    "   left outer join mstIFSCMapping MI With(nolock) on left(BeneIFSC,4)=left(MI.IFSC,4) " +
                    "   where R.MSGTYPE =0  and RR.RESPCODE in ('1','700420') and R.PCODE in ('DMTNEFT') AND appid='" + appid + "' group by MI.BankName" +
                    "	drop table #TEMP	" +
                    "	drop table #TEMP2	" +
                    "	drop table #TEMP3";
                    return qry;
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                qry = null;
            }
        }

        #endregion

        //CMS Clients Query
        #region Cash Collection Query
        private string GetQueryOfCashCollection(string TransactionName)
        {
            string qry = "";
            try
            {
                if (TransactionName == "CMSSummary")
                {
                    qry = "select * into #TEMP	 " +
                          " from dbo.TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) 	 " +
                          " where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112) " +
                          " and r.LOCAL_TIME between @STime and @ETime  " +
                          " And PCODE IN ('CASHDAIR','CASHDAIRE','CASHDAUF','CASHDBAJA','CASHDDISF','CASHDHERO','CASHDIGOL','CASHDIPRU','CASHDJFS','CASHDLNT','CASHDLPL','CASHDOLAA','CASHDOLAC','CASHDOLAS','CASHDPAR','CASHDPTM','CASHDRNR','CASHDSFAX','CASHDSHRF','CASHDSHRT','CASHDSVA','CASHDSWG','CASHDUJW','CASHDHICB','CASHDOLAO','CASHDIIFL','CASHWALLA','CASHDMAHI','CASHWRIVG','CASHDOLAN','CASHDMMFS','CASHDPYSO','CASHDBPAY','CASHDDLIT','CASHDHERO1','IGoldLead','CASHDJIO','CASHDSATIN','CASHDASVD','CASHDSVMFI','CASHDLOKS','CASHDSVMFI1','CASHDEXDR','CASHDIPRUR') " +
                          " and R.MSGTYPE =0	 " +

                          " select * into #TEMP2	 " +
                          " from dbo.TransactionsResponse R  with (nolock INDEX = IX_LOCAL_DATE) 	 " +
                          " where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112) " +
                          " and r.LOCAL_TIME between @STime and @ETime  " +
                          " and R.MSGTYPE =0 " +

                          " Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                          " From #Temp2 " +
                          " where MSGTYPE =0 " +
                          " Group By Trace,X_Correlation_ID " +

                          "select appid, case " +
                          " when R.PCODE in('CASHDAIR') then 'Airpay'  " +
                          " when R.PCODE in('CASHDAIRE') then 'Airpay-Eureka' " +
                          " when R.PCODE in('CASHDAUF') then 'AU-Finance' " +
                          " when R.PCODE in('CASHDBAJA') then 'Bajaj Auto' " +
                          " when R.PCODE in('CASHDDISF') then 'Disha Fincare' " +
                          " when R.PCODE in('CASHDHERO') then 'Hero FinCorp' " +
                          " when R.PCODE in('CASHDIGOL') then 'Igold' " +
                          " when R.PCODE in('CASHDIPRU') then 'ICICIPRULIFE' " +
                          " when R.PCODE in('CASHDJFS') then 'JFS' " +
                          " when R.PCODE in('CASHDLNT') then 'LnT' " +
                          " when R.PCODE in('CASHDLPL') then 'Dr. LPL' " +
                          " when R.PCODE in('CASHDOLAA') then 'OLA AUTO' " +
                          " when R.PCODE in('CASHDOLAC') then 'OLA CAB' " +
                          " when R.PCODE in('CASHDOLAS') then 'OLA Store' " +
                          " when R.PCODE in('CASHDPAR') then 'Paras' " +
                          " when R.PCODE in('CASHDPTM') then 'PayTM' " +
                          " when R.PCODE in('CASHDRNR') then 'RoadRunnr' " +
                          " when R.PCODE in('CASHDSFAX') then 'Shadow Fax' " +
                          " when R.PCODE in('CASHDSHRF') then 'SHRIRAM CITY UNION FINANCE' " +
                          " when R.PCODE in('CASHDSHRT') then 'Shriram Transport' " +
                          " when R.PCODE in('CASHDSVA') then 'Svatantra' " +
                          " when R.PCODE in('CASHDSWG') then 'Swiggy' " +
                          " when R.PCODE in('CASHDUJW') then 'Ujjivan' " +
                          " when R.PCODE in('CASHDHICB') then 'Wheels EMI' " +
                          " when R.PCODE in('CASHDOLAO') then 'OLA OFT' " +
                          " when R.PCODE in('CASHDIIFL') then 'IIFL' " +
                          " when R.PCODE in('CASHWALLA') then 'Allana' " +
                          " when R.PCODE in('CASHDMAHI') then 'Mahindra MRHFL' " +
                          " when R.PCODE in('CASHWRIVG') then 'Rivigo' " +
                          " when R.PCODE in('CASHDOLAN') then 'OLA ANI' " +
                          " when R.PCODE in('CASHDMMFS') then 'Mahindra MMFSL' " +
                          " when R.PCODE in('CASHDPYSO') then 'Payso' " +
                          " when R.PCODE in('CASHDBPAY') then 'BPayLoyalty' " +
                          " when R.PCODE in('CASHDDLIT') then 'D.Light' " +
                          " when R.PCODE in('CASHDHERO1') then 'HeroFinCorp Customer' " +
                          " when R.PCODE in('IGoldLead') then 'IGold Lead' " +
                          " when R.PCODE in('CASHDJIO') then 'CASHDJIO' " +
                          " when R.PCODE in('CASHDSATIN') then 'SATIN' " +
                          " when R.PCODE in('CASHDASVD') then 'Ashirwaad' " +
                          " when R.PCODE in('CASHDSVMFI') then 'Svatantra MFI' " +
                          " when R.PCODE in('CASHDLOKS') then 'Lok Suvidha' " +
                          " when R.PCODE in('CASHDSVMFI1') then 'Svatantra MFI1' " +
                          " when R.PCODE in('CASHDEXDR') then 'Exide Life renewal' " +
                          " when R.PCODE in('CASHDIPRUR') then 'ICICI Prudential Renewal' " +
                          " end as TransactionType, count(distinct r.trace) as Initiated	 " +
                          " ,SUM(case when RR.RESPCODE='0' then 1 else 0 end) as Success	" +
                          " ,SUM(case when ((R.PCODE in('CASHDAIR','CASHDAIRE','CASHDAUF','CASHDBAJA','CASHDDISF','CASHDHERO','CASHDIGOL','CASHDIPRU','CASHDJFS','CASHDLNT','CASHDLPL','CASHDOLAA','CASHDOLAC', " +
                          " 'CASHDOLAS','CASHDPAR','CASHDPTM','CASHDRNR','CASHDSFAX','CASHDSHRF','CASHDSHRT','CASHDSVA','CASHDSWG','CASHDUJW','CASHDHICB','CASHDOLAO','CASHDIIFL', " +
                          " 'CASHWALLA','CASHDMAHI','CASHWRIVG','CASHDOLAN','CASHDMMFS','CASHDPYSO','CASHDBPAY','CASHDDLIT','CASHDHERO1','IGoldLead','CASHDJIO','CASHDSATIN','CASHDASVD'," +
                          " 'CASHDSVMFI','CASHDLOKS','CASHDSVMFI1','CASHDEXDR','CASHDIPRUR') and RR.RESPCODE In ('1','700420')))   then 1 else 0 end) as Failed	 " +
                          " from #TEMP R  with (nolock) " +
                          " left outer join  #TEMP3 TT with(nolock) " +
                          " on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                          " inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                          " where R.MSGTYPE =0 " +
                          " group by  appid,case " +
                          " when R.PCODE in('CASHDAIR') then 'Airpay'  " +
                          " when R.PCODE in('CASHDAIRE') then 'Airpay-Eureka' " +
                          " when R.PCODE in('CASHDAUF') then 'AU-Finance' " +
                          " when R.PCODE in('CASHDBAJA') then 'Bajaj Auto' " +
                          " when R.PCODE in('CASHDDISF') then 'Disha Fincare' " +
                          " when R.PCODE in('CASHDHERO') then 'Hero FinCorp' " +
                          " when R.PCODE in('CASHDIGOL') then 'Igold' " +
                          " when R.PCODE in('CASHDIPRU') then 'ICICIPRULIFE' " +
                          " when R.PCODE in('CASHDJFS') then 'JFS' " +
                          " when R.PCODE in('CASHDLNT') then 'LnT' " +
                          " when R.PCODE in('CASHDLPL') then 'Dr. LPL' " +
                          " when R.PCODE in('CASHDOLAA') then 'OLA AUTO' " +
                          " when R.PCODE in('CASHDOLAC') then 'OLA CAB' " +
                          " when R.PCODE in('CASHDOLAS') then 'OLA Store' " +
                          " when R.PCODE in('CASHDPAR') then 'Paras' " +
                          " when R.PCODE in('CASHDPTM') then 'PayTM' " +
                          " when R.PCODE in('CASHDRNR') then 'RoadRunnr' " +
                          " when R.PCODE in('CASHDSFAX') then 'Shadow Fax' " +
                          " when R.PCODE in('CASHDSHRF') then 'SHRIRAM CITY UNION FINANCE' " +
                          " when R.PCODE in('CASHDSHRT') then 'Shriram Transport' " +
                          " when R.PCODE in('CASHDSVA') then 'Svatantra' " +
                          " when R.PCODE in('CASHDSWG') then 'Swiggy' " +
                          " when R.PCODE in('CASHDUJW') then 'Ujjivan' " +
                          " when R.PCODE in('CASHDHICB') then 'Wheels EMI' " +
                          " when R.PCODE in('CASHDOLAO') then 'OLA OFT' " +
                          " when R.PCODE in('CASHDIIFL') then 'IIFL' " +
                          " when R.PCODE in('CASHWALLA') then 'Allana' " +
                          " when R.PCODE in('CASHDMAHI') then 'Mahindra MRHFL' " +
                          " when R.PCODE in('CASHWRIVG') then 'Rivigo' " +
                          " when R.PCODE in('CASHDOLAN') then 'OLA ANI' " +
                          " when R.PCODE in('CASHDMMFS') then 'Mahindra MMFSL' " +
                          " when R.PCODE in('CASHDPYSO') then 'Payso' " +
                          " when R.PCODE in('CASHDBPAY') then 'BPayLoyalty' " +
                          " when R.PCODE in('CASHDDLIT') then 'D.Light' " +
                          " when R.PCODE in('CASHDHERO1') then 'HeroFinCorp Customer' " +
                          " when R.PCODE in('IGoldLead') then 'IGold Lead' " +
                          " when R.PCODE in('CASHDJIO') then 'CASHDJIO' " +
                          " when R.PCODE in('CASHDSATIN') then 'SATIN' " +
                          " when R.PCODE in('CASHDASVD') then 'Ashirwaad' " +
                          " when R.PCODE in('CASHDSVMFI') then 'Svatantra MFI' " +
                          " when R.PCODE in('CASHDLOKS') then 'Lok Suvidha' " +
                          " when R.PCODE in('CASHDSVMFI1') then 'Svatantra MFI1' " +
                          " when R.PCODE in('CASHDEXDR') then 'Exide Life renewal' " +
                          " when R.PCODE in('CASHDIPRUR') then 'ICICI Prudential Renewal' " +
                          " end " +
                          " drop table #TEMP	 " +
                          " drop table #TEMP2" +
                          " drop table #TEMP3";
                }
                else
                {
                    qry = " select * into #TEMP4 " +
                    " from dbo.TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) " +
                    " where convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112) " +
                    " And PCODE IN ('CASHDAIR','CASHDAIRE','CASHDAUF','CASHDBAJA','CASHDDISF','CASHDHERO','CASHDIGOL','CASHDIPRU','CASHDJFS','CASHDLNT','CASHDLPL','CASHDOLAA','CASHDOLAC', " +
                    " 'CASHDOLAS','CASHDPAR','CASHDPTM','CASHDRNR','CASHDSFAX','CASHDSHRF','CASHDSHRT','CASHDSVA','CASHDSWG','CASHDUJW','CASHDHICB','CASHDOLAO','CASHDIIFL', " +
                    " 'CASHWALLA','CASHDMAHI','CASHWRIVG','CASHDOLAN','CASHDMMFS','CASHDPYSO','CASHDBPAY','CASHDDLIT','CASHDHERO1','IGoldLead','CASHDJIO','CASHDSATIN','CASHDASVD', " +
                    " 'CASHDSVMFI','CASHDLOKS','CASHDSVMFI1','CASHDEXDR','CASHDIPRUR') " +
                    " and r.LOCAL_TIME between @STime and @ETime  " +
                    " and R.MSGTYPE =0 " +

                    " select * into #TEMP5 " +
                    " from dbo.TransactionsResponse R  with (nolock INDEX = IX_LOCAL_DATE) " +
                    " where X_Correlation_ID In (Select Distinct(X_Correlation_ID) From #Temp4) " +

                    " Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #Temp6 " +
                    " From #Temp5 " +
                    " where MSGTYPE =0  " +
                    " Group By Trace,X_Correlation_ID " +

                    " select appid, case " +
                    " when R.PCODE in('CASHDAIR') then 'Airpay'  " +
                    " when R.PCODE in('CASHDAIRE') then 'Airpay-Eureka'  " +
                    " when R.PCODE in('CASHDAUF') then 'AU-Finance'  " +
                    " when R.PCODE in('CASHDBAJA') then 'Bajaj Auto'  " +
                    " when R.PCODE in('CASHDDISF') then 'Disha Fincare'  " +
                    " when R.PCODE in('CASHDHERO') then 'Hero FinCorp'  " +
                    " when R.PCODE in('CASHDIGOL') then 'Igold'  " +
                    " when R.PCODE in('CASHDIPRU') then 'ICICIPRULIFE'  " +
                    " when R.PCODE in('CASHDJFS') then 'JFS'  " +
                    " when R.PCODE in('CASHDLNT') then 'LnT'  " +
                    " when R.PCODE in('CASHDLPL') then 'Dr. LPL'  " +
                    " when R.PCODE in('CASHDOLAA') then 'OLA AUTO'  " +
                    " when R.PCODE in('CASHDOLAC') then 'OLA CAB'  " +
                    " when R.PCODE in('CASHDOLAS') then 'OLA Store'  " +
                    " when R.PCODE in('CASHDPAR') then 'Paras'  " +
                    " when R.PCODE in('CASHDPTM') then 'PayTM'  " +
                    " when R.PCODE in('CASHDRNR') then 'RoadRunnr'  " +
                    " when R.PCODE in('CASHDSFAX') then 'Shadow Fax'  " +
                    " when R.PCODE in('CASHDSHRF') then 'SHRIRAM CITY UNION FINANCE'  " +
                    " when R.PCODE in('CASHDSHRT') then 'Shriram Transport'  " +
                    " when R.PCODE in('CASHDSVA') then 'Svatantra'  " +
                    " when R.PCODE in('CASHDSWG') then 'Swiggy'  " +
                    " when R.PCODE in('CASHDUJW') then 'Ujjivan'  " +
                    " when R.PCODE in('CASHDHICB') then 'Wheels EMI'  " +
                    " when R.PCODE in('CASHDOLAO') then 'OLA OFT'  " +
                    " when R.PCODE in('CASHDIIFL') then 'IIFL'  " +
                    " when R.PCODE in('CASHWALLA') then 'Allana'  " +
                    " when R.PCODE in('CASHDMAHI') then 'Mahindra MRHFL'  " +
                    " when R.PCODE in('CASHWRIVG') then 'Rivigo'  " +
                    " when R.PCODE in('CASHDOLAN') then 'OLA ANI'  " +
                    " when R.PCODE in('CASHDMMFS') then 'Mahindra MMFSL'  " +
                    " when R.PCODE in('CASHDPYSO') then 'Payso'  " +
                    " when R.PCODE in('CASHDBPAY') then 'BPayLoyalty'  " +
                    " when R.PCODE in('CASHDDLIT') then 'D.Light'  " +
                    " when R.PCODE in('CASHDHERO1') then 'HeroFinCorp Customer'  " +
                    " when R.PCODE in('IGoldLead') then 'IGold Lead'  " +
                    " when R.PCODE in('CASHDJIO') then 'CASHDJIO'  " +
                    " when R.PCODE in('CASHDSATIN') then 'SATIN'  " +
                    " when R.PCODE in('CASHDASVD') then 'Ashirwaad'  " +
                    " when R.PCODE in('CASHDSVMFI') then 'Svatantra MFI'  " +
                    " when R.PCODE in('CASHDLOKS') then 'Lok Suvidha'  " +
                    " when R.PCODE in('CASHDSVMFI1') then 'Svatantra MFI1'  " +
                    " when R.PCODE in('CASHDEXDR') then 'Exide Life renewal'  " +
                    " when R.PCODE in('CASHDIPRUR') then 'ICICI Prudential Renewal'  " +
                    " end as TransactionType, Response_Msg,Payment_Status,RESPCODE,Count(RESPCODE) as [Count] into #Temp7 " +
                    " from #TEMP4 R  with (nolock)  " +
                    " left outer join  #TEMP6 TT with(nolock) " +
                    " on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                    " inner join #TEMP5 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                    " where R.MSGTYPE =0 " +
                    " group by  appid,case " +
                    " when R.PCODE in('CASHDAIR') then 'Airpay'  " +
                    " when R.PCODE in('CASHDAIRE') then 'Airpay-Eureka'  " +
                    " when R.PCODE in('CASHDAUF') then 'AU-Finance'  " +
                    " when R.PCODE in('CASHDBAJA') then 'Bajaj Auto'  " +
                    " when R.PCODE in('CASHDDISF') then 'Disha Fincare'  " +
                    " when R.PCODE in('CASHDHERO') then 'Hero FinCorp'  " +
                    " when R.PCODE in('CASHDIGOL') then 'Igold'  " +
                    " when R.PCODE in('CASHDIPRU') then 'ICICIPRULIFE'  " +
                    " when R.PCODE in('CASHDJFS') then 'JFS'  " +
                    " when R.PCODE in('CASHDLNT') then 'LnT'  " +
                    " when R.PCODE in('CASHDLPL') then 'Dr. LPL'  " +
                    " when R.PCODE in('CASHDOLAA') then 'OLA AUTO'  " +
                    " when R.PCODE in('CASHDOLAC') then 'OLA CAB'  " +
                    " when R.PCODE in('CASHDOLAS') then 'OLA Store'  " +
                    " when R.PCODE in('CASHDPAR') then 'Paras'  " +
                    " when R.PCODE in('CASHDPTM') then 'PayTM'  " +
                    " when R.PCODE in('CASHDRNR') then 'RoadRunnr'  " +
                    " when R.PCODE in('CASHDSFAX') then 'Shadow Fax'  " +
                    " when R.PCODE in('CASHDSHRF') then 'SHRIRAM CITY UNION FINANCE'  " +
                    " when R.PCODE in('CASHDSHRT') then 'Shriram Transport'  " +
                    " when R.PCODE in('CASHDSVA') then 'Svatantra'  " +
                    " when R.PCODE in('CASHDSWG') then 'Swiggy'  " +
                    " when R.PCODE in('CASHDUJW') then 'Ujjivan'  " +
                    " when R.PCODE in('CASHDHICB') then 'Wheels EMI'  " +
                    " when R.PCODE in('CASHDOLAO') then 'OLA OFT'  " +
                    " when R.PCODE in('CASHDIIFL') then 'IIFL'  " +
                    " when R.PCODE in('CASHWALLA') then 'Allana'  " +
                    " when R.PCODE in('CASHDMAHI') then 'Mahindra MRHFL'  " +
                    " when R.PCODE in('CASHWRIVG') then 'Rivigo'  " +
                    " when R.PCODE in('CASHDOLAN') then 'OLA ANI'  " +
                    " when R.PCODE in('CASHDMMFS') then 'Mahindra MMFSL'  " +
                    " when R.PCODE in('CASHDPYSO') then 'Payso'  " +
                    " when R.PCODE in('CASHDBPAY') then 'BPayLoyalty'  " +
                    " when R.PCODE in('CASHDDLIT') then 'D.Light'  " +
                    " when R.PCODE in('CASHDHERO1') then 'HeroFinCorp Customer'  " +
                    " when R.PCODE in('IGoldLead') then 'IGold Lead'  " +
                    " when R.PCODE in('CASHDJIO') then 'CASHDJIO'  " +
                    " when R.PCODE in('CASHDSATIN') then 'SATIN'  " +
                    " when R.PCODE in('CASHDASVD') then 'Ashirwaad'  " +
                    " when R.PCODE in('CASHDSVMFI') then 'Svatantra MFI'  " +
                    " when R.PCODE in('CASHDLOKS') then 'Lok Suvidha'  " +
                    " when R.PCODE in('CASHDSVMFI1') then 'Svatantra MFI1'  " +
                    " when R.PCODE in('CASHDEXDR') then 'Exide Life renewal'  " +
                    " when R.PCODE in('CASHDIPRUR') then 'ICICI Prudential Renewal'  " +
                    " end,Response_Msg,Payment_Status,RESPCODE " +
                    " Order By 1,2 " +

                    " Select AppID,TransactionType,Response_Msg,Payment_Status,[Count] From #Temp7 " +
                    " Where RESPCODE NOT IN (0) " +

                    " drop table #TEMP4 " +
                    " drop table #TEMP5 " +
                    " drop table #TEMP6 " +
                    " drop table #TEMP7 ";
                }
                return qry;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                qry = null;
            }
        }

        #endregion


        //tab 9
        #region Merchant Query
        private string GetQueryOfMerchantData(string UserID)
        {
            String qry;
            try
            {
                qry = "	select * into #TEMP	" +
                "	from dbo.TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                "	where PCODE in('IMPSBENVC','DMTIMPSBENV','DMTIMPSP2A','DMTNEFT','DMTBILLPAY1','DMTTOPUP1','IMPSFTP2A','P2MPURCH','TOPUP1','01','BILLPAY1','CASHDOLAC','CASHTXFR') 	" +
                "	and convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                "	and r.LOCAL_TIME between @STime and @ETime" +
                "	and R.MSGTYPE =0	And ISNUMERIC(CH_AMOUNT) = 1" +
                "	select * into #TEMP2	" +
                "	from dbo.TransactionsResponse R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
                "	where PCODE in('IMPSPosting','IMPSPostingPPI','DMTNEFT','DMTBILLPAY1','DMTTOPUP1','IMPSFTP2A','P2MPURCH','TOPUP1','01','BILLPAY1','CASHDOLAC','CASHTXFR') 	" +
                "	and convert(varchar(8),r.LOCAL_Date,112)= convert(varchar(8),GETDATE(),112)	" +
                "	and r.LOCAL_TIME between @STime and @ETime" +
                "	and R.MSGTYPE =0	" +
                "	select appid,R.TermID as UserID,case when R.PCODE ='DMTIMPSP2A' then 'IMPS Walk-In' 	" +
                "	else case when R.PCODE in('IMPSFTP2A') then 'IMPS PPI' 	" +
                "	else case when R.PCODE in('IMPSBENVC','DMTIMPSBENV') then 'Bene verification' 	" +
                "	else case when R.PCODE='DMTNEFT' then 'NEFT' 	" +
                "	else case when R.PCODE in('DMTBILLPAY1','BILLPAY1') then 'BillPay' 	" +
                "	else case when R.PCODE in('DMTTOPUP1','TOPUP1') then 'Recharge' 	" +
                "	else case when R.PCODE in('CASHDOLAC') then 'Cash Collection' 	" +
                "	else case when R.PCODE in('CASHTXFR') then 'Cash-In' 	" +
                "	else case when R.PCODE in('P2MPURCH') then 'PPI-Purchase'	" +
                "	else case when R.PCODE in('01') then 'Pay To Mobile'	" +
                "	end end end end end end end end end end as TransactionType, count(distinct r.trace) as Initiated	" +
                "	,SUM(case when RR.RESPCODE='0' then 1 else 0 end) as Success	" +
                "	,SUM(case when ((RR.RESPCODE in ('1','2','3','4','5','6','10','12','13','14','16','18','20','901','902','51','33','32','23','60','61','62','700322','999','1515','2388') and R.PCODE in ('DMTIMPSP2A','IMPSBENVC','DMTIMPSBENV','IMPSFTP2A')) or 	" +
                "	(R.PCODE in('DMTBILLPAY1','DMTTOPUP1','DMTNEFT','P2MPURCH','TOPUP1','BILLPAY1','CASHTXFR','01') and RR.RESPCODE='1'))   then 1 else 0 end) as Failed,	" +
                "	 SUM(case when ((RR.RESPCODE in ('11','19','21','9999','30','31','101','421','420','17','F14','F99','422','425') and R.PCODE in ('DMTIMPSP2A','IMPSBENVC','DMTIMPSBENV','IMPSFTP2A')) or 	" +
                "	 (R.PCODE in('DMTBILLPAY1','DMTTOPUP1','TOPUP1','BILLPAY1','CASHTXFR','01') and RR.RESPCODE in('2','')))   then 1 else 0 end) as Awaiting	" +
                "   ,sum(cast(CH_Amount as float)) as Amount " +
                "	from #TEMP R  with (nolock) 	" +
                "	left outer join #TEMP2 RR  with (nolock) on 	" +
                "	R.TRACE=RR.TRACE 	" +
                "	and R.MSGTYPE =0 and RR.MSGTYPE =0	" +
                    //"   where appid='FINOMER'   " +
                "	group by appid,R.TermID,case when R.PCODE ='DMTIMPSP2A' then 'IMPS Walk-In' 	" +
                "	else case when R.PCODE in('IMPSFTP2A') then 'IMPS PPI' 	" +
                "	else case when R.PCODE in('IMPSBENVC','DMTIMPSBENV') then 'Bene verification' 	" +
                "	else case when R.PCODE='DMTNEFT' then 'NEFT' 	" +
                "	else case when R.PCODE in('DMTBILLPAY1','BILLPAY1') then 'BillPay' 	" +
                "	else case when R.PCODE in('DMTTOPUP1','TOPUP1') then 'Recharge' 	" +
                "	else case when R.PCODE in('CASHDOLAC') then 'Cash Collection' 	" +
                "	else case when R.PCODE in('CASHTXFR') then 'Cash-In' 	" +
                "	else case when R.PCODE in('P2MPURCH') then 'PPI-Purchase' 	" +
                "	else case when R.PCODE in('01') then 'Pay To Mobile'	" +
                "	end end end end end end end end end end	" +
                "	order by appid,R.TermID	" +
                    //"   select appid,R.TermID as UserID, count(distinct r.trace) as Initiated ,SUM(case when RR.RESPCODE='0' then 1 else 0 end) as Success" +
                    //"   ,SUM(case when ((RR.RESPCODE in ('1','2','3','4','5','6','10','12','13','14','16','18','20','901','902','51','33','32','23','60','61','62','700322','999','1515','2388') and R.PCODE in ('DMTIMPSP2A','IMPSBENVC','DMTIMPSBENV','IMPSFTP2A')) or " +
                    //"   (R.PCODE in('DMTBILLPAY1','DMTTOPUP1','DMTNEFT','P2MPURCH','TOPUP1','BILLPAY1','CASHTXFR','01') and RR.RESPCODE='1'))   then 1 else 0 end) as Failed," +
                    //"   SUM(case when ((RR.RESPCODE in ('11','19','21','9999','30','31','101','421','420','17','F14','F99','422','425') and R.PCODE in ('DMTIMPSP2A','IMPSBENVC','DMTIMPSBENV','IMPSFTP2A')) or " +
                    //"   (R.PCODE in('DMTBILLPAY1','DMTTOPUP1','TOPUP1','BILLPAY1','CASHTXFR','01') and RR.RESPCODE in('2','')))   then 1 else 0 end) as Awaiting" +
                    //"   ,sum(cast(CH_Amount as float)) as Amount from #TEMP R  with (nolock) " +
                    //"   left outer join #TEMP2 RR  with (nolock) on R.TRACE=RR.TRACE and R.MSGTYPE =0 and RR.MSGTYPE =0" +
                    //"   group by appid,R.TermID having R.TermID = '"+UserID+"' order by appid,R.TermID" +
                    //"	drop table #TEMP	" +
                    //"	drop table #TEMP2";
                "	select case when R.PCODE ='DMTIMPSP2A' then 'IMPS Walk-In' 	" +
                "	else case when R.PCODE in('IMPSFTP2A') then 'IMPS PPI' 	" +
                "	else case when R.PCODE in('IMPSBENVC','DMTIMPSBENV') then 'Bene verification' 	" +
                "	else case when R.PCODE='DMTNEFT' then 'NEFT' 	" +
                "	else case when R.PCODE in('DMTBILLPAY1','BILLPAY1') then 'BillPay' 	" +
                "	else case when R.PCODE in('DMTTOPUP1','TOPUP1') then 'Recharge' 	" +
                "	else case when R.PCODE in('CASHDOLAC') then 'Cash Collection' 	" +
                "	else case when R.PCODE in('CASHTXFR') then 'Cash-In' 	" +
                "	else case when R.PCODE in('P2MPURCH') then 'PPI-Purchase'	" +
                "	else case when R.PCODE in('01') then 'Pay To Mobile'	" +
                "	end end end end end end end end end end as TransactionType, count(distinct r.trace) as Initiated	" +
                "	,SUM(case when RR.RESPCODE='0' then 1 else 0 end) as Success	" +
                "	,SUM(case when ((RR.RESPCODE in ('1','2','3','4','5','6','10','12','13','14','16','18','20','901','902','51','33','32','23','60','61','62','700322','999','1515','2388') and R.PCODE in ('DMTIMPSP2A','IMPSBENVC','DMTIMPSBENV','IMPSFTP2A')) or 	" +
                "	(R.PCODE in('DMTBILLPAY1','DMTTOPUP1','DMTNEFT','P2MPURCH','TOPUP1','BILLPAY1','CASHTXFR','01') and RR.RESPCODE='1'))   then 1 else 0 end) as Failed,	" +
                "	 SUM(case when ((RR.RESPCODE in ('11','19','21','9999','30','31','101','421','420','17','F14','F99','422','425') and R.PCODE in ('DMTIMPSP2A','IMPSBENVC','DMTIMPSBENV','IMPSFTP2A')) or 	" +
                "	 (R.PCODE in('DMTBILLPAY1','DMTTOPUP1','TOPUP1','BILLPAY1','CASHTXFR','01') and RR.RESPCODE in('2','')))   then 1 else 0 end) as Awaiting	" +
                "	 ,sum(cast(CH_Amount as float)) as Amount	" +
                "	 from #TEMP R  with (nolock) 	" +
                "	left outer join #TEMP2 RR  with (nolock) on 	" +
                "	R.TRACE=RR.TRACE 	" +
                "	and R.MSGTYPE =0 and RR.MSGTYPE =0	" +
                "	where appid='FINOMER'" +
                "	group by case when R.PCODE ='DMTIMPSP2A' then 'IMPS Walk-In' 	" +
                "	else case when R.PCODE in('IMPSFTP2A') then 'IMPS PPI' 	" +
                "	else case when R.PCODE in('IMPSBENVC','DMTIMPSBENV') then 'Bene verification' 	" +
                "	else case when R.PCODE='DMTNEFT' then 'NEFT' 	" +
                "	else case when R.PCODE in('DMTBILLPAY1','BILLPAY1') then 'BillPay' 	" +
                "	else case when R.PCODE in('DMTTOPUP1','TOPUP1') then 'Recharge' 	" +
                "	else case when R.PCODE in('CASHDOLAC') then 'Cash Collection' 	" +
                "	else case when R.PCODE in('CASHTXFR') then 'Cash-In' 	" +
                "	else case when R.PCODE in('P2MPURCH') then 'PPI-Purchase' 	" +
                "	else case when R.PCODE in('01') then 'Pay To Mobile'	" +
                "	end end end end end end end end end end	";

                return qry;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                qry = null;
            }
        }

        #endregion

        //tab 7 and 8
        #region Latency Query
        public string GetQueryOfLatencyData()
        {
            string str = string.Empty;

            try
            {
                str = "select Corelation_Request,Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
                    "	 Service_id,Method_ID,Layer_ID	" +
                    "	 into #TEMP	" +
                    "	 from dbo.Tbl_Corelation_Log (nolock)	" +
                    "	 where (RequestIn>=@DateTime or RequestOut>=@DateTime)	" +
                    "	 and (RequestIn<=getdate() or RequestOut<=getdate())	" +
                    "	 and Layer_ID IN (1,101)	" +
                    "	 group by Corelation_Request,Node_IP_Address,Service_id,Method_ID,Layer_ID	" +
                    "	 insert into #TEMP	" +
                    "	 select TT.Corelation_Request,TT.Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
                    "	 TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
                    "	 from  #TEMP T inner join dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) on T.Corelation_Request=TT.Corelation_Request	" +
                    "	 where TT.Layer_ID in(2,3)	" +
                    "	 group by TT.Corelation_Request,TT.Node_IP_Address,TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
                    "	 update T set T.RequestIn=TT.RequestIn	" +
                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
                    "	 on	 T.Corelation_Request=TT.Corelation_Request		" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=1 and TT.RequestIn is not null	" +
                    "	 update T set T.RequestOut=TT.RequestOut	" +
                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=1 and TT.RequestOut is not null	" +
                    "	 update T set T.RequestIn=TT.RequestIn	" +
                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
                    "	 on	 T.Corelation_Request=TT.Corelation_Request		" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=101 and TT.RequestIn is not null	" +
                    "	 update T set T.RequestOut=TT.RequestOut	" +
                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=101 and TT.RequestOut is not null	" +
                    "	 update T set T.RequestIn=TT.RequestIn	" +
                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=2 and TT.RequestIn is not null	" +
                    "		" +
                    "	 update T set T.RequestOut=TT.RequestOut	" +
                    "	 from #TEMP T 	" +
                    "	 inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=2 and TT.RequestOut is not null	" +
                    "		" +
                    "	 update T set T.RequestIn=TT.RequestIn	" +
                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=3 and TT.RequestIn is not null	" +
                    "		" +
                    "	 update T set T.RequestOut=TT.RequestOut	" +
                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=3 and TT.RequestOut is not null	" +
                    "		" +
                    "	 select DATEDIFF(ms,RequestIn,RequestOut)as [TIME],* into #TEMP2 from #TEMP order by 1 desc	" +
                    "	 select Node_IP_Address,Layer_Name,Request_Type,t.Method_ID,t.Service_ID,count(1) as [Count],convert(numeric(18,2),AVG([time])) as AVGS from #temp2 t 	" +
                    "	 left outer join temp_mstRequestType MR (nolock) on t.Method_ID=MR.Method_ID	" +
                    "	 and t.Service_ID=MR.Service_ID	" +
                    "	 left outer join temp_mstLayer ML (Nolock) on T.Layer_ID=ML.Layer_ID	" +
                    //"	 where time is not null and t.Method_ID In(88,16,1,2,38,104,95,81,96,18,20,87,999,111,8,1101, 1102, 1, 1204, 1205, 1207,1303,1304, 1502, 1504, 1602, 1701,1515, 1516 , 1156,1901,1902,1903,1904,1905,1906,1907,326,327,328,1522,1523,519,520,318,518,517,516,217,216,515,215,801,802,805,813,812,811,810,809,803,804,806,807,808,814,815) and t.Service_ID In (6,7,1,19,10,2,4,101,102,103,104,106,107,108,110,24,18,30) 	" +
                    // "    where time is not null and t.Method_ID In(88,16,1,2,38,104,95,81,96,18,20,87,999,111,8,1101, 1102, 1, 1204, 1205, 1207,1303,1304, 1502, 1504, 1602, 1701,1515, 1516 , 1156,1901,1902,1903,1904,1905,1906,1907,326,327,328,1522,1523)                                                                                                     and t.Service_ID In (6,7,1,19,10,2,4,101,102,103,104,106,107,108,110,24) 	" +
                    "    where time is not null and t.Method_ID In(88,16,1,2,38,104,95,81,96,18,20,87,999,111,8,1101, 1102, 1, 1204, 1205, 1207,1303,1304, 1502, 1504, 1602, 1701,1515, 1516 , 1156,1901,1902,1903,1904,1905,1906,1907,326,327,328,1522,1523,519,520,318,518,517,526,516,217,216,515,215,801,802,805,813,812,811,810,809,803,804,806,807,808,814,815) and t.Service_ID In (6,7,1,19,10,2,4,101,102,103,104,106,107,108,110,24,18,30,15,18,43) 		 " +
                    "	 group by Node_IP_Address,Layer_Name,Request_Type,t.Method_ID,t.Service_ID	" +
                    "	 order by 7 desc	" +
                    "	 	" +
                    "	 select Node_IP_Address,Layer_Name,count(1) as [Count],convert(numeric(18,2),AVG([time])) as AVGS from #temp2 t 	" +
                    "	 left outer join temp_mstRequestType MR (nolock) on t.Method_ID=MR.Method_ID	" +
                    "	 and t.Service_ID=MR.Service_ID	" +
                    "	 left outer join temp_mstLayer ML (Nolock) on T.Layer_ID=ML.Layer_ID	" +
                    //"	 where time is not null and t.Method_ID In(88,16,1,2,38,104,95,81,96,18,20,87,999,111,8,1101, 1102, 1, 1204, 1205, 1207,1303,1304, 1502, 1504, 1602, 1701,1515, 1516 , 1156,1901,1902,1903,1904,1905,1906,1907,326,327,328,1522,1523,519,520,318,518,517,516,217,216,515,215,801,802,805,813,812,811,810,809,803,804,806,807,808,814,815) and t.Service_ID In (6,7,1,19,10,2,4,101,102,103,104,106,107,108,110,24,18,30) 	" +
                    "    where time is not null and t.Method_ID In(88,16,1,2,38,104,95,81,96,18,20,87,999,111,8,1101, 1102, 1, 1204, 1205, 1207,1303,1304, 1502, 1504, 1602, 1701,1515, 1516 , 1156,1901,1902,1903,1904,1905,1906,1907,326,327,328,1522,1523,519,520,318,518,517,526,516,217,216,515,215,801,802,805,813,812,811,810,809,803,804,806,807,808,814,815) and t.Service_ID In (6,7,1,19,10,2,4,101,102,103,104,106,107,108,110,24,18,30,15,18,43) 		 " +
                    "	 group by Node_IP_Address,Layer_Name	" +
                    "	 order by 3 desc	" +
                    "	 drop table #temp	" +
                    "	 drop table #temp2";

                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion

        //tab 10
        #region Logs Query
        public string GetLogStringBuilder(string tablename, string RequestId)
        {
            string str = string.Empty;

            try
            {
                if (tablename == "Tbl_Corelation_Log")
                {
                    str = " select TL.*,MR.Request_Type " +
                           "from PBLogs.dbo.Tbl_Corelation_Log TL with(nolock) " +
                           "left outer join PBLogs.dbo.temp_mstRequestType MR on TL.Method_ID=MR.Method_ID " +
                           "and TL.Service_ID=MR.Service_ID  " +
                           "where TL.Corelation_Request = '" + RequestId + "' " +
                           "order by ID desc ";
                }
                else
                {
                    str = "select TL.* from PBLogs.dbo.tblLog TL with(nolock) " +
                            "where TL.RequestId = '" + RequestId + "' order by ID desc";
                }
                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion

        //tab 13
        #region IMPS Query
        public string GetIMPSStringBuilder()
        {
            string str = string.Empty;

            try
            {
                str = "SELECT Txn_Type,R.TXN_RRN,  Max(ID) ID into #LastReq1	" +
                    "    FROM [ESB_NPCI].[dbo].[M_IMPS_P2A_TRAN_LOG_REQUEST] R with (nolock)	" +
                    "	 Where r.Created_Datetime >= @DateTime 	" +
                    "	 GROUP BY Txn_Type,R.TXN_RRN	" +

                    "	 select * into #OrReq from #LastReq1 where  Txn_Type='OR'	" +
                    "	 select * into #VeReq from #LastReq1 where  Txn_Type='VE'	" +

                    "	 SELECT A.* into #OrRes FROM [ESB_NPCI].[dbo].[M_IMPS_P2A_TRAN_LOG_RESPONSE] A	" +
                    "	 INNER JOIN #LastReq1 B ON A.REQ_ID=B.Id AND B.Txn_Type='OR'	" +

                    "	 SELECT A.* into #VeRes FROM [ESB_NPCI].[dbo].[M_IMPS_P2A_TRAN_LOG_RESPONSE] A	" +
                    "	 INNER JOIN #LastReq1 B ON A.REQ_ID=B.Id AND B.Txn_Type='VE'	" +

                    "	 SELECT A.Txn_RRN,A.ID Org_Id ,C.ActRespCode Org_Resp,B.Id Ver_Id,D.ActRespCode Ver_Resp INTO #FinalResp	" +
                    "	 FROM #OrReq A 	" +
                    "	 LEFT OUTER JOIN #VeReq B ON A.TXN_RRN=B.TXN_RRN	" +
                    "	 LEFT OUTER JOIN #OrRes C ON A.Id=C.REq_Id	" +
                    "	 LEFT OUTER JOIN #VeRes D ON B.Id=D.REq_Id	" +

                    "	 Select a.*, a.TransRefNo,a.TXN_RRN ,a.BeneIFSCCode IFSC_Code, a.BeneAccountno ACCTNUM, 	" +
                    "	 a.RemMob,a.RemName,DelChannel,a.Tran_Amount , a.Created_Datetime DateTime,	" +
                    "	 CASE WHEN ISNULL(b.Org_Resp,'91')='91' AND b.Ver_Resp IS NULL THEN '-1'	" +
                    "	 WHEN ISNULL(b.Org_Resp,'91')='91' AND b.Ver_Resp IN ('08','91') THEN '-1' 	" +
                    "	 WHEN ISNULL(b.Org_Resp,'91')='91' AND b.Ver_Resp IN ('0', '00') THEN '0'	" +
                    "	 WHEN ISNULL(b.Org_Resp,'91')='91' AND b.Ver_Resp NOT IN ('0','00') THEN '1'	" +
                    "	 WHEN b.Org_Resp IN ('00','0') THEN '0' 	" +
                    "	 WHEN b.Org_Resp NOT IN ('91','00','0') THEN '1'	" +
                    "	 ELSE 'XX' END as 'Status' ,	" +
                    "	 CASE WHEN ISNULL(b.Org_Resp,'91')='91' AND b.Ver_Resp IS NULL THEN 'No Response'	" +
                    "	 WHEN ISNULL(b.Org_Resp,'91')='91' AND b.Ver_Resp IN ('08','91') THEN 'Unknown Ver Status (08/91)'	" +
                    "	 WHEN ISNULL(b.Org_Resp,'91')='91' AND b.Ver_Resp IN ('0', '00') THEN 'Success'	" +
                    "	 WHEN ISNULL(b.Org_Resp,'91')='91' AND b.Ver_Resp NOT IN ('0','00') THEN 'Fail'	" +
                    "	 WHEN b.Org_Resp IN ('00','0') THEN 'Success'	" +
                    "	 WHEN b.Org_Resp NOT IN ('91','00','0') THEN 'Fail'	" +
                    "	 ELSE 'Unknown' END as 'Description'	" +
                    "	 from [ESB_NPCI].[dbo].[M_IMPS_P2A_TRAN_LOG_REQUEST] a with (nolock)	" +
                    "	 INNER JOIN #FinalResp b on a.TXN_RRN = b.TXN_RRN AND a.Txn_Type='OR'	" +

                    "	 Drop Table #LastReq1	" +
                    "	 Drop Table #OrReq	" +
                    "	 Drop Table #VeReq	" +
                    "	 Drop Table #OrRes	" +
                    "	 Drop Table #VeRes	" +
                    "	 Drop Table #FinalResp	";


                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion


        #region VS Query
        public string GetVSStringBuilder()
        {
            string str = string.Empty;

            try
            {
                //    str = " Select distinct cc.Node_IP_Address,ll.Layer_Name "+
                //          " From PBCONFIGURATION.dbo.tblESBUrl esb "+
                //          " Left Outer Join PBLogs.dbo.Tbl_Corelation_Log cc On esb.ServiceUrlId = Service_ID "+
                //          " left Outer Join PBMaster.dbo.mstLayer ll On ll.Layer_ID = cc.Layer_ID "+
                //          " Where ll.Layer_Name In ('UI','BLL') " +
                //          " order By 1";

                //return str;

                str = "select Corelation_Request,Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
                    "	 Service_id,Method_ID,Layer_ID	" +
                    "	 into #TEMP	" +
                    "	 from PBLogs.dbo.Tbl_Corelation_Log (nolock)	" +
                    "	 where (RequestIn>=@DateTime or RequestOut>=@DateTime)	" +
                    "	 and (RequestIn<=getdate() or RequestOut<=getdate())	" +
                    "	 and Layer_ID=1	" +
                    "	 group by Corelation_Request,Node_IP_Address,Service_id,Method_ID,Layer_ID	" +
                    "	 insert into #TEMP	" +
                    "	 select TT.Corelation_Request,TT.Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
                    "	 TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
                    "	 from  #TEMP T inner join PBLogs.dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) on T.Corelation_Request=TT.Corelation_Request	" +
                    "	 where TT.Layer_ID in(2,3)	" +
                    "	 group by TT.Corelation_Request,TT.Node_IP_Address,TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
                    "	 update T set T.RequestIn=TT.RequestIn	" +
                    "	 from #TEMP T inner join PBLogs.dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
                    "	 on	 T.Corelation_Request=TT.Corelation_Request		" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=1 and TT.RequestIn is not null	" +
                    "	 update T set T.RequestOut=TT.RequestOut	" +
                    "	 from #TEMP T inner join PBLogs.dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                    "	 where T.Layer_ID=1 and TT.RequestOut is not null	" +
                    "	 update T set T.RequestIn=TT.RequestIn	" +
"	 from #TEMP T inner join PBLogs.dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
"	 on T.Corelation_Request=TT.Corelation_Request	" +
"	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
"	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
"	 where T.Layer_ID=2 and TT.RequestIn is not null	" +
"		" +
"	 update T set T.RequestOut=TT.RequestOut	" +
"	 from #TEMP T 	" +
"	 inner join PBLogs.dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
"	 on T.Corelation_Request=TT.Corelation_Request	" +
"	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
"	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
"	 where T.Layer_ID=2 and TT.RequestOut is not null	" +
"		" +
"	 update T set T.RequestIn=TT.RequestIn	" +
"	 from #TEMP T inner join PBLogs.dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
"	 on T.Corelation_Request=TT.Corelation_Request	" +
"	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
"	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
"	 where T.Layer_ID=3 and TT.RequestIn is not null	" +
"		" +
"	 update T set T.RequestOut=TT.RequestOut	" +
"	 from #TEMP T inner join PBLogs.dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
"	 on T.Corelation_Request=TT.Corelation_Request	" +
"	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
"	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
"	 where T.Layer_ID=3 and TT.RequestOut is not null	" +
"		" +
"	 select DATEDIFF(ms,RequestIn,RequestOut)as [TIME],* into #TEMP2 from #TEMP order by 1 desc	" +

"	 select distinct Node_IP_Address,Layer_Name,count(1) as [Count] from #temp2 t	" +
"	 left outer join temp_mstRequestType MR (nolock) on t.Method_ID=MR.Method_ID and t.Service_ID=MR.Service_ID	" +
"	 left outer join temp_mstLayer ML (Nolock) on T.Layer_ID=ML.Layer_ID	" +
"	 where Layer_Name in ('UI','BLL','ESB')	" +
"	 group by Node_IP_Address,Layer_Name Order By 1 	" +
"	 drop table #temp	" +
"	 drop table #temp2";

                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion

        //tab 6
        #region ESB Failed And Awaiting Query
        private string GetQueryOfESBFailedAwaiting(int ESBTab, string TransactionName, string TType)
        {
            String qry;
            try
            {
                if (ESBTab == 1 && TransactionName == "IMPS" && TType == "Failed")
                {
                    qry = " DECLARE @Localdate DATETIME = convert(VARCHAR(8), GETDATE(), 112) " +
                          " SELECT Trace,X_Correlation_ID,MSGTYPE,PCODE INTO #TEMP	" +
                          "	FROM dbo.TransactionsRequest R WITH (NOLOCK INDEX = IX_LOCAL_DATE) " +
                          "	WHERE r.LOCAL_Date = @Localdate AND r.LOCAL_TIME >= @DateTime AND R.MSGTYPE = 0	" +

                          "	SELECT RESPCODE,Trace,X_Correlation_ID,MSGTYPE,PCODE,TransResponse_Id,Response_Msg,BeneIFSC INTO #TEMP2 " +
                          "	FROM dbo.TransactionsResponse R WITH (NOLOCK INDEX = IX_LOCAL_DATE)	" +
                          "	WHERE r.LOCAL_Date = @Localdate AND r.LOCAL_TIME >= @DateTime AND R.MSGTYPE = 0 " +

                    "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                    "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

                    "   select Isnull(MI.BankName,'Manual Posting') as Bankname,RESPCODE,Response_Msg,count(distinct R.Trace) as [Count] " +
                    "   from #TEMP R  with (nolock)  " +
                    "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                    "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                    "   left outer join mstIFSCMapping MI With(nolock) on left(BeneIFSC,4)=left(MI.IFSC,4)" +
                    "   where R.MSGTYPE =0  and RR.RESPCODE not in ('91','8','0','00') and R.PCODE in ('DMTIMPSP2A')  " +
                    "   group by MI.BankName,RESPCODE,Response_Msg " +
                    "	drop table #TEMP	" +
                    "	drop table #TEMP2	" +
                    "	drop table #TEMP3";
                    return qry;
                }
                else if (ESBTab == 1 && TransactionName == "IMPS" && TType == "Awaiting")
                {
                    qry = " DECLARE @Localdate DATETIME = convert(VARCHAR(8), GETDATE(), 112) " +
                          " SELECT Trace,X_Correlation_ID,MSGTYPE,PCODE INTO #TEMP	" +
                          "	FROM dbo.TransactionsRequest R WITH (NOLOCK INDEX = IX_LOCAL_DATE) " +
                          "	WHERE r.LOCAL_Date = @Localdate AND r.LOCAL_TIME >= @DateTime AND R.MSGTYPE = 0	" +

                          "	SELECT RESPCODE,Trace,X_Correlation_ID,MSGTYPE,PCODE,TransResponse_Id,Response_Msg,BeneIFSC INTO #TEMP2 " +
                          "	FROM dbo.TransactionsResponse R WITH (NOLOCK INDEX = IX_LOCAL_DATE)	" +
                          "	WHERE r.LOCAL_Date = @Localdate AND r.LOCAL_TIME >= @DateTime AND R.MSGTYPE = 0 " +

                   "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                   "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

                   "   select Isnull(MI.BankName,'Manual Posting') as Bankname,RESPCODE,Response_Msg,count(distinct R.Trace) as [Count] " +
                   "   from  #TEMP R  with (nolock) " +
                   "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                   "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                   "   left outer join mstIFSCMapping MI With(nolock) on left(BeneIFSC,4)=left(MI.IFSC,4) " +
                   "   where R.MSGTYPE =0  and RR.RESPCODE in ('91','8') and R.PCODE in ('DMTIMPSP2A') " +
                   "   group by MI.BankName,RESPCODE,Response_Msg " +
                   "	drop table #TEMP	" +
                   "	drop table #TEMP2	" +
                   "	drop table #TEMP3";
                    return qry;
                }
                else if (ESBTab == 2 && TransactionName == "Bene verification" && TType == "Failed")
                {
                    qry = " DECLARE @Localdate DATETIME = convert(VARCHAR(8), GETDATE(), 112) " +
                          " SELECT Trace,X_Correlation_ID,MSGTYPE,PCODE INTO #TEMP	" +
                          "	FROM dbo.TransactionsRequest R WITH (NOLOCK INDEX = IX_LOCAL_DATE) " +
                          "	WHERE r.LOCAL_Date = @Localdate AND r.LOCAL_TIME >= @DateTime AND R.MSGTYPE = 0	" +

                          "	SELECT RESPCODE,Trace,X_Correlation_ID,MSGTYPE,PCODE,TransResponse_Id,Response_Msg,BeneIFSC INTO #TEMP2 " +
                          "	FROM dbo.TransactionsResponse R WITH (NOLOCK INDEX = IX_LOCAL_DATE)	" +
                          "	WHERE r.LOCAL_Date = @Localdate AND r.LOCAL_TIME >= @DateTime AND R.MSGTYPE = 0 " +

                     "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                    "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

                    "   select Isnull(MI.BankName,'Manual Posting') as Bankname,RESPCODE,Response_Msg,count(distinct R.Trace) as [Count] " +
                    "   from  #TEMP R  with (nolock) " +
                    "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                    "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                    "   left outer join mstIFSCMapping MI With(nolock) on left(BeneIFSC,4)=left(MI.IFSC,4) " +
                    "   where R.MSGTYPE =0  and RR.RESPCODE Not in ('91','8','0','00') and R.PCODE in ('IMPSBENVC','DMTIMPSBENV','IMPSBENV') " +
                    "   group by MI.BankName,RESPCODE,Response_Msg " +
                    "	drop table #TEMP	" +
                    "	drop table #TEMP2	" +
                    "	drop table #TEMP3";
                    return qry;
                }
                else
                {
                    qry = " DECLARE @Localdate DATETIME = convert(VARCHAR(8), GETDATE(), 112) " +
                          " SELECT Trace,X_Correlation_ID,MSGTYPE,PCODE INTO #TEMP	" +
                          "	FROM dbo.TransactionsRequest R WITH (NOLOCK INDEX = IX_LOCAL_DATE) " +
                          "	WHERE r.LOCAL_Date = @Localdate AND r.LOCAL_TIME >= @DateTime AND R.MSGTYPE = 0	" +

                          "	SELECT RESPCODE,Trace,X_Correlation_ID,MSGTYPE,PCODE,TransResponse_Id,Response_Msg,BeneIFSC INTO #TEMP2 " +
                          "	FROM dbo.TransactionsResponse R WITH (NOLOCK INDEX = IX_LOCAL_DATE)	" +
                          "	WHERE r.LOCAL_Date = @Localdate AND r.LOCAL_TIME >= @DateTime AND R.MSGTYPE = 0 " +

                          "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
                          "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

                          "   select Isnull(MI.BankName,'Manual Posting') as Bankname,RESPCODE,Response_Msg,count(distinct R.Trace) as [Count] " +
                          "   from  #TEMP R  with (nolock) " +
                          "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
                          "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
                          "   left outer join mstIFSCMapping MI With(nolock) on left(BeneIFSC,4)=left(MI.IFSC,4) " +
                          "   where R.MSGTYPE =0  and RR.RESPCODE in ('91','8') and R.PCODE in ('IMPSBENVC','DMTIMPSBENV') " +
                          "   group by MI.BankName,RESPCODE,Response_Msg " +
                          "	drop table #TEMP	" +
                          "	drop table #TEMP2	" +
                          "	drop table #TEMP3";
                    return qry;
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                qry = null;
            }
        }
        #endregion

        //tab AEPSFailed
        #region AEPS Failed Query
        private string GetQueryOfAEPSFailed()
        {
            String qry;
            try
            {
                qry = " DECLARE @Localdate DATETIME = convert(VARCHAR(8), GETDATE(), 112) " +
                    "	select Trace, X_CORRELATION_ID, MSGTYPE, PCODE, PAN into #TEMP	" +
               "	from dbo.TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
               "	where r.LOCAL_Date = @Localdate	" +
               "	and r.LOCAL_TIME >= @DateTime 	" +
               "	and R.MSGTYPE =0	" +

               "	select Trace, X_Correlation_ID, TransResponse_Id, MSGTYPE, Response_Msg, RESPCODE into #TEMP2	" +
               "	from dbo.TransactionsResponse R  with (nolock INDEX = IX_LOCAL_DATE) 	" +
               "	where r.LOCAL_Date = @Localdate	" +
               "	and r.LOCAL_TIME >= @DateTime 	" +
               "	and R.MSGTYPE =0	" +

               "   Select Trace,X_Correlation_ID,max(TransResponse_Id) as TransResponse_Id into #temp3 " +
               "   From #Temp2 where MSGTYPE =0 Group By Trace,X_Correlation_ID" +

               "   select Isnull(MI.BankName,'Manual Posting') as Bankname,RESPCODE,Response_Msg,count(distinct R.Trace) as [Count] " +
               "   from  #TEMP R  with (nolock) " +
               "   left outer join  #TEMP3 TT with(nolock) on  R.X_CORRELATION_ID = TT.X_CORRELATION_ID and R.TRACE=TT.TRACE " +
               "   inner join #TEMP2 RR  with (nolock) on TT.TransResponse_Id=RR.TransResponse_Id " +
               "   left outer join mstIFSCMapping MI With(nolock) on left(PAN,6)=MI.NBIN " +
               "   where R.MSGTYPE =0  and RR.RESPCODE Not IN ('0','00') and R.PCODE in ('CASHWAEPSACQ','AEPSPosting')  And MI.BankName is Not null" +
               "   group by MI.BankName,RESPCODE,Response_Msg " +
               "	drop table #TEMP	" +
               "	drop table #TEMP2	" +
               "	drop table #TEMP3";
                return qry;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                qry = null;
            }
        }
        #endregion

        //tab 18
        #region ESB_Latency Report Query
        private string GetESB_Latency_DBInsertionQuery()
        {
            String qry;
            try
            {
                qry = "Select [URL], " +
                       "SUM(Cast(ResponseCode_200 as Int)) as [ResponseCode_200], " +
                       "SUM(Cast(ResponseCode_400 as Int)) as [ResponseCode_400], " +
                       "SUM(Cast(ResponseCode_401 as Int)) as [ResponseCode_401], " +
                       "SUM(Cast(ResponseCode_500 as Int)) as [ResponseCode_404], " +
                       "SUM(Cast(ResponseCode_503 as Int)) as [ResponseCode_405], " +
                       "SUM(Cast(ResponseCode_504 as Int)) as [ResponseCode_500], " +
                       "SUM(Cast(ResponseCode_502 as Int)) as [ResponseCode_502], " +
                       "SUM(Cast(ResponseCode_404 as Int)) as [ResponseCode_503], " +
                       "SUM(Cast(ResponseCode_405 as Int)) as [ResponseCode_504], " +
                       "SUM(Cast(Hits_on_srvprd1esbapp1 as Int)) as [Server1], " +
                       "SUM(Cast(Hits_on_srvprd1esbapp2 as Int)) as [Server2], " +
                       "SUM(Cast(Hits_on_srvprd1esbapp3 as Int)) as [Server3], " +
                       "SUM(Cast(Hits_on_srvprd1esbapp4 as Int)) as [Server4], " +
                       "SUM(Cast(Hits_on_srvprd1esbapp5 as Int)) as [Server5], " +
                       "SUM(Cast([TotalHits] as Int)) as [TotalHits], " +
                       "Avg(Cast(Avg_Time_on_srvprd1esbapp1 as Int)) as [Average1], " +
                       "Avg(Cast(Avg_Time_on_srvprd1esbapp2 as Int)) as [Average2], " +
                       "Avg(Cast(Avg_Time_on_srvprd1esbapp3 as Int)) as [Average3], " +
                       "Avg(Cast(Avg_Time_on_srvprd1esbapp4 as Int)) as [Average4], " +
                       "Avg(Cast(Avg_Time_on_srvprd1esbapp5 as Int)) as [Average5] " +
                       "FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] " +
                       "Where Local_DateTime >= @DateTime And TotalHits <>0 " +
                       "Group By URL  " +
                       "Order by [TotalHits] desc";

                return qry;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                qry = null;
            }
        }

        #endregion

        //tab 15
        #region ESB Response Time Query
        private string GetESB_Latency_ReportQuery()
        {
            String qry;
            try
            {
                qry = "select  Corelation_Request,Layer_id,Service_id, Method_id,Corelation_Session, RequestIn " +
                       "into #1 from Tbl_Correlation_Log with(nolock) " +
                       "where RequestIn >= @from AND RequestIn <=@to  AND   request_flag='1' " +

                       "select  Corelation_Request,Layer_id,Service_id, Method_id,Corelation_Session, RequestIn " +
                       "into #2 from Tbl_Correlation_Log with(nolock) " +
                       "where RequestIn >= @from AND RequestIn <=@to  AND response_flag='1' " +

                       "Select a.Corelation_Request,a.Layer_id,a.Service_id,a.Method_id, a.Corelation_Session ,a.RequestIn RequestTime,b.RequestIn ResponseTime,datediff(ms,a.RequestIn,b.RequestIn) Timediff " +
                       "into #time from #1 a inner join #2 b on a.Corelation_Request = b.Corelation_Request and a.Layer_id = b.Layer_id and a.Service_id = b.Service_id  and a.Method_id = b.Method_id and a.Corelation_Session = b.Corelation_Session " +

                       "Select  c.Service_name,b.Layer_Name,d.Request_Type,a.* , cast(Timediff/1000.0 as numeric(18,2)) inSec " +
                       "into #data from #Time a INNER JOIN mstLayer b on b.Layer_id=a.Layer_id " +
                       "INNER JOIN  mstServiceName c on c.Service_id = a.Service_id " +
                       "INNER JOIN mstRequestType d on D.Service_ID = a.Service_ID and D.Method_ID = a.Method_ID " +

                       "Select Service_name, Request_Type, 0 as 'Response ESB 0-1(Sec)',0 'Response ESB 1-3(Sec)',0 	'Response ESB >3(Sec)' " +
                       "into #Summary from #data group by Service_name, Request_Type " +

                       "update a set [Response ESB 0-1(Sec)]=cnt FROM #summary a " +
                       "inner join (Select Service_name, Request_Type, Count(1) as cnt From #data Where inSec<1 group by Service_name, Request_Type) b " +
                       "on a.Service_name=b.Service_name AND a.Request_Type = b.Request_Type " +

                       "update a set [Response ESB 1-3(Sec)]=cnt FROM #summary a " +
                       "inner join (Select Service_name, Request_Type, Count(1) as cnt From #data Where  inSec>1 and inSec<3 group by Service_name, Request_Type) b " +
                       "on a.Service_name=b.Service_name AND a.Request_Type = b.Request_Type " +

                       "update a set [Response ESB >3(Sec)]=cnt FROM #summary a " +
                       "inner join (Select Service_name, Request_Type, Count(1) as cnt From #data Where  inSec>=3 group by Service_name, Request_Type) b " +
                       "on a.Service_name=b.Service_name AND a.Request_Type = b.Request_Type " +

                       "Select Service_name,Request_Type,[Response ESB 0-1(Sec)] as [Response_ESB_0_1Sec],[Response ESB 1-3(Sec)] as [Response_ESB_1_3Sec],[Response ESB >3(Sec)] as [Response_ESB_3_Sec] from #Summary order by Service_name " +

                       "drop table #1 " +
                       "drop table #2 " +
                       "drop table #time " +
                       "drop table #data " +
                       "drop table #Summary";

                return qry;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                qry = null;
            }
        }

        #endregion

        //tab 16 and 17
        #region ESB NODE and SERVICE Latency Query
        public string GetQueryOfESBLatencyData()
        {
            string str = string.Empty;

            try
            {
                str = "Select URL,SUM(Cast([TotalHits] as Int)) as [TotalHits],	" +
                      "sum(cast(Hits_on_srvprd1esbapp1 as Int)) as Total_on_srvprd1esbapp1,avg(Cast(Avg_Time_on_srvprd1esbapp1 as Int)) as [Average1],  " +
                      "sum(cast(Hits_on_srvprd1esbapp2 as Int)) as Total_on_srvprd1esbapp2,avg(Cast(Avg_Time_on_srvprd1esbapp2 as Int)) as [Average2],  " +
                      "sum(cast(Hits_on_srvprd1esbapp3 as Int)) as Total_on_srvprd1esbapp3,avg(Cast(Avg_Time_on_srvprd1esbapp3 as Int)) as [Average3],  " +
                      "sum(cast(Hits_on_srvprd1esbapp4 as Int)) as Total_on_srvprd1esbapp4,avg(Cast(Avg_Time_on_srvprd1esbapp4 as Int)) as [Average4],  " +
                      "sum(cast(Hits_on_srvprd1esbapp5 as Int)) as Total_on_srvprd1esbapp5,avg(Cast(Avg_Time_on_srvprd1esbapp5 as Int)) as [Average5],  " +
                      "max(Local_DateTime) as localdate FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion]  " +
                      "Where Local_DateTime >=@DateTime  " +
                      "Group By URL having SUM(Cast([TotalHits] as Int))>1" +

                      "Select sum(cast(Hits_on_srvprd1esbapp1 as Int)) as Total_on_srvprd1esbapp,avg(Cast(Avg_Time_on_srvprd1esbapp1 as Int)) as [Average] FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime  " +
                      "UNION ALL  " +
                      "Select sum(cast(Hits_on_srvprd1esbapp2 as Int)) as Total_on_srvprd1esbapp,avg(Cast(Avg_Time_on_srvprd1esbapp2 as Int)) as [Average] FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime  " +
                      "UNION ALL  " +
                      "Select sum(cast(Hits_on_srvprd1esbapp3 as Int)) as Total_on_srvprd1esbapp,avg(Cast(Avg_Time_on_srvprd1esbapp3 as Int)) as [Average] FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime  " +
                      "UNION ALL  " +
                      "Select sum(cast(Hits_on_srvprd1esbapp4 as Int)) as Total_on_srvprd1esbapp,avg(Cast(Avg_Time_on_srvprd1esbapp4 as Int)) as [Average] FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime  " +
                      "UNION ALL  " +
                      "Select sum(cast(Hits_on_srvprd1esbapp5 as Int)) as Total_on_srvprd1esbapp,avg(Cast(Avg_Time_on_srvprd1esbapp5 as Int)) as [Average] FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime  ";



                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion

        //tab 14
        #region HA-ESB Latency Query
        public string GetQueryOfHA_ESBLatency()
        {
            string str = string.Empty;

            try
            {
                str = "Select a.* into #data FROM ( " +
                      "	select URL, Hits_on_srvprd1esbapp1 SERVER_Total_Hits,'Server1' ServerName, Avg_Time_on_srvprd1esbapp1 SERVER1_AVG_Time,Last_Hits_Time,srvprd1esbapp1_200,	" +
                      "	srvprd1esbapp1_400,srvprd1esbapp1_401,srvprd1esbapp1_404,srvprd1esbapp1_405,srvprd1esbapp1_500,srvprd1esbapp1_502,srvprd1esbapp1_503,srvprd1esbapp1_504 	" +
                      "	FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime  and TotalHits>0 	" +
                      "	Union 	" +
                      "	select URL, Hits_on_srvprd1esbapp2 SERVER_Total_Hits,'Server2' ServerName,Avg_Time_on_srvprd1esbapp2 SERVER2_AVG_Time,Last_Hits_Time,srvprd1esbapp2_200,	" +
                      "	srvprd1esbapp2_400,srvprd1esbapp2_401,srvprd1esbapp2_404,srvprd1esbapp2_405,srvprd1esbapp2_500,srvprd1esbapp2_502,srvprd1esbapp2_503,srvprd1esbapp2_504 	" +
                      "	FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime  and TotalHits>0 	" +
                      "	Union 	" +
                      "	select URL,Hits_on_srvprd1esbapp3 SERVER_Total_Hits ,'Server3' ServerName,Avg_Time_on_srvprd1esbapp3 SERVER3_AVG_Time,Last_Hits_Time,srvprd1esbapp3_200,    	" +
                      "	srvprd1esbapp3_400,srvprd1esbapp3_401,srvprd1esbapp3_404,srvprd1esbapp3_405,srvprd1esbapp3_500,srvprd1esbapp3_502,srvprd1esbapp3_503,srvprd1esbapp3_504 	" +
                      "	FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime  and TotalHits>0 	" +
                      "	Union 	" +
                      "	select URL,Hits_on_srvprd1esbapp4 SERVER_Total_Hits,'Server4' ServerName,Avg_Time_on_srvprd1esbapp4 SERVER4_AVG_Time,Last_Hits_Time,srvprd1esbapp4_200,	" +
                      "	srvprd1esbapp4_400,srvprd1esbapp4_401,srvprd1esbapp4_404,srvprd1esbapp4_405,srvprd1esbapp4_500,srvprd1esbapp4_502,srvprd1esbapp4_503,srvprd1esbapp4_504 	" +
                      "	FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime and TotalHits>0 	" +
                      "	Union 	" +
                      "	select URL, Hits_on_srvprd1esbapp5 SERVER_Total_Hits,'Server5' ServerName,Avg_Time_on_srvprd1esbapp5 SERVER5_AVG_Time,Last_Hits_Time,srvprd1esbapp5_200,	" +
                      "	srvprd1esbapp5_400,srvprd1esbapp5_401,srvprd1esbapp5_404,srvprd1esbapp5_405,srvprd1esbapp5_500,srvprd1esbapp5_502,srvprd1esbapp5_503,srvprd1esbapp5_504 	" +
                      "	FROM [ESB_Transactions].[dbo].[ESB_Latency_DBInsertion] Where Local_DateTime >= @DateTime and TotalHits>0) a	" +

                      " Select URL,ServerName, SUM(cast(SERVER_Total_Hits as int)) SERVER_Total_Hits, " +
                      " Avg(cast(SERVER1_AVG_Time as int)) SERVER1_AVG_Time, " +
                      " Max(Last_Hits_Time) Last_Hits_Time, " +
                      " SUM(cast(srvprd1esbapp1_200 as int)) Response_200_Success," +
                      " SUM(cast(srvprd1esbapp1_400 as int)) Response_400," +
                      " SUM(cast(srvprd1esbapp1_401 as int)) Response_401," +
                      " SUM(cast(srvprd1esbapp1_404 as int)) Response_404," +
                      " SUM(cast(srvprd1esbapp1_405 as int)) Response_405," +
                      " SUM(cast(srvprd1esbapp1_500 as int)) Response_500," +
                      " SUM(cast(srvprd1esbapp1_502 as int)) Response_502," +
                      " SUM(cast(srvprd1esbapp1_503 as int)) Response_503," +
                      " SUM(cast(srvprd1esbapp1_504 as int)) Response_504" +
                      " From #Data" +
                      " GROUP BY URL,ServerName" +
                      " order by URL,ServerName" +
                      " drop table #data";

                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion

        #region DMS Query
        public string GetDMSQuery(int i)
        {
            string str = string.Empty;

            try
            {
                if (i == 1)
                {
                    str = " SELECT count(1) [ESB],CONVERT(date, INSERTDATE) [DATE],DATEPART(HOUR, INSERTDATE) [HOUR], " +
                          " Case When (DATEPART(MINUTE, INSERTDATE) / 15) = 0 Then 15 When (DATEPART(MINUTE, INSERTDATE) / 15) = 1 Then 30 When (DATEPART(MINUTE, INSERTDATE) / 15) = 2 Then 45 When (DATEPART(MINUTE, INSERTDATE) / 15) = 3 Then 60  End as Minute " +
                          "	FROM ACCOUNTOPENINGDETAILS " +
                          "	WHERE INSERTDATE >= @DateTime AND RESPONSEBITMAP = '0000000000000' AND [PGNCIFNum]!='' " +
                          "	group by CONVERT(date, INSERTDATE),DATEPART(HOUR, INSERTDATE), " +
                          " Case When (DATEPART(MINUTE, INSERTDATE) / 15) = 0 Then 15 When (DATEPART(MINUTE, INSERTDATE) / 15) = 1 Then 30 When (DATEPART(MINUTE, INSERTDATE) / 15) = 2 Then 45 When (DATEPART(MINUTE, INSERTDATE) / 15) = 3 Then 60  End " +
                          "	order by 3 ";

                    return str;
                }
                else
                {
                    str = " SELECT count(1) [DMS],CONVERT(date, A.CASEINITTIME) [DATE],DATEPART(HOUR, A.CASEINITTIME) [HOUR], " +
                          " Case When (DATEPART(MINUTE, A.CASEINITTIME) / 15) = 0 Then 15 When (DATEPART(MINUTE, A.CASEINITTIME) / 15) = 1 Then 30 When (DATEPART(MINUTE, A.CASEINITTIME) / 15) = 2 Then 45 When (DATEPART(MINUTE, A.CASEINITTIME) / 15) = 3 Then 60  End as Minute " +
                          "	FROM (SELECT X_CORRELATION_ID,CASEINITTIME,PINSTID,ACCOUNTNUMBER,CURRENT_WS,CUSTOMERNUMBER FROM FINO_ACCOUNT_VIEW UNION ALL " +
                          "	SELECT X_CORRELATION_ID,CASEINITTIME,PINSTID,ACCOUNTNUMBER,CURRENT_WS,CUSTOMERNUMBER FROM FINOLENDING_EXT UNION ALL " +
                          "	SELECT X_CORRELATION_ID,CASEINITTIME,PINSTID,ACCOUNTNUMBER,CURRENT_WS,CUSTOMERNUMBER FROM FINOLENDING_EXTHISTORY) AS A " +
                          "	WHERE A.CASEINITTIME >= @DateTime " +
                          " group by CONVERT(date, A.CASEINITTIME),DATEPART(HOUR, A.CASEINITTIME), " +
                          " Case When (DATEPART(MINUTE, A.CASEINITTIME) / 15) = 0 Then 15 When (DATEPART(MINUTE, A.CASEINITTIME) / 15) = 1 Then 30 When (DATEPART(MINUTE, A.CASEINITTIME) / 15) = 2 Then 45 When (DATEPART(MINUTE, A.CASEINITTIME) / 15) = 3 Then 60 end " +
                          " order by 3";

                    return str;
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {

            }
        }
        #endregion

        //tab 11 and 12
        #region RequestType Query
        public string GetQueryofRequestType(string reqtype)
        {
            string str = string.Empty;
            try
            {
                if (reqtype == "Nothing")
                {
                    str = " select * from PBMaster.dbo.mstRequestType Where Service_ID < 100 Order By Request_Type ";
                }
                else if (reqtype == "BPay")
                {
                    str = " select * from PBMaster.dbo.mstRequestType Where Service_ID > 100 Order By Request_Type ";
                }
                else
                {
                    str = " select Method_ID,Service_ID from PBMaster.dbo.mstRequestType where Request_Type='" + reqtype + "' ";
                }
                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {

            }
        }
        #endregion

        //tab 11 and 12
        #region ESB Zone Latency POC Query
        public string GetQueryofESBLatencyData(string LatencyType, string ServiceID, string MethodID)
        {
            string str = string.Empty;

            try
            {
                if (LatencyType == "NoLayer")
                {
                    str =
                        "    Use PBLogs " +
                        "	 select Corelation_Request,Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
                        "	 Service_id,Method_ID,Layer_ID	" +
                        "	 into #TEMP	" +
                        "	 from dbo.Tbl_Corelation_Log (nolock)	" +
                        "	 where (RequestIn>=@DateTime or RequestOut>=@DateTime)	" +
                        "	 and (RequestIn<=getdate() or RequestOut<=getdate())	" +
                        "	 and Layer_ID=1 and service_id='" + ServiceID + "'  and Method_ID ='" + MethodID + "'	" +
                        "	 group by Corelation_Request,Node_IP_Address,Service_id,Method_ID,Layer_ID	" +

                        "	 insert into #TEMP	" +
                        "	 select TT.Corelation_Request,TT.Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
                        "	 TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
                        "	 from  #TEMP T inner join dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 where TT.Layer_ID in(2,3)	" +
                        "	 group by TT.Corelation_Request,TT.Node_IP_Address,TT.Service_id,TT.Method_ID,TT.Layer_ID	" +

                        "	 update T set T.RequestIn=TT.RequestIn	" +
                        "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
                        "	 on	T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=1 and TT.RequestIn is not null	" +

                        "	 update T set T.RequestOut=TT.RequestOut	" +
                        "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=1 and TT.RequestOut is not null	" +

                        "	 update T set T.RequestIn=TT.RequestIn	" +
                        "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=2 and TT.RequestIn is not null	" +

                        "	 update T set T.RequestOut=TT.RequestOut	" +
                        "	 from #TEMP T 	" +
                        "	 inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=2 and TT.RequestOut is not null	" +

                        "	 update T set T.RequestIn=TT.RequestIn	" +
                        "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=3 and TT.RequestIn is not null	" +

                        "	 update T set T.RequestOut=TT.RequestOut	" +
                        "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=3 and TT.RequestOut is not null	" +

                        "	 insert into #TEMP	" +
                        "	 select TT.Corelation_Request,TT.Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
                        "	 TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
                        "	 from  #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock) 	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 where TT.Layer_ID in(9)	" +
                        "	 group by TT.Corelation_Request,TT.Node_IP_Address,TT.Service_id,TT.Method_ID,TT.Layer_ID	" +

                        "	 update T set T.RequestIn=TT.RequestIn	" +
                        "	 from #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock)	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=9 and TT.RequestIn is not null and Corelation_Session = 1 And Request_Flag = 1	" +

                        "	 update T set T.RequestOut=TT.RequestOut	" +
                        "	 from #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock)	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=9 and TT.RequestOut is not null and Corelation_Session = 1 And Response_Flag = 1	" +

                        "    Select * Into #TEMP4 From #TEMP where Layer_ID=9  " +

                        "	 insert into #TEMP	" +
                        "	 select TT.Corelation_Request,TT.Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
                        "	 TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
                        "	 from  #TEMP4 T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock) 	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 where TT.Layer_ID in(4)	" +
                        "	 group by TT.Corelation_Request,TT.Node_IP_Address,TT.Service_id,TT.Method_ID,TT.Layer_ID	" +

                        "	 update T set T.RequestIn=TT.RequestIn	" +
                        "	 from #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock)	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=4 and TT.RequestIn is not null and Corelation_Session = 0 And Request_Flag = 1	" +

                        "	 update T set T.RequestOut=TT.RequestOut	" +
                        "	 from #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock)	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
                        "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
                        "	 where T.Layer_ID=4 and TT.RequestOut is not null and Corelation_Session = 0 And Response_Flag = 1	" +


                        "	 update T set T.Node_IP_Address=	" +
                        "	 case when TTT.Node_IP_Address In ('10.71.87.18','10.71.87.19','10.71.87.24','10.71.87.25','10.71.87.17','10.71.87.22') 	" +
                        "	 then REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(T.Node_IP_Address,'esbsrv26','10.71.87.26'),'esbsrv27','10.71.87.27'),'esbsrv28','10.71.87.28'),'srvdc2esbapp1','10.71.87.31'),'srvdc2esbapp2','10.71.87.32'),'srvdc2esbapp3','10.71.87.33')	" +
                        "	 else case when TTT.Node_IP_Address In ('10.71.87.13','10.71.87.15','10.71.87.20','10.71.87.23') 	" +
                        "	 then REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(T.Node_IP_Address,'srvprd1esbapp1','10.71.87.100'),'srvprd1esbapp2','10.71.87.101'),'srvprd1esbapp3','10.71.87.102'),'srvprd1esbapp4','10.71.87.103'),'srvprd1esbapp4','10.71.87.104')	" +
                        "	 end end	" +
                        "	 from  #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock) 	" +
                        "	 on T.Corelation_Request=TT.Corelation_Request	" +
                        "	 inner join dbo.Tbl_Corelation_Log TTT with(nolock) 	" +
                        "	  on T.Corelation_Request=TTT.Corelation_Request	" +
                        "	 where T.Layer_ID in(4,9)	" +

                        "	 select DATEDIFF(ms,RequestIn,RequestOut)as [TIME],* into #TEMP2 from #TEMP order by 1 desc	" +

                        "	 select Node_IP_Address,count(1) as [Count1],ML.Layer_Name,convert(numeric(18,2),AVG([time])) as AVGS  into #temp3 	" +
                        "	 from #temp2 t 	" +
                        "	 left outer join temp_mstRequestType MR (nolock) on t.Method_ID=MR.Method_ID	" +
                        "	 and t.Service_ID=MR.Service_ID	" +
                        "	 left outer join temp_mstLayer ML (Nolock) on T.Layer_ID=ML.Layer_ID	" +
                        "	 where time is not null " +
                        "	 group by Node_IP_Address,Layer_Name	" +
                        "	 order by 1	" +

                        "	select Node_IP_Address,Count1 [Count],Layer_Name,AVGS,Case	" +
                        "	When Node_IP_Address In ('10.71.87.18','10.71.87.19','10.71.87.17') and layer_Name='UI' Then 'UI_Merchant'	" +
                        "	When Node_IP_Address In ('10.71.87.24','10.71.87.25','10.71.87.22','10.71.87.21') and layer_Name='BLL' Then 'BLL_Merchant'	" +
                        "	When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33','10.71.87.26','10.71.87.27','10.71.87.28') and layer_Name='ESB_IN' Then 'ESB_Merchant'	" +
                        "	When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33','10.71.87.26','10.71.87.27','10.71.87.28') and layer_Name='CBS' Then 'CBS_Merchant'	" +
                        "	When Node_IP_Address In ('10.71.87.13','10.71.87.15') and layer_Name='UI' Then 'UI_Branch'	" +
                        "	When Node_IP_Address In ('10.71.87.20','10.71.87.23') and layer_Name='BLL' Then 'BLL_Branch'	" +
                        "	When Node_IP_Address In ('10.71.87.100','10.71.87.101','10.71.87.102','10.71.87.103','10.71.87.104') and layer_Name='ESB_IN' Then 'ESB_Branch'	" +
                        "	When Node_IP_Address In ('10.71.87.100','10.71.87.101','10.71.87.102','10.71.87.103','10.71.87.104') and layer_Name='CBS' Then 'CBS_Branch' End AS Layer	" +
                        "	from #temp3	" +
                        "	group by Node_IP_Address,Count1,Layer_Name,AVGS,Case	" +
                        "	When Node_IP_Address In ('10.71.87.18','10.71.87.19','10.71.87.17') and layer_Name='UI' Then 'UI_Merchant'	" +
                        "	When Node_IP_Address In ('10.71.87.24','10.71.87.25','10.71.87.22','10.71.87.21') and layer_Name='BLL' Then 'BLL_Merchant'	" +
                        "	When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33','10.71.87.26','10.71.87.27','10.71.87.28') and layer_Name='ESB_IN' Then 'ESB_Merchant'	" +
                        "	When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33','10.71.87.26','10.71.87.27','10.71.87.28') and layer_Name='CBS' Then 'CBS_Merchant'	" +
                        "	When Node_IP_Address In ('10.71.87.13','10.71.87.15') and layer_Name='UI' Then 'UI_Branch'	" +
                        "	When Node_IP_Address In ('10.71.87.20','10.71.87.23') and layer_Name='BLL' Then 'BLL_Branch'	" +
                        "	When Node_IP_Address In ('10.71.87.100','10.71.87.101','10.71.87.102','10.71.87.103','10.71.87.104') and layer_Name='ESB_IN' Then 'ESB_Branch'	" +
                        "	When Node_IP_Address In ('10.71.87.100','10.71.87.101','10.71.87.102','10.71.87.103','10.71.87.104') and layer_Name='CBS' Then 'CBS_Branch' End	" +
                        "	order by 3	" +
                        "	drop table #temp	" +
                        "	drop table #temp2	" +
                        "	drop table #temp4	" +
                        "	drop table #temp3	";

                }
                else if (LatencyType == "Layer")
                {
                    str =
                        " select * Into #TEMP " +
                        " From ESB_Transactions.dbo.Tbl_Correlation_Log with(nolock) " +
                        " where RequestIn>=@DateTime And Service_ID = '" + ServiceID + "' And Method_ID = '" + MethodID + "' And Layer_ID=9 " +

                        " Select s.Node_IP_Address,s.Layer_Name,s.Count into #temp3 From " +
                        " (select count(x.Corelation_Request) [Count],y.Layer_Name,IsNull(x.Node_IP_Address,0) [Node_IP_Address] " +
                        " From PBLogs.dbo.Tbl_Corelation_Log x  with(nolock)" +
                        " Inner Join temp_mstLayer y ON x.Layer_ID = y.layer_ID " +
                        " where Node_IP_Address In ('10.71.87.18','10.71.87.19','10.71.87.24','10.71.87.25','10.71.87.17','10.71.87.22') " +
                        " And RequestIn>=@DateTime And Service_ID = '" + ServiceID + "' And Method_ID = '" + MethodID + "' " +
                        " Group By x.Node_IP_Address,y.Layer_Name " +
                        " Union " +
                        " select count(x.Corelation_Request) [Count],y.Layer_Name,IsNull(x.Node_IP_Address,0) [Node_IP_Address] " +
                        " From PBLogs.dbo.Tbl_Corelation_Log x  with(nolock)" +
                        " Inner Join temp_mstLayer y ON x.Layer_ID = y.layer_ID " +
                        " where Node_IP_Address In ('10.71.87.13','10.71.87.15','10.71.87.20','10.71.87.23') " +
                        " And RequestIn>=@DateTime And Service_ID = '" + ServiceID + "' And Method_ID = '" + MethodID + "' " +
                        " Group By x.Node_IP_Address,y.Layer_Name " +
                        " Union " +
                        " select count(x.Corelation_Request) [Count],y.Layer_Name,Case " +
                        " When x.Node_IP_Address ='esbsrv26' Then '10.71.87.26' " +
                        " When x.Node_IP_Address ='esbsrv27' Then '10.71.87.27' " +
                        " When x.Node_IP_Address ='esbsrv28' Then '10.71.87.28' " +
                        " When x.Node_IP_Address ='srvdc2esbapp1' Then '10.71.87.31' " +
                        " When x.Node_IP_Address ='srvdc2esbapp2' Then '10.71.87.32' " +
                        " When x.Node_IP_Address ='srvdc2esbapp3' Then '10.71.87.33' " +
                        " When x.Node_IP_Address ='srvprd1esbapp1' Then '10.71.87.100' " +
                        " When x.Node_IP_Address ='srvprd1esbapp2' Then '10.71.87.101' " +
                        " When x.Node_IP_Address ='srvprd1esbapp3' Then '10.71.87.102' " +
                        " When x.Node_IP_Address ='srvprd1esbapp4' Then '10.71.87.103' " +
                        " When x.Node_IP_Address ='srvprd1esbapp5' Then '10.71.87.104' " +
                        " End AS Node_IP_Address " +
                        " From #TEMP t with(nolock) " +
                        " Inner Join ESB_Transactions.dbo.Tbl_Correlation_Log x  ON x.Corelation_Request = t.Corelation_Request " +
                        " Inner Join temp_mstLayer y ON x.Layer_ID = y.layer_ID " +
                        " where x.RequestIn>=@DateTime And x.Service_ID = '" + ServiceID + "' And x.Method_ID = '" + MethodID + "' " +
                        " Group By x.Node_IP_Address,y.Layer_Name) as s " +
                        " Order By 1 " +

                        " select Node_IP_Address,Count [Count],Layer_Name,Case " +
                        " When Node_IP_Address In ('10.71.87.18','10.71.87.19','10.71.87.17') and Layer_Name='UI' Then 'UI_Merchant' " +
                        " When Node_IP_Address In ('10.71.87.24','10.71.87.25','10.71.87.22') and Layer_Name='BLL' Then 'BLL_Merchant' " +
                        " When Node_IP_Address In ('10.71.87.26','10.71.87.27','10.71.87.28','10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='ESB_IN' Then 'ESB_Merchant' " +
                        " When Node_IP_Address In ('10.71.87.26','10.71.87.27','10.71.87.28','10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='CBS' Then 'CBS_Merchant' " +
                        " When Node_IP_Address In ('10.71.87.13','10.71.87.15') and Layer_Name='UI' Then 'UI_Branch' " +
                        " When Node_IP_Address In ('10.71.87.20','10.71.87.23') and Layer_Name='BLL' Then 'BLL_Branch' " +
                        " When Node_IP_Address In ('10.71.87.100','10.71.87.101','10.71.87.102','10.71.87.103','10.71.87.104') and Layer_Name='ESB_IN' Then 'ESB_Branch' " +
                        " When Node_IP_Address In ('10.71.87.100','10.71.87.101','10.71.87.102','10.71.87.103','10.71.87.104') and Layer_Name='CBS' Then 'CBS_Branch' End AS Branch " +
                        " from #temp3 " +
                        " group by Node_IP_Address,Count,Layer_Name,Case " +
                        " When Node_IP_Address In ('10.71.87.18','10.71.87.19','10.71.87.17') and Layer_Name='UI' Then 'UI_Merchant' " +
                        " When Node_IP_Address In ('10.71.87.24','10.71.87.25','10.71.87.22') and Layer_Name='BLL' Then 'BLL_Merchant' " +
                        " When Node_IP_Address In ('10.71.87.26','10.71.87.27','10.71.87.28','10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='ESB' Then 'ESB_Merchant' " +
                        " When Node_IP_Address In ('10.71.87.26','10.71.87.27','10.71.87.28','10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='CBS' Then 'CBS_Merchant' " +
                        " When Node_IP_Address In ('10.71.87.13','10.71.87.15') and Layer_Name='UI' Then 'UI_Branch' " +
                        " When Node_IP_Address In ('10.71.87.20','10.71.87.23') and Layer_Name='BLL' Then 'BLL_Branch' " +
                        " When Node_IP_Address In ('10.71.87.100','10.71.87.101','10.71.87.102','10.71.87.103','10.71.87.104') and Layer_Name='ESB_IN' Then 'ESB_Branch' " +
                        " When Node_IP_Address In ('10.71.87.100','10.71.87.101','10.71.87.102','10.71.87.103','10.71.87.104') and Layer_Name='CBS' Then 'CBS_Branch' End " +
                        " Order By 3 " +
                        " drop table #temp" +
                        " drop table #temp3 ";
                }
                else
                {
                    str =
                        " Select Node_IP_Address,count(Node_IP_Address) [Count1] into #temp " +
                        " from PBLogs.dbo.Tbl_Corelation_Log (nolock) " +
                        " Where Method_ID = '" + MethodID + "' And Service_ID = '" + ServiceID + "' And RequestIn>=@DateTime " +
                        " Group By Node_IP_Address " +

                        " select Node_IP_Address,Count1 [Count],Case " +
                        " When Node_IP_Address In ('10.71.87.141','10.71.87.142','10.71.87.143','10.71.87.144') Then 'BPay'End AS Layer " +
                        " from #temp " +
                        " group by Node_IP_Address,Count1,Case " +
                        " When Node_IP_Address In ('10.71.87.141','10.71.87.142','10.71.87.143','10.71.87.144') Then 'BPay'End " +
                        " order by 3 " +

                        " drop table #temp ";
                }
                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion

        //Active tab 14
        #region EKO Query
        public string GetQueryofEKOData(string eko_status, string TxnStatus, string clientname)
        {
            string str = string.Empty;

            string clientids = WebConfigurationManager.AppSettings["ClientID"];
            string clientnames = WebConfigurationManager.AppSettings["ClientName"];

            string[] clids = clientids.Split(',');
            string[] clnm = clientnames.Split(',');

            try
            {
                if (eko_status == "Detail")
                {
                    string Casestr = string.Empty;

                    if (clids.Length == clnm.Length)
                    {
                        for (int i = 0; i < clids.Length; i++)
                        {
                            clids[i] = clids[i].Trim();
                        }

                        for (int j = 0; j < clnm.Length; j++)
                        {
                            clnm[j] = clnm[j].Trim();
                            Casestr += " When R.ClientId = '" + clids[j] + "' Then '" + clnm[j] + "'";
                        }
                    }

                    str = "	select * into #TEMP	" +
                          "	from  PBLOGS.dbo.TxnPGPaymentRequest with(nolock)	" +
                          "	where ClientId In (" + clientids + ") and ReqDateTime >= @DateTime	and ReqDateTime <= @EndDateTime " +

                          "	select * into #TEMP2	" +
                          "	from PBLOGS.dbo.TxnPGPaymentResponse R  with (nolock) 	" +
                          "	where ClientId In (" + clientids + ") and r.LogDateTime >= @DateTime and r.LogDateTime <= @EndDateTime " +

                          "	select Case " + Casestr + " End as ClientID, case 	" +
                          "	when R.ProductCode ='IMPSFT' then 'IMPS' else 	" +
                          "	case when R.ProductCode='NEFTFT' then 'NEFT' else 'TxnEnq' end end as TransactionType,	" +
                          "	count(distinct r.RequestId) as Initiated,	" +
                          "	Sum( case when RR.ResponseCode='0' then 1 else 0 end) as Success	" +
                          "	,sum( case when RR.ResponseCode='1' then 1 else 0 end) as Failed	" +
                          "	from #TEMP R  with (nolock) 	" +
                          "	left outer join #TEMP2 RR  with (nolock) on R.RequestId=RR.RequestId 	" +

                          "	group by Case " + Casestr + " End ,case when R.ProductCode ='IMPSFT' then 'IMPS' else 	" +
                          "	case when R.ProductCode='NEFTFT' then 'NEFT' else 'TxnEnq' end end	" +

                          "	drop table #TEMP	" +
                          "	drop table #TEMP2";
                }
                else if (eko_status == "Detail_Success")
                {
                    int clientid = 0;
                    if (clids.Length == clnm.Length)
                    {
                        for (int i = 0; i < clids.Length; i++)
                        {
                            if (clientname == clnm[i])
                            {
                                clientid = Convert.ToInt32(clids[i]);
                            }
                        }
                    }

                    if (TxnStatus != "")
                    {
                        str = "	select * into #TEMP	" +
                             "	from PBLOGS.dbo.TxnPGPaymentRequest with(nolock) where ClientId= " + clientid + "	" +
                             "	and ReqDateTime >=@DateTime and ReqDateTime <= @EndDateTime and ProductCode = '" + TxnStatus + "'	" +

                             "	select * into #TEMP2	" +
                             "	from PBLOGS.dbo.TxnPGPaymentResponse R  with (nolock) 	" +
                             "	where ClientId=" + clientid + " and r.LogDateTime >= @DateTime and r.LogDateTime <= @EndDateTime  And ResponseCode='0'" +

                             "	select TT.Actcode,isnull(Message,'Unknown Response')as[Message],isnull(TypeStatus,'Pending') as TxnStatus ,COUNT(1) as CNT	" +
                             "	from #Temp T with(nolock)	" +
                             "	left outer join #TEMP2 TT with(nolocK) on T.RequestId=TT.RequestId " +
                             "	left outer join PBMaster.dbo.mstEKoResponseCode EK with(nolock) on TT.ResponseCode=EK.ResponseCode and TT.Actcode=EK.ActCode and LEFT(T.ProductCode,4)=EK.TransType	" +
                             "  Where TT.ResponseCode = '0' " +
                             "	group by TT.Actcode,isnull(Message,'Unknown Response'),isnull(TypeStatus,'Pending')	" +

                             "	drop table #TEMP	" +
                             "	drop table #TEMP2";
                    }
                    else
                    {
                        str = "	select * into #TEMP	" +
                             "	from PBLOGS.dbo.TxnPGPaymentRequest with(nolock) where ClientId=" + clientid + "	" +
                             "	and ReqDateTime >= @DateTime and ReqDateTime <= @EndDateTime  and ProductCode = ''	" +

                             "	select * into #TEMP2	" +
                             "	from PBLOGS.dbo.TxnPGPaymentResponse with(nolock) where ClientId= " + clientid + "	" +
                             "	And LogDateTime >= @DateTime and LogDateTime <= @EndDateTime 		" +

                             "	Select Enq.Actcode,Enq.Message,Enq.TypeStatus as TxnStatus,Count(1) as CNT	" +
                             "	From #Temp T with(nolock)	" +
                             "	Left Outer Join #Temp2 TT with(nolock) ON T.RequestId=TT.RequestId " +
                             "	Left Outer Join PBMaster.dbo.mstEkoTransactionEnquiryCode Enq ON TT.ActCode = Enq.ActCode	" +
                             "	where TT.ClientId=" + clientid + " and TT.ResponseCode='0' 	" +
                             "	group by Enq.Actcode,Enq.Message,Enq.typeStatus	" +

                             "	drop table #TEMP " +
                             "	drop table #TEMP2";
                    }
                }
                else if (eko_status == "Detail_Failed")
                {
                    int clientid = 0;
                    if (clids.Length == clnm.Length)
                    {
                        for (int i = 0; i < clids.Length; i++)
                        {
                            if (clientname == clnm[i])
                            {
                                clientid = Convert.ToInt32(clids[i]);
                            }
                        }
                    }

                    if (TxnStatus != "")
                    {
                        str = "	select * into #TEMP	" +
                                 "	from PBLOGS.dbo.TxnPGPaymentRequest with(nolock) where ClientId= " + clientid + "	" +
                                 "	and ReqDateTime >=@DateTime and ReqDateTime <= @EndDateTime and ProductCode = '" + TxnStatus + "'	" +

                                 "	select * into #TEMP2	" +
                                 "	from PBLOGS.dbo.TxnPGPaymentResponse R  with (nolock) 	" +
                                 "	where ClientId= " + clientid + " and r.LogDateTime >= @DateTime and r.LogDateTime <= @EndDateTime And ResponseCode='1'" +

                                 "	select TT.Actcode,isnull(Message,'Unknown Response')as[Message],isnull(TypeStatus,'Pending') as TxnStatus ,COUNT(1) as CNT	" +
                                 "	from #Temp T with(nolock)	" +
                                 "	left outer join #TEMP2 TT with(nolocK) on T.RequestId=TT.RequestId " +
                                 "	left outer join PBMaster.dbo.mstEKoResponseCode EK with(nolock) on TT.ResponseCode=EK.ResponseCode and TT.Actcode=EK.ActCode and LEFT(T.ProductCode,4)=EK.TransType	" +
                                 "  Where TT.ResponseCode = '1' " +
                                 "	group by TT.Actcode,isnull(Message,'Unknown Response'),isnull(TypeStatus,'Pending')	" +
                                 "	drop table #TEMP	" +
                                 "	drop table #TEMP2";
                    }
                    else
                    {
                        str = "	select * into #TEMP	" +
                             "	from PBLOGS.dbo.TxnPGPaymentRequest with(nolock) where ClientId= " + clientid + "	" +
                             "	and ReqDateTime >= @DateTime and ReqDateTime <= @EndDateTime and ProductCode = ''	" +

                             "	select * into #TEMP2	" +
                             "	from PBLOGS.dbo.TxnPGPaymentResponse with(nolock) where ClientId= " + clientid + "	" +
                             "	And LogDateTime >= @DateTime and LogDateTime <= @EndDateTime 		" +

                             "	Select Enq.Actcode,Enq.Message,Enq.TypeStatus as TxnStatus,Count(1) as CNT	" +
                             "	From #Temp T with(nolock)	" +
                             "	Left Outer Join #Temp2 TT with(nolock) ON T.RequestId=TT.RequestId " +
                             "	Left Outer Join PBMaster.dbo.mstEkoTransactionEnquiryCode Enq ON TT.ActCode = Enq.ActCode	" +
                             "	where TT.ClientId=" + clientid + " and TT.ResponseCode='1' 	" +
                             "	group by Enq.Actcode,Enq.Message,Enq.typeStatus	" +

                             "	drop table #TEMP " +
                             "	drop table #TEMP2";
                    }
                }
                else if (eko_status == "ServerStatus")
                {
                    str = "	select (Case When left(Corelation_RequestID,12)='ECO_SYS_USR5' then '10.71.87.191'	" +
                         "	When left(Corelation_RequestID,12)='ECO_SYS_USR6' then '10.71.87.192' When left(Corelation_RequestID,12)='ECO_SYS_USR7' then '10.71.87.193' When left(Corelation_RequestID,12)='ECO_SYS_USR8' then '10.71.87.194' end) Node_Server,count(1) Req_Count  " +
                         "	From PBLOGS.dbo.TxnPGPaymentRequest with(nolock) " +
                         "	where LogDateTime > @DateTime and LogDateTime <= @EndDateTime And substring(Corelation_RequestID,0,13) IN ('ECO_SYS_USR5','ECO_SYS_USR6','ECO_SYS_USR7','ECO_SYS_USR8') " +
                         "	group by left(Corelation_RequestID,12) order by 1 ";
                }
                else
                {
                    str = "	Select  CASE WHEN req.ProductCode = '' THEN 'TxnEnquiry' ELSE req.ProductCode END  AS ProductCode,req.amount,req.ReqDateTime,res.CtrxId,res.MessageString,res.Actcode From PBLOGS..TxnPGPaymentRequest req	" +
                          "	Inner Join PBLOGS..TxnPGPaymentResponse res ON req.RequestId = res.RequestId	" +
                          "	Where res.ResponseCode=1 And req.LogDateTime >= @DateTime and req.LogDateTime <= @EndDateTime And res.ClientId = '7'	" +
                          "	Order By ReqDateTime Desc	";
                }

                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion

        #region AEPS API Query
        public string GetQueryofAEPSAPI(string aepsapi_status, string requestflag, string clientname, string ProductCode)
        {
            string str = string.Empty;

            string clientids = WebConfigurationManager.AppSettings["AEPSClientID"];
            string clientnames = WebConfigurationManager.AppSettings["AEPSClientName"];

            string[] clids = clientids.Split(',');
            string[] clnm = clientnames.Split(',');

            try
            {
                if (aepsapi_status == "Detail")
                {
                    string Casestr = string.Empty;

                    if (clids.Length == clnm.Length)
                    {
                        for (int i = 0; i < clids.Length; i++)
                        {
                            clids[i] = clids[i].Trim();
                        }

                        for (int j = 0; j < clnm.Length; j++)
                        {
                            clnm[j] = clnm[j].Trim();
                            Casestr += " When R.ClientId = '" + clids[j] + "' Then '" + clnm[j] + "'";
                        }
                    }

                    str = "	select * into #TEMP	" +
                          "	from  PBBC_API.dbo.TxnAepsPGPaymentRequest with(nolock)	" +
                          "	where ClientId In (" + clientids + ") and ReqDateTime >= @DateTime	and ReqDateTime <= @EndDateTime " +

                          "	select * into #TEMP2	" +
                          "	from PBBC_API.dbo.TxnAepsPGPaymentResponse R  with (nolock) 	" +
                          "	where ClientId In (" + clientids + ") and r.LogDateTime >= @DateTime and r.LogDateTime <= @EndDateTime " +

                          "	select Case " + Casestr + " End as ClientName,R.ProductCode,R.RequestFlag, " +
                          " Case When R.RequestFlag = 'PG' AND R.ProductCode = 'CASHWAEPSACQ' Then 'AEPS CASH Withdrawal'  " +
                          " When R.RequestFlag = 'PG' AND R.ProductCode = 'ATMWD' Then 'MATM CASH Withdrawal' " +
                          " When R.RequestFlag = 'AEPS_BalanceEnquiry' AND R.ProductCode = 'CASHWAEPSACQ' Then 'AEPS BalanceEnquiry' " +
                          " When R.RequestFlag = 'CardTransaction_BalanceEnquiry' AND R.ProductCode = 'ATMWD' Then 'MATM BalanceEnquiry'  " +
                          " When R.RequestFlag = 'CardTransactionStatus' AND R.ProductCode = 'ATMWD' Then 'MATM Transaction Status'  " +
                          " When R.RequestFlag = 'AEPSTransactionStatus' AND R.ProductCode = 'CASHWAEPSACQ' Then 'AEPS Transaction Status' " +
                          " End As TransactionType, " +
                          "	count(distinct r.RequestId) as Initiated,	" +
                          "	Sum( case when RR.ResponseCode='0' then 1 else 0 end) as Success	" +
                          "	,sum( case when RR.ResponseCode != '0' then 1 else 0 end) as Failed	" +
                          "	from #TEMP R  with (nolock) 	" +
                          "	left outer join #TEMP2 RR  with (nolock) on R.RequestId=RR.RequestId 	" +

                          "	group by Case " + Casestr + " End , R.ProductCode,R.RequestFlag," +
                          " Case When R.RequestFlag = 'PG' AND R.ProductCode = 'CASHWAEPSACQ' Then 'AEPS CASH Withdrawal'  " +
                          " When R.RequestFlag = 'PG' AND R.ProductCode = 'ATMWD' Then 'MATM CASH Withdrawal' " +
                          " When R.RequestFlag = 'AEPS_BalanceEnquiry' AND R.ProductCode = 'CASHWAEPSACQ' Then 'AEPS BalanceEnquiry' " +
                          " When R.RequestFlag = 'CardTransaction_BalanceEnquiry' AND R.ProductCode = 'ATMWD' Then 'MATM BalanceEnquiry'  " +
                          " When R.RequestFlag = 'CardTransactionStatus' AND R.ProductCode = 'ATMWD' Then 'MATM Transaction Status' " +
                          " When R.RequestFlag = 'AEPSTransactionStatus' AND R.ProductCode = 'CASHWAEPSACQ' Then 'AEPS Transaction Status' END " +

                          "	drop table #TEMP	" +
                          "	drop table #TEMP2";
                }
                else if (aepsapi_status == "Detail_Success")
                {
                    int clientid = 0;
                    if (clids.Length == clnm.Length)
                    {
                        for (int i = 0; i < clids.Length; i++)
                        {
                            if (clientname == clnm[i])
                            {
                                clientid = Convert.ToInt32(clids[i]);
                            }
                        }
                    }

                    str = "	select * into #TEMP	" +
                         "	from PBBC_API.dbo.TxnAepsPGPaymentRequest with(nolock) where ClientId= " + clientid + "	" +
                         "	and ReqDateTime >=@DateTime and ReqDateTime <= @EndDateTime and RequestFlag = '" + requestflag + "' And ProductCode = '" + ProductCode + "'" +

                         "	select * into #TEMP2	" +
                         "	from PBBC_API.dbo.TxnAepsPGPaymentResponse R  with (nolock) 	" +
                         "	where ClientId=" + clientid + " and r.LogDateTime >= @DateTime and r.LogDateTime <= @EndDateTime  And ResponseCode='0'" +

                         "	Select TT.Actcode,TT.DisplayMesage,Count(1) as CNT	" +
                         "	from #Temp T with(nolock)	" +
                         "	left outer join #TEMP2 TT with(nolocK) on T.RequestId=TT.RequestId " +
                         "  Where TT.ResponseCode = '0' " +
                         "	group by TT.Actcode,TT.DisplayMesage	" +

                         "	drop table #TEMP	" +
                         "	drop table #TEMP2";
                }
                else if (aepsapi_status == "Detail_Failed")
                {
                    int clientid = 0;
                    if (clids.Length == clnm.Length)
                    {
                        for (int i = 0; i < clids.Length; i++)
                        {
                            if (clientname == clnm[i])
                            {
                                clientid = Convert.ToInt32(clids[i]);
                            }
                        }
                    }

                    str = "	select * into #TEMP	" +
                         "	from PBBC_API.dbo.TxnAepsPGPaymentRequest with(nolock) where ClientId= " + clientid + "	" +
                         "	and ReqDateTime >=@DateTime and ReqDateTime <= @EndDateTime and RequestFlag = '" + requestflag + "' And ProductCode = '" + ProductCode + "'" +

                         "	select * into #TEMP2	" +
                         "	from PBBC_API.dbo.TxnAepsPGPaymentResponse R  with (nolock) 	" +
                         "	where ClientId=" + clientid + " and r.LogDateTime >= @DateTime and r.LogDateTime <= @EndDateTime  And ResponseCode!='0'" +

                         "	Select TT.Actcode,TT.DisplayMesage,Count(1) as CNT	" +
                         "	from #Temp T with(nolock)	" +
                         "	left outer join #TEMP2 TT with(nolocK) on T.RequestId=TT.RequestId " +
                         "  Where TT.ResponseCode != '0' " +
                         "	group by TT.Actcode,TT.DisplayMesage	" +

                         "	drop table #TEMP	" +
                         "	drop table #TEMP2";
                }

                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion

        #region CityCash Query
        public string GetQueryofCityCashData(string citycashstatus, string transactiontype, int projectid)
        {
            string str = string.Empty;

            try
            {
                if (citycashstatus == "GetAllData")
                {
                    str = "	select * into #TEMP " +
                          "	from  PBTransactionInfo.dbo.TxnAPIPaymentRequest with(nolock)	" +
                          "	where ClientId = 10 and ReqDateTime >= @DateTime	and ReqDateTime <= @EndDateTime " +

                          "	select * into #TEMP2	" +
                          "	from PBTransactionInfo.dbo.TxnAPIPaymentResponse R  with (nolock) 	" +
                          "	where ClientId = 10 and r.LogDateTime >= @DateTime and r.LogDateTime <= @EndDateTime " +

                          " select Case When R.ProductCode = M.ProductCode Then M.TransactionDescription Else R.ProductCode End As [TransactionType], " +
                          " R.ProductCode,p.ProjectID,ISNULL(p.Description,'No Project ID') AS [Project], " +
                          " count(distinct r.RequestId) as Initiated,  " +
                          " Sum( case when RR.ResponseCode='0' then 1 else 0 end) as Success	 " +
                          " ,sum( case when RR.ResponseCode !='0' then 1 else 0 end) as Failed	 " +
                          " from #TEMP R  with (nolock) 	 " +
                          " left outer join #TEMP2 RR  with (nolock) on R.RequestId=RR.RequestId 	 " +
                          " left outer join PBMaster..mstProjectID p ON R.ProjectID = p.ProjectID  " +
                          " left outer join pbmaster..MstPGProductCityCash M ON R.ProjectID = M.ProjectID And R.ProductCode = M.ProductCode " +
                          " group by Case When R.ProductCode = M.ProductCode Then M.TransactionDescription Else R.ProductCode End, " +
                          " R.ProductCode,p.ProjectID,p.Description " +

                          "	drop table #TEMP	" +
                          "	drop table #TEMP2";
                }
                else if (citycashstatus == "GetSuccessData")
                {
                    str = "	select * into #TEMP2	" +
                          "	from PBTransactionInfo.dbo.TxnAPIPaymentResponse  with (nolock) " +
                          "	where RequestId IN (select Distinct(RequestId)	" +
                          "	from  PBTransactionInfo.dbo.TxnAPIPaymentRequest with(nolock) " +
                          "	where ClientId = 10  And ReqDateTime >= @DateTime	and ReqDateTime <= @EndDateTime And ProductCode = '" + transactiontype + "'  And ProjectID = " + projectid + ") " +

                          "	Select ResponseCode,DisplayMesage,Rfu1,Count(1) AS [Count] From #TEMP2 Where ResponseCode = 0 Group By ResponseCode,DisplayMesage,Rfu1	" +

                          "	drop table #TEMP2";
                }
                else
                {
                    str = "	select * into #TEMP2	" +
                         "	from PBTransactionInfo.dbo.TxnAPIPaymentResponse  with (nolock) " +
                         "	where RequestId IN (select Distinct(RequestId)	" +
                         "	from  PBTransactionInfo.dbo.TxnAPIPaymentRequest with(nolock) " +
                         "	where ClientId = 10  And ReqDateTime >= @DateTime	and ReqDateTime <= @EndDateTime And ProductCode = '" + transactiontype + "'  And ProjectID = " + projectid + ") " +

                         "	Select ResponseCode,DisplayMesage,Rfu1,Count(1) AS [Count] From #TEMP2 Where ResponseCode != 0 Group By ResponseCode,DisplayMesage,Rfu1	" +

                         "	drop table #TEMP2";
                }

                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }

        public string GetQueryofCityCashDataByMobileNumber(string citycashstatus, string transactiontype, string customernumber)
        {
            string str = string.Empty;

            try
            {
                if (citycashstatus == "GetAllData")
                {
                    str = "	select * into #TEMP " +
                          "	from  PBTransactionInfo.dbo.TxnAPIPaymentRequest with(nolock)	" +
                          "	where ClientId = 10 and Convert(Date,ReqDateTime) = Convert(Date,GetDate()) And CustomerMobileNo = '" + customernumber + "' " +

                          "	select * into #TEMP2	" +
                          "	from PBTransactionInfo.dbo.TxnAPIPaymentResponse R  with (nolock) 	" +
                          "	where RequestId IN (select Distinct(RequestId) from  #TEMP) " +

                          " select Case When R.ProductCode = M.ProductCode Then M.TransactionDescription Else R.ProductCode End As [TransactionType], " +
                          " R.ProductCode,p.ProjectID,ISNULL(p.Description,'No Project ID') AS [Project], " +
                          " count(distinct r.RequestId) as Initiated,  " +
                          " Sum( case when RR.ResponseCode='0' then 1 else 0 end) as Success	 " +
                          " ,sum( case when RR.ResponseCode !='0' then 1 else 0 end) as Failed	 " +
                          " from #TEMP R  with (nolock) 	 " +
                          " left outer join #TEMP2 RR  with (nolock) on R.RequestId=RR.RequestId 	 " +
                          " left outer join PBMaster..mstProjectID p ON R.ProjectID = p.ProjectID  " +
                          " left outer join pbmaster..MstPGProductCityCash M ON R.ProjectID = M.ProjectID And R.ProductCode = M.ProductCode " +
                          " group by Case When R.ProductCode = M.ProductCode Then M.TransactionDescription Else R.ProductCode End, " +
                          " R.ProductCode,p.ProjectID,p.Description " +

                          "	drop table #TEMP	" +
                          "	drop table #TEMP2";
                }
                else if (citycashstatus == "GetSuccessData")
                {
                    str = "	select ResponseCode,DisplayMesage,Count(1) AS [Count]  from PBTransactionInfo..TxnAPIPaymentResponse with (nolock) 	" +
                          "	where RequestId IN (select Distinct(RequestId)from  PBTransactionInfo.dbo.TxnAPIPaymentRequest with(nolock) " +
                          "	where ProductCode = '" + transactiontype + "' And CustomerMobileNo = '" + customernumber + "') And Convert(Date,LogDateTime) = Convert(Date,GetDate()) And ResponseCode = 0	" +
                          "	Group By ResponseCode,DisplayMesage ";
                }
                else
                {
                    str = "	select ResponseCode,DisplayMesage,Count(1) AS [Count]  from PBTransactionInfo..TxnAPIPaymentResponse with (nolock) 	" +
                          "	where RequestId IN (select Distinct(RequestId)from  PBTransactionInfo.dbo.TxnAPIPaymentRequest with(nolock) " +
                          "	where ProductCode = '" + transactiontype + "' And CustomerMobileNo = '" + customernumber + "') And Convert(Date,LogDateTime) = Convert(Date,GetDate()) And ResponseCode != 0	" +
                          "	Group By ResponseCode,DisplayMesage ";
                }

                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }
        #endregion

        #region CMS API Query
        public string GetQueryofCMSAPIData(string cmsstatus, string PartnerID, string productcode)
        {
            string str = string.Empty;

            try
            {
                if (cmsstatus == "GetAllData")
                {
                    str = "	select * into #TEMP " +
                          "	from  PBTransactionInfo..TxnPartnerCMSRequest with(nolock)	" +
                          "	where ReqDateTime >= @DateTime	and ReqDateTime <= @EndDateTime " +

                          "	select * into #TEMP2	" +
                          "	from PBTransactionInfo..TxnPartnerCMSResponse  with (nolock) 	" +
                          "	where LogDateTime >= @DateTime and LogDateTime <= @EndDateTime " +

                          " Select R1.PartnerID,R1.productcode,R5.ClientName,R3.Request_Type, " +
                          " count(distinct R1.RequestId) as Initiated, " +
                          " Sum( case when R2.ResponseCode='0' then 1 else 0 end) as Success,  " +
                          " Sum( case when R2.ResponseCode !='0' then 1 else 0 end) as Failed	 " +
                          " From #TEMP R1 	 " +
                          " Inner Join #TEMP2 R2 ON R1.RequestID=R2.RequestID 	 " +
                          " Inner Join pbmaster..mstRequestType R3 ON R1.productcode=R3.Method_ID  " +
                          " Inner Join pbmaster..mstPartnerCMSClient R4 ON R1.ClientId=R4.ClientId " +
                          " Inner Join pbmaster..mstpgclient R5 ON R1.PartnerID=R5.clientid " +
                          " Where R3.service_ID = 42 " +
                          " group by R1.PartnerID,R1.productcode,R3.Request_Type,R5.ClientName " +

                          "	drop table #TEMP	" +
                          "	drop table #TEMP2";
                }
                else if (cmsstatus == "GetSuccessData")
                {
                    str = "	select * into #TEMP2	" +
                          "	from PBTransactionInfo..TxnPartnerCMSResponse  with (nolock) " +
                          "	where RequestId IN (select Distinct(RequestId)	" +
                          "	from  PBTransactionInfo..TxnPartnerCMSRequest R1 with(nolock) " +
                          " Inner Join pbmaster..mstRequestType R3 ON R1.productcode=R3.Method_ID " +
                          "	where R1.ReqDateTime >= @DateTime	and R1.ReqDateTime <= @EndDateTime And R1.PartnerID = " + PartnerID + " AND R1.productcode = " + productcode + " AND R3.service_ID = 42) " +

                          "	Select T2.ClientName,T1.ResponseCode,T1.DisplayMesage,Count(1) AS [Count] From #TEMP2 T1 Inner Join pbmaster..mstPartnerCMSClient T2 ON T1.ClientId = T2.ClientID Where  T1.ResponseCode = 0 And LogDateTime >=  @DateTime and LogDateTime <= @EndDateTime Group By T2.ClientName,T1.ResponseCode,T1.DisplayMesage		" +

                          "	drop table #TEMP2";
                }
                else
                {
                    str = "	select * into #TEMP2	" +
                          "	from PBTransactionInfo..TxnPartnerCMSResponse  with (nolock) " +
                          "	where RequestId IN (select Distinct(RequestId)	" +
                          "	from  PBTransactionInfo..TxnPartnerCMSRequest R1 with(nolock) " +
                          " Inner Join pbmaster..mstRequestType R3 ON R1.productcode=R3.Method_ID " +
                          "	where R1.ReqDateTime >= @DateTime	and R1.ReqDateTime <= @EndDateTime And R1.PartnerID = " + PartnerID + " AND R1.productcode = " + productcode + " AND R3.service_ID = 42) " +

                          "	Select T2.ClientName,T1.ResponseCode,T1.DisplayMesage,Count(1) AS [Count] From #TEMP2 T1 Inner Join pbmaster..mstPartnerCMSClient T2 ON T1.ClientId = T2.ClientID Where  T1.ResponseCode != 0 And LogDateTime >=  @DateTime and LogDateTime <= @EndDateTime Group By T2.ClientName,T1.ResponseCode,T1.DisplayMesage		" +

                          "	drop table #TEMP2";
                }

                return str;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                str = string.Empty;
            }
        }

        #endregion

        //#region ESB Zone Latency BETA Query
        //public string GetQueryofESBLatencyData(string LatencyType, string ServiceID, string MethodID)
        //{
        //    string str = string.Empty;

        //    try
        //    {
        //        if (LatencyType == "NoLayer")
        //        {
        //            str =
        //                    "  Use PBLogs " +
        //                    "	 select Corelation_Request,Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
        //                    "	 Service_id,Method_ID,Layer_ID	" +
        //                    "	 into #TEMP	" +
        //                    "	 from dbo.Tbl_Corelation_Log with(nolocK)	" +
        //                    "	 where (RequestIn>=@DateTime or RequestOut>=@DateTime)	" +
        //                    "	 and (RequestIn<=getdate() or RequestOut<=getdate())	" +
        //                    "	 and Layer_ID=1 and service_id='" + ServiceID + "'  and Method_ID ='" + MethodID + "'	" +
        //                    "	 group by Corelation_Request,Node_IP_Address,Service_id,Method_ID,Layer_ID	" +

        //                    "	 insert into #TEMP	" +
        //                    "	 select TT.Corelation_Request,TT.Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
        //                    "	 TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
        //                    "	 from  #TEMP T inner join dbo.Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 where TT.Layer_ID in(2,3)	" +
        //                    "	 group by TT.Corelation_Request,TT.Node_IP_Address,TT.Service_id,TT.Method_ID,TT.Layer_ID	" +

        //                    "	 update T set T.RequestIn=TT.RequestIn	" +
        //                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
        //                    "	 on	 T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=1 and TT.RequestIn is not null	" +

        //                    "	 update T set T.RequestOut=TT.RequestOut	" +
        //                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=1 and TT.RequestOut is not null	" +

        //                    "	 update T set T.RequestIn=TT.RequestIn	" +
        //                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=2 and TT.RequestIn is not null	" +

        //                    "	 update T set T.RequestOut=TT.RequestOut	" +
        //                    "	 from #TEMP T 	" +
        //                    "	 inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=2 and TT.RequestOut is not null	" +

        //                    "	 update T set T.RequestIn=TT.RequestIn	" +
        //                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request)	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=3 and TT.RequestIn is not null	" +

        //                    "	 update T set T.RequestOut=TT.RequestOut	" +
        //                    "	 from #TEMP T inner join Tbl_Corelation_Log TT with(nolock index=IX_Corelation_Request) 	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=3 and TT.RequestOut is not null	" +

        //                    "	 insert into #TEMP	" +
        //                    "	 select TT.Corelation_Request,TT.Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
        //                    "	 TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
        //                    "	 from  #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock) 	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 where TT.Layer_ID in(4)	" +
        //                    "	 group by TT.Corelation_Request,TT.Node_IP_Address,TT.Service_id,TT.Method_ID,TT.Layer_ID	" +

        //                    "	 update T set T.RequestIn=TT.RequestIn	" +
        //                    "	 from #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock)	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=4 and TT.RequestIn is not null and Corelation_Session = 0 And Request_Flag = 1	" +

        //                    "	 update T set T.RequestOut=TT.RequestOut	" +
        //                    "	 from #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock)	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=4 and TT.RequestOut is not null and Corelation_Session = 0 And Response_Flag = 1	" +

        //                    "	 insert into #TEMP	" +
        //                    "	 select TT.Corelation_Request,TT.Node_IP_Address,convert(datetime,null) as RequestIn,convert(datetime,null) as RequestOut,	" +
        //                    "	 TT.Service_id,TT.Method_ID,TT.Layer_ID	" +
        //                    "	 from  #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock) 	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 where TT.Layer_ID in(9)	" +
        //                    "	 group by TT.Corelation_Request,TT.Node_IP_Address,TT.Service_id,TT.Method_ID,TT.Layer_ID	" +

        //                    "	 update T set T.RequestIn=TT.RequestIn	" +
        //                    "	 from #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock)	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=9 and TT.RequestIn is not null and Corelation_Session = 1 And Request_Flag = 1	" +

        //                    "	 update T set T.RequestOut=TT.RequestOut	" +
        //                    "	 from #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock)	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 and T.Node_IP_Address=TT.Node_IP_Address and T.Service_id=TT.Service_id	" +
        //                    "	 and T.Method_ID=TT.Method_ID and T.Layer_ID=TT.Layer_ID	" +
        //                    "	 where T.Layer_ID=9 and TT.RequestOut is not null and Corelation_Session = 1 And Response_Flag = 1	" +

        //                    "	 update T set T.Node_IP_Address=	" +
        //                    "	 case when TTT.Node_IP_Address In ('10.71.93.37','10.71.93.65') 	" +
        //                    "	 then REPLACE(REPLACE(REPLACE(T.Node_IP_Address,'srvdc2esbapp1','10.71.87.31'),'srvdc2esbapp2','10.71.87.32'),'srvdc2esbapp3','10.71.87.33')	" +
        //                    "	 end 	" +
        //                    "	 from  #TEMP T inner join ESB_Transactions.dbo.Tbl_Correlation_Log TT with(nolock) 	" +
        //                    "	 on T.Corelation_Request=TT.Corelation_Request	" +
        //                    "	 inner join dbo.Tbl_Corelation_Log TTT with(nolock) 	" +
        //                    "	  on T.Corelation_Request=TTT.Corelation_Request	" +
        //                    "	 where T.Layer_ID in(4,9)	" +

        //                    "	 select DATEDIFF(ms,RequestIn,RequestOut)as [TIME],* into #TEMP2 from #TEMP order by 1 desc	" +

        //                    "	 select Node_IP_Address,count(1) as [Count1],ML.Layer_Name,convert(numeric(18,2),AVG([time])) as AVGS  into #temp3 	" +
        //                    "	 from #temp2 t 	" +
        //                    "	 left outer join temp_mstRequestType MR (nolock) on t.Method_ID=MR.Method_ID	" +
        //                    "	 and t.Service_ID=MR.Service_ID	" +
        //                    "	 left outer join temp_mstLayer ML (Nolock) on T.Layer_ID=ML.Layer_ID	" +
        //                    "	 where time is not null 	" +
        //                    "	 group by Node_IP_Address,Layer_Name	" +
        //                    "	 order by 1	" +

        //                    "   select Node_IP_Address,Count1 [Count],Layer_Name,AVGS,Case	" +
        //                    "	When Node_IP_Address In ('10.71.93.37') and Layer_Name='UI' Then 'UI_Beta'	" +
        //                    "	When Node_IP_Address In ('10.71.93.65') and Layer_Name='BLL' Then 'BLL_Beta'	" +
        //                    "	When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='ESB_IN' Then 'ESB_Beta'	" +
        //                    "	When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='CBS' Then 'CBS_Beta' End AS Layer	" +
        //                    "	from #temp3	 " +
        //                    "	group by Node_IP_Address,Count1,Layer_Name,AVGS,Case	" +

        //                    "	When Node_IP_Address In ('10.71.93.37') and Layer_Name='UI' Then 'UI_Beta'	" +
        //                    "	When Node_IP_Address In ('10.71.93.65') and Layer_Name='BLL' Then 'BLL_Beta'	" +
        //                    "	When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='ESB_IN' Then 'ESB_Beta'	" +
        //                    "	When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='CBS' Then 'CBS_Beta' End	" +
        //                    "	order by 3	" +
        //                    "	drop table #temp	" +
        //                    "	drop table #temp2	" +
        //                    "	drop table #temp3	";


        //        }
        //        else
        //        {
        //            str =
        //                " Select s.Node_IP_Address,s.Layer_Name,s.Count into #temp3 From " +
        //                " (select count(x.Corelation_Request) [Count],y.Layer_Name,IsNull(x.Node_IP_Address,0) [Node_IP_Address] " +
        //                " From PBLogs.dbo.Tbl_Corelation_Log x  " +
        //                " Inner Join PBMaster.dbo.mstLayer y ON x.Layer_ID = y.layer_ID " +
        //                " where Node_IP_Address In ('10.71.93.37','10.71.93.65') " +
        //                " And RequestIn>=@DateTime And Service_ID = '" + ServiceID + "' And Method_ID = '" + MethodID + "' " +
        //                " Group By x.Node_IP_Address,y.Layer_Name " +
        //                " Union " +
        //                " select count(x.Corelation_Request) [Count],y.Layer_Name,IsNull(x.Node_IP_Address,0) [Node_IP_Address] " +
        //                " From PBLogs.dbo.Tbl_Corelation_Log x  " +
        //                " Inner Join PBMaster.dbo.mstLayer y ON x.Layer_ID = y.layer_ID " +
        //                " where Node_IP_Address In ('10.71.93.37','10.71.93.65') " +
        //                " And RequestIn>=@DateTime And Service_ID = '" + ServiceID + "' And Method_ID = '" + MethodID + "' " +
        //                " Group By x.Node_IP_Address,y.Layer_Name " +
        //                " Union " +
        //                " select count(x.Corelation_Request) [Count],y.Layer_Name,Case " +
        //                " When x.Node_IP_Address ='srvdc2esbapp1' Then '10.71.87.31' " +
        //                " When x.Node_IP_Address ='srvdc2esbapp2' Then '10.71.87.32' " +
        //                " When x.Node_IP_Address ='srvdc2esbapp3' Then '10.71.87.33' End AS Node_IP_Address " +
        //                " From ESB_Transactions.dbo.Tbl_Correlation_Log x  " +
        //                " Inner Join PBMaster.dbo.mstLayer y ON x.Layer_ID = y.layer_ID " +
        //                " where RequestIn>=@DateTime And Service_ID = '" + ServiceID + "' And Method_ID = '" + MethodID + "' " +
        //                " Group By x.Node_IP_Address,y.Layer_Name) as s " +
        //                " Order By 1 " +

        //                " select Node_IP_Address,Count [Count],Layer_Name,Case " +
        //                " When Node_IP_Address In ('10.71.93.37') and Layer_Name='UI' Then 'UI_Beta' " +
        //                " When Node_IP_Address In ('10.71.93.65') and Layer_Name='BLL' Then 'BLL_Beta' " +
        //                " When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='ESB_IN' Then 'ESB_Beta' " +
        //                " When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='CBS' Then 'CBS_Beta' End AS Branch " +
        //                " from #temp3 " +
        //                " group by Node_IP_Address,Count,Layer_Name,Case " +
        //                " When Node_IP_Address In ('10.71.93.37') and Layer_Name='UI' Then 'UI_Beta' " +
        //                " When Node_IP_Address In ('10.71.93.65') and Layer_Name='BLL' Then 'BLL_Beta' " +
        //                " When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='ESB_IN' Then 'ESB_Beta' " +
        //                " When Node_IP_Address In ('10.71.87.31','10.71.87.32','10.71.87.33') and Layer_Name='CBS' Then 'CBS_Beta' End " +
        //                " Order By 3 " +
        //                " drop table #temp3 ";
        //        }
        //        return str;
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message.ToString();
        //    }
        //    finally
        //    {
        //        str = string.Empty;
        //    }
        //}
        //#endregion                

        #region Transactions Query
        private string GetTransactionsStringBuilder(int TransactionsType)
        {
            StringBuilder strQuery;
            try
            {
                if (TransactionsType == 1)
                {
                    strQuery = new StringBuilder();
                    strQuery.Append("select R.PCODE,R.TRACE,R.X_CORRELATION_ID,R.AMOUNT,R.LOCAL_DATE,R.TERMID,R.LOCAL_TIME,RESPCODE,Response_Msg,STAN,AEPS_RUPAY_TranDate,AEPS_RUPAY_TranTime,AcquiringInstitution_Id, ");

                    strQuery.Append("replace(convert(varchar(14),R.LOCAL_TIME,101),'/','') + replace(convert(varchar(8),R.LOCAL_TIME,114),':','')  LOCAL_TIME,RES.MSGTYPE ReqType ");
                    strQuery.Append("from dbo.TransactionsRequest R with(nolock INDEX = IX_LOCAL_DATE) ");
                    strQuery.Append("inner join dbo.TransactionsResponse Res with(nolock INDEX = IX_LOCAL_DATE) on r.TRACE=res.TRACE and r.Request_Type=res.Request_Type ");
                    strQuery.Append("where R.pcode in ('RUPAYBalanceEnqPosting','RUPAYPosting') And R.LOCAL_TIME >= @DateTime ");
                    return strQuery.ToString();
                }
                else if (TransactionsType == 2)
                {
                    strQuery = new StringBuilder();
                    strQuery.Append("select Corelation_Request, RequestIn, RequestOut,Method_ID,Service_ID into #temp from Tbl_Corelation_Log with (nolock) where Layer_ID=1 and Service_ID<>0 and RequestIn >=@DateTime and Status_Code in(1,0)   ");
                    strQuery.Append("Update b Set RequestOut = a.RequestOut from #temp b inner join Tbl_Corelation_Log a with (nolock index = IX_Corelation_Request) on a.Corelation_Request = b.Corelation_Request ");
                    strQuery.Append("Select x.Request_Type as [Request_Type],ISNULL(x.timeslot1,0) as [Response_UI_0_1Sec], ");
                    strQuery.Append("ISNULL(y.timeslot2,0) as [Response_UI_1_3Sec], ISNULL(z.timeslot2,0) as [Response_UI_3_Sec] ");
                    strQuery.Append("from (select mrt.Request_Type, Count(DATEDIFF(ss,tl.RequestIn,tl.RequestOut)) as timeslot1 ");
                    strQuery.Append("from #temp tl (nolock) ");
                    strQuery.Append("inner join temp_mstRequestType mrt (nolock)on tl.Method_ID = mrt.Method_ID and tl.Service_ID = mrt.Service_ID  ");
                    strQuery.Append("where DATEDIFF(ss,tl.RequestIn,tl.RequestOut)<=1 group by mrt.Request_Type) as x ");
                    strQuery.Append("left outer join (select mrt.Request_Type, Count(DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)) as timeslot2 from #temp tl1  ");
                    strQuery.Append("inner join temp_mstRequestType mrt (nolock) on tl1.Method_ID = mrt.Method_ID and tl1.Service_ID = mrt.Service_ID ");
                    strQuery.Append("where DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)>=1 and DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)<=3 ");
                    strQuery.Append("group by mrt.Request_Type ) as y on x.Request_Type=y.Request_Type left outer join ");
                    strQuery.Append("(select mrt.Request_Type,Count(DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)) as timeslot2 from #temp tl1 ");
                    strQuery.Append("inner join temp_mstRequestType mrt (nolock) on tl1.Method_ID = mrt.Method_ID and tl1.Service_ID = mrt.Service_ID ");
                    strQuery.Append("where DATEDIFF(ss,tl1.RequestIn,tl1.RequestOut)>3 ");
                    strQuery.Append("group by mrt.Request_Type )as Z on x.Request_Type=Z.Request_Type ");
                    strQuery.Append("where x.Request_Type is not NULL order by x.Request_Type  ");

                    strQuery.Append(" drop table #temp  ");
                    return strQuery.ToString();
                }
                else
                {
                    strQuery = new StringBuilder();
                    strQuery.Append("select Corelation_Request,Request_Type,Layer_Name,Response_Message,Node_IP_Address,count(1) as [Count],RequestOut From ");
                    strQuery.Append("(select Corelation_Request,Request_Type,Layer_Name,Response_Message,Node_IP_Address,RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with(nolocK) left outer join temp_mstRequestType mstR (nolock) on x.Method_ID=mstr.Method_ID and x.Service_ID=mstr.Service_ID left outer join temp_mstLayer L (nolock) on x.Layer_ID=L.Layer_ID ");
                    strQuery.Append("where (RequestIn>=@DateTime or RequestOut>=@DateTime) and Response_Flag='1' and Response_Message<>'' and Layer_Name='UI' and Response_Message like '%Failed%' ");
                    strQuery.Append("Union all ");
                    strQuery.Append("select Corelation_Request,Request_Type,Layer_Name,Response_Message,Node_IP_Address,RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with(nolocK) left outer join temp_mstRequestType mstR (nolock)on x.Method_ID=mstr.Method_ID and x.Service_ID=mstr.Service_ID left outer join temp_mstLayer L (nolock) on x.Layer_ID=L.Layer_ID ");
                    strQuery.Append("where (Requestin>=@DateTime or RequestOut>=@DateTime) and Response_Flag='1' and Response_Message<>'' and Layer_Name='UI'and Response_Message like '%Error%' ");
                    strQuery.Append("Union all ");
                    strQuery.Append("select Corelation_Request,Request_Type,Layer_Name,Response_Message,Node_IP_Address,RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with(nolocK) left outer join temp_mstRequestType mstR (nolock)on x.Method_ID=mstr.Method_ID and x.Service_ID=mstr.Service_ID left outer join temp_mstLayer L (nolock) on x.Layer_ID=L.Layer_ID ");
                    strQuery.Append("where (Requestin>=@DateTime or RequestOut>=@DateTime) and Response_Flag='1' and mstR.Service_ID  not in('3','19') and Layer_Name='UI'and Response_Message like '%Primary%' ");
                    strQuery.Append("Union all  ");
                    strQuery.Append("select Corelation_Request,Request_Type,Layer_Name,Response_Message,Node_IP_Address,RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with(nolocK) left outer join temp_mstRequestType mstR (nolock) on x.Method_ID=mstr.Method_ID and x.Service_ID=mstr.Service_ID left outer join temp_mstLayer L (nolock) on x.Layer_ID=L.Layer_ID ");
                    strQuery.Append("where (RequestIn>=@DateTime or RequestOut>=@DateTime) and Response_Flag='1' and Response_Message<>'' and Layer_Name='UI' and Response_Message like '%Server%' ");
                    strQuery.Append("Union all  ");
                    strQuery.Append("select Corelation_Request,Request_Type,Layer_Name,Response_Message,Node_IP_Address,RequestOut ");
                    strQuery.Append("from PBLogs.dbo.Tbl_Corelation_Log x with(nolocK) left outer join temp_mstRequestType mstR (nolock) on x.Method_ID=mstr.Method_ID and x.Service_ID=mstr.Service_ID left outer join temp_mstLayer L (nolock) on x.Layer_ID=L.Layer_ID ");
                    strQuery.Append("where (RequestIn>=@DateTime or RequestOut>=@DateTime) and Response_Flag='1' and Response_Message<>'' and Layer_Name='UI' and Response_Message like '%IMPS Disable%') As temp ");
                    strQuery.Append("group by Corelation_Request,Request_Type,Layer_Name,Response_Message,Node_IP_Address,RequestOut order by RequestOut desc ");
                    return strQuery.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                strQuery = null;
            }
        }
        #endregion

        #region AEPS Transactions Query
        private string Get_AEPS_RUPAYTransactionsStringBuilder(string TransactionsType)
        {
            StringBuilder strQuery = new StringBuilder();
            try
            {
                strQuery = new StringBuilder();
                //strQuery.Append("select r.appid,res.PCODE,res.RESPCODE,COUNT(r.trace) RequestCount,sum(case when x.trace is null then 0 else 1 end) TotalReversal ");
                //strQuery.Append("from ESB_Transactions..TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) ");
                //strQuery.Append("inner join  ESB_Transactions..TransactionsResponse res   with (nolock INDEX = IX_LOCAL_DATE) on R.Trace=res.trace and res.Request_type=r.Request_type and  r.MSGTYPE = res.MSGTYPE  ");
                //strQuery.Append("left outer join (select trace from ESB_Transactions..TransactionsRequest res with (nolock) where res.msgtype=1) x on Res.trace=x.trace ");
                //strQuery.Append("where convert(varchar(8),r.LOCAL_DATE,112)= convert(varchar(8),GETDATE(),112) and r.PCODE in ('" + TransactionsType + "') And r.LOCAL_TIME >= @DateTime ");
                //strQuery.Append("group by r.appid,res.PCODE,res.RESPCODE");

                //17/05/2019
                strQuery.Append(" DECLARE @Localdate DATETIME = convert(VARCHAR(8), GETDATE(), 112) ");

                strQuery.Append(" SELECT appid, trace, Request_Type, MSGTYPE, LOCAL_TIME INTO #TEMP ");
                strQuery.Append(" FROM dbo.TransactionsRequest R WITH (NOLOCK INDEX = IX_LOCAL_DATE) ");
                strQuery.Append(" WHERE r.LOCAL_Date = @Localdate AND r.PCODE IN ('" + TransactionsType + "') ");

                strQuery.Append(" SELECT Res.Trace, Res.Request_Type, Res.MSGTYPE, PCODE, RESPCODE INTO #TEMP2  ");
                strQuery.Append(" FROM dbo.TransactionsResponse Res WITH (NOLOCK INDEX = Ix_Local_date) ");
                strQuery.Append(" INNER JOIN #TEMP R ON R.Trace = res.trace  AND res.Request_type = r.Request_type AND r.MSGTYPE = res.MSGTYPE ");
                strQuery.Append(" WHERE res.LOCAL_Date = @Localdate ");

                strQuery.Append(" SELECT r.appid, res.PCODE, res.RESPCODE, COUNT(r.trace) RequestCount, ");
                strQuery.Append(" sum(CASE WHEN x.trace IS NULL THEN 0 ELSE 1 END) TotalReversal ");
                strQuery.Append(" FROM #Temp R ");
                strQuery.Append(" INNER JOIN #Temp2 Res WITH (NOLOCK) ON R.Trace = res.trace AND res.Request_type = r.Request_type AND r.MSGTYPE = res.MSGTYPE ");
                strQuery.Append(" LEFT OUTER JOIN (SELECT Trace FROM #Temp WHERE msgtype = 1 ) x ON Res.trace = x.trace ");
                strQuery.Append(" WHERE r.LOCAL_TIME >= @DateTime GROUP BY r.appid, res.PCODE, res.RESPCODE ");

                strQuery.Append(" DROP TABLE #Temp DROP TABLE #TEMP2 ");
                return strQuery.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                strQuery = null;
            }
        }
        #endregion

        #region AEPS ISSUER Transactions Query
        private string Get_AEPSISSUER_TransactionsStringBuilder()
        {
            StringBuilder strQuery = new StringBuilder();
            try
            {
                strQuery = new StringBuilder();
                strQuery.Append("select r.appid,res.PCODE,res.RESPCODE,COUNT(r.trace) RequestCount,sum(case when x.trace is null then 0 else 1 end) TotalReversal,Sum(Cast(R.AMOUNT As Int)) AS [Amt] ");
                strQuery.Append("from ESB_Transactions..TransactionsRequest R  with (nolock INDEX = IX_LOCAL_DATE) ");
                strQuery.Append("inner join  ESB_Transactions..TransactionsResponse res   with (nolock INDEX = IX_LOCAL_DATE) on R.Trace=res.trace and res.Request_type=r.Request_type and  r.MSGTYPE = res.MSGTYPE  ");
                strQuery.Append("left outer join (select trace from ESB_Transactions..TransactionsRequest res with (nolock) where res.msgtype=1) x on Res.trace=x.trace ");
                strQuery.Append("where convert(varchar(8),r.LOCAL_DATE,112)= convert(varchar(8),GETDATE(),112) and r.PCODE in ('CASHDAEPSISS','FTAEPSISS','CASHWAEPSISS','CASHWAPAYISS','AEPSIssuerBalanceInq','AEPSIssuerReversal') And r.LOCAL_TIME >= @DateTime ");
                strQuery.Append("group by r.appid,res.PCODE,res.RESPCODE");

                return strQuery.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                strQuery = null;
            }
        }
        #endregion

        #region Refire Query
        public string RefireQueryData(int refire, string from, string to)
        {
            string query = string.Empty;

            if (refire == 1)
            {
                query = " SELECT X_Corelation_Id,appId,UserId,Trace,PCODE,ResponseCode,CostCenter,GL_AccountNo,Case  " +
                        " When Status = 200 Then 'Success' When Status = 201 Then 'Already Process' Else 'Pending' End as Status,Logdatetime " +
                        " FROM [ESB_Transactions].[dbo].[Refire_Txn_Log]  with (nolock)  " +
                        " Where (Logdatetime>= '" + from + " 00:00:00:000' and Logdatetime<= '" + to + " 23:59:59:999')" +
                        " Group By X_Corelation_Id,appId,UserId,Trace,PCODE,ResponseCode,CostCenter,GL_AccountNo,Case  " +
                        " When Status = 200 Then 'Success' When Status = 201 Then 'Already Process' Else 'Pending' End,Logdatetime " +
                        " Order By Logdatetime desc ";

            }
            else if (refire == 2)
            {
                query = " SELECT X_Corelation_Id,appId,UserId,Trace,PCODE,ResponseCode,CostCenter,GL_AccountNo,Status,Logdatetime " +
                        " FROM [ESB_Transactions].[dbo].[Refire_Txn_Log]  with (nolock) " +
                        " Where (appId = 'FINOMER' or appId = 'FINOMERNP') and len(UserId) = 9  " +
                        " and Logdatetime>= @DateTime " +
                        " and Status <> 200 " +
                        " Order By Logdatetime desc";
            }
            else if (refire == 3)
            {
                query = " SELECT appId,PCODE, SUM(case when Status='0' then 1 else 0 end) as Pending," +
                        " SUM(case when Status='200' then 1 else 0 end) as Success,SUM(case when Status='201' then 1 else 0 end) as AlreadyProcess, " +
                        " SUM(case when Status not in('0','200','201') then 1 else 0 end) as Retried, " +
                        " Count(1) as [Total] FROM [ESB_Transactions].[dbo].[Refire_Txn_Log]  with (nolock) " +
                        " Where (Logdatetime>= ' " + from + " 00:00:00:000' and Logdatetime<= '" + to + " 23:59:59:999') Group By appId,PCODE ";
            }
            return query;
        }
        #endregion

        #region Suryoday Query
        public string SuryodayQueryData(string date)
        {
            string query = string.Empty;

            query = " Select f1.File_Name,f1.Total_No_Of_Records,f1.No_Record_Processed As [Records_Processed], " +
                    " SUM(case when f2.IsDuplicateFlag = 1 And f2.IsDuplicateFlag != '' then 1 else 0 end) as [Duplicate_Record], " +
                    " SUM(case when f3.Status_Flag = 0 And f3.Status_Flag != '' then 1 else 0 end) as [Record_Not_Process], " +
                    " SUM(case when f3.Partner_CIF_Return_Code = 0 And f3.Partner_CIF_Return_Code != '' then 1 else 0 end) as [Cif_Created], " +
                    " SUM(case when f3.Partner_CIF_Return_Code = 204 And f3.Partner_CIF_Return_Code != '' then 1 else 0 end) as [Cif_Already_Available], " +
                    " SUM(case when f3.Partner_CIF_Return_Code NOT IN (0,204) then 1 else 0 end) as [Cif_Creation_Failed], " +
                    " SUM(case when f3.Partner_Account_Return_Code = 0 And f3.Partner_Account_Return_Code != '' then 1 else 0 end) as [Acc_Created], " +
                    " SUM(case when f3.Partner_Account_Return_Code = 204 And f3.Partner_Account_Return_Code != '' then 1 else 0 end) as [Acc_Already_Available], " +
                    " SUM(case when f3.Partner_Account_Return_Code NOT IN (0,204) then 1 else 0 end) as [Acc_Creation_Failed], " +
                    " SUM(case when f3.External_Account_Return_Code= 0 And f3.External_Account_Return_Code != '' then 1 else 0 end) as [Cif_Extension_Done], " +
                    " SUM(case when f3.External_Account_Return_Code= 204 And f3.External_Account_Return_Code != '' then 1 else 0 end) as [Cif_Extension_Already_Available], " +
                    " SUM(case when f3.External_Account_Return_Code NOT IN (0,204) then 1 else 0 end) as [Cif_Extension_Failed], " +
                    " SUM(case when f3.Status_Flag= 200 And f3.Status_Flag != '' then 1 else 0 end) as [Fund_Transfer_Success], " +
                    " SUM(case when f3.Status_Flag != '' And  f3.Status_Flag not in (0,200,1,2,3) then 1 else 0 end) as [Fund_Transfer_Failed] , "+
                    " SUM(case when f3.Status_Flag != '' And  f3.Status_Flag in(1,2,3) then 1 else 0 end) as [In_Process] , "+
                    " SUM(case when f3.Status_Flag != '' And  f3.Status_Flag =0 then 1 else 0 end) as [Pending_ForProcess]  "+
                    //" SUM(case when f3.Status_Flag != '' And  f3.Status_Flag not in (0,200) then 1 else 0 end) as [Fund_Transfer_Failed] " +
                    " From PartnerFile_SFTP_Process f1 with(nolock) " +
                    " Inner Join Partner_FileProcess_Details f2 with(nolock) ON f1.File_id = f2.FileId " +
                    " Left Join Partner_AccountOpening_Process f3 with(nolock) ON f2.ProcessId = f3.ProcessId " +
                    " Where Convert(Date,f2.ProcessDateTime) = '" + date + "' " +
                    " group By f1.File_Name,f1.Total_No_Of_Records,f1.No_Record_Processed ";

            return query;
        }
        #endregion

        #region Suryoday Sub Query
        public string SuryodaySubQueryData(string date)
        {
            string query = string.Empty;

            query = " SELECT Local_Date_Time, Partner_CIF_ResponseMessage,Partner_Account_ResponseMessage, Remark, Status_Flag " +
                    " FROM [ESB_Transactions].[dbo].[Partner_AccountOpening_Process] with (nolock) " +
                    " where status_flag <>'200' " +
                    " and Local_Date_Time > cast(getDate() as date) " +
                    " order by Local_Date_Time desc ";

            return query;
        }
        #endregion

        #region AccountOpeningQuery
        public string AccountOpeningQuery()
        {
            string query = "";
            query = " select ProductType,RequestType,UserClass,convert(varchar,requestIn,106) as [Date],count(distinct AccountNumber) as [Count] " +
                    " from dbo.tbl_accOpenInOutRequestTrack with(nolock) " +
                    " where requestIn >= @DateTime " +
                    " group by productType,RequestType,userClass,convert(varchar,requestIn,106)";
            return query;
        }
        #endregion

        #region Passthrough Query
        private string GetPassthroughStringBuilder()
        {
            string strQuery;
            try
            {
                strQuery = " IF OBJECT_ID('tempdb..#TxnDump') IS NOT NULL " +
                            " DROP TABLE #TxnDump  " +
                            " CREATE TABLE #TxnDump  " +
                            " (AppLications VARCHAR(50),InstanceId VARCHAR(50),IP_Port VARCHAR(50),NPCI_Port VARCHAR(50)) " +

                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('IMPS_OUT','O1','10.71.87.123:9015','192.168.62.159:8842') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('IMPS_OUT','O2','10.71.87.123:9016','192.168.62.159:8844') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('IMPS_OUT','O3','10.71.87.122:9015','192.168.62.159:9271') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('IMPS_OUT','O4','10.71.87.122:9016','192.168.62.159:9272') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('AEPS_OUT','A1','10.71.87.68:9001','192.168.62.158:8676') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('AEPS_OUT','A2','10.71.87.68:9002','192.168.62.158:8776') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('RUPAY_OUT','R1','10.71.87.68:9003','FIS_10.100.0.111:56001') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('IMPS_IN','I1','10.71.87.68:9004','192.168.62.159:8552') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('AEPS_IN','A1','10.71.87.68:9001','192.168.62.158:8676') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('AEPS_OUT','A3','10.71.87.68:9005','192.168.62.158:8813') " +
                            " Insert into #TxnDump (AppLications,InstanceId,IP_Port,NPCI_Port) values('AEPS_OUT','A4','10.71.87.68:9006','192.168.62.158:8815') " +

                            " IF OBJECT_ID('tempdb..#Final') IS NOT NULL " +
                            " DROP TABLE #Final  " +

                            " IF OBJECT_ID('tempdb..#IMPS_Results_Response') IS NOT NULL " +
                            " DROP TABLE #IMPS_Results_Response " +
                            " IF OBJECT_ID('tempdb..#IMPS_Results_Request') IS NOT NULL " +
                            " DROP TABLE #IMPS_Results_Request  " +
                            " IF OBJECT_ID('tempdb..#Final_IMPS_REQ') IS NOT NULL " +
                            " DROP TABLE #Final_IMPS_REQ " +
                            " IF OBJECT_ID('tempdb..#Final_IMPS_REP') IS NOT NULL " +
                            " DROP TABLE #Final_IMPS_REP  " +

                            " Select * into #IMPS_Results_Response From T_IMPS_Pass_OUT_Response WITH(NOLOCK) where   Log_Date >=  @DateTime    " +
                            " Select * into #IMPS_Results_Request From T_IMPS_Pass_OUT_Request WITH(NOLOCK) where   Log_Date >=  @DateTime  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100.0 / Count(1)) as Fail_Request_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_IMPS_REQ " +
                            " FROM  #IMPS_Results_Request a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100.0 / Count(1)) as Fail_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_IMPS_REP " +
                            " FROM  #IMPS_Results_Response a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc  " +

                            " Select AppLications,a.InstanceId,a.Request_Total,a.Success Request_Success,a.Fail Request_Socket_Error,CONVERT(NUMERIC(10,2),a.Fail_Request_Percent)Fail_Request_Percent, " +
                            " b.Request_Total Response_Total,b.Success Response_Success,b.Fail Response_Time_Out ,CONVERT(NUMERIC(10,2),b.Fail_Percent) Fail_Response_Percent, " +
                            " IP_Port,NPCI_Port,a.Max_SuccessDateTime Last_SuccessRequest,a.Max_FailDateTime Last_FailRequest,b.Max_SuccessDateTime Last_SuccessResponse,b.Max_FailDateTime Last_FailResponse " +
                            " into #Final " +
                            " from #Final_IMPS_REQ a left outer join " +
                            " #Final_IMPS_REP b on a.InstanceId=b.InstanceId  " +
                            " left join #TxnDump c on a.InstanceId=c.InstanceId  " +

                            " IF OBJECT_ID('tempdb..#IMPS_Results_Response_IN') IS NOT NULL " +
                            " DROP TABLE #IMPS_Results_Response_IN " +
                            " IF OBJECT_ID('tempdb..#IMPS_Results_Request_IN') IS NOT NULL " +
                            " DROP TABLE #IMPS_Results_Request_IN  " +
                            " IF OBJECT_ID('tempdb..#Final_IMPS_REQ_IN') IS NOT NULL " +
                            " DROP TABLE #Final_IMPS_REQ_IN " +
                            " IF OBJECT_ID('tempdb..#Final_IMPS_REP_IN') IS NOT NULL " +
                            " DROP TABLE #Final_IMPS_REP_IN " +

                            " Select * into #IMPS_Results_Response_IN From T_IMPS_Pass_IN_Response WITH(NOLOCK) where   Log_Date >=  @DateTime    " +
                            " Select * into #IMPS_Results_Request_IN From T_IMPS_Pass_IN_Request WITH(NOLOCK) where   Log_Date >=  @DateTime  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100.0 / Count(1)) as Fail_Request_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_IMPS_REQ_IN " +
                            " FROM  #IMPS_Results_Request_IN a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100.0 / Count(1)) as Fail_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_IMPS_REP_IN " +
                            " FROM  #IMPS_Results_Response_IN a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc " +

                            " Insert into #Final (AppLications,InstanceId,Request_Total,Request_Success,Request_Socket_Error,Fail_Request_Percent,Response_Total,Response_Success,Response_Time_Out,Fail_Response_Percent,IP_Port,NPCI_Port, Last_SuccessRequest, Last_FailRequest, Last_SuccessResponse, Last_FailResponse) " +
                            " Select AppLications,a.InstanceId,a.Request_Total,a.Success Request_Success,a.Fail Request_Socket_Error,a.Fail_Request_Percent, " +
                            " b.Request_Total Response_Total,b.Success Response_Success,b.Fail Response_Time_Out ,b.Fail_Percent Fail_Response_Percent, " +
                            " IP_Port,NPCI_Port,a.Max_SuccessDateTime Last_SuccessRequest,a.Max_FailDateTime Last_FailRequest,b.Max_SuccessDateTime Last_SuccessResponse,b.Max_FailDateTime Last_FailResponse " +
                            " from #Final_IMPS_REQ_IN a left outer join " +
                            " #Final_IMPS_REP_IN b on a.InstanceId=b.InstanceId  " +
                            " left join #TxnDump c on a.InstanceId=c.InstanceId  " +

                            " IF OBJECT_ID('tempdb..#RUPAY_Results_Response') IS NOT NULL " +
                            " DROP TABLE #RUPAY_Results_Response " +
                            " IF OBJECT_ID('tempdb..#RUPAY_Results_Request') IS NOT NULL " +
                            " DROP TABLE #RUPAY_Results_Request  " +
                            " IF OBJECT_ID('tempdb..#Final_RUPAY_REQ') IS NOT NULL " +
                            " DROP TABLE #Final_RUPAY_REQ " +
                            " IF OBJECT_ID('tempdb..#Final_RUPAY_REP') IS NOT NULL " +
                            " DROP TABLE #Final_RUPAY_REP  " +

                            " Select * into #RUPAY_Results_Response From T_RUPAY_Pass_OUT_Response WITH(NOLOCK) where   Log_Date >=  @DateTime    " +
                            " Select * into #RUPAY_Results_Request From T_RUPAY_Pass_OUT_Request WITH(NOLOCK) where   Log_Date >=  @DateTime  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100.0 / Count(1)) as Fail_Request_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_RUPAY_REQ " +
                            " FROM  #RUPAY_Results_Request a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100.0 / Count(1)) as Fail_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_RUPAY_REP " +
                            " FROM  #RUPAY_Results_Response a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc  " +

                            " Insert into #Final (AppLications,InstanceId,Request_Total,Request_Success,Request_Socket_Error,Fail_Request_Percent,Response_Total,Response_Success,Response_Time_Out,Fail_Response_Percent,IP_Port,NPCI_Port, Last_SuccessRequest, Last_FailRequest, Last_SuccessResponse, Last_FailResponse) " +
                            " Select AppLications,a.InstanceId,a.Request_Total,a.Success Request_Success,a.Fail Request_Socket_Error,a.Fail_Request_Percent, " +
                            " b.Request_Total Response_Total,b.Success Response_Success,b.Fail Response_Time_Out ,b.Fail_Percent Fail_Response_Percent, " +
                            " IP_Port,NPCI_Port,a.Max_SuccessDateTime Last_SuccessRequest,a.Max_FailDateTime Last_FailRequest,b.Max_SuccessDateTime Last_SuccessResponse,b.Max_FailDateTime Last_FailResponse " +
                            " from #Final_RUPAY_REQ a left outer join " +
                            " #Final_RUPAY_REP b on a.InstanceId=b.InstanceId  " +
                            " left join #TxnDump c on a.InstanceId=c.InstanceId  " +

                            " IF OBJECT_ID('tempdb..#AEPS_Results_Response') IS NOT NULL " +
                            " DROP TABLE #AEPS_Results_Response " +
                            " IF OBJECT_ID('tempdb..#AEPS_Results_Request') IS NOT NULL " +
                            " DROP TABLE #AEPS_Results_Request  " +
                            " IF OBJECT_ID('tempdb..#Final_AEPS_REQ') IS NOT NULL " +
                            " DROP TABLE #Final_AEPS_REQ " +
                            " IF OBJECT_ID('tempdb..#Final_AEPS_REP') IS NOT NULL " +
                            " DROP TABLE #Final_AEPS_REP  " +

                            " Select * into #AEPS_Results_Response From T_AEPS_Pass_OUT_Response WITH(NOLOCK) where   Log_Date >=  @DateTime    " +
                            " Select * into #AEPS_Results_Request From T_AEPS_Pass_OUT_Request WITH(NOLOCK) where   Log_Date >=  @DateTime  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100.0 / Count(1)) as Fail_Request_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_AEPS_REQ " +
                            " FROM  #AEPS_Results_Request a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100.0 / Count(1)) as Fail_Percent, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100.0 / Count(1)) as Fail_Request_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_AEPS_REP " +
                            " FROM  #AEPS_Results_Response a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc  " +

                            " Insert into #Final (AppLications,InstanceId,Request_Total,Request_Success,Request_Socket_Error,Fail_Request_Percent,Response_Total,Response_Success,Response_Time_Out,Fail_Response_Percent,IP_Port,NPCI_Port, Last_SuccessRequest, Last_FailRequest, Last_SuccessResponse, Last_FailResponse) " +
                            " Select AppLications,a.InstanceId,a.Request_Total,a.Success Request_Success,a.Fail Request_Socket_Error,a.Fail_Request_Percent, " +
                            " b.Request_Total Response_Total,b.Success Response_Success,b.Fail Response_Time_Out ,b.Fail_Percent Fail_Response_Percent, " +
                            " IP_Port,NPCI_Port,a.Max_SuccessDateTime Last_SuccessRequest,a.Max_FailDateTime Last_FailRequest,b.Max_SuccessDateTime Last_SuccessResponse,b.Max_FailDateTime Last_FailResponse " +
                            " from #Final_AEPS_REQ a left outer join " +
                            " #Final_AEPS_REP b on a.InstanceId=b.InstanceId  " +
                            " left join #TxnDump c on a.InstanceId=c.InstanceId and c.AppLications='AEPS_OUT' " +

                            " IF OBJECT_ID('tempdb..#AEPS_Results_Response_IN') IS NOT NULL " +
                            " DROP TABLE #AEPS_Results_Response_IN " +
                            " IF OBJECT_ID('tempdb..#AEPS_Results_Request_IN') IS NOT NULL " +
                            " DROP TABLE #AEPS_Results_Request_IN  " +
                            " IF OBJECT_ID('tempdb..#Final_AEPS_REQ_IN') IS NOT NULL " +
                            " DROP TABLE #Final_AEPS_REQ_IN " +
                            " IF OBJECT_ID('tempdb..#Final_AEPS_REP_IN') IS NOT NULL " +
                            " DROP TABLE #Final_AEPS_REP_IN " +

                            " Select * into #AEPS_Results_Response_IN From T_AEPS_Pass_IN_Response WITH(NOLOCK) where   Log_Date >=  @DateTime    " +
                            " Select * into #AEPS_Results_Request_IN From T_AEPS_Pass_IN_Request WITH(NOLOCK) where   Log_Date >=  @DateTime  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100 / Count(1)) as Fail_Request_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_AEPS_REQ_IN " +
                            " FROM  #AEPS_Results_Request_IN a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc  " +

                            " SELECT a.InstanceId,Count(a.Status) Request_Total, " +
                            " SUM( CASE WHEN a.Status=0 THEN 1 ELSE 0 END ) Success, " +
                            " SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) Fail, " +
                            " (SUM( CASE WHEN a.Status=1 THEN 1 ELSE 0 END ) * 100 / Count(1)) as Fail_Percent, " +
                            " MAX(CASE WHEN a.Status=0 THEN a.Log_date ELSE NULL END) Max_SuccessDateTime, " +
                            " MAX(CASE WHEN a.Status=1 THEN a.Log_date ELSE NULL END) Max_FailDateTime " +
                            " Into #Final_AEPS_REP_IN " +
                            " FROM  #AEPS_Results_Response_IN a  " +
                            " Group By a.InstanceId " +
                            " ORDER BY a.InstanceId desc  " +

                            " Insert into #Final (AppLications,InstanceId,Request_Total,Request_Success,Request_Socket_Error,Fail_Request_Percent,Response_Total,Response_Success,Response_Time_Out,Fail_Response_Percent,IP_Port,NPCI_Port, Last_SuccessRequest, Last_FailRequest, Last_SuccessResponse, Last_FailResponse) " +
                            " Select AppLications,a.InstanceId,a.Request_Total,a.Success Request_Success,a.Fail Request_Socket_Error,a.Fail_Request_Percent, " +
                            " b.Request_Total Response_Total,b.Success Response_Success,b.Fail Response_Time_Out ,b.Fail_Percent Fail_Response_Percent, " +
                            " IP_Port,NPCI_Port,a.Max_SuccessDateTime Last_SuccessRequest,a.Max_FailDateTime Last_FailRequest,b.Max_SuccessDateTime Last_SuccessResponse,b.Max_FailDateTime Last_FailResponse " +
                            " from #Final_AEPS_REQ_IN a left outer join " +
                            " #Final_AEPS_REP_IN b on a.InstanceId=b.InstanceId  " +
                            " left join #TxnDump c on a.InstanceId=c.InstanceId  and  c.AppLications='AEPS_IN'  " +

                            " Select * from #Final with(nolock) order by Applications,InstanceId";
                return strQuery.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                strQuery = null;
            }
        }
        #endregion

        #region AUA ASA Query
        public string AUAASAQueryData(string requesttype, string Instance_Id, string Authres_err, string Log_Date)
        {
            string query = string.Empty;

            if (requesttype == "Main")
            {
                query = " select Instance_ID, " +
                        " Case substring(Function_Code,1,1) when 1 then 'Auth Txn' when 2 then 'Ekyc Txn'  " +
                        " when 3 then 'BFD Txn' when 4 then 'OTP Txn'   end TXN_TYPE, " +
                        " count(case when AuthRes_err='000' and isnull(Mapper_Id,'')<>'' then	'1' end) as SuccessCount, " +
                        " count(case when AuthRes_err='000' and isnull(Mapper_Id,'')='' then	'1' end) as ADV_MapperId_Blank, " +
                        " count(case when AuthRes_err NOT in ('000', '010') then	'1' end) as FailureCount , " +
                        " count(case when AuthRes_err in ('010', 'R12') then	'1' end) as TimeoutCount, " +
                        " COUNT('1') as 'TotalCount',  " +
                        " Min(Log_Date) First_txn_time, " +
                        " Max(Log_Date) Last_txn_time " +
                        " into #tblInstance " +
                        " from [FINO_PB_AUA].dbo.T_RESPONSE_SUB_AUA with (nolock)  " +
                        " where  " +
                        " Log_Date >= @DateTime  " +
                        " and auth_vc<>'string' and  substring(Function_Code,1,1)<>''  " +
                        " group by auth_vc,substring(Function_Code,1,1),Instance_id  " +

                        " Select  AuthRes_err,  " +
                        " count( AuthRes_err) Error_cnt,tbl.Instance_ID,TXN_TYPE  " +
                        " into #AuthRes_Cnt  " +
                        " from [FINO_PB_AUA].dbo.T_RESPONSE_SUB_AUA rs with (nolock)   inner join  #tblInstance tbl   " +
                        " on rs.Instance_ID=tbl.Instance_ID and TXN_Type=(Case substring(Function_Code,1,1) when 1 then 'Auth Txn' when 2 then 'Ekyc Txn' 										when 3 then 'BFD Txn' when 4 then 'OTP Txn'   end)  " +
                        " where   " +
                        " Log_Date >= @DateTime  " +
                        " and auth_vc<>'string' and  substring(Function_Code,1,1)<>''  " +
                        " and AuthRes_err not in('010','R12','000')  " +
                        " group by tbl.Instance_ID,TXN_TYPE,AuthRes_err  " +

                        " Select max(Error_cnt) maxError_Cnt,Instance_ID,TXN_TYPE   " +
                        " into #MaxError  " +
                        " from #AuthRes_Cnt  " +
                        " group by Instance_ID,TXN_TYPE  " +

                        " Create table #temp_Error (Error_Code varchar(12),Error_Desc varchar(500))  " +

                        " insert into #temp_Error (Error_Code,Error_Desc) values   " +
                        " ('100', 'Pi (basic) attributes of demographic data did not match.'),  " +
                        " ('200', 'Pa (address) attributes of demographic data did not match.'),  " +
                        " ('300', 'Biometric data did not match.'),  " +
                        " ('310', 'Duplicate fingers used.'),  " +
                        " ('311', 'Duplicate Irises used.'),  " +
                        " ('312', 'FMR and FIR cannot be used in same transaction.'),  " +
                        " ('313', 'Single FIR record contains more than one finger.'),  " +
                        " ('314', 'Number of FMR/FIR should not exceed 10.'),  " +
                        " ('315', 'Number of IIR should not exceed 2.'),  " +
                        " ('316', 'Number of FID should not exceed 1.'),  " +
                        " ('317', 'Number of biometric modalities (Face/Finger/IRIS) should not exceed 2.'),  " +
                        " ('318', 'BFD transaction should not contain other modalities in input.'),  " +
                        " ('330', 'Biometrics locked by Aadhaar number holder.'),  " +
                        " ('331', 'Aadhaar locked by Aadhaar number holder for all authentications.'),  " +
                        " ('332', 'Aadhaar number usage is blocked by Aadhaar number holder.'),  " +
                        " ('400', 'Invalid OTP value.'),  " +
                        " ('402', 'txn value did not match with txn value of Request OTP API.'),  " +
                        " ('500', 'Invalid encryption of session key.'),  " +
                        " ('501', 'Invalid certificate identifier in ci attribute of Skey.'),  " +
                        " ('502', 'Invalid encryption of PID.'),  " +
                        " ('503', 'Invalid encryption of Hmac.'),  " +
                        " ('504', 'Session key re-initiation required due texpiry or key out of sync.'),  " +
                        " ('505', 'Synchronized Key usage not allowed for the AUA.'),  " +
                        " ('510', 'Invalid Auth XML format.'),  " +
                        " ('511', 'Invalid PID XML format.'),  " +
                        " ('512', 'Invalid consent value in rc attribute of Auth.'),  " +
                        " ('513', 'Invalid Protobuf Format'),  " +
                        " ('514', 'Invalid UID token in input.'),  " +
                        " ('515', 'Invalid VID Number in input.'),  " +
                        " ('517', 'Expired VID is used in input.'),  " +
                        " ('520', 'Invalid tid value.'),  " +
                        " ('521', 'Invalid dc code under Meta tag.'),  " +
                        " ('524', 'Invalid mi code under Meta tag.'),  " +
                        " ('527', 'Invalid mc code under Meta tag.'),  " +
                        " ('530', 'Invalid authenticator code.'),  " +
                        " ('540', 'Invalid Auth XML version.'),  " +
                        " ('541', 'Invalid PID XML version.'),  " +
                        " ('542', 'AUA not authorized for ASA. This error will be returned if AUA and ASA dnot have linking in the portal.'),  " +
                        " ('543', 'Sub-AUA not associated with AUA. This error will be returned if Sub-AUA specified in sa attribute is not added as Sub-AUA in portal.'),  " +
                        " ('550', 'Invalid Uses element attributes.'),  " +
                        " ('552', 'Invalid wadh element.'),  " +
                        " ('528', 'Device key rotation related issue.'),  " +
                        " ('553', 'Registered devices currently not supported. This feature is being implemented in a phased manner.'),  " +
                        " ('554', 'Public devices are not allowed to be used.'),  " +
                        " ('555', 'rdsId is invalid and not part of certification registry.'),  " +
                        " ('556', 'rdsVer is invalid and not part of certification registry.'),  " +
                        " ('557', 'dpId is invalid and not part of certification registry.'),  " +
                        " ('558', 'Invalid dih'),  " +
                        " ('559', 'Device Certificate has expired'),  " +
                        " ('560', 'DP Master Certificate has expired'),  " +
                        " ('561', 'Request expired (Pid->ts value is older than N hours where N is a configured threshold in authentication server).'),  " +
                        " ('562', 'Timestamp value is future time (value specified Pid->ts is ahead of authentication server time beyond acceptable threshold).'),  " +
                        " ('563', 'Duplicate request (this error occurs when exactly same authentication request was re-sent by AUA).'),  " +
                        " ('564', 'HMAC Validation failed.'),  " +
                        " ('565', 'AUA license has expired.'),  " +
                        " ('566', 'Invalid non-decryptable license key.'),  " +
                        " ('567', 'Invalid input (this error occurs when unsupported characters were found in Indian language values, lname or lav).'),  " +
                        " ('568', 'Unsupported Language.'),  " +
                        " ('569', 'Digital signature verification failed (means that authentication request XML was modified after it was signed).'),  " +
                        " ('570', 'Invalid key info in digital signature (this means that certificate used for signing the authentication request is not valid – it is either expired, or does not belong to the AUA or is not created by a well-known Certification Authority).'),  " +
                        " ('571', 'PIN requires reset.'),  " +
                        " ('572', 'Invalid biometric position.'),  " +
                        " ('573', 'Pi usage not allowed as per license.'),  " +
                        " ('574','Pa usage not allowed as per license.'),  " +
                        " ('575','Pfa usage not allowed as per license.'),  " +
                        " ('576','FMR usage not allowed as per license.'),  " +
                        " ('577','FIR usage not allowed as per license.'),  " +
                        " ('578', 'IIR usage not allowed as per license.'),  " +
                        " ('579', 'OTP usage not allowed as per license.'),  " +
                        " ('580','PIN usage not allowed as per license.'),  " +
                        " ('581', 'Fuzzy matching usage not allowed as per license.'),  " +
                        " ('582', 'Local language usage not allowed as per license.'),  " +
                        " ('586', 'FID usage not allowed as per license.'),  " +
                        " ('587', 'Name space not allowed.'),  " +
                        " ('588', 'Registered device not allowed as per license.'),  " +
                        " ('590', 'Public device not allowed as per license.'),  " +
                        " ('591', 'BFD usage is not allowed as per license.'),  " +
                        " ('710', 'Missing Pi data as specified in Uses.'),  " +
                        " ('720', 'Missing Pa data as specified in Uses.'),  " +
                        " ('721', 'Missing Pfa data as specified in Uses.'),  " +
                        " ('730', 'Missing PIN data as specified in Uses.'),  " +
                        " ('740', 'Missing OTP data as specified in Uses.'),  " +
                        " ('800', 'Invalid biometric data.'),  " +
                        " ('810', 'Missing biometric data as specified in Uses.'),  " +
                        " ('811', 'Missing biometric data in CIDR for the given Aadhaar Number/Virtual ID.'),  " +
                        " ('812', 'Aadhaar number holder has not done Best Finger Detection. Application should initiate BFD thelp Aadhaar number holder identify their best fingers.'),  " +
                        " ('820', 'Missing or empty value for bt attribute in Uses element.'),  " +
                        " ('821', 'Invalid value in the bt attribute of Uses element.'),  " +
                        " ('822', 'Invalid value in the ''bs'' attribute of Bio element within Pid.'),  " +
                        " ('901', 'No authentication data found in the request (this corresponds to a scenario wherein none of the auth data – Demo, Pv, or Bios – is present).'),  " +
                        " ('902', 'Invalid dob value in the Pi element (this corresponds ta scenarios wherein dob attribute is not of the format YYYY or YYYY-MM-DD, or the age is not in valid range).'),  " +
                        " ('910', 'Invalid mv value in the Pi element.'),  " +
                        " ('911', 'Invalid mv value in the Pfa element.'),  " +
                        " ('912', 'Invalid ms value.'),  " +
                        " ('913', 'Both Pa and Pfa are present in the authentication request (Pa and Pfa are mutually exclusive).'),  " +
                        " ('914', 'Face alone is of allowed as biometric modality. You should send face along with another biometric modality like Finger or IRIS or OTP.'),  " +
                        " ('915', 'Face auth is not allowed for this age of resident.'),  " +
                        " ('916', 'Invalid face Image format in input.'),  " +
                        " ('917', 'Invalid face capture type.'),  " +
                        " ('930','Technical error that are internal to authentication server.'),  " +
                        " ('931','Technical error that are internal to authentication server.'),  " +
                        " ('932','Technical error that are internal to authentication server.'),  " +
                        " ('933','Technical error that are internal to authentication server.'),  " +
                        " ('934','Technical error that are internal to authentication server.'),  " +
                        " ('935','Technical error that are internal to authentication server.'),  " +
                        " ('936','Technical error that are internal to authentication server.'),  " +
                        " ('937','Technical error that are internal to authentication server.'),  " +
                        " ('938','Technical error that are internal to authentication server.'),  " +
                        " ('939','Technical error that are internal to authentication server.'),  " +
                        " ('940', 'Unauthorized ASA channel.'),  " +
                        " ('941', 'Unspecified ASA channel.'),  " +
                        " ('950', 'OTP store related technical error.'),  " +
                        " ('951', 'Biometric lock related technical error.'),  " +
                        " ('980', 'Unsupported option.'),  " +
                        " ('995', 'Aadhaar suspended by competent authority.'),  " +
                        " ('996', 'Aadhaar cancelled (Aadhaar is not in authenticable status).'),  " +
                        " ('997', 'Aadhaar suspended (Aadhaar is not in authenticatable status).'),  " +
                        " ('998', 'Invalid Aadhaar Number/Virtual ID.'),  " +
                        " ('999', 'Unknown error.'),  " +
                        " ('K-100', 'Resident authentication failed'),  " +
                        " ('K-200', 'Resident data currently not available'),  " +
                        " ('K-540', 'Invalid KYC XML'),  " +
                        " ('K-541', 'Invalid e-KYC API version'),  " +
                        " ('K-542', 'Invalid resident consent (rc attribute in Kyc element)'),  " +
                        " ('K-544', 'Invalid resident auth type (ra attribute in Kyc element does not match what is in PID block)'),  " +
                        " ('K-545', 'Resident has opted-out of this service. This feature is not implemented currently.'),  " +
                        " ('K-546', 'Invalid value for pfr attribute'),  " +
                        " ('K-547', 'Invalid value for wadh attribute within PID block'),  " +
                        " ('K-550','Invalid Uses Attribute'),  " +
                        " ('K-551', 'Invalid Txn namespace'),  " +
                        " ('K-552', 'Invalid License key'),  " +
                        " ('K-569', 'Digital signature verification failed for e-KYC XML'),  " +
                        " ('K-570', 'Invalid key info in digital signature for e-KYC XML (it is either expired, or does not belong to the AUA or is not created by a well-known Certification Authority)'),  " +
                        " ('K-600', 'AUA is invalid or not an authorized KUA'),  " +
                        " ('K-601', 'ASA is invalid or not an authorized ASA'),  " +
                        " ('K-602', 'KUA encryption key not available'),  " +
                        " ('K-603', 'ASA encryption key not available'),  " +
                        " ('K-604', 'ASA Signature not allowed'),  " +
                        " ('K-605', 'Neither KUA key nor ASA encryption key are available'),  " +
                        " ('K-955', 'Technical Failure'),  " +
                        " ('K-999', 'Unknown error'),  " +
                        " ('110', 'Aadhaar number does not have email ID.'),  " +
                        " ('111', 'Aadhaar number does not have mobile number.'),  " +
                        " ('112', 'Aadhaar number does not have both email ID and mobile number.'),  " +
                        " ('113', 'Aadhaar Number doesn’t have verified email ID.'),  " +
                        " ('114', 'Aadhaar Number doesn’t have verified Mobile Number.'),  " +
                        " ('115', 'Aadhaar Number doesn’t have verified email and Mobile.'),  " +
                        " ('510', 'Invalid Otp XML format.'),  " +
                        " ('515', 'Invalid VID Number in input.'),  " +
                        " ('517', 'Expired VID is used in input.'),  " +
                        " ('520', 'Invalid device.'),  " +
                        " ('521', 'Invalid mobile number.'),  " +
                        " ('522', 'Invalid type attribute.'),  " +
                        " ('523', 'Invalid ts attribute. Either it is not in correct format or is older than 20 min.'),  " +
                        " ('530', 'Invalid AUA code.'),  " +
                        " ('540', 'Invalid OTP XML version.'),  " +
                        " ('542', 'AUA not authorized for ASA. This error will be returned if AUA and ASA dnot have linking in the portal.'),  " +
                        " ('543', 'Sub-AUA not associated with AUA. This error will be returned if Sub-AUA specified in sa attribute is not added as Sub-AUA in portal.'),  " +
                        " ('565', 'AUA License key has expired or is invalid.'),  " +
                        " ('566', 'ASA license key has expired or is invalid.'),  " +
                        " ('569', 'Digital signature verification failed.'),  " +
                        " ('570', 'Invalid key info in digital signature (this means that certificate used for signing the OTP request is not valid – it is either expired, or does not belong to the AUA or is not created by a CA).'),  " +
                        " ('940', 'Unauthorized ASA channel.'),  " +
                        " ('941', 'Unspecified ASA channel.'),  " +
                        " ('950', 'Could not generate and/or send OTP.'),  " +
                        " ('952', 'OTP Flooding error.'),  " +
                        " ('999', 'Unknown error.')  " +

                        " select Ins.Instance_Id,Ins.TXN_Type,  " +
                        " Ins.SuccessCount,Ins.FailureCount,Ins.TimeoutCount, ADV_MapperId_Blank,Ins.TotalCount,  " +
                        " Authres_err,  " +
                        " maxError_cnt,  " +
                        " maxError_cnt * 100 / Ins.TotalCount  as Fail_Request_Percent,  " +
                        " isnull(Error_Desc,'NA')Error_Desc , Ins.First_txn_time,Ins.Last_Txn_Time  " +
                        " from #tblInstance Ins inner join #AuthRes_Cnt ac on Ins.Instance_ID=ac.Instance_ID and ac.TXN_TYPE=Ins.TXN_TYPE  " +
                        " inner join  #MaxError me on ac.Instance_ID=me.Instance_ID and ac.TXN_TYPE=me.TXN_TYPE  " +
                        " left join #temp_Error err on err.Error_Code=Authres_err  " +
                        " where me.maxError_Cnt=ac.Error_cnt  " +

                        " drop table #tblInstance  " +
                        " drop table #AuthRes_Cnt  " +
                        " drop table #MaxError  " +
                        " drop table #temp_Error ";

            }
            else if (requesttype == "GetDetail")
            {
                query = " Select Top 5 AuthRes_code,AuthRes_txn,AuthRes_err,AuthRes_ts,Log_Date,SessionID,EkycAuthRes_err,Mapper_ID " +
                        " from T_Response_Sub_AUA with (nolock) " +
                        " where Instance_Id = '" + Instance_Id + "'  And Authres_err = '" + Authres_err + "' And Log_Date > '" + Log_Date + "' " +
                        " order by log_date desc ";
            }
            return query;
        }
        #endregion

        #region ResponseMessage From web.config
        public string GetCacheresponseMessage()
        {
            string ResponseMessageString = string.Empty;
            try
            {
                string RespMsgs = WebConfigurationManager.AppSettings["ResponseMessage1"];
                string[] RespMsgs1 = RespMsgs.Split(',');
                ResponseMessageString = JsonConvert.SerializeObject(RespMsgs1);
                return ResponseMessageString;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {

            }
        }
        #endregion

        #region CacheFailedResponseMessage Query
        public string CacheFailedResponseMessageQuery()
        {
            string strQuery = string.Empty;

            string RespMsgs = WebConfigurationManager.AppSettings["ResponseMessage"];


            strQuery = " select  Layer_Name, Response_Message,Count(Response_Message) As [Cnt], Node_IP_Address " +
                       " from PBLogs.dbo.Tbl_Corelation_Log x with (nolock  index=Ix_RequestOut)  " +
                       " left outer join temp_mstRequestType mstR on x.Method_ID = mstr.Method_ID and x.Service_ID = mstr.Service_ID  " +
                       " left outer join temp_mstLayer L on x.Layer_ID = L.Layer_ID " +
                       " where RequestOut>=@DateTime and Response_Flag='1' and Response_Message<>'' and Layer_Name='UI'  " +
                       " and Response_Message IN (" + RespMsgs + ") " +
                       " Group By  Layer_Name, Response_Message, Node_IP_Address " +
                       " Order by 1 ";

            return strQuery.ToString();

        }
        #endregion

        #region COLA Query
        public string COLAQuery(string cola, string fdate, string tdate)
        {
            string strQuery = string.Empty;

            if (cola == "1")
            {
                //Total  Transactions since Inception
                strQuery = " SELECT S.id , S.Name TxnStatus, COUNT (TransactionStatusId) FullTxnCount " +
                           " FROM TransactionStatuses S with(nolock) " +
                           " LEFT OUTER JOIN dbo.TransactionRequests R with(nolock) ON S.Id = R.TransactionStatusId  " +
                           " GROUP BY S.Id, TransactionStatusId, S.Name ";
            }
            else if (cola == "2")
            {
                //Todays Transactions 
                strQuery = " SELECT S.id , S.Name TxnStatus, COUNT (TransactionStatusId) TodaysTxnCount " +
                           " FROM TransactionStatuses S with(nolock) " +
                           " LEFT OUTER JOIN dbo.TransactionRequests R with(nolock) ON S.Id = R.TransactionStatusId AND CONVERT(DATE,RequestedDateTime) = CONVERT (DATE, Getdate()) " +
                           " GROUP BY S.Id, TransactionStatusId, S.Name ";
            }
            else
            {
                //Date Wise Transactions 
                strQuery = " SELECT S.id , S.Name TxnStatus, COUNT (TransactionStatusId) TxnCount,case " +
                           " When R.TransactionTypeId = 1 Then 'CASHD' When R.TransactionTypeId = 2 Then 'CASHW' ELSE '0' END AS [TransactionType] " +
                           " ,Sum(RequestedAmount) AS [Amount]" +
                           " FROM TransactionStatuses S with(nolock) " +
                           " LEFT OUTER JOIN dbo.TransactionRequests R with(nolock) ON S.Id = R.TransactionStatusId AND CONVERT(DATE,RequestedDateTime) Between '" + fdate + "' And '" + tdate + "' " +
                           " GROUP BY S.Id, TransactionStatusId, S.Name,R.TransactionTypeId ";
            }
            return strQuery.ToString();

        }
        #endregion

    }

    #region DashBoard Model Class
    [ServiceContract]
    public interface IDashboardRequest
    {
        int DashboardType { get; set; }
        string TransactionType { get; set; }
        string DateTime { get; set; }
    }
    [DataContract]
    public class DashboardRequest : IDashboardRequest
    {
        [DataMember(IsRequired = false, Name = "DashboardType", Order = 0)]
        public int DashboardType { get; set; }

        [DataMember(IsRequired = false, Name = "TransactionType", Order = 1)]
        public string TransactionType { get; set; }

        [DataMember(IsRequired = false, Name = "DateTime", Order = 2)]
        public string DateTime { get; set; }

    }

    [ServiceContract]
    public interface IDashboardResponse
    {
        int DashboardType { get; set; }
        dynamic DashboardGrid { get; set; }
    }

    [DataContract]
    public class DashboardResponse : IDashboardResponse
    {
        [DataMember(IsRequired = false, Name = "DashboardType", Order = 0)]
        public int DashboardType { get; set; }

        [DataMember(IsRequired = false, Name = "DashboardGrid", Order = 1)]
        public dynamic DashboardGrid { get; set; }
    }

    #endregion

    #region ESB Model Class
    //ESB Model
    [ServiceContract]
    public interface IESBRequest
    {
        int ESBTab { get; set; }
        string Sdattime { get; set; }
        string Edattime { get; set; }
        string TransactionName { get; set; }
        string TType { get; set; }
        string DateTime { get; set; }
        string appid { get; set; }
    }

    [DataContract]
    public class ESBRequest : IESBRequest
    {
        [DataMember(IsRequired = false, Name = "ESBTab", Order = 0)]
        public int ESBTab { get; set; }

        [DataMember(IsRequired = false, Name = "Sdattime", Order = 1)]
        public string Sdattime { get; set; }

        [DataMember(IsRequired = false, Name = "Edattime", Order = 2)]
        public string Edattime { get; set; }

        [DataMember(IsRequired = false, Name = "TransactionName", Order = 3)]
        public string TransactionName { get; set; }

        [DataMember(IsRequired = false, Name = "TType", Order = 4)]
        public string TType { get; set; }

        [DataMember(IsRequired = false, Name = "DateTime", Order = 5)]
        public string DateTime { get; set; }

        [DataMember(IsRequired = false, Name = "appid", Order = 6)]
        public string appid { get; set; }
    }

    [ServiceContract]
    public interface IESBResponse
    {
        dynamic ESBGrid { get; set; }
    }

    [DataContract]
    public class ESBResponse : IESBResponse
    {

        [DataMember(IsRequired = false, Name = "ESBGrid", Order = 0)]
        public dynamic ESBGrid { get; set; }
    }
    #endregion

    #region AEPS Model Class
    //ESB Model
    [ServiceContract]
    public interface IAEPSRequest
    {
        string DateTime { get; set; }
    }

    [DataContract]
    public class AEPSRequest : IAEPSRequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }
    }

    [ServiceContract]
    public interface IAEPSResponse
    {
        dynamic AEPSGrid { get; set; }
    }

    [DataContract]
    public class AEPSResponse : IAEPSResponse
    {

        [DataMember(IsRequired = false, Name = "AEPSGrid", Order = 0)]
        public dynamic AEPSGrid { get; set; }
    }
    #endregion

    #region Latency Model
    [ServiceContract]
    public interface ILatencyRequest
    {
        string LatencyType { get; set; }
        string DateTime { get; set; }

    }

    [DataContract]
    public class LatencyRequest : ILatencyRequest
    {
        [DataMember(IsRequired = false, Name = "LatencyType", Order = 0)]
        public string LatencyType { get; set; }

        [DataMember(IsRequired = false, Name = "DateTime", Order = 1)]
        public string DateTime { get; set; }
    }

    [ServiceContract]
    public interface ILatencyResponse
    {
        dynamic LatencyGrid { get; set; }
    }

    [DataContract]
    public class LatencyResponse : ILatencyResponse
    {

        [DataMember(IsRequired = false, Name = "LatencyGrid", Order = 0)]
        public dynamic LatencyGrid { get; set; }
    }

    #endregion

    #region Merchant Transcation Model Class
    //ESB Model
    [ServiceContract]
    public interface IMerchantRequest
    {
        int MerchantTab { get; set; }
        string Sdattime { get; set; }
        string Edattime { get; set; }
        string UserID { get; set; }
    }

    [DataContract]
    public class MerchantRequest : IMerchantRequest
    {
        [DataMember(IsRequired = false, Name = "MerchantTab", Order = 0)]
        public int MerchantTab { get; set; }

        [DataMember(IsRequired = false, Name = "Sdattime", Order = 1)]
        public string Sdattime { get; set; }

        [DataMember(IsRequired = false, Name = "Edattime", Order = 2)]
        public string Edattime { get; set; }

        [DataMember(IsRequired = false, Name = "UserID", Order = 3)]
        public string UserID { get; set; }

    }

    [ServiceContract]
    public interface IMerchantResponse
    {
        dynamic MerchantGrid { get; set; }
    }

    [DataContract]
    public class MerchantResponse : IMerchantResponse
    {

        [DataMember(IsRequired = false, Name = "MerchantGrid", Order = 0)]
        public dynamic MerchantGrid { get; set; }
    }
    #endregion

    #region ESB Latency Model
    [ServiceContract]
    public interface IESBLatencyRequest
    {
        string LatencyType { get; set; }
        string Sdattime { get; set; }
        string Edattime { get; set; }
    }

    [DataContract]
    public class ESBLatencyRequest : IESBLatencyRequest
    {
        [DataMember(IsRequired = false, Name = "LatencyType", Order = 0)]
        public string LatencyType { get; set; }

        [DataMember(IsRequired = false, Name = "Sdattime", Order = 1)]
        public string Sdattime { get; set; }

        [DataMember(IsRequired = false, Name = "Edattime", Order = 2)]
        public string Edattime { get; set; }

    }

    [ServiceContract]
    public interface IESBLatencyResponse
    {
        dynamic ESBLatencyGrid { get; set; }
    }

    [DataContract]
    public class ESBLatencyResponse : IESBLatencyResponse
    {

        [DataMember(IsRequired = false, Name = "ESBLatencyGrid", Order = 0)]
        public dynamic ESBLatencyGrid { get; set; }
    }
    #endregion

    #region Logs Model

    [ServiceContract]
    public interface ILogsRequest
    {
        string RequestId { get; set; }

        string tblName { get; set; }
    }

    [DataContract]
    public class LogsRequest : ILogsRequest
    {
        [DataMember(IsRequired = false, Name = "RequestId", Order = 0)]
        public string RequestId { get; set; }

        [DataMember(IsRequired = false, Name = "tblName", Order = 0)]
        public string tblName { get; set; }

    }

    [ServiceContract]
    public interface ILogsResponse
    {
        dynamic LogResponse { get; set; }
    }

    [DataContract]
    public class LogsResponse : ILogsResponse
    {
        [DataMember(IsRequired = false, Name = "LogResponse", Order = 0)]
        public dynamic LogResponse { get; set; }

    }

    #endregion

    #region VS Model
    [ServiceContract]
    public interface IVSRequest
    {
        string DateTime { get; set; }

    }

    [DataContract]
    public class VSRequest : IVSRequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }

    }

    [ServiceContract]
    public interface IVSResponse
    {
        dynamic VSResponseData { get; set; }
    }

    [DataContract]
    public class VSResponse : IVSResponse
    {
        [DataMember(IsRequired = false, Name = "VSResponseData", Order = 0)]
        public dynamic VSResponseData { get; set; }

    }
    #endregion

    #region IMPS Model
    [ServiceContract]
    public interface IIMPSRequest
    {
        string DateTime { get; set; }

    }

    [DataContract]
    public class IMPSRequest : IIMPSRequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }

    }

    [ServiceContract]
    public interface IIMPSResponse
    {
        dynamic IMPSResponseData { get; set; }
    }

    [DataContract]
    public class IMPSResponse : IIMPSResponse
    {
        [DataMember(IsRequired = false, Name = "IMPSResponseData", Order = 0)]
        public dynamic IMPSResponseData { get; set; }

    }
    #endregion

    #region DMS Model
    [ServiceContract]
    public interface IDMSRequest
    {
        string DateTime { get; set; }
        string TabName { get; set; }

    }

    [DataContract]
    public class DMSRequest : IDMSRequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }

        [DataMember(IsRequired = false, Name = "TabName", Order = 1)]
        public string TabName { get; set; }

    }

    [ServiceContract]
    public interface IDMSResponse
    {
        dynamic DMSResponseData { get; set; }
    }

    [DataContract]
    public class DMSResponse : IDMSResponse
    {
        [DataMember(IsRequired = false, Name = "DMSResponseData", Order = 0)]
        public dynamic DMSResponseData { get; set; }

    }
    #endregion

    #region ZONE Model
    [ServiceContract]
    public interface IZoneRequest
    {
        string LatencyType { get; set; }
        string DateTime { get; set; }
        string MetID { get; set; }
        string SerID { get; set; }

    }

    [DataContract]
    public class ZoneRequest : IZoneRequest
    {
        [DataMember(IsRequired = false, Name = "LatencyType", Order = 0)]
        public string LatencyType { get; set; }

        [DataMember(IsRequired = false, Name = "DateTime", Order = 1)]
        public string DateTime { get; set; }

        [DataMember(IsRequired = false, Name = "MetID", Order = 2)]
        public string MetID { get; set; }

        [DataMember(IsRequired = false, Name = "SerID", Order = 1)]
        public string SerID { get; set; }
    }

    [ServiceContract]
    public interface IZoneResponse
    {
        dynamic ZoneGrid { get; set; }
    }

    [DataContract]
    public class ZoneResponse : IZoneResponse
    {

        [DataMember(IsRequired = false, Name = "ZoneGrid", Order = 0)]
        public dynamic ZoneGrid { get; set; }
    }

    #endregion

    #region RequestType Model
    [ServiceContract]
    public interface IRequestType_Request
    {
        string Request_Type { get; set; }
    }

    [DataContract]
    public class RequestType_Request : IRequestType_Request
    {
        [DataMember(IsRequired = false, Name = "Request_Type", Order = 0)]
        public string Request_Type { get; set; }

    }

    [ServiceContract]
    public interface IRequsetType_Response
    {
        dynamic ServiceAndMethodIDResponse { get; set; }
    }

    [DataContract]
    public class RequsetType_Response : IRequsetType_Response
    {
        [DataMember(IsRequired = false, Name = "ServiceAndMethodID", Order = 0)]
        public dynamic ServiceAndMethodIDResponse { get; set; }

    }
    #endregion

    #region EKO Model
    [ServiceContract]
    public interface IEKORequest
    {
        string DateTime { get; set; }
        string EKOStatus { get; set; }
        string TxnStatus { get; set; }
        string ClientName { get; set; }
        string EndDateTime { get; set; }
    }
    [DataContract]
    public class EKORequest : IEKORequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }

        [DataMember(IsRequired = false, Name = "EKOStatus", Order = 1)]
        public string EKOStatus { get; set; }

        [DataMember(IsRequired = false, Name = "TxnStatus", Order = 2)]
        public string TxnStatus { get; set; }

        [DataMember(IsRequired = false, Name = "ClientName", Order = 3)]
        public string ClientName { get; set; }

        [DataMember(IsRequired = false, Name = "EndDateTime", Order = 4)]
        public string EndDateTime { get; set; }

    }

    [ServiceContract]
    public interface IEKOResponse
    {
        dynamic EKODashboardGrid { get; set; }
    }

    [DataContract]
    public class EKOResponse : IEKOResponse
    {
        [DataMember(IsRequired = false, Name = "EKODashboardGrid", Order = 0)]
        public dynamic EKODashboardGrid { get; set; }
    }
    #endregion

    #region AEPS API Model
    [ServiceContract]
    public interface IAEPSAPIRequest
    {
        string DateTime { get; set; }
        string AEPSAPIStatus { get; set; }
        string RequestFlag { get; set; }
        string ClientName { get; set; }
        string EndDateTime { get; set; }
    }
    [DataContract]
    public class AEPSAPIRequest : IAEPSAPIRequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }

        [DataMember(IsRequired = false, Name = "AEPSAPIStatus", Order = 1)]
        public string AEPSAPIStatus { get; set; }

        [DataMember(IsRequired = false, Name = "RequestFlag", Order = 2)]
        public string RequestFlag { get; set; }

        [DataMember(IsRequired = false, Name = "ClientName", Order = 3)]
        public string ClientName { get; set; }

        [DataMember(IsRequired = false, Name = "EndDateTime", Order = 4)]
        public string EndDateTime { get; set; }

        [DataMember(IsRequired = false, Name = "ProductCode", Order = 5)]
        public string ProductCode { get; set; }

    }

    [ServiceContract]
    public interface IAEPSAPIResponse
    {
        dynamic AEPSAPIDashboardGrid { get; set; }
    }

    [DataContract]
    public class AEPSAPIResponse : IAEPSAPIResponse
    {
        [DataMember(IsRequired = false, Name = "AEPSAPIDashboardGrid", Order = 0)]
        public dynamic AEPSAPIDashboardGrid { get; set; }
    }
    #endregion

    #region Refire Model
    [ServiceContract]
    public interface IRefireRequest
    {
        string fromdaterefire { get; set; }
        string todaterefire { get; set; }
        int refire { get; set; }
    }
    [DataContract]
    public class RefireRequest : IRefireRequest
    {
        [DataMember(IsRequired = true, Name = "fromdaterefire", Order = 0)]
        public string fromdaterefire { get; set; }

        [DataMember(IsRequired = true, Name = "todaterefire", Order = 1)]
        public string todaterefire { get; set; }

        [DataMember(IsRequired = true, Name = "refire", Order = 2)]
        public int refire { get; set; }
    }

    [ServiceContract]
    public interface IRefireResponse
    {
        dynamic dtrefireresponse { get; set; }
    }

    [DataContract]
    public class RefireResponse : IRefireResponse
    {
        [DataMember(IsRequired = false, Name = "dtrefireresponse", Order = 0)]
        public dynamic dtrefireresponse { get; set; }

    }
    #endregion

    #region Suryoday Model
    [ServiceContract]
    public interface ISuryodayRequest
    {
        string DateTime { get; set; }
    }
    [DataContract]
    public class SuryodayRequest : ISuryodayRequest
    {
        [DataMember(IsRequired = true, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }
        [DataMember(IsRequired = false, Name = "EsbTab", Order = 1)]
        public string EsbTab { get; set; }

    }

    [ServiceContract]
    public interface ISuryodayResponse
    {
        dynamic dtsuryodayresp { get; set; }
    }

    [DataContract]
    public class SuryodayResponse : ISuryodayResponse
    {
        [DataMember(IsRequired = false, Name = "dtsuryodayresp", Order = 0)]
        public dynamic dtsuryodayresp { get; set; }

    }
    #endregion

    #region AccountOpening Model
    [ServiceContract]
    public interface IAccountOpeningRequest
    {
        string accdatetime { get; set; }
    }

    [DataContract]
    public class AccountOpeningRequest : IAccountOpeningRequest
    {
        [DataMember(IsRequired = true, Name = "accdatetime", Order = 0)]
        public string accdatetime { get; set; }
    }

    [ServiceContract]
    public interface IAccountOpeningResponse
    {
        dynamic accopeningresponse { get; set; }
    }

    [DataContract]
    public class AccountOpeningResponse : IAccountOpeningResponse
    {
        [DataMember(IsRequired = false, Name = "accopeningresponse", Order = 0)]
        public dynamic accopeningresponse { get; set; }
    }

    #endregion

    #region CityCash Model
    [ServiceContract]
    public interface ICityCashRequest
    {
        string DateTime { get; set; }
        string CityCashStatus { get; set; }
        string TransactionType { get; set; }
        string EndDateTime { get; set; }
        int ProjectID { get; set; }
    }
    [DataContract]
    public class CityCashRequest : ICityCashRequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }

        [DataMember(IsRequired = false, Name = "CityCashStatus", Order = 1)]
        public string CityCashStatus { get; set; }

        [DataMember(IsRequired = false, Name = "TransactionType", Order = 2)]
        public string TransactionType { get; set; }

        [DataMember(IsRequired = false, Name = "EndDateTime", Order = 3)]
        public string EndDateTime { get; set; }

        [DataMember(IsRequired = false, Name = "ProjectID", Order = 4)]
        public int ProjectID { get; set; }

        [DataMember(IsRequired = false, Name = "MobileNumber", Order = 5)]
        public string MobileNumber { get; set; }

    }

    [ServiceContract]
    public interface ICityCashResponse
    {
        dynamic CityCashDashboardGrid { get; set; }
    }

    [DataContract]
    public class CityCashResponse : ICityCashResponse
    {
        [DataMember(IsRequired = false, Name = "CityCashDashboardGrid", Order = 0)]
        public dynamic CityCashDashboardGrid { get; set; }
    }
    #endregion

    #region DMS Account Opening
    [ServiceContract]
    public interface IDMSAccountOpeningRequest
    {
        string DMSDateTime { get; set; }
        string Tab { get; set; }
    }
    [DataContract]
    public class DMSAccountOpeningRequest : IDMSAccountOpeningRequest
    {
        [DataMember(IsRequired = false, Name = "DMSDateTime", Order = 0)]
        public string DMSDateTime { get; set; }

        [DataMember(IsRequired = false, Name = "Tab", Order = 1)]
        public string Tab { get; set; }

    }

    [ServiceContract]
    public interface IDMSAccountOpeningResponse
    {
        dynamic DMSDashboardGrid { get; set; }
    }

    [DataContract]
    public class DMSAccountOpeningResponse : IDMSAccountOpeningResponse
    {
        [DataMember(IsRequired = false, Name = "DMSDashboardGrid", Order = 0)]
        public dynamic DMSDashboardGrid { get; set; }
    }
    #endregion

    #region Passthrough Model Class
    [ServiceContract]
    public interface IPassthroughRequest
    {
        string DateTime { get; set; }
    }
    [DataContract]
    public class PassthroughRequest : IPassthroughRequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }
    }

    [ServiceContract]
    public interface IPassthroughResponse
    {
        dynamic PassthroughGrid { get; set; }
    }

    [DataContract]
    public class PassthroughResponse : IPassthroughResponse
    {
        [DataMember(IsRequired = false, Name = "PassthroughGrid", Order = 0)]
        public dynamic PassthroughGrid { get; set; }
    }

    #endregion

    #region AUA ASA Model Class
    //ESB Model
    [ServiceContract]
    public interface IAUAASARequest
    {
        string DateTime { get; set; }
        string RequestType { get; set; }
        string Instance_Id { get; set; }
        string Authres_err { get; set; }
        string Log_Date { get; set; }
    }

    [DataContract]
    public class AUAASARequest : IAUAASARequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }

        [DataMember(IsRequired = false, Name = "RequestType", Order = 1)]
        public string RequestType { get; set; }

        [DataMember(IsRequired = false, Name = "Instance_Id", Order = 2)]
        public string Instance_Id { get; set; }

        [DataMember(IsRequired = false, Name = "Authres_err", Order = 3)]
        public string Authres_err { get; set; }

        [DataMember(IsRequired = false, Name = "Log_Date", Order = 4)]
        public string Log_Date { get; set; }
    }

    [ServiceContract]
    public interface IAUAASAResponse
    {
        dynamic AUAASAGrid { get; set; }
    }

    [DataContract]
    public class AUAASAResponse : IAUAASAResponse
    {

        [DataMember(IsRequired = false, Name = "AUAASAGrid", Order = 0)]
        public dynamic AUAASAGrid { get; set; }
    }
    #endregion

    #region Cache Response Model Class
    //ESB Model
    [ServiceContract]
    public interface ICacheRequest
    {
        string ResponseMessage { get; set; }
        string DateTime { get; set; }
        bool WebConfig { get; set; }
    }

    [DataContract]
    public class CacheRequest : ICacheRequest
    {
        [DataMember(IsRequired = false, Name = "ResponseMessage", Order = 0)]
        public string ResponseMessage { get; set; }

        [DataMember(IsRequired = false, Name = "DateTime", Order = 1)]
        public string DateTime { get; set; }

        [DataMember(IsRequired = false, Name = "WebConfig", Order = 2)]
        public bool WebConfig { get; set; }
    }

    [ServiceContract]
    public interface ICacheResponse
    {
        dynamic CacheGrid { get; set; }
    }

    [DataContract]
    public class CacheResponse : ICacheResponse
    {

        [DataMember(IsRequired = false, Name = "CacheGrid", Order = 0)]
        public dynamic CacheGrid { get; set; }
    }
    #endregion


    #region CityCash Model
    [ServiceContract]
    public interface ICMSRequest
    {
        string DateTime { get; set; }
        string EndDateTime { get; set; }
        string CMSStatus { get; set; }
        string PartnerID { get; set; }
        string productcode { get; set; }
    }

    [DataContract]
    public class CMSRequest : ICMSRequest
    {
        [DataMember(IsRequired = false, Name = "DateTime", Order = 0)]
        public string DateTime { get; set; }

        [DataMember(IsRequired = false, Name = "EndDateTime", Order = 1)]
        public string EndDateTime { get; set; }

        [DataMember(IsRequired = false, Name = "CMSStatus", Order = 2)]
        public string CMSStatus { get; set; }

        [DataMember(IsRequired = false, Name = "PartnerID", Order = 3)]
        public string PartnerID { get; set; }

        [DataMember(IsRequired = false, Name = "productcode", Order = 4)]
        public string productcode { get; set; }

    }

    [ServiceContract]
    public interface ICMSResponse
    {
        dynamic CMSDashboardGrid { get; set; }
    }

    [DataContract]
    public class CMSResponse : ICMSResponse
    {
        [DataMember(IsRequired = false, Name = "CMSDashboardGrid", Order = 0)]
        public dynamic CMSDashboardGrid { get; set; }
    }
    #endregion

    #region Cash Bazaar Model
    [ServiceContract]
    public interface ICashBazaarRequest
    {
        string COLAID { get; set; }
        string FromDate { get; set; }
        string ToDate { get; set; }
    }

    [DataContract]
    public class CashBazaarRequest : ICashBazaarRequest
    {
        [DataMember(IsRequired = false, Name = "COLAID", Order = 0)]
        public string COLAID { get; set; }

        [DataMember(IsRequired = false, Name = "FromDate", Order = 1)]
        public string FromDate { get; set; }

        [DataMember(IsRequired = false, Name = "ToDate", Order = 2)]
        public string ToDate { get; set; }
    }

    [ServiceContract]
    public interface ICashBazaarResponse
    {
        dynamic CashBazaarGrid { get; set; }
    }

    [DataContract]
    public class CashBazaarResponse : ICashBazaarResponse
    {
        [DataMember(IsRequired = false, Name = "CashBazaarGrid", Order = 0)]
        public dynamic CashBazaarGrid { get; set; }
    }
    #endregion

    public static class GlobalConnectionString
    {
        public static string strFinoPaymentConfigurationConnString;
        public static string strFinoPaymentFPRepositoryConnString;
        public static string strFinoPaymentLogConnString;
        public static string strFinoPaymentMastersConnString;
        public static string strFinoPaymentTransactionInfoConnString;
    }

    public class clsAESEncrytDecrypt
    {
        public string GetDecryptedConnStringByName(string ConnStringName)
        {

            string strGetConnString = string.Empty;
            string strGetConnEncryptDecryptKey = string.Empty;
            clsAESEncrytDecrypt objencript = null;
            try
            {
                strGetConnEncryptDecryptKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings[param.MasterConnStringKey.ToString()].ToString().Trim());
                objencript = new clsAESEncrytDecrypt();
                strGetConnString = Convert.ToString(objencript.DecryptConnectionString(ConnStringName, strGetConnEncryptDecryptKey));

                return strGetConnString;
            }
            catch (Exception)
            {
                return strGetConnString;
            }
            finally
            {
                if (strGetConnString != null) strGetConnString = null;
                if (strGetConnEncryptDecryptKey != null) strGetConnEncryptDecryptKey = null;
                if (objencript != null) objencript = null;
            }
        }
        public string DecryptConnectionString(string cipherText, string keyval)
        {
            string EncryptionKey = keyval;
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
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
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public enum param
        {
            MasterConnStringKey,
            FinoPaymentLogConnString,
            FinoPaymentConfigurationConnString,
            FinoPaymentMastersConnString,
        }
    }
}