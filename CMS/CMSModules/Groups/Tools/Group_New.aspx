<%@ Page Language="C#" AutoEventWireup="true" 
    Inherits="CMSModules_Groups_Tools_Group_New" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="New group"  Codebehind="Group_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/Groups/Controls/GroupEdit.ascx" TagPrefix="cms" TagName="GroupEdit" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:GroupEdit ID="groupEditElem" runat="server" GroupID="0" />
</asp:Content>
