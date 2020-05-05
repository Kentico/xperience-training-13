<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_FormControls_MultipleGroupSelector"
     Codebehind="MultipleGroupSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>
    
<cms:UniSelector ObjectType="community.group" SelectionMode="Multiple" ReturnColumnName="GroupName"
    OrderBy="GroupDisplayName" ResourcePrefix="groups" AllowEmpty="false" runat="server" ID="usGroups" ShortID="s" />
