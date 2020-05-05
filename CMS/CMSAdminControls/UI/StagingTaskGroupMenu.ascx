<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StagingTaskGroupMenu.ascx.cs" Inherits="CMSAdminControls_UI_StagingTaskGroupMenu" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>

<h2 class="sr-only"><%: GetString("staging.taskGroupMenu") %></h2>
<div class="navbar-inverse cms-navbar">
    <ul class="nav navbar-nav navbar-right navbar-inverse">
        <li runat="server" id="stagingTaskGroupMenuDropdown">
            <a href="#" data-toggle="dropdown" class="dropdown-toggle" title="<%: GetString("staging.stagingTaskGroupMenu") %>">
                <i aria-hidden="false" class="icon-app-staging cms-nav-icon-medium"></i><span class="sr-only"><%: GetString("staging.stagingTaskGroupMenu") %></span>
                <asp:Label runat="server" ID="lblStagingTaskGroupMenuText" CssClass="cms-nav-text"></asp:Label>
            </a>
            <ul class="dropdown-menu" role="menu">
                <li class="dropdown-menu-item">
                    <cms:LocalizedLabel id="lblStagingTaskGroupSelector" runat="server" CssClass="dropdown-menu-item-label" EnableViewState="false" ResourceString="staging.selectTaskGroup" />
                    <cms:UniSelector ID="stagingTaskGroupSelector" ObjectType="staging.taskGroup" CheckChanges="True" runat="server"
                        ReturnColumnName="taskGroupID" SelectionMode="SingleDropDownList" IsLiveSite="false" AllowEmpty="true" EnableViewState="false" />
                    <cms:LocalizedHyperlink runat="server" ID="lnkEditTaskGroup" CssClass="dropdown-menu-item-link" EnableViewState="false" Visible="true"></cms:LocalizedHyperlink>
                </li>
                <asp:PlaceHolder ID="plcCreateTaskGroup" runat="server">
                    <li class="dropdown-menu-item">
                        <cms:LocalizedLabel AssociatedControlID="inputTaskGroup" runat="server" CssClass="dropdown-menu-item-label" EnableViewState="false" ResourceString="staging.createTaskGroup" />
                        <cms:LocalizedLabel AssociatedControlID="btnCreateTaskGroup" runat="server" CssClass="src-only" EnableViewState="false" />
                        <div class="container-fluid">
                            <asp:Panel runat="server" ID="pnlCreateTaskGroup" DefaultButton="btnCreateTaskGroup" CssClass="row">
                                <div class="col-md-8">
                                    <asp:TextBox ID="inputTaskGroup" CssClass="form-control" runat="server" EnableViewState="false" />
                                </div>
                                <div class="col-md-4">
                                    <asp:Button type="button" ID="btnCreateTaskGroup" class="btn btn-primary pull-right" runat="server" EnableViewState="false" />
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlCodeNameMessage" CssClass="dropdown-menu-item-info-message" EnableViewState="false">
                                <cms:LocalizedLabel ID="lblCodeNameMessage" runat="server" EnableViewState="false" ResourceString="staging.stagingTaskGroupMenuInfoMessage" />
                            </asp:Panel>
                        </div>
                    </li>
                </asp:PlaceHolder>
            </ul>
        </li>
    </ul>
</div>
