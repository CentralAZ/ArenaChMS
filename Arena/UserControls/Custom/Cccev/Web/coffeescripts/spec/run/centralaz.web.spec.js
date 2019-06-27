(function() {
  describe('FacebookAuth Feature', function() {
    describe('Controller Classes', function() {
      beforeEach(function() {
        return this.controller = new CentralAZ.Web.Controllers.FacebookController({
          model: new CentralAZ.Web.Models.FacebookUser()
        });
      });
      afterEach(function() {
        return window.location.hash = '';
      });
      describe('Routing Definitions', function() {
        beforeEach(function() {
          this.controller.defaults.authenticateInArena = false;
          return this.routeSpy = jasmine.createSpy();
        });
        it('should fire the index route with a blank hash', function() {
          this.controller.bind('route:index', this.routeSpy);
          window.location.hash = '';
          Backbone.history.loadUrl();
          expect(this.routeSpy).toHaveBeenCalled();
          return expect(this.routeSpy).toHaveBeenCalledWith();
        });
        it('should fire the index route with a log-in hash', function() {
          this.controller.bind('route:index', this.routeSpy);
          window.location.hash = '#!/log-in';
          Backbone.history.loadUrl();
          expect(this.routeSpy).toHaveBeenCalled();
          return expect(this.routeSpy).toHaveBeenCalledWith('/log-in');
        });
        it('should fire the index route with a bogus hash', function() {
          this.controller.bind('route:index', this.routeSpy);
          window.location.hash = '#foo';
          Backbone.history.loadUrl();
          expect(this.routeSpy).toHaveBeenCalled();
          return expect(this.routeSpy).toHaveBeenCalledWith('foo');
        });
        it('should fire the signUp route with a sign-up hash', function() {
          this.controller.bind('route:signUp', this.routeSpy);
          window.location.hash = '#!/sign-up';
          Backbone.history.loadUrl();
          expect(this.routeSpy).toHaveBeenCalled();
          return expect(this.routeSpy).toHaveBeenCalledWith();
        });
        return it('should fire the oauth route with an oauth hash', function() {
          this.controller.bind('route:oauth', this.routeSpy);
          window.location.hash = '#!/oauth';
          Backbone.history.loadUrl();
          expect(this.routeSpy).toHaveBeenCalled();
          return expect(this.routeSpy).toHaveBeenCalledWith();
        });
      });
      describe('Index Handler', function() {
        return it('should show index view', function() {
          var view;
          window.location.hash = '';
          Backbone.history.loadUrl();
          view = this.controller.indexView;
          return expect(view).not.toBeNull();
        });
      });
      describe('Signup Handler', function() {
        return it('should show signup view', function() {
          var view;
          window.location.hash = '#!/sign-up';
          Backbone.history.loadUrl();
          view = this.controller.signUpView;
          return expect(view).not.toBeNull();
        });
      });
      return describe('OAuth Handler', function() {
        beforeEach(function() {
          this.controller.defaults.authenticateInArena = false;
          this.routeSpy = jasmine.createSpy();
          this.controller.defaults.onAuthSuccess = jasmine.createSpy();
          return this.controller.defaults.onAuthError = jasmine.createSpy();
        });
        it('should redirect to signUp if "error_reason" found in query string', function() {
          this.controller.bind('route:signUp', this.routeSpy);
          spyOn(CentralAZ.Web.Helpers.UrlHelper, 'parseQueryString').andReturn({
            error_reason: 'foobar'
          });
          window.location.hash = '#!/oauth';
          Backbone.history.loadUrl();
          waitsFor(function() {
            return window.location.hash === '#!/sign-up';
          });
          runs(function() {
            return Backbone.history.loadUrl();
          });
          runs(function() {
            return expect(this.routeSpy).toHaveBeenCalled();
          });
          return runs(function() {
            return expect(this.routeSpy).toHaveBeenCalledWith();
          });
        });
        return it('should show user info view', function() {
          var view;
          window.location.hash = '#!/oauth';
          Backbone.history.loadUrl();
          view = this.controller.userInfoView;
          return expect(view).not.toBeNull();
        });
      });
    });
    return describe('View Classes', function() {
      afterEach(function() {
        return $('.fb-auth > div').hide();
      });
      describe('Index View', function() {
        beforeEach(function() {
          return this.view = new CentralAZ.Web.Views.FacebookLogin;
        });
        return it('should show login button', function() {
          var $el;
          this.view.render();
          $el = $(this.view.el);
          expect($el.length).toBeGreaterThan(0);
          return expect($el.is(':visible')).toBeTruthy();
        });
      });
      describe('User Info View', function() {
        beforeEach(function() {
          var user;
          user = new CentralAZ.Web.Models.FacebookUser({
            name: 'Jon Doe'
          });
          return this.view = new CentralAZ.Web.Views.FacebookWelcome({
            model: user
          });
        });
        return it('should render greeting', function() {
          var $el;
          this.view.render();
          $el = $(this.view.el);
          return expect($el.html() === '').not.toBeTruthy();
        });
      });
      return describe('Signup View', function() {
        beforeEach(function() {
          var user;
          user = new CentralAZ.Web.Models.FacebookUser({
            name: 'Jon Doe'
          });
          return this.view = new CentralAZ.Web.Views.FacebookSignUp({
            model: user
          });
        });
        it('should show signup form', function() {
          var $el;
          this.view.render();
          $el = $(this.view.el);
          waitsFor(function() {
            return $el.is(":visible");
          });
          runs(function() {
            return expect($el.length).toBeGreaterThan(0);
          });
          return runs(function() {
            return expect($el.is(":visible")).toBeTruthy();
          });
        });
        return it('should render greeting', function() {
          var $greeting;
          this.view.render();
          $greeting = $(this.view.el).children(".greeting");
          return expect($greeting.html() === '').not.toBeTruthy();
        });
      });
    });
  });
}).call(this);
