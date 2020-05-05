cmsdefine(['CMS/WebFormCaller'], function (webFormCaller) {

    var Module = function(data) {
        var received = 0,
            callBackParam,
            asyncProcessFinished = false,
            timeout = null,
            asyncBusy = false,
            config = data,
            warnings = '',
            errors = '',
            infos = '',
            
            setNextTimeout = function (ms) {
                if (ms == null) {
                    ms = 200;
                }

                if (!asyncProcessFinished) {
                    timeout = setTimeout(getAsyncStatus, ms);
                }
            },

            getAsyncStatus = function() {
                if (!asyncBusy) {
                    try {
                        asyncBusy = true;
                        setTimeout('asyncBusy = false;', 2000);
                        callBackParam = received + '|' + config.machineName;
                        var cbOptions = {
                            targetControlUniqueId: config.uniqueId,
                            args: callBackParam,
                            successCallback: receiveAsyncStatus,
                            errorCallback: receiveError
                        };
                        webFormCaller.doCallback(cbOptions);
                    } catch (ex) {
                        setNextTimeout();
                        throw ex;
                    }
                }

                if (asyncProcessFinished) {
                    cancel(false);
                    return;
                }

                setNextTimeout();
            },

            setClose = function() {
                var cancelElem = document.getElementById(config.id + '_btnCancel');
                if (cancelElem != null) {
                    cancelElem.value = config.closeText;
                }
            },

            receiveError = function () {
                setNextTimeout();
            },

            receiveAsyncStatus = function(rvalue) {
                var totalReceived, i, resultValue, values, code;

                asyncBusy = false;
                if (asyncProcessFinished) {
                    return;
                }

                values = rvalue.split('|');

                code = values[0];
                totalReceived = parseInt(values[1]);

                resultValue = '';

                for (i = 2; i < values.length; i++) {
                    resultValue += values[i];
                }

                if (resultValue != '') {
                    setLog(resultValue, totalReceived);
                }

                if (code == 'running') {
                    // Keep running, no extra actions
                } else if (code == 'finished') {
                    if (errors == '') {
                        finish();
                    } else {
                        finishWithErrors();
                    }
                } else if ((code == 'threadlost') || (code == 'stopped')) {
                    processFinished();
                    setClose();
                } else if (code == 'error') {
                    finishWithErrors();
                }
            },

            finish = function () {
                processFinished();

                if (config.postbackOnFinish) {
                    webFormCaller.doPostback({
                        targetControlUniqueId: config.uniqueId,
                        args: 'finished|' + config.machineName
                    });
                } else {
                    setClose();
                }
            },

            finishWithErrors = function() {
                processFinished();

                if (config.postbackOnError) {
                    webFormCaller.doPostback({
                        targetControlUniqueId: config.uniqueId,
                        args: 'error|' + config.machineName
                    });
                } else {
                    setClose();
                }
            },

            processFinished = function () {
                asyncProcessFinished = true;

                if (config.finishClientCallback) {
                    var cb = window[config.finishClientCallback];
                    cb(window.CMS['AC_' + config.id]);
                }
            },

            setLog = function(text, totalReceived) {
                var elem = document.getElementById(config.logId),
                    lines, line, i, log = '';

                received = totalReceived;
                lines = text.split('\n');

                if (config.reversed) {
                    lines = lines.reverse();
                }

                // Process lines
                for (i = 0; i < lines.length; i++) {
                    line = lines[i];
                    if (line != '') {
                        if (line.indexOf("##ERROR##") == 0) {
                            line = line.substr(9);
                            errors = addLine(errors, line);
                        }
                        else if (line.indexOf("##WARNING##") == 0) {
                            line = line.substr(11);
                            warnings = addLine(warnings, line);
                        }
                        else {
                            infos = addLine(infos, line);
                        }

                        log = addLine(log, line);
                    }
                }

                // Set log in output placeholder
                if (log != '') {
                    var node = document.createElement("div");
                    node.innerHTML = log;

                    if ((elem.childNodes.count < 1) || !config.reversed) {
                        elem.appendChild(node);
                    } else {
                        elem.insertBefore(node, elem.childNodes[0]);
                    }
                }
            },

            addLine = function(log, line) {
                if (log != '') {
                    log += '<br />' + line;
                } else {
                    log += line;
                }

                return log;
            },
            
            cancel = function(withPostback) {
                var t;

                asyncProcessFinished = true;
                if (withPostback) {
                    webFormCaller.doPostback({
                        targetControlUniqueId: config.cancelButtonUniqueId
                    });
                } else {
                    t = timeout;
                    if ((t != 'undefined') && (t != null)) {
                        clearTimeout(timeout);
                    }
                }
            }

        window.CMS = window.CMS || {};

        setNextTimeout(1);

        return window.CMS['AC_' + config.id] = {
            cancel: cancel,
            getWarnings: function ( ) { return warnings; },
            getErrors: function ( ) { return errors; },
            getInfos: function ( ) { return infos; },
        };
    }

    return Module;
});