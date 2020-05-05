<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_SearchDialog"
     Codebehind="SearchDialog.ascx.cs" %>

<asp:Panel ID="pnlDialog" runat="server" DefaultButton="btnSearch" CssClass="search-dialog">
    <div class="form-horizontal">
        <asp:PlaceHolder ID="plcSearchOptions" runat="server" Visible="true">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSearchFor" AssociatedControlID="txtSearchFor"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtSearchFor" MaxLength="1000" ProcessMacroSecurity="false" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcSearchMode" Visible="true">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSearchMode" AssociatedControlID="drpSearchMode"
                            DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSDropDownList runat="server" ID="drpSearchMode" CssClass="DropDownField" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <div class="form-group form-group-submit">
            <cms:LocalizedButton runat="server" ID="btnSearch" ButtonStyle="Default" OnClick="btnSearch_Click" />
        </div>
    </div>
</asp:Panel>
