<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_CustomTables_Controls_CustomTableDataList"
     Codebehind="CustomTableDataList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:UniGrid runat="server" ID="gridData" GridName="~/CMSModules/CustomTables/Controls/CustomTableDataList.xml"
    IsLiveSite="false" />
<asp:Literal ID="ltlScript" runat="server" />