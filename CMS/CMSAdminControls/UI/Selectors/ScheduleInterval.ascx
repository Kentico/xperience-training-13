<%@ Control Language="C#" AutoEventWireup="false" Inherits="CMSAdminControls_UI_Selectors_ScheduleInterval"
     Codebehind="ScheduleInterval.ascx.cs" %>

<asp:Panel runat="server" ID="pnlBody">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <%--Period--%>
    <asp:PlaceHolder runat="server" ID="pnlPeriod">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblPeriod" runat="server" EnableViewState="false" ResourceString="scheduleinterval.period"
                    AssociatedControlID="drpPeriod" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSDropDownList ID="drpPeriod" runat="server" OnSelectedIndexChanged="DrpPeriod_OnSelectedIndexChanged"
                    AutoPostBack="true" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="pnlStartTime">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblStart" runat="server" EnableViewState="false" ResourceString="scheduleinterval.start"
                        AssociatedControlID="dateTimePicker" ShowRequiredMark="true"/>
                </div>
                <div class="editing-form-value-cell">
                    <cms:DateTimePicker ID="dateTimePicker" runat="server" />
                    <cms:CMSRequiredFieldValidator ID="rfvInterval" runat="server" ControlToValidate="dateTimePicker:txtDateTime" Display="Dynamic"></cms:CMSRequiredFieldValidator>
                </div>
            </div>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
    <%--Every--%>
    <asp:PlaceHolder runat="server" ID="pnlEvery">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEvery" runat="server" EnableViewState="false" ResourceString="scheduleinterval.every"
                    AssociatedControlID="txtEvery" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtEvery" runat="server" MaxLength="5" />
                <asp:Label ID="lblEveryPeriod" CssClass="form-control-text" runat="server" />

                <cms:CMSRequiredFieldValidator ID="rfvEvery" runat="server" ControlToValidate="txtEvery"
                    Display="dynamic" EnableViewState="False" />
                <cms:CMSRangeValidator ID="rvEvery" runat="server" ControlToValidate="txtEvery" Type="integer"
                    Display="dynamic" EnableViewState="False" />
            </div>
        </div>
    </asp:PlaceHolder>
    <%--Between--%>
    <asp:PlaceHolder runat="server" ID="pnlBetween">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblBetween" runat="server" EnableViewState="false" ResourceString="scheduleinterval.between"
                    AssociatedControlID="txtFromHours" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtFromHours" CssClass="input-width-15" runat="server" MaxLength="2" />
                    <span class="form-control-text">:</span>
                    <cms:CMSTextBox ID="txtFromMinutes" CssClass="input-width-15" runat="server" MaxLength="2" />
                    <cms:LocalizedLabel ID="lblAnd" runat="server" EnableViewState="false" CssClass="form-control-text" ResourceString="scheduleinterval.and"
                        AssociatedControlID="txtToHours" />
                    <cms:CMSTextBox ID="txtToHours" CssClass="input-width-15" runat="server" MaxLength="2" />
                    <span class="form-control-text">:</span>
                    <cms:CMSTextBox ID="txtToMinutes" CssClass="input-width-15" runat="server" MaxLength="2" />

                    <cms:CMSRequiredFieldValidator ID="rfvFromHours" runat="server" ControlToValidate="txtFromHours"
                        Display="dynamic" EnableViewState="False" />
                    <cms:CMSRangeValidator ID="rvFromHours" runat="server" ControlToValidate="txtFromHours"
                        Type="integer" Display="dynamic" EnableViewState="False" />
                    <cms:CMSRequiredFieldValidator ID="rfvFromMinutes" runat="server" ControlToValidate="txtFromMinutes"
                        Display="dynamic" EnableViewState="False" />
                    <cms:CMSRangeValidator ID="rvFromMinutes" runat="server" ControlToValidate="txtFromMinutes"
                        Type="integer" Display="dynamic" EnableViewState="False" />
                    <cms:CMSRequiredFieldValidator ID="rfvToHours" runat="server" ControlToValidate="txtToHours"
                        Display="dynamic" EnableViewState="False" />
                    <cms:CMSRangeValidator ID="rvToHours" runat="server" ControlToValidate="txtToHours"
                        Type="integer" Display="dynamic" EnableViewState="False" />
                    <cms:CMSRequiredFieldValidator ID="rfvToMinutes" runat="server" ControlToValidate="txtToMinutes"
                        Display="dynamic" EnableViewState="False" />
                    <cms:CMSRangeValidator ID="rvToMinutes" runat="server" ControlToValidate="txtToMinutes"
                        Type="integer" Display="dynamic" EnableViewState="False" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <%--Days--%>
    <asp:PlaceHolder runat="server" ID="pnlDays">
        <div class="form-group scheduled-task-days">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblDays" runat="server" EnableViewState="false" ResourceString="scheduleinterval.days"
                    AssociatedControlID="chkWeek" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBoxList ID="chkWeek" runat="server" />
                <cms:CMSCheckBoxList ID="chkWeekEnd" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    <%--Month--%>
    <asp:PlaceHolder runat="server" ID="pnlMonth" Visible="false">
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <div class="control-group-inline">
                    <cms:CMSRadioButton ID="radMonthDate" runat="server" AutoPostBack="true" OnCheckedChanged="radMonthDate_CheckedChanged"
                        Checked="True" ResourceString="scheduleinterval.period.every.day" />
                    <cms:CMSDropDownList CssClass="input-width-40" ID="drpMonthDate" runat="server" />
                    <cms:LocalizedLabel ID="lblMonth1" runat="server" EnableViewState="false" ResourceString="scheduleinterval.months.ofthemonth(s)" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <div class="control-group-inline">
                    <cms:CMSRadioButton ID="radMonthSpecification" runat="server" AutoPostBack="true"
                        OnCheckedChanged="radMonthSpecification_CheckedChanged" ResourceString="scheduleinterval.months.the" />
                    <cms:CMSDropDownList CssClass="input-width-40" ID="drpMonthOrder" runat="server" />
                    <cms:CMSDropDownList CssClass="input-width-40" ID="drpMonthDay" runat="server" />
                    <cms:LocalizedLabel ID="lblMonth2" runat="server" CssClass="form-control-text" EnableViewState="false" ResourceString="scheduleinterval.months.ofthemonth(s)" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Panel>
