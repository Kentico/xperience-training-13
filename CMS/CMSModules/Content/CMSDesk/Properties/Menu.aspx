<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Menu"
    Theme="Default" CodeBehind="Menu.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" ShowReject="true" ShowSubmitToApproval="true" ShowProperties="false" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server">
        <asp:Label ID="lblWorkflow" runat="server" CssClass="InfoLabel" EnableViewState="false"
            Visible="false" />
        <asp:Panel ID="pnlForm" runat="server">
            <asp:Panel ID="pnlBasicProperties" runat="server">
                <cms:LocalizedHeading runat="server" ID="headBasicProperties" Level="4" ResourceString="content.menu.basic" EnableViewState="false" />
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblShowMenu" runat="server" ResourceString="MenuProperties.showinmenu" AssociatedControlID="chkShowInMenu" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkShowInMenu" runat="server" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <br />
        </asp:Panel>
    </asp:Panel>
    <div class="Clear">
    </div>
</asp:Content>
