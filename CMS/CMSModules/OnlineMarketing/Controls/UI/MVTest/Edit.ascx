<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Controls_UI_MVTest_Edit"
     Codebehind="Edit.ascx.cs" %>
<cms:UIForm runat="server" ObjectType="om.mvtest" AlternativeFormName="update" ID="form" RefreshHeader="True" OnOnBeforeSave="form_OnBeforeSave"
    OnOnBeforeRedirect="form_OnAfterSave" OnOnBeforeDataLoad="form_OnBeforeDataLoad"
    OnOnBeforeValidate="form_OnBeforeValidate">
    <SecurityCheck Permission="manage" Resource="CMS.MVTest" DisableForm="true" />
</cms:UIForm>
