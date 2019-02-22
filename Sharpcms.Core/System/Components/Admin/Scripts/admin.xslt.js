$(function () {
    // Hyperlinks (Commonactions)
    var $hlSave = $('.hlSave');
    var $hlSaveAndShow = $('.hlSaveAndShow');
    var $hlShow = $('.hlShow');

    // Hyperlinks (Pages)
    var $hlRemoveContrainer = $('.hlRemoveContrainer');
    var $hlRemoveElement = $('.hlRemoveElement');
    var $hlMoveElementTop = $('.hlMoveElementTop');
    var $hlMoveElementDown = $('.hlMoveElementDown');
    var $hlMoveElementUp = $('.hlMoveElementUp');
    var $hlCopyElement = $('.hlCopyElement');
    var $hlAddPage = $('.hlAddPage');
    var $hlThrowEvent = $('.hlThrowEvent');

    // Hyperlinks (Files)
    var $hlUploadFile = $('.hlUploadFile');
    var $hlRemoveFile = $('.hlRemoveFile');
    var $hlRenameFile = $('.hlRenameFile');
    var $hlMoveFile = $('.hlMoveFile');
    var $hlResizeFile = $('.hlResizeFile');
    var $hlAddFolder = $('.hlAddFolder');
    var $hlRemoveFolder = $('.hlRemoveFolder');
    var $hlRenameFolder = $('.hlRenameFolder');
    var $hlMoveFolder = $('.hlMoveFolder');
    var $hlMoreFiles = $('.hlMoreFiles');

    // Hyperlinks (Users/Groups)
    var $hlAddUser = $('.hlAddUser');
    var $hlAddGroup = $('.hlAddGroup');
    var $hlDeleteGroup = $('.hlDeleteGroup');
    var $hlSaveUser = $('.hlSaveUser');
    var $hlDeleteUser = $('.hlDeleteUser');

    // Hyperlinks (Choose)
    var $hlChoosePage = $('.hlChoosePage');
    var $hlChooseFile = $('.hlChooseFile');
    var $hlChooseFolder = $('.hlChooseFolder');
    var $hlChooseImage = $('.hlChooseImage');

    $hlSave.click(function () {
        Sharpcms.Actions.SavePage();
    });

    $hlSaveAndShow.click(function () {
        Sharpcms.Actions.SaveAndPreviewPage();
    });

    $hlShow.click(function () {
        Sharpcms.Actions.PreviewPage();
    });

    $hlRemoveContrainer.click(function () {
        var containerId = $(this).data('containerid');

        Sharpcms.Actions.RemovePageContainer(containerId);
    });

    $hlRemoveElement.click(function () {
        var containerId = $(this).data('containerid');
        var elementId = $(this).data('elementid');

        Sharpcms.Actions.RemoveElement(containerId, elementId);
    });

    $hlMoveElementTop.click(function () {
        var containerId = $(this).data('containerid');
        var elementId = $(this).data('elementid');

        Sharpcms.Actions.MoveElementTop(containerId, elementId);
    });

    $hlMoveElementDown.click(function () {
        var containerId = $(this).data('containerid');
        var elementId = $(this).data('elementid');

        Sharpcms.Actions.MoveElementDown(containerId, elementId);
    });

    $hlMoveElementUp.click(function () {
        var containerId = $(this).data('containerid');
        var elementId = $(this).data('elementid');

        Sharpcms.Actions.MoveElementUp(containerId, elementId);
    });

    $hlCopyElement.click(function () {
        var containerId = $(this).data('containerid');
        var elementId = $(this).data('elementid');

        Sharpcms.Actions.CopyElement(containerId, elementId);
    });

    $hlAddPage.click(function () {
        var pageId = $(this).data('pageid');

        Sharpcms.Actions.AddPage(pageId);
    });

    $hlChoosePage.click(function () {
        var attributeValue = $(this).data('value').split(Sharpcms.Common.Splitters);
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        Sharpcms.Actions.ChoosePage(id, attribute);
    });

    $hlChooseFile.click(function () {
        var attributeValue = $(this).data('value').split(Sharpcms.Common.Splitters);
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        Sharpcms.Actions.ChooseFile(id, attribute);
    });

    $hlChooseFolder.click(function () {
        var attributeValue = $(this).data('value').split(Sharpcms.Common.Splitters);
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        Sharpcms.Actions.ChooseFolder(id, attribute);
    });

    $hlChooseImage.click(function() {
        var attributeValue = $(this).data('value').split(Sharpcms.Common.Splitters);
        var id = attributeValue[0];
        var attribute = attributeValue[1];

        Sharpcms.Actions.ChooseImage(id, attribute);
    });

    $hlThrowEvent.click(function () {
        var action = $(this).data('action');

        Sharpcms.Actions.ThrowEvent(action, '', '');
    });

    $hlUploadFile.click(function () {
        var path = $(this).data('path');

        Sharpcms.Actions.UploadFile(path);
    });

    $hlRemoveFile.click(function () {
        var path = $(this).data('path');

        Sharpcms.Actions.RemoveFile(path);
    });

    $hlRenameFile.click(function () {
        var path = $(this).data('path');

        Sharpcms.Actions.RenameFile(path);
    });

    $hlMoveFile.click(function () {
        var path = $(this).data('path');

        Sharpcms.Actions.MoveFile(path);
    });

    $hlResizeFile.click(function () {
        Sharpcms.Actions.ResizeImage();
    });

    $hlAddFolder.click(function () {
        var path = $(this).data('path');

        Sharpcms.Actions.AddFolder(path);
    });

    $hlRemoveFolder.click(function () {
        var path = $(this).data('path');

        Sharpcms.Actions.RemoveFolder(path);
    });

    $hlRenameFolder.click(function () {
        var path = $(this).data('path');

        Sharpcms.Actions.RenameFolder(path);
    });

    $hlMoveFolder.click(function () {
        var path = $(this).data('path');

        Sharpcms.Actions.MoveFolder(path);
    });

    $hlMoreFiles.click(function () {
        var currentlevel = $(this).data('currentlevel');
        var level = parseInt(currentlevel) + 1;

        $('#file_' + level).show();
        $('#showlink_' + level).hide();
    });

    $hlAddUser.click(function () {
        Sharpcms.Actions.AddUser();
    });

    $hlSaveUser.click(function () {
        var userName = $(this).data('username');

        Sharpcms.Actions.SaveUser(userName);
    });

    $hlDeleteUser.click(function () {
        var userName = $(this).data('username');

        Sharpcms.Actions.DeleteUser(userName);
    });

    $hlAddGroup.click(function () {
        Sharpcms.Actions.AddGroup();
    });

    $hlDeleteGroup.click(function () {
        var groupName = $(this).data('groupName');

        Sharpcms.Actions.DeleteGroup(groupName);
    });

    $('#adminmoreactions').change(function () {
        var $selectedOption = $(this).find('option:selected');
        var attributeAction = $selectedOption.data('action');
        var attributeValue = $selectedOption.val().split(Sharpcms.Common.Splitters);

        if (attributeAction == Sharpcms.ActionType.RemovePage) {
            Sharpcms.Actions.RemovePage(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.AddPage) {
            Sharpcms.Actions.AddPage(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.CreatePageContainer) {
            Sharpcms.Actions.CreatePageContainer();
        } else if (attributeAction == Sharpcms.ActionType.MovePageUp) {
            Sharpcms.Actions.MovePageUp(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.MovePageDown) {
            Sharpcms.Actions.MovePageDown(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.MovePageTop) {
            Sharpcms.Actions.MovePageTop(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.MovePageBottom) {
            Sharpcms.Actions.MovePageBottom(attributeValue[0]);
        } else if (attributeAction == Sharpcms.ActionType.MovePage) {
            Sharpcms.Actions.MovePage();
        } else if (attributeAction == Sharpcms.ActionType.CopyPage) {
            Sharpcms.Actions.CopyPage(attributeValue[0], attributeValue[1]);
        } else if (attributeAction == Sharpcms.ActionType.SetAsStartPage) {
            Sharpcms.Actions.SetAsStartPage();
        }
    });

    $('.addelement').change(function () {
        var elementType = $(this).find('option:selected').val();
        var containerId = $(this).data('name').replace('data_container_', '');

        Sharpcms.Actions.AddElement(containerId, elementType);
    });

    // TinyMCE
    $('textarea.mceeditor').tinymce({
        script_url: '/System/Components/Admin/Scripts/tiny_mce/tiny_mce.js',
        theme: 'advanced',
        plugins: 'sharpcmschooser,fullscreen,spellchecker,safari,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template',
        skin: 'sharpcms',
        theme_advanced_buttons1: 'fullscreen,formatselect,cleanup,forecolor,bold,italic,underline,strikethrough,justifyleft,justifycenter,justifyright,image,indent,outdent,bullist,numlist,undo,redo,link,unlink',
        theme_advanced_buttons2: 'tablecontrols,|,sharpcmslinkchooser,sharpcmsfilechooser,sharpcmsimagechooser,|,code',
        theme_advanced_buttons3: '',
        theme_advanced_buttons4: '',
        theme_advanced_toolbar_location: 'top',
        theme_advanced_toolbar_align: 'left',
        theme_advanced_statusbar_location: 'bottom',
        theme_advanced_resizing: false,
        relative_urls: false,
        content_css: '/System/Components/Admin/Styles/tiny_mce/tinystyle.css'
    });

    // Dialogs
    $('.choose').hide();

    // Select
    $('.pagedata_menu select').selectmenu({ width: 441 });
    $('.container_menu select').selectmenu({ width: 624 });

    // TreeView
    $('.filetree').treeview({ persist: 'location', collapsed: true, unique: false });

    // Tabs
    $('.tabs').tabs();
    
    // Dirty Hack :-)
    $(window).resize(function () {
        setContentWidth();
    });

    $('.treeview li').click(function () {
        setContentWidth();
    });

    function setContentWidth() {
        var leftWidth = $('div.content div.left').width();
        var rightWidth = $('div.content div.right').width();

        $('div.header').css('min-width', parseInt(leftWidth) + parseInt(rightWidth) + 20);
        $('div.mainnavi').css('min-width', parseInt(leftWidth) + parseInt(rightWidth) + 20);
        $('div.content').css('min-width', parseInt(leftWidth) + parseInt(rightWidth) + 20);

        if ($(document).width() <= parseInt(leftWidth) + parseInt(rightWidth) + 20) {
            $('.messages').css('width', parseInt(leftWidth) + parseInt(rightWidth) + 20);
        } else {
            $('.messages').css('width', '100%');
        }
    }

    function preview() {
        var param = $.url.param('preview');

        if (param == 'true') {
            var page = $("input[name=page]").val();

            if (page != null) {
                window.open('/show/' + page);
                location.href = location.href.split("?")[0];
            }
        }
    }

    setContentWidth();
    preview();
});