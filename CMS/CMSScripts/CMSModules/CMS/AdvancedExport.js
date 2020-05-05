cmsdefine(['CMS/WebFormCaller', 'jQuery'], function (webFormCaller, $) {

    var Module = function(data) {
        var config = data,
            numVal = 'none',
            colVal = 'none',
            
            postback = function(id, arg) {
                webFormCaller.doPostback({
                    targetControlUniqueId: id,
                    args: arg
                });
            },
            
            ugExport = function(format) {
                $('#' + config.hdnParamId).val(format);

                if (format === 'advancedexport') {
                    postback(config.uniqueId, format);
                } else {
                    postback(config.btnFullPostbackUniqueId, '');
                }
            },
            
            setChecked = function(checked) {
                $('#' + config.chlColumnsId + ' :checkbox').prop('checked', checked);
                return false;
            },
            
            checkAll = function() {
                return setChecked(true);
            },
            
            uncheckAll = function() {
                return setChecked(false);
            },

            defaultSelection = function () {
                var checkBoxes = $('input[type="checkbox"]', '#' + config.chlColumnsId);
                var defSelStr = $('#' + config.hdnDefaultSelectionId).val();
                var defaultSel = defSelStr.split(',');

                for (var i = 0; i < checkBoxes.length; i++) {
                    var indexOfChk = $.inArray(i.toString(), defaultSel);
                    $(checkBoxes[i]).prop('checked', (indexOfChk > -1));
                }
                return false;
            },

            fixDialogHeight = function () {
                var numberValidator = $('#' + config.revRecordsId);
                var columnValidator = $('#' + config.cvColumnsId);
                if (numberValidator && columnValidator) {
                    var process = false;
                    if (numVal !== numberValidator.css('display')) {
                        process = true;
                    }
                    numVal = numberValidator.css('display');
                    if (colVal !== columnValidator.css('display')) {
                        process = true;
                    }
                    colVal = columnValidator.css('display');
                    if (process) {
                        resizableDialog = true;
                        keepDialogAccesible(config.mdlAdvancedExportId);
                    }
                }
                setTimeout(fixDialogHeight, 500);
            },
            
            validateExport = function (source, arguments) {
                var checked = $cmsj('input[type="checkbox"]:checked','#' + config.chlColumnsId);
                arguments.IsValid = (checked.length > 0);
            };

        if (config.fixHeight) {
            setTimeout(fixDialogHeight, 500);
        }

        if (config.alertMessage) {
            setTimeout(function () { alert(config.alertMessage); }, 50);
        }

        window.CMS = window.CMS || {};

        return window.CMS['UG_Export_' + config.unigridId] = {
            checkAll: checkAll,
            uncheckAll: uncheckAll,
            ugExport: ugExport,
            defaultSelection: defaultSelection,
            validateExport: validateExport
        };
    };

    return Module;
});
