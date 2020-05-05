<%@ Page Language="C#" Theme="Default" AutoEventWireup="true"
    Inherits="CMSFormControls_Selectors_SelectFileOrFolder_Content" EnableEventValidation="false"   Codebehind="Content.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/FileSystemSelector.ascx"
    TagName="FileSystem" TagPrefix="uc1" %>
    
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Insert filesystem path</title>
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
    <cms:CMSUpdatePanel ID="pnlUpdateSelectMedia" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <uc1:FileSystem ID="fileSystem" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </form>
</body>
</html>
