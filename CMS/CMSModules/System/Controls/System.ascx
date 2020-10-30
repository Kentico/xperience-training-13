<%@ Control Language="C#" AutoEventWireup="False" CodeBehind="System.ascx.cs" Inherits="CMSModules_System_Controls_System" %>

<asp:Panel ID="pnlBody" runat="server" SkinID="Default">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblRefresh" CssClass="control-label" EnableViewState="false" ResourceString="Administration-System.Refresh" AssociatedControlID="drpRefresh" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSDropDownList runat="server" ID="drpRefresh" EnableViewState="true" AutoPostBack="true">
                    <asp:ListItem Selected="True">1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>5</asp:ListItem>
                    <asp:ListItem>10</asp:ListItem>
                    <asp:ListItem>30</asp:ListItem>
                    <asp:ListItem>60</asp:ListItem>
                </cms:CMSDropDownList>
                <cms:LocalizedButton runat="server" ID="btnRefresh" ResourceString="General.Refresh" ButtonStyle="Default" />
            </div>
        </div>
    </div>
    <div class="Clear"></div>
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" CatchErrors="true" EnableViewState="true">
        <ContentTemplate>
            <cms:LocalizedHeading runat="server" ID="headSystemInfo" Level="4" ResourceString="Administration-System.SystemInfo" EnableViewState="false" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblMachineName" runat="server" EnableViewState="false" AssociatedControlID="lblMachineNameValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblMachineNameValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblAspAccount" runat="server" EnableViewState="false" AssociatedControlID="lblValueAspAccount" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblValueAspAccount" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblAspVersion" runat="server" EnableViewState="false" AssociatedControlID="lblValueAspVersion" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblValueAspVersion" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblIP" runat="server" EnableViewState="false" AssociatedControlID="lblIPValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblIPValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
            </div>
            <cms:LocalizedHeading runat="server" ID="headDatabase" Level="4" ResourceString="Administration-System.DatabaseInfo" EnableViewState="false" />
            <div class="form-horizontal">
                <asp:PlaceHolder ID="plcSeparatedHeader" runat="server" EnableViewState="false">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblDatabase" runat="server" ResourceString="separationDB.databasepurpose" DisplayColon="true"
                                EnableViewState="false" AssociatedControlID="lblMainDB" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:LocalizedLabel CssClass="form-control-text" ID="lblMainDB" runat="server" ResourceString="separationDB.mainDB"
                                EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-value-cell-offset">
                            <cms:LocalizedLabel ID="lblSeparatedDB" CssClass="form-control-text" runat="server" ResourceString="separationDB.separatedDB"
                                EnableViewState="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblServerName" runat="server" EnableViewState="false" AssociatedControlID="lblServerNameValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblServerNameValue" runat="server" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcSeparatedServerName" runat="server">
                    <div class="form-group">
                        <div class="editing-form-value-cell-offset">
                            <asp:Label CssClass="form-control-text" ID="lblSeparatedServerName" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblServerVersion" runat="server" EnableViewState="false" AssociatedControlID="lblServerVersionValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblServerVersionValue" runat="server" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcSeparatedVersion" runat="server">
                    <div class="form-group">
                        <div class="editing-form-value-cell-offset">
                            <asp:Label CssClass="form-control-text" ID="lblSeparatedVersion" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblDBName" runat="server" EnableViewState="false" AssociatedControlID="lblDBNameValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblDBNameValue" runat="server" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcSeparatedName" runat="server">
                    <div class="form-group">
                        <div class="editing-form-value-cell-offset">
                            <asp:Label CssClass="form-control-text" ID="lblSeparatedName" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblDBSize" runat="server" EnableViewState="false" AssociatedControlID="lblDBSizeValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblDBSizeValue" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-value-cell-offset">
                        <asp:PlaceHolder ID="plcSeparatedSize" runat="server">
                            <asp:Label CssClass="form-control-text" ID="lblSeparatedSize" runat="server" />
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
            <asp:Timer ID="timRefresh" runat="server" Interval="2000" EnableViewState="false" Enabled="False" />
            <cms:LocalizedHeading runat="server" ID="headMempry" Level="4" ResourceString="Administration-System.MemoryStatistics" EnableViewState="false" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblAlocatedMemory" runat="server" EnableViewState="false" AssociatedControlID="lblValueAlocatedMemory" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblValueAlocatedMemory" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <asp:PlaceHolder runat="server" ID="plcAdvanced" EnableViewState="false">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <asp:Label CssClass="control-label" ID="lblPeakMemory" runat="server" EnableViewState="false" AssociatedControlID="lblValuePeakMemory" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label CssClass="form-control-text" ID="lblValuePeakMemory" runat="server" EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <asp:Label CssClass="control-label" ID="lblPhysicalMemory" runat="server" EnableViewState="false" AssociatedControlID="lblValuePhysicalMemory" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label CssClass="form-control-text" ID="lblValuePhysicalMemory" runat="server" EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <asp:Label CssClass="control-label" ID="lblVirtualMemory" runat="server" EnableViewState="false" AssociatedControlID="lblValueVirtualMemory" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:Label CssClass="form-control-text" ID="lblValueVirtualMemory" runat="server" EnableViewState="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:LocalizedButton ID="btnClear" runat="server" ResourceString="Administration-System.btnClear" ButtonStyle="Default" OnClick="ClearMemory"
                            Visible="false" EnableViewState="false" />
                    </div>
                </div>
            </div>
            <cms:LocalizedHeading runat="server" ID="headGC" Level="4" ResourceString="Administration-System.GC" EnableViewState="false" />
            <div class="form-horizontal">
                <asp:PlaceHolder runat="server" ID="plcGC" EnableViewState="false"></asp:PlaceHolder>
            </div>
            <cms:LocalizedHeading runat="server" ID="headCache" Level="4" ResourceString="Administration-System.CacheStatistics" EnableViewState="false" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblCacheItems" runat="server" EnableViewState="false" AssociatedControlID="lblCacheItemsValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblCacheItemsValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblCacheExpired" runat="server" EnableViewState="false" AssociatedControlID="lblCacheExpiredValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblCacheExpiredValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblCacheRemoved" runat="server" EnableViewState="false" AssociatedControlID="lblCacheRemovedValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblCacheRemovedValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblCacheDependency" runat="server" EnableViewState="false" AssociatedControlID="lblCacheDependencyValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblCacheDependencyValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblCacheUnderused" runat="server" EnableViewState="false" AssociatedControlID="lblCacheUnderusedValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblCacheUnderusedValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:LocalizedButton ID="btnClearCache" ResourceString="Administration-System.btnClearCache" runat="server" ButtonStyle="Default" OnClick="ClearCache"
                            Visible="false" EnableViewState="false" />
                    </div>
                </div>
            </div>
            <cms:LocalizedHeading runat="server" ID="headSystemTime" Level="4" ResourceString="Administration-System.SystemTimeInfo" EnableViewState="false" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblRunTime" runat="server" EnableViewState="false" AssociatedControlID="lblRunTimeValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblRunTimeValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblServerTime" runat="server" EnableViewState="false" AssociatedControlID="lblServerTimeValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label CssClass="form-control-text" ID="lblServerTimeValue" runat="server" EnableViewState="false" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <div class="Clear"></div>
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:LocalizedButton ID="btnRestart" ResourceString="Administration-System.btnRestart" runat="server" ButtonStyle="Primary" OnClick="Restart" Visible="false" EnableViewState="false" />
                <cms:LocalizedButton ID="btnRestartWebfarm" ResourceString="Administration-System.btnRestartWebfarm" runat="server" ButtonStyle="Default" OnClick="RestartWebfarm" Visible="false" EnableViewState="false" />
                <cms:LocalizedButton ID="btnRestartServices" ResourceString="Administration-System.btnRestartServices" runat="server" ButtonStyle="Default" OnClick="RestartServices" Visible="false" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Panel>
