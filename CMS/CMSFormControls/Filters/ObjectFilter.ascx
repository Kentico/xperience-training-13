<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ObjectFilter.ascx.cs"
    Inherits="CMSFormControls_Filters_ObjectFilter" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlObjectFilter">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <div class="form-horizontal form-filter">
                <asp:PlaceHolder ID="plcSite" runat="server">
                    <div class="form-group">
                        <div class="filter-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" ResourceString="general.site" EnableViewState="false"
                                DisplayColon="true" />
                        </div>
                        <div class="filter-form-value-cell">
                            <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowAll="false" AllowEmpty="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcParentObject" runat="server">
                    <div class="form-group">
                        <div class="filter-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblParent" runat="server" EnableViewState="false" DisplayColon="true" />
                        </div>
                        <div class="filter-form-value-cell">
                            <cms:UniSelector ID="parentSelector" runat="server" AllowEmpty="false" AllowAll="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
