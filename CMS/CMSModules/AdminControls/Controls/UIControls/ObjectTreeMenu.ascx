<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/ObjectTreeMenu.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_UIControls_ObjectTreeMenu" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<cms:UIElementLayout runat="server" ID="layoutElem" CssClass="nav-tree-layout">
    <Panes>
        <cms:UILayoutPane ID="paneTitle" runat="server" Direction="North" RednerAs="Div"
            Size="38" SpacingOpen="0">
            <Template>
                <cms:PageTitle runat="server" ID="pageTitle" />
            </Template>
        </cms:UILayoutPane>
        <cms:UILayoutPane ID="paneTree" runat="server" Direction="West" RenderAs="Div" Size="304"
            Resizable="true" UseUpdatePanel="false" ControlPath="~/CMSModules/AdminControls/Controls/UIControls/GeneralTree.ascx"
            SpacingOpen="8" SpacingClosed="8" TogglerLengthOpen="32" TogglerLengthClosed="32" />
        <cms:UILayoutPane ID="paneContentTMain" runat="server" Direction="Center" RenderAs="Iframe" AppendSrc="false"
            MaskContents="true" UseUpdatePanel="false" Visible="true" />
    </Panes>
</cms:UIElementLayout>
