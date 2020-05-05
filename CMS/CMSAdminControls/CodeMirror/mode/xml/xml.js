CodeMirror.defineMode("xml", function (config, parserConfig) {
    var indentUnit = config.indentUnit;
    var Kludges = parserConfig.htmlMode ? {
        autoSelfClosers: { "br": true, "img": true, "hr": true, "link": true, "input": true,
            "meta": true, "col": true, "frame": true, "base": true, "area": true
        },
        doNotIndent: { "pre": true },
        allowUnquoted: true
    } : { autoSelfClosers: {}, doNotIndent: {}, allowUnquoted: false };
    var alignCDATA = parserConfig.alignCDATA;

    // Return variables for tokenizers
    var tagName, type;

    function inText(stream, state) {
        function chain(parser) {
            state.tokenize = parser;
            return parser(stream, state);
        }

        var ch = stream.next();
        if (ch == "<") {
            if (stream.eat("!")) {
                if (stream.eat("[")) {
                    if (stream.match("CDATA[")) return chain(inBlock("atom", "]]>"));
                    else return null;
                }
                else if (stream.match("--")) return chain(inBlock("comment", "-->"));
                else if (stream.match("DOCTYPE", true, true)) {
                    stream.eatWhile(/[\w\._\-]/);
                    return chain(doctype(1));
                }
                else return null;
            }
            else if (stream.eat("?")) {
                stream.eatWhile(/[\w\._\-]/);
                state.tokenize = inBlock("meta", "?>");
                return "meta";
            }
            else {
                type = stream.eat("/") ? "closeTag" : "openTag";
                stream.eatSpace();
                tagName = "";
                var c;
                var re = new RegExp('[^\\s\\u00a0=<>\\"\'\\/?' + CodeMirror.WP_CHAR + ']');
                while ((c = stream.eat(re))) tagName += c;
                state.tokenize = inTag;
                return "tag";
            }
        }
        else if (ch == "&") {
            var ok;
            if (stream.eat("#")) {
                if (stream.eat("x")) {
                    ok = stream.eatWhile(/[a-fA-F\d]/) && stream.eat(";");
                } else {
                    ok = stream.eatWhile(/[\d]/) && stream.eat(";");
                }
            } else {
                ok = stream.eatWhile(/[\w]/) && stream.eat(";");
            }
            return ok ? "atom" : "error";
        }
        /* CMS */
        else if (ch == CodeMirror.WP_CHAR) {
            return "webpart";
        }
        else {
            var re = new RegExp('[^&<{#' + CodeMirror.WP_CHAR + ']');
            stream.eatWhile(re);
            /* CMS end */
            return null;
        }
    }

    function inTag(stream, state) {
        var ch = stream.next();
        /* CMS */
        if (ch == CodeMirror.WP_CHAR) {
            state.tokenize = inText;
            return "webpart";
        }
        else
        /* CMS end */
            if (ch == ">" || (ch == "/" && stream.eat(">"))) {
                state.tokenize = inText;
                type = ch == ">" ? "endTag" : "selfcloseTag";
                return "tag";
            }
            else if (ch == "=") {
                type = "equals";
                return null;
            }
            else if (/[\'\"]/.test(ch)) {
                state.tokenize = inAttribute(ch);
                return state.tokenize(stream, state);
            }
            else {
                /* CMS */
                var re = new RegExp('[^\\s\\u00a0=<>\\"\'\\/?' + CodeMirror.WP_CHAR + ']');
                stream.eatWhile(re);
                /* CMS end */
                return "word";
            }
    }

    function inAttribute(quote) {
        return function (stream, state) {
            while (!stream.eol()) {
                /* CMS */
                if (state.handleAspx && stream.match('<%', false)) {
                    break;
                }
                else if (stream.match('{%', false)) {
                    break;
                }
                else if (stream.peek() == CodeMirror.WP_CHAR) {
                    state.tokenize = inTag;
                    break;
                }
                else
                /* CMS end */
                    if (stream.next() == quote) {
                        state.tokenize = inTag;
                        break;
                    }
            }
            return "string";
        };
    }

    function inBlock(style, terminator) {
        return function (stream, state) {
            while (!stream.eol()) {
                if (stream.match(terminator)) {
                    state.tokenize = inText;
                    break;
                }
                stream.next();
            }
            return style;
        };
    }
    function doctype(depth) {
        return function (stream, state) {
            var ch;
            while ((ch = stream.next()) != null) {
                if (ch == "<") {
                    state.tokenize = doctype(depth + 1);
                    return state.tokenize(stream, state);
                } else if (ch == ">") {
                    if (depth == 1) {
                        state.tokenize = inText;
                        break;
                    } else {
                        state.tokenize = doctype(depth - 1);
                        return state.tokenize(stream, state);
                    }
                }
            }
            return "meta";
        };
    }

    var curState, setStyle;
    function pass() {
        for (var i = arguments.length - 1; i >= 0; i--) curState.cc.push(arguments[i]);
    }
    function cont() {
        pass.apply(null, arguments);
        return true;
    }

    function pushContext(tagName, startOfLine) {
        var noIndent = Kludges.doNotIndent.hasOwnProperty(tagName) || (curState.context && curState.context.noIndent);
        curState.context = {
            prev: curState.context,
            tagName: tagName,
            indent: curState.indented,
            startOfLine: startOfLine,
            noIndent: noIndent
        };
    }
    function popContext() {
        if (curState.context) curState.context = curState.context.prev;
    }

    function element(type) {
        if (type == "openTag") {
            curState.tagName = tagName;
            return cont(attributes, endtag(curState.startOfLine));
        } else if (type == "closeTag") {
            var err = false;
            if (curState.context) {
                /* CMS */
                if (curState.context.tagName != tagName) {
                    setStyle = "tag error"
                    return cont(endclosetag(setStyle));
                }
                /* CMS end */
            } else {
                err = true;
            }
            if (err) setStyle = "error";
            return cont(endclosetag(err));
        }
        return cont();
    }
    function endtag(startOfLine) {
        return function (type) {
            if (type == "selfcloseTag" ||
          (type == "endTag" && Kludges.autoSelfClosers.hasOwnProperty(curState.tagName.toLowerCase())))
                return cont();
            if (type == "endTag") { pushContext(curState.tagName, startOfLine); return cont(); }
            return cont();
        };
    }
    function endclosetag(err) {
        return function (type) {
            /* CMS */
            if (typeof (err) == 'string') {
                setStyle = err;
                popContext();
                return cont();
            }
            /* CMS end*/
            if (err) setStyle = "error";
            if (type == "endTag") { popContext(); return cont(); }
            setStyle = "error";
            return cont(arguments.callee);
        }
    }

    function attributes(type) {
        if (type == "word") { setStyle = "attribute"; return cont(attributes); }
        if (type == "equals") return cont(attvalue, attributes);
        if (type == "string") { setStyle = "error"; return cont(attributes); }
        return pass();
    }
    function attvalue(type) {
        if (type == "word" && Kludges.allowUnquoted) { setStyle = "string"; return cont(); }
        if (type == "string") return cont(attvaluemaybe);
        return pass();
    }
    function attvaluemaybe(type) {
        if (type == "string") return cont(attvaluemaybe);
        else return pass();
    }

    return {
        startState: function () {
            return { tokenize: inText, cc: [], indented: 0, startOfLine: true, tagName: null, context: null };
        },

        token: function (stream, state) {
            if (stream.sol()) {
                state.startOfLine = true;
                state.indented = stream.indentation();
            }
            if (stream.eatSpace()) return null;

            setStyle = type = tagName = null;
            var style = state.tokenize(stream, state);
            state.type = type;
            if ((style || type) && style != "comment") {
                curState = state;
                while (true) {
                    var comb = state.cc.pop() || element;
                    if (comb(type || style)) break;
                }
            }
            state.startOfLine = false;
            return setStyle || style;
        },

        indent: function (state, textAfter, fullLine) {
            var context = state.context;
            if ((state.tokenize != inTag && state.tokenize != inText) ||
          context && context.noIndent)
                return fullLine ? fullLine.match(/^(\s*)/)[0].length : 0;
            if (alignCDATA && /<!\[CDATA\[/.test(textAfter)) return 0;
            if (context && /^<\//.test(textAfter))
                context = context.prev;
            while (context && !context.startOfLine)
                context = context.prev;
            if (context) return context.indent + indentUnit;
            else return 0;
        },

        compareStates: function (a, b) {
            if (a.indented != b.indented || a.tokenize != b.tokenize) return false;
            for (var ca = a.context, cb = b.context; ; ca = ca.prev, cb = cb.prev) {
                if (!ca || !cb) return ca == cb;
                if (ca.tagName != cb.tagName) return false;
            }
        },

        electricChars: "/"
    };
});

CodeMirror.defineMIME("application/xml", "xml");
CodeMirror.defineMIME("text/html", {name: "xml", htmlMode: true});
