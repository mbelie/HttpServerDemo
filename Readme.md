# HttpServerDemo

## Summary
This .NET Core 8.0 solution demonstrates a simple HTTP server that exposes a weather forecast endpoint. The weather foreast endpoint relies on the [National Weather Service](https://www.weather.gov/documentation/services-web-api) API to return weather forecast data for a given latitude/longitude 

## Projects
- Http
  - A .NET Core class library
  - Contains HTTP related concrete types and interfaces including SimpleHttpServer
- HttpServerConsole
   - A .NET Core console application that provides the entry point for the demo
   - The application hosts an HTTP server (via SimpleHttpServer) that can be accessed from a browser, Postman, etc
   - The application has an appsettings.json file where the user can configure the port of the HTTP server and a User-Agent string (required by the National Weather Service API)
      - Port: Used to specify the localhost port the HTTP server will run on
      - UserAgent: A unique string required by the National Web Service endpoint
- HttpServerTests
   - An MSTest test project for the HttpServer namespace
   - The project is stubbed out for future completion
- HttpTests
   - An MSTest test project for the Http namespace
   - The project is stubbed out for future completion
- WeatherService
   - Contains weather-related interfaces, types, and services including a service called NationalWeatherService that contains the logic necessary to interface with the National Weather Service API mentioned above
- WeatherServiceTests
   - An MSTest test project for the WeatherService namespace
   - The project is stubbed out for future completion


## Instructions
- Clone or download the repo
- In appsettings.json, specify the Port and UserAgent values or leave them as is (the defaults work)
- Open the solution in Visual Studio 2022 and build
- Ensure that the HttpSever project is set as the startup project
- Press F5
- Open a browser and enter a URL with the following format (use the port specified in appsettings.json): 
   - Format: ht<span>tp://</span>localhost:&lt;port&gt;/Weather/&lt;latitude&gt;,&lt;longitude&gt;
   - Example: http://localhost:9000/Weather/33.7070,-117.0845
- JSON responses will appear in the browser