/*
	DP Toggle jQuery Plugin, Version 1.1
	Copyright (C) Dustin Poissant 2014
	See http://htmlpreview.github.io/?https://github.com/dustinpoissant/jquery.dpToggle/blob/master/License.html
	for more information reguarding usage.
*/
var Basics = {
	options: {
		mobileChangePoint: 600,
		tabletChangePoint: 900
	},
	jump: function(id){
		var dest=0;
		if($("#"+id).offset().top > $(document).height()-$(window).height()){
			dest=$(document).height()-$(window).height();
		}else{
			console.log($("#menuBtn").height());
			dest=$("#"+id).offset().top-$("nav").height()-15;
		}
		$('html,body').animate({scrollTop:dest}, 750,'swing');
	},
	preloadImg: function(url){
		return new Image().src = url;
	},
	onMobile: function(func){
		var oldWidth = $(window).width();
		$(window).resize(function(){
			var width = $(window).width();
			
			if(width <= Basics.options.mobileChangePoint && oldWidth > Basics.options.mobileChangePoint) func();
			oldWidth = width;
		});
	},
	offMobile: function(func){
		var oldWidth = $(window).width();
		$(window).resize(function(){
			var width = $(window).width();
			if(width > Basics.options.mobileChangePoint && oldWidth <= Basics.options.mobileChangePoint) func();
			oldWidth = width;
		});
	},
	onTablet: function(func){
		var oldWidth = $(window).width();
		$(window).resize(function(){
			var width = $(window).width();
			if(width > Basics.options.mobileChangePoint && oldWidth <= Basics.options.mobileChangePoint) func();
			else if (width <= Basics.options.tabletChangePoint && oldWidth > Basics.options.tabletChangePoint) func();
			oldWidth = width;
		});
	},
	offTablet: function(func){
		var oldWidth =  $(window).width();
		$(window).resize(function(){
			var width = $(window).width();
			if(width <= Basics.options.mobileChangePoint && oldWidth > Basics.options.mobileChangePoint) func();
			else if(width > Basics.options.tabletChangePoint && oldWidth <= Basics.options.tabletChangePoint) func();
			oldWidth = width;
		});
	},
	onDesktop: function(func){
		var oldWidth = $(window).width();
		$(window).resize(function(){
			var width = $(window).width();
			if(width > Basics.options.tabletChangePoint && oldWidth <= Basics.options.tabletChangePoint) func();
			oldWidth = width;
		});
	},
	offDesktop: function(func){
		var oldWidth = $(window).width();
		$(window).resize(function(){
			var width = $(window).width();
			if( width <= Basics.options.tabletChangePoint && oldWidth > Basics.options.tabletChangePoint) func();
			oldWidth = width;
		});
	},
	onOrientationChange: function(){},
	browser: function(){
		var b = navigator.appVersion;
		if(b.indexOf("OPR") > -1) return "opera";
		if(b.indexOf("Chrome") > -1) return "chrome";
		if(b.indexOf("Safari") > -1) return "safari";
		else return "mozilla";
	}
}
jQuery.fn.htmlClean = function() {
	this.contents().filter(function() {
	if (this.nodeType != 3 && this.tagName != "PRE") {
		$(this).htmlClean();
		return false;
	}
	else {
		this.innerHTML = $.trim(this.innerHTML);
		return !/\S/.test(this.nodeValue);
		}
	}).remove();
	return this;
}
$(document).ready(function(){
	$("html").htmlClean();
	if(window.DeviceOrientationEvent){
		window.addEventListener('orientationchange', function(){
			$(window).resize();
			Basics.onOrientationChange();
		}, false);
	}
});