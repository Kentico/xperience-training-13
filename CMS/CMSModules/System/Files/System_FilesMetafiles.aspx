<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Files_System_FilesMetafiles"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Administration - System - Files - Metafiles"
     Codebehind="System_FilesMetafiles.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="MetaFiles" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="plcBefore" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <asp:Panel runat="server" ID="pnlBefore">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true"
                        ResourceString="General.Site" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowGlobal="true" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlContent">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblObjectType" EnableViewState="false" DisplayColon="true"
                        ResourceString="General.ObjectType" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSUpdatePanel ID="pnlUpdateObjectType" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <cms:CMSDropDownList runat="server" ID="drpObjectType" AutoPostBack="true" CssClass="DropDownField" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </div>
        </div>
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:UniGrid runat="server" ID="gridFiles" GridName="Metafiles.xml" OrderBy="MetaFileName, MetaFileID" ShowObjectMenu="false"
                    ObjectType="cms.metafile" Columns="MetaFileID, MetaFileObjectType, MetaFileObjectID, MetaFileName, MetaFileExtension, MetaFileSize, MetaFileGUID, MetaFileSiteID, MetaFileLastModified, MetaFileImageWidth, MetaFileImageHeight, CASE WHEN MetaFileBinary IS NULL THEN 0 ELSE 1 END AS HasBinary" />
                <asp:Panel ID="pnlFooter" runat="server" CssClass="form-horizontal mass-action">
                    <div class="form-group">
                        <div class="mass-action-value-cell">
                            <cms:CMSDropDownList ID="drpWhat" runat="server" UseResourceStrings="True">
                                <asp:ListItem Text="media.file.list.lblactions" Value="selected" />
                                <asp:ListItem Text="media.file.list.filesall" Value="all" />
                            </cms:CMSDropDownList>
                            <cms:CMSDropDownList ID="drpAction" runat="server" />
                            <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary"
                                EnableViewState="false" OnClick="btnOK_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>