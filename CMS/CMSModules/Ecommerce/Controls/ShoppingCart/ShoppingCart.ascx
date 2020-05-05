<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCart"  Codebehind="ShoppingCart.ascx.cs" %>
<asp:Panel ID="pnlShoppingCart" runat="server" DefaultButton="btnNext">
    <asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel"
        Visible="false" />
    <asp:Label ID="lblInfo" runat="server" EnableViewState="false" CssClass="InfoLabel"
        Visible="false" />
    <table class="CartStepTable">
        <tr class="UniGridHead">
            <th class="CartStepHeader">
                <div class="wizard-header">
                    <div class="wizard-title">
                        <asp:Label ID="lblStepTitle" runat="server" />
                    </div>
                    <div class="wizard-description">
                        <cms:LocalizedHeading runat="server" ID="headStepTitle" Level="2" EnableViewState="False" />
                    </div>
                </div>
            </th>
        </tr>
        <asp:PlaceHolder ID="plcCheckoutProcess" runat="server" EnableViewState="false">
            <tr class="CartStepBody">
                <td colspan="2">
                    <asp:PlaceHolder ID="plcStepImages" runat="server" EnableViewState="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr class="CartStepBody">
            <td>
                <asp:Panel ID="plcCartStep" runat="server" CssClass="CartStepPanel">
                    <asp:Panel ID="pnlCartStepInner" runat="server" CssClass="CartStepInnerPanel" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td class="CartFooter">
                <div class="CartStepFooter">
                    <div class="TextLeft">
                        <cms:CMSButton ID="btnBack" runat="server" OnClick="btnBack_Click" ButtonStyle="Primary"
                            ValidationGroup="ButtonBack" EnableViewState="false" />
                    </div>
                    <div class="TextRight">
                        <cms:CMSButton ID="btnNext" runat="server" OnClick="btnNext_Click" ButtonStyle="Primary"
                            ValidationGroup="ButtonNext" RenderScript="true" EnableViewState="false" />
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>
