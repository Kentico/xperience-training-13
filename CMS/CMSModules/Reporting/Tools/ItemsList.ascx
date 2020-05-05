<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_ItemsList"
     Codebehind="~/CMSModules/Reporting/Tools/ItemsList.ascx.cs" %>
<%@ Register Src="~/CMSModules/Reporting/FormControls/ReportItemSelector.ascx" TagName="BasicReportSelector"
    TagPrefix="cms" %>
<table class="reporting-items-list">
    <tr>
        <td>
            <cms:BasicReportSelector runat="server" ID="brsItems" UseIDValue="true" />
        </td>
        <td>
            <cms:LocalizedButton runat="server" ID="btnInsert" EnableViewState="false" ButtonStyle="Primary"
                ResourceString="ItemList.Insert" />
        </td>
        <td>
            <cms:CMSMoreOptionsButton runat="server" ID="btnAdd" EnableViewState="False" />
        </td>
    </tr>
</table>
<asp:Button runat="server" ID="btnHdnDelete" CssClass="HiddenButton" OnClick="btnHdnDelete_Click"
    EnableViewState="false" />
<asp:HiddenField runat="server" ID="hdnItemId" />
