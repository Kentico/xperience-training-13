<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_Edit_SearchFields"
     Codebehind="SearchFields.ascx.cs" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/Edit/ClassFields.ascx" TagName="ClassFields"
    TagPrefix="cms" %>

<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
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
            <cms:ClassFields ID="ClassFields" runat="server" Visible="true" />
        </asp:Panel>
        <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOK_Click" />
    </div>
</asp:Panel>
