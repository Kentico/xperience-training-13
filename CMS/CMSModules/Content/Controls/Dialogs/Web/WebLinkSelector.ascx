<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Web_WebLinkSelector"  Codebehind="WebLinkSelector.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/URLSelector.ascx" TagName="URLSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/HTMLLinkProperties.ascx"
    TagName="HTMLLinkProperties" TagPrefix="cms" %>

<script type="text/javascript" language="javascript">
    function insertItem() {
        RaiseHiddenPostBack();
    }
</script>

<div class="DialogWebContent">
    <div>
        <cms:URLSelector runat="server" ID="urlSelectElem" />
    </div>
</div>
<div class="DialogLinkWebProperties">
    <div>
        <asp:Panel ID="pnlProperties" runat="server">
            <cms:HTMLLinkProperties runat="server" ID="propLinkProperties" ShowGeneralTab="false"
                IsWeb="true" />
        </asp:Panel>
        <asp:Button ID="hdnButton" runat="server" OnClick="hdnButton_Click" CssClass="HiddenButton" />
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </div>
</div>
