/**********************************************************************
* Description:  Feature slideshow with caption.
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	04/23/2009 10:42:02
*
* $Workfile: featureShow1.0.js $
* $Revision: 7 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web2/js/featureShow1.0.js   7   2010-09-02 11:29:38-07:00   JasonO $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/js/featureShow1.0.js $
*  
*  Revision: 7   Date: 2010-09-02 18:29:38Z   User: JasonO 
*  Fixing fringe case caused with fast clicking and opacity of list item 
*  getting out of sync (caused an image to float above active feature items 
*  being animated underneath). 
*  
*  Revision: 6   Date: 2010-09-01 00:35:17Z   User: JasonO 
*  Fixing animation bug. w00t! 
*  
*  Revision: 5   Date: 2010-08-26 21:31:57Z   User: nicka 
*  Change quickly if user clicks; prevent multiple timers. 
*  
*  Revision: 4   Date: 2010-07-12 17:36:40Z   User: nicka 
*  Unhide and rehide before off-screen width calculation occurs. 
*  
*  Revision: 3   Date: 2010-07-09 19:36:36Z   User: nicka 
*  Updated to handle off screen text position better. 
*  
*  Revision: 2   Date: 2010-05-04 20:16:36Z   User: nicka 
*  Improved the off-screen text handling 
*  
*  Revision: 1   Date: 2010-04-24 16:14:09Z   User: nicka 
*  Features slider 
**********************************************************************/
/*
* OPTIONS:
*   prevId:       ID of your previous button (if any).
*   nextId:       ID of your next button (if any).
*   speed:        time (in ms) of transitions.
*   autoStart:    Set to false to prevent auto start.
*   pause:        Time (in ms) to wait between images.
*   continuous:   Set to false to prevent automatic restart from beginning.
*   clickDelay:   Time (in ms) to wait before continuing auto play, if
*                 user manually clicks the navigation.
*
* Example markup for:
*
*		$("#feature").featureShow({
*			prevId: 'feature-nav-prev',
*			nextId: 'feature-nav-next'
*		});
*
*    <div id="feature">
*        <ul>
*            <li>
*                <img src="images/01.jpg" alt="" />
*                <div>
*                    <h1>FEATURE 1</h1>
*                    <p>
*                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque rutrum lorem eget magna vehicula eu dictum ipsum auctor.
*                    </p>
*                </div>
*            </li>
*            <li>
*                <img src="images/02.jpg" alt="" />
*                <div>
*                    <p>
*                        You can put anything you want in the DIV.
*                    </p>
*                </div>
*            </li>
*        </ul>
*        <div id="feature-nav">
*           <a href="#" id="feature-nav-prev" class="left">&lt;</a>
*           <a href="#" id="feature-nav-next" class="right">&gt;</a>
*        </div>
*    </div>
*/

(function ($)
{
    $.fn.featureShow = function (options)
    {
        // default configuration properties
        var defaults = {
            prevId: '',
            nextId: '',
            speed: 800,
            autoStart: true,
            pause: 2000,
            continuous: false,
            clickDelay: 10000
        };

        var options = $.extend(defaults, options);

        this.each(function ()
        {
            var obj = $(this);
            var s = $("li", obj).length;

            var lastIndex = s - 1;
            var index = 0;

            //slide all but the first LI's DIVs off screen
            $('li:not(:first-child)', obj).css('display', 'list-item'); // unhide them momentarily
            $('li:not(:first-child) > div', obj).each(function ()
            {
                var d = $(this);
                d.css('left', d.find('h1').outerWidth());
            });

            // hide all but the first LI
            $('li:not(:first-child)', obj).css('display', 'none'); ;

            $("#" + options.nextId).click(function ()
            {
                animateFeature("next", true);
                return false;
            });
            $("#" + options.prevId).click(function ()
            {
                animateFeature("prev", true);
                return false;
            });

            function animateFeature(dir, clicked)
            {
                var clickDelay = 0;
                var old = index;

                switch (dir)
                {
                    case "next":
                        index = (old >= lastIndex) ? (options.continuous ? 0 : lastIndex) : index + 1;
                        break;
                    case "prev":
                        index = (index <= 0) ? (options.continuous ? lastIndex : 0) : index - 1;
                        break;
                    default:
                        break;
                };

                var $oldItem = $('li:nth-child(' + (old + 1) + ')', obj);
                var $active = $('li:nth-child(' + (index + 1) + ')', obj);
                var $activeItemText = $('div', $active);
                var $oldItemText = $('div', $oldItem);
                var h1Width = $oldItemText.find('h1').outerWidth();
                var pWidth = $oldItemText.find('p').outerWidth();

                // use the largest width when hiding the text
                var hideWidth = (h1Width > pWidth) ? h1Width : pWidth;

                var speed = options.speed;

                // move everything quickly if the user clicked
                if (clicked)
                {
                    speed = 'fast';
                }

                $oldItem.fadeOut(speed, function ()
                {
                    $oldItemText.stop().css('left', hideWidth + 200);

                    // Fringe case w/ fast clicking:
                    // Ensure that opacity doesn't get stuck at a fraction of a percent
                    $(this).hide();
                });

                $active.fadeIn(speed, function ()
                {
                    // slide the text from the right
                    $activeItemText.animate(
                        { left: 0 },
                        speed
                    );

                    // Fringe case w/ fast clicking:
                    // Ensure that opacity doesn't get stuck at 99+ percent
                    $(this).show();
                });

                var timer = obj.data('timer');
                if (clicked)
                {
                    clearTimeout(timer);
                    // when clicked, add some time to the click delay...
                    clickDelay = options.clickDelay;
                }

                if (options.autoStart && dir == "next" && (options.continuous || index != lastIndex))
                {
                    if (timer != null || timer != 'undefined')
                    {
                        clearTimeout(timer);
                    }

                    timer = setTimeout(function ()
                    {
                        animateFeature("next", false);
                    }, options.speed + options.pause + clickDelay);
                    obj.data('timer', timer);
                };

                // Dimm the prev/next nav arrows if not continuous.
                if (!options.continuous && index == lastIndex)
                {
                    $("#" + options.nextId).fadeTo('slow', 0.3);
                }
                else if (!options.continuous && index == 0)
                {
                    $("#" + options.prevId).fadeTo('slow', 0.3);
                }
                else
                {
                    $("#" + options.nextId).fadeTo('fast', 1);
                    $("#" + options.prevId).fadeTo('fast', 1);
                }
            };

            // init
            var timer = obj.data('timer');
            if (options.autoStart)
            {
                if (!options.continuous)
                {
                    $("#" + options.prevId).fadeTo('slow', 0.3);
                }

                if (timer != null || timer != 'undefined')
                {
                    clearTimeout(timer);
                }

                timer = setTimeout(function ()
                {
                    animateFeature("next", false);
                }, options.pause);

                obj.data('timer', timer);
            };
        });
    };
})(jQuery);