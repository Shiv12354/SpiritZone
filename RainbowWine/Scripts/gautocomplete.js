
class JSONResponse {
    constructor(response) {
        this.response = response;
    }

    json() {
        return new Promise((resolve, reject) => {
            resolve(JSON.parse(this.response));
        })
    }
}
window.esXHR = new XMLHttpRequest();
window.woosXHR = new XMLHttpRequest();

function buildQueryString(params) {
    let queryStringParts = [];

    for (let key in params) {
        if (params.hasOwnProperty(key)) {
            let value = params[key];

            queryStringParts.push(encodeURIComponent(key) + '=' + encodeURIComponent(value));
        }
    }

    return queryStringParts.join('&');
}

function nFetch(url, globalXHR) {
    return new Promise((resolve, reject) => {
        let xhr = globalXHR || new XMLHttpRequest();
        xhr.abort();
        xhr.open('GET', url);

        xhr.onload = () => {
            if (xhr.status >= 200 && xhr.status < 300) {
                resolve(new JSONResponse(xhr.response));
            } else {
                reject(xhr.statusText);
            }
        };
        xhr.onerror = () => reject(xhr.statusText);
        xhr.send();
    });
}

function debounce(func, wait, immediate) {
    let timeout;
    return function () {
        let context = this,
            args = arguments;
        let later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        let callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
}

function searchStores(input) {
    let args = {
        key: 'woos-81a699ca-5082-3ffd-9f54-a684a4b82853',
        query: 'name:"' + input + '" OR city:"' + input + '"',
        stores_by_page: 5
    };
    return nFetch('https://api.woosmap.com/stores/search/?' + buildQueryString(args), window.esXHR).then(function (response) {
        return response.json()
    });
}

function initAutocomplete() {
       // Create the search box and link it to the UI element.
    let inputContainer = document.querySelector('autocomplete-input-container');
    let autocomplete_results = document.querySelector('.autocomplete-results');
    let sessionToken = new google.maps.places.AutocompleteSessionToken();
    let service = new google.maps.places.AutocompleteService();
    let geocoder = new google.maps.Geocoder();
    

    let displayWoosmapStores = function (response_stores) {
        debugger;
        autocomplete_results.classList.remove("google");
        let results_html = [];
        for (let store_id in response_stores.features || []) {
            let store = response_stores.features[store_id].properties;
            results_html.push(`<li class="autocomplete-item" data-type="store" data-id=${store_id}>
      <span class="autocomplete-icon icon-store"></span>
      			    <span class="autocomplete-text">${store.name}, ${store.address.city}</span>
		    			</li>`);
        }
        autocomplete_results.innerHTML = results_html.join("");
        autocomplete_results.style.display = 'block';
        let autocomplete_items = autocomplete_results.querySelectorAll('.autocomplete-item');
        for (let autocomplete_item of autocomplete_items) {
            autocomplete_item.addEventListener('click', function () {
                let prediction = {};
                const selected_text = this.querySelector('.autocomplete-text').textContent;
                const data_id = parseInt(this.getAttribute('data-id'));
                prediction = response_stores.features[data_id].geometry;
                marker.setPosition(new google.maps.LatLng(prediction.coordinates[1], prediction.coordinates[0]));
                map.setCenter(new google.maps.LatLng(prediction.coordinates[1], prediction.coordinates[0]));
                map.setZoom(14);
                autocomplete_input.value = selected_text;
                autocomplete_results.style.display = 'none';
            })
        }
    }
    let getShop = function () {

        $('#shoperrormsg').html('');
        var place_id = txtPlaceId.value;
        var address = autocomplete_input.value;
        var latitude = txtLatitude.value;
        var longitude = txtLongitude.value;

        if (place_id != "") {

            $('#txtShopId').attr('value', '');
            $('#txtShopName').attr('value', '');
                        
            var obj = '{ "Dest_Place_Id": "' + place_id + '","CustomerName": "' + $('#txtCustName').val() + '","ContactNo": "' + $('#customerno').val() + '","Address": "' + address + '","Latitude": "' + latitude + '","Longitude": "' + longitude + '"}';

            $.ajax({
                //url: "https://recostack.qonverse.ai:5253/getshop",
                url: "https://pyapix.spiritzone.in:8886/getshop",
                data: obj,
                type: "POST",
                dataType: "json",
                contentType: "application/json",
                success: function (data) {
                    console.log(data);
                    $('#shoperrormsg').html('');
                    if (data.Status == "OK") {
                        $('#txtShopId').attr('value', data.ShopId);
                        $('#txtShopName').attr('value', data.ShopName);
                        $('#txtZoneId').attr('value', data.ZoneID);

                    }
                    $('#shoperrormsg').html(data.Message);
                },
                error: function (error) {
                    console.log(error);
                    console.log("error: " + error);
                    $('#txtShopId').attr('value', '');
                    $('#txtShopName').attr('value', '');
                    $('#txtZoneId').attr('value', '');
                    $('#shoperrormsg').html('Get Shop Api error.');
                }
            });
        }
    };

    let displayPlacesSuggestions = function (predictions, status) {
        //console.log(google.maps.places)
        if (status != google.maps.places.PlacesServiceStatus.OK) {
            alert(status);
            return;
        }
        //autocomplete_results.classList.add("google");
        let results_html = [];
        predictions.forEach(function (prediction) {
            results_html.push(`<li class="autocomplete-item" data-type="place" data-place-id=${prediction.place_id}><span class="autocomplete-text">${prediction.description}</span></li>`);
        });

        //debugger;
        autocomplete_results.innerHTML = results_html.join("");
        autocomplete_results.style.display = 'block';
        let autocomplete_items = autocomplete_results.querySelectorAll('.autocomplete-item');
        for (let autocomplete_item of autocomplete_items) {
            autocomplete_item.addEventListener('click', function () {
                let prediction = {};
                const selected_text = this.querySelector('.autocomplete-text').textContent;
                //geocoder.geocode({
                //    'address': sellected_text
                //}, function (results, status) {
                //        if (status == 'OK') {
                //            console.log(results);
                //        var bounds = new google.maps.LatLngBounds();
                //        marker.setPosition(results[0].geometry.location);
                //        if (results[0].geometry.viewport) {
                //            bounds.union(results[0].geometry.viewport);
                //        } else {
                //            bounds.extend(results[0].geometry.location);
                //        }
                //        map.fitBounds(bounds);
                //    } else {
                //        alert('Geocode was not successful for the following reason: ' + status);
                //    }
                //    autocomplete_input.value = selected_text;
                //    autocomplete_results.style.display = 'none';
                //});

                
                //it you prefer to use the getDetails of places service, use this code
                
                const place_id = this.getAttribute('data-place-id');
                let request = {
                  placeId: place_id,
                  fields: ['name', 'geometry']
                };
                let serviceDetails = new google.maps.places.PlacesService(this);
                serviceDetails.getDetails(request, function (place, status) {

                    if (status == google.maps.places.PlacesServiceStatus.OK) {
                        if (!place.geometry) {
                            console.log("Returned place contains no geometry");
                            return;
                        }
                        var latlng = place.geometry.location;
                        //console.log(latlng.lat());
                        //console.log(latlng.lng());
                        txtLongitude.value = latlng.lng()
                        txtLatitude.value = latlng.lat();
                    }
                    autocomplete_input.value = selected_text;
                    txtFullAddressOriginal.value = selected_text;
                    txtPlaceId.value = place_id;
                    autocomplete_results.style.display = 'none';
                    getShop();
                });
            })
        }
    };


    let autocomplete_input = document.getElementById('txtFullAddress');
    let txtFullAddressOriginal = document.getElementById('txtFullAddressOriginal');
    let txtPlaceId = document.getElementById('txtPlaceId');
    let txtLongitude = document.getElementById('txtLongitude');
    let txtLatitude = document.getElementById('txtLatitude');
    autocomplete_input.addEventListener('keyup', debounce(function () {
        let value = this.value;
        value.replace('"', '\\"').replace(/^\s+|\s+$/g, '');
        if (value !== "" && value.length > 2) {
            //searchStores(value).then(function (response_stores) {
            //    if (response_stores.features.length > 0) {
            //        displayWoosmapStores(response_stores);
            //    } else {
            //        service.getPlacePredictions({
            //            input: value,
            //            sessionToken: sessionToken
            //        }, displayPlacesSuggestions);
            //    }
            //});

            service.getPlacePredictions({
                input: value,
                sessionToken: sessionToken
            }, displayPlacesSuggestions);

        } else {
            autocomplete_results.innerHTML = '';
            autocomplete_results.style.display = 'none';
        }
    }, 150));
}

document.addEventListener("DOMContentLoaded", function (event) {
    initAutocomplete();
});

