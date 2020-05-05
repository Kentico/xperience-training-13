<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_CustomTables_Tools_CustomTable_Data_List" EnableEventValidation="false"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Custom table - Data List"
    Theme="Default"  Codebehind="CustomTable_Data_List.aspx.cs" %>
    
<%@ Register Src="~/CMSModules/CustomTables/Controls/CustomTableDataList.ascx" TagName="CustomTableDataList" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcContent" runat="server">
        <cms:CustomTableDataList id="customTableDataList" runat="server" IsLiveSite="false" />
    </asp:PlaceHolder>
</asp:Content>
