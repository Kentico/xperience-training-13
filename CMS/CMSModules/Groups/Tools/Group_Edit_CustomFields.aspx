<%@ Page Language="C#" AutoEventWireup="true" 
    Inherits="CMSModules_Groups_Tools_Group_Edit_CustomFields" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Group edit - Custom fields"  Codebehind="Group_Edit_CustomFields.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DataForm ID="formCustomFields" runat="server" IsLiveSite="false" />
</asp:Content>
