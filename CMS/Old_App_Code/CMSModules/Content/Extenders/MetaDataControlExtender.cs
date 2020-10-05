using System;
using System.Collections.Generic;
using System.Linq;

using CMS;
using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

[assembly: RegisterCustomClass("MetaDataControlExtender", typeof(MetaDataControlExtender))]

/// <summary>
/// Control extender for metadata and tags tabs in document properties.
/// </summary>
public class MetaDataControlExtender : ControlExtender<CMSForm>
{
    #region "Variables"

    private const string SUFFIX_INHERIT = "Inherit";
    private bool pageOptionsVisible;
    private bool tagsVisible;
    private Dictionary<string, CMSCheckBox> checkboxes = new Dictionary<string, CMSCheckBox>();
    private Dictionary<string, object> inheritedValues = new Dictionary<string, object>();
    private UniSelector mTagGroupSelector;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the current node.
    /// </summary>
    private TreeNode Node
    {
        get
        {
            return (TreeNode)Control.EditedObject;
        }
    }


    /// <summary>
    /// Gets the tag group selector control.
    /// </summary>
    private UniSelector TagGroupSelector
    {
        get
        {
            return mTagGroupSelector ?? (mTagGroupSelector = Control.FieldControls["DocumentTagGroupID"] as UniSelector);
        }
    }


    /// <summary>
    /// Gets or sets module name for UI element check.
    /// </summary>
    public string UIModuleName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets page settings section name for UI element check.
    /// </summary>
    public string UIMetadataElementName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets tags section name for UI element check.
    /// </summary>
    public string UITagsElementName
    {
        get;
        set;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// OnInit event handler.
    /// </summary>
    public override void OnInit()
    {
        Control.OnBeforeSave += Control_OnBeforeSave;
        Control.OnAfterSave += Control_OnAfterSave;
        Control.Page.Load += Page_Load;
        Control.Page.PreRender += Page_PreRender;
    }


    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node == null)
        {
            return;
        }

        bool nodeIsRoot = Node.IsRoot();

        pageOptionsVisible = !string.IsNullOrEmpty(UIMetadataElementName) && !string.IsNullOrEmpty(UIModuleName)
            && MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement(UIModuleName, UIMetadataElementName);
        if (pageOptionsVisible && !nodeIsRoot)
        {
            SetupInheritCheckbox("DocumentPageTitle");
            SetupInheritCheckbox("DocumentPageDescription");
            SetupInheritCheckbox("DocumentPageKeyWords");
        }

        tagsVisible = !string.IsNullOrEmpty(UITagsElementName) && !string.IsNullOrEmpty(UIModuleName)
            && MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement(UIModuleName, UITagsElementName);
        if (tagsVisible)
        {
            if (!nodeIsRoot)
            {
                SetupInheritCheckbox("DocumentTagGroupID");
            }

            InitializeTagSelector();
        }

        if (!pageOptionsVisible && !tagsVisible)
        {
            // Redirect to info message if no UI available
            URLHelper.Redirect(AdministrationUrlHelper.GetInformationUrl("uiprofile.uinotavailable"));
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if ((Node == null) || (TagGroupSelector == null))
        {
            return;
        }

        bool noneSelected = ValidationHelper.GetInteger(TagGroupSelector.Value, 0) == 0;

        object val;
        inheritedValues.TryGetValue("DocumentTagGroupID", out val);
        int parentValue = ValidationHelper.GetInteger(val, 0);

        // Allow empty value in selector if node has no tag group selected and parent tag group also isn't set
        TagGroupSelector.SetValue("AllowEmpty", noneSelected || ((Node.DocumentTagGroupID == 0) && (parentValue == 0)));

        // Get all groups from original page site ID
        TagGroupSelector.SetValue("WhereCondition", "[TagGroupSiteID] = " + Node.OriginalNodeSiteID);

        if (!TagGroupSelector.HasData)
        {
            // Hide tag module controls and show information if no tag group exists
            Control.MessagesPlaceHolder.ShowInformation(ResHelper.GetString("PageProperties.TagsInfo"));
            Control.FieldsToHide.Add("DocumentTagGroupID");
            Control.FieldsToHide.Add("DocumentTagGroupIDInherit");
            Control.FieldsToHide.Add("DocumentTags");

            var lblNoTags = ControlsHelper.GetChildControl(Control, typeof(LocalizedLabel), "lblNoTags") as LocalizedLabel;
            lblNoTags.Visible = true;
        }
        else
        {
            Control.FieldControls["DocumentTags"].Enabled = !noneSelected;
            if (noneSelected)
            {
                // Clear tags if no tag group selected
                Control.FieldControls["DocumentTags"].Text = String.Empty;
            }
        }
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    protected void Control_OnBeforeSave(object sender, EventArgs e)
    {
        if (!Node.IsRoot() && pageOptionsVisible)
        {
            // Set page options properties
            SetField(Node, "DocumentPageTitle");
            SetField(Node, "DocumentPageDescription");
            SetField(Node, "DocumentPageKeyWords");
        }

        if (tagsVisible)
        {
            // Special handling for tag group selector
            if (TagGroupSelector.Text != "0")
            {
                // Set tag group property
                SetField(Node, "DocumentTagGroupID");
            }
            else
            {
                Node.SetValue("DocumentTagGroupID", null);
                // Clear tags if tag group is not set
                Node.DocumentTags = null;
            }
        }
    }


    /// <summary>
    /// Ensures special handling for DocumentTagGroup field on subpages. If any tag group is selected, inherited configuration is set.
    /// </summary>
    protected void Control_OnAfterSave(object sender, EventArgs e)
    {
        if (!tagsVisible || (Node.DocumentTagGroupID > 0) || Node.IsRoot())
        {
            return;
        }

        CMSCheckBox checkBox = GetCheckBox("DocumentTagGroupID");
        FormEngineUserControl mainControl = Control.FieldControls["DocumentTagGroupID"];
        if ((checkBox != null) && (mainControl != null))
        {
            checkBox.Checked = true;
            mainControl.Enabled = false;
        }
    }


    /// <summary>
    /// CheckedChanged event handler.
    /// </summary>
    protected void checkBox_CheckedChanged(object sender, EventArgs e)
    {
        CMSCheckBox checkBox = (CMSCheckBox)sender;

        // Get field name that can be set as inherited by checkbox
        KeyValuePair<string, CMSCheckBox> item = checkboxes.FirstOrDefault(x => x.Value.ClientID == checkBox.ClientID);
        if (item.Key != null)
        {
            FormEngineUserControl mainControl = Control.FieldControls[item.Key];

            if (checkBox.Checked)
            {
                // Value is inherited
                mainControl.Enabled = false;

                var inheritedValue = Node.GetInheritedValue(item.Key, false);
                inheritedValues.Add(item.Key, inheritedValue);
                mainControl.Value = inheritedValue;
            }
            else
            {
                // Text area is enabled
                mainControl.Enabled = true;
            }

            if (item.Key == "DocumentTagGroupID")
            {
                EnableTagSelector();
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sets value to TreeNode property given by field name.
    /// </summary>
    /// <param name="node">Current instance of edited node</param>
    /// <param name="fieldName">Name of field which can have inherited value</param>
    private void SetField(TreeNode node, string fieldName)
    {
        node.SetValue(fieldName, null);

        CMSCheckBox chk = GetCheckBox(fieldName);
        if (!chk.Checked)
        {
            // Set not inherited value
            node.SetValue(fieldName, Control.FieldControls[fieldName].Value);
        }
    }


    /// <summary>
    /// Setups checkbox for inheriting value.
    /// </summary>
    /// <param name="fieldName">Name of field which can have inherited value</param>
    private void SetupInheritCheckbox(string fieldName)
    {
        CMSCheckBox checkBox = GetCheckBox(fieldName);
        if (checkBox == null)
        {
            return;
        }

        checkBox.AutoPostBack = true;
        checkBox.CheckedChanged += checkBox_CheckedChanged;

        // Check if node has saved some value
        if ((Node.GetValue(fieldName) == null) && (!RequestHelper.IsPostBack()))
        {
            checkBox.Checked = true;

            // Init inherited value
            FormEngineUserControl mainControl = Control.FieldControls[fieldName];
            if (!String.IsNullOrEmpty(Node.NodeSiteName) && (mainControl != null))
            {
                var inheritedValue = Node.GetInheritedValue(fieldName, false);
                inheritedValues.Add(fieldName, inheritedValue);
                mainControl.Text = ValidationHelper.GetString(inheritedValue, "");
                mainControl.Enabled = false;
            }
        }
    }


    /// <summary>
    /// Gets CheckBox control for given field name
    /// </summary>
    /// <param name="fieldName">Name of field which can have inherited value</param>
    private CMSCheckBox GetCheckBox(string fieldName)
    {
        if (checkboxes.ContainsKey(fieldName))
        {
            // Return from dictionary
            return checkboxes[fieldName];
        }

        FormEngineUserControl control = Control.FieldControls[fieldName + SUFFIX_INHERIT];
        if (control != null)
        {
            // Get checkbox from form control
            CMSCheckBox checkbox = ControlsHelper.GetChildControl<CMSCheckBox>(control);
            if (checkbox != null)
            {
                checkboxes[fieldName] = checkbox;
                return checkbox;
            }
        }

        return null;
    }


    /// <summary>
    /// Initializes tag selector by selected value of tag group selector.
    /// </summary>
    private void InitializeTagSelector()
    {
        int groupId = ValidationHelper.GetInteger(TagGroupSelector.Value, 0);
        if (groupId > 0)
        {
            // Set GroupId for tag selector control
            Control.FieldControls["DocumentTags"].SetValue("TagGroupID", groupId);
        }
    }


    /// <summary>
    /// Enables tag selector by selected value of tag group selector.
    /// Tag selector is enabled if tag group selector has selected value.
    /// </summary>
    private void EnableTagSelector()
    {
        int groupId = ValidationHelper.GetInteger(TagGroupSelector.Value, 0);
        if (groupId > 0)
        {
            Control.FieldControls["DocumentTags"].Enabled = true;
        }
    }

    #endregion
}