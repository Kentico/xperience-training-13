<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_State"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="TaxClass_State.aspx.cs" %>

<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCountry" runat="server" EnableViewState="false" ResourceString="taxclass_state.lblcountry"
                    DisplayColon="true"></cms:LocalizedLabel>
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:CMSDropDownList ID="drpCountry" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpCountry_SelectedIndexChanged"
                    EnableViewState="true" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:UIGridView ID="gvStates" ShortID="g" runat="server" OnDataBound="gvStates_DataBound" AutoGenerateColumns="false" CssClass="form-inline">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label ID="lblState" runat="server" Text='<%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Eval("StateDisplayName").ToString()))%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="StateCode"></asp:BoundField>
            <asp:TemplateField>
                <ItemStyle CssClass="wrap-nowrap" />
                <ItemTemplate>
                    <div class="inline-editing-textbox">
                        <cms:CMSTextBox ID="txtTaxValue" runat="server" Text='<%# string.Format("{0:0.########}", DataBinder.Eval(Container.DataItem, "TaxValue"))%>'
                            MaxLength="10" OnTextChanged="txtTaxValue_Changed" EnableViewState="false" CssClass="input-width-15 editing-textbox"></cms:CMSTextBox>
                        <span>%</span>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="StateID">
                <ItemStyle />
            </asp:BoundField>
            <asp:TemplateField>
                <HeaderStyle CssClass="filling-column" />
                <ItemTemplate>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cms:UIGridView>
</asp:Content>
