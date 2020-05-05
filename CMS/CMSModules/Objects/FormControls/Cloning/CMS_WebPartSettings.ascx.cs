using System;
using System.Collections;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Objects_FormControls_Cloning_CMS_WebPartSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Returns close script which should be run when cloning is successfully done.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            string script = String.Empty;
            string refreshLink = UIContextHelper.GetElementUrl("cms.design", "Development.Webparts", false);
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


    /// <summary>
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return WebPartLayoutInfo.OBJECT_TYPE;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        lblFiles.ToolTip = GetString("clonning.settings.webpart.files.tooltip");
        lblFileName.ToolTip = GetString("clonning.settings.webpart.filename.tooltip");
        lblWebPartCategory.ToolTip = GetString("clonning.settings.webpart.category.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            var wpi = InfoToClone as WebPartInfo;
            if (wpi != null)
            {
                if (wpi.WebPartParentID > 0)
                {
                    WebPartInfo wparent = WebPartInfoProvider.GetWebPartInfo(wpi.WebPartParentID);
                    if (wparent != null)
                    {
                        // Hide files box for inherited webparts
                        plcFiles.Visible = false;
                    }
                }

                if (plcFiles.Visible)
                {
                    string filePath = wpi.WebPartFileName;
                    bool isRooted = filePath.StartsWith("~/", StringComparison.Ordinal);
                    if (!isRooted)
                    {
                        // Get the rooted path
                        filePath = WebPartInfoProvider.WebPartsDirectory + "/" + filePath.TrimStart('/');
                    }
                    // Ensure unique file name
                    filePath = FileHelper.GetUniqueFileName(filePath);
                    if (!isRooted)
                    {
                        // Get back the path relative to the web parts directory
                        filePath = filePath.Substring(WebPartInfoProvider.WebPartsDirectory.Length).TrimStart('/');
                    }
                    txtFileName.Text = filePath;
                }

                drpWebPartCategories.Value = wpi.WebPartCategoryID.ToString();

                if (SystemContext.IsPrecompiledWebsite)
                {
                    txtFileName.Text = wpi.WebPartFileName;
                    chkFiles.Checked = false;

                    txtFileName.Enabled = chkFiles.Enabled = false;
                    txtFileName.ToolTip = chkFiles.ToolTip = GetString("general.copyfiles.precompiled");
                }
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[WebPartInfo.OBJECT_TYPE + ".categoryid"] = drpWebPartCategories.Value;
        result[WebPartInfo.OBJECT_TYPE + ".filename"] = txtFileName.Text;
        result[WebPartInfo.OBJECT_TYPE + ".files"] = chkFiles.Checked;
        result[WebPartInfo.OBJECT_TYPE + ".appthemes"] = chkAppThemes.Checked;
        result[WebPartInfo.OBJECT_TYPE + ".layouts"] = chkWebpartLayouts.Checked;
        return result;
    }

    #endregion
}