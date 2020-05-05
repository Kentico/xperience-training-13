<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Properties_HTMLLinkProperties"  Codebehind="HTMLLinkProperties.ascx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/URLSelector.ascx" TagPrefix="cms"
    TagName="URLSelector" %>

<div>
    <asp:Panel runat="server" ID="pnlEmpty" Visible="true" CssClass="DialogInfoArea">
        <asp:Label runat="server" ID="lblEmpty" EnableViewState="false" />
    </asp:Panel>
    <cms:jquerytabcontainer id="pnlTabs" runat="server" cssclass="DialogElementHidden">
        <cms:JQueryTab ID="tabGeneral" runat="server">
            <ContentTemplate>
                <div class="PageContent">
                    <cms:URLSelector runat="server" ID="urlSelectElem" />
                </div>
            </ContentTemplate>
        </cms:JQueryTab>
        <cms:JQueryTab ID="tabTarget" runat="server">
            <ContentTemplate>
                <div class="PageContent">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblTargetName" runat="server" EnableViewState="false" ResourceString="dialogs.link.targetname"
                                    DisplayColon="true" AssociatedControlID="drpTarget" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSDropDownList ID="drpTarget" runat="server" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                 <cms:LocalizedLabel CssClass="control-label" ID="lblTargetFrame" runat="server" EnableViewState="false" ResourceString="dialogs.link.targetframe"
                                    DisplayColon="true" AssociatedControlID="txtTargetFrame" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtTargetFrame" EnableViewState="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cms:JQueryTab>
        <cms:JQueryTab ID="tabAdvanced" runat="server">
            <ContentTemplate>
                <div class="PageContent">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblAdvID" runat="server" EnableViewState="false" ResourceString="dialogs.advanced.id"
                                    DisplayColon="true" AssociatedControlID="txtAdvId" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtAdvId"  EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblAdvName" runat="server" EnableViewState="false" ResourceString="dialogs.advanced.name"
                                    DisplayColon="true" AssociatedControlID="txtAdvName" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtAdvName"  EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblAdvTooltip" runat="server" EnableViewState="false" ResourceString="dialogs.advanced.tooltip"
                                    DisplayColon="true" AssociatedControlID="txtAdvTooltip" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtAdvTooltip" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblAdvStyleSheet" runat="server" EnableViewState="false"
                                    ResourceString="dialogs.advanced.stylesheet" DisplayColon="true" AssociatedControlID="txtAdvStyleSheet" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtAdvStyleSheet" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblAdvStyle" runat="server" EnableViewState="false" ResourceString="dialogs.advanced.style"
                                    DisplayColon="true" AssociatedControlID="txtAdvStyle" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextArea runat="server" ID="txtAdvStyle" EnableViewState="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cms:JQueryTab>
    </cms:jquerytabcontainer>
</div>
<cms:CMSUpdatePanel id="pnlAdvancedTab" runat="server">
    <ContentTemplate>
        <asp:Button ID="btnHidden" runat="server" CssClass="HiddenButton" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>