class CentralAZ.Facebook.Controllers.FacebookLoginRouter extends Backbone.Router
	routes:
		"": "index"
		"!/log-in/:status": "index"
		"!/sign-up": "signUp"
		"*other": "index" # Default/catchall route
	initialize: (options) ->
		@user = options.model
		@ev = options.events
		@indexView = new CentralAZ.Facebook.Views.FacebookLogin events: @ev
		_.bindAll(@)
		# Subsribe to users successfully logging into/out of Facebook and declining permissions
		@ev.bind 'facebookLoggedIn', @onLogin
		@ev.bind 'facebookLogOut', @onLogOut
		@ev.bind 'userDeclined', @onDecline
		@ev.bind 'arenaLoginFailed', @onArenaLoginFail
	index: (status = '') ->
		# TODO: Do something better than booing the user when they decline Facebook
		if status is 'declined' then alert 'boo!'
		@indexView.render()
	signUp: ->
		if @user is null
			@navigate '!/log-in', true
			return
		@signUpView = new CentralAZ.Facebook.Views.FacebookSignUp model: @user, events: @ev
		@signUpView.render()
	onLogin: (response) ->
		query = CentralAZ.Web.Helpers.UrlHelper.parseQueryString()
		if query.error_reason?
			@user = new CentralAZ.Facebook.Models.FacebookUser name: 'anonymous person'
			@navigate '!/sign-up', true
			return
		if !response then return @navigate '!/log-in', true
		@user = new CentralAZ.Facebook.Models.FacebookUser response
		@userInfoView = new CentralAZ.Facebook.Views.FacebookWelcome model: @user, events: @ev
		@userInfoView.render()
		@ev.trigger 'arenaLogin'
	onLogOut: -> @navigate '!/log-in', true
	onDecline: -> @navigate '!/log-in/declined', true
	onArenaLoginFail: -> @navigate '!/sign-up', true