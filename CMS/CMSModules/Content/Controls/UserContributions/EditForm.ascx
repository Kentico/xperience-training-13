<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_UserContributions_EditForm"
     Codebehind="EditForm.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlForm" CssClass="EditForm">
    <asp:Panel runat="server" ID="pnlTitle" CssClass="PageHeader">
        <cms:PageTitle ID="titleElem" runat="server" />
    </asp:Panel>
    <cms:CMSDocumentManager ID="docMan" runat="server" />
    <asp:Panel runat="server" ID="pnlSelectClass" CssClass="PageContent">
        <strong>
            <asp:Label ID="lblInfo" runat="server" CssClass="ContentLabel" EnableViewState="false" />
        </strong>
        <br />
        <asp:Label ID="lblError" runat="server" CssClass="ContentError" ForeColor="Red" EnableViewState="false" />
        <br />
        <cms:UniGrid ID="gridClass" runat="server" GridName="~/CMSModules/Content/Controls/UserContributions/EditForm.xml"
            ShowActionsMenu="false" ShowObjectMenu="false" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlNewCulture" CssClass="PageContent">
        <strong>
            <asp:Label ID="lblNewCultureInfo" runat="server" CssClass="ContentLabel" /></strong><br />
        <br />
        <table>
            <tr>
                <td>
                    <cms:CMSRadioButton ID="radEmpty" runat="server" GroupName="NewVersion" Checked="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:CMSRadioButton ID="radCopy" runat="server" GroupName="NewVersion" />
                </td>
            </tr>
            <tr id="divCultures" style="<%=(radCopy.Checked ? "display: block;": "display: none;")%>">
                <td>
                    <asp:Panel runat="server" ID="pnlCultures" CssClass="SoftSelectionBorder">
                        <cms:CMSListBox runat="server" ID="lstCultures" DataTextField="DocumentCulture" DataValueField="DocumentID"
                            CssClass="ContentListBoxLow" Rows="7" />
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <cms:CMSButton ID="btnOk" runat="server" OnClick="btnOK_Click" ButtonStyle="Primary" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlEdit">
        <asp:Panel runat="server" ID="pnlMenu" CssClass="ContentEditMenu cms-bootstrap-js">
            <cms:editmenu ID="menuElem" runat="server" ShowProperties="false" RenderScript="false"
                ShowDelete="true" ShowSpellCheck="true" ShowCreateAnother="false" />
        </asp:Panel>
        <div class="clear" ></div>
        <cms:CMSDocumentPanel ID="pnlDoc" runat="server" />
        <div id="CKToolbarUC" style="clear: both;">
        </div>
        <asp:Panel runat="server" ID="pnlContent" CssClass="PageContent">
            <cms:CMSForm runat="server" ID="formElem" CssClass="UserContributionForm"
                HtmlAreaToolbarLocation="Out:CKToolbarUC" ShowOkButton="false" IsLiveSite="true"/>
        </asp:Panel>
        <cms:CMSButton ID="btnDelete" runat="server" EnableViewState="true" ButtonStyle="Default" OnClick="btnDelete_Click" />
        <cms:CMSButton ID="btnRefresh" runat="server" EnableViewState="true" ButtonStyle="Default" UseSubmitBehavior="false" OnClick="btnRefresh_Click" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDelete" CssClass="PageContent">
        <strong>
            <asp:Label ID="lblQuestion" runat="server" CssClass="ContentLabel" EnableViewState="false" /></strong><br />
        <asp:Label ID="lblDocuments" runat="server" CssClass="ContentLabel" EnableViewState="false" />
        <br />
        <asp:PlaceHolder ID="plcCheck" runat="server">
            <cms:CMSCheckBox ID="chkDestroy" runat="server" CssClass="ContentCheckbox" /><br />
            <cms:CMSCheckBox ID="chkAllCultures" runat="server" CssClass="ContentCheckbox" /><br />
            <br />
        </asp:PlaceHolder>
        <cms:CMSButton ID="btnYes" runat="server" ButtonStyle="Default" OnClick="btnYes_Click" />
        <cms:CMSButton ID="btnNo" runat="server" ButtonStyle="Default" OnClick="btnNo_Click" />
    </asp:Panel>
    <asp:Panel ID="pnlInfo" runat="server" CssClass="PageContent">
        <asp:Label ID="lblFormInfo" runat="server" EnableViewState="false" CssClass="ContentLabel" />
    </asp:Panel>
</asp:Panel>
