cmsdefine(['Underscore', 'CMS/BootstrapTooltip'], function (_, registerBootstrapTooltip) {

    var Controller = function ($scope, $element, $timeout) {

        // This code is needed to make tooltips work. jQuery is binding the tooltip elements on document load, 
        // but when tooltips are used in angular app, they are added to the DOM later.
        registerBootstrapTooltip({
            selector: '.info-icon > i'
        });

        var that = this;
        this.$scope = $scope;
        this.fileReference = null;
        this.$timeout = $timeout;
        this.$scope.fileName = this.$scope.fileName || 'file.txt';
        this.$scope.contentType = this.$scope.contentType || 'text/plain';

        this.$scope.offerCSVToDownload = function () {
            var ie = navigator.userAgent.match(/MSIE\s([\d.]+)/),
                ie11 = navigator.userAgent.match(/Trident\/7.0/) && navigator.userAgent.match(/rv:11/),
                edge = navigator.userAgent.match(/Edge/),
                ieVer = (ie ? ie[1] : (ie11 ? 11 : -1)),
                safari = navigator.userAgent.indexOf("Safari"),
                fileNameToSaveAs = that.$scope.fileName + (ie ? '.txt' : ''),
                textFileAsBlob,
                downloadLink;

            if (ie && ieVer < 10) {
                // If user is here something weird happened since she is not able to import anything because she has no FileStreamer.
                return;
            }

            textFileAsBlob = new Blob([that.$scope.toDownload], {
                type: ie || safari ? 'text/plain' : that.$scope.contentType
            });

            if (ie || ie11 || edge) {
                window.navigator.msSaveBlob(textFileAsBlob, fileNameToSaveAs);
            } else {
                downloadLink = document.createElement('a');
                downloadLink.download = fileNameToSaveAs;
                downloadLink.innerHTML = fileNameToSaveAs; // Text is not visible but is required

                if (safari) {
                    downloadLink.setAttribute('target', '_blank');
                }

                if (window.webkitURL) {
                    // Chrome allows the link to be clicked
                    // without actually adding it to the DOM.
                    downloadLink.href = window.webkitURL.createObjectURL(textFileAsBlob);
                } else {
                    // Firefox requires the link to be added to the DOM
                    // before it can be clicked.
                    downloadLink.href = window.URL.createObjectURL(textFileAsBlob);
                    downloadLink.onclick = function () { this.parentNode.removeChild(this); };
                    downloadLink.style.display = 'none';
                    document.body.appendChild(downloadLink);
                }

                downloadLink.click();
            }
        };
    },
            directive = function () {
                return {
                    restrict: 'A',
                    scope: {
                        toDownload: '=',
                        fileName: '@?',
                        contentType: '@?'
                    },
                    replace: true,
                    templateUrl: 'downloadDataDirectiveTemplate.html',
                    controller: [
                        '$scope',
                        '$element',
                        '$timeout',
                        Controller],
                };
            };

    return [directive];
});