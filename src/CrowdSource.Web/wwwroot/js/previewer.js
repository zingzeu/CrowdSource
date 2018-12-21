(function ($) {
    $(document).ready(function () {
        updatePreview();
        showhidePreviewer();

        function updatePreview() {
            $("#previewBUC").text(
                $("#TextBUC").val().trim() == "{empty}" ?
                    ""
                    :
                    $("#TextBUC").val()
            );
            $("#previewEnglish").text($("#TextEnglish").val());
            $("#previewChinese").text($("#TextChinese").val());
            $("#previewRadical").text(
                $("#IsPivotRow").is(':checked') ?
                    $("#Radical").val() : ""
            );
            $("#previewBoPoMoFo").text(
                $("#IsPivotRow").is(':checked') ?
                    $("#BoPoMoFo").val() : ""
            );
            $("#previewLabel").text(
                (($("#IsOral").is(':checked')) ? "俗" : "") +
                (($("#IsLiterary").is(':checked')) ? "文" : "")
            );
        }

        function getPreviewerState() {
            var currentState = localStorage.getItem("ShowPreviewer");
            if (currentState == null) currentState = false;
            return currentState == "true";
        }

        function showhidePreviewer() {
            if (getPreviewerState()) {
                $('#previewPanel').show();
            } else {
                $('#previewPanel').hide();
            }
        }

        function togglePreviewer() {
            var newState = !getPreviewerState();
            localStorage.setItem("ShowPreviewer", newState ? "true" : "false");
            showhidePreviewer();
        }

        $("#previewerToggle").click(togglePreviewer);


        // bind events to form
        $('#TextChinese').change(updatePreview);
        $('#TextBUC').change(updatePreview);
        $('#TextEnglish').change(updatePreview);
        $('#Radical').change(updatePreview);
        $('#BoPoMoFo').change(updatePreview);
        $('#IsPivotRow').change(updatePreview);
        $('#IsOral').change(updatePreview);
        $('#IsLiterary').change(updatePreview);
    });
} (jQuery));