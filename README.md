# PICamera
.NET Web API for controlling a Raspberry Pi camera module.

*NOTE: Rasberry Pi Zero 1 is not supported.*

1. Publish PiCamera.Service:

    `dotnet publish -r linux-arm -p:PublishSingleFile=true --self-contained false`

2. Copy `Publish` folder to Rasbperry Pi.

3. SSH to Rasberry Pi and go to the `Publish` directory you just copied.

4. Add required environmental variables. 

    `export ASPNETCORE_URLS="http://*:5000;https://*:5001"`

    `export DefaultConnection="Data Source=picamera.db"`

    `export AdminAccessKey="{GUID}"`

6. Provide execute permissions:

    `chmod 777 ./picamera`

7. Start web service:

    `./picamera`

8. From another computer open your browser to: `http://{raspi-pi}:5000/swagger`.
