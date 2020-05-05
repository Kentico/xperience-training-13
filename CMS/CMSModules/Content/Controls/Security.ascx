<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Security.ascx.cs" Inherits="CMSModules_Content_Controls_Security" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Roles/securityAddRoles.ascx"
    TagName="AddRoles" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/securityAddUsers.ascx"
    TagName="AddUsers" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:PlaceHolder ID="plcSecurityMessage" runat="server" Visible="false">
            <div class="form-group">
                <asp:Label ID="lblPermission" runat="server" CssClass="InfoLabel" EnableViewState="false" />
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <cms:LocalizedLabel ID="lblUsersRoles" CssClass="control-label" runat="server"
                EnableViewState="false" ResourceString="Security.UsersRoles" />
            <asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnFilter">
                <div class="control-group-inline form-group dont-check-changes">
                    <cms:CMSTextBox ID="txtFilter" runat="server" />
                    <cms:LocalizedButton ID="btnFilter" runat="server" ButtonStyle="Default" ResourceString="general.search" OnClientClick="if(!CheckChanges()) {return false;} " />
                </div>
            </asp:Panel>
            <div class="form-group">
                <div class="control-group-inline">
                    <cms:CMSListBox ID="lstOperators" runat="server" AutoPostBack="True" Rows="12" CssClass="PermissionsListBox dont-check-changes"
                        OnSelectedIndexChanged="lstOperators_SelectedIndexChanged" />
                    <div class="btns-vertical">
                        <cms:AddUsers ID="addUsers" runat="server" />
                        <cms:AddRoles ID="addRoles" runat="server" />
                        <cms:LocalizedButton ID="btnRemoveOperator" runat="server" ButtonStyle="Default"
                            ResourceString="general.remove" OnClick="btnRemoveOperator_Click" />
                    </div>
                </div>
            </div>
            <div class="Clear"></div>
            <div>
                <cms:MessagesPlaceHolder ID="plcMess" runat="server" LiveSiteOnly="true" />
                <cms:LocalizedLabel ID="lblAccessRights" CssClass="control-label" runat="server"
                    EnableViewState="false" ResourceString="Security.AccessRights" />
                <asp:Panel ID="pnlAccessRights" runat="server">
                    <table class="table table-hover input-width-100 permission-matrix">
                        <thead>
                            <tr>
                                <th>&nbsp;
                                </th>
                                <th>
                                    <cms:LocalizedLabel ID="lblAllow" runat="server" EnableViewState="false" ResourceString="Security.Allow" />
                                </th>
                                <th>
                                    <cms:LocalizedLabel ID="lblDeny" runat="server" EnableViewState="false" ResourceString="Security.Deny" />
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>
                                    <cms:LocalizedLabel ID="lblFullControl" runat="server" EnableViewState="false" ResourceString="Security.FullControl" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkFullControlAllow" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkFullControlDeny" runat="server" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cms:LocalizedLabel ID="lblRead" runat="server" EnableViewState="false" ResourceString="Security.Read" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkReadAllow" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkReadDeny" runat="server" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cms:LocalizedLabel ID="lblModify" runat="server" EnableViewState="false" ResourceString="Security.Modify" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkModifyAllow" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkModifyDeny" runat="server" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cms:LocalizedLabel ID="lblCreate" runat="server" EnableViewState="false" ResourceString="Security.Create" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkCreateAllow" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkCreateDeny" runat="server" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cms:LocalizedLabel ID="lblDelete" runat="server" EnableViewState="false" ResourceString="general.delete" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkDeleteAllow" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkDeleteDeny" runat="server" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cms:LocalizedLabel ID="lblDestroy" runat="server" EnableViewState="false" ResourceString="Security.Destroy" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkDestroyAllow" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkDestroyDeny" runat="server" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cms:LocalizedLabel ID="lblExploreTree" runat="server" EnableViewState="false" ResourceString="Security.ExploreTree" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkExploreTreeAllow" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkExploreTreeDeny" runat="server" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cms:LocalizedLabel ID="lblManagePermissions" runat="server" EnableViewState="false"
                                        ResourceString="Security.ManagePermissions" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkManagePermissionsAllow" runat="server" AutoPostBack="false" />
                                </td>
                                <td>
                                    <cms:CMSCheckBox ID="chkManagePermissionsDeny" runat="server" AutoPostBack="false" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <cms:FormSubmitButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
                        EnableViewState="false" ResourceString="general.ok" />
                </asp:Panel>
            </div>
        </div>
        <asp:HiddenField runat="server" ID="hdnOriginalValues" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
