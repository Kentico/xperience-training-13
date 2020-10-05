<%@ Control Language="C#" AutoEventWireup="false" Inherits="CMSModules_ImportExport_Controls_ImportSiteDetails"  Codebehind="ImportSiteDetails.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="CultureSelector" TagPrefix="cms" %>

<div class="form-horizontal">
    <asp:PlaceHolder runat="server" ID="plcNewSelection">
        <cms:CMSRadioButton runat="server" ID="radNewSite" Checked="true" GroupName="Site" AutoPostBack="true" />
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcNewSite">
        <div class="selector-subitem">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSiteDisplayName" runat="server"
                        EnableViewState="false" AssociatedControlID="txtSiteDisplayName" ShowRequiredMark="True"/>
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtSiteDisplayName" runat="server" MaxLength="200" />
                    <cms:CMSRequiredFieldValidator ID="rfvSiteDisplayName" runat="server" ControlToValidate="txtSiteDisplayName:cntrlContainer:textbox" Display="Dynamic" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblSiteName" runat="server" EnableViewState="false" AssociatedControlID="txtSiteName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CodeName ID="txtSiteName" runat="server" MaxLength="100" />
                    <cms:LocalizedLabel CssClass="form-control-error label-full-width" id="lblErrorSiteName" runat="server" Visible="False" EnableViewState="False" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSitePresentationUrl" runat="server" EnableViewState="false" AssociatedControlID="txtSitePresentationUrl" ShowRequiredMark="True"/>
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtSitePresentationUrl" runat="server" MaxLength="400" />
                    <cms:CMSRequiredFieldValidator ID="rfvSitePresentationUrl" runat="server" ControlToValidate="txtSitePresentationUrl" Display="Dynamic" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblDomain" runat="server" EnableViewState="false" AssociatedControlID="txtDomain" ShowRequiredMark="True"/>
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtDomain" runat="server" MaxLength="400" />
                    <cms:CMSRequiredFieldValidator ID="rfvDomain" runat="server" ControlToValidate="txtDomain" Display="Dynamic" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcCulture" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblCulture" runat="server" EnableViewState="false" AssociatedControlID="cultureElem" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CultureSelector runat="server" ID="cultureElem" DisplayAllCultures="true" IsLiveSite="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcExisting">
        <asp:PlaceHolder runat="server" ID="plcExistingSelection">
            <cms:CMSRadioButton runat="server" ID="radExisting" GroupName="Site" AutoPostBack="true" />
        </asp:PlaceHolder>
        <div class="selector-subitem">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblSite" runat="server" EnableViewState="false" AssociatedControlID="siteSelector" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </div>
        <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
    </asp:PlaceHolder>
</div>