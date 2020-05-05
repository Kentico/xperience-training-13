<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Controls_GroupList"  Codebehind="GroupList.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<cms:UniGrid runat="server" ID="gridElem" GridName="~/CMSModules/Groups/Controls/Group_List.xml"
    OrderBy="GroupDisplayName" />