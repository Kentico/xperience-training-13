<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Pages_TranslateDocuments"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="TranslateDocuments.aspx.cs"
    Title="Content - Translate" %>

<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx" TagName="SelectPath"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Translations/Controls/TranslationServiceSelector.ascx"
    TagName="TranslationServiceSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Documents" />
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel runat="server" ID="pnlContent">
        <cms:MessagesPlaceHolder runat="server" ID="plcMessages" />
        <asp:PlaceHolder runat="server" ID="plcInfo" EnableViewState="false">
            <cms:LocalizedHeading Level="4" runat="server" ID="lblTranslateFollowing" EnableViewState="false" ResourceString="content.translatefollowing" />
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlList" EnableViewState="false">
            <div class="form-horizontal">
                <asp:Panel ID="pnlDocList" runat="server" CssClass="form-group" EnableViewState="false">
                    <div class="editing-form-label-cell sr-only">
                        <cms:LocalizedLabel runat="server" ResourceString="content.translatefollowing" />
                    </div>
                    <div class="editing-form-value-cell textarea-full-width">
                        <div class="form-control vertical-scrollable-list">
                            <asp:Label ID="lblDocuments" runat="server" EnableViewState="true" />
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlDocSelector" runat="server" CssClass="form-group" Visible="false">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblSelectDocInfo" CssClass="control-label" runat="server" AssociatedControlID="pathElem"
                            EnableViewState="true" ResourceString="translationservice.selectdocuments" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SelectPath runat="server" ID="pathElem" IsLiveSite="false" SubItemsNotByDefault="true" SinglePathMode="False" />
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlSkipTranslated" runat="server" CssClass="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" DisplayColon="True"
                            AssociatedControlID="chkSkipTranslated" ResourceString="translations.skipalreadytranslated" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkSkipTranslated" runat="server" Checked="true" EnableViewState="false" />
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSettings">
            <cms:LocalizedHeading runat="server" ID="headTranslate" Level="4" EnableViewState="false" ResourceString="content.selecttranslationservice" />
            <asp:Panel runat="server" ID="pnlServiceSelector">
                <cms:TranslationServiceSelector runat="server" ID="translationElem" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlButtons" CssClass="control-group-inline">
            <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnTranslate_Click"
                ResourceString="general.translate" EnableViewState="false" />
            <cms:LocalizedButton ID="btnNo" runat="server" ButtonStyle="Default" ResourceString="general.cancel"
                EnableViewState="false" />
        </asp:Panel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
