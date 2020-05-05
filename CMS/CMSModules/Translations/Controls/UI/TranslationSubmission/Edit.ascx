<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Controls_UI_TranslationSubmission_Edit"
     Codebehind="Edit.ascx.cs" %>
<cms:UIForm runat="server" ID="EditForm" ObjectType="cms.translationsubmission" >
    <SecurityCheck Resource="CMS.TranslationServices" Permission="modify" />
</cms:UIForm>
