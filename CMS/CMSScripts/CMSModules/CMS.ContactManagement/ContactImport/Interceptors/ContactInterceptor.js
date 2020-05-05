cmsdefine(['CMS/Messages/MessageHub'], function (MessageHub) {
    return ['resolveFilter', function (resolveFilter) {
        var interceptor = {
            responseError: function (response) {
                if (response.data) {
                    new MessageHub().publishError(response.data);
                } else {
                    new MessageHub().publishError(resolveFilter("om.contact.importcsv.unknownerrorclientside"));
                }
            }
        };

        return interceptor;
    }];
});
