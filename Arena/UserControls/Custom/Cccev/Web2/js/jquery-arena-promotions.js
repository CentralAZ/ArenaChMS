/**********************************************************************
* Description:  Arena Promotions via WebService jQuery Plugin.
*				Main client side script for the PromotionsViaWS module.
*				Promotions will reload on CAMPUS_CHANGED event.
*				Triggers CONTENT_RENDERED after loaded and rendered.
*
* Dependencies:
*				~/templates/Cccev/hasselhoff/js/campus-scripts.js
*				~/webservices/custom/cccev/web2/promotionservice.asmx/GetByAreaFilter
*
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created:	08/27/2010 10:42:02
*
* $Workfile: jquery-arena-promotions.js $
* $Revision: 2 $
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web2/js/jquery-arena-promotions.js   2   2010-11-08 17:11:44-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/js/jquery-arena-promotions.js $
*  
*  Revision: 2   Date: 2010-11-09 00:11:44Z   User: nicka 
*  Trigger CONTENT_RENDERED *after* any success callback. 
*  
*  Revision: 1   Date: 2010-08-30 23:37:01Z   User: nicka 
*  Total rewrite to work 100% via client ajax webservice calls. 
**********************************************************************/

(function ($)
{
	$.fn.arenaPromotions = function (options)
	{
		// default configuration properties
		var defaults = {
			topicList: '',
			areaFilter: '',
			campusID: '',
			documentTypeID: '',
			promotionDisplayPageID: -1,
			eventDetailPageID: -1,
			updateOnCampusChange: true,
			onSuccessCallback: '',
			placeholderID: '',
			containerID: '',
			maxItems: 16,
			priorityLevel: 99,
			promotionTemplateID: ''
		};

		var options = $.extend(defaults, options);
		var promotions = null;

		this.each(function ()
		{
			var obj = $(this);

			if (options.updateOnCampusChange)
			{
				// fade out the content area while the campus is updating.
				$(document).bind("CAMPUS_UPDATING", function ()
				{
					obj.fadeTo("fast", "0.01");
					return false;
				});

				$(document).bind("CAMPUS_UPDATED", function (e, data)
				{
					obj.fadeOut("fast");
					// TODO -- have CAMPUS_UPDATED event pass the new campusID
					//options.campusID = data.campusID;
					options.campusID = campus.campusID;
					promotions = null;
					showPromotions();
					return false;
				});
			}

			showPromotions();

			function showPromotions()
			{
				if (!promotions)
				{
					loadPromotions(options.topicList, options.areaFilter, options.campusID, options.maxItems, options.documentTypeID, options.promotionDisplayPageID, options.eventDetailPageID, onLoadPromotionSuccess, onLoadError);
				}
				else
				{
					renderPromotions();
				}
			}

			// NOTE: This is generic.  Can move to stand alone script.
			function loadPromotions(topicIDs, areaFilter, campusID, maxItems, documentTypeID, promotionDetailPageID, eventDetailPageID, success, error)
			{
				postAsyncJson("webservices/custom/cccev/web2/promotionservice.asmx/GetByAreaFilter",
					'{"topicIDs": "' + topicIDs + '", "areaFilter": "' + areaFilter + '", "campusID":' + campusID + ', "maxItems":' + maxItems + ', "documentTypeID":' + documentTypeID + ', "promotionDetailPageID": ' + promotionDetailPageID + ', "eventDetailPageID": ' + eventDetailPageID + ' }',
					success,
					error);
				return false;
			}

			function renderPromotions()
			{
				obj.empty();
				
				$("#" + options.promotionTemplateID).render(promotions).appendTo(obj);
				obj.fadeTo('fast', "1.0");
				obj.fadeIn('fast');

				return false;
			}

			/// <summary>
			///  Called after successfully loading promotions asynchronously from the webservice.
			/// </summary>
			function onLoadPromotionSuccess(result)
			{
				promotions = getPriorityFilteredPromotions(result.d);

				if (promotions.length > 0)
				{
					// show the container if the ID is defined and it's NOT visible
					if (options.containerID && !$("#" + options.containerID).is(":visible"))
					{
						$("#" + options.containerID).show();
					}
				}
				else
				{
					loadBlankPromotions("There are no features at this time.");
					// hide if a containerID was defined
					if (options.containerID && $("#" + options.containerID).is(":visible"))
					{
						$("#" + options.containerID).hide();
					}
				}

				// render the promotions.
				renderPromotions();

				// call the configured callback with the results
				if (options.onSuccessCallback)
				{
					options.onSuccessCallback(result);
				}

				$(document).trigger("CONTENT_RENDERED");

				return false;
			}

			/// <summary>
			/// Filters promotions by priority level.  Only those below the configured
			/// priority level will be included in the returned list.
			/// </summary>
			function getPriorityFilteredPromotions(promoList)
			{
				if (options.priorityLevel == 99)
				{
					return promoList;
				}

				var filteredList = [];

				for (var i = 0; i < promoList.length; i++)
				{
					var promotion = promoList[i];
					if (promotion.priority <= options.priorityLevel)
					{
						filteredList.push(promotion);
					}
				}

				return filteredList;
			}

			function onLoadError(result, errorText, thrownError)
			{
				loadBlankPromotions("Features not available at this time.")
				return false;
			}

			function loadBlankPromotions(message)
			{
				promotions = new Array();
				promotions[0] = {
					id: -1,
					topic: "Features",
					title: "Uh Oh!",
					summary: message,
					imageUrl: "#",
					detailsUrl: "#"
				};
			}

		});
	};
})(jQuery);

