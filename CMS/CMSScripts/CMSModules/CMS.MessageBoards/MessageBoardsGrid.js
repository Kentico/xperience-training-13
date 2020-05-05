cmsdefine(['CMS/EventHub', 'CMS/AppList', 'CMS/GridActions'], function (eventHub, AppList, GridActions) {

    var appList = new AppList(),

        MessageBoardsGrid = function (data) {
            var that = this;

            this.pagesApplicationHash = data.pagesApplicationHash;

            var gridActions = new GridActions({
                gridSelector: data.gridSelector,
                actionButtonsActions: {
                    '.js-edit': function (actionData) {
                        that.editMessageBoard(actionData.nodeId, actionData.documentCulture);
                    },
                    '.js-viewDocument': function (actionData) {
                        that.openDocumentInPages(actionData.nodeId, actionData.documentCulture);
                    }
                }
            });
        };

    MessageBoardsGrid.prototype.openDocumentInPages = function (documentId, documentCulture) {
        appList.openApplication(undefined, undefined, '?nodeid=' + documentId + '&culture=' + documentCulture, this.pagesApplicationHash);
    };

    MessageBoardsGrid.prototype.editMessageBoard = function (eventId) {
        if (this.eventDetailUrl) {
            window.location.replace(this.eventDetailUrl + '&eventId=' + eventId + '&objectid=' + eventId);
        }
    };

    return MessageBoardsGrid;
});