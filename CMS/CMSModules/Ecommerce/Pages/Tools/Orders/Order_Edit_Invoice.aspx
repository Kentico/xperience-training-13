<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Invoice"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="Order_Edit_Invoice.aspx.cs" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblInvoiceNumber" CssClass="control-label" AssociatedControlID="txtInvoiceNumber"
                    ResourceString="order_invoice.lblInvoiceNumber" runat="server" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtInvoiceNumber" runat="server" MaxLength="200" CssClass="input-width-40" EnableViewState="false" />
                    <cms:CMSButton ID="btnGenerate" runat="server" OnClick="btnGenerate_Click" ButtonStyle="Default" EnableViewState="false" />
                    <cms:CMSButton ID="btnPrintPreview" runat="server" OnClientClick="showPrintPreview(); return false;"
                        ButtonStyle="Default" EnableViewState="false" />
                </div>
            </div>
        </div>
        <iframe runat="server" id="invoiceFrame" ClientIDMode="Static" frameborder="0" style="width: 100%;"></iframe>
        <script type="text/javascript">
            // Adjust iframe height
            $cmsj('#invoiceFrame').load(function () {
                $cmsj(this).height($cmsj(this).contents().find('html')[0].scrollHeight);
            });
        </script>
    </div>
</asp:Content>
