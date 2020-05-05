<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_Modules_Pages_Class_Fields"
    Title="Class- Edit- Fields" Theme="Default"  Codebehind="Fields.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldEditor.ascx"
    TagName="FieldEditor" TagPrefix="cms" %>
<asp:Content ID="plcContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:FieldEditor ID="fieldEditor" ShortID="f" runat="server" AllowDummyFields="true"
        Visible="false" Enabled="false" IsLiveSite="false" />
</asp:Content>
