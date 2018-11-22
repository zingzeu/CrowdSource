(function($) {
    // tour
    if (typeof Tour !== "undefined") {
        var tour = new Tour({
            template: '<div class="popover" role="tooltip">' +
                '<div class="arrow"></div>' +
                '<h3 class="popover-title"></h3>'+
                '<div class="popover-content"></div>'+
                '<div class="popover-navigation">'+
                '  <div class="btn-group">'+
                '    <button class="btn btn-sm btn-default" data-role="prev">&laquo; 上一步</button>'+
                '    <button class="btn btn-sm btn-default" data-role="next">下一步 &raquo;</button>'+
                '    <button class="btn btn-sm btn-default"'+
                '            data-role="pause-resume"'+
                '            data-pause-text="暂停"'+
                '            data-resume-text="继续">暂停</button>'+
                '  </div>'+
                '  <button class="btn btn-sm btn-default" data-role="end">结束向导</button>'+
                '</div>'+
                '</div>',
            steps: [
                {
                    orphan: true,
                    title: "欢迎",
                    content: "本向导将带你熟悉《闽英大辞典》众包的界面。"
                },

                {
                    element:"#OriginalImage",
                    title:"原始图片",
                    content:"这是你要录入的图片。每个图片代表字典中的一个条目。每个图片左、中、右分为 罗马字、汉字、英文三个部分。",
                    backdrop: true,
                    backdropPadding: 5
                },
                    {
                    element:"#rightPanel",
                    title:"录入",
                    content:"你要做的是在右边录入左边图片里的内容。",
                    backdrop: true,
                    backdropPadding: 5,
                        placement: "left"
                },
                {
                    element:"#selector",
                    title:"录入模式",
                    content:"你可以选择只输入一种类型。比如，你可以只录入汉字。没有录入完的部分，会自动交给其他人录入。",
                    backdrop: true,
                    backdropPadding: 5,
                    placement: "bottom"
                },
                {
                    element:"#OriginalImage",
                    title:"录入模式指示器",
                    content:"图片下面的图标，提醒你正在录入哪个类型。",
                    backdrop: true,
                    backdropPadding: 5,
                    placement: "bottom",
                },
                {
                    element:"#editor",
                    title:"",
                    content:"有的时候，录入框里已经有内容了。这是之前电脑识别的结果，或是其他用户录入的。如果有错，就把它更正。",
                    backdrop: true,
                    backdropPadding: 5,
                    placement: "left"
                },
                {
                    element:"#PreviewPanel",
                    title:"预览",
                    content:"这是已录入内容的预览。",
                    backdrop: true,
                    backdropPadding: 5,
                    placement: "top"
                },
                    {
                    element:"#btnSubmit",
                    title:"提交",
                    content:"录入完一条，点这里提交。",
                    backdrop: true,
                    backdropPadding: 5,
                    placement: "top"
                },
                    {
                    element:"#btnSkip",
                    title:"跳过",
                    content:"或者点这里跳过，换一条。",
                    backdrop: true,
                    backdropPadding: 5,
                    placement: "top"
                },
                    {
                    element:"#btnReport",
                    title:"报告错误",
                    content:"如果图片不清楚、空白或者包含了多个条目，请点这里汇报。",
                    backdrop: true,
                    backdropPadding: 5,
                    placement: "top"
                },
                    {
                    orphan: true,
                    title: "开始行动吧",
                    content: "你可以点右上角的“帮助向导”重新启动此向导。"
                }
            ]
        });
        $(document).ready(function() {
            tour.init();
            tour.start();
            $("#RestartTour").click(function () {
                tour.restart();
            });
        });
    }
   
   /** end of tour */
   
   
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
        if (confirm("确定要标记此条为【图片不清楚】吗？若是不会输入请选择【换一条】。")) {
            console.log('Reporting!');
            $("#reportForm").submit();
        }
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