using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

namespace CMSApp.CMSAdminControls.UI.UniGrid.Pages
{
    public partial class CMSAdminControls_UI_UniGrid_Pages_MassDelete : CMSAdministrationPage, ICallbackEventHandler
    {
        #region "Constants and Properties"

        // Represents the number of records shown in list of object which will be deleted
        private const int SHOWN_RECORDS_NUMBER = 500;


        private string mParametersKey;


        /// <summary>
        /// All errors occurred during deletion.
        /// </summary>
        private string CurrentError
        {
            get
            {
                return ctlAsyncLog.ProcessData.Error;
            }
            set
            {
                ctlAsyncLog.ProcessData.Error = value;
            }
        }


        /// <summary>
        /// Identifiers of info objects which will be deleted.
        /// </summary>
        private ICollection<int> InfoIds
        {
            get;
            set;
        }


        /// <summary>
        /// Type of objects which will be deleted.
        /// </summary>
        private string ObjectType
        {
            get;
            set;
        }


        /// <summary>
        /// <see cref="ObjectQuery"/> of info objects which will be deleted.
        /// </summary>
        private ObjectQuery ObjectQuery
        {
            get;
            set;
        }


        /// <summary>
        /// Key used to retrieve stored parameters from session.
        /// </summary>
        private string ParametersKey
        {
            get
            {
                return mParametersKey ?? (mParametersKey = QueryHelper.GetString("parameters", String.Empty));
            }
        }


        /// <summary>
        /// Displayable name of type of info which will be deleted.
        /// </summary>
        private string ObjectTypeDisplayableName
        {
            get;
            set;
        }


        /// <summary>
        /// ID column of the object type.
        /// </summary>
        private string ObjectIdColumn
        {
            get;
            set;
        }


        /// <summary>
        /// JavaScript for parent page reload when delete is shown in modal dialog.
        /// </summary>
        private string ReloadScript
        {
            get;
            set;
        }

        #endregion


        #region "Page events and event handlers"

        protected void Page_Init(object sender, EventArgs e)
        {
            // Set message placeholder
            if (CurrentMaster != null)
            {
                CurrentMaster.MessagesPlaceHolder = pnlMessagePlaceholder;
            }

            // Register save handler and closing JavaScript 
            var master = CurrentMaster as ICMSModalMasterPage;
            if (master != null)
            {
                master.ShowSaveAndCloseButton();
                master.SetSaveResourceString("general.delete");
                master.Save += btnDelete_OnClick;

                master.SetCloseJavaScript("ReloadAndCallback();");
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            QueryHelper.ValidateHash("hash", settings: new HashSettings("") { Redirect = true });

            if (RequestHelper.IsCallback() || !InitializeProperties())
            {
                return;
            }

            ScriptHelper.RegisterWOpenerScript(Page);
            RegisterCallbackScript();

            SetAsyncLogParameters();

            TogglePanels(showContent: true);
            LoadInfosView();
        }


        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            TogglePanels(showContent: false);
            ctlAsyncLog.EnsureLog();
            ctlAsyncLog.RunAsync(DeleteInfos, WindowsIdentity.GetCurrent());
        }


        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            ReturnToListing();
        }


        private void OnFinished(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentError))
            {
                ShowError(GetString("massdelete.errormessage"), description: CurrentError);
                LoadInfosView();
            }
            else
            {
                ReturnToListing();
            }
        }


        private void OnCancel(object sender, EventArgs e)
        {
            string canceled = GetString("massdelete.cancel");
            AddLog(canceled);

            LoadInfosView();
            ShowWarning(canceled);
        }

        #endregion


        #region "Methods"

        /// <summary>
        /// Sets visibility of content panel and log panel.
        /// Only one can be shown at the time.
        /// </summary>
        private void TogglePanels(bool showContent)
        {
            pnlContent.Visible = showContent;
            pnlLog.Visible = !showContent;
        }


        /// <summary>
        /// Registers a callback script that clears session when dialog is closed.
        /// </summary>
        private void RegisterCallbackScript()
        {
            String callbackEventReference = Page.ClientScript.GetCallbackEventReference(this, String.Empty, "CloseDialog", String.Empty);
            string closeJavaScript = String.Format("function ReloadAndCallback() {{ wopener.{0}; {1} }}", ReloadScript, callbackEventReference);
            ScriptHelper.RegisterClientScriptBlock(Page, GetType(), "ReloadAndCallback", closeJavaScript, true);
        }


        /// <summary>
        /// Sets parameters of <see cref="AsyncControl"/> dialog.
        /// </summary>
        private void SetAsyncLogParameters()
        {
            var titleText = GetString("massdelete.deleting");
            ctlAsyncLog.TitleText = String.Format(titleText, ObjectTypeDisplayableName);

            ctlAsyncLog.OnFinished += OnFinished;
            ctlAsyncLog.OnCancel += OnCancel;
        }


        /// <summary>
        /// Retrieves parameters from the session.
        /// Returns <c>true</c> when all properties were set successfully.
        /// </summary>
        private bool InitializeProperties()
        {
            var parameters = WindowHelper.GetItem(ParametersKey) as MassActionParameters;
            if (parameters == null)
            {
                HandleInvalidParameters("There were no parameters found under " + ParametersKey + " key.");
                return false;
            }

            InfoIds = (parameters.IDs ?? Enumerable.Empty<int>()).ToList();
            ObjectType = parameters.ObjectType;
            ReloadScript = parameters.ReloadScript;

            if (!AreParametersValid())
            {
                HandleInvalidParameters("One or more parameters are invalid:" + Environment.NewLine + parameters);
                return false;
            }

            return InitializeObjectQuery();
        }


        /// <summary>
        /// Returns <c>true</c> if and only if all session parameters are valid.
        /// </summary>
        private bool AreParametersValid()
        {
            return InfoIds != null
                && InfoIds.Any()
                && !String.IsNullOrEmpty(ObjectType)
                && !String.IsNullOrEmpty(ReloadScript);
        }


        /// <summary>
        /// Initializes ObjectQuery from ObjectType property.
        /// </summary>
        private bool InitializeObjectQuery()
        {
            try
            {
                // If type info is not original (e.g. a UserListInfo), use the original one (e.g. UserInfo)
                ObjectType = ObjectTypeManager.GetTypeInfo(ObjectType).OriginalObjectType;

                ObjectQuery = new ObjectQuery(ObjectType).Immutable();
                ObjectIdColumn = ObjectQuery.TypeInfo.IDColumn;
                ObjectTypeDisplayableName = ObjectQuery.TypeInfo.GetNiceObjectTypeName();
            }
            catch (Exception exception)
            {
                Service.Resolve<IEventLogService>().LogException(EventType.ERROR, "Delete page", exception);
                RedirectToInformation(GetString("massdelete.invalidparameters"));
                return false;
            }

            return true;
        }


        /// <summary>
        /// Shows and logs error.
        /// </summary>
        private void HandleInvalidParameters(string eventDescription)
        {
            Service.Resolve<IEventLogService>().LogError("Delete page", "InvalidSessionParameter", eventDescription);
            RedirectToInformation(GetString("massdelete.invalidparameters"));
        }


        /// <summary>
        /// Shows info objects which are about to be deleted by the user. 
        /// </summary>
        private void LoadInfosView()
        {
            // Show title and question 
            var announcement = GetString("massdelete.announcement");
            headAnnouncement.ResourceString = String.Format(announcement, ObjectTypeDisplayableName);
            var titleText = GetString("massdelete.title");
            PageTitle.TitleText = String.Format(titleText, ObjectTypeDisplayableName);

            // Add element to HTML
            lblItems.Text = GetDisplayableInfoNames();
        }


        /// <summary>
        /// Builds HTML string with names of chosen info objects.
        /// </summary>
        private string GetDisplayableInfoNames()
        {
            var builder = new StringBuilder();
            AppendLimitMessage(builder);

            // Appends names of info objects which are about to delete 
            IEnumerable<string> infoNames = GetInfoNames();
            foreach (var name in infoNames)
            {
                builder.AppendFormat("<div>{0}</div>{1}", HTMLHelper.HTMLEncode(name), Environment.NewLine);
            }

            // If message is not empty set panel visible
            if (builder.Length > 1)
            {
                pnlItemList.Visible = true;
            }

            return builder.ToString();
        }


        /// <summary>
        /// Eventually appends a message which is shown when more than <see cref="SHOWN_RECORDS_NUMBER"/> info objects are about to be deleted.
        /// </summary>
        private void AppendLimitMessage(StringBuilder builder)
        {
            if (InfoIds.Count <= SHOWN_RECORDS_NUMBER)
            {
                return;
            }

            var moreThanMax = String.Format(@"
                <div>
                    <b>{0}</b>
                </div>
                <br />",
                GetString("massdelete.showlimit"));

            builder.AppendFormat(moreThanMax, SHOWN_RECORDS_NUMBER, InfoIds.Count);
        }


        /// <summary>
        /// Returns names of selected info objects.
        /// </summary>
        private IEnumerable<string> GetInfoNames()
        {
            return GetInfosToDelete()
                .Take(SHOWN_RECORDS_NUMBER)
                .ToList()
                .Where(info => info.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
                .Select(info => info.Generalized.ObjectDisplayName);
        }


        /// <summary>
        /// Handles deleting of selected info objects within asynchronous dialog.
        /// </summary>
        private void DeleteInfos(object parameter)
        {
            var errorLog = new StringBuilder();

            using (var logProgress = new LogContext())
            using (var logPermissionError = new LogContext())
            {
                GetInfosToDelete()
                    .ForEachObject(info =>
                    {
                        if (!info.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
                        {
                            // Prevent displaying deletion error for info objects that user does not have permission to read
                            return;
                        }

                        DeleteSingleInfo(info, errorLog, logProgress, logPermissionError);
                    });
            }

            if (errorLog.Length != 0)
            {
                CurrentError = errorLog.ToString();
            }
        }


        /// <summary>
        /// Deletes single info object. Handles logging to event log and to deleting page.
        /// </summary>
        /// <param name="info">Info which will be deleted</param>
        /// <param name="errorLog"> Log where errors will be recorded</param>
        /// <param name="logProgress">Log where progress will be recorded</param>
        /// <param name="logPermissionError">Log where security errors will be recorded</param>
        private void DeleteSingleInfo(BaseInfo info, StringBuilder errorLog, LogContext logProgress, LogContext logPermissionError)
        {
            var displayableName = String.Empty;
            try
            {
                // Prevent XSS attack
                displayableName = HTMLHelper.HTMLEncode(info.Generalized.ObjectDisplayName);

                using (new CMSActionContext { LogEvents = false })
                {
                    if (info.CheckPermissions(PermissionsEnum.Delete, CurrentSiteName, CurrentUser, exceptionOnFailure: true))
                    {
                        info.Delete();
                    }
                }

                AddSuccessLog(logProgress, displayableName);
            }
            catch (ThreadAbortException)
            {
                // Do not log any exception to event log for ThreadAbortException
            }
            catch (Exception exception)
            {
                HandleException(errorLog, logPermissionError, displayableName, exception);
            }
        }


        /// <summary>
        /// Adds error message to the <paramref name="errorLog"/> 
        /// and either accumulates permission-related exceptions to the <paramref name="logPermissionError"/> 
        /// or logs other <see cref="Exception"/> directly to the event log.
        /// </summary>
        private void HandleException(StringBuilder errorLog, LogContext logPermissionError, string displayableName, Exception exception)
        {
            AddErrorLog(errorLog, displayableName);

            if (IsPermissionRelated(exception))
            {
                logPermissionError.LogEvent(EventType.ERROR, ObjectTypeDisplayableName, "DELETEOBJ", exception.Message, RequestContext.RawURL, CurrentUser.UserID, CurrentUser.UserName, 0, null, RequestContext.UserHostAddress, SiteContext.CurrentSiteID, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);
            }
            else
            {
                Service.Resolve<IEventLogService>().LogException("Application_Error", "DELETEOBJ", exception);
            }
        }


        /// <summary>
        /// Returns <c>true</c> if <paramref name="exception"/> is related to a user not being authorized to delete the info object or some of its dependencies.
        /// </summary>
        /// <remarks>
        /// <see cref="SecurityException"/> is considered permission-related in this context because some info objects throw
        ///  this <see cref="Exception"/> during <see cref="BaseInfo.CheckPermissions"/> method call.
        /// </remarks>
        private static bool IsPermissionRelated(Exception exception)
        {
            return (exception is SecurityException) || (exception is PermissionCheckException) || (exception is CheckDependenciesException);
        }


        /// <summary>
        /// Logs successful delete.
        /// </summary>
        /// <param name="logProgress">Log where successful delete will be recorded</param>
        /// <param name="displayableName">Name of successfully deleted item</param>
        private void AddSuccessLog(LogContext logProgress, string displayableName)
        {
            AddLog(displayableName);
            string deletedMessage = String.Format(GetString("massdelete.wasdeleted"), ObjectTypeDisplayableName, displayableName);
            logProgress.LogEvent(EventType.INFORMATION, ObjectTypeDisplayableName, "DELETEOBJ", deletedMessage, RequestContext.RawURL, CurrentUser.UserID, CurrentUser.UserName, 0, null, RequestContext.UserHostAddress, SiteContext.CurrentSiteID, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);
        }


        /// <summary>
        /// Appends the error information to errorLog.
        /// </summary>
        /// <param name="errorLog">Log where errors will be recorded</param>
        /// <param name="displayableName">Name of info where error occurred</param>
        private void AddErrorLog(StringBuilder errorLog, string displayableName)
        {
            var currentError = GetString("massdelete.unabletodelete");
            currentError = String.Format(currentError, ObjectTypeDisplayableName, displayableName);
            AddLog(currentError);
            errorLog.AppendFormat("<div>{0}</div>{1}", currentError, Environment.NewLine);
        }


        /// <summary>
        /// Adds the log information.
        /// </summary>
        /// <param name="newLog">New log information</param>
        private void AddLog(string newLog)
        {
            ctlAsyncLog.AddLog(newLog);
        }
        

        /// <summary>
        /// Gets current info objects from database.
        /// </summary>
        private ObjectQuery GetInfosToDelete()
        {
            return ObjectQuery
                .WhereIn(ObjectIdColumn, InfoIds);
        }


        /// <summary>
        /// Redirects back to parent listing.
        /// </summary>
        private void ReturnToListing()
        {
            WindowHelper.Remove(ParametersKey);

            var script = @"wopener." + ReloadScript + "; CloseDialog();";
            ScriptHelper.RegisterStartupScript(Page, GetType(), "ReloadGridAndClose", script, addScriptTags: true);
        }

        #endregion


        #region "ICallbackEventHandler Members"

        public void RaiseCallbackEvent(string eventArgument)
        {
            // Raised when Close button in the dialog is clicked, so the parameters can be cleared from session
            WindowHelper.Remove(ParametersKey);
        }


        public string GetCallbackResult()
        {
            // CloseDialog JavaScript method is called to receive the callback results, thus no data needs to be passed to it
            return String.Empty;
        }

        #endregion
    }
}