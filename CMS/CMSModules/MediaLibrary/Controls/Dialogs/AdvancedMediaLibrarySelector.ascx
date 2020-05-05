<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_Dialogs_AdvancedMediaLibrarySelector"  Codebehind="AdvancedMediaLibrarySelector.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/FormControls/MediaLibrarySelector.ascx"
    TagName="LibrarySelector" TagPrefix="cms" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" DisplayColon="true" EnableViewState="false"
                ResourceString="general.site"></cms:LocalizedLabel>
        </div>
        <div class="editing-form-value-cell">
            <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowAll="false" AllowEmpty="false"
                StopProcessing="true" UseCodeNameForSelection="false" OnlyRunningSites="true" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcGroupSelector" runat="server" Visible="true">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblGroup" runat="server" DisplayColon="true" EnableViewState="false"
                    ResourceString="general.group"></cms:LocalizedLabel>
            </div>
            <div class="editing-form-value-cell">
                <asp:PlaceHolder ID="pnlGroupSelector" runat="server"></asp:PlaceHolder>
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblLibrary" runat="server" DisplayColon="true" EnableViewState="false"
                ResourceString="dialogs.media.library"></cms:LocalizedLabel>
        </div>
        <div class="editing-form-value-cell">
            <cms:LibrarySelector ID="librarySelector" runat="server" NoneWhenEmpty="true" AddCurrentLibraryRecord="false"
                UseAutoPostBack="true" OnSelectedLibraryChanged="librarySelector_SelectedLibraryChanged"
                UseLibraryNameForSelection="false" />
        </div>
    </div>
</div>