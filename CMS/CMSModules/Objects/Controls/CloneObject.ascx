<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Objects_Controls_CloneObject"
     Codebehind="CloneObject.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<script type="text/javascript">
    //<![CDATA[
    function ShowHideAdvancedSection() {
        $cmsj('#divAdvanced, #divSimple, #advancedItems').toggleClass("hidden");
    }
    //]]>
</script>
<cms:LocalizedHeading runat="server" ID="headGeneral" Level="4" ResourceString="general.general" EnableViewState="false" />
<div class="form-horizontal">
    <asp:PlaceHolder runat="server" ID="plcDisplayName" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDisplayName" EnableViewState="false" DisplayColon="true" AssociatedControlID="txtDisplayName:cntrlContainer:textbox" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox runat="server" ID="txtDisplayName" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcCodeName" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCodeName" EnableViewState="false" DisplayColon="true" AssociatedControlID="txtCodeName:textbox" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName runat="server" ID="txtCodeName" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div id="divSimple">
        <cms:LocalizedHyperlink runat="server" ID="lblShowAdvanced" EnableViewState="false" href="#" onclick="ShowHideAdvancedSection();" ResourceString="clonning.settings.showadvanced" />
    </div>
    <div id="divAdvanced" class="hidden form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedHyperlink runat="server" ID="lblShowSimple" EnableViewState="false" href="#" onclick="ShowHideAdvancedSection();" ResourceString="clonning.settings.showsimple" />
        </div>
    </div>
    <div id="advancedItems" class="hidden">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUseTransaction" ResourceString="clonning.settings.usetransaction" AssociatedControlID="chkUseTransaction"
                    EnableViewState="false" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkUseTransaction" Checked="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblKeepFieldsTranslated" ResourceString="clonning.settings.keepfieldstranslated" AssociatedControlID="chkKeepFieldsTranslated"
                    EnableViewState="false" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkKeepFieldsTranslated" Checked="false" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcCloneUnderSite" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCloneUnderSite" ResourceString="clonning.settings.cloneundersite" AssociatedControlID="siteElem:ss:drpSingleSelect"
                        EnableViewState="false" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:SiteSelector runat="server" ID="siteElem" AllowAll="false" AllowEmpty="false"
                        AllowGlobal="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcChildren" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblChildren" EnableViewState="false" DisplayColon="true" AssociatedControlID="chkChildren" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkChildren" Checked="true" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcChildrenLevel" Visible="true">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblMaxRelativeLevel" ResourceString="clonning.settings.maxrelativelevel" AssociatedControlID="drpMaxRelativeLevel"
                            EnableViewState="false" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSDropDownList runat="server" ID="drpMaxRelativeLevel" CssClass="DropDownField" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcAssignToCurrentSite" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAssignToCurrentSite" ResourceString="clonning.settings.assigntocurrentsite" AssociatedControlID="chkAssignToCurrentSite"
                        EnableViewState="false" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkAssignToCurrentSite" Checked="true" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcSiteBindings" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSiteBindings" ResourceString="clonning.settings.sitebindings" AssociatedControlID="chkSiteBindings"
                        EnableViewState="false" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkSiteBindings" Checked="true" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcBindings" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblBindings" EnableViewState="false" DisplayColon="true" AssociatedControlID="chkBindings" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkBindings" Checked="true" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcMetafiles" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblMetafiles" ResourceString="clonning.settings.metafiles" AssociatedControlID="chkMetafiles"
                        EnableViewState="false" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkMetafiles" Checked="true" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</div>
<asp:PlaceHolder runat="server" ID="plcCustomParametersBox" Visible="false">
    <cms:LocalizedHeading runat="server" ID="headCustom" Level="4" EnableViewState="false" />
    <asp:PlaceHolder runat="server" ID="plcCustomParameters" />
</asp:PlaceHolder>
