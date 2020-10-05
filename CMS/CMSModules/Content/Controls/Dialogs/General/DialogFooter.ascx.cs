using System;
using System.Collections;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_General_DialogFooter : CMSUserControl
{
    #region "Variables"

    private OutputFormatEnum mOutputFormat = OutputFormatEnum.HTMLMedia;
    private string CustomOutputCode = "";

    #endregion


    #region "Private properties"

    /// <summary>
    /// Base tab file path.
    /// </summary>
    private string BaseFilePath
    {
        get
        {
            return "~/CMSFormControls/Selectors/InsertImageOrMedia/";
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Output format of the dialog.
    /// </summary>
    public OutputFormatEnum OutputFormat
    {
        get
        {
            return mOutputFormat;
        }
        set
        {
            mOutputFormat = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            // Setup controls
            SetupControls();
        }
    }


    #region "Public methods"

    public void InitFromQueryString()
    {
        // Get output format from query string
        string output = QueryHelper.GetString("output", "html");
        bool link = QueryHelper.GetBoolean("link", false);

        // Get output format
        OutputFormat = CMSDialogHelper.GetOutputFormat(output, link);

        // Set custom format code if required
        if (OutputFormat == OutputFormatEnum.Custom)
        {
            CustomOutputCode = output;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns true if the given output format need XXXEditor.js.
    /// </summary>
    private bool UseEditorScript()
    {
        if (OutputFormat == OutputFormatEnum.Custom)
        {
            switch (CustomOutputCode.ToLowerCSafe())
            {
                case "copy":
                case "move":
                case "link":
                case "linkdoc":
                    return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        ScriptHelper.RegisterJQuery(Page);
        CMSDialogHelper.RegisterDialogHelper(Page);

        // Register scripts for current code editor
        if (UseEditorScript())
        {
            ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/Dialogs/" + GetEditorFileName());
        }

        if (!RequestHelper.IsPostBack())
        {
            ltlScript.Text = ScriptHelper.GetScript("function DoHiddenPostback(){" + Page.ClientScript.GetPostBackEventReference(btnHidden, "") + "}");
            string getSelectionScript;
            if (UseEditorScript())
            {
                string clientId = QueryHelper.GetString("editor_clientid", "");
                getSelectionScript = String.Format("GetSelected('{0}','{1}', '{2}', '{3}');", hdnSelected.ClientID, hdnAnchors.ClientID, hdnIds.ClientID, ScriptHelper.GetString(clientId, false));
            }
            else
            {
                getSelectionScript = "DoHiddenPostback();";
            }

            // Wait for full load, otherwise Chrome don't loads CSS backgrounds for footer and buttons
            ScriptHelper.RegisterStartupScript(this, typeof(String), "DoHiddenPostback", String.Format("$cmsj(window).load(function() {{ {0} }});", getSelectionScript), true);
        }

        if (OutputFormat == OutputFormatEnum.URL)
        {
            btnInsert.ResourceString = "general.select";
        }
        else if (OutputFormat == OutputFormatEnum.Custom)
        {
            switch (CustomOutputCode.ToLowerCSafe())
            {
                case "copy":
                    btnInsert.ResourceString = "general.copy";
                    break;

                case "move":
                    btnInsert.ResourceString = "general.move";
                    break;

                case "link":
                    btnInsert.ResourceString = "general.link";
                    break;

                case "linkdoc":
                    btnInsert.ResourceString = "general.link";
                    break;

                default:
                    btnInsert.ResourceString = "general.select";
                    break;
            }
        }
    }


    /// <summary>
    /// Returns file name with javascript methods for inserting/selecting items using specified editor.
    /// File name format is XXXEditor.js where XXX is current output code.
    /// </summary>
    private string GetEditorFileName()
    {
        string output = "";

        // Select output format
        switch (OutputFormat)
        {
            case OutputFormatEnum.HTMLMedia:
            case OutputFormatEnum.HTMLLink:
                output = "HTML";
                break;

            case OutputFormatEnum.BBMedia:
            case OutputFormatEnum.BBLink:
                output = "BB";
                break;

            case OutputFormatEnum.URL:
                output = "URL";
                break;

            case OutputFormatEnum.Custom:
                output = CustomOutputCode;
                break;
        }

        return (output + "Editor.js");
    }


    /// <summary>
    /// Setup media dialog from selected item.
    /// </summary>
    /// <param name="selectionTable">Hash table from selected item</param>
    /// <param name="anchorsList">List of anchors from document</param>
    /// <param name="idsList">List of ids from document</param>
    private void SelectMediaDialog(IDictionary selectionTable, ICollection anchorsList, ICollection idsList)
    {
        string insertHeaderLocation = BaseFilePath + "Header.aspx" + RequestContext.CurrentQueryString;
        if (selectionTable.Count > 0)
        {
            string siteName = null;
            string url = null;
            // If link dialog use only link url
            if (RequestContext.CurrentQueryString.ToLowerCSafe().Contains("link=1"))
            {
                if (selectionTable[DialogParameters.LINK_URL] != null)
                {
                    url = selectionTable[DialogParameters.LINK_URL].ToString();
                    if ((selectionTable[DialogParameters.LINK_PROTOCOL] != null) && (selectionTable[DialogParameters.LINK_PROTOCOL].ToString() != "other"))
                    {
                        // Add protocol only if not already presents
                        if (!url.StartsWithCSafe(selectionTable[DialogParameters.LINK_PROTOCOL].ToString()))
                        {
                            url = selectionTable[DialogParameters.LINK_PROTOCOL] + url;
                        }
                    }
                }
                else if (selectionTable[DialogParameters.URL_URL] != null)
                {
                    url = selectionTable[DialogParameters.URL_URL].ToString();
                }
            }
            else
            {
                // Get url from selection table
                if (selectionTable[DialogParameters.IMG_URL] != null)
                {
                    url = selectionTable[DialogParameters.IMG_URL].ToString();
                }
                else if (selectionTable[DialogParameters.AV_URL] != null)
                {
                    url = selectionTable[DialogParameters.AV_URL].ToString();
                }
                else if (selectionTable[DialogParameters.LINK_URL] != null)
                {
                    url = selectionTable[DialogParameters.LINK_URL].ToString();
                }
                else if (selectionTable[DialogParameters.URL_URL] != null)
                {
                    url = selectionTable[DialogParameters.URL_URL].ToString();
                    siteName = (selectionTable[DialogParameters.URL_SITENAME] != null ? selectionTable[DialogParameters.URL_SITENAME].ToString() : null);
                }
            }
            string query = URLHelper.RemoveUrlParameter(RequestContext.CurrentQueryString, "hash");

            // Get the data for media source
            MediaSource ms = CMSDialogHelper.GetMediaData(url, siteName);
            if (ms != null)
            {
                SessionHelper.SetValue("MediaSource", ms);

                // Preselect the tab
                if (!selectionTable.Contains(DialogParameters.EMAIL_TO) || !selectionTable.Contains(DialogParameters.ANCHOR_NAME))
                {
                    switch (ms.SourceType)
                    {
                        case MediaSourceEnum.DocumentAttachments:
                        case MediaSourceEnum.MetaFile:
                            query = URLHelper.AddUrlParameter(query, "tab", "attachments");
                            break;

                        case MediaSourceEnum.Content:
                            query = URLHelper.AddUrlParameter(query, "tab", "content");
                            break;

                        case MediaSourceEnum.MediaLibraries:
                            query = URLHelper.AddUrlParameter(query, "tab", "libraries");
                            break;

                        default:
                            query = URLHelper.AddUrlParameter(query, "tab", "web");
                            break;
                    }
                }

                // Update old format url
                if ((selectionTable.Contains(DialogParameters.URL_OLDFORMAT)) && (selectionTable.Contains(DialogParameters.URL_GUID)))
                {
                    if (String.IsNullOrEmpty(siteName))
                    {
                        siteName = SiteContext.CurrentSiteName;
                    }
                    string outUrl = ModuleCommands.MediaLibraryGetMediaFileUrl(selectionTable[DialogParameters.URL_GUID].ToString(), siteName);
                    if (!String.IsNullOrEmpty(outUrl))
                    {
                        selectionTable[DialogParameters.URL_URL] = outUrl;
                    }
                }

                // Set extension if not exist in selection table
                if ((selectionTable[DialogParameters.URL_EXT] == null) || ((selectionTable[DialogParameters.URL_EXT] != null) && (String.IsNullOrEmpty(selectionTable[DialogParameters.URL_EXT].ToString()))))
                {
                    selectionTable[DialogParameters.URL_EXT] = ms.Extension;
                }

                // Update selection table if only URL presents
                if (selectionTable.Contains(DialogParameters.URL_URL))
                {
                    switch (ms.MediaType)
                    {
                        case MediaTypeEnum.Image:
                            // Image
                            selectionTable[DialogParameters.IMG_URL] = UrlResolver.ResolveUrl(selectionTable[DialogParameters.URL_URL].ToString());
                            selectionTable[DialogParameters.IMG_WIDTH] = selectionTable[DialogParameters.URL_WIDTH];
                            selectionTable[DialogParameters.IMG_HEIGHT] = selectionTable[DialogParameters.URL_HEIGHT];
                            break;

                        case MediaTypeEnum.AudioVideo:
                            // Media
                            selectionTable[DialogParameters.AV_URL] = UrlResolver.ResolveUrl(selectionTable[DialogParameters.URL_URL].ToString());
                            selectionTable[DialogParameters.AV_WIDTH] = selectionTable[DialogParameters.URL_WIDTH];
                            selectionTable[DialogParameters.AV_HEIGHT] = selectionTable[DialogParameters.URL_HEIGHT];
                            selectionTable[DialogParameters.AV_EXT] = ms.Extension;
                            break;
                    }

                    if (OutputFormat != OutputFormatEnum.URL)
                    {
                        selectionTable[DialogParameters.URL_URL] = UrlResolver.ResolveUrl(selectionTable[DialogParameters.URL_URL].ToString());
                    }

                    selectionTable[DialogParameters.FILE_NAME] = ms.FileName;
                    selectionTable[DialogParameters.FILE_SIZE] = ms.FileSize;
                }

                // Add original size into table
                selectionTable[DialogParameters.IMG_ORIGINALWIDTH] = ms.MediaWidth;
                selectionTable[DialogParameters.IMG_ORIGINALHEIGHT] = ms.MediaHeight;
            }
            else
            {
                if (selectionTable.Contains(DialogParameters.EMAIL_TO))
                {
                    query = URLHelper.AddUrlParameter(query, "tab", "email");
                }
                if (selectionTable.Contains(DialogParameters.ANCHOR_NAME))
                {
                    query = URLHelper.AddUrlParameter(query, "tab", "anchor");
                }
            }

            query = URLHelper.AddUrlParameter(query, "hash", QueryHelper.GetHash(query));
            insertHeaderLocation = BaseFilePath + "Header.aspx" + query;
        }


        // Set selected item into session
        SessionHelper.SetValue("DialogParameters", selectionTable);

        if ((anchorsList != null) && (anchorsList.Count > 0))
        {
            SessionHelper.SetValue("Anchors", anchorsList);
        }
        if ((idsList != null) && (idsList.Count > 0))
        {
            SessionHelper.SetValue("Ids", idsList);
        }

        if (((selectionTable[DialogParameters.LINK_TEXT] != null) &&
             (selectionTable[DialogParameters.LINK_TEXT].ToString() == "##LINKTEXT##")) ||
            ((selectionTable[DialogParameters.EMAIL_LINKTEXT] != null) &&
             (selectionTable[DialogParameters.EMAIL_LINKTEXT].ToString() == "##LINKTEXT##")) ||
            ((selectionTable[DialogParameters.ANCHOR_LINKTEXT] != null) &&
             (selectionTable[DialogParameters.ANCHOR_LINKTEXT].ToString() == "##LINKTEXT##")))
        {
            SessionHelper.SetValue("HideLinkText", true);
        }

        ltlScript.Text = ScriptHelper.GetScript("if (window.parent.frames['insertHeader']) { window.parent.frames['insertHeader'].location= \"" + ResolveUrl(insertHeaderLocation) + "\";} ");
    }

    #endregion


    #region "Event handlers"

    protected void btnHidden_Click(object sender, EventArgs e)
    {
        SessionHelper.SetValue("MediaSource", null);
        SessionHelper.SetValue("DialogParameters", null);
        SessionHelper.SetValue("DialogSelectedParameters", null);
        SessionHelper.SetValue("Anchors", null);
        SessionHelper.SetValue("Ids", null);
        SessionHelper.SetValue("HideLinkText", null);

        string selected = hdnSelected.Value;
        Hashtable tSelection = CMSDialogHelper.GetHashTableFromString(selected);

        // 'Insert link' dialog in CK editor
        if (OutputFormat == OutputFormatEnum.HTMLLink)
        {
            string anchors = hdnAnchors.Value;
            string ids = hdnIds.Value;
            ArrayList lAnchors = CMSDialogHelper.GetListFromString(anchors);
            ArrayList lIds = CMSDialogHelper.GetListFromString(ids);
            lAnchors.Sort();
            lIds.Sort();
            SelectMediaDialog(tSelection, lAnchors, lIds);
        }
        // Dialogs in other editors
        else
        {
            SelectMediaDialog(tSelection, null, null);
        }
    }

    #endregion
}
