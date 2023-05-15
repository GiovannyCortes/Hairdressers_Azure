function getGoogleMapsIframe(address) {
    $.ajax({
        url: '/Secret/GetSecret?secretName=googlemapsapikey',
        method: 'GET',
        success: function (data) {
            var mapUrl = `https://www.google.com/maps/embed/v1/place?q=${encodeURIComponent(address)}&key=` + data;
            // Creamos el objeto iframe que será devuelto por la función
            const iframe = $('<iframe></iframe>', {
                'src': mapUrl,
                'width': '600',
                'height': '450',
                'frameborder': '0',
                'style': 'border:0'
            });
            // Agregamos el iframe a nuestro enlace
            $('a.button_suggestions:last').append(iframe);
        },
        error: function (xhr, status, error) {
            console.log('Error retrieving secret: ' + error);
        }
    });
}

//function getGoogleMapsIframe(address) {
//    var mapUrl = "";
//    debugger;
//    $.ajax({
//        url: '/Secret/GetSecret?secretName=googlemapsapikey',
//        method: 'GET',
//        success: function (data) {
//            debugger;
//            mapUrl = `https://www.google.com/maps/embed/v1/place?q=${encodeURIComponent(address)}&key=` + data;
//        },
//        error: function (xhr, status, error) {
//            console.log('Error retrieving secret: ' + error);
//        }
//    });
//    // Creamos el objeto iframe que será devuelto por la función
//    const iframe = $('<iframe></iframe>', {
//        'src': mapUrl,
//        'width': '600',
//        'height': '450',
//        'frameborder': '0',
//        'style': 'border:0'
//    });
//    return iframe;
//}