<%@ Page Language="C#" AutoEventWireup="true" 
    Inherits="CMSModules_Groups_Tools_Group_Edit_General" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Group edit - General"  Codebehind="Group_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Groups/Controls/GroupEdit.ascx" TagPrefix="cms" TagName="GroupEdit" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:GroupEdit ID="groupEditElem" runat="server" />
</asp:Content>
