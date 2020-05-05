using System;
using System.Collections.Generic;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;


public partial class CMSAdminControls_ModalCalendar_RangeModalCalendar : CMSCustomCalendarControl
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        RangeDateTimePicker datePickerObject = PickerControl as RangeDateTimePicker;
        if ((datePickerObject == null) || !Visible)
        {
            return;
        }

        // Register default scripts
        RegisterDefaultScripts(datePickerObject);

        // Register initial range picker scripts
        RegisterPickerInitScripts(datePickerObject);

        // Register custom scripts for control components
        RegisterCustomScripts(datePickerObject);
    }


    private void RegisterPickerInitScripts(RangeDateTimePicker datePickerObject)
    {
        // Init picker settings
        var settings = new HashSet<string>();
        settings.Add(String.Format("showTimePanel:{0}", (datePickerObject.EditTime && !datePickerObject.DisableDaySelect && !datePickerObject.DisableMonthSelect).ToString().ToLowerCSafe()));
        settings.Add(String.Format("stepMonths:{0}", datePickerObject.DisableMonthSelect ? 12 : 1));
        settings.Add(String.Format("disableDaySelect:{0}", datePickerObject.DisableDaySelect.ToString().ToLowerCSafe()));
        settings.Add(String.Format("disableMonthSelect:{0}", datePickerObject.DisableMonthSelect.ToString().ToLowerCSafe()));
        settings.Add(String.Format("changeMonth:{0}", (!datePickerObject.DisableMonthSelect).ToString().ToLowerCSafe()));
        settings.Add("showButtonPanel:false");

        // Get all parameters for picker initialization
        string pickerParams = GetScriptParameters(datePickerObject, settings);

        // Init both calendars
        string calendarInit = String.Format(@"$cmsj(function() {{$cmsj('#{0}').cmsdatepicker({{ {1},defaultTimeValue:{2} }});
$cmsj('#{3}').cmsdatepicker({{ {1},defaultTimeValue:{4} }})}});", dateFrom.ClientID, pickerParams, (datePickerObject.UseDynamicDefaultTime ? 1 : 0), dateTo.ClientID, (datePickerObject.UseDynamicDefaultTime ? 2 : 0));

        ScriptHelper.RegisterClientScriptBlock(Page, GetType(), ClientID + "_RegisterDatePickerFunction", ScriptHelper.GetScript(calendarInit));
    }


    private void RegisterCustomScripts(RangeDateTimePicker datePickerObject)
    {
        // Get client IDs
        string rangeCalendarClientId = rangeCalendar.ClientID;
        string textBoxClientId = datePickerObject.DateTimeTextBox.ClientID;
        string altTextBoxClientId = datePickerObject.AlternateDateTimeTextBox.ClientID;
        string dateFromClientId = dateFrom.ClientID;
        string dateToClientId = dateTo.ClientID;
        string calendarImgClientId = datePickerObject.CalendarImageClientID;

        // Button OK
        btnOK.OnClientClick = String.Format(@"$cmsj('#{0}').val ($cmsj('#{1}').cmsdatepicker ('getFormattedDate'));
$cmsj('#{2}').val ($cmsj('#{3}').cmsdatepicker ('getFormattedDate'));
$cmsj('#{4}').hide(); {5}", textBoxClientId, dateFromClientId, altTextBoxClientId, dateToClientId, rangeCalendarClientId, !datePickerObject.PostbackOnOK ? "return false;" : "");

        if (datePickerObject.DisplayNAButton)
        {
            btnNA.OnClientClick = String.Format("$cmsj('#{0}').hide(); $cmsj('#{1}').val(''); $cmsj('#{2}').val(''); return false;", rangeCalendarClientId, textBoxClientId, altTextBoxClientId);
            btnNA.Visible = true;
        }

        // Icon
        string clickScript = String.Format(@"if( $cmsj('#{0}').is(':hidden')) {{
$cmsj('#{1}').cmsdatepicker('setDate',$cmsj('#{3}').val());
$cmsj('#{2}').cmsdatepicker('setDate',$cmsj('#{4}').val());
var offset = localizeRangeCalendar($cmsj('#{0}'),$cmsj('#{3}'),{5},true);
$cmsj('#{0}').css({{left:offset.left+'px',top:offset.top+'px'}});$cmsj('#{0}').show();}} return false;", rangeCalendarClientId, dateFromClientId, dateToClientId, textBoxClientId, altTextBoxClientId, CultureHelper.IsUICultureRTL().ToString().ToLowerCSafe());

        datePickerObject.DateTimeTextBox.Attributes["OnClick"] = clickScript;
        datePickerObject.AlternateDateTimeTextBox.Attributes["OnClick"] = clickScript;

        // Add image on click
        String script = String.Format("$cmsj(\"#{0}\").click(function() {{ {1} }});", calendarImgClientId, clickScript);
        ScriptHelper.RegisterStartupScript(this, typeof(string), ClientID + "CalendarImageInitScript", ScriptHelper.GetScript(script));

        // Script for hiding calendar when clicked somewhere else
        ltlScript.Text = ScriptHelper.GetScript(String.Format(@"$cmsj(document).mousedown(function(event) {{
var target = $cmsj(event.target);
if ((target.closest('#{0}').length == 0) && (target.parents('#{0}').length == 0)
    && (target.closest('#{1}').length == 0) && (target.closest('#{2}').length == 0) && (target.closest('#{3}').length == 0))
        $cmsj('#{0}').hide();
}});", rangeCalendarClientId, calendarImgClientId, textBoxClientId, altTextBoxClientId));
    }
}
