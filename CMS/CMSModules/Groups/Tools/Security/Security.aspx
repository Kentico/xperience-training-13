<%@ Page MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Security_Security"  Codebehind="Security.aspx.cs" %>

<%@ Register Src="~/CMSModules/Groups/Controls/Security/GroupSecurity.ascx" TagPrefix="cms"
    TagName="GroupSecurity" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:GroupSecurity runat="server" ID="groupSecurity"></cms:GroupSecurity>
</asp:Content>
