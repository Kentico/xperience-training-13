<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Controls_UI_TranslationService_Edit"  Codebehind="Edit.ascx.cs" %>
    
<cms:UIForm runat="server" ID="EditForm" ObjectType="cms.translationservice" RedirectUrlAfterCreate="Edit.aspx?serviceid={%EditedObject.ID%}&saved=1" >
    <SecurityCheck Resource="CMS.TranslationServices" Permission="modify" />
</cms:UIForm>
