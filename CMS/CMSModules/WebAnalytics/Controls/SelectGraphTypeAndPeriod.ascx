<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SelectGraphTypeAndPeriod.ascx.cs"
    Inherits="CMSModules_WebAnalytics_Controls_SelectGraphTypeAndPeriod" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphType.ascx" TagName="GraphType"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphPeriod.ascx" TagName="GraphPeriod"
    TagPrefix="cms" %>
<div>
    <div class="FloatLeft" runat="server" id="pnlGraphType">
        <cms:GraphType runat="server" ID="ucGraphType" />
    </div>
    <div class="FloatRight">
        <cms:GraphPeriod runat="server" ID="ucGraphPeriod" />
    </div>
</div>
<div style="clear: both">
</div>
