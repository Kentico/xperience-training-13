using System;
using System.Collections.Generic;

using CMS.Base;

using System.Security.Principal;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContinuousIntegration;
using CMS.Core;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


[UIElement("CMS.ContinuousIntegration", "ContinuousIntegration")]
public partial class CMSModules_ContinuousIntegration_Pages_Settings : GlobalAdminPage
{
    #region "Nested classes"

    /// <summary>
    /// Wrapper for serialize all operation result and its cancellation token.
    /// </summary>
    private class SerializationOperationState
    {
        /// <summary>
        /// Result of serialize all operation.
        /// </summary>
        public RepositoryActionResult Result
        {
            get;
            set;
        }


        /// <summary>
        /// Cancellation token of serialize all operation.
        /// </summary>
        public CancellationTokenSource CancellationToken
        {
            get;
            set;
        }
    }

    #endregion


    #region "Variables and properties"

    private const string CI_SETTINGS_KEY = "CMSEnableCI";
    private const string CONTINUOUS_INTEGRATION = "Continuous Integration";
    private const string SERIALIZATION_CANCELLED_EVENT_CODE = "SerializationCancelled";
    private const string SERIALIZATION_FAILED_EVENT_CODE = "SerializationFailed";
    private const string SERIALIZATION_SUCCESSFUL_EVENT_CODE = "SerializationSuccessful";

    private readonly string[] mSettingKeyNames = { CI_SETTINGS_KEY };
    private IEnumerable<SettingsKeyInfo> mSettingKeys;
    private HeaderAction mSerializeAction;


    /// <summary>
    /// Gets enumeration of SettingKeyInfos shown in the form.
    /// </summary>
    private IEnumerable<SettingsKeyInfo> SettingKeys
    {
        get
        {
            return mSettingKeys ?? (mSettingKeys = SettingsKeyInfoProvider.GetSettingsKeys().WhereIn("KeyName", mSettingKeyNames));
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Load page life-cycle event.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        LoadForm();

        InitAsyncLog();

        // Add Serialize all objects button
        mSerializeAction = new HeaderAction
        {
            Text = GetString("ci.serializeallobjects"),
            CommandName = "serialize",
            ButtonStyle = ButtonStyle.Default
        };
        HeaderActions.AddAction(mSerializeAction);
        HeaderActions.ActionPerformed += ActionPerformed;

        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");

        ShowInformation(GetString("ci.performancemessage"));
    }


    /// <summary>
    /// PreRender page life-cycle event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!SettingsKeyInfoProvider.GetBoolValue(CI_SETTINGS_KEY))
        {
            mSerializeAction.Enabled = false;
        }
    }


    /// <summary>
    /// Handles after save event of the form. All form data are stored to the database.
    /// </summary>
    private void OnAfterSave(object sender, EventArgs eventArgs)
    {
        foreach (var key in mSettingKeyNames)
        {
            SettingsKeyInfoProvider.SetGlobalValue(key, form.Data[key], false);
        }
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    private void ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                form.SaveData(null);
                break;

            case "serialize":
                StartSerialization();
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the form and loads its data.
    /// </summary>
    private void LoadForm()
    {
        InitForm();

        var data = form.FormInformation.GetDataRow();
        foreach (var settingKey in SettingKeys)
        {
            data[settingKey.KeyName] = settingKey.KeyValue;
        }

        form.LoadData(data);
    }


    /// <summary>
    /// Initializes the form.
    /// </summary>
    private void InitForm()
    {
        var fi = new FormInfo();
        foreach (var settingKey in SettingKeys)
        {
            fi.AddFormItem(CreateFormFieldInfo(settingKey));
        }

        form.FormInformation = fi;
        form.EnsureMessagesPlaceholder(MessagesPlaceHolder);
        form.OnAfterSave += OnAfterSave;
    }


    /// <summary>
    /// Initializes the async log control.
    /// </summary>
    private void InitAsyncLog()
    {
        ctlAsyncLog.OnFinished += OnFinished;
        ctlAsyncLog.OnError += OnError;
        ctlAsyncLog.OnCancel += OnCancel;

        ctlAsyncLog.CancelAction = Cancel;
        ctlAsyncLog.TitleText = GetString("ci.serializingobjects");
    }


    /// <summary>
    /// Creates new FormFieldInfo from given SettingKeyInfo.
    /// </summary>
    /// <param name="setting">Setting key info.</param>
    private FormFieldInfo CreateFormFieldInfo(SettingsKeyInfo setting)
    {
        var ffi = new FormFieldInfo
        {
            Name = setting.KeyName,
            DataType = setting.KeyType
        };

        ffi.SetControlName(FormHelper.GetFormFieldDefaultControlName(setting.KeyType));
        ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, setting.KeyDisplayName);
        ffi.SetPropertyValue(FormFieldPropertyEnum.ExplanationText, setting.KeyExplanationText);
        ffi.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, setting.KeyDefaultValue);
        if (!string.IsNullOrEmpty(setting.KeyDescription))
        {
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCssClass, "form-group settings-group-inline");
            ffi.SetPropertyValue(FormFieldPropertyEnum.ContentAfter, UIHelper.GetIcon("icon-question-circle", GetString(setting.KeyDescription)).GetRenderedHTML());
        }

        return ffi;
    }


    /// <summary>
    /// Cancels running serialization.
    /// </summary>
    private void Cancel()
    {
        var state = ctlAsyncLog.ProcessData.Data as SerializationOperationState;
        if (state == null)
        {
            ShowError(GetString("ci.serialization.failed"));

            return;
        }

        state.CancellationToken.Cancel();
    }


    /// <summary>
    /// Starts serialization in separate thread.
    /// </summary>
    private void StartSerialization()
    {
        pnlLog.Visible = true;
        ctlAsyncLog.EnsureLog();
        ctlAsyncLog.RunAsync(SerializeAllObjects, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Serializes all supported objects to file system.
    /// </summary>
    /// <param name="parameters">AsyncAction parameters (not used in this case)</param>
    private void SerializeAllObjects(object parameters)
    {
        var state = new SerializationOperationState
        {
            CancellationToken = new CancellationTokenSource()
        };

        ctlAsyncLog.ProcessData.AllowUpdateThroughPersistentMedium = false;
        ctlAsyncLog.ProcessData.Data = state;

        state.Result = FileSystemRepositoryManager.StoreAll(
            logItem => ctlAsyncLog.AddLog(HTMLHelper.HTMLEncode(logItem.Message)),
            state.CancellationToken.Token);
    }


    /// <summary>
    /// Returns combined message obtained from given resource and from general messages informing about detailed event log.
    /// </summary>
    private static string GetMessageCombinedWithEventLogInfo(string resourceKey)
    {
        return String.Format("{0} {1}", GetString(resourceKey), GetString("general.seeeventlog"));
    }


    /// <summary>
    /// Saves entire continuous integration log as one entry to event log.
    /// If null or empty description is passed, nothing is logged!
    /// </summary>
    /// <param name="eventType">Event log severity</param>
    /// <param name="eventCode">Event code</param>
    /// <param name="description">Event description</param>
    private void SaveIntegrationLogToEventLog(EventTypeEnum eventType, string eventCode, string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return;
        }

        var logData = new EventLogData(eventType, CONTINUOUS_INTEGRATION, eventCode)
        {
            EventDescription = description,
            EventUrl = RequestContext.RawURL,
            UserID = CurrentUser.UserID,
            UserName = CurrentUser.UserName,
            IPAddress = RequestContext.UserHostAddress,
            EventMachineName = SystemContext.MachineName,
            EventUrlReferrer = RequestContext.URLReferrer,
            EventUserAgent = RequestContext.UserAgent,
            EventTime = DateTime.Now,
            LoggingPolicy = LoggingPolicy.DEFAULT
        };

        Service.Resolve<IEventLogService>().LogEvent(logData);
    }


    /// <summary>
    /// Returns errors description combined from given repository result and asynchronous log
    /// </summary>
    private string GetCombinedErrorDescription(RepositoryActionResult result)
    {
        var errors = new List<string>();
        if (result != null)
        {
            errors.AddRange(result.Errors);
        }

        if (!string.IsNullOrEmpty(ctlAsyncLog.Log))
        {
            errors.Add(GetRawLogMessage());
        }

        return string.Join(Environment.NewLine, errors);
    }


    /// <summary>
    /// Hides dialog.
    /// </summary>
    private void HideDialog()
    {
        pnlLog.Visible = false;
    }


    /// <summary>
    /// Gets the log message in raw format.
    /// </summary>
    private string GetRawLogMessage()
    {
        return HTMLHelper.HTMLDecode(ctlAsyncLog.Log);
    }

    #endregion


    #region "Handling async thread"

    private void OnCancel(object sender, EventArgs e)
    {
        ShowError(GetMessageCombinedWithEventLogInfo("ci.serialization.canceled"));
        SaveIntegrationLogToEventLog(EventTypeEnum.Error, SERIALIZATION_CANCELLED_EVENT_CODE, GetRawLogMessage());
        HideDialog();
    }


    private void OnError(object sender, EventArgs e)
    {
        AddError(GetMessageCombinedWithEventLogInfo("ci.serialization.failed"));
        SaveIntegrationLogToEventLog(EventTypeEnum.Error, SERIALIZATION_FAILED_EVENT_CODE, GetRawLogMessage());
        HideDialog();
    }


    private void OnFinished(object sender, EventArgs e)
    {
        var state = ctlAsyncLog.ProcessData.Data as SerializationOperationState;
        var result = (state == null) ? null : state.Result;

        if (result != null && result.Success)
        {
            ShowConfirmation(GetMessageCombinedWithEventLogInfo("ci.msg.serializationsuccessful"), true);
            SaveIntegrationLogToEventLog(EventTypeEnum.Information, SERIALIZATION_SUCCESSFUL_EVENT_CODE, GetRawLogMessage());
        }
        else
        {
            ShowError(GetMessageCombinedWithEventLogInfo("ci.serialization.failed"));
            SaveIntegrationLogToEventLog(EventTypeEnum.Error, SERIALIZATION_FAILED_EVENT_CODE,
                GetCombinedErrorDescription(result));
        }

        HideDialog();
    }

    #endregion
}
