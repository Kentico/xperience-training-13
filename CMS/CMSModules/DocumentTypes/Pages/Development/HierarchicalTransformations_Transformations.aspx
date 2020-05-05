<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
     Codebehind="HierarchicalTransformations_Transformations.aspx.cs" Inherits="CMSModules_DocumentTypes_Pages_Development_HierarchicalTransformations_Transformations" %>

<%@ Register Src="~/CMSModules/DocumentTypes/Controls/HierarchicalTransformations_List.ascx"
    TagName="HierarchicalTransfList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/DocumentTypes/Controls/HierarchicalTransformations_Edit.ascx"
    TagName="HierarchicalTransfNew" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContainer" runat="server">
        <asp:Panel ID="pnlFilter" runat="server" CssClass="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server"
                        ID="lblTemplateType" ResourceString="documenttype_edit_transformation_edit.transformtype" AssociatedControlID="drpTransformations" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSDropDownList runat="server" ID="drpTransformations" CssClass="DropDownField" AutoPostBack="true" />
                </div>
            </div>
        </asp:Panel>
        <cms:HierarchicalTransfList runat="server" ID="ucTransf" />
        <cms:HierarchicalTransfNew runat="server" ID="ucNewTransf" Visible="false" />
    </asp:Panel>
</asp:Content>
