<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_Menu"
     Codebehind="Menu.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/ViewModeMenu.ascx"
    TagName="ViewModeMenu" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/NewFile.ascx" TagPrefix="cms" TagName="NewFile" %>
<div class="cms-edit-menu">
    <div class="FloatLeft">
        <cms:NewFile runat="server" ID="NewFile" FileSystemMode="true" />
    </div>
    <asp:PlaceHolder ID="plcParentButton" runat="server">
        <div class="FloatLeft">
            <cms:LocalizedButton ID="btnParent" runat="server" ButtonStyle="Default" ResourceString="dialogs.mediaview.gotoparent" />
            <asp:HiddenField ID="hdnLastNodeParentPath" runat="server" />
        </div>
    </asp:PlaceHolder>
    <div class="RightAlign">
        <div class="control-group-inline">
            <div class="FloatLeft">
                <cms:ViewModeMenu ID="menuView" runat="server" />
            </div>
        </div>
    </div>
</div>
