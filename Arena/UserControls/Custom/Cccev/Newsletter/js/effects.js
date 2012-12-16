
$(document).ready(function () {

	initEffectEvents();

});

function initEffectEvents() {

	// set up a handler to enable/unhide the save link when a checkbox is changed.
	$("input[type=checkbox]").click(function () {
		// <table><tbody><tr><td><input...> ... </table><a class="buttonSaveGroups">
		$(this).parent().parent().parent().parent().next("a.buttonSaveGroups").show();
	});

    var $message = $("[class*='fadingMessage']");
    if ($message.length > 0) {
        $message.show().animate({ opacity: 1.0 }, 3000).fadeOut(2000,
            function() {
                $(this).html('');
            });
    }
}