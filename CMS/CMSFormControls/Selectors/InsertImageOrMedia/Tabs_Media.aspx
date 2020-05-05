<%@ Page Language="C#" Theme="Default" AutoEventWireup="true" Inherits="CMSFormControls_Selectors_InsertImageOrMedia_Tabs_Media"
    EnableEventValidation="false"  Codebehind="Tabs_Media.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/LinkMediaSelector.ascx"
    TagName="LinkMedia" TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Insert image or media - content</title>
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
    <asp:PlaceHolder runat="server" ID="plcManagers"></asp:PlaceHolder>
    <cms:cmsupdatepanel id="pnlUpdateSelectMedia" runat="server" updatemode="Conditional">
        <ContentTemplate>
            <cms:LinkMedia ID="linkMedia" ShortID="m" runat="server" IsLiveSite="false" />
        </ContentTemplate>
    </cms:cmsupdatepanel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </form>
</body>
</html>
