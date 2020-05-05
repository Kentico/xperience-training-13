<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FaviconSelector.ascx.cs" Inherits="CMSAdminControls_UI_Selectors_FaviconSelector" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagName="FileSystemSelector" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlChoice" runat="server" CssClass="radio-list-vertical">
            <%-- Single favicon selection --%>
            <div class="selector-subitem">
                <cms:CMSRadioButton ID="radSingleFavicon" GroupName="FaviconRadGroup" runat="server" AutoPostBack="True" ResourceString="settingkey.faviconpath.single"
                   CheckOnCheckedChanged="component_Changed" />
            </div>
            <asp:Panel ID="pnlSingleFileSelector" runat="server" CssClass="selector-subitem">
                <cms:FileSystemSelector ID="fsSingleFavicon" runat="server" AllowEmptyValue="True" ShowFolders="False" />
            </asp:Panel>
            <%-- Multiple favicons selection --%>
            <div class="selector-subitem">
                <cms:CMSRadioButton ID="radMultipleFavicons" GroupName="FaviconRadGroup" runat="server" AutoPostBack="true" ResourceString="settingkey.faviconpath.multiple"
                    OnCheckedChanged="component_Changed" />
            </div>
            <asp:Panel ID="pnlMultipleFileSelector" runat="server" CssClass="selector-subitem" Visible="False">
                 <cms:FileSystemSelector ID="fsMultipleFavicon" runat="server" AllowEmptyValue="True" ShowFolders="True" />
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
