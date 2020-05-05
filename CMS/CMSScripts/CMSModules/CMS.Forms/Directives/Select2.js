/**
 * Angular directive that wraps jQuery Select2 plugin.
 * 
 * This directive handles creation of advanced dropdown with AJAX items loading and searching.
 * Dropdown requires a rest service for communication with server (see ISelectorController)
 * 
 * Attributes:
 *      modelItem - Model which holds current item object.
 *      restUrl - URL of rest service.
 *      restUrlParams - additional query parameters
 *      pageSize - Number of results in the page.
 *      dropdownParentId - Identifier of element which will contains the selector HTML code.
 *      placeholder - Default non-selectable 'please select' text in selector.
 *      resultTemplate - Function that returns HTML code for selectable item presentation.
 *      showAdditionalOption - Indicates whether the additional option with zero ID can be selectable (e.g. 'none' option).
 *      additionalOptionText - Sets the text of the additional option.
 */
cmsdefine(['Underscore', 'jQuery', 'select2'], function (_, $) {

    var select2Link = function ($timeout, resourceFilter) {

        // link function is executed per instance
        return function (scope, element) {
            var resolveFilter,
                $hiddenSelectElement,
                options = {},
                additionalOption = {
                    id: 0,
                    text: '(none)',
                    isAdditionalOption: true
                },

                isValidID = function (id) {
                    // Checks if 'id' of type string or int is positive integer greater than 0
                    var n = parseInt(id, 10);
                    return !isNaN(n) && (n > 0);
                },

                defaultTemplate = function (data) {
                    // Just escape HTML
                    return _.escape(data.text);
                },

                getDropdownParent = function (dropdownParentId) {
                    var dropdownParent;

                    if (dropdownParentId) {
                        dropdownParent = $('#' + dropdownParentId);
                        if (!dropdownParent || !dropdownParent.length) {
                            // Try top frame if element not found
                            dropdownParent = $('#' +dropdownParentId, window.top.document);
                        }
                    }
                    return dropdownParent;
                },

                initOptions = function () {
                    options = {
                        restUrl: scope.restUrl,
                        restUrlParams: scope.restUrlParams || {},
                        preSelectedID: scope.modelItem ? scope.modelItem.id : undefined,
                        showAdditionalOption: scope.showAdditionalOption,
                        pageSize: scope.pageSize,
                        dropdownParentElement: getDropdownParent(scope.dropdownParentId),
                        placeholder: scope.showAdditionalOption ? additionalOption.text : scope.placeholder || resolveFilter('general.select'),
                        defaultTemplate: defaultTemplate,
                        resultTemplate: scope.resultTemplate || defaultTemplate
                    }
                },

                processDataResults = function (data, params) {
                    var pageSize = Number(options.pageSize) || 10,
                        $parentElement = options.dropdownParentElement || $(window.document.body);

                    params.page = params.page || 0;

                    if (options.showAdditionalOption && data.length && (params.page === 0)) {
                        data.unshift(additionalOption);
                    }

                    if (data.length >= pageSize) {
                        // Show search-box for more than pageSize+ items
                        $parentElement.find('.select2-search--dropdown').css('display', 'block');
                        $parentElement.find('.select2-search--dropdown input').focus();
                    }

                    return {
                        results: data,
                        pagination: {
                            more: (data.length > (pageSize - 1))
                        }
                    };
                },

                initSelect2Options = function () {
                    return {
                        placeholder: options.placeholder,
                        dropdownParent: options.dropdownParentElement || undefined,
                        width: '100%',
                        language: {
                            loadingMore: function () {
                                return resolveFilter('general.loading.more');
                            },
                            errorLoading: function () {
                                return resolveFilter('general.loading.error');
                            },
                            noResults: function () {
                                return resolveFilter('general.noresults');
                            },
                            searching: function () {
                                return resolveFilter('general.searching');
                            }
                        },
                        templateResult: function (data) {
                            return (data.loading || data.isAdditionalOption)
                                ? options.defaultTemplate(data)
                                : options.resultTemplate(data);
                        },
                        templateSelection: defaultTemplate,
                        escapeMarkup: function (markup) { return markup; },

                        ajax: {
                            url: options.restUrl,
                            dataType: 'json',
                            data: function (params) {
                                var queryParams = {
                                    name: params.term || '', // search term
                                    pageIndex: params.page || 0,
                                    pageSize: options.pageSize
                                };

                                return _.extend({}, options.restUrlParams, queryParams);
                            },
                            delay: 200,
                            processResults: processDataResults
                        }
                    };
                },

                setItem = function(item) {
                    scope.modelItem = item;
                    scope.$apply();
                },

                /**
                * Sets the current option displayed in the dropdown.
                */
                setOptionText = function (text, id) {
                    var $newOption = $('<option/>').text(text).val(id);

                    $hiddenSelectElement.find('option').remove();
                    $hiddenSelectElement.append($newOption);           
                    $hiddenSelectElement.val(id);

                    $timeout(function () {
                        $hiddenSelectElement.trigger('change');
                    });
                },

                /**
                * Handles the AJAX request to fetch object in edit mode.
                */
                preselectItem = function (id) {
                    // create loading indicator
                    setOptionText(resolveFilter('general.loading.more'), additionalOption.id);
                    var queryData = _.extend({}, options.restUrlParams, { ID: id });

                    $.ajax({
                        url: options.restUrl,
                        dataType: 'json',
                        data: queryData,
                        error: function () {
                            setOptionText(additionalOption.text, additionalOption.id);
                            setItem(additionalOption);
                        }
                    }).then(function (data) {
                        setOptionText(data.text, data.id);
                        setItem(data);
                    });
                },

                /**
                * Ensures pre-selection of current item or '(none)' placeholder.
                */
                ensurePreSelectedItems = function () {
                    if (isValidID(options.preSelectedID)) {
                        // fetch current item
                        preselectItem(options.preSelectedID);

                    } else {
                        // Set '(none)' item.
                        setOptionText(options.placeholder, additionalOption.id);
                        setItem(additionalOption);
                    }
                },

                /**
                 * Initialize select2 plugin and select2 configuration.
                 */
                initSelect2 = function () {
                    $hiddenSelectElement = $(element);
                    resolveFilter = resourceFilter;
                    additionalOption.text = scope.additionalOptionText || resolveFilter('general.empty');

                    initOptions();

                    $hiddenSelectElement.on('select2:select', function (eventData) {
                        setItem(eventData.params.data);
                    });

                    // Init select2 jQuery plugin
                    $timeout(function () {
                        $hiddenSelectElement.select2(initSelect2Options());
                        ensurePreSelectedItems();
                    });
                };

            initSelect2();
        };
    },

        directive = function ($timeout, resourceFilter) {
            return {
                restrict: 'A',
                scope: {
                    modelItem: '=',
                    resultTemplate: '<',
                    restUrl: '@',
                    restUrlParams: '<',
                    pageSize: '@',
                    dropdownParentId: '@',
                    placeholder: '@',
                    showAdditionalOption: '<',
                    additionalOptionText: '@'
                },
                link: select2Link($timeout, resourceFilter)
            };
        };

    return ['$timeout', 'resolveFilter', directive];
});
