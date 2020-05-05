using System;
using System.Collections;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Objects_FormControls_Cloning_CMS_FormUserControlSettings : CloneSettingsControl
{
    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            var control = InfoToClone as FormUserControlInfo;
            if (control != null)
            {
                txtFileName.Text = FileHelper.GetUniqueFileName(control.UserControlFileName);
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[FormUserControlInfo.OBJECT_TYPE + ".filename"] = txtFileName.Text;
        result[FormUserControlInfo.OBJECT_TYPE + ".files"] = chkFiles.Checked;
        return result;
    }


    /// <summary>
    /// Indicates whether advanced clone options should be displayed.
    /// </summary>
    public override bool DisplayControl
    {
        get
        {
            // Show control only for ASCX based form controls
            var control = InfoToClone as FormUserControlInfo;
            return (control != null && !String.IsNullOrEmpty(control.UserControlFileName) && !"inherited".Equals(control.UserControlFileName, StringComparison.OrdinalIgnoreCase));
        }
    }
}