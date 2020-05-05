<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_CheckoutProcess"
     Codebehind="CheckoutProcess.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:HiddenField ID="hdnCheckoutProcessXml" runat="server" EnableViewState="false" />
<div class="CheckoutProcess">
    <asp:PlaceHolder ID="plcList" runat="server">
        <asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
            <cms:UniGrid ID="ugSteps" runat="server" ShowObjectMenu="false" PageSize="All">
                <GridActions>
                    <ug:Action Name="edit" CommandArgument="Name" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="delete" CommandArgument="Name" Caption="$General.Delete$"
                        FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$CheckoutProcess.ConfirmDefaultProcess$" />
                    <ug:Action Name="up" CommandArgument="Name" Caption="$CheckoutProcess.btnMoveUpToolTip$"
                        FontIconClass="icon-chevron-up" />
                    <ug:Action Name="down" CommandArgument="Name" Caption="$CheckoutProcess.btnMoveDownToolTip$"
                        FontIconClass="icon-chevron-down" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="##All##" Caption="$CheckoutProcess.Order$" Wrap="false" ExternalSourceName="StepOrder">
                    </ug:Column>
                    <ug:Column Source="Caption" Caption="$CheckoutProcess.Caption$" Wrap="false">
                    </ug:Column>
                    <ug:Column Source="CMSDeskCustomer" Name="CMSDeskCustomer" Caption="$CheckoutProcess.ShowInCMSDeskCustomer$" Wrap="false" ExternalSourceName="#yesno">
                    </ug:Column>
                    <ug:Column Source="CMSDeskOrder" Name="CMSDeskOrder" Caption="$CheckoutProcess.ShowInCMSDeskOrder$" Wrap="false" ExternalSourceName="#yesno">
                    </ug:Column>
                    <ug:Column Source="CMSDeskOrderItems" Name="CMSDeskOrderItems" Caption="$CheckoutProcess.ShowInCMSDeskOrderItems$" Wrap="false" ExternalSourceName="#yesno">
                    </ug:Column>
                    <ug:Column Wrap="false" CssClass="main-column-100" />
                </GridColumns>
                <GridOptions DisplayFilter="false" ShowSelection="false" SelectionColumn="Name" />
            </cms:UniGrid>
        </asp:Panel>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcEdit" runat="server">
        <div class="SimpleHeader">
            <cms:Breadcrumbs ID="breadcrumbs" runat="server" ChangeTargetFrame="False" />
        </div>
        <div class="cms-edit-menu">
            <cms:HeaderActions ID="headerActions" runat="server" />
        </div>
        <asp:Panel ID="pnlEditContent" runat="server" CssClass="PageContent">
            <cms:MessagesPlaceHolder ID="plcMessNew" runat="server" IsLiveSite="false" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ResourceString="CheckoutProcess.lblStepCaption" ID="lblStepCaption" runat="server" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:LocalizableTextBox ID="txtStepCaption" runat="server" />
                        <cms:CMSRequiredFieldValidator ID="rfvStepCaption" runat="server" ControlToValidate="txtStepCaption:cntrlContainer:textbox"
                            ValidationGroup="CheckoutProcess" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ResourceString="general.codename" DisplayColon="true" ID="lblStepName" runat="server" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtStepName" runat="server" />
                        <cms:CMSRequiredFieldValidator ID="rfvStepName" runat="server" ControlToValidate="txtStepName"
                            ValidationGroup="CheckoutProcess" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ResourceString="CheckoutProcess.lblStepImageUrl" ID="lblStepImageUrl" runat="server" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtStepImageUrl" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ResourceString="CheckoutProcess.lblStepControlPath" ID="lblStepControlPath" runat="server" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtStepControlPath" runat="server" />
                        <cms:CMSRequiredFieldValidator ID="rfvStepControlPath" runat="server" ControlToValidate="txtStepControlPath"
                            ValidationGroup="CheckoutProcess" Display="Dynamic" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcDefaultTypes" runat="server">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ResourceString="CheckoutProcess.lblCMSDeskCustomer" ID="lblCMSDeskCustomer" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkCMSDeskCustomer" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ResourceString="CheckoutProcess.lblCMSDeskOrder" ID="lblCMSDeskOrder" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkCMSDeskOrder" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ResourceString="CheckoutProcess.lblCMSDeskOrderItems" ID="lblCMSDeskOrderItems" runat="server" EnableViewState="false" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkCMSDeskOrderItems" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click" ValidationGroup="CheckoutProcess" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </asp:PlaceHolder>
</div>