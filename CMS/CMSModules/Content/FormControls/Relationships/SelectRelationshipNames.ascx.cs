using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Relationships;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_FormControls_Relationships_SelectRelationshipNames : FormEngineUserControl
{
    #region "Private variables"

    private bool mAllowEmpty;
    private string mReturnColumnName = "RelationshipName";
    private int mSiteId = SiteContext.CurrentSiteID;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Enables or disables (empty) item in selector.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return mAllowEmpty;
        }
        set
        {
            mAllowEmpty = value;
            if (uniSelector != null)
            {
                uniSelector.AllowEmpty = value;
            }
        }
    }


    /// <summary>
    /// Indicates if ad-hoc relationship names should be hidden from the selection.
    /// </summary>
    public bool HideAdHocRelationshipNames
    {
        get;
        set;
    }


    /// <summary>
    /// Name is allowed for document relationships.
    /// </summary>
    public bool AllowedForDocuments
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowedForDocuments"), true);
        }
        set
        {
            SetValue("AllowedForDocuments", value);
        }
    }


    /// <summary>
    /// Name is allowed for object relationships.
    /// </summary>
    public bool AllowedForObjects
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowedForObjects"), true);
        }
        set
        {
            SetValue("AllowedForObjects", value);
        }
    }


    /// <summary>
    /// Determines which column should be returned as value.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            return mReturnColumnName;
        }
        set
        {
            mReturnColumnName = value;
            if (uniSelector != null)
            {
                uniSelector.ReturnColumnName = value;
            }
        }
    }


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
            uniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            if (uniSelector != null)
            {
                uniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets the current UniSelector instance.
    /// </summary>
    public UniSelector CurrentSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets ClientID of the CMSDropDownList with relationshipnames.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            uniSelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the site id. If set, only relationships of the site are displayed.
    /// </summary>
    public int SiteId
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        var condition = RelationshipNameInfoProvider.GetRelationshipNamesWhereCondition(AllowedForObjects, AllowedForDocuments, SiteId, !HideAdHocRelationshipNames);
        uniSelector.WhereCondition = condition.ToString(true);
        
        uniSelector.UseTypeCondition = HideAdHocRelationshipNames;
        uniSelector.AllowAll = ValidationHelper.GetBoolean(GetValue("AllowAll"), false);
        uniSelector.AllowEmpty = AllowEmpty;
        uniSelector.ReturnColumnName = ReturnColumnName;
        uniSelector.OrderBy = "RelationshipDisplayName";
        uniSelector.AllRecordValue = String.Empty;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Disable DDL if no relationships available
        if (uniSelector.HasData)
        {
            return;
        }

        uniSelector.DropDownSingleSelect.Items.Add(new ListItem(ResHelper.GetString("General.NoneAvailable"), ""));
        uniSelector.Enabled = false;
    }

    #endregion
}