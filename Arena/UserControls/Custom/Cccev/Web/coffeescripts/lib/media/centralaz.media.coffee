window.CentralAZ =  window.CentralAZ or {}
CentralAZ.Media = CentralAZ.Media or {}

class CentralAZ.Media.Video extends Backbone.Model
	initialize: ->
		@set htmlID: "video_#{@id}"

class CentralAZ.Media.VideoCollection extends Backbone.Collection
	initialize: ->
		@model = CentralAZ.Media.Video
	comparator: (item) ->
		-item.get('id')

class CentralAZ.Media.IndexView extends Backbone.View
	constructor: ->
		@setElement('#centralaz-vimeo-main')
		@indexTemplate = '''
						{{#videos}}
						<li id="{{htmlID}}">
						<a href="#!/message/{{id}}" title="{{title}}"><img src="{{thumbnail_medium}}" alt="{{title}}" /></a><br/>
						<strong>{{title}}</strong></br>
						<span>{{upload_date}}</span>
						</li>
						{{/videos}}
						'''
		super
	initialize: (options) ->
		# VideoCollection:
		@model = options.videos
	render: ->
		$main = @$el
		jsonData = videos: @model.toJSON()

		$main.fadeOut "fast", =>
			$main.empty()
			$ul = $("<ul />")
			renderedHtml = Mustache.to_html( @indexTemplate, jsonData )
			$ul.html( renderedHtml )
			$ul.appendTo $main
			$main.fadeIn "fast"

class CentralAZ.Media.MessageView extends Backbone.View
	constructor: ->
		@setElement('#centralaz-vimeo-main')
		@itemTemplate = '''
						<div class="centralaz-vimeo-player">
						<h1>{{title}}</h1>
						<iframe src="http://player.vimeo.com/video/{{id}}?title=0&amp;byline=0&amp;portrait=0" width="600" height="338" frameborder="0"></iframe>
						<p>{{description}}</p>
						<span><a href="http://vimeo.com/{{id}}">{{title}}</a> by {{user_name}}.</span>
						<a href="#!/home" title="Back">Back</a>
						</div>
						'''
		super
	initialize: (options) ->
		@videoEmbed = options.videoEmbed
		@model = options.model
	render: ->
		$main = @$el

		$main.fadeOut "fast", =>
			$main.empty()
			renderedHtml = Mustache.to_html( @itemTemplate, @model.toJSON() )
			$main.html( renderedHtml )
			$main.fadeIn "fast"

class CentralAZ.Media.MediaApp extends Backbone.Router
	routes: 
		"": "index" 
		"!/home": "index"
		"!/message/:id": "message"
		"!/latest": "latest"
	initialize: (options) ->
		@message = null
		@currentVideo = null
		# VideoCollection:
		@videos = options.collection
		@index = new CentralAZ.Media.IndexView( videos: @videos )
	index: ->
		@index.render()
	message: (id) ->
		@currentVideo = @videos.get id
		@message = new CentralAZ.Media.MessageView model: @currentVideo
		@message.render()
	latest: ->
		@currentVideo = @videos.at 0
		@message = new CentralAZ.Media.MessageView model: @currentVideo
		@message.render()
		
class CentralAZ.Media.Runner
	constructor: (@accountID) ->
	init: ->
		$.getJSON "http://vimeo.com/api/v2/"+@accountID+"/videos.json?callback=?", (data) ->
			window.mediaApp = new CentralAZ.Media.MediaApp (collection: new CentralAZ.Media.VideoCollection(data) )
			Backbone.history.start()