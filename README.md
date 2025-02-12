# BusInfoMVCSample
Bus Information System for Gyeonggi-do (Gyeonggi Province), South Korea. The estimated arrival time is calculated based on real-time bus location data retrieved from the server once every 20 seconds. The system displays refined information on the Windows Forms Project.

<div align="center">
<img alt="GitHub Last Commit" src="https://img.shields.io/github/last-commit/happybono/BusInfoMVCSample"> 
<img alt="GitHub Repo Size" src="https://img.shields.io/github/repo-size/happybono/BusInfoMVCSample">
<img alt="GitHub Repo Languages" src="https://img.shields.io/github/languages/count/happybono/BusInfoMVCSample">
<img alt="GitHub Top Languages" src="https://img.shields.io/github/languages/top/HappyBono/BusInfoMVCSample">
</div>

## Features
- Retrieve and display real-time bus arrival information.
- Display bus route names, destinations, and arrival times.
- Show remaining stops and congestion levels.
- Indicate low-floor buses for accessibility.
- Handle error messages and provide updates.

## Technologies Used
- C#
- .NET Framework
- Windows Forms
- MVC Pattern (**This sample is designed to be easily understandable for those new to the MVC pattern.**)

## Advantages of Using MVC Pattern
- Separation of Concerns : Divides the application into three interconnected components - Model, View, and Controller. This separation makes it easier to manage and scale the application.
- Maintainability : Changes in one component (like the user interface) can be made independently of the others (like business logic or data), which simplifies maintenance and updates.
- Testability : Each component can be tested individually, making it easier to perform unit testing and ensure high code quality.
- Reusability : Components such as Models and Controllers can be reused across different views, promoting code reusability and reducing duplication.
- Flexibility : Enhances the flexibility of the application by allowing developers to work on different parts of the application simultaneously without interfering with each other's work.

## Usage
### MainForm
The `MainForm` class is the main user interface that displays real-time bus arrival information. It consists of various labels and panels to show bus routes, arrival times, remaining stops, congestion levels, and error messages.

### Timers
Two timers are used in the application:
- `_timer` : Updates bus information every 20 seconds.
- `_currentTimeTimer` : Updates the current time every second.

### Controllers
`BusController` : Handles the logic for retrieving bus information and processing it for display.

### Models
`BusInfo` : Represents the information of a bus, including route name, destination, arrival times, stop counts, congestion levels, and error messages.

### Code Structure
`MainForm.cs` : Contains the logic for initializing timers, updating bus information, and displaying it on the form.
`BusController.cs` : Manages the retrieval and processing of bus information.
`BusInfo.cs` : Defines the data structure for bus information.

### Functions and Methods
#### MainForm.cs
- `MainForm()` : Constructor that initializes the form and timers.
- `MainForm_Load(object sender, EventArgs e)` : Asynchronously updates bus information when the form loads.
- `InitializeTimer()` : Initializes the timer to update bus information every 20 seconds.
- `InitializeCurrentTimeTimer()` : Initializes the timer to update the current time every second.
- `Timer_Tick(object sender, EventArgs e)` : Asynchronous method to update bus information at each timer tick.
- `UpdateBusInfo()` : Asynchronously retrieves bus information and displays it.
- `DisplayBusInfo(List<BusInfo> busInfoList)` : Displays the retrieved bus information on the form.
- `CurrentTimeTimer_Tick(object sender, EventArgs e)` : Updates the current time label every second.

#### BusController.cs
- `GetBusInfoAsync()`: Asynchronously retrieves bus information from the data source.
- `GetUrgentBusInfo(List<BusInfo> busInfoList)` : Retrieves urgent bus information that needs immediate attention.
- `GetCongestionText(int congestionLevel)` : Returns a textual representation of the congestion level.
- `GetLowPlateText(bool isLowPlate)` : Returns a textual representation of whether the bus is a low-floor bus.

#### BusInfo.cs
- `RouteName` : Name of the bus route.
- `DestName` : Destination of the bus route.
- `ArrivalTime1` : First arrival time of the bus.
- `ArrivalTime2` : Second arrival time of the bus.
- `StopCount1` : Remaining stops until the first arrival.
- `StopCount2` : Remaining stops until the second arrival.
- `Crowded1` : Congestion level for the first arrival.
- `Crowded2` : Congestion level for the second arrival.
- `LowPlate1` : Indicates if the first bus is a low-floor bus.
- `LowPlate2` : Indicates if the second bus is a low-floor bus.
- `ErrorMessage` : Error message if there is an issue with retrieving bus information.

## License
This project is licensed under the MIT License. See the `LICENSE` file for details.

## Copyright 
Copyright ⓒ HappyBono 2025. All rights Reserved.
