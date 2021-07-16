(function () {
    window.kentico.pageBuilder.registerInlineEditor("text-editor", {
        init: function (options) {
            var editor = options.editor;
            var config = {
                toolbar: {
                    buttons: [
                        "bold",
                        "italic",
                        "underline",
                        "orderedlist",
                        "unorderedlist",
                        "h1",
                        "h2",
                        "h3"
                    ]
                },
                imageDragging: false,
                extensions: {
                  imageDragging: {}
                }
            };

            var mediumEditor = new MediumEditor(editor, config);

            mediumEditor.subscribe("editableInput", function () {
                var event = new CustomEvent("updateProperty", {
                    detail: {
                        name: options.propertyName,
                        value: mediumEditor.getContent(),
                        refreshMarkup: false
                    }
                });

                editor.dispatchEvent(event);
            });
        },

        destroy: function (options) {
            var mediumEditor = MediumEditor.getEditorFromElement(options.editor);

            if (mediumEditor) {
                mediumEditor.destroy();
            }
        }
    });
})();
