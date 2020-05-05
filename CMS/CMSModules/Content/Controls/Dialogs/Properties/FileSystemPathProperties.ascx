<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Properties_FileSystemPathProperties"  Codebehind="FileSystemPathProperties.ascx.cs" %>

<div class="DialogInfoArea">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <asp:Label ID="lblEmpty" runat="server" />
    <cms:CMSUpdatePanel ID="plnPathUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
                Visible="false" />
            <div class="form-horizontal">
                <asp:PlaceHolder runat="server" ID="plcPathText">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblPathText" runat="server" EnableViewState="false" ResourceString="general.path"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtPathText" ReadOnly="true" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblUrl" runat="server" EnableViewState="false" ResourceString="generalproperties.liveurl"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label runat="server" ID="lblFileUrl" CssClass="form-control-text" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcFileSize">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblFileSize" runat="server" EnableViewState="false" ResourceString="media.file.filesize"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label runat="server" ID="lblFileSizeText" CssClass="form-control-text" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <div class="Hidden">
        <cms:CMSUpdatePanel ID="plnFileSystemButtonsUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Button ID="hdnButton" runat="server" OnClick="hdnButton_Click" CssClass="HiddenButton"
                    EnableViewState="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</div>
