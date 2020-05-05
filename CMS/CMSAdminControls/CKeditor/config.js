/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
    config.allowedContent = true; // To disable CKEditor ACF
    config.enterMode = CKEDITOR.ENTER_BR;
    config.shiftEnterMode = CKEDITOR.ENTER_P;
    config.entities_latin = false;

    var sourceName = config.useInlineMode ? 'Sourcedialog' : 'Source';

    config.toolbar_Standard = config.toolbar_Default =
    [
        [sourceName, '-'],
        ['Undo', 'Redo', '-'],
        ['Bold', 'Italic', 'Underline', 'TextColor', '-'],
        ['Styles'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
        ['InsertLink', 'Unlink', '-'],
        ['InsertImageOrMedia', 'QuicklyInsertImage', 'Table', 'InsertMacro', '-'],
        ['Maximize']
    ];

    config.toolbar_Full = [
        [sourceName, '-'],
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', 'Scayt', '-'],
        ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
        ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent', 'Blockquote', 'CreateDiv', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
        '/',
        ['InsertLink', 'Unlink', 'Anchor', '-'],
        ['InsertImageOrMedia', 'QuicklyInsertImage', 'Table', 'HorizontalRule', 'SpecialChar', '-'],
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor', '-'],
        ['InsertMacro', '-'],
        ['Maximize', 'ShowBlocks']
    ];

    config.toolbar_Basic = [
        ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'InsertLink', 'Unlink']
    ];

    config.toolbar_BizForm = [
        ['Source', '-'],
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-'],
        ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
        ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent', '-'],
        ['InsertLink', 'Unlink', 'Anchor', '-'],
        ['InsertImageOrMedia', 'Table', 'HorizontalRule', 'SpecialChar', '-'],
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor', '-'],
        ['InsertMacro', '-'],
        ['Maximize']
    ];

    config.toolbar_Forum = [
        ['Bold', 'Italic', '-', 'InsertLink', 'InsertUrl', 'InsertImageOrMedia', 'InsertImage', 'InsertQuote', '-', 'NumberedList', 'BulletedList', '-', 'TextColor', 'BGColor']
    ];

    config.toolbar_Reporting = [
        ['Source', '-'],
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-'],
        ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
        ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
        ['InsertLink', 'Unlink', 'Anchor', '-'],
        ['InsertImageOrMedia', 'QuicklyInsertImage', 'Table', 'HorizontalRule', 'SpecialChar', '-'],
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor', '-'],
        ['InsertMacro', '-'],
        ['Maximize']
    ];

    config.toolbar_SimpleEdit = [
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-'],
        ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
        ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
        ['InsertLink', 'Unlink', 'Anchor', '-'],
        ['InsertImageOrMedia', 'QuicklyInsertImage', 'Table', 'HorizontalRule', 'SpecialChar', '-'],
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor', '-'],
        ['Maximize']
    ];

    config.toolbar_Invoice = [
        ['Source', '-'],
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', 'Scayt', '-'],
        ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
        ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent', 'Blockquote', 'CreateDiv', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
        ['InsertImageOrMedia', 'Table', 'HorizontalRule', 'SpecialChar', '-'],
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor', '-'],
        ['InsertMacro', '-'],
        ['Maximize', 'ShowBlocks']
    ];

    config.toolbar_Group = [
        ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'InsertLink', 'Unlink']
    ];

    config.toolbar_Widgets = [
        ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'InsertLink', 'Unlink', 'InsertImageOrMedia', '-'],
        ['Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor']
    ];

    config.toolbar_EmailWidgets = [
        ['Bold', 'Italic', 'Underline', '-', 'NumberedList', 'BulletedList', '-', 'PasteText', 'PasteFromWord', '-', 'InsertMacro', '-']
    ];

    config.toolbar_Consents_ShortText = [
        ['Source', '-', 'Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'InsertLink', 'Unlink', '-', 'PasteText', 'PasteFromWord']
    ];

    config.toolbar_Consents_FullText = [
        ['Source', '-', 'Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'InsertLink', 'Unlink', '-', 'PasteText', 'PasteFromWord'],
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor', '-']
    ];

    config.toolbar_Disabled = [
        ['Maximize']
    ];

    config.toolbar = config.toolbar_Standard;

    config.scayt_customerid = '1:vhwPv1-GjUlu4-PiZbR3-lgyTz1-uLT5t-9hGBg2-rs6zY-qWz4Z3-ujfLE3-lheru4-Zzxzv-kq4';
};
