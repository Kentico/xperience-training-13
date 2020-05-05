cmsdefine(['jQuery'], function ($) {
    var Module = function (data) {

        function init() {
            var $selectorRoot = $('#' + data.selectorId),
                $selectedIcon = $selectorRoot.find('.selected-icon'),
                $popupIconSelector = $selectorRoot.find('.popup-icon-selector'),
                $popupIcons = $popupIconSelector.find('i'),
                $noIconsMessage = $popupIconSelector.find('.no-icons'),
                $selectedIconTextBox = $selectedIcon.find('input'),
                $selectedIconPreviewButton = $selectedIcon.find('button'),
                $hiddenIconClassValue = $('#' + data.hiddenIconClassId);

            function setIconTextboxValue(iconClass) {
                $selectedIconTextBox.val(iconClass);
                setIconButton(iconClass);
            }

            function setIconButton(iconClass) {
                $selectedIconPreviewButton.find("i").attr('class', iconClass);

                // Store the selected icon class value in the hidden field to pass it to the server
                $hiddenIconClassValue.val(iconClass);
            }

            function showPopupDialog() {
                $noIconsMessage.hide();
                $popupIcons.show();
                $popupIconSelector.show();
            }

            function hidePopupDialog() {
                $popupIconSelector.hide();
            }

            // Display preselected icon
            var preselectedIconClass = $hiddenIconClassValue.val() || "";
            setIconTextboxValue(preselectedIconClass);

            // Events
            $selectedIconTextBox.click(function () {
                if (!$popupIconSelector.is(':visible')) {
                    showPopupDialog();
                }
                return false;
            });

            $selectedIconPreviewButton.click(function () {
                if (!$popupIconSelector.is(':visible')) {
                    showPopupDialog();
                }
                else {
                    hidePopupDialog();
                }
                return false;
            });

            $('body').click(function () {
                $popupIconSelector.hide();
            });

            $selectedIconTextBox.on('input', function () {
                $noIconsMessage.hide();

                var textBoxValue = this.value;
                if (textBoxValue) {
                    // Display only matching icons
                    $popupIcons.hide();
                    var matchingPopupIcons = $popupIconSelector.find('i[class*="' + textBoxValue + '"]');
                    matchingPopupIcons.show();

                    if (!matchingPopupIcons.length) {
                        $noIconsMessage.show();
                    }
                }
                else {
                    // Empty class text box
                    // Display all icons
                    $popupIcons.show();
                }

                // Show selected icon 
                setIconButton(textBoxValue);
            });

            $popupIcons.click(function () {
                var selectedIconClass = $(this).attr('class');

                // Display the selected icon CSS class name
                setIconTextboxValue(selectedIconClass);

                $popupIconSelector.hide();
            });
        };

        init();
    };

    return Module;
});