<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Controls_UI_ABVariant_Edit"
     Codebehind="Edit.ascx.cs" %>
<cms:UIForm runat="server" ObjectType="om.abvariant" ID="form" OnOnBeforeSave="form_OnBeforeSave"
    RedirectUrlAfterCreate="Edit.aspx?abTestID={?abTestID?}&variantId={%EditedObject.ID%}&nodeID={?nodeID?}&saved=1" >
    <SecurityCheck Permission="manage" Resource="CMS.ABTest" DisableForm="true" />
</cms:UIForm>
