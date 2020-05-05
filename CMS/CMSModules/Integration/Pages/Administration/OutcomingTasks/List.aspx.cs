using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Integration_Pages_Administration_OutcomingTasks_List : CMSIntegrationPage
{
    #region "Properties"

    /// <summary>
    /// Identifier of selected connector.
    /// </summary>
    private int ConnectorID
    {
        get
        {
            return ValidationHelper.GetInteger(connectorSelector.Value, -1);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        CurrentMaster.DisplaySiteSelectorPanel = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        listElem.ConnectorID = ConnectorID;
    }

    #endregion
}