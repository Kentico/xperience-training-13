<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="RangeModalCalendar.ascx.cs"
    Inherits="CMSAdminControls_ModalCalendar_RangeModalCalendar" %>
<div id="rangeCalendar" runat="server" class="datetime-ui-range-header datetime-ui-range-div">
    <table>
        <tr>
            <td>
                <div id="dateFrom" runat="server" />
            </td>
            <td>
                <div id="dateTo" runat="server" />
            </td>
        </tr>
    </table>
    <div class="form-group form-group-buttons rangemodal-buttons">
        <cms:LocalizedButton runat="server" ID="btnNA" ResourceString="general.na" ButtonStyle="Default" Visible="False" EnableViewState="false" />
        <cms:LocalizedButton runat="server" ID="btnOK" ResourceString="general.select" ButtonStyle="Primary" EnableViewState="false" />
    </div>
</div>
<asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
