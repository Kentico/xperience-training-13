/*!
* docwrite jQuery plugin v1.0.0
*
* Copyright 2011, Slando Ltd. <dev@slando.com>
* Dual licensed under the MIT or GPL Version 2 licenses.
* http://jquery.org/license
*
* Date: Thu Feb 24 11:32:37 GMT 2011
*/

/**
* @fileOverview
* Make document.write() a little bit safer
*
* // Redirect document.write() to an element:
* $(element).docwrite( function(arg1, arg2) {
*     document.write("appended to element");
*     document.write("<script>document.write('also appended to element')</script>");
* }, "passed to function as arg1", "passed to function as arg2" );
*
* $(document).load(function() {
*     document.write("will be appended to an invisible <DIV>");
* });
*
* $(document).bind( 'beforedocwrite', function(event, data) {
*     data.html.replace("whatever was going to be written",
*                       "what you would rather write");
*     data.target = $(destination);
*     data.unused = "pass data to the afterdocwrite event";
*     event.preventDefault(); // cancel writing
* });
*
* $(document).bind( 'afterdocwrite', function(event, data) {
*     if ( data.unused ) {
*         # ...
*     };
* });
*/
(function($) {

    var match_script = /<script ([^>]*)>/i,
         match_script_src = /^<script ([^>]* )?src=/i,
         match_script_end = /<\/script>/i,
         document_write = document.write,
         write_target = undefined;

    // document.write() implementation before the ready event has fired:
    function write(html) {

        var data = { target: null, html: html },
             event = $.Event("beforedocwrite");

        $(document).trigger(event, data);

        if (!event.isDefaultPrevented()) {

            if (null === data.target)
                document_write.call(document, data.html);
            else if (data.target.docwrite)
                data.target.docwrite(data.html);
            else
                $(data.target).docwrite(data.html);

            $(document).trigger('afterdocwrite', data);

        }

    };
    function writeln(html) { write(html + "\n"); };

    // document.write() implementation after the ready event has fired:
    function write_late(html) {

        if (write_target === undefined) {
            write_target = document.getElementsByTagName("head")[0];
            write_target = $(write_target);
        };

        var data = { target: write_target, html: html },
             event = $.Event("beforedocwrite");

        $(document).trigger(event, data);

        if (!event.isDefaultPrevented()) {
            data.target.docwrite(data.html);
            $(document).trigger('afterdocwrite', data);
        };

    };
    function writeln_late(html) { write_late(html + "\n"); };

    document.write = write;
    document.writeln = writeln;
    $(document).ready(function() {
        document.write = write_late;
        document.writeln = writeln_late;
    });

    $.fn.docwrite = function(arg) {

        var html = [],
        // we're about to override the system document.write (again)
        // but first we make sure it will be reset, even if we throw an exception:
             document_write = document.write,
             document_writeln = document.writeln,
             timeout = setTimeout(function() {
                 document.write = document_write;
                 document.writeln = document_writeln;
             }, 0),
             i = 0, args = [];

        /*
        * Protect against nested document.write's:
        *
        *     document.write('<script type="text/javascript">document.write("...")</script>');
        *
        * Browsers ignore <script>s inserted with innerHTML,
        * so we have to parse out and eval() the nested <script>s.
        *
        */
        document.write = function write_inner(text) {

            var start_pos,
                 end_pos; // needs to be a local variable because this is called recursively

            while (-1 != (start_pos = text.search(match_script))) {
                // <script...>

                html.push(text.substr(0, start_pos));

                if (match_script_src.test(text)) {
                    // <script src="...">: remove the "<script" and continue parsing:
                    html.push(text.substr(0, start_pos + 7));
                    text = text.substr(start_pos + 7);
                } else if (-1 == (end_pos = text.search(match_script_end))) {
                    // <script> with no </script>:
                    html.push(text);
                    return;
                } else {
                    // <script>...</script>, may do a document.write() of its own:
                    start_pos = text.indexOf('>', start_pos) + 1;
                    $.globalEval(text.substr(start_pos, end_pos - start_pos));
                    text = text.substr(end_pos + 9);
                };
            };

            html.push(text);
        };
        document.writeln = function(html) { write_inner(html + "\n"); };

        if (arg.apply) {
            while (++i < arguments.length) args[i - 1] = arguments[i];
            arg.apply(this, args);
        } else {
            document.write(arg);
        };

        document.write = document_write;
        clearTimeout(timeout);

        if ('' != (html = html.join(''))) {
            this.append(html);
        };

        return this;
    };

})($cmsj);
