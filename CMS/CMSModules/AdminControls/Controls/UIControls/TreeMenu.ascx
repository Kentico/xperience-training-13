<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/TreeMenu.ascx.cs" Inherits="CMSModules_AdminControls_Controls_UIControls_TreeMenu" %>
<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIMenu.ascx" TagName="UIMenu"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>

<cms:UIElementLayout runat="server" ID="layoutElem">
    <Panes>
        <cms:UILayoutPane ID="paneTitle" runat="server" Direction="North" RednerAs="Div"
            Size="38" SpacingOpen="0">
            <Template>
                <cms:PageTitle runat="server" ID="pageTitle" />
            </Template>
        </cms:UILayoutPane>
        <cms:UILayoutPane ID="paneContent" runat="server" Direction="West" RenderAs="Div" PaneClass="ui-layout-pane-scroll"
            Size="304" Resizable="true" UseUpdatePanel="false" SpacingOpen="8" SpacingClosed="8"
            TogglerLengthOpen="32" TogglerLengthClosed="32">
            <Template>
                <asp:Panel runat="server" ID="pnlWrapper" CssClass="TreeAreaTree">
                    <asp:Panel ID="ContentTree" runat="server" CssClass="ContentTree">
                        <cms:UIMenu runat="server" ID="treeElem" ModuleName="<%# ResourceName %>" ElementName="<%# ElementName %>"
                            UseIFrame="true" QueryParameterName="node" 
                            TargetFrame="paneContentTMain" EnableRootSelect="False" MaxRelativeLevel="<%# MaxRelativeLevel %>" />
                    </asp:Panel>
                </asp:Panel>
            </Template>
        </cms:UILayoutPane>
        <cms:UILayoutPane ID="paneContentTMain" runat="server" Direction="Center" RenderAs="Iframe" MaskContents="true" AppendSrc="false"
            UseUpdatePanel="false" Visible="true" />
    </Panes>
</cms:UIElementLayout>
