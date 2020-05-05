<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Edit.ascx.cs" Inherits="CMSModules_Scoring_Controls_UI_Score_Edit" %>

<cms:UIForm runat="server" ID="EditForm" ObjectType="om.score" OnOnBeforeSave="EditForm_OnBeforeSave" 
    OnOnBeforeValidate="EditForm_OnAfterValidate" RefreshHeader="True">
    <SecurityCheck Resource="CMS.Scoring" Permission="modify" DisableForm="true" />
</cms:UIForm>