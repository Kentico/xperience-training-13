using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.EventLog;
using CMS.Helpers;
using CMS.UIControls;

using Newtonsoft.Json;


[UIElement(DataProtectionModule.MODULE_NAME, "ErasureConfigurationDialog")]
public partial class CMSModules_DataProtection_Pages_ErasureConfigurationDialog : CMSModalPage
{
    private IDictionary<string, object> mDataSubjectIdentifiersFilter;
    private ErasureConfigurationControl mErasureConfigurationControl;
    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthorizedPerResource(DataProtectionModule.MODULE_NAME, "Modify"))
        {
            RedirectToAccessDenied(DataProtectionModule.MODULE_NAME, "Modify");
        }

        MessagesPlaceHolder = plcMessages;

        PageTitle.TitleText = GetString("dataprotection.app.deletedata");
        PageTitle.ShowFullScreenButton = false;
        Page.Title = PageTitle.TitleText;

        LoadErasureConfigurationControl();

        var dataSubjectIdentifiersFilterJson = QueryHelper.GetString("subjectIdentifiers", null);
        mDataSubjectIdentifiersFilter = JsonConvert.DeserializeObject<IDictionary<string, object>>(dataSubjectIdentifiersFilterJson);
    }


    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (mErasureConfigurationControl.IsValid())
            {
                IDictionary<string, object> configuration = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                configuration = mErasureConfigurationControl.GetConfiguration(configuration);

                var identities = IdentityCollectorRegister.Instance.CollectIdentities(mDataSubjectIdentifiersFilter);
                PersonalDataEraserRegister.Instance.EraseData(identities, configuration);

                ShowConfirmation(GetString("dataprotection.app.deletedata.success"), true);
            }
        }
        catch (CheckDependenciesException ex)
        {
            var dependenciesCount = 10;
            var dependencyNames = ex.Object.GetDependenciesNames(topN: dependenciesCount);

            var formatString = GetString("dataprotection.app.deletedata.objecthasdependencies");
            var message = String.Format(formatString, ex.Object.ObjectDisplayName, dependenciesCount);
            var description = String.Format(
                "{0}<br/>{1}", 
                GetString("unigrid.objectlist"),
                dependencyNames.Select(item => ResHelper.LocalizeString(item, null, true)).Join("<br/>"));

            Service.Resolve<IEventLogService>().LogException("Data protection", "DELETEPERSONALDATA", ex);
            ShowError(message, description);
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("Data protection", "DELETEPERSONALDATA", ex);
            ShowError(GetString("dataprotection.app.deletedata.fail"));
        }
    }


    private void LoadErasureConfigurationControl()
    {
        string virtualPath = DataProtectionControlsRegister.Instance.GetErasureConfigurationControl();

        try
        {
            mErasureConfigurationControl = (ErasureConfigurationControl)Page.LoadUserControl(virtualPath);
            mErasureConfigurationControl.EnableViewState = true;
            mErasureConfigurationControl.ID = "erasureConfiguration";
            mErasureConfigurationControl.ShortID = "e";

            plcErasureConfiguration.Controls.Add(mErasureConfigurationControl);
        }
        catch (Exception ex)
        {
            btnDelete.Visible = false;

            Service.Resolve<IEventLogService>().LogException("Data protection", "ERASURECONFIGURATIONCONTROLLOAD", ex, additionalMessage: $"Could not load control '{virtualPath}'.");
            ShowError($"Could not load control '{virtualPath}'. Please see the Event log for more details.");
        }
    }
}