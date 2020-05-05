<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_SelectFolder"  Codebehind="SelectFolder.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaLibrary.ascx"
    TagName="MediaLibrary" TagPrefix="cms" %>

<script language="javascript" type="text/javascript">
    function RaiseAction() {
        SetAction('copymove', '');
        RaiseHiddenPostBack();
    }
</script>

<asp:Panel ID="pnlBody" runat="server">
    <div class="MediaLibrary">
        <div class="DialogsPageHeader">
            <div class="DialogHeader">
                <cms:PageTitle ID="titleElem" runat="server" EnableViewState="false" IsDialog="true" />
            </div>
        </div>
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        <asp:Panel ID="pnlMediaLibrary" runat="server">
            <cms:MediaLibrary ID="mediaLibrary" runat="server" IsLiveSite="false" DisplayFilesCount="false"
                DisplayMode="Simple" />
        </asp:Panel>
    </div>
</asp:Panel>
