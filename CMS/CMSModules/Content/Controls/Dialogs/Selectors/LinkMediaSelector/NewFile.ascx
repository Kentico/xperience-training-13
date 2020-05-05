<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="NewFile.ascx.cs" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_NewFile" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<asp:PlaceHolder ID="plcDirectFileUploader" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
                <div class="LeftAlign">
                    <div id="dialogsUploaderDiv">
                        <cms:DirectFileUploader ID="fileUploader" runat="server" InsertMode="true" IncludeNewItemInfo="true"
                            CheckPermissions="false" ShowProgress="true" />
                    </div>
                    <div id="dialogsUploaderDisabledDiv" class="DialogsUploaderDisabled" style="display: none;">
                        <cms:LocalizedButton ResourceString="general.upload" Enabled="false" EnableViewState="false" runat="server"/>
                    </div>
                </div>
                <asp:PlaceHolder ID="plcNewFile" runat="server" Visible="false">
                    <div class="LeftAlign NewFile">
                        <cms:CMSButton ID="btnNew" runat="server" ButtonStyle="Default" />
                    </div>
                </asp:PlaceHolder>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:PlaceHolder>
