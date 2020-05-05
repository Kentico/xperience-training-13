<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PersonalDataManagement.aspx.cs" Inherits="CMSModules_DataProtection_Pages_PersonalDataManagement"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagPrefix="cms"
    TagName="SmartTip" %>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="plcContent">
    <div class="cms-bootstrap padding-100">
        <cms:SmartTip runat="server" ID="tipDataSubjectRights" />

        <cms:MessagesPlaceHolder ID="plcMessages" runat="server" IsLiveSite="false"></cms:MessagesPlaceHolder>

        <div class="form-horizontal form-filter">
            <asp:PlaceHolder ID="plcDataSubjectIdentifiersFilter" runat="server" />

            <div class="form-group form-group-buttons">
                <div class="filter-form-buttons-cell-wide">
                    <cms:LocalizedButton ID="btnSearch" runat="server" ResourceString="dataprotection.app.searchbutton" OnClick="btnSearch_Click" EnableViewState="false" />
                </div>
            </div>
        </div>

        <div class="form-horizontal">
            <asp:Panel runat="server" ID="pnlNoResults" Visible="false" EnableViewState="false">
                <div class="editing-form-label-cell label-full-width">
                    <cms:LocalizedLabel ID="lblNoData" runat="server" ResourceString="dataprotection.app.nopersonaldata" EnableViewState="false" />
                </div>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlResults" Visible="false" EnableViewState="false">
                <div class="editing-form-label-cell label-full-width">
                    <cms:LocalizedHeading runat="server" Level="4" ResourceString="dataprotection.app.personaldata" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell textarea-full-width">
                    <div class="form-group">
                        <cms:LocalizedButton ID="btnDeleteData" runat="server" ResourceString="dataprotection.app.deletedatabutton" EnableViewState="false" Visible="false" ButtonStyle="Default" />
                        <cms:LocalizedCopyToClipboardButton ID="btnCopy" runat="server" CopySourceControlID="txtOutput" EnableViewState="false" ButtonStyle="Default" />
                    </div>
                    <div class="form-group">
                        <cms:CMSTextArea ID="txtOutput" runat="server" ReadOnly="true" Rows="33" TextMode="MultiLine" EnableViewState="false" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
