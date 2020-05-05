cmsdefine(['require', 'jQuery', 'exports', 'CMS/NumberShortener', 'CMS/EventHub'], function (cmsrequire, $, exports, numberShortener, eventHub) {
    var Controller = (function () {
        function Controller($scope, $compile, $element, $templateCache, $timeout, $animate, $q, liveTilePushService, $window) {
            var _this = this;
            this.$scope = $scope;
            this.$compile = $compile;
            this.$element = $element;
            this.$templateCache = $templateCache;
            this.$timeout = $timeout;
            this.$animate = $animate;
            this.$q = $q;
            this.liveTilePushService = liveTilePushService;
            this.$window = $window;
            /**
            * Event fired when editable mode toggles.
            */
            this.EDITABLE_MODE_CHANGED_EVENT = 'cms.applicationdashboard.DashboardEditableModeChanged';
            /**
            * Delay of the shrink/unshrink animation in milliseconds.
            */
            this.SHRINK_DELAY = 125;
            /**
            * CSS class for the live tile.
            */
            this.LIVE_WRAPPER_CSS_CLASS = 'tile-wrapper-live';
            /**
            * Indicates whether data was successfully loaded for the live tile.
            */
            this.hasData = false;
            /**
            * Flag determining whether the object was destroyed (removed from the DOM).
            */
            this.destroyed = false;
            /**
            * Starts loading of the live data.
            */
            this.startLiveData = function () {

                var firstData = true, previousShortenedValue = null;

                // Tile is dead or is in edit mode, no need to load data
                if (_this.$scope.tileModel.TileModelType !== 'ApplicationLiveTileModel' || _this.$scope.isEditableMode) {
                    return;
                }

                _this.interval = _this.liveTilePushService.start(_this.$scope.tileModel.ApplicationGuid, function (liveTile) {
                    var newShortenedValue = numberShortener.shortenNumber(liveTile.Value);

                    if (newShortenedValue === previousShortenedValue) {
                        return;
                    }

                    liveTile.ShortenedValue = previousShortenedValue = newShortenedValue;

                    // First data is true only at initialization of the tile
                    if (!firstData) {
                        _this.killTile().then(function () {
                            _this.refreshLiveTileData(liveTile);
                            _this.resurrectTile(true);
                        });
                    } else {
                        // When initializing tile, it does not have to be converted to the dead state
                        _this.refreshLiveTileData(liveTile);
                        _this.resurrectTile(true);
                    }

                    _this.hasData = true;
                    firstData = false;
                }, function () {
                    // An error occurred or no content is available, switch to dead tile
                    _this.killTile();
                    _this.hasData = false;
                });
            };
            /**
            * Initializes basic scope properties.
            */
            this.initializeScope = function () {
                _this.$scope.isEditableMode = false;
                _this.$scope.shrinked = false;
                _this.$scope.removeTile = _this.removeTile;
                _this.$scope.isEditableModeButtonDisabled = true;
            };
            /**
            * Handles clicking inside the current tile.
            *
            * @param e: MouseEvent Mouse click event
            */
            this.onClick = function (e) {
                if (_this.$scope.isEditableMode) {
                    e.preventDefault();
                }
            };
            /**
            * Handles clicking outside the current tile. Unselects the tile if was previously selected.
            *
            * @param e: JQueryEventObject Mouse click event
            */
            this.onClickOutside = function (e) {
                if (_this.getMainDataTileElement($(e.target)).data('id') !== _this.$scope.tileModel.Id) {
                    _this.deactivateTile();
                }
            };
            /**
            * Gets the main DOM element for the current tile.
            *
            * @param anyDom: ng.IAugmentedJQuery Any DOM element descendant to the main element
            */
            this.getMainDataTileElement = function (anyDom) {
                return anyDom.data('applicationTile') === 'tile' ? anyDom : $(anyDom.closest('[data-application-tile=tile]'));
            };
            /**
            * Activates all active/hover effects on tile. It is called on onmousedown event.
            */
            this.activateTile = function () {
                _this.$timeout(function () {
                    _this.$scope.active = true;
                });
            };
            /**
            * Deactivates all active/hover effects on tile.
            */
            this.deactivateTile = function () {
                _this.$timeout(function () {
                    _this.$scope.active = false;
                    _this.$scope.hover = false;
                });
            };
            /**
            * Hover in effect
            */
            this.onmouseenter = function () {
                _this.$timeout(function () {
                    _this.$scope.hover = true;
                });
            };
            /**
            * Hover out effect
            */
            this.onmouseleave = function () {
                _this.$timeout(function () {
                    _this.$scope.hover = false;
                });
            };
            /**
            * Handles changes after enabling or disabling editable mode.
            */
            this.handleEditMode = function () {
                // If tile is in edited mode while initialization, handle the shrinking
                if (_this.$scope.isEditableMode) {
                    _this.waitForShrink();
                }

                eventHub.subscribe(_this.EDITABLE_MODE_CHANGED_EVENT, function (isEditableMode) {
                    _this.$scope.isEditableMode = isEditableMode;
                    _this.deactivateTile();

                    if (_this.$scope.isEditableMode) {
                        _this.killTile();
                        _this.waitForShrink();
                        $(_this.$window).on('mousedown', _this.onClickOutside);
                    } else {
                        $(_this.$window).off('mousedown', _this.onClickOutside);
                        if (!_this.destroyed) {
                            _this.startLiveData();
                        }
                        if (_this.hasData) {
                            _this.resurrectTile(false);
                        }
                        _this.waitForUnshrink();
                    }
                });
            };
            /**
            * Waits until the shrinking animation is finished and then set scope.shrinked to true.
            */
            this.waitForShrink = function () {
                _this.$timeout(function () {
                    _this.$scope.shrinked = true;
                }, _this.SHRINK_DELAY);
            };
            /**
            * Waits until the unshrinking animation is finished and then set scope.shrinked to false.
            */
            this.waitForUnshrink = function () {
                _this.$timeout(function () {
                    _this.$scope.shrinked = false;
                }, _this.SHRINK_DELAY);
            };
            /**
            * Removes tile from the dashboard.
            */
            this.removeTile = function () {
                eventHub.publish('cms.applicationdashboard.ApplicationRemoved', _this.$scope.tileModel.Id);
            };
            /**
            * Converts the tile to the dead state. Performs proper animations.
            */
            this.killTile = function () {
                var deferred = _this.$q.defer();
                if (!_this.wrapper) {
                    return deferred.reject;
                }
                _this.$animate.removeClass(_this.wrapper[0], _this.LIVE_WRAPPER_CSS_CLASS).then(function () {
                    return deferred.resolve();
                });
                return deferred.promise;
            };
            /**
            * Converts the tile to the live state. Performs proper animations.
            */
            this.resurrectTile = function (useRandom) {
                var deferred = _this.$q.defer();

                _this.$timeout(function () {
                    if (_this.$scope.isEditableMode) {
                        return deferred.reject;
                    }

                    _this.$animate.addClass(_this.wrapper[0], _this.LIVE_WRAPPER_CSS_CLASS).then(function () {
                        return deferred.resolve();
                    });
                }, useRandom ? Math.random() * 1000 : 0);
                
                return deferred.promise;
            };
            /**
            * Refreshes data of the live tile template, ensures correct template is loaded.
            */
            this.refreshLiveTileData = function (liveTile) {
                _this.wrapper.append(_this.$compile(_this.$templateCache.get('applicationLiveTileTemplate.html'))(_this.$scope));
                _this.$scope.liveTileModel = liveTile;
            };
            // Initialization
            this.initializeScope();

            // Events registration
            this.handleEditMode();

            // Live data handling
            this.startLiveData();

            // Ends loading of the data when tile is being removed
            $scope.$on('$destroy', function () {
                _this.destroyed = true;
            });
        }
        Controller.$inject = [
            '$scope', '$compile', '$element', '$templateCache', '$timeout', '$animate', '$q', 'cms.service.liveTilePushService', '$window'
        ];
        return Controller;
    })();
    ;

    exports.Directive = [
        '$parse', '$compile', '$templateCache', function ($parse, $compile, $templateCache) {
            var tile = {
                restrict: 'A',
                scope: {
                    tileModel: '=applicationTile'
                },
                link: function (scope, element, attrs, controller) {
                    element = $(element);
                    // Load template to the current element
                    element.html($templateCache.get('applicationTileTemplate.html'));
                    $compile(element.contents())(scope);

                    // Handle click on the tile
                    element.on('click', controller.onClick);
                    element.on('mousedown', controller.activateTile);

                    // Handle hover
                    element.on('mouseenter', controller.onmouseenter);
                    element.on('mouseleave', controller.onmouseleave);

                    // Load the most outer div of the tile
                    controller.wrapper = element.find('.tile-wrapper');

                    // Store the parent controller
                    scope.isEditableMode = scope.$parent.model.isEditableMode;
                },
                controller: Controller
            };

            return tile;
        }];
});
