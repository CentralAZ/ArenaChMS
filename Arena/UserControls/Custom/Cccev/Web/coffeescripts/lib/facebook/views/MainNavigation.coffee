# TODO: Rather than calling 'registerEvents()', hook into Backbone.View events API 
# TODO: Rather than using a @container instance variable, hook into Backbone.View's existing @el property
class CentralAZ.Web.Views.MainNavigation
	constructor: ->
		@hovering = false
	init: ->
		instance = @
		$(".main-nav > li > a").click ->
			$ul = $(this).next "ul"
			if $ul.length is 0 then return true
			if $ul.is ":visible" then $ul.fadeOut("fast").removeClass "selected"
			else
				CentralAZ.Web.Views.MainNavigation.hideNav()
				$ul.fadeIn("fast").addClass "selected"
			false
		.hover ->
			instance.hovering = true
			$ul = $(this).next "ul"
			if $ul.length > 0 and $ul.is ":hidden"
				CentralAZ.Web.Views.MainNavigation.hideNav()
				$ul.fadeIn("fast").addClass "selected"
			true
		,
		->
			if instance.hovering is false then CentralAZ.Web.Views.MainNavigation.hideNav()
			true

		$(document).click =>
			if @hovering is false then CentralAZ.Web.Views.MainNavigation.hideNav()
			true

		# Use fat arrow to pass "this" into event handler
		$(".main-nav > li > ul").hover => 
			@hovering = true
			true
		,
		=>
			@hovering = false
			true

	#Static Methods
	@hideNav: ->
		$(".main-nav > li > ul").fadeOut("fast").removeClass "selected"
		#console.log "howdy from static method"
		false

$ ->
	window.nav = new CentralAZ.Web.Views.MainNavigation()
	nav.init()