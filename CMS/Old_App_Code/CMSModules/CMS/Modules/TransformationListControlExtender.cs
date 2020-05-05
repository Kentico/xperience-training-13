using System;
using System.Data;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[assembly: RegisterCustomClass("TransformationListControlExtender", typeof(TransformationListControlExtender))]

/// <summary>
/// Permission list control extender
/// </summary>
public class TransformationListControlExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;

        Control.Load += (sender, args) =>
        {
            if (!SettingsKeyInfoProvider.VirtualObjectsAllowed)
            {
                Control.ShowWarning(Control.GetString("VirtualPathProvider.NotRunning"));
            }
        };
    }


    /// <summary>
    /// OnExternalDataBound event handler
    /// </summary>
    private object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "transformationtype":
                DataRowView dr = (DataRowView)parameter;
                bool isHierarchical = ValidationHelper.GetBoolean(dr["TransformationIsHierarchical"], false);
                string type = ValidationHelper.GetString(dr["TransformationType"], String.Empty);
                if (isHierarchical)
                {
                    return Control.GetString("transformation.hierarchical");
                }
                switch (type.ToLowerCSafe())
                {
                    case "ascx":
                        return "ASCX";

                    case "text":
                        return "Text / XML";

                    case "xslt":
                        return "XSLT";

                    case "html":
                        return "HTML";

                    case "jquery":
                        return "jQuery";
                }
                break;
        }
        return parameter;
    }
}