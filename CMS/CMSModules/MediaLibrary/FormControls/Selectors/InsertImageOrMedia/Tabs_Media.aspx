<%@ Page Language="C#" Theme="Default" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_FormControls_Selectors_InsertImageOrMedia_Tabs_Media"
    EnableEventValidation="false"  Codebehind="Tabs_Media.aspx.cs" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/Dialogs/LinkMediaSelector.ascx"
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
        .ImageExtraClass
        {
            position: absolute;
        }
        .ImageTooltip
        {
            border: 1px solid #ccc;
            background-color: #fff;
            padding: 3px;
            display: block;
        }
    </style>
</head>
<body class="<%=mBodyClass%>">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="scriptManager" runat="server">
    </asp:ScriptManager>
    <cms:CMSUpdatePanel ID="pnlUpdateSelectMedia" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <cms:LinkMedia ID="linkMedia" runat="server" IsLiveSite="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    </form>

    <script language="javascript" type="text/javascript">
        //<![CDATA[
            InitResizers();
        //]]>
    </script>

</body>
</html>
