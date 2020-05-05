<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Members_Member_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Group nembers list"  Codebehind="Member_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Groups/Controls/Members/MemberList.ascx" TagName="MemberList"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MemberList ID="memberListElem" runat="server" IsLiveSite="false" />
</asp:Content>