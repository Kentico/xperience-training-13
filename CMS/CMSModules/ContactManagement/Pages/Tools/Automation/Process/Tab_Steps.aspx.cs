using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Automation;
using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine.Query;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

using Newtonsoft.Json;

[EditedObject(WorkflowInfo.OBJECT_TYPE_AUTOMATION, "processid")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess;EditProcessSteps")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Steps : CMSAutomationPage
{
    #region "Constants"

    private const string DESIGNER_VIEW_URL = "AutomationDesignerPage.aspx";
    private const string DESIGNER_VIEW_NAME = "DesignerView";
    private const string CONTACTS_VIEW_URL = "Tab_Contacts.aspx";
    private const string CONTACTS_VIEW_NAME = "ContactsView";

    private const string SAVE_AS_TEMPLATE_CLIENT_SCRIPT = "SaveAsTemplate(); return false;";
    private const string CHANGE_VIEW_CLIENT_SCRIPT = "ChangeView(this); return false;";
    private const string UNDERLYING_VIEW_ATTR_KEY_NAME = "UnderlyingView";

    #endregion


    #region "Variables"

    private int mWorkflowID;

    private WorkflowInfo mWorkflow;

    private WebControl mIcon;

    private Dictionary<string, string> mViewSettings;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the icon control.
    /// </summary>
    private WebControl Icon => mIcon ?? (mIcon = GetIcon());


    /// <summary>
    /// Dictionary of all possible views, their names and URLs.
    /// </summary>
    private Dictionary<string, string> ViewSettings => mViewSettings ?? (mViewSettings = new Dictionary<string, string>
    {
        { DESIGNER_VIEW_NAME, URLHelper.AppendQuery(DESIGNER_VIEW_URL, RequestContext.CurrentQueryString) },
        { CONTACTS_VIEW_NAME, URLHelper.AppendQuery(CONTACTS_VIEW_URL, RequestContext.CurrentQueryString) }
    });


    /// <summary>
    /// Gets current view name saved in cookies.
    /// Default value <see cref="DESIGNER_VIEW_NAME"/> is returned
    /// if the cookie doesn't exist or contains invalid value.
    /// </summary>
    private string CurrentViewName
    {
        get
        {
            if (GetCookieValue().TryGetValue(WorkflowID, out var lastSelectedView) && ViewSettings.ContainsKey(lastSelectedView))
            {
                return lastSelectedView;
            }

            return DESIGNER_VIEW_NAME;
        }
    }


    /// <summary>
    /// Current workflow ID
    /// </summary>
    public int WorkflowID
    {
        get
        {
            if (mWorkflowID <= 0)
            {
                mWorkflowID = QueryHelper.GetInteger("processid", 0);
            }
            return mWorkflowID;
        }
    }


    /// <summary>
    /// Current workflow info object.
    /// </summary>
    private WorkflowInfo Workflow
    {
        get
        {
            if (mWorkflow == null)
            {
                mWorkflow = WorkflowInfo.Provider.Get(WorkflowID);
                if ((mWorkflow != null) && (mWorkflow.WorkflowType != WorkflowTypeEnum.Automation))
                {
                    RedirectToAccessDenied(GetString("workflow.type.notsupported"));
                    mWorkflow = null;
                }
            }
            return mWorkflow;
        }
    }


    /// <summary>
    /// Checks permissions and license sufficiency for process state edit.
    /// </summary>
    private bool CanEditProcessState => Workflow != null && LicenseIsSufficient && AuthorizedToManageAutomation;


    /// <summary>
    /// Checks permissions and license sufficiency for template creation.
    /// </summary>
    private bool CanSaveAsTemplate => Workflow != null && LicenseIsSufficient && AuthorizedToManageTemplates;

    #endregion


    #region "Event handlers and methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CheckLicense();
        RegisterScripts();
        InitializeHeader();
        InitializeMoreOptionsButton();

        ScriptHelper.HideVerticalTabs(this);
    }


    private void RegisterScripts()
    {
        RegisterNavigationScripts();
        RegisterSaveAsTemplateScripts();
    }


    private void RegisterNavigationScripts()
    {
        var changeViewScript = $@"
function ChangeView(elem) {{
    var clickedButtonJQ = $cmsj(elem);

    if (clickedButtonJQ.hasClass('active')) {{
        return;
    }}

    var selectedViewName = clickedButtonJQ.attr('{UNDERLYING_VIEW_ATTR_KEY_NAME}');
    var viewSettings = {JsonConvert.SerializeObject(ViewSettings)};
    var selectedViewUrl = viewSettings[selectedViewName];

    UpdateNavigationState(clickedButtonJQ);
    SaveNavigationState(selectedViewName);
    LoadView(selectedViewUrl);
}}


function UpdateNavigationState(clickedButtonJQ) {{
    $cmsj('#{navigation.ClientID} button').removeClass('active');
    clickedButtonJQ.addClass('active');
}}


function SaveNavigationState(viewName) {{
    var today = new Date();
    var expiration = new Date();
    expiration.setTime(today.getTime() + 3600 * 1000 * 24 * 60);

    var cookieValue = {JsonConvert.SerializeObject(GetCookieValue())};
    cookieValue[{WorkflowID}] = viewName;

    document.cookie = '{CookieName.MarketingAutomationSelectedView}=' + JSON.stringify(cookieValue) + ';Domain={URLHelper.RemovePort(SiteContext.CurrentSite.DomainName)};Path=/;SameSite=Lax;Expires=' + expiration.toGMTString();
}}


function LoadView(viewUrl) {{
    $cmsj('#{mainView.ClientID}')[0].src = viewUrl;
}}";

        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "MA_ChangeView", ScriptHelper.GetScript(changeViewScript));
    }


    private void RegisterSaveAsTemplateScripts()
    {
        if (!CanSaveAsTemplate)
        {
            return;
        }

        var saveAsTemplateScript = $@"
function SaveAsTemplate() {{
    modalDialog('{UrlResolver.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/Automation/Process/Template_Edit.aspx")}?processId={WorkflowID}', 'MA_Edit_Template', 800, 650);
}}";

        ScriptHelper.RegisterDialogScript(this);
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "MA_SaveAsTemplate", ScriptHelper.GetScript(saveAsTemplateScript));
    }


    private void RefreshView()
    {
        var refreshScript = $"var view = $cmsj('#{mainView.ClientID}')[0]; view.src = view.contentWindow.document.location;";
        ScriptHelper.RegisterStartupScript(this, GetType(), "RefreshView", ScriptHelper.GetScript(refreshScript));
    }


    private IDictionary<int, string> GetCookieValue()
    {
        var cookieDictionary = new Dictionary<int, string>();

        try
        {
            var cookieValue = CookieHelper.GetValue(CookieName.MarketingAutomationSelectedView);

            if (!String.IsNullOrEmpty(cookieValue))
            {
                cookieDictionary = JsonConvert.DeserializeObject<Dictionary<int, string>>(cookieValue);
            }
        }
        catch (JsonReaderException)
        {
            // Ignore deserialization issues
        }

        return cookieDictionary;
    }


    private bool ProcessHasContacts()
    {
        return AutomationStateInfo.Provider.Get()
            .WhereEquals("StateWorkflowID", WorkflowID)
            .GetCount() > 0;
    }


    private void InitializeHeader()
    {
        InitializeProcessState();
        InitializeNavigationButtons();

        if (RequestHelper.IsPostBack())
        {
            return;
        }

        InitializeView();
    }


    private void InitializeNavigationButtons()
    {
        var isContactsButtonEnabled = ProcessHasContacts() || Workflow.WorkflowEnabled;
        var cookieNeedsUpdate = RequestHelper.IsPostBack() && CurrentViewName.Equals(CONTACTS_VIEW_NAME, StringComparison.OrdinalIgnoreCase) && !isContactsButtonEnabled && btnContacts.Enabled;

        btnContacts.Enabled = isContactsButtonEnabled;
        btnContacts.ToolTip = isContactsButtonEnabled ? String.Empty : GetString("ma.contact.contacts.tooltip.disabled");

        btnDesigner.Text = GetString("ma.designer");
        btnContacts.Text = GetString("ma.contact.contacts");

        btnDesigner.Attributes.Add(UNDERLYING_VIEW_ATTR_KEY_NAME, DESIGNER_VIEW_NAME);
        btnContacts.Attributes.Add(UNDERLYING_VIEW_ATTR_KEY_NAME, CONTACTS_VIEW_NAME);

        btnDesigner.OnClientClick = btnContacts.OnClientClick = CHANGE_VIEW_CLIENT_SCRIPT;

        if (CurrentViewName.Equals(DESIGNER_VIEW_NAME, StringComparison.OrdinalIgnoreCase))
        {
            btnDesigner.AddCssClass("active");
        }
        else
        {
            btnContacts.AddCssClass("active");
        }

        if (cookieNeedsUpdate)
        {
            var cookieValue = GetCookieValue();
            if (cookieValue.Remove(WorkflowID))
            {
                CookieHelper.SetValue(CookieName.MarketingAutomationSelectedView, JsonConvert.SerializeObject(cookieValue), "/", DateTime.Now.AddMonths(2), false);
            }
        }
    }


    private void InitializeView()
    {
        if (ViewSettings.TryGetValue(CurrentViewName, out var viewUrl))
        {
            mainView.Src = viewUrl;
        }
    }


    private void InitializeProcessState()
    {
        Icon.RemoveCssClass(Workflow.WorkflowEnabled ? "disabled" : "enabled");
        Icon.AddCssClass(Workflow.WorkflowEnabled ? "enabled" : "disabled");
        lblState.Text = Workflow.WorkflowEnabled ? GetString("automationdesigner.processactive") : GetString("automationdesigner.processinactive");

        if (!CanEditProcessState)
        {
            btnToggleState.Visible = false;
            return;
        }

        btnToggleState.Text = Workflow.WorkflowEnabled ? GetString("general.disable") : GetString("general.enable");
    }


    private void InitializeMoreOptionsButton()
    {
        if (!CanSaveAsTemplate)
        {
            return;
        }

        btnMoreOptions.Visible = true;
        btnMoreOptions.ToolTip = GetString("EditMenu.MoreActions");
        btnMoreOptions.Actions.Add(new CMSButtonAction
        {
            Text = GetString("ma.template.create"),
            ToolTip = GetString("ma.template.create.tooltip"),
            OnClientClick = SAVE_AS_TEMPLATE_CLIENT_SCRIPT
        });
    }


    private WebControl GetIcon()
    {
        var icon = new WebControl(HtmlTextWriterTag.I);
        icon.Attributes.Add("aria-hidden", "true");
        icon.AddCssClass("cms-icon-50");
        icon.AddCssClass("icon-circle");
        icnState.Controls.AddAt(0, icon);

        return icon;
    }


    protected void ToggleState(object sender, EventArgs args)
    {
        if (!CanEditProcessState)
        {
            return;
        }

        Workflow.WorkflowEnabled = !Workflow.WorkflowEnabled;
        WorkflowInfo.Provider.Set(Workflow);

        InitializeHeader();
        RefreshView();
    }

    #endregion
}
