using System;
using System.Collections.Generic;

using CMS.Base;
using CMS.Base.Web.UI;


public partial class CMSAdminControls_ModalCalendar_ModalCalendar : CMSCustomCalendarControl
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        DateTimePicker datePickerObject = PickerControl as DateTimePicker;
        if ((datePickerObject == null) || !Visible)
        {
            return;
        }

        // Register default scripts
        RegisterDefaultScripts(datePickerObject);

        // Register initial date picker scripts
        RegisterPickerInitScripts(datePickerObject);

        // Register custom scripts for control components
        RegisterCustomScripts(datePickerObject);
    }


    private void RegisterPickerInitScripts(DateTimePicker datePickerObject)
    {
        // If true display time
        bool displayTime = datePickerObject.EditTime;

        // Init picker settings
        var settings = new HashSet<string>();
        settings.Add(String.Format("currentText:{0}", ScriptHelper.GetLocalizedString(displayTime ? "calendar.now" : "Calendar.Today")));
        settings.Add(String.Format("NAText:{0}", ScriptHelper.GetLocalizedString("general.notavailable")));
        settings.Add(String.Format("actionPanelButtons:['ok',{0},{1}]", (datePickerObject.AllowEmptyValue ? "'na'" : ""), (datePickerObject.DisplayNow ? "'now'" : "")));
        settings.Add(String.Format("showTimePanel:{0}", displayTime.ToString().ToLowerCSafe()));
        settings.Add("showButtonPanel:true");
        settings.Add("changeMonth:true");

        string calendarInit = String.Format("$cmsj(function() {{$cmsj('#{0}').cmsdatepicker({{ {1} }})}});", datePickerObject.DateTimeTextBox.ClientID, GetScriptParameters(datePickerObject, settings));

        ScriptHelper.RegisterClientScriptBlock(Page, GetType(), ClientID + "_RegisterDatePickerFunction", ScriptHelper.GetScript(calendarInit));
    }


    private void RegisterCustomScripts(DateTimePicker datePickerObject)
    {
        string textBoxClientId = datePickerObject.DateTimeTextBox.ClientID;

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DTPickerModalSelectDate" + textBoxClientId, ScriptHelper.GetScript(
            String.Format("function SelectModalDate_{0}(param, pickerId) {{ {1} }} \n", textBoxClientId, Page.ClientScript.GetCallbackEventReference(datePickerObject, "param", "SetDateModal", "pickerId"))));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DTPickerModalSetDate", ScriptHelper.GetScript("function SetDateModal(result, context) { $cmsj('#' + context).cmsdatepicker('setDateNoTextBox',result); } \n"));
    }
}
