<%@ Page Language="C#" AutoEventWireup="true" Theme="Default"
    Inherits="CMSFormControls_Selectors_InsertImageOrMedia_Tabs_Email" EnableEventValidation="false"  Codebehind="Tabs_Email.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/HTMLEmailProperties.ascx"
    TagName="HTMLEmailProperties" TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Insert image or media - e-mail</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<body class="<%=mBodyClass%>">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="scrManager" runat="server">
    </asp:ScriptManager>
    <cms:HTMLEmailProperties ID="emailProperties" runat="server" IsLiveSite="false" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </form>
</body>
</html>
