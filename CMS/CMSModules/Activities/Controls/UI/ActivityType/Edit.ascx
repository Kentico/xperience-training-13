<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Activities_Controls_UI_ActivityType_Edit"
     Codebehind="Edit.ascx.cs" %>
<cms:UIForm runat="server" ID="EditForm" ObjectType="om.activitytype" RedirectUrlAfterCreate="Tab_General.aspx?typeid={%EditedObject.ID%}&saved=1"
    IsLiveSite="false">
    <SecurityCheck Resource="CMS.Activities" Permission="ManageActivities" DisableForm="true" />
</cms:UIForm>
