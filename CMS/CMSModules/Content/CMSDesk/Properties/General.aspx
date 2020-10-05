<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_General"
    Theme="Default" CodeBehind="General.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server">
        <cms:UIPlaceHolder ID="pnlUIOther" runat="server" ModuleName="CMS.Content" ElementName="General.OtherProperties">
            <asp:Panel ID="pnlOther" runat="server">
                <cms:LocalizedHeading runat="server" ID="headOtherProperties" Level="4" EnableViewState="false" />
                <div class="form-horizontal properties-form">
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
                            <cms:LocalizedLabel CssClass="control-label" ID="lblPublishedTitle" ResourceString="PageProperties.Published" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblPublished" runat="server" EnableViewState="false" CssClass="form-control-text" />
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
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblCultureTitle" ResourceString="GeneralProperties.Culture" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label ID="lblCulture" runat="server" EnableViewState="false" CssClass="form-control-text" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUISearch" runat="server" ModuleName="CMS.Content" ElementName="Menu.Search">
            <asp:Panel ID="pnlSearch" runat="server" CssClass="NodePermissions" EnableViewState="false">
                <cms:LocalizedHeading runat="server" ID="headSearch" Level="4" ResourceString="GeneralProperties.Search" EnableViewState="false" />
                <div class="form-horizontal properties-form">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblExcludeFromSearch" runat="server" EnableViewState="false"
                                ResourceString="GeneralProperties.ExcludeFromSearch" DisplayColon="true" AssociatedControlID="chkExcludeFromSearch" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkExcludeFromSearch" runat="server" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <br />
        </cms:UIPlaceHolder>
        <cms:UIPlaceHolder ID="pnlUIOwner" runat="server" ModuleName="CMS.Content" ElementName="General.Owner">
            <asp:Panel ID="pnlOwner" runat="server" CssClass="NodePermissions">
                <cms:LocalizedHeading runat="server" ID="headOwner" Level="4" ResourceString="GeneralProperties.OwnerGroup" EnableViewState="false" />
                <div class="form-horizontal properties-form">
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
        <cms:CMSUpdatePanel runat="server" >
            <ContentTemplate>
                <asp:Panel runat="server" ID="pnlSwitch" CssClass="form-group">
                    <cms:CMSIcon runat="server" ID="icAdvanced" CssClass="icon-caret-down cms-icon-30" />
                    <cms:LocalizedLinkButton runat="server" ID="lnkAdvanced" OnClick="advancedLink_Click" />
                </asp:Panel>
                <asp:PlaceHolder ID="plcAdvanced" runat="server" Visible="false">
                    <cms:UIPlaceHolder ID="pnlUIDesignation" runat="server" ModuleName="CMS.Content" ElementName="General.Designation">
                        <asp:Panel ID="pnlDesignation" runat="server">
                            <cms:LocalizedHeading runat="server" ID="headDesignation" Level="4" ResourceString="generalproperties.designation" EnableViewState="false" />
                            <div class="form-horizontal properties-form">
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
                            </div>
                        </asp:Panel>
                    </cms:UIPlaceHolder>
                    <cms:UIPlaceHolder ID="pnlUIAlias" runat="server" ModuleName="CMS.Content" ElementName="URLs.Path">
                        <asp:Panel ID="pnlAlias" runat="server">
                            <cms:LocalizedHeading runat="server" ID="headAlias" Level="4" ResourceString="GeneralProperties.DocumentAlias" EnableViewState="false" />
                            <div class="form-horizontal properties-form">
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
                </asp:PlaceHolder>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
    <div class="Clear">
    </div>
</asp:Content>
