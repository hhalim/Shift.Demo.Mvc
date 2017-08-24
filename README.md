# Shift.Demo.Mvc
A Shift client running in ASP.NET MVC web app. This demo demonstrates the Shift capability to run client and server in the same application process.

## Quick Startup
Install Redis for windows [Redis-x64-<version>.msi](https://github.com/MSOpenTech/redis/releases) package.

Or to use the SQL Server, first run the sql script to create Shift database in [/setup/create_db.sql](https://github.com/hhalim/Shift.Demo.Mvc/blob/master/setup/create_db.sql). 

Open this project solution in Visual Studio 2015, update the App.config connection string and cache.
```
  <connectionStrings>
    <!--
    <add name="ShiftDBConnection" connectionString="mongodb://localhost" providerName="MongoDB" />
    <add name="ShiftDBConnection" connectionString="https://localhost:8081/" providerName="DocumentDB" />
    <add name="ShiftDBConnection" connectionString="Data Source=localhost\SQL2014;Initial Catalog=ShiftJobsDB;Integrated Security=SSPI;" providerName="System.Data.SqlClient" />
    -->
    <add name="ShiftDBConnection" connectionString="localhost:6379" providerName="Redis" />
  </connectionStrings>

  <appSettings>   
    <!-- Shift running jobs settings if running process in IIS -->
    <add key="ApplicationID" value="Demo.MVC" />
    <add key="MaxRunnableJobs" value="2" />
    <add key="ShiftWorkers" value="2" />
    <!--
    <add key="ShiftPID" value="4d9dd3d371804165b9a5783051b8debe" />
    -->
    
    <!-- 
    <add key="StorageMode" value="mongo" />
    <add key="StorageMode" value="documentdb" />
    <add key="DocumentDBAuthKey" value="C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" />
    <add key="StorageMode" value="mssql" />
    -->
    <add key="StorageMode" value="redis" />

    <!-- Set to 0 or low 1 sec for StorageMode = redis/mongoDB (hour:min:sec) -->
    <add key="ProgressDBInterval" value="00:00:00" />
    
    <add key="ForceStopServer" value="true" />
    <add key="StopServerDelay" value="5000" />

    <!--
    <add key="AutoDeletePeriod" value="120" />
    <add key="ServerTimerInterval" value="5000" />
    <add key="ServerTimerInterval2" value="10000" />
    <add key="AssemblyFolder" value="" />
    <add key="ShiftEncryptionParametersKey" value="[OPTIONAL_ENCRYPTIONKEY]" /> 
    <add key="PollingOnce" value="true" />
    -->
  
  </appSettings>
```

- Build and run the site.
- Click Add Jobs link to add multiple test jobs into the queue.
- Go to the Dashboard and click `Run Server` to run Shift server. Or select jobs and click `Run Selected` to run selected jobs.
- Use the Status & Progress link to view the running jobs in auto refreshing grid. 
- Try other action buttons to see what Shift can do.

The `Stop Cmd`, `Pause Cmd`, `Continue Cmd`, and `Run-Now Cmd` buttons only marks the selected jobs for those actions. Shift server will pick up jobs and acted on them as marked. However if the jobs are manually started through `Run Selected` button, then you must manually use `Process Commands` action to execute the commands in the server process or nothing would happen to the background jobs.

