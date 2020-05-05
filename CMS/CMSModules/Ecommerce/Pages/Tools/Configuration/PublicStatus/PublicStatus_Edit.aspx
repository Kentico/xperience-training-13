<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_PublicStatus_PublicStatus_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="PublicStatus_Edit.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <cms:UIForm runat="server" ID="EditForm" ObjectType="ecommerce.publicstatus" OnOnBeforeDataLoad="EditForm_OnBeforeDataLoad"
        RedirectUrlAfterCreate="PublicStatus_Edit.aspx?publicStatusId={%EditedObject.ID%}&siteId={?SiteID?}&saved=1" />
</asp:Content>
