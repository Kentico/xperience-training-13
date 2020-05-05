<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Development_DocumentTypes_DocumentType_Edit_Ecommerce"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Type Edit - Ecommerce"
     Codebehind="DocumentType_Edit_Ecommerce.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="DepartmentSelector" Src="~/CMSModules/Ecommerce/FormControls/DepartmentSelector.ascx" %>
<%@ Register TagPrefix="cms" TagName="SelectProductType" Src="~/CMSModules/Ecommerce/FormControls/SelectProductType.ascx" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">    
    <cms:CMSPanel runat="server" ID="pnlProductSection" ShortID="pp">
        <cms:LocalizedHeading runat="server" ID="headParelSection" Level="4" EnableViewState="false" ResourceString="com.documentproductrelation" />
        <cms:CMSCheckBox runat="server" ID="chkIsProduct" ResourceString="DocType.Ecommerce.IsProduct"
            CssClass="InfoLabel" AutoPostBack="true" />
        <cms:CMSCheckBox runat="server" ID="chkIsProductSection" ResourceString="DocType.Ecommerce.IsProductSection"
            CssClass="InfoLabel" />
    </cms:CMSPanel>

    <cms:CMSPanel runat="server" id="pnlDefaultSelection" class="form-horizontal" Visible="false">
        <cms:LocalizedHeading runat="server" ID="headCustom" Level="4" EnableViewState="false" ResourceString="com.productautocreation" />
        <div class="editing-form-category">
            <div class="editing-form-category-fields">
                <%-- Default department --%>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDepartments" EnableViewState="false" DisplayColon="true" ResourceString="doctype.ecommerce.defaultdepartment" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:DepartmentSelector runat="server" ID="departmentElem" DropDownListMode="false"
                            UseNameForSelection="false" ShowAllSites="true" AddNoneRecord="true" IsLiveSite="false" />
                    </div>
                </div>
                <%-- Default product type --%>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="LocalizedLabel1" runat="server" ResourceString="doctype.ecommerce.defaultproducttype" EnableViewState="false" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SelectProductType runat="server" ID="productTypeElem" />
                    </div>
                </div>

            </div>
        </div>
    </cms:CMSPanel>

    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />    
</asp:Content>
