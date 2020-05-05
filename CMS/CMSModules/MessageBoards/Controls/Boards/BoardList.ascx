<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_Boards_BoardList"  Codebehind="BoardList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Panel class="form-horizontal form-filter" runat="server" DefaultButton="btnFilter">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-group">
        <div class="filter-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblBoardName" AssociatedControlID="txtBoardName" runat="server" EnableViewState="false" />
        </div>
        <div class="filter-form-value-cell">
            <cms:CMSTextBox ID="txtBoardName" runat="server" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group form-group-buttons">
        <div class="filter-form-buttons-cell">
            <cms:CMSButton ID="btnReset" runat="server" ButtonStyle="Default" OnClick="btnReset_Click" EnableViewState="false" />
            <cms:CMSButton ID="btnFilter" runat="server" ButtonStyle="Primary" OnClick="btnFilter_Click"
                EnableViewState="false" />
        </div>
    </div>
</asp:Panel>
<cms:UniGrid ID="gridBoards" runat="server" ExportFileName="board_board" />
