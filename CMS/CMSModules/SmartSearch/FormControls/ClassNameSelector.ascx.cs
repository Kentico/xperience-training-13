using System;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_FormControls_ClassNameSelector : FormEngineUserControl
{
    #region "Variables"

    private bool dataLoaded = false;
    private bool mShowPleaseSelect = false;

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
            EnsureChildControls();
            base.Enabled = value;
            selObjects.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the control is used in a live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            EnsureChildControls();
            base.IsLiveSite = value;
            selObjects.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return selObjects.Value;
        }
        set
        {
            EnsureChildControls();
            selObjects.Value = value;
        }
    }


    /// <summary>
    /// Gets the uni selector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return selObjects;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether an extra item "(Please select)" will be displayed in the uniSelector's drop down list.
    /// </summary>
    public bool ShowPleaseSelect
    {
        get
        {
            return mShowPleaseSelect;
        }
        set
        {
            mShowPleaseSelect = value;
        }
    }


    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Load the data
        Reload(false);
    }


    /// <summary>
    /// Reloads the selector data.
    /// </summary>
    public void Reload(bool forceReload)
    {
        if (!dataLoaded || forceReload)
        {
            // Add "Please select" item into the drop down list
            if (ShowPleaseSelect)
            {
                selObjects.SpecialFields.Add(new SpecialField { Text = GetString("general.pleaseselect"), Value = String.Empty }); 
            }

            // Reload the uniSelector
            selObjects.Reload(forceReload);
            dataLoaded = true;
        }
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If the selector is not defined load the update panel container
        if (selObjects == null)
        {
            pnlUpdate.LoadContainer();
        }

        // Call base method
        base.CreateChildControls();
    }

    #endregion
}