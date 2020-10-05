<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ApplicationDashboard.aspx.cs" Inherits="CMSModules_ApplicationDashboard_ApplicationDashboard"
    Theme="Default" EnableEventValidation="false"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="ApplicationDashboard" EnableViewState="false" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <%-- Welcome tile --%>
    <script type="text/ng-template" id="welcomeTileTemplate.html">
        <div class="tile" data-ng-show="model.visible">
            <div class="welcome-tile">
                <a href="javascript:void(0)" data-ng-click="model.hide()">
                    <i aria-hidden="true" class="icon-modal-close"></i>
                </a>
                <h2>{{model.header}}</h2>
                <p class="lead" data-ng-bind-html="model.description"></p>
            </div>
        </div>
    </script>
    
    <%-- General tile --%>
    <script type="text/ng-template" id="tileTemplate.html">
        <div data-ng-class="{'editable-mode':isEditableMode, 'shrinked':shrinked }" >
            <div data-ng-class="{'tile-shrink':isEditableMode && !(hover || active )}" class="tile-background" >
            </div>      
            <div class="tile-header-panel" data-ng-show="isEditableMode && (hover || active)" >
                <button type="button" class="icon-only btn-icon btn" title="<%= GetString("cms.dashboard.removeApplication") %>" data-ng-click="removeTile()">
                    <i aria-hidden="true" class="icon-modal-close"></i><span class="sr-only"><%= GetString("cms.dashboard.removeApplication") %></span>
                </button>
            </div>
            <div class="tile-mask" data-ng-class="{'tile-shrink': isEditableMode}" >
                <div class="tile-wrapper">        
                    <div class="tile-dead">
                        <a class="tile-btn tile-dead-btn {{ tileModel.ListItemCssClass }}" data-ng-class="{'editable-mode':isEditableMode}" data-ng-href="{{ tileModel.Path }}" target="_top">
                            <div data-tile-icon="tileModel.TileIcon" data-icon-alternative-text="tileModel.DisplayName" data-icon-style="cms-icon-200 tile-dead-icon"  />                    
                            <h3>{{ tileModel.DisplayName }}</h3>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </script>
    
    <%-- Application tile --%>
    <script type="text/ng-template" id="applicationTileTemplate.html">
        <div data-ng-class="{'editable-mode':isEditableMode, 'shrinked':shrinked }" >
            <div data-ng-class="{'tile-shrink':isEditableMode && !(hover || active )}" class="tile-background" >
            </div>      
            <div class="tile-header-panel" data-ng-show="isEditableMode && (hover || active)" >
                <button type="button" class="icon-only btn-icon btn" title="<%= GetString("cms.dashboard.removeApplication") %>" data-ng-click="removeTile()">
                    <i aria-hidden="true" class="icon-modal-close"></i><span class="sr-only"><%= GetString("cms.dashboard.removeApplication") %></span>
                </button>
            </div>
            <div class="tile-mask" data-ng-class="{'tile-shrink': isEditableMode}" >
                <div class="tile-wrapper">        
                    <div class="tile-dead">
                        <a class="tile-btn tile-dead-btn {{ tileModel.ListItemCssClass }}" data-ng-class="{'editable-mode':isEditableMode}" data-ng-href="{{ tileModel.Path }}" target="_top">
                            <div data-tile-icon="tileModel.TileIcon" data-icon-alternative-text="tileModel.DisplayName" data-icon-style="cms-icon-200 tile-dead-icon"  />                    
                            <h3>{{ tileModel.DisplayName }}</h3>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </script>


    <%-- Live part of the tile --%>
    <script type="text/ng-template" id="applicationLiveTileTemplate.html">
        <div class="tile-live">
            <a class="tile-btn tile-live-btn {{ tileModel.ListItemCssClass }}" data-ng-href="{{ tileModel.Path }}" target="_top">
                <div class="clearfix">
                    <div data-tile-icon="tileModel.TileIcon" data-icon-alternative-text="tileModel.DisplayName" data-icon-style="cms-icon-150 tile-live-icon" />            
                    <span class="tile-live-title-container">
                        <h3>{{ tileModel.DisplayName }}</h3>
                    </span>
                </div>
                <h4 class="tile-live-value">{{ liveTileModel.ShortenedValue }}</h4>
                <div class="tile-live-description">{{ liveTileModel.Description }}</div>
            </a>
        </div>      
    </script>

    <%-- Single object part of the tile --%>
    <script type="text/ng-template" id="singleObjectTileTemplate.html">
         <div data-ng-class="{'editable-mode':isEditableMode, 'shrinked':shrinked }" >
            <div data-ng-class="{'tile-shrink':isEditableMode && !(hover || active )}" class="tile-background" >
            </div>      
            <div class="tile-header-panel" data-ng-show="isEditableMode && (hover || active)" >
                <button type="button" class="icon-only btn-icon btn" title="<%= GetString("cms.dashboard.removeApplication") %>" data-ng-click="removeTile()">
                    <i aria-hidden="true" class="icon-modal-close"></i><span class="sr-only"><%= GetString("cms.dashboard.removeApplication") %></span>
                </button>
            </div>
            <div class="tile-mask" data-ng-class="{'tile-shrink': isEditableMode}" >
                <div class="tile-wrapper">        
                     <div class="tile-single-object">
                        <a class="tile-btn tile-single-object-btn {{ tileModel.ListItemCssClass }}" data-ng-href="{{ tileModel.Path }}" data-ng-class="{'editable-mode':isEditableMode}"  target="_top">
                            <div class="clearfix">
                                <div data-tile-icon="tileModel.TileIcon" data-icon-alternative-text="tileModel.DisplayName" data-icon-style="cms-icon-150 tile-live-icon" />            
                                <span class="tile-live-title-container">
                                    <h3>{{ tileModel.ApplicationDisplayName }}</h3>
                                </span>
                            </div>
                            <div class="tile-description" data-ng-bind="tileModel.ObjectDisplayName" data-ellipsis="ellipsis"></div>
                        </a>
                    </div>  
                </div>
            </div>
        </div>
    </script>
    
    <%-- Tile icon layout --%>
    <script type="text/ng-template" id="tileIconTemplate.html">
        <img data-ng-if="icon.IconType == 'Image'" data-ng-src="{{ icon.IconImagePath }}" alt="{{ iconAlternativeText }}" class="tile-icon {{ iconStyle }}" />
        <i data-ng-if="icon.IconType == 'CssClass'" aria-hidden="true" class="{{ icon.IconCssClass }} tile-icon {{ iconStyle }}"></i>
    </script>

    <%-- Dashboard layout --%>
    <script type="text/ng-template" id="dashboard.html">
        <div class="dashboard">
            <div id="dashboard-drag-area" class="dashboard-inner" data-ng-class="{'edit-mode' : model.isEditableMode}">
                <div data-welcometile></div>
                <ul data-ui-sortable="model.sortableOptions" data-ng-model="model.tiles">
                    <li class="tile" data-ng-repeat="tile in model.tiles" data-remove="model.removeTile($index)" data-ng-show="tile.IsVisible">
                        <div data-application-tile="tile" data-id="{{ tile.Id }}" class="tile-outer-wrapper" data-ng-if="(tile.TileModelType == 'ApplicationTileModel' || tile.TileModelType == 'ApplicationLiveTileModel') && tile.IsVisible" ></div>
                        <div data-single-object-tile="tile" data-id="{{ tile.Id }}" class="tile-outer-wrapper" data-ng-if="tile.TileModelType == 'SingleObjectTileModel' && tile.IsVisible" ></div>
                    </li>
            
                    <%-- Add 'new app' tile --%>
                    <li data-ng-show="model.isEditableMode" data-ng-click="model.openApplicationsList()" class="tile tile-shrink" data-no-drag="no-drag">
                        <a class="tile-dead-btn tile-btn tile-btn-add" href="javascript:void(0);" title="<%= GetString("cms.dashboard.addapp") %>">
                            <i aria-hidden="true" class="icon-plus cms-icon-200 tile-icon"></i><span class="sr-only"><%= GetString("cms.dashboard.addapp") %></span>
                        </a>
                    </li>
                </ul>
            </div>
            <%-- Edit mode button --%>
            <div class="btn-group dropup anchor-dropup pull-right">
                <button data-ng-click="model.toggleEditableMode()" type="button" class="btn btn-edit-mode btn-default icon-only {{ model.isEditableMode ? 'active' : '' }}" data-edit-btn="edit-btn" data-ng-disabled="model.isEditableModeButtonDisabled" title="<%= GetString("cms.dashboard.edit") %>">
                    <i aria-hidden="true" class="icon-cogwheel"></i><span class="sr-only"><%= GetString("cms.dashboard.edit") %></span>
                </button>
            </div>
        </div>   
    </script>
    
    <div id="container" data-ng-view="ng-view"></div>

    <asp:Panel runat="server" ID="pnlLicenseOwner" Visible="False">
        <cms:LocalizedLabel runat="server" ID="lblLicenseOwner" CssClass="license-owner-info" />
    </asp:Panel>
</asp:Content>