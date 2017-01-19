(function($) {
   $(document).ready(function () {
    $('#btnSubmit').click(submitEditorForm);
    $('#btnReport').click(submitReportForm);
     $('#btnSkip').click(skipCurrent);

    function submitEditorForm(){
        console.log('Submitting!');
        $('#btnSubmit').removeClass('uk-button-primary');
        $('#btnSubmit').text("提交中...");
        $("#editorForm").submit();
    }

    var form = document.querySelector('#editorForm');
    Mousetrap.bind('mod+s', function (e) {
        console.log(e);
        if (e.preventDefault) {
            e.preventDefault();
        } else {
            // internet explorer
            e.returnValue = false;
        }
        submitEditorForm();
    });
    Mousetrap(form).bind('mod+s', function (e) {
        console.log(e);
        if (e.preventDefault) {
            e.preventDefault();
        } else {
            // internet explorer
            e.returnValue = false;
        }
        submitEditorForm();
    });

    function skipCurrent() {
        $("#skipForm").submit();
    }

    function submitReportForm(){
        console.log('Reporting!');
        $("#reportForm").submit();
    }

    Mousetrap.bind('f2', function (e) {
        console.log(e);
        if (e.preventDefault) {
            e.preventDefault();
        } else {
            // internet explorer
            e.returnValue = false;
        }
        skipCurrent();
    });
    Mousetrap(form).bind('f2', function (e) {
        console.log(e);
        if (e.preventDefault) {
            e.preventDefault();
        } else {
            // internet explorer
            e.returnValue = false;
        }
        skipCurrent();
    });
   });
}(jQuery));