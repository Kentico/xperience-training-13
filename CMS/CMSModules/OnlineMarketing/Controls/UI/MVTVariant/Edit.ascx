<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Controls_UI_MVTVariant_Edit"  Codebehind="Edit.ascx.cs" %>

<cms:UIForm runat="server" ID="EditForm" ObjectType="om.mvtvariant" RedirectUrlAfterCreate="Edit.aspx?variantid={%EditedObject.ID%}&saved=1">
    <SecurityCheck Resource="CMS.MVTest" Permission="Manage" />
</cms:UIForm>