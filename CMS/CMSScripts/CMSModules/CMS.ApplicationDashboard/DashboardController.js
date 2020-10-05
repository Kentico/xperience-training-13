cmsdefine(['require', 'jQuery', 'exports', 'CMS/EventHub', 'lodash'], function(cmsrequire, $, exports, eventHub, lodash) {
    var _ = lodash;

    var Controller = (function () {
        /**
        * Controller's constructor. Initializes $scope with default values and
        * subscribes to events used in this module.
        */
        function Controller($scope, $window, $document, $timeout, $animate, __tiles) {
            var _this = this;
            this.$scope = $scope;
            this.$window = $window;
            this.$document = $document;
            this.$timeout = $timeout;
            this.$animate = $animate;
            this.__tiles = __tiles;
            /**
            * Determines whether application list has changed and thus should be saved to the server.
            */
            this.isDirty = false;
            /**
            * Saves the current state of the dashboard on the server, but only once in 2 seconds. Throttling is used to avoid
            * sending multiple save calls in a short time which could cause consistency problem.
            */
            this.performSaveChangesThrottled = _.throttle(function () {
                return _this.performSaveChanges();
            }, 2000);
            var applications = __tiles.query();

            $scope.model = {
                tiles: applications,
                isEditableMode: false,
                sortableOptions: {
                    cancel: '[data-no-drag]',
                    placeholder: 'drag-placeholder',
                    tolerance: 'pointer',
                    distance: 50,
                    containment: '#dashboard-drag-area',
                    cursor: "move",
                    items: "li:not(:last-child)",
                    start: function (event, ui) {
                        ui.item.addClass("shadow");
                        eventHub.publish('cms.applicationdashboard.DashboardSortingStarted');
                    },
                    stop: function (event, ui) {
                        ui.item.removeClass("shadow");
                        ui.placeholder.remove();
                        eventHub.publish('cms.applicationdashboard.DashboardSortingStopped');
                        _this.saveChanges();
                    }
                },
                toggleEditableMode: function () {
                    return _this.toggleEditable();
                },
                removeTile: function (position) {
                    return _this.removeTile(position);
                },
                openApplicationsList: function () {
                    return eventHub.publish('ToggleApplicationList');
                },
                applicationListIsExpanding: false
            };

            // Perform save of the current application list if changes were made, but not saved on server yet (due to throttling)
            $window.onbeforeunload = function () {
                if (_this.isDirty) {
                    _this.performSaveChanges();
                }
            };

            // Listen for click event to be able to close application on mouse click outside the tiles
            $document.on('click', function (e) {
                if (_this.$scope.model.isEditableMode) {
                    var $target = $(e.target);
                    if ($target.hasClass('dashboard') || $target.attr('id') === 'dashboard-drag-area') {
                        eventHub.publish('HideApplicationList');
                    }
                }

                var element = angular.element(e.srcElement);
                var appListLinkClass = 'js-app-list-link';
                var helpLinkClass = 'js-context-help-link';

                if (element.hasClass(appListLinkClass) || element.parent().hasClass(appListLinkClass)) {
                    eventHub.publish('ShowApplicationList');
                } else if (element.hasClass(helpLinkClass) || element.parent().hasClass(helpLinkClass)) {
                    eventHub.publish('ShowContextHelp');
                }
            });

            eventHub.subscribe('ApplicationListShown', function () {
                _this.$timeout(function() {
                    _this.$animate.addClass($('[data-edit-btn]')[0], 'btn-edit-mode-translate');
                });
            });

            eventHub.subscribe('ApplicationListHidden', function () {
                _this.$timeout(function() {
                    _this.$animate.removeClass($('[data-edit-btn]')[0], 'btn-edit-mode-translate');
                });
            });

            eventHub.subscribe('cms.applicationdashboard.ApplicationAdded', function (id) {
                return _this.addTile(id);
            });

            eventHub.subscribe('cms.applicationdashboard.ApplicationRemoved', function (id) {
                _this.removeTileById(id);
            });
        }

        /**
        * Removes given tile from all tiles inside $scope
        * @type {number} index   Index of tile to be removed
        */
        Controller.prototype.removeTile = function (index) {
            var _this = this;
            // Code in timeout will be safely applied after next animation loop
            // See http://stackoverflow.com/a/18996042
            this.$timeout(function () {
                if (index !== -1) {
                    _this.$scope.model.tiles.splice(index, 1);
                    _this.saveChanges();
                }
            });
        };

        /**
        * Toggles editable mode. Saves changes when
        * the toggle direction is from editable to noneditable
        */
        Controller.prototype.toggleEditable = function () {
            if (this.$scope.model.isEditableMode) {
                this.disableEditable();
            } else {
                this.enableEditable();
            }
        };

        /**
        * Gets application from service based on appGuid and creates
        * new tile, which is then inserted at the end of existing ones.
        */
        Controller.prototype.addTile = function (appGuid) {
            var _this = this;
            this.__tiles.load({ guid: appGuid }, function (tile) {
                if (!_.contains(_.pluck(_this.$scope.model.tiles, 'Id'), tile.Id)) {
                    eventHub.publish('cms.applicationdashboard.DashboardItemLoaded', tile.Id);
                    _this.$scope.model.tiles.push(tile);
                    _this.saveChanges();
                }
            });
        };

        /**
        * Removes tile from the dashboard based on the application guid
        */
        Controller.prototype.removeTileById = function (id) {
            var tileIdx = _.findIndex(this.$scope.model.tiles, function (t) {
                return t.Id === id;
            });

            this.removeTile(tileIdx);
        };

        /**
        * Enables editable mode
        */
        Controller.prototype.enableEditable = function () {
            eventHub.publish('cms.applicationdashboard.DashboardEditableModeChanged', true, this.$scope.model.tiles.map(function (t) {
                return t.Id;
            }));

            this.$scope.model.isEditableMode = true;
        };

        /**
        * Disables editable mode
        */
        Controller.prototype.disableEditable = function () {
            eventHub.publish('cms.applicationdashboard.DashboardEditableModeChanged', false);

            this.$scope.model.isEditableMode = false;
        };

        /**
        * Publish the event for saving current applications to server
        */
        Controller.prototype.saveChanges = function () {
            this.isDirty = true;
            this.performSaveChangesThrottled();
        };

        /**
        * Performs post request for saving the current application list
        * This method should not be called directly, use saveChanges() instead
        */
        Controller.prototype.performSaveChanges = function () {
            this.isDirty = false;
            this.__tiles.save(this.$scope.model.tiles, function () {
            }, function (error) {
                if (error.status == 403) {
                    window.top.location.href = error.data.LogonPageUrl;
                }
            });
        };
        Controller.$inject = [
            '$scope',
            '$window',
            '$document',
            '$timeout',
            '$animate',
            'cms.resource.tile'
        ];
        return Controller;
    })();
    exports.Controller = Controller;
});
