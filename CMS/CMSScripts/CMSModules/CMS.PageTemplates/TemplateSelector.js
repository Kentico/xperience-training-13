cmsdefine(['jQuery', 'Underscore', 'CMS/ClientLocalization', 'CMS/MessageService', 'CMS/UrlHelper'], function ($, _, localization, messaging, urlHelper) {

    return function (serverData) {
        var headerActionsDiv = document.getElementById(serverData.headerActionsDivId);
        var continueButton = document.getElementById(serverData.continueButtonId);
        var defaultTemplatesDiv = document.getElementById(serverData.defaultTemplatesDivId);
        var customTemplatesDiv = document.getElementById(serverData.customTemplatesDivId);
        var serviceUrl = serverData.serviceUrl;
        var redirectionUrl = serverData.redirectionUrl;        
        var sitePresentationUrl = serverData.sitePresentationUrl;
        var defaultImageUrlForCustomTemplates = serverData.defaultImageUrlForCustomTemplates;

        var DEFAULT_ICON_CLASS = "icon-l-header-cols-2-footer";

        var TEMPLATE =
            '<div class="FlatItem template-item" data-identifier="<%= identifier %>">' +
                '<div class="SelectorEnvelope" title="<%- title %>">' +
                    '<div class="SelectorFlatImage">' +
                        '<%= thumbnail %>' +
                    '</div>' +
                    '<span class="SelectorFlatText"><%- name %></span>' +
                '</div>' +
            '</div>';

        var CUSTOM_TEMPLATE =
            '<div class="FlatItem template-item" data-identifier="<%= identifier %>">' +
                '<div class="SelectorEnvelope EnvelopeBig" title="<%- title %>">' +
                    '<div class="SelectorFlatImage">' +
                        '<%= thumbnail %>' +
                    '</div>' +
                    '<span class="SelectorFlatText"><%- name %></span>' +
                '</div>' +
            '</div>';

        function getPageTemplates() {
            displayLoader();
            $.ajax({
                url: serviceUrl,
                method: 'GET',
                contentType: 'application/json',
                success: function (data) {
                    processTemplates(data.defaultPageTemplates, data.customPageTemplates);
                },
                error: function () {
                    processError();
                },
                complete: function () {
                    hideLoader();
                }
            });
        }

        function registerEventListeners() {
            $(continueButton).click(handleContinueClick);
        }
               
        function handleContinueClick() {
            var selectedTemplateIdentifier = "";
            var selectedDefaultTemplate = $(defaultTemplatesDiv).find('.template-item.FlatSelectedItem');
            var selectedCustomTemplate = $(customTemplatesDiv).find('.template-item.FlatSelectedItem');

            if (selectedDefaultTemplate.length > 0) {
                selectedTemplateIdentifier = $(selectedDefaultTemplate[0]).data('identifier');
                redirectionUrl = addTemplateIdentifierAndTypeToQueryString(redirectionUrl, selectedTemplateIdentifier, 'default');
            }
            else if (selectedCustomTemplate.length > 0) {
                selectedTemplateIdentifier = $(selectedCustomTemplate[0]).data('identifier');
                redirectionUrl = addTemplateIdentifierAndTypeToQueryString(redirectionUrl, selectedTemplateIdentifier, 'custom');
            }
            else {
                messaging.showError(localization.getString('pagetemplatesmvc.notemplateselected'));
                return;
            }

            redirect(redirectionUrl);
        }

        function addTemplateIdentifierAndTypeToQueryString(redirectionUrl, templateIdentifier, type) {
            return urlHelper.addParameterToUrl(
                urlHelper.addParameterToUrl(redirectionUrl, 'templateidentifier', templateIdentifier),
                'templateType',
                type);
        }

        function getImageTag(imagePath) {
            var imageUrl = imagePath.replace('~', sitePresentationUrl);
            return '<img src="' + imageUrl + '" class="selector-image" />';
        }
               
        function getIconTag(iconClass) {
            return '<i class="' + iconClass + '" aria-hidden="true"></i>';
        }

        function selectTemplate($element) {
            $([defaultTemplatesDiv, customTemplatesDiv]).find('.template-item.FlatSelectedItem').each(function () {
                $(this).removeClass('FlatSelectedItem');
                $(this).addClass('FlatItem');
            });

            $element.addClass('FlatSelectedItem');
            $element.removeClass('FlatItem');
        }

        function getThumbnailForDefaultTemplate(templateItem) {
            return templateItem.iconClass !== null ? getIconTag(templateItem.iconClass) : getIconTag(DEFAULT_ICON_CLASS);
        }

        function getThumbnailForCustomTemplate(templateItem) {
            return templateItem.imagePath !== null ? getImageTag(templateItem.imagePath) : getImageTag(defaultImageUrlForCustomTemplates);
        }

        function renderTemplates(pageTemplates, templatesDiv, getThumbnail, template) {
            if (pageTemplates.length === 0) {
                return;
            }

            $(templatesDiv).removeClass("hidden");

            var result = "";
            pageTemplates.forEach(function (item) {
                var html = _.template(template, {
                    identifier: item.identifier,
                    thumbnail: getThumbnail(item),
                    name: item.name,
                    title: item.description
                });

                result += html;
            });

            templatesDiv.querySelector(".SelectorFlatItems").insertAdjacentHTML('afterbegin', result);
            $(templatesDiv).find('.template-item').each(function () {
                var $that = $(this);
                $that.on("click", function () { selectTemplate($that); });
            });
        }

        function processTemplates(defaultTemplates, customTemplates) {
            if (defaultTemplates.length === 0 && customTemplates.length === 0) {
                redirectionUrl = urlHelper.addParameterToUrl(redirectionUrl, 'noTemplate', true);
                redirect(redirectionUrl);
                return;
            }

            // If only a single template is available => select it and redirect to the next page
            if (defaultTemplates.length + customTemplates.length === 1) {
                if (defaultTemplates.length === 1) {
                    redirectionUrl = addTemplateIdentifierAndTypeToQueryString(redirectionUrl, defaultTemplates[0].identifier, 'default');
                }
                else if (customTemplates.length === 1) {
                    redirectionUrl = addTemplateIdentifierAndTypeToQueryString(redirectionUrl, customTemplates[0].identifier, 'custom');
                }

                redirect(redirectionUrl);
                return;
            }


            // show continue button
            $(headerActionsDiv).removeClass('hidden');

            renderTemplates(defaultTemplates, defaultTemplatesDiv, getThumbnailForDefaultTemplate, TEMPLATE);
            renderTemplates(customTemplates, customTemplatesDiv, getThumbnailForCustomTemplate, CUSTOM_TEMPLATE);
            trimTemplateNames();
        }

        function processError() {            
            messaging.showError(localization.getString('pagetemplatesmvc.requesterror'));            
        }

        function displayLoader() {
            if (window.Loader) {
                window.Loader.show();
            }
        }

        function hideLoader() {
            if (window.Loader) {
                window.Loader.hide();
            }
        }

        function redirect(url) {
            $(location).attr('href', url);
        }

        function trimTemplateNames() {
            var elems = document.getElementsByClassName('SelectorFlatText');
            for (var i = 0; i < elems.length; i++) {
                lineClamp(elems[i], 3);
            }
        }

        function lineClamp(element, lines) {
            const originalText = element.innerHTML;
            const truncationChar = '…';
            const splitOnChars = ['.', '-', '–', '—', ' ', '_'].slice(0);

            let splitChar = splitOnChars[0];
            let chunks;
            let lastChunk;

            function truncate(target, maxHeight) {
                const text = originalText.replace(truncationChar, '');

                // Grab the next chunks
                if (!chunks) {
                    // If there are more characters to try, grab the next one
                    if (splitOnChars.length > 0) {
                        splitChar = splitOnChars.shift();
                    } else {
                        splitChar = '';
                    }

                    chunks = text.split(splitChar);
                }

                // If there are chunks left to remove, remove the last one and see if the text fits
                if (chunks.length > 1) {
                    lastChunk = chunks.pop();
                    applyEllipsis(target, chunks.join(splitChar));
                } else {
                    chunks = null;
                }

                // Search produced valid chunks
                if (chunks) {
                    // It fits
                    if (getElementHeight(target) <= maxHeight) {
                        // There's still more characters to try splitting on, not quite done yet
                        if (splitChar !== '') {
                            applyEllipsis(target, chunks.join(splitChar) + splitChar + lastChunk);
                            chunks = null;
                        } else {
                            return;
                        }
                    }
                }

                truncate(target, maxHeight);
            }

            function applyEllipsis(elem, str) {
                elem.innerHTML = str + truncationChar;
            }

            const height = parseInt(getComputedStyle(element).getPropertyValue('line-height'), 10) * lines;
            if (height < getElementHeight(element)) {
                truncate(element, height);
            }
        }

        function getElementHeight(element) {
            return parseInt(getComputedStyle(element).getPropertyValue('height'), 10);
        }

        getPageTemplates();
        registerEventListeners();
    };
});
