<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_FormControls_Documents_SelectDocument"
     Codebehind="SelectDocument.ascx.cs" %>
<cms:CMSUpdatePanel ID="pnlUpdateHidden" runat="server">
    <ContentTemplate>
        <div class="control-group-inline">
            <cms:CMSTextBox ID="txtName" runat="server" MaxLength="800" CssClass="form-control" />
            <cms:CMSButton
                ID="btnSelect" runat="server" ButtonStyle="Default" />
            <cms:CMSButton ID="btnClear" runat="server" ButtonStyle="Default" />
            <cms:CMSTextBox ID="txtGuid" runat="server" AutoPostBack="true" CssClass="Hidden" />
            <cms:LocalizedLabel ID="lblGuid" runat="server" EnableViewState="false" Display="false" AssociatedControlID="txtGuid" ResourceString="development_formusercontrol_edit.lblforguid" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
