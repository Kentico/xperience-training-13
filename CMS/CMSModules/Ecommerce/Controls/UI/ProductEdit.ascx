<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ProductEdit.ascx.cs" Inherits="CMSModules_Ecommerce_Controls_UI_ProductEdit" %>
<%@ Register TagPrefix="cms" TagName="EditMenuUC" Src="~/CMSModules/Content/Controls/EditMenu.ascx" %>
<%@ Register TagPrefix="cms" TagName="HeaderActionsUC" Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>
<%@ Register TagPrefix="cms" TagName="SelectSKUBinding" Src="~/CMSModules/Ecommerce/FormControls/SelectSKUBinding.ascx" %>
<%-- Header --%>
<div id="CMSHeaderDiv">
    <asp:PlaceHolder ID="plcHeaderActions" runat="server">
        <%-- Edit menu --%>
        <cms:EditMenuUC ID="editMenuElem" runat="server" ShortID="em" Visible="false" IsLiveSite="false" StopProcessing="true" />
        <%-- Header actions --%>
        <asp:Panel ID="pnlHeaderActions" runat="server" Visible="false" CssClass="control-group-inline header-actions-container">
            <cms:HeaderActionsUC ID="headerActionsElem" runat="server" StopProcessing="true" IsLiveSite="false" />
        </asp:Panel>
    </asp:PlaceHolder>
    <%-- CK toolbar --%>
    <div id="CKToolbar">
    </div>
</div>
<%-- Anchor links --%>
<cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="2" />
<%-- Forms --%>
<cms:CMSPanel ID="pnlForms" runat="server" CssClass="ProductEditForms" ShortID="pf">
    <cms:CMSPanel ID="pnlFormsInner" runat="server" ShortID="pfi">
        <%-- Messages --%>
        <cms:CMSPanel ID="pnlMessages" runat="server">
            <cms:MessagesPlaceHolder ID="plcMessages" runat="server" WrapperControlID="pnlMessages" IsLiveSite="false" />
        </cms:CMSPanel>
        <%-- SKU binding - create --%>
        <cms:CMSPanel ID="pnlCreateSkuBinding" runat="server" ShortID="psb" Visible="false">
            <cms:FormCategoryHeading runat="server" ID="headBinding" Level="4" EnableViewState="false" ResourceString="com.productedit.skubinding" IsAnchor="True" />
            <cms:SelectSKUBinding ID="selectSkuBindingElem" runat="server" ShortID="sb" />
        </cms:CMSPanel>
        <span class="ClearBoth"></span>
        <%-- General SKU properties --%>
        <cms:BasicForm ID="skuGeneralForm" runat="server" ShortID="g" HtmlAreaToolbarLocation="Out:CKToolbar"
            SetDefaultValuesToDisabledFields="false" />
        <span class="ClearBoth"></span>
        <%-- Membership SKU properties --%>
        <cms:BasicForm ID="skuMembershipForm" runat="server" ShortID="m" HtmlAreaToolbarLocation="Out:CKToolbar"
            SetDefaultValuesToDisabledFields="false" Visible="false" />
        <%-- E-product SKU properties --%>
        <cms:BasicForm ID="skuEproductForm" runat="server" ShortID="e" HtmlAreaToolbarLocation="Out:CKToolbar"
            SetDefaultValuesToDisabledFields="false" Visible="false" />
        <%-- Bundle SKU properties --%>
        <cms:BasicForm ID="skuBundleForm" runat="server" ShortID="b" HtmlAreaToolbarLocation="Out:CKToolbar"
            SetDefaultValuesToDisabledFields="false" Visible="false" />
        <span class="ClearBoth"></span>
        <%-- Custom properties --%>
        <cms:CMSPanel ID="pnlCustomProperties" runat="server" ShortID="cp" CssClass="FormPanel EditingFormFieldSet"
            Visible="false">
            <cms:FormCategoryHeading runat="server" ID="headCustomProperties" Level="4" ResourceString="com.productedit.customproperties" IsAnchor="True" />
            <%-- Custom SKU properties --%>
            <cms:BasicForm ID="skuCustomForm" runat="server" ShortID="cf" HtmlAreaToolbarLocation="Out:CKToolbar"
                SetDefaultValuesToDisabledFields="false" Visible="false" FieldGroupHeadingLevel="5" FieldGroupHeadingIsAnchor="false" />
            <span class="ClearBoth"></span>
            <%-- Custom page properties --%>
            <cms:CMSForm ID="documentForm" runat="server" ShortID="df" HtmlAreaToolbarLocation="Out:CKToolbar"
                SetDefaultValuesToDisabledFields="false" Visible="false" MarkRequiredFields="true" FieldGroupHeadingLevel="5" FieldGroupHeadingIsAnchor="false" />
        </cms:CMSPanel>
        <span class="ClearBoth"></span>
        <%-- Other SKU properties --%>
        <cms:CMSUpdatePanel runat="server" ID="upSkuOtherForm">
            <ContentTemplate>
                <cms:BasicForm ID="skuOtherForm" runat="server" ShortID="of" HtmlAreaToolbarLocation="Out:CKToolbar"
                    SetDefaultValuesToDisabledFields="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <span class="ClearBoth"></span>
        <%-- SKU binding - remove --%>
        <cms:CMSPanel ID="pnlRemoveSkuBinding" runat="server" ShortID="rsb" CssClass="FormPanel EditingFormFieldSet"
            Visible="false">
            <cms:FormCategoryHeading runat="server" ID="headRemoveBinding" Level="4" ResourceString="com.skubinding" IsAnchor="True" />
            <cms:LocalizedButton ID="btnRemoveSkuBinding" ButtonStyle="Default" runat="server" EnableViewState="false" ResourceString="com.skubinding.unbind" />
        </cms:CMSPanel>
    </cms:CMSPanel>
</cms:CMSPanel>