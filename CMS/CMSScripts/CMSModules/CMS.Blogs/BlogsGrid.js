cmsdefine(['CMS/EventHub', 'CMS/GridActions', 'CMS/AppList'], function (eventHub, GridActions, AppList) {

    var appList = new AppList(),

        BlogsGrid = function (data) {
            var that = this,
                gridActions;

            this.pagesApplicationHash = data.pagesApplicationHash;
            this.blogPostClassId = data.blogPostClassId;

            gridActions = new GridActions({
                gridSelector: data.gridSelector,
                actionButtonsActions: {
                    '.js-edit': function (actionData) {
                        that.openBlogInPages(actionData.nodeId, actionData.documentCulture);
                    },
                    '.js-newpost': function (actionData) {
                        that.createNewBlogPost(actionData.nodeId, actionData.documentCulture);
                    }
                }
            });
        };

    BlogsGrid.prototype.openBlogInPages = function (blogId, blogCulture) {
        if (blogId !== 0) {
            appList.openApplication(undefined, undefined, '?action=edit&nodeid=' + blogId + '&culture=' + blogCulture, this.pagesApplicationHash);
        }
    };

    BlogsGrid.prototype.createNewBlogPost = function (blogId, culture) {
        if (blogId !== 0) {
            appList.openApplication(undefined, undefined, '?action=new&nodeid=' + blogId + '&classid=' + this.blogPostClassId + '&culture=' + culture, this.pagesApplicationHash);
        }
    }

    return BlogsGrid;
});