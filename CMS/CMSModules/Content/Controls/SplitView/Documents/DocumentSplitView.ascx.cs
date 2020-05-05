using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_SplitView_Documents_DocumentSplitView : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// URL of the first frame.
    /// </summary>
    public string Frame1Url
    {
        get
        {
            return splitView.Frame1Url;
        }
        set
        {
            splitView.Frame1Url = value;
        }
    }


    /// <summary>
    /// URL of the second frame.
    /// </summary>
    public string Frame2Url
    {
        get
        {
            return splitView.Frame2Url;
        }
        set
        {
            splitView.Frame2Url = value;
        }
    }


    /// <summary>
    /// Identifier of the document to work with.
    /// </summary>
    public int DocumentID
    {
        get;
        set;
    }


    /// <summary>
    /// Identifier of the node to work with.
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Culture code identifiyng culture version of node.
    /// </summary>
    public string Culture
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        bool hasNodeIdAndCulture = ((NodeID > 0) && (Culture != null));
        if (hasNodeIdAndCulture)
        {
            const string SPLIT_VIEW_FOLDER = "~/CMSModules/Content/CMSDesk/SplitView/";

            // Toolbar URL
            string toolbarUrl = SPLIT_VIEW_FOLDER + "Toolbar.aspx";
            toolbarUrl = URLHelper.AddParameterToUrl(toolbarUrl, "nodeid", NodeID.ToString());
            toolbarUrl = URLHelper.AddParameterToUrl(toolbarUrl, "culture", Culture);

            // Separator URL 
            splitView.SeparatorUrl = SPLIT_VIEW_FOLDER + "Separator.aspx";
            splitView.ToolbarUrl = toolbarUrl;
            splitView.ToolbarHeight = 51;
        }
    }

    #endregion
}