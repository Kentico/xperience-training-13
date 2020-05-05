cmsdefine(['CMS/EventHub'], function (EventHub, $) {

    var Module = function (serverData) {
        var parameters = {
            taskGroupID: serverData.taskGroupID,
            taskGroupName: serverData.taskGroupName
        };

        // Publishes event for module StagingTaskGroupMenu.js, that task group was saved in UI
        EventHub.publish('TaskGroupSaved', parameters);

    };
    return Module;
});