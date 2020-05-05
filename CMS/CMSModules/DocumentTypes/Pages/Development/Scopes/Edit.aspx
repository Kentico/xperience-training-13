<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_DocumentTypes_Pages_Development_Scopes_Edit"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page type scopes list"
    Theme="Default"  Codebehind="Edit.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm ID="form" runat="server" ObjectType="CMS.DocumentTypeScope" RedirectUrlAfterCreate="Edit.aspx?siteid={?siteid?}&scopeid={%EditedObject.ID%}&saved=1" OnOnBeforeDataLoad="form_OnBeforeDataLoad"  DefaultFieldLayout="TwoColumns" />
</asp:Content>
