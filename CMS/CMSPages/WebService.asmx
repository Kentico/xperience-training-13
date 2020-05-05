<%@ WebService Language="C#" Class="WebService" %>

using System;
using System.Data;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Empty web service template.
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class WebService : System.Web.Services.WebService
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public WebService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }


    /// <summary>
    /// Returns the data from DB.
    /// </summary>
    /// <param name="parameter">String parameter for sql command</param>
    [WebMethod]
    public DataSet GetDataSet(string parameter)
    {
        // INSERT YOUR WEB SERVICE CODE AND RETURN THE RESULTING DATASET

        return null;
    }


    /// <summary>
    /// The web service method to be called by AJAXControlToolkit. The signature of this method must match this method.
    /// Note that you can replace "GetCompletionList" with a name of your choice, but the return type and parameter name and type must exactly match, including case.
    /// </summary>
    /// <param name="prefixText">Prefix to be searched</param>
    /// <param name="count">Number of suggestions to be retrieved from the web service</param>
    /// <returns>Array of suggestions</returns>
    [WebMethod(MessageName = "GetCompletionList")]
    [ScriptMethod]
    public string[] GetCompletionList(string prefixText, int count)
    {
        // INSERT YOUR WEB SERVICE CODE AND RETURN THE RESULTING STRING ARRAY

        return null;
    }
}