using System;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSModules_Workflows_FormControls_ScopeDefinition : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return pathElem.Enabled;
        }
        set
        {
            pathElem.Enabled = value;
            rbChildren.Enabled = value;
            rbDoc.Enabled = value;
            rbDocAndChildren.Enabled = value;
        }
    }


    /// <summary>
    /// Client ID of primary input control. If not explicitly set, first client ID of inner control of the form control is returned.
    /// </summary>
    public override string InputClientID
    {
        get
        {
            return pathElem.PathTextBox.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Get current path
            var path = ValidationHelper.GetString(pathElem.Value, String.Empty);
            
            // Format path
            pathElem.Value = FormatPath(path);

            return pathElem.Value;
        }
        set
        {
            var path = ValidationHelper.GetString(value, String.Empty);

            // Initialize inner controls
            InitializeInnerControls(path);
        }
    }


    /// <summary>
    /// Current site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return GetValue("SiteID", SiteContext.CurrentSiteID);
        }
        set
        {
            SetValue("SiteID", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Get other values for fields
    /// </summary>
    public override object[,] GetOtherValues()
    {
        object[,] values = new object[1, 2];
        values[0, 0] = "ScopeExcludeChildren";
        values[0, 1] = !rbChildren.Checked && rbDoc.Checked;
        return values;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup inner control
        pathElem.SiteID = SiteID;
    }


    /// <summary>
    /// Checks if control values are valid
    /// </summary>
    public override bool IsValid()
    {
        // Check selected path
        var path = ValidationHelper.GetString(pathElem.Value, null);
        if (string.IsNullOrEmpty(path))
        {
            ValidationError = GetString("Development-Workflow_Scope_Edit.RequiresStartingAliasPath");
            return false;
        }

        return base.IsValid();
    }


    /// <summary>
    /// Formats path
    /// </summary>
    /// <param name="path">Node alias path</param>
    private string FormatPath(string path)
    {
        // Get single node path
        path = TreePathUtils.EnsureSingleNodePath(path);

        // Ensure slash at the beginning
        if (!string.IsNullOrEmpty(path) && !path.StartsWithCSafe("/"))
        {
            path = "/" + path;
        }

        // Include children if set
        if (rbChildren.Checked)
        {
            path = ((path != null) ? path.TrimEnd('/') : "") + "/%";
        }

        return path;
    }


    /// <summary>
    /// Initialize inner controls
    /// </summary>
    /// <param name="path">Node alias path</param>
    private void InitializeInnerControls(string path)
    {
        // Ensure single node path
        pathElem.Value = TreePathUtils.EnsureSingleNodePath(path);

        // Only child documents
        if (path.EndsWithCSafe("/%"))
        {
            rbChildren.Checked = true;
        }
        else
        {
            LoadOtherValues();
        }
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // Only document
        if (ValidationHelper.GetBoolean(Form.Data["ScopeExcludeChildren"], false))
        {
            rbDoc.Checked = true;
        }
            // Document including children
        else
        {
            rbDocAndChildren.Checked = true;
        }
    }

    #endregion
}