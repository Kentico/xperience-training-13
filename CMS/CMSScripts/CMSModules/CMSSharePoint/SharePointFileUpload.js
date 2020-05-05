cmsdefine([], function () {

    var Module = function (opt) {

        window.SharePointUpload = { };

        window.SharePointUpload.Refresh = function () {
            window.location = window.location;
        }

        window.SharePointUpload.UploadCompleted = function () {
            window.SharePointUpload.Refresh();
        }

        DFU.OnUploadCompleted = window.SharePointUpload.UploadCompleted;
    };

    return Module;
});
