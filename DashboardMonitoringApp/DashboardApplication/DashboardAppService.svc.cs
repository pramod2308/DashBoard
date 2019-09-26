using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Web.Hosting;
using System.ServiceModel.Web;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json;

namespace DashboardApplication
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DashboardAppService" in code, svc and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DashboardAppService : IDashboardAppService
    {
        DashboardResponse objDashboardResponse = new DashboardResponse();
        ESBResponse objESBResponse = new ESBResponse();
        AEPSResponse objAEPSresponse = new AEPSResponse();
        LatencyResponse objLatencyResponse = new LatencyResponse();
        MerchantResponse objMerchantResponse = new MerchantResponse();
        ESBLatencyResponse objESBLatencyResponse = new ESBLatencyResponse();
        LogsResponse objLogs = new LogsResponse();
        VSResponse objVSResponse = new VSResponse();
        IMPSResponse objIMPSResponse = new IMPSResponse();
        DMSResponse objDMSResponse = new DMSResponse();
        ZoneResponse objZoneResponse = new ZoneResponse();
        RequsetType_Response objRequestTypeResponse = new RequsetType_Response();
        EKOResponse objEKOResponse = new EKOResponse();
        AEPSAPIResponse objAEPSAPIResponse = new AEPSAPIResponse();
        RefireResponse objrefire = new RefireResponse();
        SuryodayResponse objsurresp = new SuryodayResponse();
        AccountOpeningResponse objaccresp = new AccountOpeningResponse();
        CityCashResponse objcitycash = new CityCashResponse();
        DMSAccountOpeningResponse objdmsaccopening = new DMSAccountOpeningResponse();
        PassthroughResponse objPassthrough = new PassthroughResponse();
        AUAASAResponse objAUAASAresp = new AUAASAResponse();
        CacheResponse objcacheresp = new CacheResponse();
        CMSResponse objcmsResponse = new CMSResponse();
        CashBazaarResponse objColaResp = new CashBazaarResponse();

        clsDashboard objclsDashboard = new clsDashboard();
        public DashboardResponse ProcessDashboardRequest(DashboardRequest objRequest)
        {
            try
            {
                objDashboardResponse.DashboardType = objRequest.DashboardType;
                objDashboardResponse.DashboardGrid = objclsDashboard.GetDashboard(objRequest.DashboardType, objRequest.DateTime);
                return objDashboardResponse;
            }
            catch (Exception)
            {
                return objDashboardResponse;
            }
            finally
            {
                objDashboardResponse = null;
                objclsDashboard = null;
            }
        }

        public ESBResponse ProcessESBRequest(ESBRequest objRequest)
        {
            try
            {
                objESBResponse.ESBGrid = objclsDashboard.GetESBData(objRequest.ESBTab, objRequest.Sdattime, objRequest.Edattime, objRequest.TransactionName, objRequest.TType, objRequest.appid);
                return objESBResponse;
            }
            catch (Exception)
            {
                return objESBResponse;
            }
            finally
            {
                objESBResponse = null;
                objclsDashboard = null;
            }
        }

        public ESBResponse ProcessFailedAwaiting(ESBRequest objRequest)
        {
            try
            {
                objESBResponse.ESBGrid = objclsDashboard.GetFailedAwaitingData(objRequest.ESBTab, objRequest.DateTime, objRequest.TransactionName, objRequest.TType);
                return objESBResponse;
            }
            catch (Exception)
            {
                return objESBResponse;
            }
            finally
            {
                objESBResponse = null;
                objclsDashboard = null;
            }
        }

        public AEPSResponse ProcessAEPSFailed(AEPSRequest objRequest)
        {
            try
            {
                objAEPSresponse.AEPSGrid = objclsDashboard.GetAEPSFailedData(objRequest.DateTime);
                return objAEPSresponse;
            }
            catch (Exception)
            {
                return objAEPSresponse;
            }
            finally
            {
                objAEPSresponse = null;
                objclsDashboard = null;
            }
        }

        public LatencyResponse ProcessLatency(LatencyRequest objRequest)
        {
            try
            {
                if (objRequest.LatencyType == null || objRequest.LatencyType == "DBInsertion")
                {
                    objLatencyResponse.LatencyGrid = objclsDashboard.GetESB_Latency_DBInsertion(objRequest.DateTime);
                    return objLatencyResponse;
                }
                else
                {
                    objLatencyResponse.LatencyGrid = objclsDashboard.GetLatencyData(objRequest.LatencyType, objRequest.DateTime);
                    return objLatencyResponse;
                }
            }
            catch (Exception)
            {
                return objLatencyResponse;
            }
            finally
            {
                objLatencyResponse = null;
                objclsDashboard = null;
            }
        }

        public ESBLatencyResponse ProcessESBLatency(ESBLatencyRequest objRequest)
        {
            try
            {
                if (objRequest.LatencyType == null || objRequest.LatencyType == "ESBLatencyReport")
                {
                    objESBLatencyResponse.ESBLatencyGrid = objclsDashboard.GetESB_Latency_Report(objRequest.Sdattime, objRequest.Edattime);
                    return objESBLatencyResponse;
                }
                return objESBLatencyResponse;
            }
            catch (Exception)
            {
                return objESBLatencyResponse;
            }
            finally
            {
                objLatencyResponse = null;
                objclsDashboard = null;
            }
        }

        public MerchantResponse ProcessMerchantTransaction(MerchantRequest objRequest)
        {
            try
            {
                objMerchantResponse.MerchantGrid = objclsDashboard.GetMerchantData(objRequest.MerchantTab, objRequest.Sdattime, objRequest.Edattime, objRequest.UserID);
                return objMerchantResponse;
            }
            catch (Exception)
            {
                return objMerchantResponse;
            }
            finally
            {
                objMerchantResponse = null;
                objclsDashboard = null;
            }
        }

        public LatencyResponse ProcessESBNODESERVICELatency(LatencyRequest objRequest)
        {
            try
            {
                {
                    objLatencyResponse.LatencyGrid = objclsDashboard.GetESBNODESERVICE_Latency_Report(objRequest.LatencyType, objRequest.DateTime);
                    return objLatencyResponse;
                }
            }
            catch (Exception)
            {
                return objLatencyResponse;
            }
            finally
            {
                objLatencyResponse = null;
                objclsDashboard = null;
            }
        }

        public LatencyResponse ProcessHA_ESBLatency(LatencyRequest objRequest)
        {
            try
            {
                {
                    objLatencyResponse.LatencyGrid = objclsDashboard.Get_HA_ESBLatencyReport(objRequest.DateTime);
                    return objLatencyResponse;
                }
            }
            catch (Exception)
            {
                return objLatencyResponse;
            }
            finally
            {
                objLatencyResponse = null;
                objclsDashboard = null;
            }
        }

        public LogsResponse ProcessGetLogs(LogsRequest objRequest)
        {
            try
            {
                {
                    objLogs.LogResponse = objclsDashboard.GetLogsDashboard(objRequest.tblName, objRequest.RequestId);
                    return objLogs;
                }
            }
            catch (Exception)
            {
                return objLogs;
            }
            finally
            {
                objLogs = null;
                objclsDashboard = null;
            }
        }

        public IMPSResponse ProcessIMPSData(IMPSRequest objRequest)
        {
            try
            {
                {
                    objIMPSResponse.IMPSResponseData = objclsDashboard.GetIMPSDashboard(objRequest.DateTime);
                    return objIMPSResponse;
                }
            }
            catch (Exception)
            {
                return objIMPSResponse;
            }
            finally
            {
                objIMPSResponse = null;
                objclsDashboard = null;
            }
        }

        public DMSResponse ProcessDMSData(DMSRequest objRequest)
        {
            try
            {
                {
                    objDMSResponse.DMSResponseData = objclsDashboard.GetDMSData(objRequest.DateTime);
                    return objDMSResponse;
                }
            }
            catch (Exception)
            {
                return objDMSResponse;
            }
            finally
            {
                objDMSResponse = null;
                objclsDashboard = null;
            }
        }

        public ZoneResponse ProcessESB(ZoneRequest objRequest)
        {
            try
            {
                {
                    objZoneResponse.ZoneGrid = objclsDashboard.GetESBLatencyData(objRequest.DateTime, objRequest.LatencyType, objRequest.SerID, objRequest.MetID);
                    return objZoneResponse;
                }
            }
            catch (Exception)
            {
                return objZoneResponse;
            }
            finally
            {
                objZoneResponse = null;
                objclsDashboard = null;
            }
        }

        public RequsetType_Response ProcessToGetRequestType(RequestType_Request objRequest)
        {
            try
            {
                {
                    objRequestTypeResponse.ServiceAndMethodIDResponse = objclsDashboard.GetRequestTypeData(objRequest.Request_Type);
                    return objRequestTypeResponse;
                }
            }
            catch (Exception)
            {
                return objRequestTypeResponse;
            }
            finally
            {
                objRequestTypeResponse = null;
                objclsDashboard = null;
            }
        }

        public EKOResponse ProcessToGetEKO(EKORequest objRequest)
        {
            try
            {
                objEKOResponse.EKODashboardGrid = objclsDashboard.GetEKOData(objRequest.DateTime, objRequest.EKOStatus, objRequest.TxnStatus, objRequest.EndDateTime, objRequest.ClientName);
                return objEKOResponse;
            }
            catch (Exception)
            {
                return objEKOResponse;
            }
            finally
            {
                objEKOResponse = null;
                objclsDashboard = null;
            }
        }

        public DashboardResponse Transactions(DashboardRequest objRequest)
        {
            try
            {
                if (objRequest.DashboardType == 16)
                {
                    objDashboardResponse.DashboardGrid = objclsDashboard.GetAEPS_RUPAYTransactions(objRequest.TransactionType, objRequest.DateTime);
                }
                else if (objRequest.DashboardType == 17)
                {
                    objDashboardResponse.DashboardGrid = objclsDashboard.GetAEPSISSUERTransactions(objRequest.DateTime);
                }
                else
                {
                    objDashboardResponse.DashboardGrid = objclsDashboard.GetTransactions(objRequest.DashboardType, objRequest.DateTime);
                }
                return objDashboardResponse;
            }
            catch (Exception)
            {
                return objDashboardResponse;
            }
            finally
            {
                objDashboardResponse = null;
                objclsDashboard = null;
            }
        }

        public DMSResponse ProcessDMSStoredProcedureData(DMSRequest objRequest)
        {
            try
            {
                {
                    objDMSResponse.DMSResponseData = objclsDashboard.DMSStoredProcedure(objRequest.DateTime, objRequest.TabName);
                    return objDMSResponse;
                }
            }
            catch (Exception)
            {
                return objDMSResponse;
            }
            finally
            {
                objDMSResponse = null;
                objclsDashboard = null;
            }
        }

        public RefireResponse Processrefire(RefireRequest objrefirerequest)
        {
            try
            {
                {
                    objrefire.dtrefireresponse = objclsDashboard.GetRefireSuccFailTransactions(objrefirerequest.fromdaterefire, objrefirerequest.todaterefire, objrefirerequest.refire);
                    return objrefire;
                }
            }
            catch (Exception)
            {
                return objrefire;
            }
            finally
            {
                objrefire = null;
                objclsDashboard = null;
            }
        }

        public SuryodayResponse ProcessSuryoday(SuryodayRequest objsuryodayrequest)
        {
            try
            {
                {
                    objsurresp.dtsuryodayresp = objclsDashboard.GetSuryodayTransactions(objsuryodayrequest.DateTime, objsuryodayrequest.EsbTab);
                    return objsurresp;
                }
            }
            catch (Exception)
            {
                return objsurresp;
            }
            finally
            {
                objsurresp = null;
                objclsDashboard = null;
            }
        }

        public AccountOpeningResponse ProcessAccountOpening(AccountOpeningRequest objaccrequest)
        {
            try
            {
                {
                    objaccresp.accopeningresponse = objclsDashboard.GetAccountOpeningData(objaccrequest.accdatetime);
                    return objaccresp;
                }
            }
            catch (Exception)
            {
                return objaccresp;
            }
            finally
            {
                objaccresp = null;
                objclsDashboard = null;
            }
        }

        public CityCashResponse ProcessCityCash(CityCashRequest objcitycashrequest)
        {
            try
            {
                {
                    objcitycash.CityCashDashboardGrid = objclsDashboard.GetCityCashData(objcitycashrequest.DateTime, objcitycashrequest.CityCashStatus, objcitycashrequest.EndDateTime, objcitycashrequest.TransactionType, objcitycashrequest.ProjectID, objcitycashrequest.MobileNumber);
                    return objcitycash;
                }
            }
            catch (Exception)
            {
                return objcitycash;
            }
            finally
            {
                objcitycash = null;
                objclsDashboard = null;
            }
        }

        public AEPSAPIResponse ProcessToAEPSAPI(AEPSAPIRequest objRequest)
        {
            try
            {
                objAEPSAPIResponse.AEPSAPIDashboardGrid = objclsDashboard.GetAEPSAPIData(objRequest.DateTime, objRequest.AEPSAPIStatus, objRequest.RequestFlag, objRequest.EndDateTime, objRequest.ClientName, objRequest.ProductCode);
                return objAEPSAPIResponse;
            }
            catch (Exception)
            {
                return objAEPSAPIResponse;
            }
            finally
            {
                objAEPSAPIResponse = null;
                objclsDashboard = null;
            }
        }

        public DMSAccountOpeningResponse ProcessDMSAccountOpening(DMSAccountOpeningRequest objRequest)
        {
            try
            {
                objdmsaccopening.DMSDashboardGrid = objclsDashboard.GetAODashBoardData(objRequest.DMSDateTime, objRequest.Tab);
                return objdmsaccopening;
            }
            catch (Exception)
            {
                return objdmsaccopening;
            }
            finally
            {
                objdmsaccopening = null;
                objclsDashboard = null;
            }
        }

        public PassthroughResponse ProcessPassthrough(PassthroughRequest objRequest)
        {
            try
            {
                objPassthrough.PassthroughGrid = objclsDashboard.GetPassthroughData(objRequest.DateTime);
                return objPassthrough;
            }
            catch (Exception)
            {
                return objPassthrough;
            }
            finally
            {
                objPassthrough = null;
                objclsDashboard = null;
            }
        }

        public AUAASAResponse ProcessAUAASA(AUAASARequest objRequest)
        {
            try
            {
                objAUAASAresp.AUAASAGrid = objclsDashboard.GetAUAASATransactions(objRequest);
                return objAUAASAresp;
            }
            catch (Exception)
            {
                return objAUAASAresp;
            }
            finally
            {
                objAUAASAresp = null;
                objclsDashboard = null;
            }
        }

        public CacheResponse GetResponseMessageDetails(CacheRequest objRequest)
        {
            try
            {
                if (objRequest.WebConfig)
                {
                    objcacheresp.CacheGrid = objclsDashboard.GetCacheresponseMessage();
                }
                else
                {
                    objcacheresp.CacheGrid = objclsDashboard.GetCacheFailedData(objRequest.ResponseMessage, objRequest.DateTime);
                }
                return objcacheresp;
            }
            catch (Exception)
            {
                return objcacheresp;
            }
            finally
            {
                objcacheresp = null;
                objclsDashboard = null;
            }
        }

        public CMSResponse CMSAPIResponse(CMSRequest objRequest)
        {
            try
            {
                objcmsResponse.CMSDashboardGrid = objclsDashboard.GetCMSData(objRequest.DateTime, objRequest.EndDateTime, objRequest.CMSStatus, objRequest.PartnerID, objRequest.productcode);
                return objcmsResponse;
            }
            catch (Exception)
            {
                return objcmsResponse;
            }
            finally
            {
                objcmsResponse = null;
                objclsDashboard = null;
            }
        }


        public CashBazaarResponse CashBazaarResponse(CashBazaarRequest req)
        {
            try
            {
                objColaResp.CashBazaarGrid = objclsDashboard.GetCashBazaarData(req.COLAID,req.FromDate,req.ToDate);
                return objColaResp;
            }
            catch (Exception)
            {
                return objColaResp;
            }
            finally
            {
                objColaResp = null;
                objclsDashboard = null;
            }
        }
    }
}
