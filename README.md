# Shift.Demo.Mvc
A Shift client running in ASP.NET MVC web app. This demo demonstrates the Shift capability to run client and server in the same application process.

## Quick Startup
- Run the sql script to create Shift database in [/setup/create_db.sql](https://github.com/hhalim/Shift.Demo.Mvc/blob/master/setup/create_db.sql). 
- If you want to use Redis cache, setup and create a Redis instance.
- Open the solution in Visual Studio, update the Web.config connection string and cache.
```
<connectionStrings>
  <add name="ShiftDBConnection" connectionString="Data Source=localhost\SQL2014;Initial Catalog=ShiftJobsDB;Integrated Security=SSPI;" providerName="System.Data.SqlClient" />
</connectionStrings>

<appSettings>
  <!-- Shift Cache - Redis -->
  <add key="UseCache" value="true" />
  <add key="RedisConfiguration" value="localhost:6379" />
</appSettings>
```
- Build and run the site.
- Click Add Jobs link to add multiple test jobs into the queue.
- Go to the Dashboard and click Run Server to run Shift server. 
- Use the Status & Progress link to view the running jobs in auto refreshing grid. 
- Try other action buttons to see what Shift can do.
