<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Sites_Pages_CultureChange"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Site - Change culture"  Codebehind="CultureChange.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />
    <asp:Panel ID="pnlForm" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblNewCulture" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                     <cms:SiteCultureSelector runat="server" ID="cultureSelector" IsLiveSite="false" AllowDefault="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel DisplayColon="True" AssociatedControlID="chkDocuments" ResourceString="SiteDefaultCultureChange.Documents" CssClass="control-label" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-label-cell">
                     <cms:CMSCheckBox ID="chkDocuments" runat="server" Checked="true" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" ResourceString="general.saveandclose" EnableViewState="False"
        OnClick="btnOk_Click" />
</asp:Content>
