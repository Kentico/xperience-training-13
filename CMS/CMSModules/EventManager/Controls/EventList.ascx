<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/EventManager/Controls/EventList.ascx.cs"
    Inherits="CMSModules_EventManager_Controls_EventList" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblEventScopt" AssociatedControlID="drpEventScope" runat="server"
                ResourceString="eventmanager.eventscope" EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:CMSDropDownList ID="drpEventScope" runat="server" CssClass="DropDownFieldSmall" />
        </div>
    </div>
    <div class="form-group form-group-buttons">
        <div class="filter-form-buttons-cell-wide">
            <cms:CMSButton ID="btnFilter" runat="server" ButtonStyle="Primary" EnableViewState="false" />
        </div>
    </div>
</div>
<cms:UniGrid runat="server" ID="gridElem" OrderBy="EventDate DESC" DelayedReload="true" IsLiveSite="false" />