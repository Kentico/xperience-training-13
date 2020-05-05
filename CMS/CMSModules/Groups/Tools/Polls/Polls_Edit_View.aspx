<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Tools_Polls_Polls_Edit_View" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Groups polls edit - view"  Codebehind="Polls_Edit_View.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/View/PollView.ascx" TagName="PollView" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div style="width: 300px;" class="GlobalWizard">
        <cms:PollView ID="PollView" runat="server" />
    </div>
</asp:Content>
