<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Images_IconSelector"  Codebehind="IconSelector.ascx.cs" %>
<cms:CMSUpdatePanel ID="pnlUpdateContent" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="iconSelector">
            <div class="Hidden">
                <cms:CMSUpdatePanel ID="pnlUpdateHidden" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="hdnAction" runat="server" />
                        <asp:HiddenField ID="hdnArgument" runat="server" />
                        <asp:Button ID="hdnButton" runat="server" OnClick="hdnButton_Click" CssClass="HiddenButton"
                            EnableViewState="false" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
            <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" />
            <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="radio-list-vertical">
                        <cms:CMSRadioButton ID="radPredefinedIcon" runat="server" GroupName="iconType" />
                        <cms:CMSRadioButton ID="radCustomIcon" runat="server" GroupName="iconType" />
                        <cms:CMSRadioButton ID="radDoNotDisplay" runat="server" GroupName="iconType" />
                    </div>
                    <cms:CMSPanel runat="server" ID="pnlPredefined" EnableViewState="False" CssClass="selector">
                        <asp:Panel ID="pnlMain" runat="server" CssClass="iconGroup clearfix">
                            <strong>
                                <cms:LocalizedLabel runat="server" ID="lblColor" CssClass="clearfix" EnableViewState="False" />
                            </strong>
                        </asp:Panel>
                        <cms:CMSUpdatePanel ID="pnlUpdateIcons" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <strong>
                                    <cms:LocalizedLabel runat="server" ID="lblSize" CssClass="clearfix" EnableViewState="False" />
                                </strong>
                                <asp:Panel ID="pnlChild" runat="server" CssClass="iconGroup" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </cms:CMSPanel>
                    <cms:CMSPanel runat="server" ID="pnlCustom" CssClass="selector control-group-inline">
                        <cms:MediaSelector ID="mediaSelector" runat="server" />
                    </cms:CMSPanel>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
