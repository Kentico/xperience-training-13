cmsdefine(['require', 'jQuery', 'exports', 'CMS/NumberShortener', 'CMS/EventHub'], function (cmsrequire, $, exports, numberShortener, eventHub) {
    var Controller = (function () {
        function Controller($scope, $compile, $element, $templateCache, $timeout, $window) {
            var _this = this;
            this.$scope = $scope;
            this.$compile = $compile;
            this.$element = $element;
            this.$templateCache = $templateCache;
            this.$timeout = $timeout;
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
            * Flag determining whether the object was destroyed (removed from the DOM).
            */
            this.destroyed = false;
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
                return anyDom.data('singleObjectTile') === 'tile' ? anyDom : $(anyDom.closest('[data-single-object-tile=tile]'));
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
                        _this.waitForShrink();
                        $(_this.$window).on('mousedown', _this.onClickOutside);
                    } else {
                        $(_this.$window).off('mousedown', _this.onClickOutside);
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
            // Initialization
            this.initializeScope();

            // Events registration
            this.handleEditMode();

            // Ends loading of the data when tile is being removed
            $scope.$on('$destroy', function () {
                _this.destroyed = true;
            });
        }
        Controller.$inject = [
            '$scope', '$compile', '$element', '$templateCache', '$timeout', '$window'
        ];
        return Controller;
    })();

    exports.Directive = [
        '$parse', '$compile', '$templateCache', function ($parse, $compile, $templateCache) {
            var tile = {
                restrict: 'A',
                scope: {
                    tileModel: '=singleObjectTile'
                },
                link: function (scope, element, attrs, controller) {
                    element = $(element);
                    // Load template to the current element

                    element.html($templateCache.get('singleObjectTileTemplate.html'));
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