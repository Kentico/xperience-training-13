<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Forums_Groups_ForumGroup_View" 
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" ValidateRequest="false"   Codebehind="ForumGroup_View.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/ForumDivider.ascx" TagName="Forum" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
   <cms:Forum ID="Forum1" runat="server" RedirectToUserProfile="false"   />
</asp:Content>