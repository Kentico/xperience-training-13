using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.Newsletters.Issues.Widgets;
using CMS.Newsletters.Issues.Widgets.Configuration;
using CMS.Newsletters.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


/// <summary>
/// Page representing widget properties dialog
/// </summary>
[UIElement(ModuleName.NEWSLETTER, EmailBuilderHelper.EMAIL_BUILDER_UI_ELEMENT)]
public partial class CMSModules_Newsletters_EmailBuilder_EmailWidgetProperties : CMSNewsletterPage
{
    private IssueInfo mIssue;
    private Guid widgetInstanceGuid;
    private Widget widgetInstance;
    private FormInfo widgetTypeFormInfo;
    private EmailWidgetInfo widgetTypeDefinition;
    private IZonesConfigurationService mWidgetService;
    private bool? mEditEnabled;


    /// <summary>
    /// Service for widget manipulation within the current email issue.
    /// </summary>
    private IZonesConfigurationService WidgetService
    {
        get
        {
            if (mWidgetService == null)
            {
                mWidgetService = Service.Resolve<IZonesConfigurationServiceFactory>().Create(Issue.IssueID);
            }

            return mWidgetService;
        }
    }


    /// <summary>
    /// Represents the email issue whose properties the control displays.
    /// </summary>
    private IssueInfo Issue => mIssue ?? (mIssue = IssueInfo.Provider.Get(QueryHelper.GetInteger("issueid", 0)));


    /// <summary>
    /// Indicates whether editing is allowed.
    /// </summary>
    private bool EditEnabled
    {
        get
        {
            if (!mEditEnabled.HasValue)
            {
                mEditEnabled = EmailBuilderHelper.IsIssueEditableByUser(Issue, MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteName);
            }

            return mEditEnabled.Value;
        }
    }


    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        EnsureScriptManager();
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        widgetInstanceGuid = QueryHelper.GetGuid("widgetinstanceguid", Guid.Empty);

        widgetInstance = WidgetService.GetWidgetConfiguration(widgetInstanceGuid);
        if (widgetInstance == null)
        {
            ShowWidgetNotFound();
            return;
        }

        widgetTypeDefinition = EmailWidgetInfo.Provider.Get(widgetInstance.TypeIdentifier, SiteContext.CurrentSiteID);
        if (widgetTypeDefinition == null)
        {
            ShowWidgetNotFound();
            return;
        }

        widgetTypeFormInfo = new FormInfo(widgetTypeDefinition.EmailWidgetProperties);

        btnSubmit.Enabled = EditEnabled;
        SetClientApplicationData();
        SetupHeader();
        SetupForm();

        if (EditEnabled)
        {
            btnSubmit.Click += btnSubmit_Click;
        }

        // Add shadow below header actions
        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");
    }


    /// <summary>
    /// Displays the widget display name in the properties header.
    /// </summary>
    private void SetupHeader()
    {
        lblWidgetName.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(widgetTypeDefinition.EmailWidgetDisplayName));
        lblWidgetName.ToolTip = ResHelper.LocalizeString(widgetTypeDefinition.EmailWidgetDisplayName);
    }


    /// <summary>
    /// Configures the properties editing form and loads the widget instance properties in it.
    /// </summary>
    private void SetupForm()
    {
        if (widgetTypeFormInfo.ItemsList.Count > 0)
        {
            propertiesForm.MessagesPlaceHolder = plcMessages;
            propertiesForm.SubmitButton.Visible = false;
            propertiesForm.SiteName = SiteContext.CurrentSiteName;
            propertiesForm.Data = GetWidgetProperties();
            propertiesForm.FormInformation = widgetTypeFormInfo;
            propertiesForm.Enabled = EditEnabled;
            propertiesForm.ResolverName = WidgetResolvers.GetResolverName(widgetTypeDefinition.EmailWidgetID);

            propertiesForm.ReloadData();
        }
        else
        {
            ShowPropertiesNotDefined();
        }
    }


    /// <summary>
    /// Displays the error message and disables the control actions.
    /// </summary>
    private void ShowWidgetNotFound()
    {
        plcMessages.ShowError(GetString("emailbuilder.widgetnotfound"));

        btnSubmit.Enabled = false;
    }


    /// <summary>
    /// Displays the information message and disables the control actions.
    /// </summary>
    private void ShowPropertiesNotDefined()
    {
        plcMessages.ShowInformation(GetString("emailbuilder.nowidgetproperties"));

        btnSubmit.Enabled = false;
    }


    /// <summary>
    /// Loads DataRow container for BasicForm with widget properties.
    /// </summary>
    private DataRowContainer GetWidgetProperties()
    {
        var result = new DataRowContainer(widgetTypeFormInfo.GetDataRow());

        foreach (var property in widgetInstance.Properties)
        {
            if (property.Value == null)
            {
                continue;
            }

            var formField = widgetTypeFormInfo.GetFormField(property.Name);
            if (formField == null)
            {
                continue;
            }

            result[property.Name] = DataTypeManager.ConvertToSystemType(TypeEnum.Field, formField.DataType, property.Value, CultureHelper.EnglishCulture);
        }

        return result;
    }


    /// <summary>
    /// Handles the Click event of the btnSubmit control.
    /// </summary>
    private void btnSubmit_Click(object sender, EventArgs e)
    {
        if (EditEnabled && propertiesForm.SaveData(null, false))
        {
            var properties = new NameValueCollection();
            propertiesForm.Fields.ForEach(fieldName =>
            {
                var fieldDataType = propertiesForm.FormInformation.GetFormField(fieldName).DataType;
                var fieldValue = propertiesForm.Data.GetValue(fieldName);
                var value = DataTypeManager.GetStringValue(TypeEnum.Field, fieldDataType, fieldValue, CultureHelper.EnglishCulture);
            
                properties.Add(fieldName, value);
            });

            try
            {
                WidgetService.StoreWidgetProperties(widgetInstanceGuid, properties);
                hdnSaveStatus.Value = "1";
            }
            catch (InvalidOperationException exception)
            {
                Service.Resolve<IEventLogService>().LogException("Newsletter", "SAVEWIDGETPROP", exception);
            }
        }
    }


    /// <summary>
    /// Sets the data passed to the client application.
    /// </summary>
    private void SetClientApplicationData()
    {
        var emailBuilderData = new Dictionary<string, object>();

        emailBuilderData.Add("saveStatusFieldSelector", $"#{hdnSaveStatus.ClientID}");
        RequestContext.ClientApplication.Add("widgetProperties", emailBuilderData);
    }
}

