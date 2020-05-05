using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_CMSPages_Unsubscribe : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;
        title.TitleText = HTMLHelper.HTMLEncode(string.Format(GetString("board.unsubscription"), (unsubscription.SubscriptionSubject != null) ? ScriptHelper.GetString(unsubscription.SubscriptionSubject.BoardDisplayName) : null)); ;
    }
}