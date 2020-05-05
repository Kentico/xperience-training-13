//Because of backward compatibility
function ShowStrength(passwordID, minLength, preferedLength, minNonAlphaNumChars, preferedNonAlphaNumChars, regularExpression, passwordLabelID, policyStrings, classPrefix, usePolicy, indicatorPanelID, useStylesForStrenghtIndicator) {
    PASSWORDSTRENGTH.ShowStrength(passwordID, minLength, preferedLength, minNonAlphaNumChars, preferedNonAlphaNumChars, regularExpression, passwordLabelID, policyStrings, classPrefix, usePolicy, indicatorPanelID, useStylesForStrenghtIndicator)
}


//Module for computing and showing password strength.
var PASSWORDSTRENGTH = (function () {
    var alphaNum = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    var passElem = new Object;

    var NOT_ENOUGH_LENGTH = 0;
    var NOT_ENOUGH_ALFA_NUMERIC = 1;
    var REGULAR_EXPRESSION_FAILED = 2;


    return {
        getPassword: function (passwordID) {
            if (passwordID !== null) {
                return document.getElementById(passwordID);
            }
            // Used in tests
            return passElem;
        },

        //For testing purposes
        setPassword: function (passValue, passLength) {
            passElem.value = passValue;
            passElem.length = passLength;
        },


        //Compute and show password strength
        ShowStrength: function (passwordID, minLength, preferedLength, minNonAlphaNumChars, preferedNonAlphaNumChars, regularExpression, passwordLabelID, policyStrings, classPrefix, usePolicy, indicatorPanelID, useStylesForStrenghtIndicator) {
            var strings = policyStrings.split(';');
            var passElem = this.getPassword(passwordID);
            var labelElem = {};
            var indicatorElem = {};
            var indicatorClassPrefix = 'passw-indicator-';

            var passwordValue = passElem.value;
            var passwordLength = passwordValue.length;
            if (usePolicy === 'True') {
                labelElem.cls = classPrefix + 'not-acceptable';
                indicatorElem.cls = indicatorClassPrefix + 'not-acceptable';
                handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID);
            }
            else {
                labelElem.cls = classPrefix + 'weak';
                indicatorElem.cls = indicatorClassPrefix + 'weak';
                handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID);
            }

            // Minimal length
            if (minLength) {
                if (passwordLength === 0) {
                    indicatorElem.cls = '';
                    labelElem.innerHTML = '';
                    handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID);
                    return NOT_ENOUGH_LENGTH;
                }
                else if (passwordLength < parseInt(minLength)) {
                    labelElem.innerHTML = strings[0];
                    handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID)
                    return NOT_ENOUGH_LENGTH;
                }
            }

            // Number of non alpha characters        
            var nonAlphaNum = 0;
            if (minNonAlphaNumChars) {
                var nonAlphaNum = getNumberNonAlfaNum(passwordValue, passwordLength)
                if ((usePolicy === 'True') && (nonAlphaNum < parseInt(minNonAlphaNumChars))) {
                    labelElem.innerHTML = strings[0];
                    handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID)
                    return NOT_ENOUGH_ALFA_NUMERIC;
                }
            }

            // Test regular expressions
            if (regularExpression) {
                var re = new RegExp(regularExpression);
                if (!re.test(passwordValue)) {
                    labelElem.innerHTML = strings[0];
                    handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID)
                    return REGULAR_EXPRESSION_FAILED;
                }
            }

            var percentResult = countPasswordStrength(preferedLength, passwordLength, preferedNonAlphaNumChars, nonAlphaNum);

            // Set right string to label
            if (percentResult < 25) {
                labelElem.innerHTML = strings[1];
                labelElem.cls = classPrefix + 'weak';
                indicatorElem.cls = indicatorClassPrefix + 'weak';
                handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID);
                return percentResult;
            }
            else if (percentResult >= 25 && percentResult < 50) {
                labelElem.innerHTML = strings[2];
                labelElem.cls = classPrefix + 'acceptable';
                indicatorElem.cls = indicatorClassPrefix + 'acceptable';
                handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID);
                return percentResult;
            }
            else if (percentResult >= 50 && percentResult < 75) {
                labelElem.innerHTML = strings[3];

                labelElem.cls = classPrefix + 'average';
                indicatorElem.cls = indicatorClassPrefix + 'average';

                handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID);
                return percentResult;
            }
            else if (percentResult >= 75 && percentResult < 100) {
                labelElem.innerHTML = strings[4];
                labelElem.cls = classPrefix + 'strong';
                indicatorElem.cls = indicatorClassPrefix + 'strong';

                handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID);
                return percentResult;

            }
            else {
                labelElem.innerHTML = strings[5];
                labelElem.cls = classPrefix + 'excellent';
                indicatorElem.cls = indicatorClassPrefix + 'excellent';

                handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID);
                return percentResult;
            }
        }
    };


    function handleUi(labelElem, passwordLabelID, indicatorElem, indicatorPanelID) {
        //Handle UI - password label and password strength indicator.
        var passwordLabelElemnt = null;
        var indicatorElement = null;

        try {
            passwordLabelElemnt = document.getElementById(passwordLabelID);
            indicatorElement = document.getElementById(indicatorPanelID);

            passwordLabelElemnt.setAttribute('class', labelElem.cls);
            passwordLabelElemnt.innerHTML = labelElem.innerHTML;

            indicatorElement.setAttribute('class', indicatorElem.cls);
        }
        catch (ex) {
            //Exception is fired in test-scenario (elements aren't presented).
        };
    }


    // Count number of non alpha-numeric characters
    function getNumberNonAlfaNum(passwordValue, passwordLength) {
        var nonAlphaNum = 0;
        for (var i = 0; i < passwordLength; i++) {
            if (!isAlphaNum(passwordValue.charAt(i))) {
                nonAlphaNum++;
            }
        }
        return nonAlphaNum++;
    }


    // Count result strength  
    function countPasswordStrength(preferedLength, passwordLength, preferedNonAlphaNumChars, nonAlphaNum) {

        var onePercent = preferedLength / 100.0;
        var lenghtPercent = passwordLength / onePercent;

        onePercent = preferedNonAlphaNumChars / 100.0;
        var nonAlphaPercent = nonAlphaNum / onePercent;

        return (lenghtPercent + nonAlphaPercent) / 2;
    }


    // Returns whether character is alpha numeric 
    function isAlphaNum(param) {
        if (alphaNum.indexOf(param, 0) === -1) {
            return false;
        }
        return true;
    }
})();



