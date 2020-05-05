<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_Country"
    Theme="Default" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
     Codebehind="TaxClass_Country.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:UIGridView ID="GridViewCountries" runat="server" ShortID="g" AutoGenerateColumns="false" OnDataBound="GridViewCountries_DataBound">
        <Columns>
            <asp:TemplateField >
                <ItemTemplate>
                    <asp:Label ID="lblName" runat="server" Text='<%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Eval("CountryDisplayName").ToString()))%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <div class="inline-editing-textbox">
                        <cms:CMSTextBox ID="txtTaxValue" runat="server" Text='<%# String.Format("{0:0.########}", DataBinder.Eval(Container.DataItem, "TaxValue"))%>'
                            MaxLength="10" OnTextChanged="txtTaxValue_Changed" EnableViewState="false" CssClass="input-width-15 editing-textbox"></cms:CMSTextBox>
                        <span>%</span>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CountryID">
                <ItemStyle />
            </asp:BoundField>
            <asp:TemplateField>
                <HeaderStyle Width="100%" />
                <ItemTemplate>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cms:UIGridView>
</asp:Content>
