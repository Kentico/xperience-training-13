<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CMSDeskTree.ascx.cs" Inherits="CMSModules_Content_Controls_CMSDeskTree" %>
<%@ Register TagPrefix="cms" TagName="ContentTree" Src="~/CMSModules/Content/Controls/ContentTree.ascx" %>
<%@ Register TagPrefix="cms" TagName="RefreshIcon" Src="~/CMSModules/Content/Controls/RefreshIcon.ascx" %>
<cms:ContextMenuContainer runat="server" ID="menuCont" MenuID="nodeMenu" OffsetX="10"
    ActiveItemCssClass="TreeContextActiveNode" HorizontalPosition="Left">
</cms:ContextMenuContainer>
<asp:Panel runat="server" ID="pnlBody" EnableViewState="false"
    oncontextmenu="return false;">
    <asp:Panel onclick="HideAllContextMenus();" ID="pnlTreeArea" runat="server" CssClass="ContentTreeArea tree-vertical-scroll">
        <div class="TreeAreaTree">
            <cms:ContentTree ID="treeElem" runat="server" AllowDragAndDrop="true" ShortID="t"
                IsLiveSite="false" AllowMarks="true" />
        </div>
    </asp:Panel>
</asp:Panel>
<input type="hidden" id="hdnAction" name="hdnAction" />
<input type="hidden" id="hdnParam1" name="hdnParam1" />
<input type="hidden" id="hdnParam2" name="hdnParam2" />
<input type="hidden" id="hdnScroll" name="hdnScroll" />