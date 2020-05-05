<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_RecycleBin_Controls_RecycleBin"
     Codebehind="RecycleBin.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>

<asp:Panel runat="server" ID="pnlLog" Visible="false">
    <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="RecycleBin" />
</asp:Panel>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div>
            <cms:UniGrid ID="ugRecycleBin" runat="server" GridName="~/CMSModules/RecycleBin/Controls/RecycleBin.xml"
                IsLiveSite="false" HideControlForZeroRows="true" />
            <asp:Panel ID="pnlFooter" runat="server" CssClass="form-horizontal mass-action">
                <div class="form-group">
                    <div class="mass-action-value-cell">
                        <cms:CMSDropDownList ID="drpWhat" runat="server" />
                        <cms:CMSDropDownList ID="drpAction" runat="server" />
                        <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary"
                            EnableViewState="false" OnClick="btnOk_OnClick" />
                        <asp:Label ID="lblValidation" runat="server" CssClass="InfoLabel" EnableViewState="false"
                            Style="display: none;" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
