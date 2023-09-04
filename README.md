# GraphTerminal
## Description
A multiplatform terminal-based tool for visualizing large amounts of temperature information from MQTT brokers.<br><br>
Platforms:
* Windows X-86
* Windows X-64
* Windows ARM (Untested)
* Windows ARM64 (Untested)
* Linux X-64
* Linux ARM (Untested)
* Linux ARM64 (Untested)
* OSX-X64 (Untested)
* OSX-ARM (Untested)
## Dependencies
* .net7.0
* [ConsoleGUI](https://github.com/TomaszRewak/C-sharp-console-gui-framework) by: Tomasz Rewak.
* [MQTTnet](https://github.com/dotnet/MQTTnet) by: dotnet.
## Usage
Launch parameters
>Launch parameters are defined by a token, followed by a corresponding value separated by a space.
>| Token         | Description   |
>| ------------- |:-------------:|
>| -url          | Broker address|
>| -usr          | Login username|
>| -pwd          | Login password|
>| -top          | MQTT topic    |

Example syntax:
>```
>GraphTerminal.exe -url mybrokeraddress -usr username -pwd password -top Temperature_Sensors
>```

### Launch instructions

**Windows**<br>
>**Desktop**
>>1. Create a new shortcut to the executable 
>>2. Open the properties of the shortcut
>>3. Add your launch parameters to the target path after the executable path with a preceeding space
>> ![Launch parameters.](/Images/0_fig0.png "Add your launch parameters here.")
>>4. Launch the application

>**Command line / Powershell**
>>1. Navigate to the executable directory
>>2. Type the exectuable name, followed by your launch parameters separated with a space
>>3. Launch the application

**Linux / OSX**<br>
>1. Make sure you have [.net7.0 runtime](https://learn.microsoft.com/en-us/dotnet/core/install/linux) installed.
>2. Navigate to the executable directory
>3. Type `./GraphTerminal`, followed by our launch parameters
>4. Launch the application
>5. (Please note that shells like Bash don't support certain characters in launch parameters, so if your username, password, URL or topic includes any of these characters, you need to preceed them with a backslash, or type the whole parameter containing said character within single quotes).

### Running the application
As you launch the application, information about the broker connection is displayed.<br>
![Connection info.](/Images/1_fig0.png "Information about the connection process is shown as such.")<br>
The applications needs an initial sample of message before displaying a graph.<br> This depends completely on the refreshing frequency of the current topic the application is subscribed to. In some cases, this may take up to a minute.<br>
![Program display.](/Images/1_fig1.png "Displaying 128 sensors.")<br>
The figure above shows the following information:
1. Subscribed topic and timestamp from last message
2. Bar graph of the individual sensor readings within the message
3. Range of temperatures between the highest and lowest reading 
4. Pointer to highest and lowest value in the sensor group

## Compatible format
The program consumes and displays temperature readings according to a specific format.<br>
The message is formatted in the following manner:<br>
```
{
    "DeviceID": "GroupName",                //Name of the sensor group
    "Sensors": [                            //Array containing every sensor as an object
        {
            "SensorID": "Sensor001"         //Sensor name
            "Value": "20.00"                //Sensor temperature rading
        },
        {
            "SensorID": "Sensor002"         
            "Value": "20.00"                
        },
        ...
        {
            "SensorID": "Sensor128"         
            "Value": "20.00"                
        }
    ]
}
```
The root of the message contains the sensor group name `DeviceID`, and each sensor in the group as objects in the `Sensors` array.<br>
Each sensor object contains the sensor name `SensorID`, and the sensor reading `Value` as a decimal.<br>

Please note that the number of sensors that can be displayed at once is limited by the number of available columns on your terminal.<br>
Any readings exceeding this amount will not be displayed.<br>The columns occupied by the temperature labels also affect this limit.

---
<p align="center">

![Charm logo.](/Images/CharmLogo.png)<br>
 [charm-ecsel.eu](https://charm-ecsel.eu/)<br>

</p>

![Charm footer logo.](/Images/Charm_footer_website.png)<br>