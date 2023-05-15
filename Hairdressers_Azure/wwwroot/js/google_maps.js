async function getGoogleMapsIframe(address, link) { // Link es el lugar dónde agregaremos el iframe
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
            // Agregamos el iframe al enlace que lo llamó
            link.append(iframe);
        },
        error: function (xhr, status, error) {
            console.log('Error retrieving secret: ' + error);
        }
    });
}

//async function getGoogleMapsIframe(address) {
//    $.ajax({
//        url: '/Secret/GetSecret?secretName=googlemapsapikey',
//        method: 'GET',
//        success: function (data) {
//            var mapUrl = `https://www.google.com/maps/embed/v1/place?q=${encodeURIComponent(address)}&key=` + data;
//            // Creamos el objeto iframe que será devuelto por la función
//            const iframe = $('<iframe></iframe>', {
//                'src': mapUrl,
//                'width': '600',
//                'height': '450',
//                'frameborder': '0',
//                'style': 'border:0'
//            });
//            // Agregamos el iframe a nuestro enlace
//            //$('a.button_suggestions:last').append(iframe);
//            return iframe;
//        },
//        error: function (xhr, status, error) {
//            console.log('Error retrieving secret: ' + error);
//        }
//    });
//}