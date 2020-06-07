using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_GetWebPartProperties : CMSDeskPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string aliasPath = QueryHelper.GetString("aliaspath", "");
        string webpartId = QueryHelper.GetString("webpartid", "");
        string zoneId = QueryHelper.GetString("zoneid", "");
        Guid webpartGuid = QueryHelper.GetGuid("webpartguid", Guid.Empty);
        int templateId = QueryHelper.GetInteger("templateId", 0);

        PageTemplateInfo pti = null;

        if (templateId > 0)
        {
            pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
        }

        if (pti != null)
        {
            // Get web part
            WebPartInstance webPart = pti.TemplateInstance.GetWebPart(webpartGuid, webpartId);
            if (webPart != null)
            {
                StringBuilder sb = new StringBuilder();
                Hashtable properties = webPart.Properties;

                // Get the webpart object
                WebPartInfo wi = WebPartInfoProvider.GetWebPartInfo(webPart.WebPartType);
                if (wi != null)
                {
                    // Add the header
                    sb.Append("Webpart properties (" + wi.WebPartDisplayName + ")" + Environment.NewLine + Environment.NewLine);
                    sb.Append("Alias path: " + aliasPath + Environment.NewLine);
                    sb.Append("Zone ID: " + zoneId + Environment.NewLine + Environment.NewLine);


                    string wpProperties = "<default></default>";
                    // Get the form info object and load it with the data

                    if (wi.WebPartParentID > 0)
                    {
                        // Load parent properties
                        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(wi.WebPartParentID);
                        if (wpi != null)
                        {
                            wpProperties = FormHelper.MergeFormDefinitions(wpi.WebPartProperties, wi.WebPartProperties);
                        }
                    }
                    else
                    {
                        wpProperties = wi.WebPartProperties;
                    }

                    FormInfo fi = new FormInfo(wpProperties);

                    // General properties of webparts
                    string beforeFormDefinition = PortalFormHelper.GetWebPartProperties((WebPartTypeEnum)wi.WebPartType, PropertiesPosition.Before);
                    string afterFormDefinition = PortalFormHelper.GetWebPartProperties((WebPartTypeEnum)wi.WebPartType, PropertiesPosition.After);

                    // General properties before custom
                    if (!String.IsNullOrEmpty(beforeFormDefinition))
                    {
                        // Load before definition
                        fi = new FormInfo(FormHelper.MergeFormDefinitions(beforeFormDefinition, wpProperties, true));

                        // Add Default category for first before items
                        sb.Append(Environment.NewLine + "Default" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                    }


                    // General properties after custom
                    if (!String.IsNullOrEmpty(afterFormDefinition))
                    {
                        // Load after definition
                        fi = new FormInfo(FormHelper.MergeFormDefinitions(fi.GetXmlDefinition(), afterFormDefinition, true));
                    }

                    // Generate all properties
                    sb.Append(GetProperties(fi.GetFormElements(true, false), webPart));

                    // Send the text file to the user to download
                    UTF8Encoding enc = new UTF8Encoding();
                    byte[] file = enc.GetBytes(sb.ToString());

                    HTTPHelper.AddContentDispositionHeader(Response, true, $"webpartproperties_{webPart.ControlID}.txt");
                    Response.ContentType = "text/plain";
                    Response.BinaryWrite(file);

                    RequestHelper.EndResponse();
                }
            }
        }
    }


    /// <summary>
    /// Exports properties given in the array list.
    /// </summary>
    /// <param name="elem">List of properties to export</param>
    /// <param name="webPart">WebPart instance with data</param>
    private string GetProperties(IEnumerable<IDataDefinitionItem> elem, WebPartInstance webPart)
    {
        StringBuilder sb = new StringBuilder();

        // Iterate through all fields
        foreach (object o in elem)
        {
            FormCategoryInfo fci = o as FormCategoryInfo;
            if (fci != null)
            {
                // We have category now
                sb.Append(Environment.NewLine + fci.GetPropertyValue(FormCategoryPropertyEnum.Caption, MacroContext.CurrentResolver) + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }
            else
            {
                // Form element
                FormFieldInfo ffi = o as FormFieldInfo;
                if (ffi != null)
                {
                    object value = webPart.GetValue(ffi.Name);
                    sb.Append(ffi.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, MacroContext.CurrentResolver) + ": " + (value == null ? "" : value.ToString()) + Environment.NewLine + Environment.NewLine);
                }
            }
        }

        return sb.ToString();
    }
}