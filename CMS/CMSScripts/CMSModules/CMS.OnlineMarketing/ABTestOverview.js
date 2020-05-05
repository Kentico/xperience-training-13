cmsdefine(['jQuery'], function ($) {

    var ABOverview = function (serverData) {

        var advancedControls, spanShowMoreLess, filters;

        // Saves state of all selectors (sampling, success metric, counting methodology, ..) + status of whether the advanced controls
        // are collapsed or expanded to a cookie, so it can be retrieved on further requests from code-behind.
        var saveSelectorsState = function (selectorsState) {
            if (!selectorsState) {
                selectorsState = {};
            }

            if (selectorsState[serverData.samplingElementId] === undefined) {
                selectorsState[serverData.samplingElementId] = filters.sampling.index(filters.sampling.filter(".active"));
            }

            if (selectorsState[serverData.graphDataElementId] === undefined) {
                selectorsState[serverData.graphDataElementId] = filters.graphData.index(filters.graphData.filter(".active"));
            }

            selectorsState[serverData.successMetricDropdownId] = filters.successMetric[0].selectedIndex;
            selectorsState[serverData.countingMethodologyDropdownId] = filters.countingMethodology[0].selectedIndex;
            selectorsState[serverData.conversionsDropdownId] = filters.conversion[0].value;
            // Advanced controls with culture selector are not available in MVC
            if (advancedControls && filters.culture) {
                selectorsState[serverData.cultureDropdownId] = filters.culture[0].value;
                selectorsState[serverData.advancedControlsId] = advancedControls.is(':visible');
            }

            var today = new Date(),
                expiration = new Date();

            expiration.setTime(today.getTime() + 3600 * 1000 * 24 * 60);
            document.cookie = serverData.selectorsCookieKey + "=" + JSON.stringify(selectorsState) + "; path=/; expires=" + expiration.toGMTString();
        };

        // Save the state of all selectors. State of the sampling buttons is set to given button.
        var saveSelectorStateSamplingClick = function () {
            var selectorsState = {};

            selectorsState[serverData.samplingElementId] = filters.sampling.index($(this));
            saveSelectorsState(selectorsState);
        };

        // Save the state of all selectors. State of the graph data buttons is set to given button.
        var saveSelectorStateGraphDataClick = function () {
            var selectorsState = {};

            selectorsState[serverData.graphDataElementId] = filters.graphData.index($(this));
            saveSelectorsState(selectorsState);
        };

        // Switches visibility of advanced controls and changes text of the switch control.
        var changeAdvancedFilters = function () {
            if (advancedControls.is(':visible')) {
                advancedControls.hide();
                spanShowMoreLess.text(serverData.showFiltersText);
            } else {
                advancedControls.attr('style', 'display: block;');
                spanShowMoreLess.text(serverData.hideFiltersText);
            }
        };

        // Finds all controls needed by this module. They cannot be found on module init, because by then jQuery is not initialized yet.
        var findControls = function () {
            advancedControls = serverData.advancedControlsClientId && $('#' + serverData.advancedControlsClientId);
            spanShowMoreLess = serverData.showAdvancedFiltersSpanClientId && $('#' + serverData.showAdvancedFiltersSpanClientId);
            filters = {
                sampling: $('#' + serverData.samplingElementClientId + ' > .btn'),
                successMetric: $('#' + serverData.successMetricDropdownClientId),
                countingMethodology: $('#' + serverData.countingMethodologyDropdownClientId),
                graphData: $('#' + serverData.graphDataElementClientId + ' > .btn'),
                conversion: $('select[id^=\'' + serverData.conversionsDropdownClientId + '\']')
            };

            if (serverData.cultureDropdownClientId) {
                filters.culture = $('#' + serverData.cultureDropdownClientId + ' select');
            }
        };

        // Creates handlers that save selector state.
        var createFilterHandlers = function () {
            $.each(filters, function (name, filter) {
                filter.change(function () {
                    saveSelectorsState();
                });
            });
        };

        // Creates a handler that switches visibility of advanced filters and saves selector state.
        var createMoreLessButtonHandler = function () {
            spanShowMoreLess.click(function () {
                changeAdvancedFilters();
                saveSelectorsState();
            });
        };

        // Creates onclick events for each filter button
        var setOnClickEventToButtons = function () {
            filters.sampling.each(function () {
                overrideClickHandler(this, saveSelectorStateSamplingClick.bind(this));
            });

            filters.graphData.each(function () {
                overrideClickHandler(this, saveSelectorStateGraphDataClick.bind(this));
            });
        };

        var overrideClickHandler = function (target, beforeCallback) {
            // Save the original click handler
            var onClickHandler = target.onclick;

            target.onclick = function () {
                // execute the callback before the original handler
                beforeCallback();

                onClickHandler();
            };
        };

        // Module initialization
        var initialize = function () {
            findControls();
            createFilterHandlers();

            // There are no advanced controls in MVC
            if (advancedControls) {
                createMoreLessButtonHandler();
            }
        };

        initialize();
        setOnClickEventToButtons();
    };

    return ABOverview;
});
