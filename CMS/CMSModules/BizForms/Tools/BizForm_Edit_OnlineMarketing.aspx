<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="BizForm_Edit_OnlineMarketing.aspx.cs" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_OnlineMarketing"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Form properties - On-line marketing" Theme="Default" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <cms:CMSCheckBox ID="chkLogActivity" runat="server" ResourceString="bizformgeneral.lbllogactivity" CssClass="ContentCheckbox" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcMapping" runat="server" />
</asp:Content>