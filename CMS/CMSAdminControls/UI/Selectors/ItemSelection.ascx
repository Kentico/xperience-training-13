<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSAdminControls_UI_Selectors_ItemSelection"  Codebehind="ItemSelection.ascx.cs" %>

<div class="item-selection">
    <div class="list-column">
        <strong><asp:Label ID="lblLeftColumn" runat="server" /></strong>
        <cms:CMSListBox ID="lstLeftColumn" runat="server" SelectionMode="Multiple" Rows="11" />
     </div>
    <div class="button-column">
        <cms:CMSButton ID="btnMoveRight" runat="server" Text=">" ButtonStyle="Primary" Font-Bold="True"
        Font-Size="Larger" OnClick="MoveRightButton_Click" />
        <cms:CMSButton ID="btnMoveLeft" runat="server" Text="<" ButtonStyle="Primary" Font-Bold="True"
            Font-Size="Larger" OnClick="MoveLeftButton_Click" />
    </div>
    <div class="list-column">
        <strong><asp:Label ID="lblRightColumn" runat="server" /></strong>
        <cms:CMSListBox ID="lstRightColumn" runat="server" SelectionMode="Multiple" Rows="11"
                        OnSelectedIndexChanged="lstRightColumn_SelectedIndexChanged" />
    </div>
    <div class="button-column">
        <cms:CMSPlaceHolder ID="plcButtons" runat="server">
            <cms:CMSButton ID="btnUp" runat="server" ButtonStyle="Primary" OnClick="btnUp_Click"
                Enabled="False" />
            <cms:CMSButton ID="btnDown" runat="server" ButtonStyle="Primary" OnClick="btnDown_Click"
                Enabled="False" />
        </cms:CMSPlaceHolder>
    </div>
</div>