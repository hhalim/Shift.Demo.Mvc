# Shift.Demo.Mvc
A Shift client running in ASP.NET MVC web app. This demo demonstrates the Shift capability to run client and server in the same application process.

## Quick Startup
Install Redis for windows [Redis-x64-<version>.msi](https://github.com/MSOpenTech/redis/releases) package.

Or to use the SQL Server:
- Run the sql script to create Shift database in [/setup/create_db.sql](https://github.com/hhalim/Shift.Demo.Mvc/blob/master/setup/create_db.sql). 
- If you want to use Redis cache, setup and create a Redis instance. 

Open this project solution in Visual Studio, update the App.config connection string and cache.
```
<connectionStrings>
    <!-- LOCAL SQL 
    <add name="ShiftDBConnection" connectionString="Data Source=localhost\SQL2014;Initial Catalog=ShiftJobsDB;Integrated Security=SSPI;" providerName="System.Data.SqlClient" />
    -->
    <add name="ShiftDBConnection" connectionString="localhost:6379" providerName="System.Data.Redis" />
</connectionStrings>

<appSettings>
    <!-- Shift running jobs settings if running process in IIS -->
    <add key="ApplicationID" value="Demo.MVC" />
    <add key="MaxRunableJobs" value="10" />
    <add key="ShiftPID" value="4d9dd3d371804165b9a5783051b8debe" />
    
    <!-- 
    <add key="StorageMode" value="mssql" />
    -->
    <add key="StorageMode" value="redis" />

    <!-- Set to 0 or low 1 sec for StorageMode = redis-->
    <add key="ProgressDBInterval" value="00:00:00" />
    
    <!-- Shift AutoDelete -->
    <add key="AutoDeletePeriod" value="120" />

    <!-- Shift Cache - Redis -->
    <!--
    <add key="UseCache" value="true" />
    <add key="RedisConfiguration" value="localhost:6379" />
    -->
    
    <!--
    <add key="ServerTimerInterval" value="5000" />
    <add key="ServerTimerInterval2" value="10000" />
    <add key="AssemblyFolder" value="" />
    <add key="ShiftEncryptionParametersKey" value="[OPTIONAL_ENCRYPTIONKEY]" /> 
    --></appSettings>
```

- Build and run the site.
- Click Add Jobs link to add multiple test jobs into the queue.
- Go to the Dashboard and click Run Server to run Shift server. Or select jobs and click Run Selected to run selected jobs.
- Use the Status & Progress link to view the running jobs in auto refreshing grid. 
- Try other action buttons to see what Shift can do.

The `Set to Stop` and `Set to Run-Now` buttons only marks the selected jobs for those actions. Shift server will pick up jobs and acted on them as marked. 

