using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSModules_Chat_Controls_UI_ChatUser_ChatUserIsOnlineFilter : CMSAbstractBaseFilterControl
{
    #region "Nested enum"

    private enum IsOnlineValuesEnum
    {
        All,
        Yes,
        No
    }

    #endregion


    #region "Constructor"

    public CMSModules_Chat_Controls_UI_ChatUser_ChatUserIsOnlineFilter()
    {
        Init += new EventHandler(CMSModules_Chat_Controls_UI_ChatUser_ChatUserIsOnlineFilter_Init);
    }

    #endregion


    #region "Page events"

    private void CMSModules_Chat_Controls_UI_ChatUser_ChatUserIsOnlineFilter_Init(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            drpIsOnline.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), IsOnlineValuesEnum.All.ToString()));
            drpIsOnline.Items.Add(new ListItem(ResHelper.GetString("general.yes"), IsOnlineValuesEnum.Yes.ToString()));
            drpIsOnline.Items.Add(new ListItem(ResHelper.GetString("general.no"), IsOnlineValuesEnum.No.ToString()));
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion


    #region "Properties"

    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Private methods"

    private string GetCondition()
    {
        IsOnlineValuesEnum val = (IsOnlineValuesEnum)Enum.Parse(typeof(IsOnlineValuesEnum), drpIsOnline.SelectedValue);
        
        switch (val)
        {
            case IsOnlineValuesEnum.Yes:
            case IsOnlineValuesEnum.No:
                string format = "{0}EXISTS (SELECT * FROM Chat_OnlineUser COU WHERE COU.ChatOnlineUserSiteID = {1} AND COU.ChatOnlineUserChatUserID = ChatUserID AND COU.ChatOnlineUserJoinTime IS NOT NULL)";

                return String.Format(format, val == IsOnlineValuesEnum.No ? "NOT " : "", SiteContext.CurrentSiteID);
            case IsOnlineValuesEnum.All:
            default:
                return "";
        }
    }

    #endregion
}
