<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Forums_Groups_ForumGroup_General" 
 MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Group forum group general"  Codebehind="ForumGroup_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Groups/GroupEdit.ascx" TagPrefix="cms" TagName="GroupEdit" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:GroupEdit ID="groupEdit" runat="server" />
 </asp:Content>