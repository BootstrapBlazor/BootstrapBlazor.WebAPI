import '/_content/BootstrapBlazor.WebAPI/zxing.min.js';

let width = 640;
let height = 0;
let streaming = false;

let log = null;
let video = null;
let canvas = null;
let photo = null;
let startbutton = null;
let timmer = false;
let sourceSelect = null;
let sourceSelectPanel = null;
let selectedDeviceId = null;
let quality = 0.8;
let options = null;
let decodeTimer = false;
let codeReaderImage = null;
let supportsVibrate = false;
let tryInvertColors = 0;
export function vibrate() {
    if (supportsVibrate) navigator.vibrate(1000);
}

export function Capture(instance, element, opt, command) {
    supportsVibrate = "vibrate" in navigator;
    if (command == 'restart') {
        restart();
    }

    options = opt;
    if (options.quality) {
        quality = options.quality;
    }

    if (command == 'start') {
        startup();
    } else if (command == 'takepicture') {
        takepicture();
        return;
    } else if (command == 'decode') {
        decodeTimer = !decodeTimer;
        tryInvertColors = 0;
        if (decodeTimer) {
            codeReaderImage = new ZXing.BrowserMultiFormatReader();
            var interval = setInterval(() => {
                makepicture();
                if (decodeTimer == false) {
                    clearInterval(interval);
                    return;
                }
            }, 100);
        }
        return;
    } else if (command == 'destroy') {
        destroy();
        return;
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

    function restart() {
        width = 640;
        height = 0;
        streaming = false;

        log = null;
        video = null;
        canvas = null;
        photo = null;
        startbutton = null;
        timmer = false;
        sourceSelect = null;
        sourceSelectPanel = null;
        selectedDeviceId = null;
        quality = 0.8;
        options = null;
        decodeTimer = false;
        codeReaderImage = null;
        tryInvertColors = 0;
    }

    function startup() {
        if (showViewLiveResultButton()) {
            return;
        }

        timmer = false;
        decodeTimer = false;
        log = element.querySelector("[data-action=log]");
        video = element.querySelector("[data-action=video]");
        canvas = element.querySelector("[data-action=canvas]");
        photo = element.querySelector("[data-action=photo]");
        startbutton = element.querySelector("[data-action=startbutton]");
        sourceSelect = element.querySelector("[data-action=sourceSelect]");
        sourceSelectPanel = element.querySelector("[data-action=sourceSelectPanel]");

        destroy();

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

        } else if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {

            if (!navigator.mediaDevices?.enumerateDevices) {
                console.log("enumerateDevices() not supported.");
            } else {
                if (!options.width) options.width = 640;
                if (!options.height) options.height = 480;
                width = options.width;
                console.log(`Set: ${selectedDeviceId} video ${options.width} x ${options.height}`);
                var constraints = {
                    video: {
                        width: { ideal: options.width },
                        height: { ideal: options.height },
                        facingMode: "environment",
                        focusMode: "continuous",
                    }, audio: false
                };

                if (selectedDeviceId != null || options.deviceID != null) {
                    let deviceId = selectedDeviceId;
                    if (deviceId == null) deviceId = options.deviceID;
                    constraints = {
                        video: {
                            deviceId: deviceId ? { exact: deviceId } : undefined,
                            width: { ideal: options.width },
                            height: { ideal: options.height },
                            facingMode: "environment",
                            focusMode: "continuous",
                            //fillLightMode: "flash",
                            //torch: "true",
                            //focusDistance: 0.1,
                        },
                        audio: false
                    }
                    console.log(constraints.video.deviceId);
                }
                navigator.mediaDevices
                    .getUserMedia(constraints)
                    .then((stream) => {

                        try {
                            video.srcObject = null;
                        }
                        catch (err) {
                            video.src = '';
                        }
                        if (video) {
                            video.removeAttribute('src');
                        }

                        video.srcObject = stream;
                        video.play();

                        if (options.effectPreview) {
                            timmer = true;
                            // 每 100 毫秒将视频流绘制到画布上
                            var interval = setInterval(() => {
                                makepicture();
                                if (timmer == false) {
                                    clearInterval(interval);
                                    return;
                                }
                            }, 100);
                        }

                        if (selectedDeviceId == null) {
                            navigator.mediaDevices.enumerateDevices()
                                .then((devices) => {
                                    let videoInputDevices = [];
                                    devices.forEach((device) => {
                                        if (device.kind === 'videoinput') {
                                            videoInputDevices.push(device);
                                        }
                                    });
                                    if (options.deviceID != null) {
                                        selectedDeviceId = options.deviceID
                                    } else if (videoInputDevices.length > 1) {
                                        selectedDeviceId = videoInputDevices[1].deviceId
                                    } else {
                                        selectedDeviceId = videoInputDevices[0].deviceId
                                    }
                                    devices.forEach((device) => {
                                        if (device.kind === 'videoinput') {
                                            if (options.debug) console.log(`${device.label} id = ${device.deviceId}`);
                                            const sourceOption = document.createElement('option');
                                            if (device.label === '') {
                                                sourceOption.text = 'Camera' + (sourceSelect.length + 1);
                                            } else {
                                                sourceOption.text = device.label
                                            }
                                            sourceOption.value = device.deviceId
                                            if (selectedDeviceId != null && device.deviceId == selectedDeviceId) {
                                                sourceOption.selected = true;
                                            }
                                            sourceSelect.appendChild(sourceOption)
                                        }
                                    });

                                    sourceSelect.onchange = () => {
                                        selectedDeviceId = sourceSelect.value;
                                        if (options.debug) console.log(`selectedDevice: ${sourceSelect.options[sourceSelect.selectedIndex].text} id = ${sourceSelect.value}`);
                                        instance.invokeMethodAsync('SelectDeviceID', selectedDeviceId);
                                        startup();
                                    }

                                    sourceSelectPanel.style.display = 'block'

                                })
                                .catch((err) => {
                                    console.error(`${err.name}: ${err.message}`);
                                });
                        }
                    })
                    .then((photoCapabilities) => {
                    })
                    .then((photoSettings) => {
                    })
                    .catch((err) => {
                        console.error(`An error occurred: ${err}`);
                        instance.invokeMethodAsync('GetError', `An error occurred: ${err}`);
                    });

            }


        } else {
            alert('不支持这个特性');
        }


        video.removeEventListener('canplay', videoCanPlayListener);

        video.addEventListener("canplay", videoCanPlayListener, false);

        if (startbutton) {
            startbutton.removeEventListener('canplay', videoCanPlayListener);
            startbutton.addEventListener("click", takepictureListener, false);
        }
        clearphoto();

        if (options.continuous) {
            takepicture();
            sendTimer = window.setInterval(() => {
                takepicture();
            }, 5000)
        }

    }
    function takepictureListener(ev) {
        takepicture();
        ev.preventDefault();
    }

    function videoCanPlayListener() {
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
            //if (options.debug)
            console.log(`play DeviceId: ${selectedDeviceId} video ${video.videoWidth} x ${video.videoHeight}`);
        }

    }

    function clearphoto() {
        const context = canvas.getContext("2d");
        context.fillStyle = "#AAA";
        context.fillRect(0, 0, canvas.width, canvas.height);

        const data = canvas.toDataURL("image/png");
        if (photo) photo.setAttribute("src", data);

        if (options.continuous) {
            const data2 = canvas.toDataURL("image/jpeg", quality);
            instance.invokeMethodAsync('GetCaptureResult', data2);
        }
    }

    function takepicture() {
        makepicture(false);
    }

    function makepicture(preview = true) {
        const context = canvas.getContext("2d");
        if (width && height) {
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, video.videoWidth, video.videoHeight, 0, 0, canvas.width, canvas.height);

            if (options.debug) console.log(`take: ${video.videoWidth} x ${video.videoHeight} , effect: ${options.effect}`);

            if (options.effect > 0) {
                const imgData = context.getImageData(0, 0, canvas.width, canvas.height);
                const data = imgData.data;
                let luminance = 60 // 调节参数
                for (var i = 0; i < data.length; i += 4) {
                    if (options.effect == 1) {
                        //反色
                        data[i + 0] = 255 - data[i + 0] // r，红通道
                        data[i + 1] = 255 - data[i + 1] // g，绿通道
                        data[i + 2] = 255 - data[i + 2] // b，蓝通道
                    }
                    else if (options.effect == 2) {
                        //灰度
                        const grayscale = data[i] * 0.3 + data[i + 1] * 0.6 + data[i + 2] * 0.1
                        data[i + 0] = grayscale // r，红通道
                        data[i + 1] = grayscale // g，绿通道
                        data[i + 2] = grayscale // b，蓝通道 
                    }
                    else if (options.effect == 3) {
                        //反色+灰度
                        data[i + 0] = 255 - data[i + 0] // r，红通道
                        data[i + 1] = 255 - data[i + 1] // g，绿通道
                        data[i + 2] = 255 - data[i + 2] // b，蓝通道

                        const grayscale = data[i] * 0.3 + data[i + 1] * 0.6 + data[i + 2] * 0.1
                        data[i + 0] = grayscale // r，红通道
                        data[i + 1] = grayscale // g，绿通道
                        data[i + 2] = grayscale // b，蓝通道 
                    }
                    else if (options.effect == 15) {
                        //反色+黑白
                        data[i + 0] = 255 - data[i + 0] // r，红通道
                        data[i + 1] = 255 - data[i + 1] // g，绿通道
                        data[i + 2] = 255 - data[i + 2] // b，蓝通道

                        const r = data[i + 0]
                        const g = data[i + 1]
                        const b = data[i + 2]
                        const avg = (r + g + b) / 3
                        data[i + 0] = data[i + 1] = data[i + 2] = avg >= 128 ? 255 : 0
                    }
                    else if (options.effect == 4) {
                        //黑白
                        const r = data[i + 0]
                        const g = data[i + 1]
                        const b = data[i + 2]
                        const avg = (r + g + b) / 3
                        data[i + 0] = data[i + 1] = data[i + 2] = avg >= 128 ? 255 : 0
                    }
                    else if (options.effect == 5) {
                        //调亮
                        data[i + 0] += luminance // r，红通道
                        data[i + 1] += luminance // g，绿通道
                        data[i + 2] += luminance // b，蓝通道
                    }
                    else if (options.effect == 6) {
                        //调暗
                        data[i + 0] -= luminance // r，红通道
                        data[i + 1] -= luminance // g，绿通道
                        data[i + 2] -= luminance // b，蓝通道
                    }
                    else if (options.effect == 7) {
                        //透明度
                        data[i + 3] -= 100 // a通道，设置不透明度
                    }
                    else if (options.effect == 8) {
                        //模糊
                        const n = 30 // 模糊程度
                        const m = 1 / (n * n)
                        const matrix = []
                        for (let i = 0; i < n * n; i++) {
                            matrix.push(m)
                        }
                        const r = data[i + 0]
                        const g = data[i + 1]
                        const b = data[i + 2]
                        const avg = (r + g + b) / 3
                        data[i + 0] = data[i + 1] = data[i + 2] = avg >= 128 ? 255 : 0
                    }
                    else if (options.effect == 9) {
                        //RGB蒙版
                    }
                    else if (options.effect == 10) {
                        //老照片滤镜
                    }
                    else if (options.effect == 11) {
                        //马赛克
                    }
                    else if (options.effect == 12) {
                        //RGB通道_红
                    }
                    else if (options.effect == 13) {
                        //RGB通道_绿
                    }
                    else if (options.effect == 14) {
                        //RGB通道_蓝
                    }
                }
                context.putImageData(imgData, 0, 0)
            }

            const data = canvas.toDataURL("image/jpeg", quality);
            if (!preview) {
                if (photo) photo.setAttribute("src", data);
                instance.invokeMethodAsync('GetCaptureResult', data);
            }
            if (decodeTimer) {
                codeReaderImage.decodeFromImageUrl(data).then(result => {
                    if (result) {
                        console.log("解码成功,模式" + options.effect);
                        decodeTimer = false;
                        tryInvertColors = 0;
                        vibrate();
                        if (options.debug) console.log(result.text);
                        instance.invokeMethodAsync('GetDecode', result.text)
                    }
                }).catch((err) => {
                    if (err) {
                        if (options.debug) console.log(err)
                        instance.invokeMethodAsync('GetError', err.message)
                    }
                });

                if (options.effect == 0 && tryInvertColors == 3) {
                    console.log("主动切换反色+黑白解码, 解码尝试" + tryInvertColors);
                    options.effect = 15;
                } else if (tryInvertColors == 6) {
                    console.log("主动切换普通解码, 解码尝试" + tryInvertColors);
                    tryInvertColors = 0;
                    options.effect = 0;
                }
                tryInvertColors += 1;
            }
        } else {
            clearphoto();
        }
    }

    function destroy() {
        timmer = false;
        decodeTimer = false;
        tryInvertColors = 0;
        video = element.querySelector("[data-action=video]");
        if (video.srcObject) {
            video.srcObject.getTracks().forEach(track => {
                track.stop();
                console.log(track.label + ' stop');
            });
        }
    }
    return true;
}
