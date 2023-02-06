﻿using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Main;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace MonitorAutoBrightness
{
    public class ArduinoUsbDevice : IDisposable
    {
        private readonly UsbDeviceFinder _myUsbFinder;

        private readonly int _productId;
        private readonly int _vendorId;
        private readonly IDeviceNotifier _usbDeviceNotifier;

        private UsbDevice _usbDevice;

        public bool IsAvailable { get; private set; }

        //default values for the DigiSpark
        public ArduinoUsbDevice()
            : this(0x16c0, 0x05df)
        {
        }

        public ArduinoUsbDevice(int vendorId, int productId)
        {
            _vendorId = vendorId;
            _productId = productId;

            _myUsbFinder = new UsbDeviceFinder(_vendorId, _productId);
            _usbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();

            _usbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent;

            ConnectUsbDevice();
        }



        public event EventHandler<EventArgs> ArduinoUsbDeviceChangeNotifier;

        private void ConnectUsbDevice()
        {
            _usbDevice = UsbDevice.OpenUsbDevice(_myUsbFinder);

            if (_usbDevice != null)
            {
                IsAvailable = true;

                if (ArduinoUsbDeviceChangeNotifier != null)
                    ArduinoUsbDeviceChangeNotifier.Invoke(true, null);
            }
            else
            {
                IsAvailable = false;
            }
        }

        private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
        {
            if (e.Device.IdVendor == _vendorId && e.Device.IdProduct == _productId)
            {
                if (e.EventType == EventType.DeviceArrival)
                {
                    ConnectUsbDevice();
                }

                else if (e.EventType == EventType.DeviceRemoveComplete)
                {
                    _usbDevice = null;

                    IsAvailable = false;

                    if (ArduinoUsbDeviceChangeNotifier != null)

                        ArduinoUsbDeviceChangeNotifier.Invoke(false, null);
                }
            }
        }

        public string GetStringDescriptor(byte index)
        {
            if (IsAvailable == false)
                return null;

            var packet = new UsbSetupPacket((byte)UsbEndpointDirection.EndpointIn,
                (byte)UsbStandardRequest.GetDescriptor,
                (short)(0x0300 | index), // (usb.util.DESC_TYPE_STRING << 8) | index
                0, //Language ID
                255); //Length

            var byteArray = new byte[256];
            _usbDevice.ControlTransfer(ref packet, byteArray, byteArray.Length, out _);

            return Encoding.Unicode.GetString(byteArray);
        }

        public bool WriteByte(byte value)
        {
            if (IsAvailable == false)
                return false;

            var packet = new UsbSetupPacket(
                (byte)(UsbCtrlFlags.RequestType_Class | UsbCtrlFlags.Recipient_Device | UsbCtrlFlags.Direction_Out),
                0x09, // USBRQ_HID_SET_REPORT 
                0x300, // (USB_HID_REPORT_TYPE_FEATURE << 8) | 0,
                value, // the byte to write
                0); // according to usbdevice.py this is ignored, so passing in 0
            return _usbDevice.ControlTransfer(ref packet, null, 0, out _);
        }

        public bool WriteBytes(byte[] values)
        {
            if (IsAvailable == false)
                return false;

            bool result = true;

            foreach (byte value in values)
            {
                result &= WriteByte(value);
            }

            return result;
        }

        public bool ReadByte(out byte value)
        {
            var buffer = new byte[1];
            value = byte.MinValue;

            if (IsAvailable == false)
                return false;

            var packet = new UsbSetupPacket(
                (byte)(UsbCtrlFlags.RequestType_Class | UsbCtrlFlags.Recipient_Device | UsbCtrlFlags.Direction_In),
                0x01, // USBRQ_HID_GET_REPORT 
                0x300, // (USB_HID_REPORT_TYPE_FEATURE << 8) | 0,
                0, // according to usbdevice.py this is ignored, so passing in 0
                1); // length

            bool sendResult = _usbDevice.ControlTransfer(ref packet, buffer, 1, out int numBytesTransferred);
            bool result = sendResult & (numBytesTransferred > 0);
            if (result)
                value = buffer[0];

            return result;
        }

        public bool ReadBytes(out byte[] buffer, int length, uint timeout)
        {
            var sw = Stopwatch.StartNew();
            int bytesRead = 0;
            buffer = new byte[length];
            while (sw.ElapsedMilliseconds < timeout && bytesRead < length)
            {
                while (ReadByte(out byte newByte) && bytesRead < length)
                {
                    buffer[bytesRead++] = newByte;
                }

                if (bytesRead == length)
                    return true;

                Thread.Sleep(50);
            }
            return false;
        }

        #region Dispose implementation

        private bool _disposed;
        ~ArduinoUsbDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed && _usbDeviceNotifier != null)
            {
                _usbDeviceNotifier.OnDeviceNotify -= OnDeviceNotifyEvent;
            }

            _disposed = true;
        }

        #endregion
    }
}