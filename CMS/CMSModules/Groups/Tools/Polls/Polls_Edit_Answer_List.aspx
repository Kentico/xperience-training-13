<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Tools_Polls_Polls_Edit_Answer_List" Title="Poll answers - List"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"  Codebehind="Polls_Edit_Answer_List.aspx.cs" %>
<%@ Register Src="~/CMSModules/Polls/Controls/AnswerList.ascx" TagName="AnswerList"
    TagPrefix="cms" %>
    
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:AnswerList ID="AnswerList" runat="server" Visible="true" />
</asp:Content>
