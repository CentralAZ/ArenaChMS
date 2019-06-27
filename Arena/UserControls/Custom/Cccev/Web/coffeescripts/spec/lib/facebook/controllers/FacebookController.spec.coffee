describe 'FacebookAuth Feature', ->
	describe 'Controller Classes', ->
		beforeEach -> 
			@controller = new CentralAZ.Web.Controllers.FacebookLoginRouter model: new CentralAZ.Web.Models.FacebookUser()
	
		afterEach -> window.location.hash = ''
	
		describe 'Routing Definitions', ->
			beforeEach ->
				@controller.defaults.authenticateInArena = false
				@routeSpy = jasmine.createSpy()
		
			it 'should fire the index route with a blank hash', ->
				@controller.bind 'route:index', @routeSpy
				window.location.hash = ''
				Backbone.history.loadUrl()
				expect(@routeSpy).toHaveBeenCalled()
				expect(@routeSpy).toHaveBeenCalledWith()
			
			it 'should fire the index route with a log-in hash', ->
				@controller.bind 'route:index', @routeSpy
				window.location.hash = '#!/log-in'
				Backbone.history.loadUrl()
				expect(@routeSpy).toHaveBeenCalled()
				expect(@routeSpy).toHaveBeenCalledWith('/log-in')
			
			it 'should fire the index route with a bogus hash', ->
				@controller.bind 'route:index', @routeSpy
				window.location.hash = '#foo'
				Backbone.history.loadUrl()
				expect(@routeSpy).toHaveBeenCalled()
				expect(@routeSpy).toHaveBeenCalledWith('foo')
			
			it 'should fire the signUp route with a sign-up hash', ->
				@controller.bind 'route:signUp', @routeSpy
				window.location.hash = '#!/sign-up'
				Backbone.history.loadUrl()
				expect(@routeSpy).toHaveBeenCalled()
				expect(@routeSpy).toHaveBeenCalledWith()
			
			it 'should fire the oauth route with an oauth hash', ->
				@controller.bind 'route:oauth', @routeSpy
				window.location.hash = '#!/oauth'
				Backbone.history.loadUrl()
				expect(@routeSpy).toHaveBeenCalled()
				expect(@routeSpy).toHaveBeenCalledWith()
	
		describe 'Index Handler', ->
			it 'should show index view', ->
				window.location.hash = ''
				Backbone.history.loadUrl()
				view = @controller.indexView
				expect(view).not.toBeNull()
	
		describe 'Signup Handler', ->
			it 'should show signup view', ->
				window.location.hash = '#!/sign-up'
				Backbone.history.loadUrl()
				view = @controller.signUpView
				expect(view).not.toBeNull()
	
		describe 'OAuth Handler', ->
			beforeEach ->
				@controller.defaults.authenticateInArena = false
				@routeSpy = jasmine.createSpy()
				@controller.defaults.onAuthSuccess = jasmine.createSpy()
				@controller.defaults.onAuthError = jasmine.createSpy()
			
			it 'should redirect to signUp if "error_reason" found in query string', ->
				@controller.bind 'route:signUp', @routeSpy
				spyOn(CentralAZ.Web.Helpers.UrlHelper, 'parseQueryString').andReturn error_reason: 'foobar'
				window.location.hash = '#!/oauth'
				Backbone.history.loadUrl()
				waitsFor -> window.location.hash is '#!/sign-up'
				runs -> Backbone.history.loadUrl()
				runs -> expect(@routeSpy).toHaveBeenCalled()
				runs -> expect(@routeSpy).toHaveBeenCalledWith()
			
			it 'should show user info view', ->
				window.location.hash = '#!/oauth'
				Backbone.history.loadUrl()
				view = @controller.userInfoView
				expect(view).not.toBeNull()
	
	describe 'View Classes', ->
		afterEach -> $('.fb-auth > div').hide()
		
		describe 'Index View', ->
			beforeEach -> @view = new CentralAZ.Web.Views.FacebookLogin
			
			it 'should show login button', ->
				@view.render()
				$el = $(@view.el)
				expect($el.length).toBeGreaterThan 0
				expect($el.is(':visible')).toBeTruthy()
				
		describe 'User Info View', ->
			beforeEach -> 
				user = new CentralAZ.Web.Models.FacebookUser name: 'Jon Doe'
				@view = new CentralAZ.Web.Views.FacebookWelcome model: user
			
			it 'should render greeting', ->
				@view.render()
				$el = $(@view.el)
				expect($el.html() is '').not.toBeTruthy()
		
		describe 'Signup View', ->
			beforeEach -> 
				user = new CentralAZ.Web.Models.FacebookUser name: 'Jon Doe' 
				@view = new CentralAZ.Web.Views.FacebookSignUp model: user
			
			it 'should show signup form', ->
				@view.render()
				$el = $(@view.el)
				waitsFor -> $el.is(":visible")
				runs -> expect($el.length).toBeGreaterThan 0
				runs -> expect($el.is(":visible")).toBeTruthy()
				
			it 'should render greeting', ->
				@view.render()
				$greeting = $(@view.el).children(".greeting")
				expect($greeting.html() is '').not.toBeTruthy()
				
				