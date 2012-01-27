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

#### iis-exists(*siteName*)

`iis-site-exists` takes one parameter (the site's name in IIS), and returns a True if the site was found, and False otherwise

Example

	<if test="${not sparkles::iis-site-exists(YourSite)}">
		<echo message="Site was not found" />
	</if>

#### db-exists(*servername*, *databaseName*), db-exists(*databaseName*)

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

#### file-contains(*path*, *test*)

`file-contains` takes two parameters: the filepath and a regex string to use to test the file with. Use this to test if a file has a particular string in it

Example

	<if test="$(not sparkles::('/path/to/file.txt', 'test at the end$')">
		<echo append="true" file="/path/to/file.txt" message="test at the end" />
	</if>

### Tasks

#### movedir

`movedir` was implemented due to a bug in the core NAnt tasks when moving directories (https://github.com/nant/nant/issues/11). It takes
two parameters, `from` and `to` which are pretty self explanatory.

Example

	<movedir from="/path/of/original/directory" to="/path/of/new/directory" />

#### restore-db

`restore-db` uses a database backup (.bak) to restore it to a SQL Server instance

* `replace` - *Boolean* - Set true to overwrite an existing database on the server with the backup
* `db-name` - *String*, **Required** - Name of the database to restore to
* `backup-path` - *String*, **Required** - Location of the database backup to use
* `server` - *String* - Server instance to use

Example

	<restore-db replace="true" db-name="SuperAwesomeDatabase" backup-path="/path/to/the/backup.bak" />

#### attach-db

`attach-db` attaches an existing MDF file to a server instance. It will move the MDF/LDF files to your SQL Data directory automatically.

* `replace` - *Boolean* - Set true to overwrite an existing database on the server with the backup
* `db-name` - *String*, **Required** - Name of the database to restore to
* `mdf-path` - *String*, **Required** - Location of the MDF file
* `ldf-path` - *String*, **Required** - Location of the LDF file
* `owner` - *String* - Owner to assign to attached database
* `server` - *String* - Server instance to use

Example

	<attach-db replace="false" db-name="CoolDatabase" mdf-path="/path/to/db.mdf" ldf-path="/path/to/db.ldf" owner="superuser" />

#### db-assign-user

`db-assign-user` essentially executes the stored procedure `sp_changedbowner` with the specified owner parameter

* `db-name` - *String*, **Required** - Name of the database to assign user to
* `db-user` - *String*, **Required** - Username to assign to database
* `server` - *String* - Server to connect to, defaults to localhost

Example

	<db-assign-user db-name="YourDatabase" db-user"superuser" />
	
#### delete-db

`delete-db` will remove a database from the specified server

* `db-name` - *String*, **Required** - Name of the database to delete
* `server` - *String* - Server to connect to, defaults to localhost

#### open-url

`open-url` is a utiltity task that launches the specified URL

* `url` - *String*, **Required** - Url to open

Example

	<open-url url="http://google.com" />