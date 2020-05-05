<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_Design"
    ValidateRequest="false" MaintainScrollPositionOnPostback="true" EnableEventValidation="false"
     Codebehind="PageTemplate_Design.aspx.cs" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title id="Title1" runat="server">My site</title>
    <asp:Literal runat="server" ID="ltlTags" EnableViewState="false" />
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
            font-family: Arial;
            font-size: small;
        }
    </style>
</head>
<body class="<%=BodyClass%>">
    <form id="form1" runat="server">
    <asp:PlaceHolder runat="server" ID="plcManagers">
        <asp:ScriptManager ID="manScript" runat="server" ScriptMode="Release"
            EnableViewState="false" />
        <cms:CMSPortalManager ID="manPortal" ShortID="m" runat="server" EnableViewState="false" />
        <cms:CMSDocumentManager ID="docMan" ShortID="dm" runat="server" StopProcessing="true"
            Visible="false" IsLiveSite="false" />
    </asp:PlaceHolder>
    <cms:CMSPagePlaceholder ID="plc" runat="server" Root="true" />
    </form>
</body>
</html>
