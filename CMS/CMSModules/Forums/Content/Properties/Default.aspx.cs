using System;

using CMS.Base.Web.UI;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Content_Properties_Default : CMSForumsPage
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        tree.Attributes["src"] = "tree.aspx" + RequestContext.CurrentQueryString;

        RegisterModalPageScripts();
    }
}