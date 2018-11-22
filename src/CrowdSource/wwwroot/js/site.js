// Write your Javascript code.

$(document).ready(function () {

    // set empty
    $("#BUCSetEmptyBtn").click(function () {
        $("#TextBUC").val("{empty}");
    });
    
    if ($('#IsPivotRow').is(":checked")) {
        $("#PivotRowSpecificFields").show();
    } else {
        $("#PivotRowSpecificFields").hide();
    }

    $('#IsPivotRow').click(function () {
        if ($(this).is(':checked')) {
            $("#PivotRowSpecificFields").show();
        } else {
            $("#PivotRowSpecificFields").hide();
        }
    }); 

    // 在校对状态下
    var btnSubmit = $("#btnSubmit");
    var btnReview = $("#btnReview");
    btnReview.click(onReviewSubmit);
    var promptModified = $("#promptModified");
    var isDirty = false;
    // when reviewing
    function fieldChanged(e) {
        //  btnReview.hide();
        isDirty = true;
        promptModified.text("*你做出了修改。若要保存新的修改，请按“保存修改”。如果是误修改，请按“这条没错”。");
        btnSubmit.show();
    }
    function onReviewSubmit(e) {
        if (isDirty) {
            if (!confirm("确定要标记为【这条没错】吗？")) {
                e.preventDefault();
            }
        }
    }
    // bind events to form
    $('#TextChinese').change(fieldChanged);
    $('#TextBUC').change(fieldChanged);
    $('#TextEnglish').change(fieldChanged);
    $('#Radical').change(fieldChanged);
    $('#BoPoMoFo').change(fieldChanged);
    $('#IsPivotRow').change(fieldChanged);
    $('#IsOral').change(fieldChanged);
    $('#IsLiterary').change(fieldChanged);

    $('#btnReport').click(function () {
        $('#reportForm').submit();
    });
});

