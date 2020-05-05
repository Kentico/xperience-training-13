using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Settings")]
public partial class CMSModules_Settings_Pages_Default : CMSDeskPage
{
    /// <summary>
    /// OnPreLoad event. 
    /// </summary>
    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);
        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        keysFrame.Attributes.Add("src", UIContextHelper.GetElementUrl(ModuleName.CMS, "Settings.Keys", false));
    }
}