using System;
using System.Linq;

using CMS;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;

[assembly: RegisterCustomClass("BadgesListControlExtender", typeof(BadgesListControlExtender))]

/// <summary>
/// Bad words list control extender
/// </summary>
public class BadgesListControlExtender : ControlExtender<UniGrid>
{
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;
    }


    private object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "imageurl":
                string url = ValidationHelper.GetString(parameter, "");
                if (!String.IsNullOrEmpty(url))
                {
                    url = "<img alt=\"Badge image\" src=\"" + Control.GetImageUrl(url) + "\" style=\"max-width:50px; max-height: 50px;\"  />";
                    return url;
                }
                return "";
        }

        return null;
    }
}