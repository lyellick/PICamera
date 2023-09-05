# PICamera
.NET Web API for controlling a Raspberry PI camera module.

1. Publish PiCamera.Service:

    `dotnet publish -r linux-arm -p:PublishSingleFile=true --self-contained false`

2. Copy `Publish` folder to Rasbperry PI.

3. SSH to Rasberry PI terminal and go to the `Publish` directory.

4. Add required environmental variables. 

    `export ASPNETCORE_URLS="http://*:5000;https://*:5001"`

    `export DefaultConnection="Data Source=picamera.db"`

5. Provide execute permissions:

    `chmod 777 ./picamera`

6. Start web service:

    `./picamera`

7. From another computer open your browser to: `http://{raspi-pi}:5000/swagger`.
