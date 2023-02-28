export function GetBattery(wrapper, addListener = true) {
    navigator.getBattery().then(function (battery) {

        console.log("Battery charging? " + (battery.charging ? "Yes" : "No"));
        console.log("Battery level: " + battery.level * 100 + "%");
        console.log("Battery charging time: " + battery.chargingTime + " seconds");
        console.log("Battery discharging time: " + battery.dischargingTime + " seconds");

        if (addListener) {
            battery.addEventListener('chargingchange', function () {
                console.log("Battery charging? " + (battery.charging ? "Yes" : "No"));
                logbatteryitem();
            });

            battery.addEventListener('levelchange', function () {
                console.log("Battery level: " + battery.level * 100 + "%");
                logbatteryitem();
            });

            battery.addEventListener('chargingtimechange', function () {
                console.log("Battery charging time: " + battery.chargingTime + " seconds");
                logbatteryitem();
            });

            battery.addEventListener('dischargingtimechange', function () {
                console.log("Battery discharging time: " + battery.dischargingTime + " seconds");
                logbatteryitem();
            });
        }

        function logbatteryitem() {

            var batteryitem = {
                "charging": battery.charging,
                "level": battery.level * 100,
                "chargingTime": battery.chargingTime == 'Infinity' ? null : battery.chargingTime,
                "dischargingTime": battery.dischargingTime == 'Infinity' ? null : battery.dischargingTime
            };
            wrapper.invokeMethodAsync('GetBatteryResult', batteryitem);
        }

        logbatteryitem();
    });
}

export function GetNetworkInfo(wrapper) {
    navigator.connection.addEventListener('change', logNetworkInfo);
    function logNetworkInfo() {
        // Network type that browser uses
        console.log('         type: ' + navigator.connection.type);

        // Effective bandwidth estimate
        console.log('     downlink: ' + navigator.connection.downlink + ' Mb/s');

        // Effective round-trip time estimate
        console.log('          rtt: ' + navigator.connection.rtt + ' ms');

        // Upper bound on the downlink speed of the first network hop
        console.log('  downlinkMax: ' + navigator.connection.downlinkMax + ' Mb/s');

        // Effective connection type determined using a combination of recently
        // observed rtt and downlink values: ' +
        console.log('effectiveType: ' + navigator.connection.effectiveType);

        // True if the user has requested a reduced data usage mode from the user
        // agent.
        console.log('     saveData: ' + navigator.connection.saveData);

        // Add whitespace for readability
        console.log('');

        var networkInfo = {
            "type": navigator.connection.type,
            "downlink": navigator.connection.downlink == undefined ? null : navigator.connection.downlink,
            "rtt": navigator.connection.rtt,
            "downlinkMax": navigator.connection.downlinkMax == undefined ? null : navigator.connection.downlinkMax,
            "effectiveType": navigator.connection.effectiveType,
            "saveData": navigator.connection.saveData,
        };
        wrapper.invokeMethodAsync('GetNetworkInfoResult', networkInfo);

    }

    logNetworkInfo();
}

export async function Capture(instance, element, options, command) {

    const width = 500;  
    let height = 0; 
    let streaming = false;

    let log = null;
    let video = null;
    let canvas = null;
    let photo = null;
    let startbutton = null;
    let sendTimer = null;
    let sourceSelect = null;
    let sourceSelectPanel = null;
    let selectedDeviceId = null;

    if (command == 'Start') {
        startup();
    }

    function showViewLiveResultButton() {
        if (window.self !== window.top) {
            element.querySelector(".contentarea").remove();
            const button = document.createElement("button");
            button.textContent = "View live result of the example code above";
            document.body.append(button);
            button.addEventListener("click", () => window.open(location.href));
            return true;
        }
        return false;
    }

    function startup() {
        if (showViewLiveResultButton()) {
            return;
        }
        log = element.querySelector("[data-action=log]");
        video = element.querySelector("[data-action=video]");
        canvas = element.querySelector("[data-action=canvas]");
        photo = element.querySelector("[data-action=photo]");
        startbutton = element.querySelector("[data-action=startbutton]");
        sourceSelect = element.querySelector("[data-action=sourceSelect]");
        sourceSelectPanel = element.querySelector("[data-action=sourceSelectPanel]");

        if (!options.camera && navigator.mediaDevices && navigator.mediaDevices.getDisplayMedia) {
            navigator.mediaDevices
                .getDisplayMedia({ video: true, audio: false })
                .then((stream) => {
                    video.srcObject = stream;
                    video.play();
                })
                .catch((err) => {
                    console.error(`An error occurred: ${err}`);
                    instance.invokeMethodAsync('GetError', `An error occurred: ${err}`);
                });
        }else if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {

            if (!navigator.mediaDevices?.enumerateDevices) {
                console.log("enumerateDevices() not supported.");
            } else if (selectedDeviceId==null) {
                navigator.mediaDevices.enumerateDevices()
                    .then((devices) => {
                        devices.forEach((device) => {
                            if (device.kind === 'videoinput') {
                                if (options.debug) console.log(`${device.label} id = ${device.deviceId}`);
                                const sourceOption = document.createElement('option');
                                sourceOption.text = device.label
                                sourceOption.value = device.deviceId
                                sourceSelect.appendChild(sourceOption)
                                selectedDeviceId = device.deviceId;
                            }
                        });

                        sourceSelect.onchange = () => {
                            selectedDeviceId = sourceSelect.value;
                            if (options.debug) console.log(`selectedDevice: ${sourceSelect.options[sourceSelect.selectedIndex].text} id = ${sourceSelect.value}`);
                            startup();
                        }

                        sourceSelectPanel.style.display = 'block'

                    })
                    .catch((err) => {
                        console.error(`${err.name}: ${err.message}`);
                    });
            }

            if (window.stream) {
                window.stream.getTracks().forEach(track => {
                    track.stop();
                });
            }

            navigator.mediaDevices
                .getUserMedia({ video: { deviceId: selectedDeviceId ? { exact: selectedDeviceId } : undefined }, audio: false })
                .then((stream) => {
                    video.srcObject = stream;
                    video.play();
                })
                .catch((err) => {
                    console.error(`An error occurred: ${err}`);
                    instance.invokeMethodAsync('GetError', `An error occurred: ${err}`);
                });
        } else {
            alert('不支持这个特性');
        }
        
        video.addEventListener(
            "canplay",
            (ev) => {
                if (!streaming) {
                    height = video.videoHeight / (video.videoWidth / width); 

                    if (isNaN(height)) {
                        height = width / (4 / 3);
                    }

                    video.setAttribute("width", width);
                    video.setAttribute("height", height);
                    canvas.setAttribute("width", width);
                    canvas.setAttribute("height", height);
                    streaming = true;
                }
            },
            false
        );

        startbutton.addEventListener(
            "click",
            (ev) => {
                takepicture();
                ev.preventDefault();
            },
            false
        );

        clearphoto();

        if (options.continuous) {
           takepicture();
           sendTimer = window.setInterval(async () => {
                takepicture();
            }, 5000)
        }

    } 

    function clearphoto() {
        const context = canvas.getContext("2d");
        context.fillStyle = "#AAA";
        context.fillRect(0, 0, canvas.width, canvas.height);

        const data = canvas.toDataURL("image/png");
        photo.setAttribute("src", data);
        
        if (options.continuous) {
            instance.invokeMethodAsync('GetCaptureResult', data);
        }
    }
    
    function takepicture() {
        const context = canvas.getContext("2d");
        if (width && height) {
            canvas.width = width;
            canvas.height = height;
            context.drawImage(video, 0, 0, width, height);

            const data = canvas.toDataURL("image/png");
            photo.setAttribute("src", data);
            instance.invokeMethodAsync('GetCaptureResult', data);
        } else {
            clearphoto();
        }
    }

    return true;
}

export async function getUserAgent() {
    console.log(navigator.userAgent);
    return navigator.userAgent;
}