using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Notifications.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Notifications_Controls_TemplateText : CMSUserControl
{
    #region "Variables"

    private InfoDataSet<NotificationGatewayInfo> mDsGateways;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Returns number of gateways being edited.
    /// </summary>
    public int GatewayCount
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets or sets ID of the template to edit.
    /// </summary>
    public int TemplateID
    {
        get;
        set;
    }

    #endregion


    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        mDsGateways = NotificationGatewayInfoProvider.GetNotificationGateways().TopN(11).Columns("GatewayID,GatewayDisplayName").TypedResult;

        GatewayCount = mDsGateways.Items.Count;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // For up to 10 Gateways edit all on one page, hide UniGrid
        if (GatewayCount <= 10)
        {
            gridGateways.StopProcessing = true;
            pnlGrid.Visible = false;
            string heading;
            string gatewayLbl = GetString("notification.template.gateway");
            bool isRTL = CultureHelper.IsCultureRTL(MembershipContext.AuthenticatedUser.PreferredUICultureCode);

            // Generate controls
            foreach (NotificationGatewayInfo info in mDsGateways)
            {
                if (isRTL)
                {
                    heading = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(info.GatewayDisplayName)) + gatewayLbl;
                }
                else
                {
                    heading = String.Format("{0}: {1}", gatewayLbl, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(info.GatewayDisplayName)));
                }
                var headingControl = new LocalizedHeading()
                {
                    Level = 4,
                    Text = heading
                };

                Panel pnlGrouping = new Panel();
                pnlGrouping.Controls.Add(headingControl);

                TemplateTextEdit ctrl = Page.LoadUserControl("~/CMSModules/Notifications/Controls/TemplateTextEdit.ascx") as TemplateTextEdit;
                if (ctrl != null)
                {
                    ctrl.ID = "templateEdit" + info.GatewayID;
                    ctrl.TemplateID = TemplateID;
                    ctrl.GatewayID = info.GatewayID;

                    // Add gateway edit control to the container panel
                    pnlGrouping.Controls.Add(ctrl);
                    plcTexts.Controls.Add(pnlGrouping);
                }
            }
        }
        else
        {
            // Hook event handlers
            gridGateways.OnExternalDataBound += gridGateways_OnExternalDataBound;
            gridGateways.OnAction += gridGateways_OnAction;
        }
    }

    #endregion


    #region "Unigrid events"

    protected object gridGateways_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "gatewayenabled")
        {
            return UniGridFunctions.ColoredSpanYesNo(parameter);
        }
        return parameter;
    }


    protected void gridGateways_OnAction(string actionName, object actionArgument)
    {
        int gatewayId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                // Redirect to edit UI
                URLHelper.Redirect(UrlResolver.ResolveUrl("Template_Edit_Text_Text.aspx?gatewayid=" + gatewayId + "&templateid=" + TemplateID));
                break;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Saves template texts.
    /// </summary>
    public void Save()
    {
        if (GatewayCount > 10)
        {
            return;
        }

        foreach (Control ctrl in plcTexts.Controls)
        {
            Panel pnl = ctrl as Panel;
            if (pnl != null)
            {
                TemplateTextEdit tte = ControlsHelper.GetControlOfTypeRecursive<TemplateTextEdit>(pnl);
                if (tte != null)
                {
                    tte.SaveData();
                }
            }
        }
    }

    #endregion
}