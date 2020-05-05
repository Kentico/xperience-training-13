<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="New.aspx.cs" 
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Custom activity properties" 
    Inherits="CMSModules_Activities_Pages_Tools_Activities_Activity_New" Theme="Default" %>
    
<%@ Register Src="~/CMSModules/Activities/Controls/UI/Activity/Edit.ascx" TagName="ActivityEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ActivityEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>