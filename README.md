<h1 align="center">Lift Manager</h1>

<p align="center">
  Lift manager is a dot net background service that emulates the behavior of a lift from outside and inside it. It will receive arguments where will guide the Lift Manager what to do.
</p>

<h1 align="center">Usage</h1>

This was made into a Worker to emulate the behavior of a Lift and to test it run the following command:
- dotnet run --project ./LiftManager/LiftManager.csproj --liftRequest="outside=4,inside=1,outside=2,inside=3,outside=0" 

where:
outside is a request from 'outside' the lift to a given floor by an user
inside is a request from 'inside' the lift to a given floor by an user

![Artemio_Banos_LiftManager_Demo](https://github.com/user-attachments/assets/e62b0fde-0d09-461c-b2be-8916721f8160)


### Tech stack used
- LiteDb: No-SQL lite database for small applications
- .NET: Powerful framework for developing self-containing applications 
- Serilog: Powerfol nuget package for enriched loggin
- XUnit: For testing purposes
- FluentAssertions: For more clear assertions
- AutoFixture: Help creating fake data and services for testing and provides a container to inject fake services
- Moq: To create fake services and setup to prepare its behavior before usage.

### Prerequisite
- .NET 7
  
Push the project and open a command line in the directory of the project then execute: 

### Usage
- dotnet run --project .\LiftManager\LiftManager.csproj --liftRequest="inside=1,outside=2,inside=3,outside=0"  

### Run Test
- dotnet test --project .\LiftManager\LiftManagerTests.csproj  

### Author
Artemio Banos | Software Engineer who loves developing on PC, Android and iOS with .NET, Java and Kotlin.
-  [Github: temobgallardo](https://github.com/temobgallardo/) 
-  [Linkedin: abanosga](https://www.linkedin.com/in/abanosga/)

<br/>  
