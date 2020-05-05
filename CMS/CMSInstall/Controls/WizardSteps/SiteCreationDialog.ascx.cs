using System;
using System.Data;
using System.Text;

using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.UIControls;

public partial class CMSInstall_Controls_WizardSteps_SiteCreationDialog : CMSUserControl
{
    /// <summary>
    /// Type of the site creation.
    /// </summary>
    public enum CreationTypeEnum
    {
        /// <summary>
        /// Use template.
        /// </summary>
        Template = 0,

        /// <summary>
        /// Create a new site or import an existing Kentico site
        /// </summary>
        AddOrImportSite = 1
    }


    private bool mStopProcess = true;
    private string mCurrentEdition = null;

    /// <summary>
    /// Template name to be created.
    /// </summary>
    public string TemplateName
    {
        get
        {
            return hdnName.Value;
        }
    }


    /// <summary>
    /// Stop processing.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return mStopProcess;
        }
        set
        {
            mStopProcess = value;
        }
    }


    /// <summary>
    /// Current edition.
    /// </summary>
    protected string CurrentEdition
    {
        get
        {
            if (mCurrentEdition == null)
            {
                // Get current domain and license
                string domain = RequestContext.CurrentDomain;
                LicenseKeyInfo li = LicenseKeyInfoProvider.GetLicenseKeyInfo(domain);
                if (li != null)
                {
                    mCurrentEdition = li.Edition.ToStringRepresentation<ProductEditionEnum>();
                }
            }
            return mCurrentEdition;
        }
    }

    
    /// <summary>
    /// Template name to be created.
    /// </summary>
    public CreationTypeEnum CreationType
    {
        get
        {
            if (radAddSite.Checked)
            {
                return CreationTypeEnum.AddOrImportSite;
            }

            return CreationTypeEnum.Template;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ltlScript.Text = ScriptHelper.GetScript("var hdnField=document.getElementById('" + hdnName.ClientID + "');var hdnLastSelected=document.getElementById('" + hdnLastSelected.ClientID + "');");

        if (RequestHelper.IsPostBack())
        {
            ltlScriptAfter.Text = ScriptHelper.GetScript("SelectTemplate(hdnLastSelected.value,hdnField.value);");
        }

        radAddSite.Text = ResHelper.GetFileString("Install.radaddsite");
        radTemplate.Text = ResHelper.GetFileString("Install.radTemplate");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Load web templates
        if (!StopProcessing)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Gets metafile preview.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    private string GetPreviewImage(int templateId)
    {
        var thumbnailUrl = WebTemplateInfoProvider.GetWebTemplateInfo(templateId)?.Generalized.GetThumbnailUrl(0, 0, 0);
        if (thumbnailUrl != null)
        {
            return ResolveUrl(thumbnailUrl);
        }
        else
        {
            return GetImageUrl("Others/Install/no_image.png");
        }
    }


    /// <summary>
    /// Data bind.
    /// </summary>
    public void ReloadData()
    {
        if (WebTemplateInfoProvider.IsAnyTemplatePresent())
        {
            var templates = WebTemplateInfoProvider.GetWebTemplates(null, "WebTemplateOrder", 0, null, true);
            if (!DataHelper.DataSourceIsEmpty(templates))
            {
                string firstTemplateName = "";
                int firstTemplateId = 0;

                // Find first allowed template
                foreach (var template in templates)
                {
                    string templateEditions = template.WebTemplateLicenses;
                    if (IsTemplateAllowed(templateEditions))
                    {
                        firstTemplateName = template.WebTemplateName;
                        firstTemplateId = template.WebTemplateId;
                        break;
                    }
                }

                // Bind the list
                rptSites.DataSource = templates;
                rptSites.DataBind();

                // Preselect first template
                if (string.IsNullOrEmpty(hdnLastSelected.Value) || (hdnLastSelected.Value == "0"))
                {
                    ltlScriptAfter.Text += ScriptHelper.GetScript("SelectTemplate('tpl" + firstTemplateId + "','" + firstTemplateName + "');");
                }
            }
        }
        else
        {
            plcInfo.Visible = true;
            lblInfo.Text = ResHelper.GetFileString("Install.TemplatesWarning");
            radTemplate.Enabled = false;
            plcTemplates.Visible = false;
            radAddSite.Checked = true;
        }
    }


    protected string GetItemHTML(object dataItem)
    {
        DataRowView view = (DataRowView)dataItem;
        var templateId = ValidationHelper.GetInteger(view["WebTemplateID"], 0);
        string templateName = ValidationHelper.GetString(view["WebTemplateName"], "");
        string templateDisplayName = ValidationHelper.GetString(view["WebTemplateDisplayName"], "");
        string templateDescription = ValidationHelper.GetString(view["WebTemplateShortDescription"], "");
        string templateEditions = ValidationHelper.GetString(view["WebTemplateLicenses"], "");
        bool isAllowed = false;

        // Check if the current license is suitable for web template
        isAllowed = IsTemplateAllowed(templateEditions);

        // Generate HTML code
        StringBuilder builder = new StringBuilder();

        string cssClass = isAllowed ? "install-item" : "install-disabled-item";
        string textColor = isAllowed ? "" : "color:Silver;";
        string errorMessage = "";

        if (!isAllowed)
        {
            errorMessage = "<div style=\"color:Red;\"><br />";
            errorMessage += String.Format(ResHelper.GetFileString("Install.WebTemplateRequiresLicense"), EditionsToString(templateEditions));
            errorMessage += ". " + String.Format(ResHelper.GetFileString("Install.WebTemplateCurrentLicense"), GetEditionName(CurrentEdition));
            // Finish the sentence
            errorMessage += ".</div>";
        }

        builder.Append("<div class=\"" + cssClass + "\" id=\"tpl" + templateId + "\"");
        if (isAllowed)
        {
            builder.Append(" onclick=\"SelectTemplate('tpl" + templateId + "','" + templateName + "')\"");
        }
        builder.Append(">\n");
        builder.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">\n");
        builder.Append("<tr>\n<td style=\"vertical-align:top;\">\n");
        builder.Append("<img style=\"margin: 3px;\" src=\"" + GetPreviewImage(templateId) + "\" width=\"140\" height=\"108\" alt=\"Preview\" />\n");
        builder.Append("</td>\n<td style=\"vertical-align:top;\">\n");
        builder.Append("<div style=\"margin: 3px;" + textColor + "\">\n<div>\n<strong>" + templateDisplayName + "</strong></div>\n");
        builder.Append("<br /><div>" + templateDescription + "</div>" + errorMessage + "</div></td></tr>\n</table>\n</div>\n");
        return builder.ToString();
    }


    private bool IsTemplateAllowed(string templateEditions)
    {
        if (CurrentEdition != null)
        {
            // Check if the current license is suitable for web template
            if (templateEditions.Contains(CurrentEdition))
            {
                return true;
            }
        }
        
        return false;
    }


    private static string EditionsToString(string templateEditions)
    {
        string names = "";
        string[] editions = templateEditions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string editionChar in editions)
        {
            try
            {
                // Add all editions to names
                ProductEditionEnum edition = editionChar.ToEnum<ProductEditionEnum>();
                names += LicenseHelper.GetEditionName(edition) + ", ";
            }
            catch
            {
                return "#UNKNOWN#";
            }
        }

        if (names == String.Empty)
        {
            return String.Empty;
        }

        return names.Substring(0, names.Length - 2);
    }


    private static string GetEditionName(string editionChar)
    {
        try
        {
            ProductEditionEnum edition = editionChar.ToEnum<ProductEditionEnum>();
            return LicenseHelper.GetEditionName(edition);
        }
        catch
        {
            return "#UNKNOWN#";
        }
    }
}
