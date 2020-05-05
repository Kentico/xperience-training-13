<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Frameset.aspx.cs" Inherits="CMSModules_MyDesk_CheckedOut_Frameset" %>

<%@ Register TagPrefix="cms" TagName="TabsFrameset" Src="~/CMSAdminControls/UI/PageElements/TabsFrameset.ascx" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>My desk - Checked out</title>
</head>
<cms:TabsFrameset ID="frameset" ShortID="fs" runat="server" ContentUrl="~/CMSPages/blank.aspx" />
</html>
