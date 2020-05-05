using System;
using System.Web.UI;

using CMS.FormEngine.Web.UI;


public partial class CMSModules_Newsletters_FormControls_NewsletterTemplateSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return uniNewsletterTemplate.Enabled;
        }
        set
        {
            uniNewsletterTemplate.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniNewsletterTemplate.Value;
        }
        set
        {
            EnsureChildControls();
            uniNewsletterTemplate.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return uniNewsletterTemplate.WhereCondition;
        }
        set
        {
            uniNewsletterTemplate.WhereCondition = value;
        }
    }


    /// <summary>
    /// Allows to set AutoPostBack property to simple-select drop down list.
    /// </summary>
    public bool AutoPostBack
    {
        private get;
        set;
    }


    /// <summary>
    /// 'On selection changed' event.
    /// </summary>
    public event EventHandler OnSelectionChaned;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        uniNewsletterTemplate.OrderBy = "TemplateDisplayName";
        if ((uniNewsletterTemplate.DropDownSingleSelect != null) && (AutoPostBack))
        {
            // Allow auto-postback to dropdown list and register event handler
            uniNewsletterTemplate.DropDownSingleSelect.AutoPostBack = AutoPostBack;
            uniNewsletterTemplate.OnSelectionChanged += new EventHandler(uniNewsletterTemplate_OnSelectionChanged);

            ScriptManager pageScriptManager = ScriptManager.GetCurrent(Page);
            if (pageScriptManager != null)
            {
                // Register dropdown list as a trigger for postback (selector uses update panel by default)
                pageScriptManager.RegisterPostBackControl(uniNewsletterTemplate.DropDownSingleSelect);
            }
        }
    }


    protected void uniNewsletterTemplate_OnSelectionChanged(object sender, EventArgs e)
    {
        // Update selected value and raise event
        Value = uniNewsletterTemplate.DropDownSingleSelect.SelectedValue;

        if (OnSelectionChaned != null)
        {
            OnSelectionChaned(sender, e);
        }
    }


    /// <summary>
    /// Reloads the selector's data.
    /// </summary>
    /// <param name="forceReload">Indicates whether data should be forcibly reloaded</param>
    public void Reload(bool forceReload)
    {
        uniNewsletterTemplate.Reload(forceReload);
    }

    #endregion
}