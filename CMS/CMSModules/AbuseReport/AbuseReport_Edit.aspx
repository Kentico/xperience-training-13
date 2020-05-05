<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_AbuseReport_AbuseReport_Edit" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default"  Codebehind="AbuseReport_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/AbuseReport/Controls/AbuseReportStatusEdit.ascx" TagName="AbuseReportEdit" TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
   <cms:AbuseReportEdit ID= "ucAbuseEdit" runat="server" IsLiveSite="false" />   
</asp:Content>
