<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Discount_Discount_Codes_Generator"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
     Codebehind="Discount_Codes_Generator.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Discounts" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSPanel ID="pnlGeneral" runat="server" CssClass="ContentPanel">
        <cms:LocalizedHeading ID="headGeneral" runat="server" Level="4" ResourceString="com.generatecodes.generalInfo" EnableViewState="false" />
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblnumberOfCodes" runat="server" EnableViewState="false" ResourceString="com.couponcode.numberof"
                        DisplayColon="true" AssociatedControlID="txtNumberOfCodes" CssClass="control-label editing-form-label" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtNumberOfCodes" MaxLength="6" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblPrefix" runat="server" EnableViewState="false" ResourceString="com.couponcode.prefix"
                        DisplayColon="true" CssClass="control-label editing-form-label" AssociatedControlID="txtPrefix" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtPrefix" MaxLength="100" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcTimesToUse">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblTimesToUse" runat="server" EnableViewState="false" ResourceString="com.couponcode.uselimit"
                            DisplayColon="true" CssClass="control-label editing-form-label" AssociatedControlID="txtTimesToUse" />
                    </div>
                    <div class="editing-form-value-cell">
                        <div class="control-group-inline">
                        <cms:CMSTextBox runat="server" ID="txtTimesToUse" CssClass="input-width-20" WatermarkText="{$com.couponcode.unlimited$}" Text="1" MaxLength="6" />
                        <span class="form-control-text"><%= GetString("com.couponcode.times") %></span>
                            </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </cms:CMSPanel>
</asp:Content>
