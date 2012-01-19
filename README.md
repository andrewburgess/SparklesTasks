Sparkles Tasks
==============

This project contains some custom NAnt functions and tasks that I needed for creating some project setup scripts

Setup
-----

I use functionality from some Microsoft SDKs, so you'll need to have those available to build/run the tasks

The ones I use are for:

* SQL Server 2008 (all located in %SQL_SERVER_ROOT%/100/SDK/Assemblies)
* IIS 7 (usually at %SYSTEM_ROOT%/System32/inetsrv/Microsoft.Web.Administration.dll)

Using
-----

### Functions

I have two functions currently implemented, `iis-site-exists` and `db-exists`

`iis-site-exists` takes one parameter (the site's name in IIS), and returns a True if the site was found, and False otherwise

Example

	<if test="${not sparkles::iis-site-exists(YourSite)}">
		<echo message="Site was not found" />
	</if>

`db-exists` takes one or two parameters. If only one parameter is provided, that is the database name (with a default server of `localhost`). 
If two parameters are provided, the first parameter is the server name, and the second parameter is the database name. It will
return True if the database exists in SQL Server, False otherwise.

Example

	<if test="${not sparkles::db-exists(YourDatabase)}">
		<echo message="Database not found" />
	</if>
	
	<if test="${not sparkles::db-exists(YourServer, YourDatabase)">
		<echo message="Database not found on server" />
	</if>
	
### Tasks

I implemented two tasks, one pretty basic and one a bit more complex.

The first is `movedir` which I implemented due to a bug in the core NAnt tasks when moving directories (https://github.com/nant/nant/issues/11). It takes
two parameters, `from` and `to` which are pretty self explanatory.

Example

	<movedir from="/path/of/original/directory" to="/path/of/new/directory" />
	
The second task I implemented is `restore-db` which will restore a database backup to a SQL Server instance

* `replace` - *Boolean* - Set true to overwrite an existing database on the server with the backup
* `db-name` - *String*, **Required** - Name of the database to restore to
* `backup-path` - *String*, **Required** - Location of the database backup to use
* `server` - *String* - Server instance to use

Example

	<restore-db replace="true" db-name="SuperAwesomeDatabase" backup-path="/path/to/the/backup.bak" />