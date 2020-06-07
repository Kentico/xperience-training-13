using System;
using System.Data;
using System.Text;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_DocumentFlags : DocumentFlagsControl
{
    #region "Private variables"

    private string mDefaultSiteCulture;
    private int mRepeatColumns = 10;
    private string mSelectJSFunction = "DF_Redir";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the item URL
    /// </summary>
    public override string ItemUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets Node ID.
    /// </summary>
    public override int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Number of columns for the flags.
    /// </summary>
    public override int RepeatColumns
    {
        get
        {
            return mRepeatColumns;
        }
        set
        {
            mRepeatColumns = value;
        }
    }


    /// <summary>
    /// Data source object.
    /// </summary>
    public override object DataSource
    {
        get;
        set;
    }


    /// <summary>
    /// DataSet containing all site cultures.
    /// </summary>
    public override DataSet SiteCultures
    {
        get;
        set;
    }


    /// <summary>
    /// Default culture of the site.
    /// </summary>
    public string DefaultSiteCulture
    {
        get
        {
            return mDefaultSiteCulture ?? (mDefaultSiteCulture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName));
        }
    }


    /// <summary>
    /// Name of the javascript function called when flag is clicked. NodeId, culture, translated flag and item URL are supplied as parameters.
    /// </summary>
    public override string SelectJSFunction
    {
        get
        {
            return mSelectJSFunction;
        }
        set
        {
            mSelectJSFunction = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!ScriptHelper.IsClientScriptBlockRegistered("FlagActions"))
        {
            StringBuilder sb = new StringBuilder();

            string strCulture = GetString("transman.language");
            string strStatus = GetString("transman.status");
            string strModified = GetString("transman.modified");
            string strVersion = GetString("transman.version");

            sb.Append(
                @"function DF_Tip(icon, culture, status, version, modif) {
    var code = '<div class=""document-flag-tooltip""><div style=""text-align:center;""><img class=""Icon"" src=""' + icon + '"" alt=""' + culture + '"" /></div><div class=""Text"">", string.Format(strCulture, "' + culture + '&nbsp;"), @"<br />", string.Format(strStatus, "' + status + '&nbsp;"), @"';
    if (version != '') {
        code += '<br />", string.Format(strVersion, "' + version + '&nbsp;"), @"';
    }
    if (modif != '') {
        code += '<br />", string.Format(strModified, "' + modif + '&nbsp;"), @"';
    }
    code += '</div></div>';
    Tip(code);
}

function DF_Redir(nodeId, culture, translated, url) {
    if (RedirectItem != null) {
        RedirectItem(nodeId, culture, translated, url);
    }
}
");

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "FlagActions", ScriptHelper.GetScript(sb.ToString()));
        }
    }

    #endregion


    #region "Public methods"

    public override void ReloadData()
    {
        // Hide control for one culture version
        if ((DataSource == null) || DataHelper.DataSourceIsEmpty(SiteCultures) || (SiteCultures.Tables[0].Rows.Count <= 1))
        {
            Visible = false;
        }
        else
        {
            // Check the data source
            if (!(DataSource is GroupedDataSource))
            {
                throw new Exception("[DocumentFlags]: Only GroupedDataSource is supported as a data source.");
            }

            // Register tooltip script
            ScriptHelper.RegisterTooltip(Page);

            // Get appropriate table from the data source
            GroupedDataSource gDS = (GroupedDataSource)DataSource;
            DataTable table = gDS.GetGroupDataTable(NodeID);

            // Get document in the default site culture
            DateTime defaultLastModification = DateTimeHelper.ZERO_TIME;
            DateTime defaultLastPublished = DateTimeHelper.ZERO_TIME;
            bool defaultCultureExists = false;
            DataRow[] rows = null;
            if (table != null)
            {
                rows = table.Select("DocumentCulture='" + DefaultSiteCulture + "'");
                defaultCultureExists = (rows.Length > 0);
            }

            if (defaultCultureExists)
            {
                defaultLastModification = ValidationHelper.GetDateTime(rows[0]["DocumentModifiedWhen"], DateTimeHelper.ZERO_TIME);
                defaultLastPublished = ValidationHelper.GetDateTime(rows[0]["DocumentLastPublished"], DateTimeHelper.ZERO_TIME);
            }

            // Build the content
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"document-flags\">");
            int colsInRow = 0;
            int cols = 0;
            int colsCount = SiteCultures.Tables[0].Rows.Count;
            if (colsCount < RepeatColumns)
            {
                RepeatColumns = colsCount;
            }

            foreach (DataRow dr in SiteCultures.Tables[0].Rows)
            {
                ++cols;
                ++colsInRow;
                DateTime lastModification = DateTimeHelper.ZERO_TIME;
                string versionNumber = null;
                string className = null;
                TranslationStatusEnum status = TranslationStatusEnum.NotAvailable;
                string cultureName = DataHelper.GetStringValue(dr, "CultureName", "-");
                string cultureCode = DataHelper.GetStringValue(dr, "CultureCode", "-");

                // Get document for given culture
                if (table != null)
                {
                    rows = table.Select("DocumentCulture='" + cultureCode + "'");
                    // Document doesn't exist
                    if (rows.Length != 0)
                    {
                        className = DataHelper.GetStringValue(rows[0], "ClassName", null);
                        versionNumber = DataHelper.GetStringValue(rows[0], "DocumentLastVersionNumber", null);

                        // Check if document is outdated
                        if (versionNumber != null)
                        {
                            lastModification = ValidationHelper.GetDateTime(rows[0]["DocumentLastPublished"], DateTimeHelper.ZERO_TIME);
                            status = (lastModification < defaultLastPublished) ? TranslationStatusEnum.Outdated : TranslationStatusEnum.Translated;
                        }
                        else
                        {
                            lastModification = ValidationHelper.GetDateTime(rows[0]["DocumentModifiedWhen"], DateTimeHelper.ZERO_TIME);
                            status = (lastModification < defaultLastModification) ? TranslationStatusEnum.Outdated : TranslationStatusEnum.Translated;
                        }
                    }
                }

                sb.Append("<span class=\"", GetStatusCSSClass(status), "\">");

                var itemUrl = (className != null && DataClassInfoProvider.GetDataClassInfo(className).ClassHasURL) ? UrlResolver.ResolveUrl(DocumentUIHelper.GetPageHandlerLivePath(NodeID, cultureCode)) : "#";
                sb.Append("<img onmouseout=\"UnTip()\" style=\"cursor:pointer;\" onclick=\"", SelectJSFunction, "('", NodeID, "','", cultureCode, "'," + Convert.ToInt32((status != TranslationStatusEnum.NotAvailable)) + "," + ScriptHelper.GetString(itemUrl) + ")\" onmouseover=\"DF_Tip('", GetFlagIconUrl(cultureCode, "48x48"), "', '", cultureName, "', '", GetStatusString(status), "', '");

                sb.Append(versionNumber ?? string.Empty);
                sb.Append("', '");
                sb.Append((lastModification != DateTimeHelper.ZERO_TIME) ?
                    TimeZoneHelper.ConvertToUserTimeZone(lastModification, true, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite)
                    : string.Empty);
                sb.Append("')\" src=\"");
                sb.Append(GetFlagIconUrl(cultureCode, "16x16"));
                sb.Append("\" alt=\"");
                sb.Append(cultureName);
                sb.Append("\" /></span>");

                // Ensure repeat columns
                if (((colsInRow % RepeatColumns) == 0) && (cols != colsCount))
                {
                    sb.Append("<br />\n");
                    colsInRow = 0;
                }
            }

            sb.Append("</div>\n");
            ltlFlags.Text = sb.ToString();
        }
    }


    /// <summary>
    /// Returns resolved path to the flag image for the specified culture.
    /// </summary>
    /// <param name="cultureCode">Culture code</param>
    /// <param name="iconSet">Name of the subfolder where icon images are located</param>
    public override string GetFlagIconUrl(string cultureCode, string iconSet)
    {
        // Short path to the icon
        if (ControlsExtensions.RenderShortIDs)
        {
            if (iconSet == "48x48")
            {
                return UIHelper.GetShortImageUrl(UIHelper.FLAG_ICONS_48, cultureCode + ".png");
            }
            else
            {
                return UIHelper.GetShortImageUrl(UIHelper.FLAG_ICONS, cultureCode + ".png");
            }
        }

        return base.GetFlagIconUrl(cultureCode, iconSet);
    }

    #endregion


    #region "Private methods"

    private static string GetStatusCSSClass(TranslationStatusEnum status)
    {
        switch (status)
        {
            case TranslationStatusEnum.NotAvailable:
                return "NotAvailable";

            case TranslationStatusEnum.Outdated:
                return "Outdated";

            default:
                return "Translated";
        }
    }


    private static string GetStatusString(TranslationStatusEnum status)
    {
        switch (status)
        {
            case TranslationStatusEnum.NotAvailable:
                return ResHelper.GetString("transman.NotAvailable");

            case TranslationStatusEnum.Outdated:
                return ResHelper.GetString("transman.Outdated");

            default:
                return ResHelper.GetString("transman.Translated");
        }
    }

    #endregion
}