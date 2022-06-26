 //Initialize js-views
 //Converters
 $.views.converters({
 	//set default date format
    formatDate: function(value){
        return moment(value).format("MM/DD/YYYY");
    },
    formatYear: function(value){
        return moment(value).format("YYYY");
     },
     text: function (value) {
         return value;
     }
 });


