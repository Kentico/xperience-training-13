function InitCannedResponses(txtbox, button) {
	var availableTags = CannedResponses,
		txt = $cmsj(txtbox),
		btn = $cmsj(button);

	Inicialize();

	// Set up jQuery UI Autocomplete
	function Inicialize() {
		btn.show();

		txt.autocomplete({
			autoFocus: true,
			source: function(request, response) {
				var searchTerm = extractTerm(request.term);
				if (searchTerm != null) {
				    response($cmsj.ui.autocomplete.filter(availableTags, searchTerm));
				}
			},
			focus: function(event, ui) {
				return false;
			},
			close: function(event, ui) {
				ChatManager.IntellisenseActive = false;
			},
			select: function(event, ui) {
				var caretPos = txt.getSelection().start;
				var returnedValue = replaceWord(this.value, caretPos, ui.item.value);
				this.value = returnedValue.string;
				txt.setCaretPos(returnedValue.position);
				ChatManager.IntellisenseActive = false;
				return false;
			},
			position: {
				my: "left bottom",
				at: "left top",
				collision: "none"
			}
		})
		// Overwrite default render function to suit our needs
		.data('ui-autocomplete')._renderItem = function (ul, item) {
		    return $cmsj("<li></li>")
					.attr("data-value", item.value)
		            .addClass("chat-canned-responses-item")
					.append("<a><b>" + item.label.substr(1) + "</b><br>" + item.tooltip + "</a>")
					.appendTo(ul);
		};
	}
	

	// Extracts last tag from input text
	// Returns the string without "#" if the last word is tag, null otherwise
	function extractTerm(val) {

		// Get caret position
		var caretPos = txt.getSelection().start;

		var term = extractWord(val, caretPos);
		if (term.charAt(0) == '#' && (availableTags.length != 0)) {
			ChatManager.IntellisenseActive = true;
			return term.substr(1);
		}
		txt.autocomplete("close");
		ChatManager.IntellisenseActive = false;
		return null;
	}
	

	// Extracts substring delimited by whitespaces at position
	function extractWord(string, position) {
		// Define regex - white spaces
		var regex = /\s/g;
		
		// Get the first part of selected word
		var firstPart = string.substr(0, position);
		var last = 0;
		while (regex.test(firstPart) == true) {
			last = regex.lastIndex;
		}
		firstPart = firstPart.substr(last);
		
		// Get the second part of selected word
		var secondPart = string.substr(position);
		var isThere = regex.test(secondPart);
		var first = (isThere == true) ? (regex.lastIndex - 1) : secondPart.length;
		secondPart = secondPart.substr(0, first);
		
		// return the whole word
		return firstPart + secondPart;
	}
	

	// Replaces word in string at position by another word
	function replaceWord(string, position, word) {
		if (extractTerm(txt.val()) == null) {
			var returnObj = new Object();
			returnObj.string = string.substr(0, position) + word + string.substr(position);
			returnObj.position = string.substr(0, position).length + word.length + 1;
			return returnObj;
		}
		
		// Define regex - white spaces
		var regex = /\s/g;

		// Find the substring before the replaced word
		var firstPart = string.substr(0, position);
		var last = 0;
		while (regex.test(firstPart) == true) {
			last = regex.lastIndex;
		}
		firstPart = firstPart.substr(0, last);

		// Find the substring after the replaced word
		var secondPart = string.substr(position);
		var isThere = regex.test(secondPart);
		var first = (isThere == true) ? (regex.lastIndex - 1) : secondPart.length;
		secondPart = secondPart.substr(first);

		// return object with "string with new word" and "new caret position"
		var returnObj = new Object();
		returnObj.string = firstPart + word + secondPart;
		returnObj.position = firstPart.length + word.length + 1;
		return returnObj;
	}
}

