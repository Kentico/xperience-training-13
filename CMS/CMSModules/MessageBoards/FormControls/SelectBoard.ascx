<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MessageBoards_FormControls_SelectBoard"  Codebehind="SelectBoard.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%BoardDisplayName%}"
            ObjectType="board.board" ResourcePrefix="boardselector" SelectionMode="SingleDropDownList" AllowAll="false" AllowEmpty="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
