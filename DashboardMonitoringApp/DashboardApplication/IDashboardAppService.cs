using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.IO;

namespace DashboardApplication
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDashboardAppService" in both code and config file together.
    [ServiceContract]
    public interface IDashboardAppService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessDashboardRequest", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DashboardResponse ProcessDashboardRequest(DashboardRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessESBRequest", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ESBResponse ProcessESBRequest(ESBRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessFailedAwaiting", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ESBResponse ProcessFailedAwaiting(ESBRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessLatency", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LatencyResponse ProcessLatency(LatencyRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessMerchantTransaction", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        MerchantResponse ProcessMerchantTransaction(MerchantRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessESBLatency", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ESBLatencyResponse ProcessESBLatency(ESBLatencyRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessESBNODESERVICELatency", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LatencyResponse ProcessESBNODESERVICELatency(LatencyRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessHA_ESBLatency", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LatencyResponse ProcessHA_ESBLatency(LatencyRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessGetLogs", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LogsResponse ProcessGetLogs(LogsRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessIMPSData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        IMPSResponse ProcessIMPSData(IMPSRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessDMSData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DMSResponse ProcessDMSData(DMSRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessESB", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ZoneResponse ProcessESB(ZoneRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessToGetRequestType", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        RequsetType_Response ProcessToGetRequestType(RequestType_Request objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessToGetEKO", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        EKOResponse ProcessToGetEKO(EKORequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessToAEPSAPI", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        AEPSAPIResponse ProcessToAEPSAPI(AEPSAPIRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Transactions", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DashboardResponse Transactions(DashboardRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessDMSStoredProcedureData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DMSResponse ProcessDMSStoredProcedureData(DMSRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessAEPSFailed", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        AEPSResponse ProcessAEPSFailed(AEPSRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Processrefire", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        RefireResponse Processrefire(RefireRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessSuryoday", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SuryodayResponse ProcessSuryoday(SuryodayRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessAccountOpening", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        AccountOpeningResponse ProcessAccountOpening(AccountOpeningRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessCityCash", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CityCashResponse ProcessCityCash(CityCashRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessDMSAccountOpening", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DMSAccountOpeningResponse ProcessDMSAccountOpening(DMSAccountOpeningRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessPassthrough", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        PassthroughResponse ProcessPassthrough(PassthroughRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ProcessAUAASA", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        AUAASAResponse ProcessAUAASA(AUAASARequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetResponseMessageDetails", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CacheResponse GetResponseMessageDetails(CacheRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/CMSAPIResponse", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CMSResponse CMSAPIResponse(CMSRequest objRequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/CashBazaarResponse", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CashBazaarResponse CashBazaarResponse(CashBazaarRequest objReq);

    }
}
