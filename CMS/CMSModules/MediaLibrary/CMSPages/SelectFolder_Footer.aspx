<%@ Page Language="C#" Theme="Default" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_CMSPages_SelectFolder_Footer" EnableEventValidation="false"  Codebehind="SelectFolder_Footer.aspx.cs" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/FolderActions/SelectFolderFooter.ascx"
    TagName="FolderFooter" TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Copy / Move</title>
    <base target="_self" />
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
    <asp:ScriptManager ID="scriptManager" runat="server">
    </asp:ScriptManager>
    <div class="MediaLibrary">
        <cms:FolderFooter ID="folderFooter" runat="server"></cms:FolderFooter>
    </div>
    </form>
</body>
</html>
