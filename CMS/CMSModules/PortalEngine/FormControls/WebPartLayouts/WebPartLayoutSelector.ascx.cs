using System;

using CMS.FormEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_FormControls_WebPartLayouts_WebPartLayoutSelector : FormEngineUserControl
{
    #region "Variables"

    #endregion


    #region "Properties"

    /// <summary>
    /// Value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniselect.Value;
        }
        set
        {
            uniselect.Value = value;
        }
    }


    /// <summary>
    /// Whether (New) item is shown.
    /// </summary>
    public bool ShowNewItem
    {
        get;
        set;
    }


    /// <summary>
    /// Whehter (Defualt) item is shown.
    /// </summary>
    public bool ShowDefaultItem
    {
        get;
        set;
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return uniselect.WhereCondition;
        }
        set
        {
            uniselect.WhereCondition = value;
        }
    }


    /// <summary>
    /// UniSelector control
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniselect;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load event.
    /// </summary>    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniselect.StopProcessing = true;
            return;
        }

        var specialFields = new SpecialFieldsDefinition(null, FieldInfo, ContextResolver);

        if (ShowDefaultItem)
        {
            specialFields.Add(new SpecialField { Text = GetString("WebPartPropertise.Default"), Value = "|default|" });
        }

        if (ShowNewItem)
        {
            specialFields.Add(new SpecialField { Text = GetString("WebPartPropertise.New"), Value = "|new|" });
        }

        uniselect.SpecialFields = specialFields;
        uniselect.DropDownSingleSelect.AutoPostBack = true;
        uniselect.IsLiveSite = IsLiveSite;
    }


    /// <summary>
    /// On changed.
    /// </summary>
    protected void uniselect_OnSelectionChanged(object sender, EventArgs e)
    {
        RaiseOnChanged();
    }

    #endregion
}