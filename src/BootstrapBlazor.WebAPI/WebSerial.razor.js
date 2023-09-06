let frameBreakChar = "\n"
let autoFrameBreak = true;
export async function Init(instance, element, options, command) {
    let port;
    let reader;
    let inputDone;
    let outputDone;
    let inputStream;
    let outputStream;

    const log = element.querySelector("[data-action=log]");
    const butConnect = element.querySelector("[data-action=butConnect]");
    const butwrite = element.querySelector("[data-action=butwrite]");
    const fname = element.querySelector("[data-action=fname]");
    const notSupported = element.querySelector('[data-action=notSupported]');

    fname.style.visibility = "visible";
    butwrite.style.visibility = "visible";
    butConnect.addEventListener("click", clickConnect);
    butwrite.addEventListener("click", write);

    if (notSupported) notSupported.classList.toggle('hidden', 'serial' in navigator);

    if (!"serial" in navigator) {
        instance.invokeMethodAsync('GetError', "The Web Serial API is not supported"); 
        return;
    }

    getPorts();

    if (options.frameBreakChar) frameBreakChar = options.frameBreakChar;
    autoFrameBreak = options.autoFrameBreak;

    async function getPorts() {
        let ports = await navigator.serial.getPorts();
        if (ports.length > 0) {
            port = ports[0];
            console.log('设备0', port);
            instance.invokeMethodAsync('GetLog', `获取到授权设备: ${ports.length}`);
        }
    }

    async function open() {
        console.log('open')
        if (!port.readable) {
            console.log('尝试连接');
            // Wait for the serial port to open.
            if (!options) options = { baudRate: 9600 };
            await port.open(options);
            if (log) log.textContent += '已连接' + '\n';
            console.log('已连接', port);
            instance.invokeMethodAsync('GetLog', `已连接:${options.baudRate}`);
        }
    }

    async function write() {
        await writeToStream(fname.value);
    }

    async function connect() {
        try {
            if (!port) {
                const filters = [
                    { usbVendorId: 0x2A03, usbProductId: 0x0043 },
                    { usbVendorId: 0x2341, usbProductId: 0x0001 }
                ];
                // - 请求端口并打开连接.
                //port = await navigator.serial.requestPort({ filters });
                port = await navigator.serial.requestPort();
                const { usbProductId, usbVendorId } = port.getInfo();
                console.log(usbProductId, usbVendorId);
                instance.invokeMethodAsync('GetLog', usbProductId);
            }

            await open();
            let decoder = new TextDecoderStream();
            if (!options.outputInHex) {
                inputDone = port.readable.pipeTo(decoder.writable);
                if (autoFrameBreak) {
                    inputStream = decoder.readable
                    .pipeThrough(new TransformStream(new LineBreakTransformer()));
                } else {
                    inputStream = decoder.readable;
                } 
                reader = inputStream.getReader();
            } else {
                reader = port.readable.getReader();
            }
            readLoop();

            const encoder = new TextEncoderStream();
            outputDone = encoder.readable.pipeTo(port.writable);
            outputStream = encoder.writable;
            instance.invokeMethodAsync('Connect', true);
       } catch (error) {
            console.error(error);
            instance.invokeMethodAsync('GetError', error.message);
        }
    }

    async function disconnect() {
        try {
            if (reader) {
                await reader.cancel();
                if (!options.outputInHex) await inputDone.catch(() => { });
                reader = null;
                inputDone = null;
            }
            if (outputStream) {
                await outputStream.getWriter().close();
                await outputDone;
                outputStream = null;
                outputDone = null;
            }
            await port.close();
            port = null;
            instance.invokeMethodAsync('Connect', false);
       } catch (error) {
            console.error(error);
            instance.invokeMethodAsync('GetError', error.message);
        }
    }

    async function clickConnect() {
        if (port && port.readable) {
            await disconnect();
            toggleUIConnected(false);
            return;
        }
        await connect();
        toggleUIConnected(true);
    }

    async function readLoop() {
        while (true) {
            const { value, done } = await reader.read();
            if (value) {
                if (log) log.textContent += '收到数据: ' + value + '\n';
                instance.invokeMethodAsync('ReceiveData', value + "");
            }
            if (done) {
                console.log('[readLoop] DONE', done);
                instance.invokeMethodAsync('GetLog', `接收数据完成: ${done}`);
                reader.releaseLock();
                break;
            }
        }
    }

    /**
     * @name writeToStream
     * 从输出流获取 writer 并将行发送到 micro:bit.
     * @param  {...string} lines 行发送到 micro:bit
     */
    async function writeToStream(...lines) {
        const writer = outputStream.getWriter();
        lines.forEach(async (line) => {
            if (log) log.textContent += '发送数据' + line + '\n';
            console.log('[SEND]', line);
            instance.invokeMethodAsync('GetLog', `发送数据: ${line}`);
            await writer.write(line);
        });
        writer.releaseLock();
    }

    /**
     * @name LineBreakTransformer
     * Transform Stream 将流解析为行.
     */
    class LineBreakTransformer {
        constructor() {
            // 用于保存流数据直到新行的容器。
            this.chunks = "";
        }

        transform(chunk, controller) {
            console.log('[GET]', chunk);
            // 将新块附加到现有块。
            this.chunks += chunk + "";
            // 对于块中的每个换行符，将解析后的行发送出去。
            const lines = this.chunks.split(frameBreakChar);
            this.chunks = lines.pop();
            lines.forEach((line) => controller.enqueue(line));
        }

        flush(controller) {
            // 当流关闭时，刷新所有剩余的块。
            controller.enqueue(this.chunks);
        }
    }

    function toggleUIConnected(connected) {
        let lbl = "连接";
        if (connected) {
            lbl = "断开连接";
        }
        butConnect.textContent = lbl;
    }


}
