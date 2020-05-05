cmsdefine([], function () {

    /** 
     * Reads the file locally on the client. Contains methods for both line reading and the bulk reading.
     */
    var FileStreamer = function (file) {
        var that = this;

        this.file = file;
        this.fileReader = new window.FileReader();
        this.handle = {};
        this.start = 0;
        this.slice = null;
        this.bulkSize = 43008, // 42 KiB

        this.fileReader.onloadend = this._onloadend.bind(this);
        this.slice = this.file.slice || this.file.webkitSlice || this.file.mozSlice;

        this.handle = {
            "continue": function () {
                if (!this.finished) {
                    that._read();
                } else {
                    that._resetCursor();
                }
            },
            reset: that._resetCursor.bind(that),
            move: that._moveCursor.bind(that),
            finished: false,
        };
    };


    FileStreamer.prototype._onloadend = function (e) {
        this.start += this.bulkSize;

        this.handle.finished = this.start >= this.file.size;

        this.callback(e.target.result, this.handle);
    };


    FileStreamer.prototype._read = function () {
        var end = Math.min(this.start + this.bulkSize, this.file.size);
        this.fileReader.readAsText(this.slice.call(this.file, this.start, end));
    };


    FileStreamer.prototype._resetCursor = function () {
        this.start = 0;
    };


    FileStreamer.prototype._moveCursor = function (x) {
        this.start += x;
    };


    /**
     * Read the file given in FileReader constructor by chunks, each chunk is passed to callback with handle of filestream.
     * @param callback  action(string, handle object)   recently read chunk and state of reading (whether the file read was finished). 
     *                                                  Calls repeatedly until the file is whole read.
     */
    FileStreamer.prototype.read = function (callback) {
        this.callback = callback;
        this._read();
    };

    return FileStreamer;
});