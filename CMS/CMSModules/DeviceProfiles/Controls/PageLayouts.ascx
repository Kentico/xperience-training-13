<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="PageLayouts.ascx.cs" Inherits="CMSModules_DeviceProfiles_Controls_PageLayouts" %>

<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/DeviceProfiles/Controls/LayoutBinding.ascx" TagPrefix="cms" TagName="LayoutBinding" %>

<asp:hiddenfield id="SourceLayoutIdentifierHiddenField" runat="server" enableviewstate="false" />
<asp:hiddenfield id="TargetLayoutIdentifierHiddenField" runat="server" enableviewstate="false" />
<p style="margin-left: 5px">
    <cms:LocalizedLabel runat="server" ResourceString="device_profile.layoutmapping.introduction"></cms:LocalizedLabel>
</p>
<div class="DeviceProfileLayoutGrid UniGridClearPager">
    <cms:UniGrid ID="BindingGrid" runat="server" Columns="LayoutID, LayoutDisplayName"
        ObjectType="cms.layout" OrderBy="LayoutDisplayName"
        ShowActionsLabel="false" ShowActionsMenu="false" ShowObjectMenu="false" RememberStateByParam="">
        <GridColumns>
            <ug:Column Source="LayoutID" runat="server" Caption="$device_profile.layoutmapping$" ExternalSourceName="SourceLayout"
                AllowSorting="false">
            </ug:Column>
            <ug:Column Source="LayoutDisplayName" runat="server" Caption="$device_profile.layoutmapping.sourcelayoutname$"
                Visible="false">
                <Filter Type="text" />
            </ug:Column>
        </GridColumns>
        <GridOptions DisplayFilter="true" FilterLimit="5" />
    </cms:UniGrid>
</div>

<script type="text/javascript">
    //<![CDATA[

    function Client_SetTargetLayout(parameters) {
        $cmsj('#<%= SourceLayoutIdentifierHiddenField.ClientID %>').val(parameters.sourceLayoutId);
        $cmsj('#<%= TargetLayoutIdentifierHiddenField.ClientID %>').val(parameters.targetLayoutId);
            <%= ControlsHelper.GetPostBackEventReference(this, "SetTargetLayout") %>
    }

    function Client_UnsetTargetLayout(parameters) {
        if (confirm('<%= HTMLHelper.HTMLEncode(ResHelper.GetString("device_profile.layoutmapping.confirmunset"))%>')) {
            $cmsj('#<%= SourceLayoutIdentifierHiddenField.ClientID %>').val(parameters.sourceLayoutId);
            <%= ControlsHelper.GetPostBackEventReference(this, "UnsetTargetLayout") %>
        }
    }

    //]]>
</script>
