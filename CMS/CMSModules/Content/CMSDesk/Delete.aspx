<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Delete"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - Delete"
     Codebehind="Delete.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Documents" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="plcContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlContent">
        <asp:Panel ID="pnlDelete" runat="server">
            <cms:LocalizedHeading runat="server" ID="headQuestion" Level="3" EnableViewState="false" />
            <asp:Panel ID="pnlDocList" runat="server" CssClass="content-block-50 form-control vertical-scrollable-list"
                EnableViewState="false">
                <asp:Label ID="lblDocuments" runat="server" CssClass="ContentLabel" EnableViewState="true" />
            </asp:Panel>
            <asp:PlaceHolder ID="plcCheck" runat="server" EnableViewState="true">
                <cms:CMSPanel ID="pnlDeleteRoot" runat="server" CssClass="form-horizontal" Visible="false">
                    <cms:FormCategoryHeading runat="server" ID="LocalizedHeading1" Level="4" IsAnchor="true" ResourceString="root.deletesettingsheader" EnableViewState="false" />
                    <div class="editing-form-category-fields">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" AssociatedControlID="rblRoot" runat="server" EnableViewState="False"
                                    ID="lblRoot" DisplayColon="true" ResourceString="root.deletesettings" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSRadioButtonList ID="rblRoot" runat="server" AutoPostBack="true" />
                            </div>
                        </div>
                    </div>
                </cms:CMSPanel>
                <asp:Panel ID="pnlDeleteDocument" runat="server" EnableViewState="false" CssClass="form-horizontal">
                    <cms:FormCategoryHeading runat="server" ID="headDeleteDocument" Level="4" IsAnchor="true" EnableViewState="false" />
                    <div class="editing-form-category-fields">
                        <asp:Panel ID="pnlDestroy" runat="server" class="form-group" Visible="False">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" AssociatedControlID="chkDestroy" runat="server" EnableViewState="False"
                                    ID="lblDestroy" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSCheckBox ID="chkDestroy" runat="server" EnableViewState="false" />
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="pnlAllCultures" runat="server" class="form-group" Visible="True">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" AssociatedControlID="chkAllCultures" runat="server" EnableViewState="False"
                                    ID="lblAllCultures" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSCheckBox ID="chkAllCultures" runat="server" EnableViewState="false" />
                            </div>
                        </asp:Panel>
                    </div>
                </asp:Panel>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcDeleteRoot" Visible="false">
                <div class="form-horizontal">
                    <cms:MessagesPlaceHolder runat="server" ID="messagesPlaceholder" />
                    <div class="editing-form-category-fields">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" AssociatedControlID="chkDeleteRoot" runat="server" EnableViewState="False"
                                    ID="LocalizedLabel1" ResourceString="Delete.RootConfirm" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSCheckBox ID="chkDeleteRoot" runat="server" AutoPostBack="true" EnableViewState="true" />
                            </div>
                        </div>
                    </div>                    
                </div>
            </asp:PlaceHolder>
        </asp:Panel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
