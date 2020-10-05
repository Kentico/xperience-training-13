/**
 * Module for the basic Unigrid functionality.
 * This module handles Unigrid commands, sorting and multiple item selection with hash validation.
 *
 * @param {string} data.id - Unigrid control client ID.
 * @param {string} data.uniqueId - Unigrid control unique ID.
 * @param {string} data.gridId - Unigrid table ID.
 * @param {string} data.hdnSelId - Hidden field ID which is used to store selected items IDs.
 * @param {string} data.hdnSelHashId - Hidden field ID which is used to store selected items hash.
 * @param {string} data.hdnCmdNameId - Hidden field ID which is used to identify the command name.
 * @param {string} data.hdnCmdArgId - Hidden field ID which is used to identify the command argument.
 * @param {boolean} data.resetSelection - Indicates if the selection should be cleared.
 * @param {boolean} data.allowSorting - Indicates if the grid should be drag & drop sortable.
 * @param {boolean} data.allowShiftKeySelection - Indicates if the grid should allow selection of multiple items using Shift key.
 */
cmsdefine(['CMS/WebFormCaller', 'jQuery'], function (webFormCaller, $) {

    var Module = function (data) {
        var config = data,
            $headerBox = $('#' + config.gridId + " input[name$='headerBox']"),
            $checkBoxes = $('#' + config.gridId + " :checkbox[data-arg]"),
            $selectionIDs = $('#' + config.hdnSelId),
            $selectionHash = $('#' + config.hdnSelHashId),
            $commandName = $('#' + config.hdnCmdNameId),
            $commandArg = $('#' + config.hdnCmdArgId),

        redir = function (url) {
            document.location.replace(url);
            return false;
        },

        postback = function (arg) {
            webFormCaller.doPostback({
                targetControlUniqueId: config.uniqueId,
                args: arg
            });
        },

        showFilter = function () {
            postback('ShowFilter');
        },

        reset = function () {
            postback('Reset');
        },

        reload = function () {
            postback('Reload');
        },

        command = function (name, arg) {
            $commandName.val(name);
            $commandArg.val(arg);
            postback('UniGridAction');
            return false;
        },

        destroy = function (arg) {
            command('#destroyobject', arg);
        },

        setHash = function (hash) {
            $selectionHash.val(hash);
        },

        updateHeaderCheckbox = function () {
            var checked = $checkBoxes.filter(":checked").length === $checkBoxes.length;
            $headerBox.prop('checked', checked);
        },

        /**
         * Updates the selectionIDs hidden filed and selection hash when checkbox is clicked.
         *
         * @param {Element} checkBox - Clicked checkbox element.
         * @param {boolean} [skipCallback] - Indicates if callback to retrieve new selection hash should be skipped (used in case of multiple selection).
         */
        select = function (checkBox, skipCallback) {
            var $checkBox = $(checkBox),
                valueIDs = $selectionIDs.val(),
                arg = $checkBox.data('arg'),
                argHash = $checkBox.data('arghash');

            if (!valueIDs) {
                valueIDs = '|';
            }

            if (checkBox.checked) {
                valueIDs += arg + '|';
            }
            else {
                valueIDs = valueIDs.replace('|' + arg + '|', '|');
            }

            $selectionIDs.val(valueIDs);

            if (!skipCallback) {
                webFormCaller.doCallback({
                    targetControlUniqueId: config.uniqueId,
                    args: valueIDs + ':' + arg + '#' + argHash,
                    successCallback: setHash
                });
            }

        },

        /**
         * Updates selectionIDs when multiple items should be checked.
         *
         * @param {Element|boolean} selAll - Header checkbox element (select all) or Boolean value to identify if checkboxes should be set or cleared.
         * @param {jQuery} [$selectCheckBoxes] - JQuery object containing checkboxes which should be selected or cleared. $checkBoxes are used when the parameter is not specified.
         */
        selectAll = function (selAll, $selectCheckBoxes) {
            var callBackArgument = ':',
                isCheck = (typeof selAll === 'boolean') ? selAll : selAll.checked;

            if (!$selectCheckBoxes) {
                $selectCheckBoxes = $checkBoxes;
            }

            $selectCheckBoxes.each(function (i, chkBox) {
                if (chkBox.checked !== isCheck && chkBox.disabled !== true) {
                    chkBox.checked = isCheck;
                    select(chkBox, true);
                    callBackArgument += $(chkBox).data('arg') + '#' + $(chkBox).data('arghash') + ':';
                }
            });

            webFormCaller.doCallback({
                targetControlUniqueId: config.uniqueId,
                args: $selectionIDs.val() + callBackArgument,
                successCallback: setHash
            });
        },

        clearSelection = function () {
            $checkBoxes.each(function (i, e) {
                e.checked = false;
            });

            $selectionIDs.val('');
            setHash('');
        },

        checkSelection = function () {
            var valueIDs = $selectionIDs.val();
            return ((!valueIDs) || (valueIDs === '|'));
        },

        initMove = function (arg) {
            $commandArg.val(arg);
        },

        initSorting = function () {
            var gridBody = $('#' + config.id + ' tbody');
            gridBody.sortable({
                axis: 'y',
                cancel: '',
                containment: 'parent',
                cursor: 'move',
                handle: 'button.js-_move',
                helper: fixWidthHelper,
                tolerance: 'pointer',
                items: '> *:not(.unsortable)',
                start: function (event, ui) {
                    $(this).data('previndex', ui.item.index());
                    fixContainmentHeight(this, ui);
                },
                update: function (event, ui) {
                    var previousIndex = $(this).data('previndex');
                    var newIndex = ui.item.index();
                    command('#move', $commandArg.val() + ':' + previousIndex + ':' + newIndex);
                }
            }).disableSelection();

            function fixWidthHelper(e, ui) {
                ui.children().each(function () {
                    $(this).width($(this).width());
                });
                return ui;
            }

            function fixContainmentHeight(self, ui) {
                var sortableInstance = $(self).data()['ui-sortable'];
                var draggedItemHeight = ui.helper.height();
                sortableInstance.containment[3] += draggedItemHeight * 2 - ui.placeholder.height();
                ui.placeholder.height(draggedItemHeight);
            }
        },

        /**
         * Initializes multiple checkbox selection/clearing with pressed SHIFT button.
         */
        initShiftSelection = function () {
            var $selectionCells = $('#' + config.gridId + ' td.unigrid-selection'),
                $lastChecked,
                startIndex,
                endIndex,
                $currCheckBox,
                $checkBoxesFromTo,
                $innerCheckBoxes,
                isClear;

            // Disable default-shift selection
            $selectionCells.on('selectstart', false);

            // Stop the default click event when shift is pressed
            $selectionCells.find('label').click(function (e) {
                if (e.shiftKey) {
                    e.preventDefault();
                }
            });

            // Bind the click event to selection cells (FF is blocking SHIFT click on labels)
            $selectionCells.click(function (e) {
                $currCheckBox = $(this).find(':checkbox');

                if (e.shiftKey) {
                    if (!$lastChecked) {
                        $lastChecked = $currCheckBox;
                    }

                    startIndex = $checkBoxes.index($currCheckBox);
                    endIndex = $checkBoxes.index($lastChecked);
                    $checkBoxesFromTo = $checkBoxes.slice(Math.min(startIndex, endIndex), Math.max(startIndex, endIndex) + 1);
                    $innerCheckBoxes = $checkBoxesFromTo.slice(1, $checkBoxesFromTo.length - 1);
                    isClear = (($checkBoxesFromTo.length - 2) === $innerCheckBoxes.filter(':checked').length) && $currCheckBox.prop('checked');

                    selectAll(!isClear, $checkBoxesFromTo);
                    updateHeaderCheckbox();

                    $currCheckBox.focus();
                }

                $lastChecked = $currCheckBox;
            });
        };

        if (config.resetSelection) {
            clearSelection();
        }

        if (config.allowSorting) {
            initSorting();
        }

        updateHeaderCheckbox();
        if (config.allowShiftKeySelection) {
            initShiftSelection();
        }

        window.CMS = window.CMS || {};

        return window.CMS['UG_' + config.id] = {
            clearSelection: clearSelection,
            checkSelection: checkSelection,
            reload: reload,
            command: command,
            destroy: destroy,
            selectAll: selectAll,
            select: select,
            updateHeaderCheckbox: updateHeaderCheckbox,
            redir: redir,
            reset: reset,
            showFilter: showFilter,
            initMove: initMove
        };
    }

    return Module;
});