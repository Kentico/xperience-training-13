<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Community_UserContributions_GroupContributionList"  Codebehind="~/CMSWebParts/Community/UserContributions/GroupContributionList.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/UserContributions/ContributionList.ascx" TagName="ContributionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/UserContributions/EditForm.ascx" TagName="ContributionEdit" TagPrefix="cms" %>
<cms:ContributionList ID="list" runat="server" />


