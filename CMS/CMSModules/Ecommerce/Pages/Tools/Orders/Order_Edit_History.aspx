<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_History"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Order edit - History"
     Codebehind="Order_Edit_History.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="Orders">
        <cms:UniGrid runat="server" ID="gridElem" OrderBy="Date, ChangedByUserID" IsLiveSite="false"
            ExportFileName="ecommerce_orderstatushistory" ObjectType="ecommerce.orderstatususer">
            <GridColumns>
                <ug:Column Source="Date" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.OrderHistory.Columns.Date$" AllowSorting="false"
                    Wrap="false" />
                <ug:Column Source="ToStatusID"  ExternalSourceName="statusName" Caption="$Unigrid.OrderHistory.Columns.Status$"
                    Wrap="false" AllowSorting="false" />
                <ug:Column Source="ChangedByUserID" ExternalSourceName="#username" Caption="$general.username$"
                    Wrap="false" AllowSorting="false" />
                <ug:Column Source="ChangedByUserID" ExternalSourceName="#transform: cms.user : FullName" AllowSorting="false" Caption="$general.fullname$" Wrap="false" />
                <ug:Column Source="Note" AllowSorting="false" Caption="$Unigrid.OrderHistory.Columns.Note$"
                    Wrap="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions DisplayFilter="false" />
        </cms:UniGrid>
    </div>
</asp:Content>
