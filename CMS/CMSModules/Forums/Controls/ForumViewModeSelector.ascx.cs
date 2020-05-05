using System;
using System.Web.UI.WebControls;

using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Controls_ForumViewModeSelector : ForumViewer
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the text which should be displayed before dropdown selector.
    /// </summary>
    public string Text
    {
        get
        {
            return ValidationHelper.GetString(ViewState["PrefixText"], "");
        }
        set
        {
            ViewState["PrefixText"] = value;
            litText.Text = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (ForumContext.CurrentState != ForumStateEnum.Thread)
        {
            Visible = false;
        }

        // WAI validation
        lblViewMode.ResourceString = "FlatForum.ViewMode";
        lblViewMode.Attributes.Add("style", "display: none;");

        // Fill the drop down list
        drpViewModeSelector.Items.Add(new ListItem(GetString("FlatForum.ModeThreaded"), FlatModeEnum.Threaded.ToString()));
        drpViewModeSelector.Items.Add(new ListItem(GetString("FlatForum.ModeNewest"), FlatModeEnum.NewestToOldest.ToString()));
        drpViewModeSelector.Items.Add(new ListItem(GetString("FlatForum.ModeOldest"), FlatModeEnum.OldestToNewest.ToString()));

        // Try to preselect value
        if (SessionHelper.GetValue("CMSForumViewMode") != null)
        {
            string mode = (string)SessionHelper.GetValue("CMSForumViewMode");
            if (mode != null)
            {
                ViewMode = ForumModes.GetFlatMode(mode);
                drpViewModeSelector.SelectedValue = ViewMode.ToString();
            }
        }
        else
        {
            // Try to copy forum viewer properties from parent
            CopyValuesFromParent(this);

            // Set mode from parent            
            drpViewModeSelector.SelectedValue = ViewMode.ToString();
        }
    }


    protected override void OnInit(EventArgs e)
    {
        // Set view mode
        if ((RequestHelper.IsPostBack()) && ((ForumContext.CurrentState == ForumStateEnum.Thread)))
        {
            SessionHelper.SetValue("CMSForumViewMode", Request.Params[drpViewModeSelector.UniqueID]);
        }
        base.OnInit(e);
    }

    #endregion
}