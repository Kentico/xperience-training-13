<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Controls_GroupInvite"  Codebehind="GroupInvite.ascx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="SelectUser" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Groups/FormControls/CommunityGroupSelector.ascx" TagName="GroupSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>


<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <asp:Label runat="server" ID="lblInfo" EnableViewState="false" CssClass="form-control-text"
                Visible="false" />
    </div>
    <asp:PlaceHolder ID="plcUserType" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblUserType" runat="server" EnableViewState="false" ResourceString="groupinvitation.invitationtype"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell radio-list-vertical">
                <cms:CMSRadioButton AutoPostBack="true" ID="radSiteMember" GroupName="grpUserType"
                    ResourceString="invitation.existingsitemember" runat="server" Checked="true" />
                <cms:CMSRadioButton AutoPostBack="true" ID="radNewUser" GroupName="grpUserType"
                    ResourceString="invitation.viaemail" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcEmail" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" EnableViewState="false" ResourceString="general.email"
                    DisplayColon="true" AssociatedControlID="txtEmail" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtEmail" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" 
                    Display="dynamic" EnableViewState="false" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcUserSelector" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblUser" runat="server" EnableViewState="false" ResourceString="general.username"
                    DisplayColon="true" AssociatedControlID="userSelector" />
            </div>
            <div class="editing-form-value-cell">
                <cms:SelectUser ID="userSelector" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcGroupSelector" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblGroups" runat="server" EnableViewState="false" ResourceString="general.groups"
                    DisplayColon="true" AssociatedControlID="groupSelector" />
            </div>
            <div class="editing-form-value-cell">
                <cms:GroupSelector runat="server" ID="groupSelector" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblComment" runat="server" ResourceString="general.comment"
                DisplayColon="true" EnableViewState="false" AssociatedControlID="txtComment"/>
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextArea ID="txtComment" runat="server" Rows="7"
                EnableViewState="false" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcButtons" runat="server">
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:CMSButton runat="server" ButtonStyle="Primary" ID="btnInvite" EnableViewState="false" />
                <cms:LocalizedButton ButtonStyle="Primary" ID="btnCancel" runat="server" ResourceString="General.cancel"
                    EnableViewState="false" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>

<script type="text/javascript">
    //<![CDATA[

    function Close() {
        CloseDialog();
    }

    function CloseAndRefresh() {
        if (wopener != null) {
            if (wopener.ReloadPage != null) {
                wopener.ReloadPage();
            }
        }
        Close();
    }
    //]]>
</script>
