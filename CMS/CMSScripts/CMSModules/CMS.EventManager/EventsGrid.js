cmsdefine(['CMS/EventHub', 'CMS/AppList', 'CMS/GridActions'], function (eventHub, AppList, GridActions) {

    var appList = new AppList(),

        EventsGrid = function (data) {
            var that = this,
                gridActions;

            this.pagesApplicationHash = data.pagesApplicationHash;
            this.eventsApplicationHash = data.eventsApplicationHash;
            this.eventDetailUrl = data.eventDetailURL;

            gridActions = new GridActions({
                gridSelector: data.gridSelector,
                actionButtonsActions: {
                    '.js-edit': function (actionData) {
                        that.openEventInPages(actionData.nodeId, actionData.documentCulture);
                    },
                    '.js-view': function(actionData) {
                        that.viewEventDetail(actionData.nodeId);
                    }
                }
            });
        };

    EventsGrid.prototype.openEventInPages = function (documentId, documentCulture) {
        appList.openApplication(undefined, undefined, '?action=edit&nodeid=' + documentId + '&culture=' + documentCulture, this.pagesApplicationHash);
    };

    EventsGrid.prototype.viewEventDetail = function (eventId) {
        if (this.eventDetailUrl) {
            // Change the application hash to events application when viewing from dashboards
            if (window.top.location.hash !== this.eventsApplicationHash) {
                window.top.history.pushState(null, null, this.eventsApplicationHash);
            }

            window.location.replace(this.eventDetailUrl + '&eventId=' + eventId + '&objectid=' + eventId);
        }
    };

    return EventsGrid;
});