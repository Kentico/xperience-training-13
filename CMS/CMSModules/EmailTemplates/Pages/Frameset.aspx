<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Frameset.aspx.cs" Inherits="CMSModules_EmailTemplates_Pages_Frameset" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/TabsFrameset.ascx" TagName="TabsFrameset"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="Head" runat="server" enableviewstate="false">
    <title>Email templates</title>
</head>
<cms:TabsFrameset ID="frm" runat="server" />
</html>