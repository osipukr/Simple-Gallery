(function($) {

    var $body = $('body');

	// Scrolly.
		$('.scrolly').scrolly();

		// Toggle.
			$(
				'<div id="headerToggle">' +
					'<a href="#header" class="toggle"><i class="fas fa-align-left"></i></a>' +
				'</div>'
			)
				.appendTo($body);

		// Header.
			$('#header')
				.panel({
					delay: 500,
					hideOnClick: true,
					hideOnSwipe: true,
					resetScroll: true,
					resetForms: true,
					side: 'left',
					target: $body,
					visibleClass: 'header-visible'
				});

})(jQuery);

function deleteAction(action) {
    $('#delete').click(function () {
        location.href = action;
    });
};

function myLocation(href) {
    location.href = href;
};

function logOut() {
    $('#logOutForm').submit();
}

$(document).ready(function () {

    //Lazy loader
    $('.lazy').lazyload({
        //event: "lazyload",
        effect: 'fadeIn',
        effectspeed: 500
    }).trigger("lazyload");

    
});