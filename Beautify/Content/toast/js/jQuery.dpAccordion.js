/*
	DP Accordion jQuery Plugin, Version 1.0
	Copyright (C) Dustin Poissant 2014
	See http://htmlpreview.github.io/?https://github.com/dustinpoissant/jquery.dpAccordion/blob/master/License.html
	for more information reguarding usage.
*/
;(function($){
	$.fn.dpAccordion = function(){
		if(arguments.length > 0 && typeof(arguments[0]) == "string"){
			if(this.hasClass("dp-accordion")){
				if(arguments[0] == "close") this.each(function(){this.close()});
				if(arguments.length > 1 && arguments[0] == "open") {
					var open = arguments[1];
					this.each(function(){this.open(open);});
				}
			}
			return this;
		}
		var defaultOptions = {
			sectionSelector: '*[data-role="section"]',
			titleSelector: '*[data-role="title"]',
			contentSelector: '*[data-role="content"]',
			open: false,
			beforeOpen: function(){},
			afterOpen: function(){},
			beforeClose: function(){},
			afterClose: function(){}
		}
		var args = arguments;
		if(arguments.length > 0 && typeof(arguments[0]) == "object") this.each(function(){this.options = $.extend(defaultOptions, args[0]);});
		else this.each(function(){this.options = defaultOptions;});
		this.each(function(){
			var element = this;
			var a = $(this).addClass("dp-accordion");
			var sections = a.find(element.options.sectionSelector).addClass("dp-accordion-section");
			var titles = sections.find(element.options.titleSelector).addClass("dp-accordion-title");
			var contents = sections.find(element.options.contentSelector).addClass("dp-accordion-content").hide()
			
			element.close = function(){
				if(element.options.open !== false){
					element.options.beforeClose();
					sections.removeClass("open");
					contents.eq(element.options.open).slideUp({done: element.options.afterClose});
					element.options.open = false;
				}
				return this;
			};
			element.open = function(){
				if(element.options.open !== false){
					element.options.beforeClose();
					contents.eq(element.options.open).slideUp({done: element.options.afterClose});
					element.options.open = false;
				}
				var args = arguments;
				if(args.length > 0 && typeof(args[0]) == "number" && args[0] >= 0 && args[0] < contents.length) {
					element.options.beforeOpen();
					sections.removeClass("open").eq(args[0]).addClass("open");
					contents.eq(args[0]).slideDown({done: function(){
						element.options.open = args[0];
						element.options.afterOpen();
					}});
				} else if(args.length > 0 && typeof(args[0]) == "string"){
					for(var i=0; i<contents.length; i++) if( $(sections[i]).hasId(args[0]) ) element.open(i);
				}
				return this;
			};
			titles.on("click", function(){
				var index;
				for(var i=0; i<titles.length; i++) if(this == titles[i]) index = i;
				if(contents.eq(index).isDisplayed()) {
					element.close();
					return index;
				}
				element.open(index);
				return this;
			});
			if(element.options.open !== false) element.open(element.options.open);
		});
	};
	$.fn.isDisplayed = function(){
		for(var i=0; i<this.length; i++) if($(this).css("display") != "none") return true;
		return false;
	}
	$.fn.hasId = function(id){
		if(typeof(id) == "string")
			for(var i=0; i<this.length; i++) if(this[i].id == id) return true;
		return false;
	}
}(jQuery));