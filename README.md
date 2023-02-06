# Monitor Auto-brightness

Software for auto-adjusting screen brightness.

Required hardware:
- Digispark ATTiny85
- Photoresistor

Required software:
- [Twinkle tray](https://github.com/xanderfrangos/twinkle-tray)
- [libusb-win32 driver](https://sourceforge.net/projects/libusb-win32/files/libusb-win32-releases/)

### Setup
- Install required software
- Connect photoresitor to ATTiny85's **P2** and **GND**
- Upload **LightSensor.ino** firmware to your ATTiny85
- Create BrightnessLevels.txt beside MonitorAutoBrightness.exe.
Fill with key-value pairs (key = sensor value, value = brightness level).

Example:
```
900 10
940 15
950 20
960 25
970 30
975 40
980 55
985 60
990 70
1000 80
1010 90
1023 100
```
- Add **MonitorAutoBrightness.exe --Minimized** shortcut to Autorun (shell:startup)
