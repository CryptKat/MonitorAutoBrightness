# Monitor Auto-brightness

Software for auto-adjusting screen brightness.

Required hardware:
- Digispark ATTiny85
- Photoresistor

Required software:
- .NET Framework 4.6.1+
- [Twinkle Tray](https://github.com/xanderfrangos/twinkle-tray)
- [libusb-win32 driver](https://sourceforge.net/projects/libusb-win32/files/libusb-win32-releases/)

### Digispark ATTiny85 setup
- Connect photoresitor to ATTiny85's **P2** and **GND**
- Prepare environment for uploading firmware to Digispark ATTiny85. [Useful guide](https://www.best-microcontroller-projects.com/digispark-attiny85-arduino-install.html)
- Upload **LightSensor.ino** firmware to your ATTiny85
  - Board: Digispark
  - Clock: 16.5 MHz - for V-USB
  - Micronucleus variant: Recommended

### Application setup
- Install required software
- Add **MonitorAutoBrightness.exe --Minimized** shortcut to Autorun (shell:startup)
- Adjust settings inside MonitorAutoBrightness.exe.config, if necessary
- Adjust BrightnessLevels.txt (supports "hot reloading")

Fill with key-value pairs (key = sensor value, value = brightness level).

Example:
```
930 8
940 10
950 12
960 15
965 22
970 25
975 30
980 55
985 60
990 70
1000 80
9999 100
```
