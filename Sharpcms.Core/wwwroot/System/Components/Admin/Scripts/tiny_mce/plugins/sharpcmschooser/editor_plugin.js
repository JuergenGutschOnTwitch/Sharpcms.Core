/**
* $Id: sharpcmschooser_src.js 201 2007-02-12 15:56:56Z spocke $
*
* @author Thomas Huber
* @copyright Copyright © 2013, sharpcms.org, All rights reserved.
*/

(function () {
    tinymce.PluginManager.requireLangPack('sharpcmschooser');

    tinymce.create('tinymce.plugins.SharpcmsChooserPlugin', {
        init: function (ed, url) {
            ed.addCommand('mceInsertPageLink', function () {
                Sharpcms.Actions.ChoosePageDialog(function (path) {
                    var selection = ed.selection.getContent({ format: 'text' });
                    if (selection == '') {
                        selection = path;
                    }

                    var html = '<a href="' + Sharpcms.Common.BaseUrl + 'show/' + path + '/">' + selection + '</a>';

                    ed.execCommand('mceInsertContent', false, html);
                });
            });

            ed.addButton('sharpcmslinkchooser', {
                title: 'insert Link',
                cmd: 'mceInsertPageLink',
                image: url + '/img/internal_link.png'
            });

            ed.addCommand('mceInsertFileLink', function () {
                Sharpcms.Actions.ChooseFileDialog(function (path) {
                    var selection = ed.selection.getContent({ format: 'text' });
                    if (selection == '') {
                        selection = path;
                    }
                    
                    var html = '<a href="' + Sharpcms.Common.BaseUrl + 'download/' + path + '/?download=true">' + selection + '</a>';
                    
                    ed.execCommand('mceInsertContent', false, html);
                });
            });

            ed.addButton('sharpcmsfilechooser', {
                title: 'insert File',
                cmd: 'mceInsertFileLink',
                image: url + '/img/page_white_add.png'
            });

            ed.addCommand('mceInsertImage', function () {
                Sharpcms.Actions.ChooseImageDialog(function (path) {
                    var html = '<img src="' + Sharpcms.Common.BaseUrl + 'download/' + path + '/" />';

                    ed.execCommand('mceInsertContent', false, html);
                });
            });

            ed.addButton('sharpcmsimagechooser', {
                title: 'insert Picture',
                cmd: 'mceInsertImage',
                image: url + '/img/image_add.png'
            });
        },

        createControl: function () {
            return null;
        },

        getInfo: function () {
            return {
                longname: 'sharpcms Chooser',
                author: 'Thomas Huber',
                authorurl: 'http://www.klickflupp.ch',
                infourl: 'http://www.sharpcms.org',
                version: '1.1'
            };
        }
    });

    // Register plugin
    tinymce.PluginManager.add('sharpcmschooser', tinymce.plugins.SharpcmsChooserPlugin);
})();