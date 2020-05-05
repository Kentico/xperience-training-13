using System;
using System.Linq;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DeviceProfiles;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_DeviceProfiles_Controls_PageLayouts : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    /// <summary>
    /// The current device profile to edit.
    /// </summary>
    private DeviceProfileInfo mDeviceProfile = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the current device profile to edit.
    /// </summary>
    protected DeviceProfileInfo DeviceProfile
    {
        get
        {
            if (mDeviceProfile == null)
            {
                mDeviceProfile = UIContext.EditedObject as DeviceProfileInfo;
            }
            return mDeviceProfile;
        }
    }

    #endregion


    #region "Life-cycle methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterDialogScript(Page);

        BindingGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(BindingGrid_OnExternalDataBound);
    }


    /// <summary>
    /// OnLoad override, setup access denied page with dependence on current usage.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        BindingGrid.GridView.ShowHeader = false;
        BindingGrid.GridView.BorderWidth = 0;
    }


    /// <summary>
    /// Bindings the grid_ on external data bound.
    /// </summary>
    protected object BindingGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        int sourceLayoutId = ValidationHelper.GetInteger(parameter, 0);
        return ExecuteFunction(() => CreateLayoutBindingControl(sourceLayoutId));
    }


    /// <summary>
    /// Processes an event raised when a form is posted to the server.
    /// </summary>
    /// <param name="eventArgument">An event argument.</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        ExecuteClientRequest(eventArgument);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates a new instance of control that displays layout mapping for the specified layout, and returns it.
    /// </summary>
    /// <param name="sourceLayoutId">An identifier of the source layout.</param>
    /// <returns>A new instance of control that displays layout mapping for the specified layout.</returns>
    private CMSModules_DeviceProfiles_Controls_LayoutBinding CreateLayoutBindingControl(int sourceLayoutId)
    {
        CMSModules_DeviceProfiles_Controls_LayoutBinding control = LoadControl("~/CMSModules/DeviceProfiles/Controls/LayoutBinding.ascx") as CMSModules_DeviceProfiles_Controls_LayoutBinding;
        control.SourceLayout = LayoutInfoProvider.GetLayoutInfo(sourceLayoutId);
        control.DeviceProfile = DeviceProfile;

        return control;
    }


    /// <summary>
    /// Processes the specified action from the client.
    /// </summary>
    /// <remarks>
    /// The page contains a simple client API.
    /// If an API function is called, the arguments are stored in hidden fields and postback is executed.
    /// </remarks>
    /// <param name="action">An action name.</param>
    private void ExecuteClientRequest(string action)
    {
        if (!StopProcessing)
        {
            switch (action)
            {
                case "SetTargetLayout":
                    ExecuteClientRequest(SetTargetLayout, "device_profile.layoutmapping.confirmations.set", "device_profile.layoutmapping.errors.set");
                    break;
                case "UnsetTargetLayout":
                    ExecuteClientRequest(UnsetTargetLayout, "device_profile.layoutmapping.confirmations.unset", "device_profile.layoutmapping.errors.unset");
                    break;
            }
        }
    }


    /// <summary>
    /// Processes the specified action from the client.
    /// </summary>
    /// <remarks>
    /// The specified delegate is invoked and an information message is displayed.
    /// If there is an exception, the request processing is stopped and an error message is displayed.
    /// </remarks>
    /// <param name="request">A delegate to execute.</param>
    /// <param name="confirmationResourceName">A resource name of the message that will be displayed in case of success.</param>
    /// <param name="errorResourceName">A resource name of the message that will be displayed in case of error.</param>
    private void ExecuteClientRequest(Action request, string confirmationResourceName, string errorResourceName)
    {
        try
        {
            request.Invoke();
            ShowConfirmation(GetString(confirmationResourceName));
        }
        catch (ApplicationException exception)
        {
            ShowError(exception.Message);
            StopProcessing = true;
        }
        catch (Exception exception)
        {
            string text = GetString(errorResourceName);
            if (SystemContext.DevelopmentMode)
            {
                ShowError(text, null, exception.Message ?? exception.ToString());
            }
            else
            {
                ShowError(text);
            }
            StopProcessing = true;
        }
    }


    /// <summary>
    /// Executes the SetTragetLayout client action using parameters passed in hidden fields.
    /// </summary>
    private void SetTargetLayout()
    {
        int sourceLayoutId = ValidationHelper.GetInteger(SourceLayoutIdentifierHiddenField.Value, 0);
        int targetLayoutId = ValidationHelper.GetInteger(TargetLayoutIdentifierHiddenField.Value, 0);
        using (var scope = new CMSTransactionScope())
        {
            LayoutInfo sourceLayout = LayoutInfoProvider.GetLayoutInfo(sourceLayoutId);
            if (sourceLayout == null)
            {
                throw new ApplicationException(GetString("device_profile.layoutmapping.errors.nosourcelayout"));
            }
            LayoutInfo targetLayout = LayoutInfoProvider.GetLayoutInfo(targetLayoutId);
            if (targetLayout == null)
            {
                throw new ApplicationException(GetString("device_profile.layoutmapping.errors.notargetlayout"));
            }

            var bindings = DeviceProfileLayoutInfoProvider.GetDeviceProfileLayouts()
                .Where("DeviceProfileID", QueryOperator.Equals, DeviceProfile.ProfileID)
                .Where("SourceLayoutID", QueryOperator.Equals, sourceLayout.LayoutId)
                .ToList();
            
            DeviceProfileLayoutInfo binding = null;
            if (bindings.Count > 0)
            {
                binding = bindings[0];
            }
            else
            {
                binding = new DeviceProfileLayoutInfo
                {
                    DeviceProfileID = DeviceProfile.ProfileID,
                    SourceLayoutID = sourceLayout.LayoutId
                };
            }
            binding.TargetLayoutID = targetLayout.LayoutId;
            DeviceProfileLayoutInfoProvider.SetDeviceProfileLayoutInfo(binding);
            scope.Commit();
        }
    }


    /// <summary>
    /// Executes the UnsetTragetLayout client action using parameters passed in hidden fields.
    /// </summary>
    private void UnsetTargetLayout()
    {
        int sourceLayoutId = ValidationHelper.GetInteger(SourceLayoutIdentifierHiddenField.Value, 0);
        using (var scope = new CMSTransactionScope())
        {
            LayoutInfo sourceLayout = LayoutInfoProvider.GetLayoutInfo(sourceLayoutId);
            if (sourceLayout == null)
            {
                throw new ApplicationException(GetString("device_profile.layoutmapping.errors.nosourcelayout"));
            }

            var bindings = DeviceProfileLayoutInfoProvider.GetDeviceProfileLayouts()
                .WhereEquals("DeviceProfileID", DeviceProfile.ProfileID)
                .WhereEquals("SourceLayoutID", sourceLayout.LayoutId)
                .ToList();

            if (bindings.Count > 0)
            {
                DeviceProfileLayoutInfoProvider.DeleteDeviceProfileLayoutInfo(bindings[0]);
            }
            scope.Commit();
        }
    }


    /// <summary>
    /// Invokes the specified delegate safely, and returns the result.
    /// </summary>
    /// <remarks>
    /// The specified delegate is invoked only if the request processing is not stopped.
    /// If there is an exception during invocation, the request processing is stopped and an error message is displayed.
    /// </remarks>
    /// <typeparam name="T">Type of delegate result.</typeparam>
    /// <param name="function">A delegate to invoke.</param>
    /// <returns>The result of delegate invocation.</returns>
    private T ExecuteFunction<T>(Func<T> function)
    {
        if (!StopProcessing)
        {
            try
            {
                return function.Invoke();
            }
            catch (Exception exception)
            {
                string text = GetString("general.exception");
                if (SystemContext.DevelopmentMode)
                {
                    ShowError(text, null, exception.Message ?? exception.ToString());
                }
                else
                {
                    ShowError(text);
                }
                StopProcessing = true;
            }
        }

        return default(T);
    }

    #endregion
}
