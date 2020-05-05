<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Forums_ForumList"
     Codebehind="ForumList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:UniGrid runat="server" ID="gridElem" GridName="~/CMSModules/Forums/Controls/Forums/Forum_List.xml"
    DelayedReload="true" />
