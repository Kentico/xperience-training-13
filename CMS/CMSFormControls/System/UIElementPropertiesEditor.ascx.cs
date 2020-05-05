using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;


public partial class CMSFormControls_System_UIElementPropertiesEditor : FormEngineUserControl
{
    #region "Compare class"

    /// <summary>
    /// Compare class
    /// </summary>
    public class CustomStringComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            return String.Compare((String)((DictionaryEntry)x).Key, (String)((DictionaryEntry)y).Key, StringComparison.InvariantCulture);
        }
    }

    #endregion


    #region "Variables"

    XmlData data = new XmlData();
    private bool? mIsValid = null;
    private List<Label> labels = new List<Label>();
    private List<TextBox> textboxes = new List<TextBox>();
    private String keyRegExp = "^(?:[A-Za-z_\\-]+)([.A-Za-z0-9_\\-]+)*$";
    private Regex mReg;
    private bool isNewValid = true;
    private String invalidKey = String.Empty;
    private const String INVALIDTOKEN = "__invalidtoken__";

    #endregion


    #region "Properties"

    /// <summary>
    /// Regex for key validation
    /// </summary>
    private Regex Reg
    {
        get
        {
            if (mReg == null)
            {
                mReg = new Regex(keyRegExp);
            }

            return mReg;
        }
    }


    /// <summary>
    /// String representation (XML format) of collection
    /// </summary>
    public override object Value
    {
        get
        {
            // Get data from Request
            LoadData();

            // Regenerate table for new items added by save action directly
            GenerateTable();
            GenerateNewRow();

            return data.GetData();
        }
        set
        {
            if (!RequestHelper.IsPostBack())
            {
                String val = ValidationHelper.GetString(value, String.Empty);
                data.LoadData(val);
            }
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterScript();

        // For postback, load data based on stored in hidden control
        if (RequestHelper.IsPostBack())
        {
            LoadData();
        }

        // Handle postbacks for custom data control
        String target = Request[Page.postEventSourceID];
        String argument = Request[Page.postEventArgumentID];

        if (!String.IsNullOrEmpty(argument) && (target == tblData.UniqueID))
        {

            // Delete
            if (argument.StartsWithCSafe("delete_"))
            {
                String key = argument.Substring("delete_".Length);

                // If key has recently changed, use proper one from request
                String rightKey = Request.Form[UniqueID + "$tk" + key];
                data.Remove(rightKey);
            }

            // Add
            if (argument == "add")
            {
                String key = Request[UniqueID + "$utk_newkey"];
                String value = Request[UniqueID + "$utv_newvalue"];

                if (!IsValidKey(key))
                {
                    mIsValid = false;
                    isNewValid = false;
                }
                else
                {
                    data.SetValue(key, value);
                }
            }
        }
    }


    /// <summary>
    /// Returns true, if all keys are valid
    /// </summary>
    private bool IsValidInternal()
    {
        // Test for all textbox keys
        foreach (String key in Request.Form.AllKeys)
        {
            if (!String.IsNullOrEmpty(key))
            {
                if (key.StartsWithCSafe(UniqueID + "$tk"))
                {
                    String newKey = Request.Form[key];
                    if (!IsValidKey(newKey))
                    {
                        ShowError(String.Format(GetString("uicontext.editor.invalidkey"), newKey));
                        mIsValid = false;
                        return false;
                    }
                }
            }
        }

        // Validate new field
        String newValue = Request.Form[UniqueID + "$utv_newvalue"];
        String nk = Request.Form[UniqueID + "$utk_newkey"];

        if (!String.IsNullOrEmpty(newValue) || !String.IsNullOrEmpty(nk))
        {
            if (!IsValidKey(nk))
            {
                mIsValid = false;
                isNewValid = false;
                ShowError((nk == String.Empty) ? GetString("uicontext.editor.enterkey") : String.Format(GetString("uicontext.editor.invalidkey"), nk));
                return false;
            }
        }

        mIsValid = true;
        return true;
    }


    /// <summary>
    /// Returns true if key is valid
    /// </summary>
    /// <param name="key">Key to test</param>    
    private bool IsValidKey(String key)
    {
        // Regular expression for collection key        
        return Reg.IsMatch(key);
    }


    /// <summary>
    /// Loads data from textboxes and store them to data collection
    /// </summary>
    private void LoadData()
    {
        data.Clear();
        var authenticatedUserIdentityOption = MacroIdentityOption.FromUserInfo(MembershipContext.AuthenticatedUser);

        foreach (String key in Request.Form.AllKeys)
        {
            if ((key != null) && key.StartsWithCSafe(UniqueID + "$tk"))
            {
                String value = Request.Form[key.Replace(UniqueID + "$tk", UniqueID + "$tv")];

                // Sign the macro
                value = MacroSecurityProcessor.AddSecurityParameters(value, authenticatedUserIdentityOption, null);
                String k = Request.Form[key];
                if (!IsValidKey(k))
                {
                    invalidKey = k;
                    k = INVALIDTOKEN;
                }

                data.SetValue(k, value);
            }
        }

        // Add data from new field
        String newValue = Request.Form[UniqueID + "$utv_newvalue"];
        String newKey = Request.Form[UniqueID + "$utk_newkey"];

        if (!String.IsNullOrEmpty(newValue) || !String.IsNullOrEmpty(newKey))
        {
            if (!IsValidKey(newKey))
            {
                isNewValid = false;
                return;
            }

            // Sign the macro
            newValue = MacroSecurityProcessor.AddSecurityParameters(newValue, authenticatedUserIdentityOption, null);
            data.SetValue(newKey, newValue);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        GenerateTable();
        GenerateNewRow();

        if (!IsValid())
        {
            // For non valid page cycle, copy all data from textboxes to labels
            // This ensures, labels and textboxes have the same value (textboxes values are loaded from control state and are actual)
            for (int i = 0; i < textboxes.Count; i++)
            {
                labels[i].Text = HTMLHelper.HTMLEncode(textboxes[i].Text);
            }
        }

        // Disable buttons for disabled control
        if (!Enabled)
        {
            RegisterEnableScript(false);
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Generates tables
    /// </summary>
    private void GenerateTable()
    {
        tblData.Controls.Clear();

        Hashtable ht = data.ConvertToHashtable();

        TableHeaderRow th = new TableHeaderRow()
        {
            TableSection = TableRowSection.TableHeader
        };
        TableHeaderCell ha = new TableHeaderCell();
        TableHeaderCell hn = new TableHeaderCell();
        TableHeaderCell hv = new TableHeaderCell();

        th.CssClass = "unigrid-head";

        ha.Text = GetString("unigrid.actions");
        ha.CssClass = "unigrid-actions-header";

        hn.Text = GetString("xmleditor.propertyname");
        hn.Width = Unit.Pixel(180);
        hv.Text = GetString("xmleditor.propertyvalue");
        hv.Width = Unit.Pixel(500);

        th.Cells.Add(ha);
        th.Cells.Add(hn);
        th.Cells.Add(hv);

        tblData.Rows.Add(th);

        ArrayList keys = new ArrayList(ht);
        keys.Sort(new CustomStringComparer());

        foreach (DictionaryEntry okey in keys)
        {
            String key = ValidationHelper.GetString(okey.Key, String.Empty);
            String value = ValidationHelper.GetString(okey.Value, String.Empty);

            bool isInvalid = (key == INVALIDTOKEN);
            key = isInvalid ? invalidKey : key;

            if (value == String.Empty)
            {
                continue;
            }

            TableRow tr = new TableRow();

            // Actions 
            TableCell tna = new TableCell();
            tna.CssClass = "unigrid-actions";

            var imgEdit = new CMSGridActionButton();
            imgEdit.OnClientClick = String.Format("displayEdit('{1}','{0}'); return false; ", key, ClientID);
            imgEdit.IconCssClass = "icon-edit";
            imgEdit.IconStyle = GridIconStyle.Allow;
            imgEdit.ID = key + "_edit";
            imgEdit.ToolTip = GetString("xmleditor.edititem");

            var imgOK = new CMSGridActionButton();
            imgOK.IconCssClass = "icon-check";
            imgOK.IconStyle = GridIconStyle.Allow;
            imgOK.OnClientClick = String.Format("approveCustomChanges('{0}','{1}');return false;", ClientID, key);
            imgOK.ID = key + "_ok";
            imgOK.ToolTip = GetString("xmleditor.approvechanges");

            var imgDelete = new CMSGridActionButton();
            imgDelete.OnClientClick = " if (confirm('" + GetString("xmleditor.deleteconfirm") + "')) {" + ControlsHelper.GetPostBackEventReference(tblData, "delete_" + key) + "} ;return false;";
            imgDelete.IconCssClass = "icon-bin";
            imgDelete.IconStyle = GridIconStyle.Critical;
            imgDelete.ID = key + "_del";
            imgDelete.ToolTip = GetString("xmleditor.deleteitem");

            var imgUndo = new CMSGridActionButton();
            imgUndo.OnClientClick = String.Format("if (confirm('" + GetString("xmleditor.confirmcancel") + "')) undoCustomChanges('{0}','{1}'); return false;", ClientID, key);
            imgUndo.IconCssClass = "icon-arrow-crooked-left";
            imgUndo.ID = key + "_undo";
            imgUndo.ToolTip = GetString("xmleditor.undochanges");

            tna.Controls.Add(imgEdit);
            tna.Controls.Add(imgOK);
            tna.Controls.Add(imgDelete);
            tna.Controls.Add(imgUndo);

            value = MacroSecurityProcessor.RemoveSecurityParameters(value, false, null);

            // Labels
            Label lblName = new Label();
            lblName.ID = "sk" + key;
            lblName.Text = key;

            Label lblValue = new Label();
            lblValue.ID = "sv" + key;
            lblValue.Text = HTMLHelper.HTMLEncode(value);

            // Textboxes
            CMSTextBox txtName = new CMSTextBox();
            txtName.Text = key;
            txtName.ID = "tk" + key;
            txtName.CssClass = "XmlEditorTextbox";

            CMSTextBox txtValue = new CMSTextBox();
            txtValue.Text = value;
            txtValue.ID = "tv" + key;
            txtValue.CssClass = "XmlEditorTextbox";
            txtValue.Width = Unit.Pixel(490);

            labels.Add(lblName);
            labels.Add(lblValue);

            textboxes.Add(txtName);
            textboxes.Add(txtValue);

            TableCell tcn = new TableCell();
            tcn.Controls.Add(lblName);
            tcn.Controls.Add(txtName);

            TableCell tcv = new TableCell();
            tcv.Controls.Add(lblValue);
            tcv.Controls.Add(txtValue);

            tr.Cells.Add(tna);
            tr.Cells.Add(tcn);
            tr.Cells.Add(tcv);

            tblData.Rows.Add(tr);

            lblValue.CssClass = String.Empty;
            lblName.CssClass = "CustomEditorKeyClass";

            if (isInvalid)
            {
                imgDelete.AddCssClass("hidden");
                imgEdit.AddCssClass("hidden");

                lblName.AddCssClass("hidden");
                lblValue.AddCssClass("hidden");

                RegisterEnableScript(false);
            }
            else
            {
                imgOK.AddCssClass("hidden");
                imgUndo.AddCssClass("hidden");

                txtName.CssClass += " hidden";
                txtValue.CssClass += " hidden";
            }
        }
    }


    /// <summary>
    /// Script for register starting state of buttons
    /// </summary>
    /// <param name="allow">Allow/Disable buttons</param>
    private void RegisterEnableScript(bool allow)
    {
        String script = ScriptHelper.GetScript(allow ? "allowEdit()" : "disableEdit()");
        ScriptHelper.RegisterStartupScript(pnlUpdate, typeof(String), "InvalidKeyPropertiesInit", script);
    }


    /// <summary>
    /// Generates row with textboxes for new values
    /// </summary>
    private void GenerateNewRow()
    {
        // New Item tab
        TableRow trNew = new TableRow();

        // Actions 
        TableCell tnew = new TableCell();
        tnew.CssClass = "unigrid-actions";

        var imgNew = new CMSGridActionButton();
        imgNew.OnClientClick = String.Format("addNewRow('{0}'); return false;", ClientID);
        imgNew.IconCssClass = "icon-plus";
        imgNew.IconStyle = GridIconStyle.Allow;
        imgNew.ToolTip = GetString("xmleditor.createitem");
        imgNew.ID = "add";

        var imgOK = new CMSGridActionButton();
        imgOK.IconCssClass = "icon-check-circle";
        imgOK.IconStyle = GridIconStyle.Allow;
        imgOK.ID = "newok";
        imgOK.ToolTip = GetString("xmleditor.additem");

        var imgDelete = new CMSGridActionButton();
        imgDelete.OnClientClick = String.Format("if (confirm('" + GetString("xmleditor.confirmcancel") + "')) cancelNewRow('{0}'); return false;", ClientID);
        imgDelete.IconCssClass = "icon-bin";
        imgDelete.IconStyle = GridIconStyle.Critical;
        imgDelete.ID = "newcancel";
        imgDelete.ToolTip = GetString("xmleditor.cancelnewitem");

        tnew.Controls.Add(imgNew);
        tnew.Controls.Add(imgOK);
        tnew.Controls.Add(imgDelete);

        // Textboxes
        CMSTextBox txtNewName = new CMSTextBox();
        txtNewName.ID = "utk_newkey";

        CMSTextBox txtNewValue = new CMSTextBox();
        txtNewValue.ID = "utv_newvalue";
        txtNewValue.Width = Unit.Pixel(490);

        TableCell tcnew = new TableCell();
        tcnew.Controls.Add(txtNewName);

        TableCell tcvnew = new TableCell();
        tcvnew.Controls.Add(txtNewValue);

        trNew.Cells.Add(tnew);
        trNew.Cells.Add(tcnew);
        trNew.Cells.Add(tcvnew);

        tblData.Rows.Add(trNew);

        imgOK.OnClientClick = "if (validateCustomProperties($cmsj('#" + txtNewName.ClientID + "').val(),null))" + ControlsHelper.GetPostBackEventReference(tblData, "add") + " ;return false";

        // Prevent load styles from control state
        imgDelete.AddCssClass("hidden");
        imgOK.AddCssClass("hidden");
        txtNewName.AddCssClass("hidden");
        txtNewValue.AddCssClass("hidden");
        txtNewValue.Text = String.Empty;
        txtNewName.Text = String.Empty;

        if (!isNewValid && RequestHelper.IsPostBack())
        {
            txtNewName.Text = Request.Form[UniqueID + "$utk_newkey"];
            txtNewValue.Text = Request.Form[UniqueID + "$utv_newvalue"];
            txtNewValue.CssClass = String.Empty;
            txtNewName.CssClass = String.Empty;

            imgOK.RemoveCssClass("hidden");
            imgDelete.RemoveCssClass("hidden");
            imgNew.AddCssClass("hidden");

            RegisterEnableScript(false);
        }
    }


    /// <summary>
    /// Returns true if all keys are valid.
    /// </summary>
    public override bool IsValid()
    {
        if (mIsValid == null)
        {
            IsValidInternal();
        }

        return mIsValid.Value;
    }


    /// <summary>
    /// Registers script for custom control.
    /// </summary>
    private void RegisterScript()
    {
        String script = @"
var disabledEdit = '" + UIHelper.GetImageUrl(Page, "/Design/Controls/UniGrid/Actions/EditDisabled.png") + @"';
var disabledDelete = '" + UIHelper.GetImageUrl(Page, "/Design/Controls/UniGrid/Actions/DeleteDisabled.png") + @"';
var enabledEdit = '" + UIHelper.GetImageUrl(Page, "/Design/Controls/UniGrid/Actions/Edit.png") + @"';
var enabledDelete = '" + UIHelper.GetImageUrl(Page, "/Design/Controls/UniGrid/Actions/Delete.png") + @"';
var disabledAdd = '" + UIHelper.GetImageUrl(Page, "/Design/Controls/UniGrid/Actions/addDisabled.png") + @"';
var enabledAdd = '" + UIHelper.GetImageUrl(Page, "/Design/Controls/UniGrid/Actions/add.png") + @"';

function disableEdit() {
    $cmsj('.customproperties_imgedit').prop('disabled', true);
    $cmsj('.customproperties_imgedit').attr('src', disabledEdit);

    $cmsj('.customproperties_imgdelete').prop('disabled', true);
    $cmsj('.customproperties_imgdelete').attr('src', disabledDelete);

    $cmsj('.customproperties_customadd').prop('disabled', true);
    $cmsj('.customproperties_customadd').attr('src', disabledAdd);
}

function allowEdit() {
    $cmsj('.customproperties_imgedit').prop('disabled', false);
    $cmsj('.customproperties_imgedit').attr('src', enabledEdit);

    $cmsj('.customproperties_imgdelete').prop('disabled', false);
    $cmsj('.customproperties_imgdelete').attr('src', enabledDelete);

    $cmsj('.customproperties_customadd').prop('disabled', false);
    $cmsj('.customproperties_customadd').attr('src', enabledAdd);
}

function disableEditButtons(key) {
    disableEdit();

    $cmsj('#' + key + '_del').addClass('hidden');
    $cmsj('#' + key + '_undo').removeClass('hidden');
}

function enableEditButtons(key) {
    allowEdit();

    $cmsj('#' + key + '_del').removeClass('hidden');
    $cmsj('#' + key + '_undo').addClass('hidden');
}

function undoCustomChanges(prefix, key) {
    var tk = $cmsj('#' + prefix + '_tk' + key);
    var tv = $cmsj('#' + prefix + '_tv' + key);
    var sk = $cmsj('#' + prefix + '_sk' + key);
    var sv = $cmsj('#' + prefix + '_sv' + key);
    tk.val(sk.text());
    tv.val(sv.text());

    tk.addClass('hidden');
    tv.addClass('hidden');

    sv.removeClass('hidden');
    sk.removeClass('hidden');

    displayDefault(prefix, key);
    clearError();
}

function approveCustomChanges(prefix, key) {    
    var tk = $cmsj('#' + prefix + '_tk' + key);
    var tv = $cmsj('#' + prefix + '_tv' + key);
    var sk = $cmsj('#' + prefix + '_sk' + key);
    var sv = $cmsj('#' + prefix + '_sv' + key);
    if (!validateCustomProperties(tk.val(), sk.text())){
      return;
    }

    sk.text(tk.val());
    sv.text(tv.val());

    tk.addClass('hidden');
    tv.addClass('hidden');

    sv.removeClass('hidden');
    sk.removeClass('hidden');

    displayDefault(prefix, key);
    clearError();
}

function displayDefault(prefix, key) {
    $cmsj('#' + prefix + '_' + key + '_edit').removeClass('hidden');
    $cmsj('#' + prefix + '_' + key + '_del').removeClass('hidden');

    $cmsj('#' + prefix + '_' + key + '_ok').addClass('hidden');
    $cmsj('#' + prefix + '_' + key + '_undo').addClass('hidden');

    allowEdit();
}

function displayEdit(prefix, key) {
    $cmsj('#' + prefix + '_' + key + '_edit').addClass('hidden');
    $cmsj('#' + prefix + '_' + key + '_del').addClass('hidden');

    $cmsj('#' + prefix + '_' + key + '_ok').removeClass('hidden');
    $cmsj('#' + prefix + '_' + key + '_undo').removeClass('hidden');

    var tk = $cmsj('#' + prefix + '_tk' + key);
    var tv = $cmsj('#' + prefix + '_tv' + key);
    var sk = $cmsj('#' + prefix + '_sk' + key);
    var sv = $cmsj('#' + prefix + '_sv' + key);

    tk.removeClass('hidden');
    tv.removeClass('hidden');

    sv.addClass('hidden');
    sk.addClass('hidden');

    disableEdit();
}

function addNewRow(prefix) {

    var nr = $cmsj('#' + prefix + '_utk_newkey');
    var nv = $cmsj('#' + prefix + '_utv_newvalue');

    nr.val('');
    nv.val('');

    nv.removeClass('hidden');
    nr.removeClass('hidden');
    disableEdit();

    $cmsj('#' + prefix + '_newok').removeClass('hidden');
    $cmsj('#' + prefix + '_newcancel').removeClass('hidden');
    $cmsj('#' + prefix + '_add').addClass('hidden');

    clearError();    
}

function cancelNewRow(prefix) {
    allowEdit();

    var nr = $cmsj('#' + prefix + '_utk_newkey');
    var nv = $cmsj('#' + prefix + '_utv_newvalue');

    nr.addClass('hidden');
    nv.addClass('hidden');

    $cmsj('#' + prefix + '_newok').addClass('hidden');
    $cmsj('#' + prefix + '_newcancel').addClass('hidden');
    $cmsj('#' + prefix + '_add').removeClass('hidden');

    nr.val('');
    nv.val('');
    clearError();
} 

function clearError() {
    $cmsj('.EditingFormErrorLabel').text('');
}

function validateCustomProperties(key, currentVal) {
 var regexp =  new RegExp('" + keyRegExp + @"');
 var match  = regexp.test(key);
 var le = $cmsj('#" + lblError.ClientID + @"');
 if (!match) {
    le.text('" + ScriptHelper.GetString(GetString("uicontext.editor.currentinvalidkey"), false) + @"');
    return false;
 }
 
 var l = $cmsj('.CustomEditorKeyClass').filter(function() {
        return $cmsj(this).text() === key;
 });

 if (l.length > 0) {    
     if (key != currentVal) {
       le.text('" + ScriptHelper.GetString(GetString("uicontext.editor.keyalreadyexists"), false) + @"');
       return false;
     }
 }

 return true;
}";

        ScriptHelper.RegisterClientScriptBlock(pnlUpdate, typeof(String), "CustomControlsFiles", ScriptHelper.GetScript(script));
    }

    #endregion
}