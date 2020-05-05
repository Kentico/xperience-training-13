<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Integration_Controls_UI_Connectors_Edit"
     Codebehind="Edit.ascx.cs" %>
<cms:UIForm runat="server" ID="form" ObjectType="integration.connector" RedirectUrlAfterCreate="Edit.aspx?connectorid={%EditedObject.ID%}&saved=1"
    CheckFieldEmptiness="true">
    <SecurityCheck Resource="CMS.Integration" Permission="modify" />
</cms:UIForm>
