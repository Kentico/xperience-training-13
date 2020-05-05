<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Tools_Groups_Group_New"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="default"  Codebehind="Group_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Groups/GroupNew.ascx" TagName="GroupNew" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:GroupNew ID="groupNewElem" runat="server" />
</asp:Content>