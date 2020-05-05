/*

highlight v3

Highlights arbitrary terms.

<http://johannburkard.de/blog/programming/javascript/highlight-javascript-text-higlighting-jquery-plugin.html>

MIT license.

Johann Burkard
<http://johannburkard.de>
<mailto:jb@eaio.com>

*/

$cmsj.fn.highlight = function(pat, className, regExp) {
    function innerHighlight(node, pat, className, regExp) {
        var skip = 0;
        if (node.nodeType == 3) {
            if (regExp == 'True') {
                var regex = new RegExp(pat, "");
                var match = node.data.match(regex);
                if ((match != null) && (match.index >= 0)) {
                    skip = createSubNode(node, match.index, match[0].length, className);
                }
            }
            else {
                var pos = node.data.indexOf(pat);
                skip = createSubNode(node, pos, pat.length, className);
            }
        }
        else if (node.nodeType == 1 && node.childNodes && !/(script|style)/i.test(node.tagName)) {
            for (var i = 0; i < node.childNodes.length; ++i) {
                i += innerHighlight(node.childNodes[i], pat, className, regExp);
            }
        }
        return skip;
    }
    function createSubNode(node, pos, length, className) {
        if (pos >= 0) {
            var spannode = document.createElement('span');
            if (typeof (className) == 'undefined') {
                spannode.className = 'highlight';
            }
            else {
                spannode.className = className;
            }
            var middlebit = node.splitText(pos);
            var endbit = middlebit.splitText(length);
            var middleclone = middlebit.cloneNode(true);
            spannode.appendChild(middleclone);
            middlebit.parentNode.replaceChild(spannode, middlebit);
            return 1;
        }
        return 0;
    }
    return this.each(function() {
        innerHighlight(this, pat, className, regExp);
    });
};