<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_InternalStatus_InternalStatus_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="InternalStatus_Edit.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <cms:UIForm runat="server" ID="EditForm" ObjectType="ecommerce.internalstatus" OnOnBeforeDataLoad="EditForm_OnBeforeDataLoad"
        RedirectUrlAfterCreate="InternalStatus_Edit.aspx?statusId={%EditedObject.ID%}&siteId={?SiteID?}&saved=1" />
</asp:Content>
