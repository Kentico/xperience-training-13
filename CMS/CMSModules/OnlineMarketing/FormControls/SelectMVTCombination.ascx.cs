using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_FormControls_SelectMVTCombination : FormEngineUserControl
{
    #region "Variables"

    private int mPageTemplateID;
    private int mDocumentID;
    private bool mDataLoaded;
    private bool mUseQueryStringOnChange;
    private string mQueryStringKey = string.Empty;
    private bool mApplyDefaultCondition = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the combination id.
    /// </summary>
    /// <value></value>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return ucUniSelector.Value;
        }
        set
        {
            EnsureChildControls();
            ucUniSelector.Value = value;
        }
    }


    /// <summary>
    /// Return column name for uni-selector
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            EnsureChildControls();
            return ucUniSelector.ReturnColumnName;
        }
        set
        {
            EnsureChildControls();
            ucUniSelector.ReturnColumnName = value;
        }
    }


    /// <summary>
    /// All record value for uni-selector
    /// </summary>
    public string AllRecordValue
    {
        get
        {
            EnsureChildControls();
            return ucUniSelector.AllRecordValue;
        }
        set
        {
            EnsureChildControls();
            ucUniSelector.AllRecordValue = value;
        }
    }


    /// <summary>
    /// Where condition for uniselector
    /// </summary>
    public string WhereCondition
    {
        get
        {
            EnsureChildControls();
            return ucUniSelector.WhereCondition;
        }
        set
        {
            EnsureChildControls();
            ucUniSelector.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the page template ID.
    /// </summary>
    public int PageTemplateID
    {
        get
        {
            EnsureChildControls();
            return mPageTemplateID;
        }
        set
        {
            EnsureChildControls();
            mPageTemplateID = value;
        }
    }


    /// <summary>
    /// Gets or sets the document ID.
    /// </summary>
    public int DocumentID
    {
        get
        {
            EnsureChildControls();
            return mDocumentID;
        }
        set
        {
            EnsureChildControls();
            mDocumentID = value;
        }
    }


    /// <summary>
    /// Gets the uni selector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return ucUniSelector;
        }
    }


    /// <summary>
    /// Gets the drop down select control.
    /// </summary>
    public CMSDropDownList DropDownSelect
    {
        get
        {
            EnsureChildControls();
            return ucUniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets a value indicating whether this instance has data.
    /// </summary>
    public bool HasData
    {
        get
        {
            EnsureChildControls();
            return ucUniSelector.HasData;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to use query string and redirect when OnChange event is fired or if use a postback.
    /// </summary>
    public bool UseQueryStringOnChange
    {
        get
        {
            return mUseQueryStringOnChange;
        }
        set
        {
            mUseQueryStringOnChange = value;
        }
    }


    /// <summary>
    /// Enables/disables all (all) value
    /// </summary>
    public bool AllowAll
    {
        get
        {
            EnsureChildControls();
            return ucUniSelector.AllowAll;
        }
        set
        {
            EnsureChildControls();
            ucUniSelector.AllowAll = value;
        }
    }


    /// <summary>
    /// Enables/disables emtpy (none) value
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            EnsureChildControls();
            return ucUniSelector.AllowEmpty;
        }
        set
        {
            EnsureChildControls();
            ucUniSelector.AllowEmpty = value;
        }
    }


    /// <summary>
    /// If true, full postback is raised when item changed.
    /// </summary>
    public bool PostbackOnChange
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the query string key when the UseQueryStringOnChangeis set to true.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return mQueryStringKey;
        }
        set
        {
            mQueryStringKey = value;
        }
    }


    /// <summary>
    /// If true, default condition (NodeID, TemplateID) is applied for selector 
    /// </summary>
    public bool ApplyDefaultCondition
    {
        get
        {
            return mApplyDefaultCondition;
        }
        set
        {
            mApplyDefaultCondition = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (PostbackOnChange)
        {
            ucUniSelector.DropDownSingleSelect.AutoPostBack = true;
            ScriptManager scr = ScriptManager.GetCurrent(Page);
            scr.RegisterPostBackControl(ucUniSelector);
        }

        if (!mUseQueryStringOnChange)
        {
            ucUniSelector.DropDownSingleSelect.AutoPostBack = true;
            ucUniSelector.DropDownSingleSelect.CssClass = "form-control";
        }
        else if (!string.IsNullOrEmpty(mQueryStringKey))
        {
            string script = @"
function ChangeCombination() {
    var ddlEl = document.getElementById('" + ucUniSelector.DropDownSingleSelect.ClientID + @"');
    if (ddlEl != null) {
        var value = ddlEl.options[ddlEl.selectedIndex].value;
        var url = '" + URLHelper.RemoveUrlParameter(URLHelper.RemoveUrlParameter(RequestContext.CurrentURL, "mvtitemupdated"), "mvtiszoneupdated") + @"';
        url = AddUrlParameter(url, 'combinationid', value, true);
        document.location.replace(url);
    }
}";

            ScriptHelper.RegisterStartupScript(this, typeof (string), "mvtSelectCombination", ScriptHelper.GetScript(script));

            // Use query string instead of postback
            ucUniSelector.OnBeforeClientChanged = "ChangeCombination();";
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    /// <param name="forceReload">If true, data are loaded in all cases</param>
    public void ReloadData(bool forceReload)
    {
        if (forceReload || !mDataLoaded)
        {
            if (ApplyDefaultCondition)
            {
                string where = SqlHelper.AddWhereCondition(ucUniSelector.WhereCondition, "MVTCombinationPageTemplateID =" + PageTemplateID);
                where = SqlHelper.AddWhereCondition(where, "(MVTCombinationDocumentID = " + DocumentID + ") OR (MVTCombinationDocumentID IS NULL)");
                ucUniSelector.WhereCondition = where;
            }
            ucUniSelector.IsLiveSite = IsLiveSite;

            ucUniSelector.Reload(forceReload);
            mDataLoaded = true;
        }
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // Call base method
        base.CreateChildControls();

        // If selector is not defined, load update panel container
        if (ucUniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }
    }

    #endregion
}