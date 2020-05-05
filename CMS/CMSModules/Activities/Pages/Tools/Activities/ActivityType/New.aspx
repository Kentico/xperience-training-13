<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="New.aspx.cs" 
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Activity type properties" 
    Inherits="CMSModules_Activities_Pages_Tools_Activities_ActivityType_New" Theme="Default" %>
    
<%@ Register Src="~/CMSModules/Activities/Controls/UI/ActivityType/Edit.ascx" TagName="ActivityTypeEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ActivityTypeEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>