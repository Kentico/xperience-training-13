<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/OnlineMarketing/Activities.ascx.cs" Inherits="CMSWebParts_OnlineMarketing_Activities" %>
<%@ Register Src="~/CMSModules/Activities/Controls/UI/Activity/List.ascx" TagName="ActivityList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms" TagName="DisabledModuleInfo" %>

<cms:DisabledModuleInfo runat="server" id="disabledModuleInfo" />
<cms:ActivityList ShowSelection="false" runat="server" ID="listElem" Visible="false" />
