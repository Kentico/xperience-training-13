<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Tools_Groups_Group_General" 
 MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Group General"  Codebehind="Group_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Groups/GroupEdit.ascx" TagName="GroupEdit" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:GroupEdit ID="groupEdit" runat="server" />
 </asp:Content>