using System;
using System.Collections;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Objects_FormControls_Cloning_CMS_PageTemplateSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Returns close script which should be run when cloning is sucessfully done.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            string script = String.Empty;
            string refreshLink = UIContextHelper.GetElementUrl("cms.design", "Development.PageTemplates", false);
            refreshLink = URLHelper.AddParameterToUrl(refreshLink, "objectid", "{0}");

            if (QueryHelper.Contains("reloadall"))
            {
                script = "wopener.location = '" + refreshLink + "';";
            }
            else
            {
                script += "wopener.parent.parent.location.href ='" + refreshLink + "';";
            }
            script += "CloseDialog();";
            return script;
        }
    }


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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblFiles.ToolTip = GetString("clonning.settings.layouts.appthemesfolder.tooltip");
        lblTemplateScope.ToolTip = GetString("clonning.settings.pagetemplate.templatescope.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[PageTemplateInfo.OBJECT_TYPE + ".appthemes"] = chkFiles.Checked;
        result[PageTemplateInfo.OBJECT_TYPE + ".templatecope"] = chkTemplateScope.Checked;
        return result;
    }

    #endregion
}