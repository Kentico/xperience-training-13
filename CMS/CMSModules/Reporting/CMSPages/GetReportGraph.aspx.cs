using System;
using System.Net;
using System.Web;

using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.UIControls;


public partial class CMSModules_Reporting_CMSPages_GetReportGraph : CMSPage
{
    protected Guid sGraphGuid;


    protected void Page_Load(object sender, EventArgs e)
    {
        //check if it is request for saved graph - by graph guid
        sGraphGuid = QueryHelper.GetGuid("graphguid", Guid.Empty);
        if (sGraphGuid != Guid.Empty)
        {
            SavedGraphInfo sGraphInfo = SavedGraphInfoProvider.GetSavedGraphInfo(sGraphGuid);
            if (sGraphInfo != null)
            {
                SavedReportInfo savedReport = SavedReportInfoProvider.GetSavedReportInfo(sGraphInfo.SavedGraphSavedReportID);
                ReportInfo report = ReportInfoProvider.GetReportInfo(savedReport.SavedReportReportID);

                //check graph security settings
                if (report.ReportAccess != ReportAccessEnum.All)
                {
                    if (!AuthenticationHelper.IsAuthenticated())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                    else
                    {
                        // Check 'Read' permission
                        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "Read"))
                        {
                            RedirectToAccessDenied("cms.reporting", "Read");
                        }
                    }
                }

                //send response with image data
                SendGraph(sGraphInfo);
                return;
            }
        }
        // Bad parameters, guid ... -> not found
        RequestHelper.Respond404();
    }


    /// <summary>
    /// Sends the graph.
    /// </summary>
    /// <param name="graphObj">Graph obj containing graph</param>
    protected void SendGraph(SavedGraphInfo graphObj)
    {
        if (graphObj != null)
        {
            SendGraph(graphObj.SavedGraphMimeType, graphObj.SavedGraphBinary);
        }
    }


    /// <summary>
    /// Sends the graph.
    /// </summary>
    /// <param name="mimeType">Response mime type</param>
    /// <param name="graph">Raw data to be sent</param>
    protected void SendGraph(string mimeType, byte[] graph)
    {
        // Clear response.
        CookieHelper.ClearResponseCookies();
        Response.Clear();

        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        // Prepare response
        Response.ContentType = mimeType;
        Response.OutputStream.Write(graph, 0, graph.Length);

        //RequestHelper.CompleteRequest();
        RequestHelper.EndResponse();
    }
}