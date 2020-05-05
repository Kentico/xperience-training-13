<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Members_Member_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Group nembers edit"  Codebehind="Member_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/Groups/Controls/Members/MemberEdit.ascx" TagName="MemberEdit"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MemberEdit ID="memberEditElem" runat="server" IsLiveSite="false" />
</asp:Content>
