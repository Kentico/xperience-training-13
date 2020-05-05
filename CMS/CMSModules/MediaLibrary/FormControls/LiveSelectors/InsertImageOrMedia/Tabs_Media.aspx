<%@ Page Language="C#" Theme="Default" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_FormControls_LiveSelectors_InsertImageOrMedia_Tabs_Media"
     Codebehind="Tabs_Media.aspx.cs" EnableEventValidation="false" %>

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
    <div class="LiveSiteDialog">
        <cms:CMSUpdatePanel ID="uplContent" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:LinkMedia ID="linkMedia" runat="server" IsLiveSite="true" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
    </form>

    <script language="javascript" type="text/javascript">
        //<![CDATA[
        InitResizers();
        //]]>
    </script>

</body>
</html>
