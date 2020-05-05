<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Frameset.aspx.cs" Inherits="CMSModules_MyDesk_WaitingForApproval_Frameset" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/TabsFrameset.ascx" TagName="TabsFrameset" TagPrefix="cms" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Pending objects</title>
</head>
<cms:TabsFrameset ID="frm" runat="server" ContentUrl="~/CMSPages/blank.aspx" />
</html>
