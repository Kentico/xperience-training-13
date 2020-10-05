<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Dialogs_DialogStartConfiguration" Codebehind="DialogStartConfiguration.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Images/AutoResizeConfiguration.ascx" TagName="AutoResize"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagPrefix="cms" TagName="UniSelector" %>

<cms:CMSUpdatePanel runat="server" ID="updatePanel" UpdateMode="Always">
    <ContentTemplate>
        <cms:LocalizedLinkButton CssClass="form-control-text" ID="lnkAdvacedFieldSettings" runat="server" ResourceString="TemplateDesigner.ConfigureSettings" EnableViewState="false" />
        <asp:PlaceHolder ID="plcAdvancedFieldSettings" runat="server">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel"
                            Visible="false" ResourceString="dialogs.config.wrongformat" AssociatedControlID="lblContentTab" />
                    </div>
                </div>
                <cms:LocalizedHeading Level="4" ID="lblContentTab" runat="server" EnableViewState="false" ResourceString="dialogs.config.contenttab" />
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayContentTab" runat="server" EnableViewState="false"
                            DisplayColon="true" ResourceString="dialogs.config.displaytab" AssociatedControlID="chkDisplayContentTab" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkDisplayContentTab" runat="server" Checked="true" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblContentSite" runat="server" EnableViewState="false" DisplayColon="true"
                            ResourceString="dialogs.config.availablesites" AssociatedControlID="siteSelectorContent" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SiteSelector ID="siteSelectorContent" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblContentStartPath" runat="server" EnableViewState="false"
                            DisplayColon="true" ResourceString="dialogs.config.startingpath" AssociatedControlID="selectPathElem" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SelectPath ID="selectPathElem" runat="server" IsLiveSite="false" SinglePathMode="true" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblContentUseRelativeUrl" runat="server" EnableViewState="false"
                            DisplayColon="true" ResourceString="dialogs.config.userelativeurl" AssociatedControlID="chkUseRelativeUrl" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkUseRelativeUrl" runat="server" Checked="true" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcMedia" runat="server">
                    <cms:LocalizedHeading Level="4" ID="lblMediaTab" runat="server" EnableViewState="false" ResourceString="dialogs.config.mediatab" />
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayMediaTab" runat="server" EnableViewState="false"
                                DisplayColon="true" ResourceString="dialogs.config.displaytab" AssociatedControlID="chkDisplayMediaTab" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkDisplayMediaTab" runat="server" Checked="true" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblMediaSite" runat="server" EnableViewState="false" DisplayColon="true"
                                ResourceString="dialogs.config.availablesites" AssociatedControlID="siteSelectorMedia" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:SiteSelector ID="siteSelectorMedia" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblMediaSiteLibraries" runat="server" EnableViewState="false"
                                DisplayColon="true" ResourceString="dialogs.config.sitelibraries" AssociatedControlID="drpSiteLibraries" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:UniSelector runat="server" ID="drpSiteLibraries" SelectionMode="SingleDropDownList" ReturnColumnName="LibraryName" ObjectType="media.library" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblMediaStartPath" runat="server" EnableViewState="false"
                                DisplayColon="true" ResourceString="dialogs.config.startingpath" AssociatedControlID="txtMediaStartPath" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtMediaStartPath" runat="server" />
                            <div class="explanation-text">
                                <cms:LocalizedLabel ID="lblMediaStartPathExample" runat="server" EnableViewState="false" ResourceString="dialogs.config.mediaexample" />
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>
                <cms:LocalizedHeading Level="4" ID="lblOtherTabs" runat="server" EnableViewState="false" ResourceString="dialogs.config.othertabs" />
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayAttachments" runat="server" EnableViewState="false"
                            DisplayColon="true" ResourceString="dialogs.config.displayattachments" AssociatedControlID="chkDisplayAttachments" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkDisplayAttachments" runat="server" Checked="true" />
                    </div>
                </div>
                <asp:PlaceHolder runat="server" ID="plcDisplayEmail">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayEmail" runat="server" EnableViewState="false" DisplayColon="true"
                                ResourceString="dialogs.config.displayemail" AssociatedControlID="chkDisplayEmail" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkDisplayEmail" runat="server" Checked="true" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcDisplayAnchor">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayAnchor" runat="server" EnableViewState="false"
                                DisplayColon="true" ResourceString="dialogs.config.displayanchor" AssociatedControlID="chkDisplayAnchor" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkDisplayAnchor" runat="server" Checked="true" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcDisplayWeb">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayWeb" runat="server" EnableViewState="false" DisplayColon="true"
                                ResourceString="dialogs.config.displayweb" AssociatedControlID="chkDisplayWeb" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkDisplayWeb" runat="server" Checked="true" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
            <asp:PlaceHolder ID="plcAutoResize" runat="server">
                <cms:LocalizedHeading Level="4" ID="lblAutoresize" runat="server" EnableViewState="false" ResourceString="dialogs.config.autoresize" />
                <cms:AutoResize ID="elemAutoResize" runat="server" />
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    </ContentTemplate>
</cms:CMSUpdatePanel>
