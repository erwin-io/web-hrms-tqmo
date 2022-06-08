
//Events for DateTimepicker
$(document).on('mouseup touched', function(e){ 
	var container = $('.bootstrap-datetimepicker-widget');
	if(!container.is(e.target)&&container.has(e.target).length === 0){
		container.parent().datetimepicker('hide');
	}
});