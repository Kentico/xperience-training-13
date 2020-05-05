<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Pages_Users_User_Edit_CustomFields"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="User edit - Custom fields"
     Codebehind="User_Edit_CustomFields.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="UserCustomFields">
        <asp:PlaceHolder runat="server" ID="plcUserCustomFields">
            <h3>
                <cms:LocalizedLabel runat="server" ID="lblUserCustomFields" EnableViewState="false" ResourceString="adm.user.customfields" />
            </h3>
            <cms:DataForm ID="formUserCustomFields" runat="server" IsLiveSite="false" />
            <div style="padding: 15px" class="ClearBoth">
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcUserSettingsCustomFields">
            <h3>
                <cms:LocalizedLabel runat="server" ID="lblUserSettingsCustomFields" EnableViewState="false" ResourceString="adm.usersettings.customfields" />
            </h3>
            <cms:DataForm ID="formUserSettingsCustomFields" runat="server" IsLiveSite="false" />
        </asp:PlaceHolder>
        <div class="ClearBoth">
        </div>
        <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_OnClick" />
    </div>
</asp:Content>
