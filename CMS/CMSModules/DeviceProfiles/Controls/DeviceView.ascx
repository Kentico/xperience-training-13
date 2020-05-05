<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DeviceView.ascx.cs" Inherits="CMSModules_DeviceProfiles_Controls_DeviceView" %>
<cms:CMSPanel ID="pnlDevice" runat="server" ShortID="pd" CssClass="DeviceFrame">
    <cms:CMSPanel ID="pnlTop" runat="server" ShortID="pt" CssClass="TopLine">
        <div class="LeftPiece">
        </div>
        <div class="CenterPiece">
        </div>
        <div class="RightPiece">
        </div>
    </cms:CMSPanel>
    <cms:CMSPanel ID="pnlCenterLine" runat="server" ShortID="pcl" CssClass="CenterLine">
        <cms:CMSPanel ID="pnlLeft" runat="server" ShortID="pl" CssClass="LeftPiece">
        </cms:CMSPanel>
        <cms:CMSPanel ID="pnlCenter" runat="server" ShortID="pc" CssClass="CenterPiece">
            <div class="CenterPanel">
                <iframe width="100%" height="100%" id="<%=FrameID%>" name="<%=FrameID%>" scrolling="<%=FrameScroll%>"
                    frameborder="0" src="<%=ViewPageSource%>" class="ContentFrame scroll-area"></iframe>
            </div>
        </cms:CMSPanel>
        <cms:CMSPanel ID="pnlRight" runat="server" ShortID="pr" CssClass="RightPiece">
        </cms:CMSPanel>
    </cms:CMSPanel>
    <cms:CMSPanel ID="pnlBottom" runat="server" ShortID="pb" CssClass="BottomLine">
        <div class="LeftPiece">
        </div>
        <div class="CenterPiece">
        </div>
        <div class="RightPiece">
        </div>
    </cms:CMSPanel>
</cms:CMSPanel>
