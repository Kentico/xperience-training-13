<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ProductSectionsContextMenu.ascx.cs"
    Inherits="CMSModules_Ecommerce_Controls_UI_ProductSectionsContextMenu" %>
<%@ Register TagPrefix="cms" TagName="ContextMenu" Src="~/CMSModules/Content/Controls/TreeContextMenu.ascx" %>
<cms:ContextMenu runat="server" ID="menuElem" DocumentTypeWhere="ClassIsProductSection = 1 OR ClassIsProduct = 1"
     DocumentTypeOrderBy="ClassIsProductSection DESC, ClassDisplayName"
    HidePropertiesItem="true" ShowSectionProperties="true" ResourceName="" />
