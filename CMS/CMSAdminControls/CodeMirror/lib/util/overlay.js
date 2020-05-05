/* CMS */
CodeMirror.RegisterMacroOverlay = function (mode, mime) {
    CodeMirror.defineMode(mode + '_macro', function (config, parserConfig) {
        var origName = mode;
        var origMode = CodeMirror.getMode(config, mime || origName);
        var clikeMode = CodeMirror.getMode(config, "text/x-csharp");
        var macroRegEx = new RegExp("^##[a-zA-Z]+##");

        function original(stream, state) {
            if (stream.match(/^{(\([0-9]+\))?[%$^@?#]/, true)) {
                var current = stream.current();
                var type = current.substr(current.length - 1, 1);
                state.macroEnd = type + current.substring(1, current.length - 1) + "}";
                if (type == '%') {
                    state.token = clike;
                    state.localState = clikeMode.startState();
                    state.mode = "clike";
                }
                else {
                    state.token = inMacro;
                }
                return "macro";
            }

            if (stream.match(macroRegEx, false)) {
                stream.next();
                stream.next();
                state.token = macro;
                return "macro";
            }

            return origMode.token(stream, state.origState);
        }

        function maybeBackup(stream, pat, style) {
            var cur = stream.current();
            var close = cur.search(pat);
            if (close > -1) stream.backUp(cur.length - close);
            return style;
        }

        function untilQuote(stream, state) {
            var ch = stream.next();
            while (ch != this.quote) {
                if ((ch = stream.next()) == null) break;
            }
            state.token = original;
            state.localState = null;
            state.mode = origName;
            this.quote = null;

            return "string";
        }

        function inMacro(stream, state) {
            var macroEnd = state.macroEnd;
            if (stream.match(macroEnd, true)) {
                state.token = original;
                return "macro";
            }

            while (!stream.match(macroEnd, false)) {
                if (stream.next() == null) break;
            }

            return "mc";
        }

        function clike(stream, state) {
            var macroEnd = state.macroEnd;
            if (stream.match(macroEnd, true)) {
                if (this.quote != null) {
                    state.token = untilQuote;
                }
                else {
                    state.token = original;
                }
                state.localState = null;
                state.mode = origName;
                return "macro";
            }

            if (stream.match(/^\|\([a-z]+\)/i, true)) {
                while (!stream.match(macroEnd, false) && stream.next());
                return "params cm-mc";
            }

            return maybeBackup(stream, /\%\}/,
                       clikeMode.token(stream, state.localState)) + " cm-mc";
        }

        function macro(stream, state) {
            if (stream.match('##', true)) {
                state.token = original;
                return "macro";
            }

            while (!stream.match('##', false)) {
                if (stream.next() == null) break;
            }

            return "mc";
        }

        return {
            noIndent: origMode.noIndent,
        
            startState: function () {
                var state = (origMode.startState ? origMode.startState() : null);
                return { token: original, localState: null, mode: origName, origState: state };
            },

            copyState: function (state) {
                if (state.localState) {
                    var local = CodeMirror.copyState(state.token == clike ? clikeMode : origName, state.localState);
                }
                return { token: state.token, localState: local, mode: state.mode,
                    origState: CodeMirror.copyState(origMode, state.origState)
                };
            },

            token: function (stream, state) {
                return state.token(stream, state);
            },

            indent: function (state, textAfter) {
                if (origMode.indent) {
                    return origMode.indent(state.origState, textAfter);
                }
            }
        };
    });
};
CodeMirror.RegisterMacroOverlay("css", "text/css");
CodeMirror.RegisterMacroOverlay("htmlmixed");
CodeMirror.RegisterMacroOverlay("aspnet");
CodeMirror.RegisterMacroOverlay("clike", "text/x-csharp");
CodeMirror.RegisterMacroOverlay("xml", "application/xml");
CodeMirror.RegisterMacroOverlay("plsql", "text/x-plsql");
CodeMirror.RegisterMacroOverlay("javascript");
CodeMirror.RegisterMacroOverlay("less", "text/less");
/* CMS end */
