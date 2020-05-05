<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_Controls_StagingTasksFilter" CodeBehind="StagingTasksFilter.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TimeSimpleFilter.ascx" TagName="TimeSimpleFilter" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="UserSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/EnumSelector.ascx" TagName="EnumSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnFilter">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblProperties" runat="server" ResourceString="staging.TaskType"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:EnumSelector ID="taskTypeSelector" runat="server" AssemblyName="CMS.DataEngine" TypeName="CMS.DataEngine.TaskTypeEnum" EnableViewState="true"/>
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblUserName" runat="server" ResourceString="User"
                    DisplayColon="true" EnableViewState="false"/>
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:UserSelector ID="userSelector" runat="server" Visible="true" SiteID="-1" SelectionMode="SingleDropDownList" AllowEmpty="true" AllowAll="true" EnableViewState="true"/>
            </div>
        </div>
        <asp:Panel runat="server" ID="stagingTaskGroupPanel">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblTaskGroup" runat="server" ResourceString="staging.TaskGroup"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:UniSelector ID="stagingTaskGroupSelector" runat="server" Visible="true" SelectionMode="SingleDropDownList" AllowEmpty="true" AllowAll="true" EnableViewState="true"/>
                </div>
            </div>
        </asp:Panel>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSearchTaskTitle" runat="server" EnableViewState="false"
                    ResourceString="staging.TaskTitle" DisplayColon="true" />
            </div>
            <cms:TextSimpleFilter ID="fltTaskTitle" runat="server" Column="TaskTitle" />
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTimeBetween" runat="server" ResourceString="staging.tasktime"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:TimeSimpleFilter ID="fltTimeBetween" runat="server" />
            </div>
        </div>
        <div class="form-group-buttons">
            <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" ResourceString="general.reset" />
            <cms:LocalizedButton ID="btnFilter" runat="server" ButtonStyle="Primary" EnableViewState="false" ResourceString="general.search" />
        </div>
    </div>
</asp:Panel>
