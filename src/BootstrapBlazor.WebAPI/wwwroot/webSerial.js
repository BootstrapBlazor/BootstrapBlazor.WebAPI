export async function Init(instance, element, command) {
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

    butConnect.addEventListener("click", clickConnect);
    butwrite.addEventListener("click", write);

    if (notSupported) notSupported.classList.toggle('hidden', 'serial' in navigator);


    getPorts();

    async function getPorts() {
        let ports = await navigator.serial.getPorts();
        if (ports.length > 0) {
            port = ports[0];
            console.log('设备0', port);
            instance.invokeMethodAsync('GetLog', `设备: ${port}`);
        }
    }

    async function open() {
        console.log('open')
        if (!port.readable) {
            console.log('尝试连接');
            // Wait for the serial port to open.
            await port.open({ baudRate: 9600 });
            if (log) log.textContent += '已连接' + '\n';
            console.log('已连接');
            instance.invokeMethodAsync('GetLog', '已连接');
        }
    }

    async function write() {
        await writeToStream(fname.value);
    }

    /**
     * @name connect
     * Opens a Web Serial connection to a micro:bit and sets up the input and
     * output stream.
     */
    async function connect() {
        if (!port) {
            const filters = [
                { usbVendorId: 0x2A03, usbProductId: 0x0043 },
                { usbVendorId: 0x2341, usbProductId: 0x0001 }
            ];
            // - Request a port and open a connection.
            //port = await navigator.serial.requestPort({ filters });
            port = await navigator.serial.requestPort();
            const { usbProductId, usbVendorId } = port.getInfo();
            console.log(usbProductId, usbVendorId);
            instance.invokeMethodAsync('GetLog', usbProductId);
       }

        // - Wait for the port to open.
        await open();
        let decoder = new TextDecoderStream();
        inputDone = port.readable.pipeTo(decoder.writable);
        inputStream = decoder.readable
            .pipeThrough(new TransformStream(new LineBreakTransformer()));

        reader = inputStream.getReader();
        readLoop();

        const encoder = new TextEncoderStream();
        outputDone = encoder.readable.pipeTo(port.writable);
        outputStream = encoder.writable;

        // CODELAB: Send CTRL-C and turn off echo on REPL
        //writeToStream('\x03', 'echo(false);');

        //await writeToStream('hello', 'super man','how are you?');

        //await writeToStream('hello');
    }

    async function disconnect() {
        if (reader) {
            await reader.cancel();
            await inputDone.catch(() => { });
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
                if (log) log.textContent +='收到数据: '+ value + '\n';
                instance.invokeMethodAsync('ReceiveData', value);
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
     * Gets a writer from the output stream and send the lines to the micro:bit.
     * @param  {...string} lines lines to send to the micro:bit
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
     * TransformStream to parse the stream into lines.
     */
    class LineBreakTransformer {
        constructor() {
            // A container for holding stream data until a new line.
            this.chunks = "";
        }

        transform(chunk, controller) {
            // Append new chunks to existing chunks.
            this.chunks += chunk;
            // For each line breaks in chunks, send the parsed lines out.
            const lines = this.chunks.split("\r\n");
            this.chunks = lines.pop();
            lines.forEach((line) => controller.enqueue(line));
        }

        flush(controller) {
            // When the stream is closed, flush any remaining chunks out.
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
