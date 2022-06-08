$(document).on("show.bs.modal", ".modal", function(event){
    setTimeout(function(){$("body").css({padding:"0"});},2000);

    var zIndex = 1040 + (10 * $('.modal:visible').length);
    $(this).css('z-index', zIndex);
    setTimeout(function(){
    	$('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
    },0);

});
$(document).on("hidden.bs.modal", ".modal", function(event){
    setTimeout(function(){$("body").css({padding:"0"});},2000);
    $('.modal:visible').length && $(document.body).addClass('modal-open');
});