class CentralAZ.Facebook.Helpers.Authentication
	@initConnect: (person) ->
		theModel = new CentralAZ.Facebook.Models.FacebookUser person
		window.facebookConnect = new CentralAZ.Facebook.Controllers.FacebookConnectRouter model: theModel, events: facebookEvents
		try
			Backbone.history.start()
		catch error
		Authentication.registerFacebookApi()
	@initLogin: ->
		window.facebookAuth = new CentralAZ.Facebook.Controllers.FacebookLoginRouter model: null, events: facebookEvents
		try
			Backbone.history.start()
		catch error
		Authentication.registerFacebookApi()
	@registerFacebookApi: ->
		window.fbAsyncInit = ->
			FB.init
				appId: Authentication.config.appID
				status: true
				cookie: true
				xfbml: true
				oauth: true
		e = document.createElement 'script'
		e.src = "#{document.location.protocol}//connect.facebook.net/en_US/all.js"
		e.async = true
		document.getElementById('fb-root').appendChild e