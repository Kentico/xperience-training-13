<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_CustomTables_Tools_CustomTable_Data_ViewItem" Theme="Default"
    ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    EnableEventValidation="false"  Codebehind="CustomTable_Data_ViewItem.aspx.cs" %>

<%@ Register Src="~/CMSModules/CustomTables/Controls/CustomTableViewItem.ascx" TagName="CustomTableViewItem"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CustomTableViewItem ID="customTableViewItem" runat="server" />
</asp:Content>
