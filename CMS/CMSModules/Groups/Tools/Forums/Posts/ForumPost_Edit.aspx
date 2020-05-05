<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Forums_Posts_ForumPost_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" ValidateRequest="false"
     Codebehind="ForumPost_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Posts/PostEdit.ascx" TagName="PostEdit"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="ForumFlat">
        <cms:PostEdit ID="postEdit" runat="server" DisableCancelButton="true" />
        <asp:Literal ID="ltlScript" runat="server" />
    </div>
</asp:Content>
