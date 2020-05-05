<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="TimeoutSelector.ascx.cs"
    Inherits="CMSAdminControls_UI_Selectors_TimeoutSelector" %>

<%@ Register Src="~/CMSFormControls/Basic/TimeControl.ascx" TagName="TimeControl" TagPrefix="cms" %>

<div class="form-group">
    <span class="radio-list-horizontal">
        <cms:CMSRadioButton ID="rbNoneMode" runat="server" GroupName="mode" ResourceString="general.none"
            AutoPostBack="true" OnCheckedChanged="rbMode_CheckedChanged" />
        <cms:CMSRadioButton ID="rbIntervalMode" runat="server" AutoPostBack="true"
            GroupName="mode" ResourceString="timeoutselector.specificinterval" OnCheckedChanged="rbMode_CheckedChanged" />
        <cms:CMSRadioButton ID="rbDateMode" runat="server" AutoPostBack="true" GroupName="mode"
            ResourceString="timeoutselector.specificdate" OnCheckedChanged="rbMode_CheckedChanged" />
    </span>
</div>
<asp:PlaceHolder ID="plcInterval" runat="server">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:FormLabel ID="lblAfter" runat="server" ResourceString="timeoutselector.after" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtQuantity" />
        </div>
        <div class="editing-form-value-cell control-group-inline">
            <cms:CMSTextBox ID="txtQuantity" runat="server" MaxLength="4" CssClass="input-width-40" />
            <cms:CMSDropDownList ID="drpScale" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpScale_SelectedIndexChanged" CssClass="input-width-40" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcDateMode" runat="server">
        <asp:PlaceHolder ID="plcExactly" runat="server">
            <div class="form-group">
                <div class="editing-form-value-cell-offset editing-form-value-cell">
                    <cms:CMSRadioButton ID="rbExactly" runat="server" GroupName="date" ResourceString="timeoutselector.exactly"
                        AutoPostBack="true" OnCheckedChanged="rbNext_CheckedChanged" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:Panel ID="plcNextDate" runat="server" CssClass="form-group">
            <div class="editing-form-value-cell-offset editing-form-value-cell control-group-inline">
                <cms:CMSRadioButton ID="rbNextDate" runat="server" GroupName="date" AutoPostBack="true"
                    OnCheckedChanged="rbNext_CheckedChanged" />
                <asp:PlaceHolder ID="pnlNextDateMonth" runat="server">
                    <cms:CMSDropDownList ID="drpNextDateMonth" runat="server" OnSelectedIndexChanged="drpNextDate_DaysCountChanged"
                        AutoPostBack="true" CssClass="input-width-40" />
                </asp:PlaceHolder>
                <cms:CMSDropDownList ID="drpNextDate" runat="server" CssClass="input-width-20" />
                <cms:LocalizedLabel ID="lblNextDateOfTheMonth" runat="server" ResourceString="timeoutselector.ofthemonth" />
            </div>
        </asp:Panel>
        <asp:Panel ID="plcNextDay" runat="server" CssClass="form-group">
            <div class="editing-form-value-cell-offset editing-form-value-cell control-group-inline">
                <cms:CMSRadioButton ID="rbNextDay" GroupName="date" runat="server" ResourceString="timeoutselector.thenext"
                    AutoPostBack="true" OnCheckedChanged="rbNext_CheckedChanged" />
                <cms:CMSDropDownList ID="drpNextOrder" runat="server" CssClass="input-width-40" />
                <cms:CMSDropDownList ID="drpNextDay" runat="server" CssClass="input-width-40" />
                <cms:LocalizedLabel ID="lblNextDayOfTheMonth" runat="server" ResourceString="timeoutselector.ofthemonth" />
            </div>
        </asp:Panel>
    </asp:PlaceHolder>
</asp:PlaceHolder>
<asp:Panel ID="plcDate" runat="server" Visible="false" CssClass="form-group">
    <div class="editing-form-label-cell">
        <cms:LocalizedLabel ID="lblDate" runat="server" ResourceString="general.date" DisplayColon="true" CssClass="control-label" />
    </div>
    <div class="editing-form-value-cell">
        <cms:DateTimePicker ID="dateTimePicker" runat="server" EditTime="false" />
        <asp:PlaceHolder ID="plcSpecificTime" runat="server">
            <div class="control-group-inline">
                <cms:CMSCheckBox ID="cbSpecificTime" runat="server" ResourceString="timeoutselector.inspecifictime"
                    AutoPostBack="true" OnCheckedChanged="cbSpecificTime_CheckedChanged" />
            </div>
            <cms:TimeControl ID="timePicker" runat="server" />
        </asp:PlaceHolder>
    </div>
</asp:Panel>