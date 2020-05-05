<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_LiveControls_MediaLibraries"  Codebehind="MediaLibraries.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/UI/MediaLibraryList.ascx" TagName="LibraryList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaLibrary.ascx"
    TagName="LibraryFiles" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/UI/MediaLibrarySecurity.ascx"
    TagName="LibrarySecurity" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/UI/MediaLibraryEdit.ascx" TagName="LibraryEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<cms:UIContextPanel ID="pnlContext" runat="server">
    <asp:PlaceHolder ID="plcTabsHeader" runat="server" Visible="false">
        <asp:Panel ID="pnlEditActions" runat="server">
            <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" HideBreadcrumbs="false" PropagateToMainNavigation="false" />
            <asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlTabsMain" CssClass="TabsPageHeader">
            <cms:BasicTabControl ID="tabElem" runat="server" UseClientScript="true" UsePostback="true" />
        </asp:Panel>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcHeaderActions" runat="server">
        <asp:Panel ID="pnlLibraryActions" runat="server">
            <cms:HeaderActions ID="newLibrary" runat="server" />
        </asp:Panel>
    </asp:PlaceHolder>
    <asp:Panel runat="server" ID="pnlContent">
        <asp:PlaceHolder ID="plcList" runat="server">
            <cms:LibraryList ID="libraryList" runat="server" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcTabs" runat="server" Visible="false">
            <asp:PlaceHolder ID="tabFiles" runat="server">
                <div class="MediaLibraryContainer">
                    <cms:LibraryFiles ID="libraryFiles" runat="server" IsLiveSite="true" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="tabEdit" runat="server" Visible="false">
                <div class="TabBody">
                    <cms:LibraryEdit ID="libraryEdit" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="tabSecurity" runat="server" Visible="false">
                <div class="TabBody">
                    <cms:LibrarySecurity ID="librarySecurity" runat="server" />
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    </asp:Panel>
</cms:UIContextPanel>
