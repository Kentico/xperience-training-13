<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SelectSKUBinding.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_SelectSKUBinding" %>
<%@ Register TagPrefix="cms" TagName="SKUSelector" Src="~/CMSModules/Ecommerce/FormControls/SKUSelector.ascx" %>
<div class="form-horizontal" role="form">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblCreate" runat="server" CssClass="control-label" ResourceString="com.selectskubinding.create" DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <div class="radio-list-vertical">
                <%-- Create new --%>
                <div class="radio">
                    <cms:CMSRadioButton ID="radCreateNew" runat="server" OnCheckedChanged="radioButton_CheckedChanged" ResourceString="com.selectskubinding.createnew"
                        GroupName="SelectSKUBindingRadioGroup" Checked="true" AutoPostBack="true" />
                </div>
                <%-- Create new global --%>
                <asp:Panel ID="pnlCreateNewGlobal" runat="server" CssClass="radio" Visible="false">
                    <cms:CMSRadioButton ID="radCreateNewGlobal" runat="server" OnCheckedChanged="radioButton_CheckedChanged" ResourceString="com.selectskubinding.createnewglobal"
                        GroupName="SelectSKUBindingRadioGroup" AutoPostBack="true" />
                </asp:Panel>
                <%-- Use existing --%>
                <div class="radio">
                    <cms:CMSRadioButton ID="radUseExisting" runat="server" OnCheckedChanged="radioButton_CheckedChanged" ResourceString="com.selectskubinding.useexisting"
                        GroupName="SelectSKUBindingRadioGroup" Visible="false" AutoPostBack="true" />
                </div>
                <asp:Panel ID="pnlSkuSelector" runat="server" CssClass="selector-subitem" Visible="false">
                    <cms:SKUSelector runat="server" ID="skuSelectorElem" IsLiveSite="false" />
                </asp:Panel>
                <%-- Do not create --%>
                <asp:Panel ID="pnlDoNotCreate" runat="server" CssClass="radio" Visible="false">
                    <cms:CMSRadioButton ID="radDoNotCreate" runat="server" OnCheckedChanged="radioButton_CheckedChanged" ResourceString="com.selectskubinding.donotcreate"
                        GroupName="SelectSKUBindingRadioGroup" AutoPostBack="true" />
                </asp:Panel>
            </div>
            <cms:LocalizedLabel ID="lblInfo" runat="server" Visible="false" CssClass="explanation-text"></cms:LocalizedLabel>
        </div>
    </div>
</div>

