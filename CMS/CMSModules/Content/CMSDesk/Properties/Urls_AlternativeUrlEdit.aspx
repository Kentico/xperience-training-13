<%@ Page Language="C#" AutoEventWireup="false" Codebehind="Urls_AlternativeUrlEdit.aspx.cs" Inherits="CMSModules_Content_CMSDesk_Properties_Urls_AlternativeUrlEdit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" MaintainScrollPositionOnPostback="true" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server">
        <div class="header-actions-container">
            <asp:Panel ID="pnlMenu" runat="server" CssClass="header-actions-main">
                <cms:HeaderActions ID="menu" runat="server" />
            </asp:Panel>
        </div>
        <div class="Clear">
        </div>
    </cms:CMSPanel>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <div class="form-horizontal url-form">
        <div class="form-group">
            <div class="editing-form-label-cell cms-pageurlpath-slug-label">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAltUrl" runat="server" EnableViewState="false"
                    ResourceString="alternativeurl.newurl" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell-wide">
                <cms:TextBoxWithPlaceholder ID="txtAltUrl" runat="server" MaxLength="400" />
            </div>
        </div>
    </div>
</asp:Content>
