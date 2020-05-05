<%@ Control Language="C#" AutoEventWireup="false" Inherits="CMSModules_OnlineMarketing_Controls_UI_AbTest_Edit"
     Codebehind="Edit.ascx.cs" %>
<cms:UIForm runat="server" ObjectType="om.abtest" ID="form" OnOnBeforeSave="form_OnBeforeSave"
    OnOnAfterSave="form_OnAfterSave" OnOnBeforeDataLoad="form_OnBeforeDataLoad"
    OnOnBeforeValidate="form_OnBeforeValidate" OnOnCreate="form_OnCreate" RefreshHeader="True">
    <SecurityCheck Permission="manage" Resource="CMS.ABTest" DisableForm="true" />
</cms:UIForm>
