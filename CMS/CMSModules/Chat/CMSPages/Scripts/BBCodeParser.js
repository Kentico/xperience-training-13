ChatBBCodeParserObj = function () {

    var that = this,
        urlRegExp = /(^|\s+|\(|\]|=)(((ht|f)tps?:\/\/)|(www\.))[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,4}(:[0-9]+)?(\/?|(\/[^\s\[\]]+))?($|\s+|\)|\[|\])/gi;
    urlRegExpValidation = /^(((ht|f)tps?:\/\/)|(www\.))[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,4}(:[0-9]+)?(\/?|(\/\S+))?$/i;

    // Returns resolved bbcode (HTML). 
    // If textOnly == true, returns only resolved text without HTML tags 
    // (for test, if message will be empty after it will be resolved or when bbcode is disabled).
    this.ParseBBCode = function (bbcode, textOnly) {
        var parsedCode = that.GetTags(bbcode);
        if (parsedCode.Tags == null) {
            return GetText(bbcode);
        }

        var tags = parsedCode.Tags,
            text = parsedCode.Text,

            html = (text[0] == null) ? "" : GetText(text[0]),
            indText = 1,
            parsedTag,
            stack = new Array();

        for (var indTag = 0; indTag < tags.length; indTag++, indText++) {

            parsedTag = ParseTag(tags[indTag]);

            // Start tag
            if (parsedTag[1] != null) {

                // Special case - image is not pair tag in html
                if ((parsedTag[0] == "[img]") && (tags.length > indTag + 1) && (tags[indTag + 1] == "[/img]")) {
                    parsedTag[2] = text[indText++];
                    indTag++;
                }
                else {
                    if (IsValidStartTag(parsedTag)) {
                        stack.push(parsedTag[1]);
                    }
                }
            }

            // End tag
            else {
                // If end tag is related to the last start tag
                if ((parsedTag[3] != null) && (stack.length > 0) && (parsedTag[3] == stack[stack.length - 1])) {
                    stack.pop();
                }

                // Parsed tag is not related to the last start tag
                else {
                    var stackInd = stack.indexOf(parsedTag[3]);
                    if (stackInd >= 0) {
                        while ((stack.length > 0) && (stack[stack.length - 1] != parsedTag[3])) {
                            html += ResolveTag(CreateEndTag(stack.pop()), textOnly);
                        }
                    }
                    // stack is emty or there is no start tag related to actual end tag, render end tag as text
                    if (stack.length == 0 || stackInd < 0) {
                        html += parsedTag[0];
                        html += (text[indText] == null) ? "" : GetText(text[indText]);
                        continue;
                    }
                }
            }

            html += ResolveTag(parsedTag, textOnly);

            // If URL tag is resolving, then don't try to resolve URL by GetText again
            if (parsedTag[1] == "url") {
                html += (text[indText] == null) ? "" : text[indText];
            }
            else {
                html += (text[indText] == null) ? "" : GetText(text[indText]);
            }
        }

        // End tags missing - create end tags
        if (stack.length > 0) {
            for (var i = stack.length - 1; i >= 0; i--) {
                html += ResolveTag(CreateEndTag(stack[i]), textOnly);
            }
        }
        return html;
    };


    this.ResolveURLs = function (msg) {
        var urls = msg.match(urlRegExp);
        if (urls == null) {
            return msg;
        }
        var output = "",
            start = 0,
            end = 0,
            url = null;

        while ((urls != null) && (urls[0] != null)) {
            url = $cmsj.trim(urls[0]);
            var resolve = true,
                trim = false,
                trimLeft = false,
                trimRigth = false,
                urlLastChar = url[url.length - 1];

            switch (url[0]) {
                case '(':
                    if (urlLastChar == ')') {
                        trim = true;
                    }
                    else {
                        trimLeft = true;
                    }
                    break;
                case ']':
                    if (urlLastChar == '[') {
                        trim = true;
                    }
                    else {
                        trimLeft = true;
                    }
                    break;
                case '=':
                    if (urlLastChar == ']') {
                        trim = true;
                    }
                    else {
                        resolve = false;
                    }
                    break;
            }

            if (!trim && !trimLeft && resolve) {
                switch (urlLastChar) {
                    case ')':
                    case '[':
                        trimRigth = true;
                        break;
                }
            }

            if (trim) {
                url = url.substring(1, url.length - 1);
            }
            else if (trimLeft) {
                url = url.substring(1, url.length);
            }
            else if (trimRigth) {
                url = url.substring(0, url.length - 1);
            }

            var protocol = url.match(/^www.\S+/) ? "http://" : "";
            end = msg.indexOf(url, start);

            if (resolve) {
                var encodedHref = url.replace(/"/g, "%22");
                output += msg.substring(start, end) + '<a class="ChatBBCodeUrl" href="' + protocol + encodedHref + '" target="_blank" rel="noopener noreferrer">' + url + "</a>";
            }
            else {
                output += msg.substring(start, end) + url;
            }

            start = end + url.length;
            urls = msg.substr(start).match(urlRegExp);
        }
        output += msg.substring(start);
        return output;
    };


    function CreateEndTag(tag) {
        var endTag = ParseTag("[" + tag + "]");
        endTag[0] = "[/" + endTag[1] + "]";
        return endTag;
    };


    function GetText(text) {
        if (ChatManager.Settings.ResolveURLEnabled) {
            return that.ResolveURLs(text);
        }
        return text;
    }


    function IsValidStartTag(parsedTag) {
        switch (parsedTag[1]) {
            case "b":
            case "i":
            case "u":
            case "s":
            case "quote":
            case "code":
                if (parsedTag[2] != null) {
                    return false;
                }
                return true;
                break;
            case "url":
            case "color":
                if (parsedTag[2] != null) {
                    return true;
                }
        }
        return false;
    };


    function IsValidURL(url) {
        return url.match(urlRegExpValidation) ? true : false;
    };


    function IsValidColor(color) {
        var valid = color.match(/^\w+$/) ? true : false;
        if (!valid) {
            valid = color.match(/^#([0-9A-Fa-f]{3}){1,2}$/) ? true : false;
        }
        return valid;
    };


    function ResolveTag(tagobj, textOnly) {
        if (tagobj == null) {
            return "";
        }
        var resolved = "";
        var isImage = false;
        switch (tagobj[0]) {
            case "[b]":
                resolved = "<strong>";
                break;
            case "[/b]":
                resolved = "</strong>";
                break;
            case "[i]":
                resolved = "<i>";
                break;
            case "[/i]":
                resolved = "</i>";
                break;
            case "[u]":
                resolved = '<span style="text-decoration:underline">';
                break;
            case "[/u]":
                resolved = "</span>";
                break;
            case "[s]":
                resolved = '<s>';
                break;
            case "[/s]":
                resolved = "</s>";
                break;
            case "[quote]":
                resolved = '<span class="ChatBBQuote" style="font-size:0.9em"><i>';
                break;
            case "[/quote]":
                resolved = "</i></span>";
                break;
            case "[code]":
                resolved = '<span class="ChatBBCode" style="font-family:Courier">';
                break;
            case "[/code]":
                resolved = "</span>";
                break;
            case "[/url]":
                resolved = "</a>";
                break;
            case "[/color]":
                resolved = "</span>";
                break;
            case "[img]":
                if (tagobj[2] != null) {
                    var encodedAttr = tagobj[2].replace(/"/g, "%22");
                    resolved = '<img class="ChatBBCodeImg" src="' + encodedAttr + '" alt="' + encodedAttr + '" />';
                    isImage = true;
                }
                break;
            default:
                switch (tagobj[1]) {
                    case "url":
                        if (tagobj[2] != null) {
                            if (IsValidURL(tagobj[2])) {
                                var encodedHref = tagobj[2].replace(/"/g, "%22");
                                resolved = '<a class="ChatBBCodeUrl" href="' + encodedHref + '" target="_blank" rel="noopener noreferrer">';
                            }
                            else {
                                resolved = tagobj[2] + " ";
                            }
                        }
                        break;
                    case "color":
                        if ((tagobj[2] != null) && IsValidColor(tagobj[2])) {
                            resolved = '<span class="ChatBBCodeColor" style="color:' + tagobj[2] + '">';
                        }
                        break;
                }
        }
        if (resolved.length > 0) {
            if (textOnly && !isImage) {
                return "";
            }
            return resolved;
        }
        return GetText(tagobj[0]);
    };



    this.GetTags = function (bbcode) {
        var reTags = /\[\s*(?:(?:\w+\s*(?:=[^\]]*)?)|(?:\/\s*\w+))\s*\]/gi;
        return { "Tags": bbcode.match(reTags),
            "Text": cbSplit(bbcode, reTags)
        };
    };


    function ParseTag(tag) {
        var reTag = /\[\s*(?:(?:(\w+)\s*(?:=([^\]]*))?)|(?:\/\s*(\w+)))\s*\]([^\[]*)/gi;
        return reTag.exec(tag);
    };


    /* Cross-Browser Split 1.0.1
    (c) Steven Levithan <stevenlevithan.com>; MIT License
    An ECMA-compliant, uniform cross-browser split method */
    function cbSplit(str, separator, limit) {

        // if `separator` is not a regex, use the native `split`
        if (Object.prototype.toString.call(separator) !== "[object RegExp]") {
            return String.prototype.split(str, separator, limit);
        }

        var output = [],
        lastLastIndex = 0,
        flags = (separator.ignoreCase ? "i" : "") +
                (separator.multiline ? "m" : "") +
                (separator.sticky ? "y" : ""),
        separator = RegExp(separator.source, flags + "g"), // make `global` and avoid `lastIndex` issues by working with a copy
        separator2, match, lastIndex, lastLength,
        _compliantExecNpcg = /()??/.exec("")[1] === undefined, // NPCG: nonparticipating capturing group
        str = str + ""; // type conversion

        if (!_compliantExecNpcg) {
            separator2 = RegExp("^" + separator.source + "$(?!\\s)", flags); // doesn't need /g or /y, but they don't hurt
        }

        /* behavior for `limit`: if it's...
        - `undefined`: no limit.
        - `NaN` or zero: return an empty array.
        - a positive number: use `Math.floor(limit)`.
        - a negative number: no limit.
        - other: type-convert, then use the above rules. */
        if (limit === undefined || +limit < 0) {
            limit = Infinity;
        } else {
            limit = Math.floor(+limit);
            if (!limit) {
                return [];
            }
        }

        while (match = separator.exec(str)) {
            lastIndex = match.index + match[0].length; // `separator.lastIndex` is not reliable cross-browser

            if (lastIndex > lastLastIndex) {
                output.push(str.slice(lastLastIndex, match.index));

                // fix browsers whose `exec` methods don't consistently return `undefined` for nonparticipating capturing groups
                if (!_compliantExecNpcg && match.length > 1) {
                    match[0].replace(separator2, function () {
                        for (var i = 1; i < arguments.length - 2; i++) {
                            if (arguments[i] === undefined) {
                                match[i] = undefined;
                            }
                        }
                    });
                }

                if (match.length > 1 && match.index < str.length) {
                    Array.prototype.push.apply(output, match.slice(1));
                }

                lastLength = match[0].length;
                lastLastIndex = lastIndex;

                if (output.length >= limit) {
                    break;
                }
            }

            if (separator.lastIndex === match.index) {
                separator.lastIndex++; // avoid an infinite loop
            }
        }

        if (lastLastIndex === str.length) {
            if (lastLength || !separator.test("")) {
                output.push("");
            }
        } else {
            output.push(str.slice(lastLastIndex));
        }

        return output.length > limit ? output.slice(0, limit) : output;
    };
    // END Cross-Browser Split 1.0.1

}