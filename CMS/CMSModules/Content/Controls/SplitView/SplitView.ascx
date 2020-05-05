<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SplitView.ascx.cs" Inherits="CMSModules_Content_Controls_SplitView_SplitView" %>
<frameset border="0" rows="40%,*,40%" id="mainFrameset" frameborder="0" runat="server">
    <frame name="frame1" id="frame1" src="" scrolling="auto" runat="server" noresize="noresize" />
    <asp:PlaceHolder ID="plcSplit" runat="server" >
        <frame name="toolbarframe" id="toolbarframe" src="" scrolling="no" runat="server" noresize="noresize" />
        <frameset id="verticalFrameset" cols="0%,*,100%"  runat="server" frameborder="0">
            <frame name="frame1Vertical" id="frame1Vertical" src="" scrolling="auto" runat="server" noresize="noresize" />
            <frame name="frameSeparator" id="frameSeparator" src="" scrolling="no" runat="server" noresize="noresize" />
            <frame name="frame2" id="frame2" src="" scrolling="auto" runat="server" noresize="noresize" />
        </frameset>
    </asp:PlaceHolder>
</frameset>
