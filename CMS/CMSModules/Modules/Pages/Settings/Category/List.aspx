<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="List.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Inherits="CMSModules_Modules_Pages_Settings_Category_List" %>

<%@ Register Src="~/CMSModules/Modules/Controls/Settings/Category/SettingsGroupEdit.ascx" TagName="SettingsGroupEdit"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <div class="WebPartForm">
        <cms:SettingsGroupEdit ID="grpEdit" runat="server" DisplayMode="Default" IsLiveSite="false" />
    </div>
</asp:Content>
