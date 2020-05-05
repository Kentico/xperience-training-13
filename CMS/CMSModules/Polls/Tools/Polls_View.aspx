<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Polls_Tools_Polls_View"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Polls - View poll" 
    Theme="Default"  Codebehind="Polls_View.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/View/PollView.ascx" TagName="PollView" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div style="width: 400px;">
        <cms:PollView ID="pollElem" runat="server" />
    </div>
</asp:Content>
