<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" 
AutoEventWireup="true"  Codebehind="Edit.aspx.cs" Inherits="CMSModules_Modules_Pages_Settings_Key_Edit" Theme="Default" %>
<%@ Register TagPrefix="cms" TagName="SettingsKeyEdit" Src="~/CMSModules/Modules/Controls/Settings/Key/SettingsKeyEdit.ascx" %>

<asp:Content ID="Content5" ContentPlaceHolderID="plcContent" runat="server">
<cms:SettingsKeyEdit ID="skeEditKey" runat="server" />
</asp:Content>
