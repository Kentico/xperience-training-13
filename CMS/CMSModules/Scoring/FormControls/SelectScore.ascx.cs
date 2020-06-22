using System;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;

public partial class CMSModules_Scoring_FormControls_SelectScore : FormEngineUserControl
{
    #region "Variables & Properties"

    private int mSiteID;


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get => uniSelector.Enabled;
        set => uniSelector.Enabled = value;
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get => uniSelector.Value;
        set => uniSelector.Value = value;
    }


    /// <summary>
    /// Indicates whether the selector contains some options.
    /// </summary>
    public bool HasData => uniSelector.HasData;


    /// <summary>
    /// Current site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteID <= 0)
            {
                mSiteID = ValidationHelper.GetInteger(GetValue("SiteID"), SiteContext.CurrentSiteID);
            }
            return mSiteID;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing)
        {
            InitSelector();
        }
    }


    /// <summary>
    /// Initializes uniselector
    /// </summary>
    private void InitSelector()
    {
        uniSelector.WhereCondition = new WhereCondition().WhereEquals("ScoreEnabled", 1).ToString(true);
        uniSelector.AllowAll = ValidationHelper.GetBoolean(GetValue("AllowAll"), uniSelector.AllowAll);
        uniSelector.AllowEmpty = ValidationHelper.GetBoolean(GetValue("AllowEmpty"), uniSelector.AllowEmpty);
        uniSelector.Reload(true);
    }

    #endregion
}
