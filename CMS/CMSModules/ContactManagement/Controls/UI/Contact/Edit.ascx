<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_Edit"
     Codebehind="Edit.ascx.cs" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<cms:UIForm runat="server" ID="EditForm" ObjectType="OM.Contact" OnOnAfterSave="EditForm_OnAfterSave" OnOnCreate="EditForm_OnCreate"
    OnOnAfterDataLoad="EditForm_OnAfterDataLoad" IsLiveSite="false">
</cms:UIForm>
<cms:AnchorDropup runat="server" ID="anchorDropup" MinimalAnchors="2" IsOpened="False" />