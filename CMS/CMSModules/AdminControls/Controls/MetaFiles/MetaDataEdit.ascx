<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="MetaDataEdit.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_MetaFiles_MetaDataEdit" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:Panel ID="pnlFormHorizontal" runat="server" CssClass="form-horizontal">
    <asp:PlaceHolder ID="plcFileName" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFileName" runat="server" AssociatedControlID="txtFileName" EnableViewState="false" ResourceString="general.filename"
                    DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtFileName" runat="server" MaxLength="250" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblTitle" runat="server" EnableViewState="false" AssociatedControlID="txtTitle" ResourceString="general.title"
                DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtTitle" runat="server" MaxLength="250" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblDescription" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.description" AssociatedControlID="txtDescription" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtDescription" runat="server" TextMode="MultiLine" MaxLength="4000" EnableViewState="false" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcExtensionAndSize" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblExtensionLabel" runat="server" AssociatedControlID="lblExtension" EnableViewState="false"
                    ResourceString="img.extension" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblExtension" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSizeLabel" runat="server" AssociatedControlID="lblSize" EnableViewState="false" ResourceString="general.size"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label CssClass="form-control-text" ID="lblSize" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Panel>

<script type="text/javascript">
    //<![CDATA[
    function RefreshMetaFile() {
        if (wopener.UpdatePage) {
            wopener.UpdatePage();
        }
        else {
            wopener.location.replace(wopener.location);
        }
    }

    function RefreshMetaData(clientId, fullRefresh, guid, action) {
        eval("if (wopener.InitRefresh_" + clientId + ") { wopener.InitRefresh_" + clientId + "('', " + fullRefresh + ", 'attachmentguid|" + guid + "', '" + action + "'); }");
    }
    //]]>
</script>

<asp:Literal ID="ltlScript" runat="server" />