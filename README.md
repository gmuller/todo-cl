Todo - CL
=========

A command line tool to quickly add tasks to Toodledo
---------------------------------------------------------------

### Installation and Usage ###
1. Download and unzip to a location in your PATH
2. Execute todo.exe with the -u and -p options to set your toodledo user id and password

    todo.exe -u myuserid -p mypassword

	*NOTE: This is not your username, but the unique id located on your settings page in toodledo*

3. Execute todo.exe with a task name to add to your inbox

	todo.exe this is the task I want to add

4. Bonus: Run it with launchy!

### Compiling From Source ###
This little project works with the Toodledo API Version 1.0. I will update it to work with 2.0 when 
Toodledo no longer supports 1.0.

This project also requires a few projects that are not provided:
ToodleDo API Client - http://archive.msdn.microsoft.com/toodledo
.NET CommandLineParser - http://commandline.codeplex.com/
