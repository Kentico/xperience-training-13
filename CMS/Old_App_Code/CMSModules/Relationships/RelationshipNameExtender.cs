using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

[assembly: RegisterCustomClass("RelationshipNameListExtender", typeof(RelationshipNameListExtender))]

/// <summary>
/// Relationship name list control extender.
/// </summary>
public class RelationshipNameListExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// Extender initialization.
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += Control_OnExternalDataBound;
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    private object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string result = string.Empty;
        switch (sourceName.ToLowerCSafe())
        {
            case "type":
                string value = ValidationHelper.GetString(parameter, string.Empty);
                if ((value == string.Empty) || (value.Contains(ObjectHelper.GROUP_DOCUMENTS)))
                {
                    result = ResHelper.GetString("general.pages");
                }
                else if (value.Contains(ObjectHelper.GROUP_OBJECTS))
                {
                    result = ResHelper.GetString("RelationshipNames.Objects");
                }

                break;
        }
        return result;
    }
}