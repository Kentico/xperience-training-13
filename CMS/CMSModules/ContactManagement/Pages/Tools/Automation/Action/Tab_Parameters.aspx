<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Parameters.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Marketing Automation Action Properties – Parameters"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Action_Tab_Parameters" Theme="Default" %>
            
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldEditor.ascx"
    TagName="FieldEditor" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:FieldEditor ID="fieldEditor" runat="server" Mode="ProcessActions" IsLiveSite="false" />
</asp:Content>
