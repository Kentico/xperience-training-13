cmsdefine(['CMS/EventHub', 'jQuery'], function (EventHub, $cmsj) {

    // Create function constructor
    var Module = function (serverData) {
        this.btnCreateTaskGroup = $cmsj('#' + serverData.btnCreateTaskGroup);
        this.taskGroupSelector = $cmsj('#' + serverData.taskGroupSelector);
        this.lnkEditTaskGroup = $cmsj('#' + serverData.lnkEditTaskGroup);
        this.stagingTaskGroupMenuDropdown = $cmsj('#' + serverData.stagingTaskGroupMenuDropdown);


        // When task group is deleted from UI repair selector
        // Subscribes to event from module TaskGroupListExtender.js
        EventHub.subscribe('TaskGroupDeleted', function (parameters) {
            if (getSelectedOption().val() == parameters.taskGroupDeletedID) {
                $cmsj('#' + serverData.taskGroupSelector)[0].value = 0;
                changeMenuTitle(getSelectedOption());

                    // Call server to change task group for user to none, if the current group was deleted, pretending that selector was changed to none
                var data = { ID: serverData.noneOption, Control: serverData.taskGroupSelector };
                taskGroupMenuCallback(JSON.stringify(data));
            }

            var option = $cmsj('#' + serverData.taskGroupSelector).find('option[value=' + parameters.taskGroupDeletedID + ']');
            option.remove();
        });


        // When task group is saved from UI repair selector
        // Subscribes to event from module TaskGroupNewEditExtender.js
        EventHub.subscribe('TaskGroupSaved', function (parameters) {
            var selector = $cmsj('#' + serverData.taskGroupSelector);
            var option = selector.find('option[value=' + parameters.taskGroupID + ']');

            // Rename existing option if changed
            if (option.length) {
                    option.text(parameters.taskGroupName);
            } else {
                selector[0].options.add(new Option(parameters.taskGroupName, parameters.taskGroupID));
                $cmsj('#' + serverData.taskGroupSelector)[0].disabled = false;
            }

            // If name of selected option has been changed call server, as if the option has been changed so the menu
            // will be correctly pre-rendered.
            if (getSelectedOption().val() == parameters.taskGroupID) {
                var data = { ID: parameters.taskGroupID, Control: serverData.taskGroupSelector };
                taskGroupMenuCallback(JSON.stringify(data));
            }
        });


        // Handles closing the menu event
        this.stagingTaskGroupMenuDropdown.on('hidden.bs.dropdown', function () {
           infoCodeNameMessage();
        });


        // Event when dropdown is shown, sets focus on element by selected task group
        this.stagingTaskGroupMenuDropdown.on('shown.bs.dropdown', function () {
            if (getSelectedOption().val() == serverData.noneOption && $cmsj('#' + serverData.inputTaskGroup).length) {
                setFocusOnElement($cmsj('#' + serverData.inputTaskGroup));
            } else {
                setFocusOnElement($cmsj('#' + serverData.taskGroupSelector));
            }
        });


        // Takes value from input and creates task group
        this.btnCreateTaskGroup.on('click', function (event) {
            var groupName = $cmsj('#' + serverData.inputTaskGroup).val();
            var pattern = new RegExp('^(?:[A-Za-z0-9_\\-]+)(?:\\.[A-Za-z0-9_\\-]+)*$');
            if (pattern.test(groupName)) {
                $cmsj('#' + serverData.inputTaskGroup).val('');
                if (groupName.length !== 0) {
                    var data = { Name: groupName, Control: serverData.btnCreateTaskGroup };
                    taskGroupMenuCallback(JSON.stringify(data));
                    $cmsj('#' + serverData.taskGroupSelector)[0].disabled = false;
                }

                closeMenu();
            } else {
                $cmsj('#' + serverData.codeNameMessage).addClass('dropdown-menu-item-error-message');
                $cmsj('#' + serverData.codeNameMessage).removeClass('dropdown-menu-item-info-message');
            }
            return false;
        });


        // Changes current group on server
        this.taskGroupSelector.on('change', function (event) {
            var selectedOption = getSelectedOption();
            var data = { ID: selectedOption.val(), Control: serverData.taskGroupSelector };
            taskGroupMenuCallback(JSON.stringify(data), '');

            closeMenu();
            return false;
        });


        // Redirects user to UI of given group, if exists
        this.lnkEditTaskGroup.on('click', function (event) {
            var selectedOption = getSelectedOption();
            var data = { ID: selectedOption.val(), Control: serverData.lnkEditTaskGroup };
            taskGroupMenuCallback(JSON.stringify(data), '');

            return true;
        });


        // Closes menu after clicking on button or changing task group in selector
        // cannot be used dropdown('toggle') function, because the dropdown is not going to open again 
        function closeMenu() {
            infoCodeNameMessage();
            $cmsj('#' + serverData.stagingTaskGroupMenuDropdown).removeClass('open');
        };


        // Returns selected option in group selector
        function getSelectedOption() {
            return $cmsj('#' +serverData.taskGroupSelector + ' option:selected');
        };


        // Change menu title
        function changeMenuTitle(selectedGroup) {
            var menuTitle = selectedGroup.text();
            if (selectedGroup.val() == serverData.noneOption) {
                menuTitle = serverData.noneText;
            }

            $cmsj('#' + serverData.taskGroupMenuText).text(menuTitle);
        };

        
        // Changing error message into info
        function infoCodeNameMessage() {
             $cmsj('#' + serverData.codeNameMessage).removeClass('dropdown-menu-item-error-message');
             $cmsj('#' + serverData.codeNameMessage).addClass('dropdown-menu-item-info-message');
        };


        // On given jQuery object which is an element calls focus
        function setFocusOnElement(elementToFocus) {
            if (elementToFocus) {
                setTimeout(function () {
                    elementToFocus.focus();
                }, 0);
            }
        };
    };

    return Module;
});