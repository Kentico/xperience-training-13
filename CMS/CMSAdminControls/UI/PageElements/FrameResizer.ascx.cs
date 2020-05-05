using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_PageElements_FrameResizer : CMSUserControl
{
    #region "Variables"

    protected string minimizeUrl = null;
    protected string maximizeUrl = null;

    protected string originalSize = null;
    protected string mMinSize = null;

    protected string mFramesetName = null;
    protected bool mVertical = false;
    protected bool mAll = false;
    protected string mCssPrefix = "";
    protected int mParentLevel = 1;

    #endregion


    #region "Properties"

    /// <summary>
    /// Frameset minimized size.
    /// </summary>
    public string MinSize
    {
        get
        {
            return mMinSize;
        }
        set
        {
            mMinSize = value;
        }
    }


    /// <summary>
    /// Vertical / horizontal mode
    /// </summary>
    public bool Vertical
    {
        get
        {
            return mVertical;
        }
        set
        {
            mVertical = value;
        }
    }


    /// <summary>
    /// Minimize / maximize all the resizers on the page
    /// </summary>
    public bool All
    {
        get
        {
            return mAll;
        }
        set
        {
            mAll = value;
        }
    }


    /// <summary>
    /// Frameset name.
    /// </summary>
    public string FramesetName
    {
        get
        {
            return ValidationHelper.GetString(mFramesetName, (Vertical ? "rowsFrameset" : "colsFrameset"));
        }
        set
        {
            mFramesetName = value;
        }
    }


    /// <summary>
    /// Css prefix.
    /// </summary>
    public string Direction
    {
        get
        {
            return All ? null : (Vertical ? "Vertical" : "Horizontal");
        }
    }


    /// <summary>
    /// Parent level (1 = immediate parent).
    /// </summary>
    public int ParentLevel
    {
        get
        {
            return mParentLevel;
        }
        set
        {
            mParentLevel = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterResizer(Page);

        const string basePath = "Design/Controls/FrameResizer/";

        if (All)
        {
            minimizeUrl = GetImageUrl(basePath + "All/minimizeall.png");
            maximizeUrl = GetImageUrl(basePath + "All/maximizeall.png");

            plcAll.Visible = true;
            plcStandard.Visible = false;
        }
        else
        {
            plcStandard.Visible = true;
            plcAll.Visible = false;

            pnlResizer.CssClass = Direction + "FrameResizer";

            if (!Vertical && CultureHelper.IsUICultureRTL())
            {
                MinSize = ControlsHelper.GetReversedColumns(MinSize);
            }
            minimizeUrl = GetImageUrl(basePath + Direction + "/minimize.png");
            maximizeUrl = GetImageUrl(basePath + Direction + "/maximize.png");

            // Define javascript variables
            string varsScript = string.Format("var minSize = '{0}'; var framesetName = '{1}'; var resizeVertical = {2}; var parentLevel = {3}; ",
                                              MinSize,
                                              FramesetName,
                                              (Vertical.ToString().ToLowerCSafe()),
                                              ParentLevel);

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "resizerVars", varsScript, true);

            if (RequestHelper.IsPostBack())
            {
                originalSize = Request.Params["originalsize"];
            }
        }
    }

    #endregion
}
