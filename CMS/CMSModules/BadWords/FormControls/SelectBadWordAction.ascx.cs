using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Protection;


public partial class CMSModules_BadWords_FormControls_SelectBadWordAction : FormEngineUserControl
{
    #region "Variables"

    private string mAction = string.Empty;
    private bool mAllowNoSelection = false;
    private string mNoSelectionText = null;
    private bool mReloadDataOnPostback = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            drpSelectAction.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates if the no action option should be displayed.
    /// </summary>
    public bool AllowNoSelection
    {
        get
        {
            return mAllowNoSelection;
        }
        set
        {
            mAllowNoSelection = value;
        }
    }


    /// <summary>
    /// No selection text.
    /// </summary>
    public string NoSelectionText
    {
        get
        {
            return mNoSelectionText;
        }
        set
        {
            mNoSelectionText = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpSelectAction.SelectedValue, string.Empty);
        }
        set
        {
            mAction = ValidationHelper.GetString(value, string.Empty);
            ReloadData();
        }
    }


    /// <summary>
    /// Indicates whether enable dropdown list autopostback.
    /// </summary>
    public bool AllowAutoPostBack
    {
        get
        {
            return drpSelectAction.AutoPostBack;
        }
        set
        {
            drpSelectAction.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Indicates whether data should be reloaded on PostBack.
    /// </summary>
    public bool ReloadDataOnPostback
    {
        get
        {
            return mReloadDataOnPostback;
        }
        set
        {
            mReloadDataOnPostback = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the DropDownList with badword action.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpSelectAction.ClientID;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        mNoSelectionText = GetString("general.selectaction");
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        if ((RequestHelper.IsPostBack() && ReloadDataOnPostback) || !RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    public void ReloadData()
    {
        // Reload dropdown list
        drpSelectAction.ClearSelection();
        if (drpSelectAction.Items.Count == 0)
        {
            if (AllowNoSelection)
            {
                drpSelectAction.Items.Add(new ListItem(NoSelectionText, string.Empty));
            }
            drpSelectAction.Items.Add(new ListItem(GetString("general.remove"), (((int)BadWordActionEnum.Remove)).ToString()));
            drpSelectAction.Items.Add(new ListItem(GetString("general.replace"), (((int)BadWordActionEnum.Replace)).ToString()));
            drpSelectAction.Items.Add(new ListItem(GetString("BadWords_Edit.ReportAbuse"), (((int)BadWordActionEnum.ReportAbuse)).ToString()));
            drpSelectAction.Items.Add(new ListItem(GetString("BadWords_Edit.RequestModeration"), (((int)BadWordActionEnum.RequestModeration)).ToString()));
            drpSelectAction.Items.Add(new ListItem(GetString("Security.Deny"), (((int)BadWordActionEnum.Deny)).ToString()));
        }

        // Preselect value
        if (drpSelectAction.Items.Count != 0)
        {
            ListItem selectedItem = drpSelectAction.Items.FindByValue(mAction);
            if (selectedItem != null)
            {
                selectedItem.Selected = true;
            }
            else
            {
                drpSelectAction.SelectedIndex = 0;
            }
        }
    }

    #endregion
}