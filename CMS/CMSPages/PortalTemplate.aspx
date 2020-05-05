<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSPages_PortalTemplate"
    ValidateRequest="false" MaintainScrollPositionOnPostback="true" EnableEventValidation="false"
     Codebehind="PortalTemplate.aspx.cs" %>

<!DOCTYPE html>
<html <%=XmlNamespace%>>
<head id="head" runat="server" enableviewstate="false">
    <title id="_title" runat="server">My site</title>
    <asp:Literal runat="server" ID="tags" EnableViewState="false" />
</head>
<body class="<%=HTMLHelper.EncodeForHtmlAttribute(BodyClass)%>">
    <form id="form" runat="server">
    <asp:PlaceHolder runat="server" ID="plcManagers">
        <asp:ScriptManager ID="manScript" runat="server" ScriptMode="Release"
            EnableViewState="false" />
        <cms:CMSPortalManager ID="manPortal" ShortID="m" runat="server" EnableViewState="false" />
        <cms:CMSDocumentManager ID="docMan" ShortID="dm" runat="server" StopProcessing="true"
            Visible="false" IsLiveSite="false" />
    </asp:PlaceHolder>
    <cms:ContextMenuPlaceHolder ID="plcCtx" runat="server" />
    <cms:CMSPagePlaceholder ID="plcRoot" ShortID="p" runat="server" Root="true" />
    <asp:PlaceHolder runat="server" ID="plcFooter" />
    </form>
</body>
</html>
