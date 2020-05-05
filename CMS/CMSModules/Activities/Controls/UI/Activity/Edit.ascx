<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Activities_Controls_UI_Activity_Edit"
     Codebehind="Edit.ascx.cs" %>
<cms:UIForm runat="server" ObjectType="om.activity" IsLiveSite="false" ID="EditForm" AlternativeFormName="CustomActivityForm"
    RedirectUrlAfterCreate="~/CMSModules/Activities/Pages/Tools/Activities/Activity/List.aspx?saved=1&siteid={?siteid?}"
    OnOnBeforeSave="EditForm_OnBeforeSave" OnOnAfterValidate="EditForm_OnAfterValidate"> 
    <SecurityCheck Resource="CMS.Activities" Permission="ManageActivities"/>
</cms:UIForm>