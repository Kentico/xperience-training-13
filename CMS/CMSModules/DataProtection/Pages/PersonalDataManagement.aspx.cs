using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataProtection;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;

using Newtonsoft.Json;


public partial class CMSModules_DataProtection_Pages_PersonalDataManagement : CMSPage
{
    private const string ERASURE_CONFIGURATION_DIALOG_ELEMENT_NAME = "ErasureConfigurationDialog";

    private const string DATA_PORTABILITY = "dataportability";
    private const string RIGHT_TO_ACCESS = "righttoaccess";
    private const string RIGHT_TO_BE_FORGOTTEN = "righttobeforgotten";

    // Short links to related help topic pages
    private const string HELP_TOPIC_CUSTOM_COLLECTION_LINK = "personal_data_custom_collection";
    private const string HELP_TOPIC_CUSTOM_ERASURE_LINK = "personal_data_custom_erasure";

    private readonly HashSet<string> mPageModes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        DATA_PORTABILITY,
        RIGHT_TO_ACCESS,
        RIGHT_TO_BE_FORGOTTEN
    };

    private string mElementName;
    private string mOutputFormat;
    private string mDataSubjectIdentifiersFilter;
    private DataSubjectIdentifiersFilterControl mDataSubjectIdentifiersFilterControl;


    /// <summary>
    /// Gets the query string parameter by which the page decides it's behavior.
    /// </summary>
    private string ElementName => mElementName ?? (mElementName = QueryHelper.GetString("elementName", String.Empty));


    /// <summary>
    /// Performs security and license check on <see cref="UIElementInfo"/>.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!mPageModes.Contains(ElementName))
        {
            RedirectToUINotAvailable();
        }

        var uiElement = new UIElementAttribute(DataProtectionModule.MODULE_NAME, ElementName, false, true);
        uiElement.Check(this);

        mOutputFormat = GetOutputFormat();

        LoadDataSubjectIdentifiersFilterControl();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        MessagesPlaceHolder = plcMessages;
    }


    private void LoadDataSubjectIdentifiersFilterControl()
    {
        string virtualPath = DataProtectionControlsRegister.Instance.GetDataSubjectIdentifiersFilterControl();
        try
        {
            if (virtualPath != null)
            {
                mDataSubjectIdentifiersFilterControl = (DataSubjectIdentifiersFilterControl)Page.LoadUserControl(virtualPath);
                mDataSubjectIdentifiersFilterControl.EnableViewState = true;
                mDataSubjectIdentifiersFilterControl.ID = "dataSubjectIdentifiersFilter";
                mDataSubjectIdentifiersFilterControl.ShortID = "dsif";

                plcDataSubjectIdentifiersFilter.Controls.Add(mDataSubjectIdentifiersFilterControl);
            }
        }
        catch (Exception ex)
        {
            btnSearch.Visible = false;

            Service.Resolve<IEventLogService>().LogException("Data protection", "FILTERCONTROLLOAD", ex, additionalMessage: $"Could not load control '{virtualPath}'.");
            ShowError($"Could not load control '{virtualPath}'. Please see the Event log for more details.");
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (mDataSubjectIdentifiersFilterControl.IsValid())
            {
                IDictionary<string, object> dataSubjectIdentifiersFilter = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

                dataSubjectIdentifiersFilter = mDataSubjectIdentifiersFilterControl.GetFilter(dataSubjectIdentifiersFilter);

                var personalData = GetPersonalData(dataSubjectIdentifiersFilter);
                RenderPersonalData(personalData);

                mDataSubjectIdentifiersFilter = JsonConvert.SerializeObject(dataSubjectIdentifiersFilter, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            }
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("Data protection", "COLLECTDATA", ex);
            ShowError(ex.Message);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ChooseAndRenderSmartTip();

        RaiseNoCollectorOrEraserRegisteredWarning();

        RenderDeleteDataButton();

        CssRegistration.RegisterBootstrap(this);
    }


    private void RenderDeleteDataButton()
    {
        if (IsPageInMode(RIGHT_TO_BE_FORGOTTEN))
        {
            btnDeleteData.Visible = true;

            if (!CurrentUser.IsAuthorizedPerResource(DataProtectionModule.MODULE_NAME, "Modify"))
            {
                btnDeleteData.Enabled = false;
                btnDeleteData.ToolTipResourceString = "dataprotection.app.nomodifypermission";

                AddWarning(GetString("dataprotection.app.nomodifypermission"));
            }
            else if (PersonalDataEraserRegister.Instance.Count <= 0)
            {
                btnDeleteData.Enabled = false;
                btnDeleteData.ToolTipResourceString = "dataprotection.app.nopersonaldataeraser.tooltip";
            }
            else
            {
                RegisterDeleteDataScripts();
            }
        }
    }


    private void RegisterDeleteDataScripts()
    {
        ScriptHelper.RegisterDialogScript(this);

        var dialogQuery = $"subjectIdentifiers={HttpUtility.UrlEncode(mDataSubjectIdentifiersFilter)}";
        var dialogUrl = ApplicationUrlHelper.GetElementDialogUrl(DataProtectionModule.MODULE_NAME, ERASURE_CONFIGURATION_DIALOG_ELEMENT_NAME, additionalQuery: dialogQuery);
        var script = $@"
            function SelectDataToDelete()
            {{
                modalDialog('{dialogUrl}', 'SelectDataToDelete', '660', '590'); 
            }}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ShowDialog", script, true);

        btnDeleteData.OnClientClick = "SelectDataToDelete();return false;";
    }


    private void RenderPersonalData(string personalData)
    {
        if (!String.IsNullOrWhiteSpace(personalData))
        {
            pnlResults.Visible = true;
            txtOutput.Text = personalData;
        }
        else
        {
            pnlNoResults.Visible = true;
        }
    }


    private string GetPersonalData(IDictionary<string, object> dataSubjectIdentifiersFilter)
    {
        if (!Page.IsValid)
        {
            return null;
        }

        if (HasNoCollectorInAnyRegister())
        {
            return null;
        }

        var identities = IdentityCollectorRegister.Instance.CollectIdentities(dataSubjectIdentifiersFilter);
        var personalData = PersonalDataCollectorRegister.Instance.CollectData(identities, mOutputFormat);
        var nonEmptyPersonalData = personalData
            .Select(data => data.Text)
            .Where(text => text != null);

        return PersonalDataHelper.JoinPersonalData(nonEmptyPersonalData, mOutputFormat);
    }


    private static bool HasNoCollectorInAnyRegister()
    {
        return IdentityCollectorRegister.Instance.Count <= 0 || PersonalDataCollectorRegister.Instance.Count <= 0;
    }


    private void ChooseAndRenderSmartTip()
    {
        string headerResString = null;
        string contentResString = null;

        switch (ElementName.ToLowerInvariant())
        {
            case RIGHT_TO_ACCESS:
                headerResString = "dataprotection.righttoaccess";
                contentResString = "dataprotection.righttoaccess.tip";
                break;

            case RIGHT_TO_BE_FORGOTTEN:
                headerResString = "dataprotection.righttobeforgotten";
                contentResString = "dataprotection.righttobeforgotten.tip";
                break;

            case DATA_PORTABILITY:
                headerResString = "dataprotection.dataportability";
                contentResString = "dataprotection.dataportability.tip";
                break;
        }

        RenderSmartTip("tip-" + ElementName, GetString(headerResString), GetString(contentResString));
    }


    private void RenderSmartTip(string collapsedStateIdentifier, string header, string content)
    {
        tipDataSubjectRights.CollapsedStateIdentifier = collapsedStateIdentifier;
        tipDataSubjectRights.CollapsedHeader = header;
        tipDataSubjectRights.Content = content;
    }


    /// <summary>
    /// Raises warning in Page UI and also into EventLog that no collector or eraser has been registered.
    /// </summary>
    private void RaiseNoCollectorOrEraserRegisteredWarning()
    {
        var logService = Service.Resolve<IEventLogService>();

        if (IdentityCollectorRegister.Instance.Count <= 0)
        {
            AddWarning(string.Format(GetString("dataprotection.app.noidentitycollector"), DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_CUSTOM_COLLECTION_LINK)), "<br/><br/>");
            logService.LogWarning(
                "DATAPROTECTION",
                "NOIDENTITYCOLLECTOR",
                $"No class implementing '{typeof(IIdentityCollector).FullName}' has been registered into '{typeof(IdentityCollectorRegister).FullName}'.\n" +
                "Data protection application won't return any personal data.",
                loggingPolicy: LoggingPolicy.ONLY_ONCE);
        }

        if (PersonalDataCollectorRegister.Instance.Count <= 0)
        {
            AddWarning(string.Format(GetString("dataprotection.app.nopersonaldatacollector"), DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_CUSTOM_COLLECTION_LINK)), "<br/><br/>");
            logService.LogWarning(
                "DATAPROTECTION",
                "NOPERSONALDATACOLLECTOR",
                $"No class implementing '{typeof(IPersonalDataCollector).FullName}' has been registered into '{typeof(PersonalDataCollectorRegister).FullName}'.\n" +
                "Data protection application won't return any personal data.",
                loggingPolicy: LoggingPolicy.ONLY_ONCE);
        }

        if (PersonalDataEraserRegister.Instance.Count <= 0 && IsPageInMode(RIGHT_TO_BE_FORGOTTEN))
        {
            AddWarning(string.Format(GetString("dataprotection.app.nopersonaldataeraser"), DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_CUSTOM_ERASURE_LINK)), "<br/><br/>");
            logService.LogWarning(
                "DATAPROTECTION",
                "NOPERSONALDATAERASER",
                $"No class implementing '{typeof(IPersonalDataEraser).FullName}' has been registered into '{typeof(PersonalDataEraserRegister).FullName}'.\n" +
                "Data protection application won't erase any personal data.",
                loggingPolicy: LoggingPolicy.ONLY_ONCE);
        }
    }


    private bool IsPageInMode(string pageMode)
    {
        return ElementName.Equals(pageMode, StringComparison.OrdinalIgnoreCase);
    }


    /// <summary>
    /// Returns output format for <see cref="IPersonalDataCollector"/>s based on current UIElement this page is used on.
    /// </summary>
    private string GetOutputFormat()
    {
        switch (ElementName.ToLowerInvariant())
        {
            case RIGHT_TO_ACCESS:
            case RIGHT_TO_BE_FORGOTTEN:
                return PersonalDataFormat.HUMAN_READABLE;

            case DATA_PORTABILITY:
            default:
                return PersonalDataFormat.MACHINE_READABLE;
        }
    }
}
