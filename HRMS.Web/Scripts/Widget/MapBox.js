    
//var legalEntityController = function() {
//    var appSettings = {
//        accessToken:null,
//    };
//    var init = function (obj) {
//        appSettings = $.extend(appSettings, obj);
//        mapboxgl.accessToken = appSettings.accessToken;
//    };

//    var CreateMap_AddressFinder = function(mapProp){
//        var map = new mapboxgl.Map({
//            container: mapProp.container,
//            style: 'mapbox://styles/mapbox/streets-v11',
//            center: mapProp.center,
//            zoom: mapProp.zoom
//        });

//        var geocoder = new MapboxGeocoder({
//            accessToken: mapboxgl.accessToken,
//            marker: {
//            color: 'orange'
//            },
//            mapboxgl: mapboxgl
//        });
         
//        map.addControl(geocoder);

//        var marker = new mapboxgl.Marker();
//        var center = map.getCenter();
//        marker.setLngLat(center);
//        marker.addTo(map);
//        map.on('move', function (e) {
//            center = map.getCenter();
//            marker.addTo(map);
//         });
//    }

//    //Function for clearing the textboxes
//    return  {
//        CreateMap_AddressFinder: CreateMap_AddressFinder,
//        init: init,
//    };
//}
//var legalEntity = new legalEntityController;
