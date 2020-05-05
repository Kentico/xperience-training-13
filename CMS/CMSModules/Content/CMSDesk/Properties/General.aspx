<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_General"
    Theme="Default" CodeBehind="General.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/ContentRating/Controls/Stars.ascx" TagName="Rating"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/SelectCssStylesheet.ascx" TagName="SelectCssStylesheet"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server">
        <cms:UIPlaceHolder ID="pnlUIDesign" runat="server" ModuleName="CMS.Content" ElementName="General.Design">
            <asp:Panel ID="pnlDesign" runat="server">
                <cms:LocalizedHeading runat="server" ID="headDesign" Level="4" ResourceString="GeneralProperties.DesignGroup" EnableViewState="false" />
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblCssStyle" ResourceString="PageProperties.CssStyle" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox runat="server" ID="chkCssStyle" AutoPostBack="true" OnCheckedChanged="chkCssStyle_CheckedChanged" ResourceString="Metadata.Inherit" CssClass="field-value-override-checkbox" />
                            <cms:SelectCssStylesheet IsLiveSite="false" ID="ctrlSiteSelectStyleSheet" runat="server" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIOther" runat="server" ModuleName="CMS.Content" ElementName="General.OtherProperties">
            <asp:Panel ID="pnlOther" runat="server">
                <cms:LocalizedHeading runat="server" ID="headOtherProperties" Level="4" EnableViewState="false" />
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblNameTitle" ResourceString="GeneralProperties.Name" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblName" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblTypeTitle" ResourceString="GeneralProperties.Type" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblType" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblCreatedByTitle" ResourceString="GeneralProperties.CreatedBy" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblCreatedBy" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblCreatedTitle" ResourceString="GeneralProperties.Created" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblCreated" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblLastModifiedByTitle" ResourceString="GeneralProperties.LastModifiedBy" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblLastModifiedBy" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblLastModifiedTitle" ResourceString="GeneralProperties.LastModified" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblLastModified" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <asp:PlaceHolder ID="plcRating" runat="server">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblContentRating" runat="server" ResourceString="GeneralProperties.ContentRating"
                                DisplayColon="true" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell control-group-inline">
                            <cms:Rating ID="ratingControl" runat="server" />
                            <asp:Label ID="lblContentRatingResult" runat="server" EnableViewState="false" CssClass="form-control-text" />
                            <cms:LocalizedButton ID="btnResetRating" runat="server" ButtonStyle="Default" OnClick="btnResetRating_Click" EnableViewState="false" ResourceString="general.reset" />
                        </div>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblNodeIDTitle" ResourceString="GeneralProperties.NodeID" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblNodeID" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblDocIDTitle" ResourceString="GeneralProperties.DocumentID" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblDocID" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblGUIDTitle" ResourceString="GeneralProperties.GUID" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblGUID" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblDocGUIDTitle" ResourceString="GeneralProperties.DocumentGUID" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblDocGUID" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblAliasPathTitle" ResourceString="GeneralProperties.AliasPath" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblAliasPath" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblCultureTitle" ResourceString="GeneralProperties.Culture" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblCulture" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                    <asp:PlaceHolder runat="server" ID="plcLive" Visible="false">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblLiveURLTitle" runat="server" ResourceString="GeneralProperties.LiveURL" EnableViewState="false" />
                            </div>
                            <div class="editing-form-value-cell">
                                <a id="lnkLiveURL" runat="server" target="_blank" class="form-control-text" enableviewstate="false"></a>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcPreview" runat="server" Visible="false">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblPreviewURLTitle" runat="server" ResourceString="GeneralProperties.PreviewURL" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSUpdatePanel ID="pnlUpdatePreviewUrl" runat="server" UpdateMode="conditional">
                                <ContentTemplate>
                                    <a id="lnkPreviewURL" runat="server" target="_blank" class="form-control-text"><%= ResHelper.GetString("GeneralProperties.ShowPreview") %></a>
                                    <cms:CMSAccessibleButton runat="server" ID="btnResetPreviewGuid" EnableViewState="false" IconOnly="True" IconCssClass="icon-rotate-right" />
                                </ContentTemplate>
                            </cms:CMSUpdatePanel>
                        </div>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblPublishedTitle" ResourceString="PageProperties.Published" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblPublished" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIOwner" runat="server" ModuleName="CMS.Content" ElementName="General.Owner">
            <asp:Panel ID="pnlOwner" runat="server" CssClass="NodePermissions">
                <cms:LocalizedHeading runat="server" ID="headOwner" Level="4" ResourceString="GeneralProperties.OwnerGroup" EnableViewState="false" />
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblOwnerTitle" runat="server" ResourceString="GeneralProperties.Owner" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblOwner" runat="server" EnableViewState="false" CssClass="form-control-text" />
                            <asp:PlaceHolder runat="server" ID="plcUsrOwner"></asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIAlias" runat="server" ModuleName="CMS.Content" ElementName="URLs.Path">
            <asp:Panel ID="pnlAlias" runat="server">
                <cms:LocalizedHeading runat="server" ID="headAlias" Level="4" ResourceString="GeneralProperties.DocumentAlias" EnableViewState="false" />
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblAlias" runat="server" EnableViewState="false"
                                ResourceString="GeneralProperties.Alias" ShowRequiredMark="true" AssociatedControlID="txtAlias" DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtAlias" runat="server" />
                            <cms:CMSRequiredFieldValidator ID="valAlias" runat="server" ControlToValidate="txtAlias"
                                Display="Dynamic" EnableViewState="false" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </cms:UIPlaceHolder>
    </asp:Panel>
    <div class="Clear">
    </div>
</asp:Content>
