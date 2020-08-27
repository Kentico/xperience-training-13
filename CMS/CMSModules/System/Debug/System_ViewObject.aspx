<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_ViewObject"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    MaintainScrollPositionOnPostback="true"  Codebehind="System_ViewObject.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/System/ViewObject.ascx" TagName="ViewObject"
    TagPrefix="cms" %>
<%@ Register Src="CacheItemsGrid.ascx" TagName="CacheItemsGrid" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <asp:Panel ID="pnlCacheItem" runat="server" Visible="false">
            <cms:LocalizedHeading ID="hProp" runat="server" ResourceString="general.properties" CssClass="editing-form-category-caption anchor" Level="4" />
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel runat="server" ID="ltlKeyTitle" ResourceString="Administration-System.CacheInfoKey"
                        EnableViewState="false" DisplayColon="true" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label runat="server" ID="ltlKey" EnableViewState="false" CssClass="form-control-text" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel runat="server" ID="ltlExpirationTitle" ResourceString="Administration-System.CacheInfoExpiration"
                        EnableViewState="false" DisplayColon="true" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label runat="server" ID="ltlExpiration" EnableViewState="false" CssClass="form-control-text" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel runat="server" ID="ltlPriorityTitle" ResourceString="Administration-System.CacheInfoPriority"
                        EnableViewState="false" DisplayColon="true" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label runat="server" ID="ltlPriority" EnableViewState="false" CssClass="form-control-text" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcDependencies">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel runat="server" ID="ltlDependenciesTitle" ResourceString="Administration-System.CacheInfoDependencies"
                            EnableViewState="false" DisplayColon="true" CssClass="control-label" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:LocalizedLabel runat="server" ID="ltlDependencies" EnableViewState="false"
                            ResourceString="Administration-System.CacheInfoNoDependencies" CssClass="form-control-text" />
                        <cms:CacheItemsGrid ID="gridDependencies" ShortID="gd" runat="server" ShowDummyItems="true" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlBody" Visible="false">
            <cms:LocalizedHeading runat="server" ID="hData" ResourceString="Administration-System.CacheInfoData" CssClass="editing-form-category-caption anchor" Level="4" />
            <cms:ViewObject ID="objElem" ShortID="o" runat="server" />
        </asp:Panel>
    </div>
</asp:Content>