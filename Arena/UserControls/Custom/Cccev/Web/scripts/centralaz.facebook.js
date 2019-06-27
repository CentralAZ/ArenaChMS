(function() {
  var __hasProp = Object.prototype.hasOwnProperty, __extends = function(child, parent) {
    for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; }
    function ctor() { this.constructor = child; }
    ctor.prototype = parent.prototype;
    child.prototype = new ctor;
    child.__super__ = parent.prototype;
    return child;
  }, __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  window.CentralAZ = window.CentralAZ || {};
  CentralAZ.Facebook = CentralAZ.Facebook || {};
  CentralAZ.Facebook.Models = CentralAZ.Facebook.Models || {};
  CentralAZ.Facebook.Views = CentralAZ.Facebook.Views || {};
  CentralAZ.Facebook.Controllers = CentralAZ.Facebook.Controllers || {};
  CentralAZ.Facebook.Helpers = CentralAZ.Facebook.Helpers || {};
  CentralAZ.Facebook.Events = CentralAZ.Facebook.Events || {};
  CentralAZ.Facebook.Controllers.FacebookConnectRouter = (function() {
    __extends(FacebookConnectRouter, Backbone.Router);
    function FacebookConnectRouter() {
      FacebookConnectRouter.__super__.constructor.apply(this, arguments);
    }
    FacebookConnectRouter.prototype.routes = {
      '': 'index',
      '*options': 'index'
    };
    FacebookConnectRouter.prototype.initialize = function(options) {
      if (options.model) {
        this.user = options.model;
      }
      this.ev = options.events;
      this.user = options.model;
      this.indexView = new CentralAZ.Facebook.Views.FacebookConnectAccount({
        model: this.user,
        events: this.ev
      });
      _.bindAll(this);
      this.ev.bind('facebookLoggedIn', this.onLogin);
      this.ev.bind('userDeclined', this.onDecline);
      return this.ev.bind('connectAccountSuccessful', this.onConnected);
    };
    FacebookConnectRouter.prototype.index = function() {
      return this.indexView.render();
    };
    FacebookConnectRouter.prototype.onLogin = function(response) {
      this.user = new CentralAZ.Facebook.Models.FacebookUser(response);
      return this.ev.trigger('connectAccounts');
    };
    FacebookConnectRouter.prototype.onConnected = function() {
      console.log(this.user);
      this.connectedView = new CentralAZ.Facebook.Views.Connected({
        model: this.user,
        events: this.ev
      });
      this.indexView.close();
      return this.connectedView.render();
    };
    FacebookConnectRouter.prototype.onDecline = function() {
      return this.indexView.close();
    };
    return FacebookConnectRouter;
  })();
  CentralAZ.Facebook.Models.FacebookUser = (function() {
    __extends(FacebookUser, Backbone.Model);
    function FacebookUser() {
      FacebookUser.__super__.constructor.apply(this, arguments);
    }
    return FacebookUser;
  })();
  CentralAZ.Facebook.Views.Connected = (function() {
    __extends(Connected, Backbone.View);
    function Connected() {
      this.className = 'connect-success';
      this.tagName = 'div';
      this.template = '<img src=\'https://graph.facebook.com/{{id}}/picture?type=square\' class="imageSmall" height="50" width="50" alt={{name}} />\n<p><strong>Sweet!</strong> {{name}}, you\'re all set!</p>';
      Connected.__super__.constructor.apply(this, arguments);
    }
    Connected.prototype.initialize = function(options) {
      this.ev = options.events;
      return this.model = options.model;
    };
    Connected.prototype.render = function() {
      var $el, jsonData, renderedHtml;
      $el = $(this.el);
      $el.prependTo('body');
      jsonData = this.model.toJSON();
      renderedHtml = Mustache.to_html(this.template, jsonData);
      $el.html(renderedHtml);
      return $el.fadeIn('fast', __bind(function() {
        return setTimeout(__bind(function() {
          return $el.fadeOut('fast', __bind(function() {
            return this.remove();
          }, this));
        }, this), 5000);
      }, this));
    };
    return Connected;
  })();
  CentralAZ.Facebook.Models.FacebookUsers = (function() {
    __extends(FacebookUsers, Backbone.Collection);
    function FacebookUsers() {
      this.model = CentralAZ.Facebook.Models.FacebookUser;
    }
    return FacebookUsers;
  })();
  CentralAZ.Facebook.Controllers.FacebookLoginRouter = (function() {
    __extends(FacebookLoginRouter, Backbone.Router);
    function FacebookLoginRouter() {
      FacebookLoginRouter.__super__.constructor.apply(this, arguments);
    }
    FacebookLoginRouter.prototype.routes = {
      "": "index",
      "!/log-in/:status": "index",
      "!/sign-up": "signUp",
      "*other": "index"
    };
    FacebookLoginRouter.prototype.initialize = function(options) {
      this.user = options.model;
      this.ev = options.events;
      this.indexView = new CentralAZ.Facebook.Views.FacebookLogin({
        events: this.ev
      });
      _.bindAll(this);
      this.ev.bind('facebookLoggedIn', this.onLogin);
      this.ev.bind('facebookLogOut', this.onLogOut);
      this.ev.bind('userDeclined', this.onDecline);
      return this.ev.bind('arenaLoginFailed', this.onArenaLoginFail);
    };
    FacebookLoginRouter.prototype.index = function(status) {
      if (status == null) {
        status = '';
      }
      if (status === 'declined') {
        alert('boo!');
      }
      return this.indexView.render();
    };
    FacebookLoginRouter.prototype.signUp = function() {
      if (this.user === null) {
        this.navigate('!/log-in', true);
        return;
      }
      this.signUpView = new CentralAZ.Facebook.Views.FacebookSignUp({
        model: this.user,
        events: this.ev
      });
      return this.signUpView.render();
    };
    FacebookLoginRouter.prototype.onLogin = function(response) {
      var query;
      query = CentralAZ.Web.Helpers.UrlHelper.parseQueryString();
      if (query.error_reason != null) {
        this.user = new CentralAZ.Facebook.Models.FacebookUser({
          name: 'anonymous person'
        });
        this.navigate('!/sign-up', true);
        return;
      }
      if (!response) {
        return this.navigate('!/log-in', true);
      }
      this.user = new CentralAZ.Facebook.Models.FacebookUser(response);
      this.userInfoView = new CentralAZ.Facebook.Views.FacebookWelcome({
        model: this.user,
        events: this.ev
      });
      this.userInfoView.render();
      return this.ev.trigger('arenaLogin');
    };
    FacebookLoginRouter.prototype.onLogOut = function() {
      return this.navigate('!/log-in', true);
    };
    FacebookLoginRouter.prototype.onDecline = function() {
      return this.navigate('!/log-in/declined', true);
    };
    FacebookLoginRouter.prototype.onArenaLoginFail = function() {
      return this.navigate('!/sign-up', true);
    };
    return FacebookLoginRouter;
  })();
  CentralAZ.Facebook.Views.FacebookConnectAccount = (function() {
    __extends(FacebookConnectAccount, Backbone.View);
    function FacebookConnectAccount() {
      this.className = 'connect-account';
      this.tagName = 'div';
      this.template = '<div class="close"><a href="#" class="fb-close">X</a></div>\n<p><strong>Hey {{name}}!</strong> We thought you might be interested in connecting your Central account to Facebook. \nThis will allow you to log into your Central account with a single click.</p>\n<p><a href="#" class="facebook-connect fbbutton" title="Connect to Facebook">Connect</a>\n<a href="#" class="fb-opt-out" title="Please don\'t ask me again">Please don\'t ask me again</a></p>';
      FacebookConnectAccount.__super__.constructor.apply(this, arguments);
    }
    FacebookConnectAccount.prototype.initialize = function(options) {
      this.ev = options.events;
      this.model = options.model;
      _.bindAll(this);
      return this.ev.bind('optOutSuccessful', this.onOptedOut);
    };
    FacebookConnectAccount.prototype.render = function() {
      var $el, renderedHtml;
      $el = $(this.el);
      $el.prependTo('body');
      renderedHtml = Mustache.to_html(this.template, this.model.toJSON());
      $el.html(renderedHtml);
      this.inTimer = setTimeout(__bind(function() {
        return $el.fadeIn('slow');
      }, this), 5000);
      return this.outTimer = setTimeout(__bind(function() {
        return this.close();
      }, this), 15000);
    };
    FacebookConnectAccount.prototype.events = {
      'click .facebook-connect': 'connectAccount',
      'click .fb-close': 'close',
      'click .fb-opt-out': 'optOut'
    };
    FacebookConnectAccount.prototype.connectAccount = function() {
      this.ev.trigger('facebookLogin');
      return false;
    };
    FacebookConnectAccount.prototype.close = function() {
      $(this.el).fadeOut('slow', __bind(function() {
        return $(this.el).remove();
      }, this));
      clearTimeout(this.outTimer);
      return false;
    };
    FacebookConnectAccount.prototype.optOut = function() {
      this.ev.trigger('optOut');
      return false;
    };
    FacebookConnectAccount.prototype.onOptedOut = function() {
      return this.close();
    };
    return FacebookConnectAccount;
  })();
  CentralAZ.Facebook.Views.FacebookSignUp = (function() {
    __extends(FacebookSignUp, Backbone.View);
    function FacebookSignUp() {
      this.el = '#signup-form';
      this.template = '<h3>Hi {{name}}</h3>\n<img src=\'https://graph.facebook.com/{{id}}/picture?type=square\' class="imageSmall" height="50" width="50" alt={{name}} />\n<p><strong>Almost done!</strong> Just need to link your Central account to Facebook. Which of these options sounds right?</p>';
      this.unbind();
      FacebookSignUp.__super__.constructor.apply(this, arguments);
    }
    FacebookSignUp.prototype.initialize = function(options) {
      this.ev = options.events;
      return _.bindAll(this, 'logOut');
    };
    FacebookSignUp.prototype.render = function() {
      var $that, renderedHtml;
      $('.greeting').empty();
      renderedHtml = Mustache.to_html(this.template, this.model.toJSON());
      $('.greeting').html(renderedHtml);
      $("input[id$='tbFirstName']").val(this.model.get('first_name'));
      $("input[id$='tbLastName']").val(this.model.get('last_name'));
      $("input[id$='tbEmail']").val(this.model.get('email'));
      $("input[id$='tbBirthdate']").val(this.model.get('birthday'));
      $("input[id$='ihAccessToken']").val(CentralAZ.Facebook.Helpers.Authentication.config.accessToken);
      $('.im-new, .have-account').next('div').hide();
      $that = this;
      return $(this.el).siblings().fadeOut('slow', function() {
        $($that.el).fadeIn('slow');
        return false;
      });
    };
    FacebookSignUp.prototype.events = {
      'click .fblogout': 'logOut',
      'click .im-new': 'register',
      'click .have-account': 'signIn'
    };
    FacebookSignUp.prototype.logOut = function() {
      this.ev.trigger('facebookLogOut');
      return false;
    };
    FacebookSignUp.prototype.register = function() {
      $('.have-account').next('div').slideUp();
      $('.im-new').next('div').slideToggle();
      return false;
    };
    FacebookSignUp.prototype.signIn = function() {
      $('.im-new').next('div').slideUp();
      $('.have-account').next('div').slideToggle();
      return false;
    };
    FacebookSignUp.prototype.unbind = function() {
      $(this.el).undelegate('.fb-logout', 'click');
      $(this.el).undelegate('.im-new', 'click');
      return $(this.el).undelegate('.have-account', 'click');
    };
    return FacebookSignUp;
  })();
  CentralAZ.Facebook.Views.FacebookWelcome = (function() {
    __extends(FacebookWelcome, Backbone.View);
    function FacebookWelcome() {
      this.el = '#user-info';
      this.template = '<img src=\'https://graph.facebook.com/{{id}}/picture?type=square\' class="imageSmall" height="50" width="50" alt={{name}} />\n<p>Welcome {{name}}! Hang tight, we\'re logging you in.</p>';
      FacebookWelcome.__super__.constructor.apply(this, arguments);
    }
    FacebookWelcome.prototype.initialize = function(options) {
      return this.ev = options.events;
    };
    FacebookWelcome.prototype.render = function() {
      var $container, renderedHtml;
      $container = $(this.el);
      renderedHtml = Mustache.to_html(this.template, this.model.toJSON());
      return $container.siblings().fadeOut('slow', function() {
        $container.html(renderedHtml);
        $container.fadeIn('slow');
        return false;
      });
    };
    return FacebookWelcome;
  })();
  CentralAZ.Facebook.Views.FacebookLogin = (function() {
    __extends(FacebookLogin, Backbone.View);
    function FacebookLogin() {
      this.el = '#log-in';
      this.unbind();
      FacebookLogin.__super__.constructor.apply(this, arguments);
    }
    FacebookLogin.prototype.initialize = function(options) {
      return this.ev = options.events;
    };
    FacebookLogin.prototype.render = function() {
      var $container;
      $container = $(this.el);
      return $('.fb-auth > div').fadeOut('slow', function() {
        $container.fadeIn('slow');
        return false;
      });
    };
    FacebookLogin.prototype.events = {
      'click .facebook-login': 'logIn'
    };
    FacebookLogin.prototype.logIn = function() {
      this.ev.trigger('facebookLogin');
      return false;
    };
    FacebookLogin.prototype.unbind = function() {
      return $(this.el).undelegate('.facebook-login', 'click');
    };
    return FacebookLogin;
  })();
  CentralAZ.Facebook.Helpers.Authentication = (function() {
    function Authentication() {}
    Authentication.initConnect = function(person) {
      var theModel;
      theModel = new CentralAZ.Facebook.Models.FacebookUser(person);
      window.facebookConnect = new CentralAZ.Facebook.Controllers.FacebookConnectRouter({
        model: theModel,
        events: facebookEvents
      });
      try {
        Backbone.history.start();
      } catch (error) {

      }
      return Authentication.registerFacebookApi();
    };
    Authentication.initLogin = function() {
      window.facebookAuth = new CentralAZ.Facebook.Controllers.FacebookLoginRouter({
        model: null,
        events: facebookEvents
      });
      try {
        Backbone.history.start();
      } catch (error) {

      }
      return Authentication.registerFacebookApi();
    };
    Authentication.registerFacebookApi = function() {
      var e;
      window.fbAsyncInit = function() {
        return FB.init({
          appId: Authentication.config.appID,
          status: true,
          cookie: true,
          xfbml: true,
          oauth: true
        });
      };
      e = document.createElement('script');
      e.src = "" + document.location.protocol + "//connect.facebook.net/en_US/all.js";
      e.async = true;
      return document.getElementById('fb-root').appendChild(e);
    };
    return Authentication;
  })();
  window.facebookEvents = _.extend(CentralAZ.Facebook.Events, Backbone.Events);
  facebookEvents.defaults = {
    onLoginSuccess: function(result) {
      if (result.d > -1) {
        return window.location = CentralAZ.Facebook.Helpers.Authentication.config.redirectPath;
      } else {
        return facebookEvents.trigger('arenaLoginFailed');
      }
    },
    onConnectSuccess: function(result) {
      return facebookEvents.trigger('connectAccountSuccessful');
    },
    onOptOutSuccess: function(result) {
      return facebookEvents.trigger('optOutSuccessful');
    },
    onError: function(result, status, error) {
      console.log(JSON.stringify(result));
      console.log(status);
      return console.log(error);
    }
  };
  facebookEvents.bind('arenaLogin', function(onSuccess, onError) {
    var accessToken, state;
    if (onSuccess == null) {
      onSuccess = facebookEvents.defaults.onLoginSuccess;
    }
    if (onError == null) {
      onError = facebookEvents.defaults.onError;
    }
    accessToken = CentralAZ.Facebook.Helpers.Authentication.config.accessToken;
    state = CentralAZ.Facebook.Helpers.Authentication.config.state;
    return $.ajax({
      url: "webservices/custom/cccev/core/AuthenticationService.asmx/AuthenticateFacebook",
      type: 'POST',
      data: "{'accessToken': '" + accessToken + "', 'state': '" + state + "'}",
      contentType: 'application/json',
      dataType: 'json',
      success: onSuccess,
      error: onError
    });
  });
  facebookEvents.bind('facebookLogin', function() {
    return FB.getLoginStatus(function(response) {
      if (response.status === 'connected') {
        return facebookEvents.trigger('facebookLoggingIn', response);
      } else {
        return FB.login(function(response) {
          if (response.authResponse) {
            return facebookEvents.trigger('facebookLoggingIn', response);
          } else {
            return facebookEvents.trigger('userDeclined');
          }
        }, {
          scope: 'email,user_birthday'
        });
      }
    });
  });
  facebookEvents.bind('facebookLoggingIn', function(response) {
    CentralAZ.Facebook.Helpers.Authentication.config.accessToken = response.authResponse.accessToken;
    return FB.api('/me', function(res) {
      return facebookEvents.trigger('facebookLoggedIn', res);
    });
  });
  facebookEvents.bind('facebookLogOut', function() {
    if (FB) {
      return FB.logout();
    }
  });
  facebookEvents.bind('connectAccounts', function(onSuccess, onError) {
    var accessToken, state;
    if (onSuccess == null) {
      onSuccess = facebookEvents.defaults.onConnectSuccess;
    }
    if (onError == null) {
      onError = facebookEvents.defaults.onError;
    }
    accessToken = CentralAZ.Facebook.Helpers.Authentication.config.accessToken;
    state = CentralAZ.Facebook.Helpers.Authentication.config.state;
    return $.ajax({
      url: 'webservices/custom/cccev/core/AuthenticationService.asmx/ConnectAccountToFacebook',
      type: 'POST',
      data: "{'accessToken': '" + accessToken + "', 'state': '" + state + "'}",
      contentType: 'application/json',
      dataType: 'json',
      success: onSuccess,
      error: onError
    });
  });
  facebookEvents.bind('optOut', function(onSuccess, onError) {
    var state;
    if (onSuccess == null) {
      onSuccess = facebookEvents.defaults.onOptOutSuccess;
    }
    if (onError == null) {
      onError = facebookEvents.defaults.onError;
    }
    state = CentralAZ.Facebook.Helpers.Authentication.config.state;
    return $.ajax({
      url: 'webservices/custom/cccev/core/AuthenticationService.asmx/OptOutOfFacebookConnection',
      type: 'POST',
      data: "{'state': '" + state + "'}",
      contentType: 'application/json',
      dataType: 'json',
      success: onSuccess,
      error: onError
    });
  });
}).call(this);
