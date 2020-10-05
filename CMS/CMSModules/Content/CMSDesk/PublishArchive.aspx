<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Content_CMSDesk_PublishArchive" Title="Publishes or archives multiple pages"
    ValidateRequest="false" Theme="Default"  Codebehind="PublishArchive.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Documents" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server" EnableViewState="false">
    <asp:Panel runat="server" ID="pnlContent" EnableViewState="false">
        <asp:Panel ID="pnlPublish" runat="server" EnableViewState="false">
            <cms:LocalizedHeading runat="server" ID="headQuestion" Level="4" EnableViewState="false" />
            <asp:Panel ID="pnlDocList" runat="server" Visible="false" CssClass="form-control vertical-scrollable-list"
                EnableViewState="false">
                <asp:Label ID="lblDocuments" runat="server" CssClass="ContentLabel" EnableViewState="false" />
            </asp:Panel>
            <div class="checkbox-list-vertical">
                <asp:PlaceHolder ID="plcCheck" runat="server" EnableViewState="false">
                    <asp:PlaceHolder ID="plcAllCultures" runat="server">
                        <cms:CMSCheckBox ID="chkAllCultures" runat="server" CssClass="ContentCheckbox"
                            EnableViewState="false" Checked="true" />
                    </asp:PlaceHolder>
                    <cms:CMSCheckBox ID="chkUnderlying" runat="server" CssClass="ContentCheckbox"
                        EnableViewState="false" Checked="true" />
                </asp:PlaceHolder>
            </div>
            <br />
            <div class="control-group-inline">
                <div class="keep-white-space-fixed">
                    <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
                        ResourceString="general.yes" EnableViewState="false" />
                    <cms:LocalizedButton ID="btnNo" runat="server" ButtonStyle="Default" OnClick="btnNo_Click"
                        ResourceString="general.no" EnableViewState="false" />
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
    <script type="text/javascript">
        //<![CDATA[

        // Display the page
        function SelectNode(nodeId) {
            if (parent != null) {
                if (parent.SelectNode != null) {
                    parent.SelectNode(nodeId);
                }
                RefreshTree(nodeId);
            }
        }

        function RefreshTree(nodeId) {
            if (parent != null) {
                if (parent.RefreshTree != null) {
                    parent.RefreshTree(nodeId, nodeId);
                }
            }

        }
        //]]>                        
    </script>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
