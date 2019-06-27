(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
  window.CentralAZ = window.CentralAZ || {};
  CentralAZ.Web = CentralAZ.Web || {};
  CentralAZ.Web.Views = CentralAZ.Web.Views || {};
  CentralAZ.Web.Helpers = CentralAZ.Web.Helpers || {};
  CentralAZ.Web.Helpers.UrlHelper = (function() {
    function UrlHelper() {}
    UrlHelper.parseQueryString = function() {
      var decode, paramExp, querystring, r, spaceExp, urlParams;
      urlParams = {};
      spaceExp = /\+/g;
      paramExp = /([^&=]+)=?([^&]*)/g;
      decode = function(str) {
        return decodeURIComponent(str.replace(spaceExp, ' '));
      };
      querystring = window.location.search.substring(1);
      while (r = paramExp.exec(querystring)) {
        urlParams[decode(r[1])] = decode(r[2]);
      }
      return urlParams;
    };
    return UrlHelper;
  })();
  CentralAZ.Web.Views.MainNavigation = (function() {
    function MainNavigation() {
      this.hovering = false;
    }
    MainNavigation.prototype.init = function() {
      var instance;
      instance = this;
      $(".main-nav > li > a").click(function() {
        var $ul;
        $ul = $(this).next("ul");
        if ($ul.length === 0) {
          return true;
        } else {
          return false;
        }
      }).hover(function() {
        var $ul;
        instance.hovering = true;
        $ul = $(this).next("ul");
        if ($ul.length > 0 && $ul.is(":hidden")) {
          CentralAZ.Web.Views.MainNavigation.hideNav();
          $ul.fadeIn("fast").addClass("selected");
        }
        return true;
      }, function() {
        instance.hovering = false;
        setTimeout(function() {
          if (instance.hovering === false) {
            return CentralAZ.Web.Views.MainNavigation.hideNav();
          }
        }, 1000);
        return true;
      });
      $(document).click(__bind(function() {
        if (instance.hovering === false) {
          CentralAZ.Web.Views.MainNavigation.hideNav();
        }
        return true;
      }, this));
      return $(".main-nav > li > ul").hover(function() {
        instance.hovering = true;
        return true;
      }, function() {
        instance.hovering = false;
        setTimeout(function() {
          if (instance.hovering === false) {
            return CentralAZ.Web.Views.MainNavigation.hideNav();
          }
        }, 3000);
        return true;
      });
    };
    MainNavigation.hideNav = function() {
      $(".main-nav > li > ul").fadeOut("fast").removeClass("selected");
      return false;
    };
    return MainNavigation;
  })();
  $(function() {
    window.nav = new CentralAZ.Web.Views.MainNavigation();
    return nav.init();
  });
}).call(this);
