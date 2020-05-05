<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Blogs_Controls_BlogCommentEdit"
     Codebehind="BlogCommentEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" 
    TagPrefix="cms" %>

<asp:Panel ID="pnlInfo" runat="server">
    <asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false"
        Visible="false" />
    <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />
</asp:Panel>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell BlogCommentName <%=LiveSiteCss%>">
            <asp:Label CssClass="control-label" ID="lblName" runat="server" AssociatedControlID="txtName" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell control-group-inline">
            <cms:CMSTextBox ID="txtName" runat="server" MaxLength="200"
                EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                Display="Dynamic" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell BlogCommentEmail <%=LiveSiteCss%>">
            <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" EnableViewState="false" ResourceString="general.email"
                DisplayColon="true" AssociatedControlID="txtEmail" />
        </div>
        <div class="editing-form-value-cell">
            <cms:EmailInput ID="txtEmail" runat="server" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell BlogCommentUrl <%=LiveSiteCss%>">
            <asp:Label CssClass="control-label" ID="lblUrl" runat="server" AssociatedControlID="txtUrl" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <asp:Panel ID="pnlUrl" runat="server" DefaultButton="btnOk">
                <cms:CMSTextBox ID="txtUrl" runat="server" MaxLength="450"
                    EnableViewState="false" />
            </asp:Panel>
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell BlogCommentComments <%=LiveSiteCss%>">
            <asp:Label CssClass="control-label" ID="lblComments" runat="server" AssociatedControlID="txtComments" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell control-group-inline">
            <cms:CMSTextArea ID="txtComments" runat="server" Rows="4" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvComments" runat="server" ControlToValidate="txtComments"
                Display="Dynamic" EnableViewState="false" />
        </div>
    </div>
    <%-- Advanced mode --%>
    <asp:PlaceHolder ID="plcAdvancedMode" runat="server" Visible="false">
        <%-- Comment inserted --%>
        <div class="form-group">
            <div class="BlogCommentInserted editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblInserted" runat="server" EnableViewState="false" />
            </div>
            <asp:Panel CssClass="editing-form-value-cell" ID="pnlInserted" runat="server" DefaultButton="btnOk">
                <asp:Label CssClass="form-control-text" ID="lblInsertedDate" runat="server" />
            </asp:Panel>
        </div>
        <%-- Comment approved --%>
        <div class="form-group">
            <div class="BlogCommentApproved editing-form-value-cell editing-form-value-cell-offset">
                <asp:Panel ID="pnlApproved" runat="server" DefaultButton="btnOk">
                    <cms:CMSCheckBox ID="chkApproved" runat="server" EnableViewState="false" />
                </asp:Panel>
            </div>
        </div>
        <%-- Comment is spam --%>
        <div class="form-group">
            <div class="BlogCommentIsSpam editing-form-label-cell editing-form-value-cell-offset">
                <asp:Panel ID="pnlSpam" runat="server" DefaultButton="btnOk">
                    <cms:CMSCheckBox ID="chkSpam" runat="server" EnableViewState="false" />
                </asp:Panel>
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcChkSubscribe" runat="server">
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:CMSCheckBox ID="chkSubscribe" runat="server" CssClass="CheckBoxMovedLeft"
                    EnableViewState="false" />
                <cms:LocalizedLabel CssClass="control-label" ID="lblSubscribe" AssociatedControlID="chkSubscribe" runat="server" EnableViewState="false" ResourceString="Blog.CommentEdit.Subscribe" />

            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcButtons" runat="server" Visible="True">
        <div class="form-group form-group-submit">
            <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOk_Click"
                EnableViewState="false" />
        </div>
    </asp:PlaceHolder>
</div>
