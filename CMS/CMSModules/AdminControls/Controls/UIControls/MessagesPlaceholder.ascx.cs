using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_MessagesPlaceholder : CMSAbstractUIWebpart
{
    protected override void OnInit(EventArgs e)
    {
        // Set control's placeholder and actions for use of non-children controls
        ICMSPage page = Page as ICMSPage;
        if (page != null)
        {
            page.MessagesPlaceHolder = plcMess;
        }

        ManageTexts();

        base.OnInit(e);
    }
}
