<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SelectValidity.ascx.cs"
    Inherits="CMSAdminControls_UI_Selectors_SelectValidity" %>

<asp:Panel ID="pnlSelectValidity" runat="server">
    <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />
    <%-- Valid for --%>
    <div class="radio-list-vertical">
        <cms:CMSRadioButton ID="radDays" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.days"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" />
        <cms:CMSRadioButton ID="radWeeks" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.weeks"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" />
        <cms:CMSRadioButton ID="radMonths" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.months"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" />
        <cms:CMSRadioButton ID="radYears" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.years"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" />
        <div class="selector-subitem">
            <cms:CMSTextBox ID="txtValidFor" runat="server" />
        </div>
        <%-- Valid until --%>
        <cms:CMSRadioButton ID="radUntil" runat="server" GroupName="Validity" ResourceString="general.selectvalidity.until"
            OnCheckedChanged="ValidityRadioGroup_CheckedChanged" Checked="true" />
        <div class="selector-subitem">
            <cms:DateTimePicker ID="untilDateElem" runat="server" EditTime="false" AllowEmptyValue="true"
                DisplayNA="true" />
        </div>
    </div>
</asp:Panel>