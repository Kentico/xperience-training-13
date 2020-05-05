using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_System_CacheDependencies : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (chkDependencies.Checked)
            {
                // Return together with default ones
                return CacheHelper.DEFAULT_CACHE_DEPENDENCIES + "\n" + txtDependencies.Value;
            }
            else
            {
                if ((string)txtDependencies.Value == "")
                {
                    // No dependencies
                    return CacheHelper.NO_CACHE_DEPENDENCIES;
                }
                else
                {
                    // Only specific dependencies
                    return txtDependencies.Value;
                }
            }
        }
        set
        {
            string val = (string)value;
            if (String.IsNullOrEmpty(val))
            {
                // Default cache dependencies
                chkDependencies.Checked = true;
                txtDependencies.TextArea.Text = "";
            }
            else if (val == CacheHelper.NO_CACHE_DEPENDENCIES)
            {
                // No cache dependencies
                chkDependencies.Checked = false;
                txtDependencies.TextArea.Text = "";
            }
            else
            {
                // Check if default is applied
                if (val.Contains(CacheHelper.DEFAULT_CACHE_DEPENDENCIES))
                {
                    chkDependencies.Checked = true;
                    val = val.Replace(CacheHelper.DEFAULT_CACHE_DEPENDENCIES, "").Trim();
                }

                txtDependencies.TextArea.Text = val;
            }
        }
    }


    /// <summary>
    /// Gets ClientID of the text area.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtDependencies.ValueElementID;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        chkDependencies.Text = GetString("CacheDependencies.UseCacheDependencies");
    }
}