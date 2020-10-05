using System;
using System.Collections;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Taxonomy;


public partial class CMSModules_Content_FormControls_Tags_TagSelector : FormEngineUserControl, ICallbackEventHandler
{
    #region "Variables"

    private bool mEnabled = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets dialog identifier.
    /// </summary>
    private string DialogIdentifier
    {
        get
        {
            if (String.IsNullOrEmpty(hdnDialogIdentifier.Value))
            {
                if (String.IsNullOrEmpty(Request.Form[hdnDialogIdentifier.UniqueID]))
                {
                    hdnDialogIdentifier.Value = Guid.NewGuid().ToString();
                }
            }

            return HTMLHelper.HTMLEncode(hdnDialogIdentifier.Value);
        }
    }


    /// <summary>
    /// Enable/disable control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
            btnSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return TagHelper.GetTagsForSave(txtTags.Text.Trim());
        }
        set
        {
            txtTags.Text = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // Currently does not support loading other values explicitly
    }


    /// <summary>
    /// Make sure that the DocumentTagGroupID always reflects the Group ID specified in form control.
    /// Otherwise it will use the default Group ID specified in metadata.
    /// </summary>
    public override object[,] GetOtherValues()
    {
        object[,] values = new object[1, 2];
        values[0, 0] = "DocumentTagGroupID";

        // Make sure that when no GroupId is set,
        // the null value will be used
        if (GroupId <= 0)
        {
            values[0, 1] = null;
        }
        else
        {
            values[0, 1] = GroupId;
        }

        return values;
    }


    /// <summary>
    /// Tag Group ID.
    /// </summary>
    public int GroupId
    {
        get
        {
            int mGroupId = ValidationHelper.GetInteger(GetValue("TagGroupID"), 0);
            if ((mGroupId == 0) && (Form != null))
            {
                TreeNode node = (TreeNode)Form.EditedObject;

                // When inserting new document
                if (Form.IsInsertMode)
                {
                    var parent = Form.ParentObject as TreeNode;
                    if (parent != null)
                    {
                        // Get path and groupID of the parent node
                        mGroupId = parent.DocumentTagGroupID;
                        // If nothing found try get inherited value
                        if (mGroupId == 0)
                        {
                            mGroupId = ValidationHelper.GetInteger(parent.GetInheritedValue("DocumentTagGroupID", false), 0);
                        }
                    }
                }
                // When editing existing document
                else if (node != null)
                {
                    // Get path and groupID of the parent node
                    mGroupId = node.DocumentTagGroupID;
                    // If nothing found try get inherited value
                    if (mGroupId == 0)
                    {
                        mGroupId = ValidationHelper.GetInteger(node.GetInheritedValue("DocumentTagGroupID", false), 0);
                    }
                }
            }

            return mGroupId;
        }
        set
        {
            SetValue("TagGroupID", value);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Init event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        // Ensure the script manager
        ControlsHelper.EnsureScriptManager(Page);

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterScripts();

        btnSelect.Attributes.Add("onclick", Page.ClientScript.GetCallbackEventReference(this, "document.getElementById('" + txtTags.ClientID + "').value", "TS_SelectionDialogReady_" + ClientID, string.Empty) + "; return false;");
        btnSelect.Text = GetString("general.select");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (GroupId == 0)
        {
            Enabled = false;
            btnSelect.ToolTip = ResHelper.GetString("tags.tagsselector.assigntaggroup");
            txtTags.ToolTip = ResHelper.GetString("tags.tagsselector.assigntaggroup");
        }

        // Enable / Disable control
        txtTags.Enabled = Enabled;
        btnSelect.Enabled = Enabled;  
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Registers required scripts
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterStartupScript(this, typeof(string), "TagSelector_" + ClientID, ScriptHelper.GetScript(GetTagScript()));
    }


    /// <summary>
    /// Returns tag JS script.
    /// </summary>
    private string GetTagScript()
    {
        return $@"
function TS_SelectionDialogReady_{ClientID}(url, context) {{
    modalDialog(url, 'TagSelector', 790, 670)
}}

function TS_SetTagsToTextBox(textBoxId, tagString) {{
    if (textBoxId != '') {{
        var textbox = document.getElementById(textBoxId);
        if (textbox != null){{
            textbox.value = decodeURI(tagString);
            if (window.Changed != null) {{
                window.Changed();
            }}
        }}
    }}
}}";
    }

    #endregion


    #region "Callback handling"

    private string callbackResult;

    /// <summary>
    /// Raises the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        var p = new Hashtable();

        p["textbox"] = txtTags.ClientID;
        p["group"] = GroupId;
        p["tags"] = eventArgument;

        WindowHelper.Add(DialogIdentifier, p);

        var url = "~/CMSFormControls/Selectors/TagSelector.aspx";
        url = URLHelper.AddParameterToUrl(url, "params", DialogIdentifier);

        callbackResult = ResolveUrl(url);
    }


    /// <summary>
    /// Prepares the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        return callbackResult;
    }

    #endregion

}