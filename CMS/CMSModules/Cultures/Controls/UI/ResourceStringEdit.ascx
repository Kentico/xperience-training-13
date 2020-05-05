<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ResourceStringEdit.ascx.cs"
    Inherits="CMSModules_Cultures_Controls_UI_ResourceStringEdit" %>

<cms:LocalizedHeading ID="lblGeneral" Level="4" runat="server" ResourceString="general.general" EnableViewState="false" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="headStringKey" runat="server" DisplayColon="true" ResourceString="culture.key"
                EnableViewState="false" AssociatedControlID="txtStringKey" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtStringKey" runat="server" MaxLength="200" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcIsCustom" runat="server" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblIsCustom" runat="server" DisplayColon="true" ResourceString="culture.customstring"
                    EnableViewState="false" AssociatedControlID="chkIsCustom" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkIsCustom" runat="server" Checked="true" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>
<cms:LocalizedHeading ID="headTranslations" runat="server" Level="4" ResourceString="localizable.translations" EnableViewState="false" />
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" ShowProgress="true">
    <ContentTemplate>
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="rbCultures" ID="lblTranlsations" runat="server" EnableViewState="false" CssClass="control-label"
                        DisplayColon="true" ResourceString="localizable.showtranslationsof" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="radio-list-vertical">
                        <cms:CMSRadioButtonList RepeatDirection="Horizontal" ID="rbCultures" UseResourceStrings="true" runat="server"
                            AutoPostBack="true" OnSelectedIndexChanged="rbCultures_SelectedIndexChanged">
                            <asp:ListItem Value="sitecultures" Text="culture.sitecultures" Selected="True" />
                            <asp:ListItem Value="uicultures" Text="culture.uicultures" />
                            <asp:ListItem Value="allcultures" Text="culture.allcultures" />
                        </cms:CMSRadioButtonList>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel runat="server" DisplayColon="True" AssociatedControlID="txtFilter" ResourceString="general.language" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtFilter" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:LocalizedButton runat="server" ID="btnFilter" ButtonStyle="Default" OnClick="btnFilter_Click" ResourceString="general.filter" EnableViewState="false" />
                </div>
            </div>
        </div>
        <asp:Table ID="tblGrid" runat="server" GridLines="None" CssClass="table table-hover" CellSpacing="-1" CellPadding="-1">
            <asp:TableHeaderRow CssClass="unigrid-head" TableSection="TableHeader" ID="tblHeaderRow" runat="server" HorizontalAlign="Left">
                <asp:TableHeaderCell HorizontalAlign="Left" Scope="Column" ID="tblHeaderCellFilter"
                    runat="server">
                    <cms:LocalizedLabel ID="lblLanguage" runat="server" EnableViewState="false" ResourceString="general.language" />

                </asp:TableHeaderCell>
                <asp:TableHeaderCell HorizontalAlign="Center" Scope="Column" ID="tblHeaderCellLabel"
                    runat="server" EnableViewState="false" Text="{$transman.Translated$}" />
            </asp:TableHeaderRow>
        </asp:Table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="rbCultures" EventName="SelectedIndexChanged" />
    </Triggers>
</cms:CMSUpdatePanel>
<cms:FormSubmitButton ID="btnOk" ButtonStyle="Primary" runat="server" OnClick="btnOk_Click" />
