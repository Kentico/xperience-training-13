<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Edit.ascx.cs"
    Inherits="CMSModules_EmailTemplates_Controls_Edit" %>

<cms:UIForm runat="server" ID="editForm" ObjectType="CMS.EmailTemplate">
    <SecurityCheck Resource="CMS.EmailTemplates" Permission="Modify" />
</cms:UIForm>