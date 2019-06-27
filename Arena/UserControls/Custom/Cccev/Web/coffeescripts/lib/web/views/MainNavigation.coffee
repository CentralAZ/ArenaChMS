class CentralAZ.Web.Views.MainNavigation
	constructor: ->
		@hovering = false
	init: ->
		instance = @
		$(".main-nav > li > a").click ->
			$ul = $(this).next "ul"
			if $ul.length is 0 then return true
			else return false
		.hover ->
			instance.hovering = true
			$ul = $(this).next "ul"
			if $ul.length > 0 and $ul.is ":hidden"
				CentralAZ.Web.Views.MainNavigation.hideNav()
				$ul.fadeIn("fast").addClass "selected"
			true
		,
		->
			instance.hovering = false
			setTimeout ->
				if instance.hovering is false 
					CentralAZ.Web.Views.MainNavigation.hideNav()
			, 1000			
			true

		$(document).click =>
			if instance.hovering is false then CentralAZ.Web.Views.MainNavigation.hideNav()
			true

		# Use fat arrow to pass "this" into event handler
		$(".main-nav > li > ul").hover -> 
			instance.hovering = true
			true
		,
		->
			instance.hovering = false
			setTimeout ->
				if instance.hovering is false 
					CentralAZ.Web.Views.MainNavigation.hideNav()
			, 3000
			true

	#Static Methods
	@hideNav: ->
		$(".main-nav > li > ul").fadeOut("fast").removeClass "selected"
		#console.log "howdy from static method"
		false

$ ->
	window.nav = new CentralAZ.Web.Views.MainNavigation()
	nav.init()