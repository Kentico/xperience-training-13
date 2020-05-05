using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

using MessageTypeEnum = CMS.DataEngine.MessageTypeEnum;

public partial class CMSInstall_Controls_WizardSteps_AsyncProgress : CMSUserControl, ICallbackEventHandler
{
    #region "Private class progress messages"

    /// <summary>
    /// Progress messages parsed from result.
    /// </summary>
    private class ProgressMessagesHelper
    {
        #region "Variables"

        private readonly string content = null;
        private readonly string state = null;
        private readonly string storageContent = null;

        #endregion


        #region "Constructors"

        public ProgressMessagesHelper()
        {
            storageContent = PersistentStorageHelper.GetValue(STORAGE_KEY) as String;
            if (!String.IsNullOrEmpty(storageContent))
            {
                string[] items = storageContent.Split(new[] { AbstractImportExportSettings.SEPARATOR }, StringSplitOptions.None);
                state = items[0];
                content = items[1];
            }
        }

        #endregion


        public void LogInfo(string message, string sourceText, string eventCodeText, bool logToEventLog)
        {
            string value = state + AbstractImportExportSettings.SEPARATOR + message + content;
            PersistentStorageHelper.SetValue(STORAGE_KEY, value);
            if (logToEventLog)
            {
                var logData = new EventLogData(EventTypeEnum.Information, sourceText, eventCodeText)
                {
                    EventDescription = message,
                    EventUrl = RequestContext.CurrentURL
                };

                Service.Resolve<IEventLogService>().LogEvent(logData);
            }
        }


        public void LogWarning(string message, string sourceText, string eventCodeText, bool logToEventLog)
        {
            string value = state + AbstractImportExportSettings.SEPARATOR + "<span class=\"SelectorError\">" + message + "</span>" + content;
            PersistentStorageHelper.SetValue(STORAGE_KEY, value);
            if (logToEventLog)
            {
                var logData = new EventLogData(EventTypeEnum.Error, sourceText, eventCodeText)
                {
                    EventDescription = message,
                    EventUrl = RequestContext.CurrentURL
                };

                Service.Resolve<IEventLogService>().LogEvent(logData);
            }
        }


        public void LogError(string message, string sourceText, string eventCodeText, bool logToEventLog)
        {
            string value = FAILED_SIGN + AbstractImportExportSettings.SEPARATOR + "<span class=\"SelectorError\">" + message + "</span>" + content;
            PersistentStorageHelper.SetValue(STORAGE_KEY, value);
            if (logToEventLog)
            {
                var logData = new EventLogData(EventTypeEnum.Error, sourceText, eventCodeText)
                {
                    EventDescription = message,
                    EventUrl = RequestContext.CurrentURL
                };

                Service.Resolve<IEventLogService>().LogEvent(logData);
            }
        }


        public void LogQuit(string message, string sourceText, string eventCodeText, bool logToEventLog)
        {
            string value = QUIT_SIGN + AbstractImportExportSettings.SEPARATOR + message + content;
            PersistentStorageHelper.SetValue(STORAGE_KEY, value);
            if (logToEventLog)
            {
                var logData = new EventLogData(EventTypeEnum.Information, sourceText, eventCodeText)
                {
                    EventDescription = message,
                    EventUrl = RequestContext.CurrentURL
                };

                Service.Resolve<IEventLogService>().LogEvent(logData);
            }
        }


        public string GetContent()
        {
            if (DatabaseSeparationHelper.SeparationStartedByServer != SystemContext.MachineName)
            {
                return AbstractImportExportSettings.SEPARATOR + ResHelper.GetString("separationDB.differentserver");
            }
            return storageContent;
        }
    }

    #endregion


    #region "Variables and constants"

    private const string FAILED_SIGN = "F";
    private const string QUIT_SIGN = "Q";
    private const string STORAGE_KEY = "SeparateDBProgressLog";
    private const string SEPARATION = "Contact management database separation";
    private const string DBJOIN = "Contact management database join";
    private const string SEPARATE = "SEPARATE";
    private const string JOIN = "JOIN";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Client ID of 'Next' button of a wizard control.
    /// </summary>
    public string NextButtonClientID
    {
        get;
        set;
    }


    /// <summary>
    /// Client ID of 'Previous' button of a wizard control.
    /// </summary>
    public string PreviousButtonClientID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control is used for separation or DB join.
    /// </summary>
    public bool IsSeparation
    {
        get;
        set;
    }


    /// <summary>
    /// Event log - source text.
    /// </summary>
    private string SourceText
    {
        get
        {
            return IsSeparation ? SEPARATION : DBJOIN;
        }
    }


    /// <summary>
    /// Event log - event code text.
    /// </summary>
    private string EventCodeText
    {
        get
        {
            return IsSeparation ? SEPARATE : JOIN;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterJS();
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Logs message to for asynchronous displaying.
    /// </summary>
    /// <param name="line">Message to be displayed</param>
    /// <param name="type">Type of a message</param>
    /// <param name="logToEventLog">Log to event log</param>
    public void LogMessage(string line, MessageTypeEnum type, bool logToEventLog)
    {
        if (!line.EndsWithCSafe("<br />", StringComparison.InvariantCultureIgnoreCase))
        {
            line = line + "<br />";
        }
        ProgressMessagesHelper progressMessages = new ProgressMessagesHelper();
        switch (type)
        {
            case MessageTypeEnum.Info:
                progressMessages.LogInfo(line, SourceText, EventCodeText, logToEventLog);
                break;

            case MessageTypeEnum.Warning:
                progressMessages.LogWarning(line, SourceText, EventCodeText, logToEventLog);
                break;

            case MessageTypeEnum.Error:
                progressMessages.LogError(line, SourceText, EventCodeText, logToEventLog);
                break;

            case MessageTypeEnum.Finished:
                progressMessages.LogQuit(line, SourceText, EventCodeText, logToEventLog);
                break;
        }
    }


    /// <summary>
    /// Start asynchronous logging.
    /// </summary>
    public void StartLogging()
    {
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "StartLogging", ScriptHelper.GetScript("StartTimer();"));
    }


    /// <summary>
    /// Displays message immediately.
    /// </summary>
    /// <param name="message">Message to be displayed</param>
    public void DisplayInfoMessage(string message)
    {
        LogMessage(message, MessageTypeEnum.Info, false);
        lblProgress.Text = message + lblProgress.Text;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Javascript functions.
    /// </summary>
    private void RegisterJS()
    {
        string jsFunctions =
@"
function CallServer(argument){
    if (getBusy)  return; 
    getBusy = true; 
    setTimeout(""getBusy = false;"", 2000); 
    if(window.Activity) {
        window.Activity();
    }
    return " + Page.ClientScript.GetCallbackEventReference(this, "argument", "SetProgress", null, true) + @";
}
";

        jsFunctions +=
@"
function BreakProgress(success)
{
    StopTimer();
    if (window.location.href.indexOf('returnurl=') > 0) 
    {
        BTN_Enable('" + NextButtonClientID + @"');
    }
    else
    {
        if (success)
        {
            BTN_Enable('" + NextButtonClientID + @"');
            BTN_Disable('" + PreviousButtonClientID + @"');
        }
        else
        {
            BTN_Disable('" + NextButtonClientID + @"');
            BTN_Enable('" + PreviousButtonClientID + @"');
        }
    }
}


function SetProgress(rValue, context){
    var messageElement = document.getElementById('" + lblProgress.ClientID + @"');

    getBusy = false;
    if (rValue != '') {
        var values = rValue.split('" + AbstractImportExportSettings.SEPARATOR + @"');
         messageElement.innerHTML = values[1];

        if((values == '') || (values[0] == '" + FAILED_SIGN + @"')) {
            BreakProgress(false);
        }
        if (values[0] == '" + QUIT_SIGN + @"'){
            BreakProgress(true);
        }
    }
}
";

        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "InstallFunctions", ScriptHelper.GetScript(jsFunctions));
    }

    #endregion


    #region "ICallbackEventHandler Members"

    public string GetCallbackResult()
    {
        return new ProgressMessagesHelper().GetContent();
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
    }

    #endregion
}
