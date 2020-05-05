<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Controls_UI_Account_Edit"
     Codebehind="Edit.ascx.cs" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<cms:UIForm runat="server" ID="EditForm" ObjectType="OM.Account"
    OnOnAfterDataLoad="EditForm_OnAfterDataLoad" OnOnAfterSave="EditForm_OnAfterSave" IsLiveSite="false">
</cms:UIForm>
<cms:AnchorDropup runat="server" ID="anchorDropup" MinimalAnchors="2" IsOpened="False" />
