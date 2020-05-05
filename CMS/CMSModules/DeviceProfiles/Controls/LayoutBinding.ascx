<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="LayoutBinding.ascx.cs"
    Inherits="CMSModules_DeviceProfiles_Controls_LayoutBinding" %>
<div class="DeviceProfileLayoutMapping">
    <div class="FlatItem">
        <div class="SelectorEnvelope DeviceProfileSourceLayout" style="overflow: hidden">
            <div class="SelectorFlatImage">
                <asp:Literal ID="ltrSourceLayoutIcon" runat="server"></asp:Literal>
            </div>
            <asp:Label ID="SourceLayoutDisplayNameLabel" runat="server" CssClass="SelectorFlatText"></asp:Label>
        </div>
    </div>
    <div class="MappingDirection">
    </div>
    <div class="FlatItem" title="<%= HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.sethint")) %>">
        <div class="SelectorEnvelope" style="overflow: hidden" id="TargetLayoutItemControl" runat="server">
            <% if (TargetLayout != null)
               { %>
                <cms:CMSAccessibleButton runat="server" ID="btnDelete" CssClass="RemoveButton" IconCssClass="icon-bin" IconOnly="true"/>
            <% } %>
            <div class="SelectorFlatImage">
                <asp:Literal ID="ltrTargetLayoutIcon" runat="server"></asp:Literal>
            </div>
            <asp:Label ID="TargetLayoutDisplayNameLabel" runat="server" CssClass="SelectorFlatText"></asp:Label>
        </div>
    </div>
    <div style="clear: both">
    </div>
</div>
