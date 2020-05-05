<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Poll answer - Edit"
    Inherits="CMSModules_Polls_Tools_Polls_Answer_Edit" Theme="Default"  Codebehind="Polls_Answer_Edit.aspx.cs" %>
<%@ Register Src="~/CMSModules/Polls/Controls/AnswerEdit.ascx" TagName="AnswerEdit"
    TagPrefix="cms" %>
        
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:AnswerEdit ID="AnswerEdit" runat="server" Visible="true" />    
</asp:Content>
