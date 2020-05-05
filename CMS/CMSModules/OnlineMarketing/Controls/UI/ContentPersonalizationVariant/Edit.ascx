<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Controls_UI_ContentPersonalizationVariant_Edit"  Codebehind="Edit.ascx.cs" %>
    
<cms:UIForm runat="server" ID="EditForm" ObjectType="om.personalizationvariant" RedirectUrlAfterCreate="Edit.aspx?variantid={%EditedObject.ID%}&saved=1" >
    <SecurityCheck Resource="CMS.ContentPersonalization" Permission="Manage" />
</cms:UIForm>
