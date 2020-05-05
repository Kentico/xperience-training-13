<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_CustomTables_Tools_CustomTable_Data_EditItem" Theme="Default" ValidateRequest="false"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Custom table data - Edit item"
    EnableEventValidation="false"  Codebehind="CustomTable_Data_EditItem.aspx.cs" %>

<%@ Register Src="~/CMSModules/CustomTables/Controls/CustomTableForm.ascx" TagName="CustomTableForm"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcContent" runat="server">
        <cms:CustomTableForm ID="customTableForm" runat="server" />
    </asp:PlaceHolder>
</asp:Content>
