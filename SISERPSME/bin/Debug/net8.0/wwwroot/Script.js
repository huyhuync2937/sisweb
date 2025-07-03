window.blazorKeyListener = {
    init: function (dotNetHelper) {
        document.addEventListener("keydown", function (event) {            
            if (event.key === "F5" || event.key === "F3" || event.key === "F4" || event.key === "F7") {
                event.preventDefault();
            }
            dotNetHelper.invokeMethodAsync("OnKeyDown", event.keyCode);
        });
    },
    remove: function () {
        if (window.keyHandler) {
            document.removeEventListener("keydown", window.keyHandler);
            window.keyHandler = null;
        }
    }
};

window.changeTextGridPageSize = function (pText) {
    var myPager = document.getElementsByClassName("my-pager")[0];
    if (myPager) {
        var label = myPager.getElementsByClassName("dxbl-text")[0];
        label.innerText = pText.toString();
    }    
}
window.SetDXGridHeaderFooter = function (hColor, hbgColor, fColor, fbgColor) {
    //const dxheader = document.getElementsByClassName("dxbl-grid-header-row");
    const style = document.createElement('style');
    style.type = 'text/css';
    style.innerHTML = ` .dxbl-grid-header-row {color: ${hColor} ; background-color: ${hbgColor};} .dxbl-grid-footer-row {color: ${fColor} ; background-color: ${fbgColor};}`;
    document.getElementsByTagName('head')[0].appendChild(style); //  csp console error on appendChild (Refused to apply inline style because it violates the following Content Security Policy directive: "style-src 'self' 'nonce-187a7ff1b551c855d7b5a6fbb974664b' https://ims-dev01.portfoliobi.com". Either the 'unsafe-inline' keyword, a hash ('sha256-cIUXM9+5uoJVUxol2E4lbOMPhQi9KZEPqZtKPmHe/Io='), or a nonce ('nonce-...') is required to enable inline execution.)
}
window.ReSetDXGridHeaderFooter = function () {
    const style = document.createElement('style');
    style.type = 'text/css';
    style.innerHTML = ' .dxbl-grid-header-row {color: black ; background-color: transparent;} .dxbl-grid-footer-row {color: black ; background-color: transparent;} ';
    document.getElementsByTagName('head')[0].appendChild(style);
}

window.Movetoend = function (divName) {
    const element = document.getElementById(divName);
    if (element != null) {
        element.scrollTop = element.scrollHeight;
    }
};

window.GetDeviceInfo = function () {
    getBDCClientInfo(
        /* provide a callback function for client info result */
        function (result) {
            console.log(result);
        });

    navigator.userAgentData
        .getHighEntropyValues([
            "architecture",
            "model",
            "platform",
            "platformVersion",
            "fullVersionList"
        ])
        .then((ua) => {
            const model = ua["model"];
            if (model) console.log("Phone: " + model);
        });

};

document.body.addEventListener('keydown', (e) => {
    //if (e.key == 'Enter') {
    //    console.log('..................Enter Down');
    //}
    //if (e.key == 'Tab') {
    //    console.log('..................Tab Down');
    //}

    if (e.key == 'F2' || e.key == 'F4' || e.key == 'F6' || e.key == 'F8' || e.key == 'Escape') {
        if (PageObj != null)
            PageObj.invokeMethodAsync('GridFMode', e.key);
        return;
    }
});

window.SetDocumentTitle = function (title) {
    document.title = title;
};

function ModifyEnterKeyPressAsTab(event) {
    var caller;
    var key;
    if (window.event) {
        caller = window.event.srcElement; //Get the event caller in IE.
        key = window.event.keyCode; //Get the keycode in IE.
    }
    else {
        caller = event.target; //Get the event caller in Firefox.
        key = event.which; //Get the keycode in Firefox.
    }
    if (key == 13) //Enter key was pressed.
    {
        cTab = caller.tabIndex; //caller tabIndex.
        maxTab = 0; //highest tabIndex (start at 0 to change)
        minTab = cTab; //lowest tabIndex (this may change, but start at caller)
        allById = document.getElementsByTagName("input"); //Get input elements.
        allByIndex = []; //Storage for elements by index.
        c = 0; //index of the caller in allByIndex (start at 0 to change)
        i = 0; //generic indexer for allByIndex;
        for (id in allById) //Loop through all the input elements by id.
        {
            allByIndex[i] = allById[id]; //Set allByIndex.
            tab = allByIndex[i].tabIndex;
            if (caller == allByIndex[i])
                c = i; //Get the index of the caller.
            if (tab > maxTab)
                maxTab = tab; //Get the highest tabIndex on the page.
            if (tab < minTab && tab >= 0)
                minTab = tab; //Get the lowest positive tabIndex on the page.
            i++;
        }
        //Loop through tab indexes from caller to highest.
        for (tab = cTab; tab <= maxTab; tab++) {
            //Look for this tabIndex from the caller to the end of page.
            for (i = c + 1; i < allByIndex.length; i++) {
                PageObj.invokeMethodAsync('GridCTEditMode', allByIndex[c].name);
                if (allByIndex[c].name.startsWith('end_input_Grid')) {
                    if (PageObj != null)
                        PageObj.invokeMethodAsync('CallGridEditMode', allByIndex[c].name);
                    return;
                }
                if (allByIndex[i].tabIndex == tab) {
                    allByIndex[i].focus(); //Move to that element and stop.  
                    return;
                }
            }          
        }    
    }
}

var PageObj;
window.SetPageObj = function (obj) {
    PageObj = obj;
}

window.SendKeyTab = function () {
    let press = new KeyboardEvent('keydown', { key: 'Tab', keyCode: 9, which: 9, ctrlKey: false });
    document.body.dispatchEvent(press);
}

/////////////////////////
ReportDesignerExit = function (s, e) {
    history.back();
}
window.GetQrcodeValue = function (resultdiv) {
    var result = document.getElementById(resultdiv).innerText;
    document.getElementById(resultdiv).innerText = '';
    return result;
}

window.ScanQrcode = function (parentdiv, resultdiv, objRef) {
    const node = document.createElement("div");
    node.setAttribute("id", "qrcodereader");
    document.getElementById(parentdiv).appendChild(node);

    const scanner = new Html5QrcodeScanner('qrcodereader', {
        // Scanner will be initialized in DOM inside element with id of 'reader'
        qrbox: {
            width: 250,
            height: 250,
        },  // Sets dimensions of scanning box (set relative to reader element width)
        fps: 20, // Frames per second to attempt a scan
    });

    scanner.render(success, error);

    // Starts scanner

    function success(result) {

        document.getElementById(resultdiv).innerText = result;
        // document.getElementById(resultdiv).value = result;
        // Prints result as a link inside result element

        scanner.clear();
        // Clears scanning instance

        document.getElementById('qrcodereader').remove();
        // Removes reader element from DOM since no longer needed

        objRef.invokeMethodAsync('JStoCSCall', result);

    }

    function error(err) {
        console.error(err);
        // Prints any errors to the console
    }

}

window.ShowToast = (divName) => {
    var toastElList = [].slice.call(document.querySelectorAll('#' + divName))
    var toastList = toastElList.map(function (toastEl) {
        return new bootstrap.Toast(toastEl, { delay: 10000})
    })
    toastList.forEach(toast => toast.show())
}

window.changeNewBG = function (tr) {
    if (tr.value != '1') {
        tr.style.backgroundColor = "#FBAA32";     //#1072B9  #FBAA32 
    }
}
window.ChangeOldBG = function (tr) {

    if (tr.value != '1') {
        tr.style.backgroundColor = "transparent";
    }
}
window.SetActiverow = function (tr) {
    let length = document.getElementById("masterTable").rows.length;
    let crows = document.getElementById("masterTable").rows;
    for (let x = 0; x < length; x++) {
        if (crows[x].value < 2) {
            crows[x].value = 0;
            crows[x].style.backgroundColor = "transparent";
        }
    }
    tr.value = "1";
    tr.style.backgroundColor = "#1072B9";   //#FBAA32
}
window.SetActiverowcustom = function (tr, divname) {
    let length = document.getElementById(divname).rows.length;
    let crows = document.getElementById(divname).rows;
    for (let x = 0; x < length; x++) {
        if (crows[x].value < 2) {
            crows[x].value = 0;
            crows[x].style.backgroundColor = "transparent";
        }
    }
    tr.value = "1";
    tr.style.backgroundColor = "#1072B9";   //#FBAA32  
}

//window.Getthispos = function (li) { 
//    let toppos = li.offsetTop;   
//    if (toppos < 110)
//        toppos = 110;
//    document.getElementById("box").style.top = toppos + 'px';
//    document.getElementById("box1").style.top = toppos + 'px';
//    document.getElementById("box2").style.top = (toppos - 44) + 'px';  
//}

window.setbackground = () => {
    document.body.style.background = "radial-gradient(circle, rgba(240,250,255,1) 0%, rgba(140,230,255,1) 100%)";
}
window.resetbackground = () => {
    document.body.style.background = "#EAF3F9";
}

window.Focusinput = function (curcontrol) {
    document.getElementById(curcontrol).focus();
}

function invokeTabKey() {
    var currInput = document.activeElement;
    if (currInput.tagName.toLowerCase() == "input") {
        var inputs = document.getElementsByTagName("input");
        var currInput = document.activeElement;
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i] == currInput) {
                var next = inputs[i + 1];
                if (next && next.focus) {
                    next.focus();
                }
                break;
            }
        }
    }
}

var map;
var searchBox;
var markers;

function initialize(latp, lngp) {
    var latlng = new google.maps.LatLng(latp, lngp);
    const myLatlng = { lat: latp, lng: lngp };
    var options = {
        zoom: 14, center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("map"), options);
    // Create the initial InfoWindow.
    let infoWindow = new google.maps.InfoWindow({
        content: "Click the map to get Lat/Lng!",
        position: myLatlng,
    });

    infoWindow.open(map);
    // Configure the click listener.
    map.addListener("click", (mapsMouseEvent) => {
        // Close the current InfoWindow.
        infoWindow.close();
        // Create a new InfoWindow.
        infoWindow = new google.maps.InfoWindow({
            position: mapsMouseEvent.latLng,
        });
        infoWindow.setContent(
            JSON.stringify(mapsMouseEvent.latLng.toJSON(), null, 2)
        );
        infoWindow.open(map);
        var txtlat = document.getElementById("txtlat");
        txtlat.value = mapsMouseEvent.latLng.lat().toString();
        var txtlng = document.getElementById("txtlng");
        txtlng.value = mapsMouseEvent.latLng.lng().toString();
        PageObj.invokeMethodAsync('updateval', mapsMouseEvent.latLng.lat(), mapsMouseEvent.latLng.lng());
    });

    var input = document.getElementById("pac-input");
    searchBox = new google.maps.places.SearchBox(input);
    searchBox.addListener("places_changed", () => {
        const places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        markers = [];

        // For each place, get the icon, name and location.
        const bounds = new google.maps.LatLngBounds();

        places.forEach((place) => {
            if (!place.geometry || !place.geometry.location) {
                console.log("Returned place contains no geometry");
                return;
            }

            const icon = {
                url: place.icon,
                size: new google.maps.Size(71, 71),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(17, 34),
                scaledSize: new google.maps.Size(25, 25),
            };

            // Create a marker for each place.
            markers.push(
                new google.maps.Marker({
                    map,
                    icon,
                    title: place.name,
                    position: place.geometry.location,
                })
            );
            if (place.geometry.viewport) {
                // Only geocodes have viewport.
                bounds.union(place.geometry.viewport);
            } else {
                bounds.extend(place.geometry.location);
            }
        });
        map.fitBounds(bounds);
    });
}

function GetAddress() {
    var script = document.createElement("script");
    script.type = "text/javascript";
    script.src = "https://api.ipify.org?format=jsonp&callback=DisplayIP";
    document.getElementsByTagName("head")[0].appendChild(script);
};

function DisplayIP(response) {
    PageObj.invokeMethodAsync('updateip', response.ip);
}

window.getlatitude = () => {
    return new Promise((resolve) => {
        navigator.geolocation.getCurrentPosition((position) => {
            resolve(position.coords.latitude);
        });
    });
}
window.getlongitude = () => {
    return new Promise((resolve) => {
        navigator.geolocation.getCurrentPosition((position) => {
            resolve(position.coords.longitude);
        });
    });
}

function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function checkCookie() {
    let user = getCookie("username");
    if (user != "") {
        alert("Welcome again " + user);
    } else {
        user = "SIS";
        if (user != "" && user != null) {
            setCookie("username", user, 30);
        }
    }
}
function deleteCookie() {
    document.cookie = "username=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";

}
window.MoveIntoView = function (divName) {
    const element = document.getElementById(divName);
    if (element != null) {
        element.scrollIntoView();
    }
};

window.RegisterNotification = function () {
    if (!Notification) {
        return;
    }

    if (Notification.permission !== 'granted') {
        Notification.requestPermission();
    }
}

document.addEventListener("DOMContentLoaded", (event) => {
    if (!Notification) {
        return;
    }

    if (Notification.permission !== 'granted') {
        Notification.requestPermission();
    }
});

window.BroweserPushNotification = function (title, content, url) {
    if (Notification.permission !== 'granted') {
        Notification.requestPermission();
    } else {
        const options = {
            body: content,
            dir: 'ltr',
            image: '../images/logo.svg' //must be 728x360px
        };
        let _title = 'S.I.S VN Thông báo';
        if (title != null)
            _title = title;
        const notification = new Notification(_title, options);

        if (url != null && url !== '') {
            notification.onclick = function () {
                window.open(url);
            };
        }
    }
}