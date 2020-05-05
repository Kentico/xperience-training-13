<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - New newsletter"
     Codebehind="Newsletter_New.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Selectors/ScheduleInterval.ascx" TagName="ScheduleInterval"
    TagPrefix="cms" %>

<asp:Content id="cntBody" runat="server" contentplaceholderid="plcContent">
    <cms:CMSPanel CssClass="CMSNewsletter NewsletterNew" runat="server">
        <cms:UIForm runat="server" ObjectType="newsletter.newsletter" ID="NewForm" Mode="Insert"
            OnOnAfterSave="AfterSave" OnOnBeforeSave="BeforeSave" OnOnBeforeValidate="ValidateValues"
            RedirectUrlAfterCreate="">          
            <SecurityCheck Permission="configure" Resource="cms.newsletter" DisableForm="true" />
        </cms:UIForm>
        <asp:PlaceHolder ID="plcSchedule" runat="server">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:FormLabel CssClass="control-label" runat="server" EnableViewState="false" ID="fSchedule" ResourceString="Newsletter_Edit.Schedule"
                            DisplayColon="true" AssociatedControlID="chkSchedule" ToolTipResourceString="newsletter_edit.newsletterdynamicscheduledtask.description" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox CssClass="EditingFormControlNestedControl" ID="chkSchedule" runat="server"
                            Checked="true" OnCheckedChanged="chkSchedule_CheckedChanged" AutoPostBack="true" />
                    </div>
                </div>
                        <cms:ScheduleInterval ID="ScheduleInterval" runat="server" />
                    </div>
        </asp:PlaceHolder>
    </cms:CMSPanel>
</asp:Content>