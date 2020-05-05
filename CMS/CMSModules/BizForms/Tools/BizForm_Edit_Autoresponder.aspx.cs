using System;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.UIControls;

// Edited object
[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
[Security(Resource = "CMS.Form", Permission = "ReadForm")]
[UIElement("CMS.Form", "Forms.Autoresponder")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_Autoresponder : CMSBizFormPage
{
    #region "Variables"

    private DataClassInfo formClassObj;
    private CurrentUserInfo currentUser;
    protected SaveAction save = null;
    protected HeaderAction attachments = null;
    protected BizFormInfo formInfo = null;
    private const string mAttachmentsActionClass = "attachments-header-action";

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether custom form layout is set or not.
    /// </summary>
    private bool IsLayoutSet
    {
        get
        {
            object obj = ViewState["IsLayoutSet"];
            return ValidationHelper.GetBoolean(obj, false);
        }
        set
        {
            ViewState["IsLayoutSet"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObject == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        currentUser = MembershipContext.AuthenticatedUser;

        // Register for action
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, lnkSave_Click);

        formInfo = EditedObject as BizFormInfo;

        if (formInfo != null)
        {
            // Control initialization
            ltlConfirmDelete.Text = "<input type=\"hidden\" id=\"confirmdelete\" value=\"" + GetString("Bizform_Edit_Autoresponder.ConfirmDelete") + "\">";

            drpEmailField.SelectedIndexChanged += drpEmailField_SelectedIndexChanged;

            // Init header actions
            InitHeaderActions();

            // Initialize HTML editor
            InitHTMLEditor();

            if (!RequestHelper.IsPostBack())
            {
                // Get bizform class object
                formClassObj = DataClassInfoProvider.GetDataClassInfo(formInfo.FormClassID);
                if (formClassObj != null)
                {
                    // Enable or disable form
                    EnableDisableForm(formInfo.FormConfirmationTemplate);

                    // Fill list of available fields                    
                    FillFieldsList();

                    // Load dropdown list with form text fields   
                    InitializeEmailDropdown();

                    // Load email subject and email from address
                    txtEmailFrom.Text = formInfo.FormConfirmationSendFromEmail;
                    txtEmailSubject.Text = formInfo.FormConfirmationEmailSubject;
                }
                else
                {
                    // Disable form by default
                    EnableDisableForm(null);
                }
            }
        }
    }


    /// <summary>
    /// Initializes and loads values into email dropdown control.
    /// </summary>
    private void InitializeEmailDropdown()
    {
        FormInfo fi = FormHelper.GetFormInfo(formClassObj.ClassName, false);
        foreach (var fieldInfo in fi.GetFields(FieldDataType.Text))
        {
            drpEmailField.Items.Add(new ListItem(fieldInfo.GetDisplayName(MacroResolver.GetInstance()), fieldInfo.Name));
        }
        drpEmailField.Items.Insert(0, new ListItem(GetString("bizform_edit_autoresponder.emptyemailfield"), string.Empty));

        // Try to select specified field
        ListItem li = drpEmailField.Items.FindByValue(formInfo.FormConfirmationEmailField);
        if (li != null)
        {
            li.Selected = true;
        }
    }


    protected void Page_PreRender(Object sender, EventArgs e)
    {
        btnInsertInput.OnClientClick = "InsertAtCursorPosition('$$value:' + document.getElementById('" + lstAvailableFields.ClientID + "').value + '$$'); return false;";
        btnInsertLabel.OnClientClick = "InsertAtCursorPosition('$$label:' + document.getElementById('" + lstAvailableFields.ClientID + "').value + '$$'); return false;";

        SetCustomLayoutVisibility(!string.IsNullOrEmpty(drpEmailField.SelectedValue));

        // Get save script
        string script = null;
        if (!pnlCustomLayout.Visible && IsLayoutSet)
        {
            script = "if(!ConfirmDelete()) { return false; } ";
        }

        // Refresh script
        save.OnClientClick = script;
        menu.ReloadData();

        // Register other scripts
        RegisterScripts();
    }


    private void drpEmailField_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetCustomLayoutVisibility(!string.IsNullOrEmpty(drpEmailField.SelectedValue));
    }


    /// <summary>
    /// Save button is clicked.
    /// </summary>
    protected void lnkSave_Click(object sender, EventArgs e)
    {
        // Check 'EditForm' permission
        if (!currentUser.IsAuthorizedPerResource("cms.form", "EditForm"))
        {
            RedirectToAccessDenied("cms.form", "EditForm");
        }

        // Validate form
        string errorMessage = new Validator().NotEmpty(txtEmailFrom.Text.Trim(), GetString("bizform_edit_autoresponder.emptyemail")).NotEmpty(txtEmailSubject.Text.Trim(), GetString("bizform_edit_autoresponder.emptysubject")).Result;

        // Check if from e-mail contains macro expression or e-mails separated by semicolon
        if (string.IsNullOrEmpty(errorMessage) && !MacroProcessor.ContainsMacro(txtEmailFrom.Text.Trim()) && !txtEmailFrom.IsValid())
        {
            errorMessage = GetString("bizform_edit_autoresponder.emptyemail");
        }

        if ((string.IsNullOrEmpty(errorMessage)) || (!pnlCustomLayout.Visible))
        {
            errorMessage = String.Empty;
            if (formInfo != null)
            {
                // Save custom layout
                if (!string.IsNullOrEmpty(drpEmailField.SelectedValue))
                {
                    formInfo.FormConfirmationTemplate = htmlEditor.ResolvedValue.Trim();
                    formInfo.FormConfirmationEmailField = drpEmailField.SelectedValue;
                    formInfo.FormConfirmationEmailSubject = txtEmailSubject.Text.Trim();
                    formInfo.FormConfirmationSendFromEmail = txtEmailFrom.Text.Trim();

                    try
                    {
                        BizFormInfoProvider.SetBizFormInfo(formInfo);
                        ShowChangesSaved();
                        EnableDisableForm(formInfo.FormConfirmationTemplate);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }
                }
                // Delete custom layout if exists
                else
                {
                    formInfo.FormConfirmationTemplate = null;
                    formInfo.FormConfirmationEmailField = drpEmailField.SelectedValue;
                    formInfo.FormConfirmationEmailSubject = string.Empty;
                    formInfo.FormConfirmationSendFromEmail = string.Empty;

                    // Delete all attachments
                    MetaFileInfoProvider.DeleteFiles(formInfo.FormID, BizFormInfo.OBJECT_TYPE, ObjectAttachmentsCategories.FORMLAYOUT);

                    try
                    {
                        BizFormInfoProvider.SetBizFormInfo(formInfo);
                        if(IsLayoutSet)
                        {
                            ShowConfirmation(GetString("Bizform_Edit_Autoresponder.LayoutDeleted"));
                        }
                        else
                        {
                            ShowChangesSaved();
                        }
                        EnableDisableForm(formInfo.FormConfirmationTemplate);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes header actions.
    /// </summary>
    protected void InitHeaderActions()
    {
        menu.ActionsList.Clear();

        // Add save action
        save = new SaveAction();
        menu.ActionsList.Add(save);

        bool isAuthorized = CurrentUser.IsAuthorizedPerResource("cms.form", "EditForm") && (EditedObject != null);

        int attachCount = 0;
        if (isAuthorized)
        {
            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(formInfo.FormID, BizFormInfo.OBJECT_TYPE, ObjectAttachmentsCategories.FORMLAYOUT, null, null, "MetafileID", -1);
            attachCount = ds.Items.Count;

            // Register attachments count update module
            ScriptHelper.RegisterModule(this, "CMS/AttachmentsCountUpdater", new { Selector = "." + mAttachmentsActionClass, Text = GetString("general.attachments") });

            // Register dialog scripts
            ScriptHelper.RegisterDialogScript(Page);
        }

        // Prepare metafile dialog URL
        string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
        string query = string.Format("?objectid={0}&objecttype={1}", formInfo.FormID, BizFormInfo.OBJECT_TYPE);
        metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, ObjectAttachmentsCategories.FORMLAYOUT, QueryHelper.GetHash(query));

        // Init attachment button
        attachments = new HeaderAction
        {
            Text = GetString("general.attachments") + ((attachCount > 0) ? " (" + attachCount + ")" : string.Empty),
            Tooltip = GetString("general.attachments"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
            Enabled = isAuthorized,
            CssClass = mAttachmentsActionClass,
            ButtonStyle = ButtonStyle.Default,
        };
        menu.ActionsList.Add(attachments);
    }


    /// <summary>
    /// Initializes HTML editor's settings.
    /// </summary>
    protected void InitHTMLEditor()
    {
        htmlEditor.AutoDetectLanguage = false;
        htmlEditor.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlEditor.LinkDialogConfig.UseFullURL = true;
        htmlEditor.QuickInsertConfig.UseFullURL = true;
        htmlEditor.ToolbarSet = "BizForm";

        DialogConfiguration config = htmlEditor.MediaDialogConfig;
        config.UseFullURL = true;
        config.MetaFileObjectID = formInfo.FormID;
        config.MetaFileObjectType = BizFormInfo.OBJECT_TYPE;
        config.MetaFileCategory = ObjectAttachmentsCategories.FORMLAYOUT;
        config.HideAttachments = false;
    }


    /// <summary>
    /// Sets visibility of custom layout form.
    /// </summary>
    private void SetCustomLayoutVisibility(bool visible)
    {
        pnlCustomLayout.Visible = visible;
        attachments.Enabled = visible;

        if (visible)
        {
            // Reload HTML editor content
            if (formInfo != null && formInfo.FormConfirmationTemplate != null)
            {
                htmlEditor.ResolvedValue = formInfo.FormConfirmationTemplate;
            }
        }
    }


    /// <summary>
    /// Fills list of available fields.
    /// </summary>
    private void FillFieldsList()
    {
        if (formClassObj != null)
        {
            // load form definition and get visible fields
            var fi = FormHelper.GetFormInfo(formClassObj.ClassName, false);
            var fields = fi.GetFields(true, true);

            lstAvailableFields.Items.Clear();

            if (fields != null)
            {
                // add visible fields to the list
                foreach (FormFieldInfo ffi in fields)
                {
                    lstAvailableFields.Items.Add(new ListItem(ffi.GetDisplayName(MacroResolver.GetInstance()), ffi.Name));
                }
            }
            lstAvailableFields.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Enables or disables form according to the confirmation email template text is defined or not.
    /// </summary>
    /// <param name="formLayout">Autoresponder layout</param>
    protected void EnableDisableForm(string formLayout)
    {
        if (RequestHelper.IsPostBack())
        {
            InitHeaderActions();
        }

        if (!string.IsNullOrEmpty(formLayout))
        {
            // Enable layout editing                                
            pnlCustomLayout.Visible = true;

            // Set confirmation email template text to the editable window of the HTML editor
            htmlEditor.ResolvedValue = formLayout;

            // Save info to viewstate 
            IsLayoutSet = true;
        }
        else
        {
            // Layout editing is not enabled by default        
            pnlCustomLayout.Visible = false;

            htmlEditor.ResolvedValue = string.Empty;

            // Save info to viewstate
            IsLayoutSet = false;
        }
    }


    /// <summary>
    /// Registers JS scripts.
    /// </summary>
    protected void RegisterScripts()
    {
        string script =
string.Format(@"// Insert desired HTML at the current cursor position of the CK editor
function InsertHTML(htmlString) {{
    // Get the editor instance that we want to interact with
    var oEditor = CKEDITOR.instances['{0}'];

    // Check the active editing mode
    if (oEditor.mode == 'wysiwyg') {{
        // Insert the desired HTML.
        oEditor.insertHtml(htmlString);
    }}
    else alert('You must be on WYSIWYG mode!');
}}

// Set content of the CK editor - replace the actual one
function SetContent(newContent) {{
    // Get the editor instance that we want to interact with.
    var oEditor = CKEDITOR.instances['{0}'];

    // Set the editor content (replace the actual one).
    oEditor.setData(newContent);
}}

function PasteImage(imageurl) {{
    imageurl = '<img src=""' + imageurl + '"" />';
    return InsertHTML(imageurl);
}}

// Returns HTML code with standard table layout
function GenerateTableLayout() {{
    var tableLayout = """";

    // indicates whether any row definition was added to the table
    var rowAdded = false;

    // list of attributes
    var list = document.getElementById('{1}');

    // attributes count
    var optionsCount = 0;
    if (list != null) {{
        optionsCount = list.options.length;
    }}

    for (var i = 0; i < optionsCount; i++) {{
        tableLayout += ""<tr><td>$$label:"" + list.options[i].value + ""$$</td><td>$$value:"" + list.options[i].value + ""$$</td></tr>"";
        rowAdded = true;
    }}

    if (rowAdded) {{
        tableLayout = ""<table><tbody>"" + tableLayout + ""</tbody></table>"";
    }}

    return tableLayout;
}}

// Insert desired HTML at the current cursor position of the CK editor if it is not already inserted 
function InsertAtCursorPosition(htmlString) {{
    InsertHTML(htmlString);
}}

function ConfirmDelete() {{
    return confirm(document.getElementById('confirmdelete').value);
}}", htmlEditor.ClientID, lstAvailableFields.ClientID);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Autoresponder_" + ClientID, script, true);
    }

    #endregion
}
