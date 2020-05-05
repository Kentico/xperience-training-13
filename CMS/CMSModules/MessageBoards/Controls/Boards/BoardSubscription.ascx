<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_Boards_BoardSubscription"
     Codebehind="BoardSubscription.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="UserSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>

<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Always">
    <ContentTemplate>
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <div class="form-horizontal">

            <cms:CMSRadioButton ID="radAnonymousSubscription" runat="server" Visible="true" AutoPostBack="true" GroupName="subscription" />
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label ID="lblEmailAnonymous" runat="server" EnableViewState="false" CssClass="control-label" AssociatedControlID="txtEmailAnonymous" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EmailInput ID="txtEmailAnonymous" runat="server" />
                    <cms:CMSRequiredFieldValidator ID="rfvEmailAnonymous" runat="server" Display="Dynamic" Enabled="false" EnableViewState="false" ValidationGroup="Email"/>
                </div>
            </div>
            <cms:CMSRadioButton ID="radRegisteredSubscription" runat="server" Visible="true" AutoPostBack="true" GroupName="subscription" />
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label ID="lblUserRegistered" runat="server" EnableViewState="false" CssClass="control-label" AssociatedControlID="userSelector" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:UserSelector ID="userSelector" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label ID="lblEmailRegistered" runat="server" EnableViewState="false" CssClass="control-label" AssociatedControlID="txtEmailRegistered" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EmailInput ID="txtEmailRegistered" runat="server" />
                    <cms:CMSRequiredFieldValidator ID="rfvEmailRegistered" runat="server" Display="Dynamic" Enabled="false" EnableViewState="false" ValidationGroup="Email" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click"
                        EnableViewState="false" ValidationGroup="Email" ResourceString="general.ok" />
                </div>
            </div>
        </div>
        <cms:CMSCheckBox runat="server" ID="chkSendConfirmationEmail" ResourceString="forums.forumsubscription.sendemail" />
        <asp:Literal ID="ltlScripMail" runat="server" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<script type="text/javascript">
    //<![CDATA[
    function SetTextForUser(mText, mId, mText2, mId2) {
        var elem2 = document.getElementById(mId2);
        var elem1 = document.getElementById(mId);

        if (elem1 != null) {
            elem1.value = mText;
        }

        if (elem2 != null) {
            elem2.value = mText2;
        }

        GetUsersEmail(mText2);

        return false;
    }
    //]]>             
</script>
