<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Customers_Customer_List" Theme="Default"
     Codebehind="Customer_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="UniGrid" OrderBy="CustomerLastName" IsLiveSite="false"
                Columns="CustomerID, CustomerLastName, CustomerCompany, CustomerFirstName, CustomerEmail, CustomerCreated, CustomerUserID, CustomerSiteID"
                ObjectType="ecommerce.customer" RememberStateByParam="">
                <GridActions>
                    <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="CustomerCompany" Caption="$Unigrid.Customers.Columns.CustomerCompanyName$"
                        Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="CustomerLastName" Caption="$Unigrid.Customers.Columns.CustomerLastName$"
                        Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="CustomerFirstName" Caption="$Unigrid.Customers.Columns.CustomerFirstName$"
                        Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="CustomerEmail" ExternalSourceName="#mailto" Caption="$general.email$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="CustomerCreated" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.Customers.Columns.CustomerUserCreated$"
                        Wrap="false" />
                    <ug:Column Source="CustomerUserID" ExternalSourceName="#yesno" Name="CustomerUserID"
                        Caption="$com.isregistered$" Wrap="false">
                        <Filter Type="custom" Path="~/CMSModules/Ecommerce/Controls/Filters/CustomerTypeFilter.ascx" />
                    </ug:Column>
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
