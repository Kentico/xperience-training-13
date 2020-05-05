<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" CodeBehind="ObjectTypeDetails.aspx.cs" Inherits="CMSModules_System_ObjectTypeDetails"
     Title="System - Object type details" Theme="Default" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="Server">
    <asp:PlaceHolder ID="plcMain" runat="server" Visible="false">

    <cms:LocalizedHeading runat="server" ID="objectTypeInfo" Level="4" ResourceString="administration.system.objecttypes.info" EnableViewState="false" />  
    <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblObjectTypeName" runat="server" EnableViewState="false" ResourceString="administration.system.objecttypes.name"
                             AssociatedControlID="lblObjectTypeNameValue" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:LocalizedLabel CssClass="form-control-text" ID="lblObjectTypeNameValue" runat="server" EnableViewState="false" />
                    </div>

                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblObjectType" runat="server" EnableViewState="false" ResourceString="administration.system.objecttypes.objecttype"
                            AssociatedControlID="lblObjectTypeValue" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:LocalizedLabel CssClass="form-control-text" ID="lblObjectTypeValue" runat="server" EnableViewState="false" />
                    </div>

                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblDBTable" runat="server" EnableViewState="false" ResourceString="administration.system.objecttypes.databasetable"
                             AssociatedControlID="lblDBTableValue" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:LocalizedLabel CssClass="form-control-text" ID="lblDBTableValue" runat="server" EnableViewState="false" />
                    </div>

                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSiteRelationship" runat="server" EnableViewState="false" ResourceString="administration.system.objecttypes.siterelation"
                            AssociatedControlID="lblSiteRelationshipValue" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:LocalizedLabel CssClass="form-control-text" ID="lblSiteRelationshipValue" runat="server" EnableViewState="false" />
                    </div>

                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblIsBinding" runat="server" EnableViewState="false" ResourceString="general.isbinding"
                             AssociatedControlID="lblIsBindingValue" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:LocalizedLabel CssClass="form-control-text" ID="lblIsBindingValue" runat="server" EnableViewState="false" />
                    </div>

                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSupportsCI" runat="server" EnableViewState="false" ResourceString="administration.system.objecttypes.supportsci"
                            AssociatedControlID="lblSupportsCIValue" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:LocalizedLabel CssClass="form-control-text" ID="lblSupportsCIValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
    </div>
    <cms:LocalizedHeading runat="server" ID="headingObjectTypeGraph" Level="4" EnableViewState="false" />    
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblGraphFilter" runat="server" EnableViewState="false" ResourceString="administration.system.objecttypegraph.display"
                    AssociatedControlID="chlGraphFilter" Visible="false" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBoxList ID="chlGraphFilter" runat="server" RepeatDirection="Vertical" Visible="false" />
           </div>
        </div>
    </div>    

    <asp:Panel ID="pnlObjectTypeGraph" runat="server" CssClass="object-type-graph" />

    </asp:PlaceHolder>
</asp:Content>