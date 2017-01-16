(function ($) {
    $(document).ready(function () {
        window.editorOptions = {
            "all": {
                "buc": true,
                "chi": true,
                "eng": true
            },
            "buc":{
                "buc": true,
                "chi": false,
                "eng": false                
            },
            "chi":{
                "buc": false,
                "chi": true,
                "eng": false                
            },
            "eng":{
                "buc": false,
                "chi": false,
                "eng": true                
            }
        };
        window.autofocusItem = {
            "all": "#TextBUC",
            "buc": "#TextBUC",
            "chi": "#TextChinese",
            "eng": "#TextEnglish"
        }
        $("#editorMode").val(getEditorMode());
        switchEdtior();
        
        function getEditorMode() {
            var currentState = localStorage.getItem("EditorMode");
            if (currentState == null) currentState = "all";
            return currentState;
        }

        function toggleIndicator(name, state) {
            $("#ModeIndicator__"+name+"__on").toggle(!!state);
            $("#ModeIndicator__"+name+"__off").toggle(!state);
    }

        function switchEdtior() {
            var mode = getEditorMode();
            console.log(window.editorOptions[mode]);
            $("#editorBUC").toggle(!!window.editorOptions[mode]['buc']);
            $("#editorChinese").toggle(!!window.editorOptions[mode]['chi']);
            $("#editorEnglish").toggle(!!window.editorOptions[mode]['eng']);
            toggleIndicator('buc',window.editorOptions[mode]['buc']);
            toggleIndicator('chi',window.editorOptions[mode]['chi']);
            toggleIndicator('eng',window.editorOptions[mode]['eng']);
            $(window.autofocusItem[mode]).focus();
        }

        function toggleEditor() {
            var newState = $("#editorMode").val();
            localStorage.setItem("EditorMode", newState);
            switchEdtior();
        }

        $("#editorMode").change(toggleEditor);

    });
} (jQuery));