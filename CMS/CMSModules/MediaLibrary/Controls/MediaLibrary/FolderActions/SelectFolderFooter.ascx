<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_SelectFolderFooter"  Codebehind="SelectFolderFooter.ascx.cs" %>

<div class="PageFooterLine">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnInsert" runat="server" ResourceString="dialogs.actions.insert"
            ButtonStyle="Primary" EnableViewState="false" /><cms:LocalizedButton ID="btnCancel"
                runat="server" ResourceString="dialogs.actions.cancel" ButtonStyle="Primary"
                EnableViewState="false" />
    </div>
</div>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />