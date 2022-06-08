var homeLoader = $('body').loadingIndicator({
					showOnInit:false,
					useImage: false,
				}).data("loadingIndicator");;

var progressCircular = function(){
	var classOnBody = true;
	var show = ()=>{
		homeLoader.show();
	}
	var close = ()=>{
		homeLoader.hide();
	}
	return{
		show: show,
		close: close
	};
};

var circleProgress = new progressCircular;


