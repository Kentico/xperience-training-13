<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Blogs_Controls_NewBlog"  Codebehind="NewBlog.ascx.cs" %>
<div class="new-blog">
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false"
        Visible="false" />
    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />
    <cms:CMSRequiredFieldValidator ID="rfvName" runat="server" CssClass="ErrorLabel" ControlToValidate="txtName"
        Display="Static" ValidationGroup="NewBlog" EnableViewState="false" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblName" runat="server" AssociatedControlID="txtName" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtName" runat="server" MaxLength="100" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblDescription" runat="server" AssociatedControlID="txtDescription"
                    EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextArea ID="txtDescription" runat="server" Rows="4" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group form-group-submit">
            <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" ValidationGroup="NewBlog"
                EnableViewState="false" />
        </div>
    </div>
</div>
