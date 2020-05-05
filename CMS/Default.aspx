<%@ Page Language="C#" AutoEventWireup="true" Inherits="_Default" Theme="Default"
    ValidateRequest="false"  Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <asp:Literal runat="server" ID="ltlTags" EnableViewState="false" />
</head>
<body class="<%=BodyClass%>">
    <form id="form1" runat="server">
    <asp:PlaceHolder runat="server" ID="plcManagers">
        <asp:ScriptManager ID="manScript" runat="server" EnableViewState="false"
            ScriptMode="Release" />
        <cms:CMSPortalManager ID="manPortal" runat="server" EnableViewState="false" />
    </asp:PlaceHolder>
    <cms:ContextMenuPlaceHolder ID="plcCtx" runat="server" />
    <asp:Label runat="server" ID="lblText" EnableViewState="false" />
    </form>
</body>
</html>
