<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_FormControls_PageTemplates_PageTemplateLevels"
     Codebehind="PageTemplateLevels.ascx.cs" %>
<%@ Register Src="~/CMSModules/PortalEngine/FormControls/PageTemplates/LevelTree.ascx"
    TagPrefix="cms" TagName="LevelTree" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
    <ContentTemplate>
        <div class="form-group">
            <div class="radio-list-vertical">
                <cms:CMSRadioButton runat="server" ID="radInheritAll" GroupName="Inheritance" AutoPostBack="true" />
                <cms:CMSRadioButton runat="server" ID="radNonInherit" GroupName="Inheritance" AutoPostBack="true" />
                <cms:CMSRadioButton runat="server" ID="radInheritMaster" GroupName="Inheritance" AutoPostBack="true" />
                <cms:CMSRadioButton runat="server" ID="radSelect" GroupName="Inheritance" AutoPostBack="true" /> 
            </div>
            <asp:PlaceHolder runat="server" ID="plcTree">
                <cms:LevelTree runat="server" ID="treeElem" />
            </asp:PlaceHolder>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
