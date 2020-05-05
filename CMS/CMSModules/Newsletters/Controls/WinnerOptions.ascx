<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="WinnerOptions.ascx.cs"
    Inherits="CMSModules_Newsletters_Controls_WinnerOptions" %>
<cms:CMSUpdatePanel runat="server" ID="pnlU">
    <ContentTemplate>
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="newsletterissue_send.grpwinnerselection"></cms:LocalizedHeading>
        <asp:Panel runat="server" ID="pnlG">
            <div class="form-horizontal">
                <cms:LocalizedLabel runat="server" ID="lblWinnerAccordingTo" ResourceString="newsletterissue_send.winneraccordingto"
                    EnableViewState="false" />
                <div class="radio-list-vertical">
                    <cms:CMSRadioButton ID="radManually" runat="server" GroupName="Send" AutoPostBack="true"
                        ResourceString="newsletterissue_send.manually" Checked="true" />
                    <cms:CMSRadioButton ID="radOpen" runat="server" GroupName="Send" AutoPostBack="true"
                        ResourceString="newsletterissue_send.openrate" Checked="false" />
                    <cms:CMSRadioButton ID="radClicks" runat="server" GroupName="Send" AutoPostBack="true"
                        ResourceString="newsletterissue_send.totalclicks" Checked="false" />
                    <div class="selector-subitem">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel runat="server" ID="lblSelectWinner" ResourceString="newsletterissue_send.selectwinner"
                                EnableViewState="false" CssClass="control-label form-control-text" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtInterval" Text="60" MaxLength="5" AutoPostBack="true" CssClass="input-width-20" />
                            <cms:CMSDropDownList runat="server" ID="drpInterval" CssClass="ExtraSmallDropDown input-width-60" AutoPostBack="true" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
