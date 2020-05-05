cmsdefine(['CMS/EventHub', 'CMS/AppList', 'CMS/GridActions'], function (eventHub, AppList, GridActions) {

    var appList = new AppList(),

        DocumentsGrid = function (data) {
            var that = this,
                gridActions;

            this.pagesApplicationHash = data.pagesApplicationHash;

            gridActions = new GridActions({
                gridSelector: data.gridSelector,
                actionButtonsActions: {
                    '.js-edit': function (actionData) {
                        that.editDocument(actionData.nodeId, actionData.documentCulture, data.openInNewWindow, actionData.siteUrl);
                    },
                    '.js-preview': function (actionData) {
                        that.previewDocument(actionData.previewUrl);
                    }
                }
            });
        };

    DocumentsGrid.prototype.editDocument = function (documentId, documentCulture, openInNewWindow, documentSiteUrl) {
        if (!documentId) {
            return;
        }

        var queryString = '?action=edit&nodeid=' + documentId + '&culture=' + documentCulture,
            locationHash = this.pagesApplicationHash;

        if (openInNewWindow) {
            appList.openApplicationInNewWindow(documentSiteUrl, '', queryString, locationHash, 'PageTemplateWindow');
        } else {
            appList.openApplication(documentSiteUrl, '', queryString, locationHash);
        }
    };

    DocumentsGrid.prototype.previewDocument = function (previewUrl) {
        appList.openApplicationInNewWindow(previewUrl, undefined, undefined, undefined, 'Preview');
    }

    return DocumentsGrid;
});