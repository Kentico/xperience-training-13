<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_Edit_SearchFields"
    CodeBehind="SearchFields.ascx.cs" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/Edit/ClassFields.ascx" TagName="ClassFields"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagPrefix="cms"
    TagName="SmartTip" %>

<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <h4>
            <cms:LocalizedLabel runat="server" ID="lblGeneral" ResourceString="general.general" DisplayColon="false"></cms:LocalizedLabel>
        </h4>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSearchEnabled" ResourceString="search.isenabled"
                    DisplayColon="true" AssociatedControlID="chkSearchEnabled" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkSearchEnabled" runat="server" AutoPostBack="true" OnCheckedChanged="chkSearchEnabled_CheckedChanged" />
            </div>
        </div>
        <asp:Panel ID="pnlSearchFields" runat="server">
            <asp:PlaceHolder ID="plcAdvancedMode" runat="server">
                <h4>
                    <cms:LocalizedLabel runat="server" ID="lblMappingResults" ResourceString="srch.fields.mappingresults" DisplayColon="false"></cms:LocalizedLabel>
                </h4>
                <cms:SmartTip ID="smarttipSearchResults" runat="server" EnableViewState="false" ExpandedHeader="{$srch.pageresults.smarttip.header$}" CollapsedHeader="{$srch.pageresults.smarttip.header$}" />
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTitleField" ResourceString="srch.titlefield"
                            DisplayColon="true" AssociatedControlID="drpTitleField" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSDropDownList ID="drpTitleField" runat="server" EnableViewState="true" CssClass="DropDownField" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblContentField" ResourceString="srch.contentfield"
                            DisplayColon="true" AssociatedControlID="drpContentField" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSDropDownList ID="drpContentField" runat="server" EnableViewState="true" CssClass="DropDownField" />
                    </div>
                </div>
                <asp:PlaceHolder runat="server" ID="plcImage">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblImageField" ResourceString="srch.imagefield"
                                DisplayColon="true" AssociatedControlID="drpImageField" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpImageField" runat="server" EnableViewState="true" CssClass="DropDownField" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDateField" ResourceString="srch.datefield"
                            DisplayColon="true" AssociatedControlID="drpDateField" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSDropDownList ID="drpDateField" runat="server" EnableViewState="true" CssClass="DropDownField" />
                    </div>
                </div>
                <div runat="server" id="pnlIndent" visible="false" class="SearchFieldsIndentPanel">
                </div>
            </asp:PlaceHolder>
        <cms:CMSUpdatePanel runat="server" >
            <ContentTemplate>
                <asp:PlaceHolder ID="plcPageIndexingOptions" runat="server" Visible="false">
                    <h4>
                        <cms:LocalizedLabel runat="server" ID="lblPageDataSource" ResourceString="srch.fields.pagedatasource" DisplayColon="false"></cms:LocalizedLabel>
                    </h4>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDataSource" ResourceString="srch.fields.datasource"
                                DisplayColon="true" AssociatedControlID="rblPageDataSource" />
                        </div>
                        <div class="editing-form-value-cell">
                            <div class="settings-group-inline keep-white-space-fixed">
                                <cms:CMSRadioButtonList runat="server" ID="rblPageDataSource" AutoPostBack="true" OnSelectedIndexChanged="rblPageDataSource_SelectedIndexChanged"
                                    UseResourceStrings="true">
                                    <asp:ListItem Text="srch.pagedatasource.htmloutput" />
                                    <asp:ListItem Text="srch.pagedatasource.contentfields" />
                                    <asp:ListItem Text="srch.pagedatasource.both" />
                                </cms:CMSRadioButtonList>
                                <span class="info-icon">
                                    <asp:Label runat="server" ID="spanScreenReaderDataSource" CssClass="sr-only"></asp:Label>
                                    <cms:CMSIcon runat="server" ID="iconHelpDataSource" EnableViewState="false" class="icon-question-circle" aria-hidden="true"></cms:CMSIcon>
                                </span>
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <cms:ClassFields ID="ClassFields" runat="server" Visible="true" />
                   </ContentTemplate>
        </cms:CMSUpdatePanel>
        </asp:Panel>
        <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOK_Click" />
    </div>
</asp:Panel>
