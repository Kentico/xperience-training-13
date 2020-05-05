<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskGroup.aspx.cs" Inherits="CMSModules_Staging_Tools_TaskGroup_TaskGroup" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"%>

<%@ Register Src="~/CMSModules/Staging/FormControls/ServerSelector.ascx" TagName="ServerSelector"
    TagPrefix="cms" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcSiteSelector">
        <div class="form-horizontal form-filter">
            <div>
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblServers" CssClass="control-label" runat="server" EnableViewState="false" ResourceString="Tasks.SelectServer" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:ServerSelector ID="selectorElem" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="plcContent" ID="cntBody">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlLog" Visible="false">
                <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Synchronization" />
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlNotLogged">
                <cms:DisabledModule runat="server" ID="ucDisabledModule" />
            </asp:Panel>
            <asp:PlaceHolder ID="plcContent" runat="server">
                <cms:UniGrid ID="gridTasks" runat="server" GridName="~/CMSModules/Staging/Tools/AllTasks/Tasks.xml"
                    IsLiveSite="false" OrderBy="TaskTime, TaskID" ExportFileName="staging_task" />
                <br />
                <asp:Panel ID="pnlFooter" runat="server" CssClass="Clear">
                    <table class="Table100">
                        <tr>
                            <td>
                                <cms:LocalizedButton runat="server" ID="btnSyncSelected" ButtonStyle="Default" OnClick="btnSyncSelected_Click"
                                    ResourceString="Tasks.SyncSelected" EnableViewState="false" />
                                <cms:LocalizedButton runat="server" ID="btnSyncAll" ButtonStyle="Default" OnClick="btnSyncAll_Click"
                                    ResourceString="Tasks.SyncAll" EnableViewState="false" />
                            </td>
                            <td class="TextRight">
                                <cms:LocalizedButton runat="server" ID="btnDeleteSelected" ButtonStyle="Default"
                                    OnClick="btnDeleteSelected_Click" ResourceString="Tasks.DeleteSelected" EnableViewState="false" />
                                <cms:LocalizedButton runat="server" ID="btnDeleteAll" ButtonStyle="Default" OnClick="btnDeleteAll_Click"
                                    ResourceString="Tasks.DeleteAll" EnableViewState="false" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:PlaceHolder>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>

