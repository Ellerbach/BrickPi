# BrickPi
This code allows you to use the excellent BrickPi from Dexter Industries running on RaspberryPi 2 with a Windows 10 IoT Core. 

## Setup the RaspberryPi running Windows 10 IoT Core
In early release of Windows 10 IoT Core, the high speed onboard serial is not supported. You need to manually change the device registry to enable the highspeed serial. Note as well that the code has been optimized to work even if the high speed serial is not fully supported. So once activated, it should just work. New drivers have/will be released for coming version which will be directly supported.
```CMD
Reg add hklm\system\controlset001\services\serpl011\parameters /v MaxBaudRateNoDmaBPS /t REG_DWORD /d 921600
Devcon restart acpi\bcm2837
```

## Setup of the project
You'll have to make sure you edit the Package.appxmanifest file in your project to add the serial port capability:

```XML
  <Capabilities>
    <Capability Name="internetClient" />
    <DeviceCapability Name="serialcommunication">
      <Device Id="any">
        <Function Type="name:serialPort" />
      </Device>
    </DeviceCapability>
  </Capabilities>
```

Then, you'll need a to reference the BickPi project to your project, add the missing using, create a private Brick class, initialize the class and pass the serial port to the Brick

```C#
private Brick brick = null;
private SerialDevice serialPort = null;

private async Task InitSerial()
{
    string aqs = SerialDevice.GetDeviceSelector();
    var dis = await DeviceInformation.FindAllAsync(aqs);
    for (int i = 0; i < dis.Count; i++)
    {
        Debug.WriteLine(string.Format("Serial device found: {0}", dis[i].Id));
        if (dis[i].Id.IndexOf("UART0") != -1)
        {
            serialPort = await SerialDevice.FromIdAsync(dis[i].Id);
        }
    }
    if (serialPort != null)
    {
        Debug.WriteLine("Serial port initialiazed");
        brick = new Brick(serialPort);
        Debug.WriteLine("Brick initialiazed");
    }
}

  private async void MainPage_Loaded(object sender, RoutedEventArgs e)
  {
      await InitSerial();
      // You are ready now!
  }
  ```
  ## USe the Sensor classes
  Using the sensor classes is straicht forward. Just reference a class and initialized it. Access properties and function. The ReadRaw(), ReadAsString() functions are common to all sensors, Value and ValueAsString properties as well. 
  A changed property event on the properties is raised with a minimum period you can determined when creating the object or later if the property has changed. That make your life easier if you're building XAML UI and want to use property binding directly in your code.
  Example creating a NXT Touch Sensor:
  ```C#
  NXTTouchSensor touch = new NXTTouchSensor(BrickPortSensor.PORT_S2);
  Debug.WriteLine(string.Format("Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", touch.ReadRaw(), touch.ReadAsString(), touch.IsPressed(), touch.NumberOfModes(), touch.GetSensorName()));
  ```
  This will create an EV3 Touch Sensor on port S1 and will tell it to check changes in properties every 20 milliseconds.
  ```C#
  EV3TouchSensor ev3Touch = new EV3TouchSensor(BrickPortSensor.PORT_S1, 20);
  ```
  
  More documentatin to come
