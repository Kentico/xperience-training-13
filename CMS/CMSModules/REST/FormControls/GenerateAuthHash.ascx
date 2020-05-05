<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_REST_FormControls_GenerateAuthHash"
     Codebehind="GenerateAuthHash.ascx.cs" %>

<cms:LocalizedButton runat="server" ID="lnkGenerate" ButtonStyle="Default" ResourceString="rest.generatehash" />
<cms:LocalizedButton runat="server" ID="lnkInvalidate" ButtonStyle="Default" ResourceString="rest.invalidatehash" />
<cms:LocalizedLabel runat="server" ID="lblInvalidate" CssClass="explanation-text-settings" Visible="false" />
