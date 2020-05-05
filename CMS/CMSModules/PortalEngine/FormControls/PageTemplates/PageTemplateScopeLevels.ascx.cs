using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_PortalEngine_FormControls_PageTemplates_PageTemplateScopeLevels : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets tree path - if set is created from TreePath.
    /// </summary>
    public string TreePath
    {
        get
        {
            return treeElem.TreePath;
        }
        set
        {
            treeElem.TreePath = value;
        }
    }


    /// <summary>
    /// Gets or sets Level, levels are rendered only if TreePath is not set. 
    /// </summary>
    public int Level
    {
        get
        {
            return treeElem.Level;
        }
        set
        {
            treeElem.Level = value;
        }
    }


    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (radAllowAll.Checked)
            {
                treeElem.Value = string.Empty;
                ltlScript.Text = ScriptHelper.GetScript("document.getElementById('treeSpan').style.display = \"none\";");
                return string.Empty;
            }
            return treeElem.Value;
        }
        set
        {
            treeElem.Value = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        radAllowAll.Text = GetString("pagetemplates.scopes.All");
        radSelect.Text = GetString("pagetemplates.scopes.Select");

        if (!RequestHelper.IsPostBack())
        {
            if (ValidationHelper.GetString(Value, string.Empty) != string.Empty)
            {
                radSelect.Checked = true;
                ltlScript.Text = ScriptHelper.GetScript("document.getElementById('treeSpan').style.display = \"inline\";");
            }
            else
            {
                radAllowAll.Checked = true;
            }

            radAllowAll.Attributes.Add("onclick", "ShowOrHideTree(false);");
            radSelect.Attributes.Add("onclick", "ShowOrHideTree(true);");
        }
        else
        {
            // Show/hide tree after postback
            ltlScript.Text = ScriptHelper.GetScript("ShowOrHideTree(" + radSelect.Checked.ToString().ToLowerCSafe() + ");");
        }
    }

    #endregion
}