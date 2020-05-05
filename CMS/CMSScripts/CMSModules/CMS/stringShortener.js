cmsdefine(['jQuery'], function ($) {

    var createTestSpan = function (container) {
            var testSpan = $('<span></span>').css('visibility', 'hidden');
            $(container).append(testSpan);

            return testSpan;
        },


        getTextWidth = function (testSpan, string) {
            testSpan.text(string);
            return testSpan.width();
        },


        containsSpaces = function (string) {
            return string.indexOf(' ') !== -1;
        },


        performShortening = function (string, testSpan, maxWidth) {
            var ellipsis = '…',
                output = string,
                offset = string.length/2,
                pivot = offset/2;

            while (currentWidth !== maxWidth && pivot > 0.25)
            {
                output = string.slice(0, Math.round(offset));
                var currentWidth = getTextWidth(testSpan, output);

                if(currentWidth > maxWidth)
                {
                    offset -= pivot;
                }
                else
                {
                    offset += pivot;
                }

                pivot /= 2;
            }

            if (containsSpaces(output)) {
                output = output.substring(0, output.lastIndexOf(' '));
            }

            return output + ellipsis;
        },


        shortenString = function (string, maxWidth, container) {
            if (!string) {
                throw 'String to be shortened has to be specified';
            }

            if (!maxWidth) {
                throw 'Max width of the text has to be specified';
            }

            if (!container) {
                throw 'Container to test against has to be specified';
            }

            var testSpan = createTestSpan(container),
                currentWidth = getTextWidth(testSpan, string),
                output;

            if (currentWidth <= maxWidth) {
                output = string;
            }
            else {
                output = performShortening(string, testSpan, maxWidth);
            }

            testSpan.remove();
            return output;
        };


    return {
        shortenString: shortenString
    };
});
