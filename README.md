
Todo - CL
=========

*A command line tool to quickly add tasks to Toodledo*

Basic Installation and Usage
----------------------------

1. Download and unzip the files to a location in your PATH
2. Execute todo.exe with the -u and -p options 
to set your toodledo user id and password
	
		todo.exe -u myuserid -p mypassword
	
	_NOTE: This is not your username, 
	but the unique id located on your 
	settings page in toodledo_

3. Execute todo.exe with a task name to add to your inbox

		todo.exe this is the task I want to add

4. Bonus: Run it with launchy

Additional Options
------------------

+ -t --tags _comma delimited list of tags_
	
		todo.exe a task with tags -t work,play,tag3

+ -f --folder _folder to insert the task into_

		todo.exe a task with a folder -f Inbox

+ -c --context _context to use_

		todo.exe a task with a context -c Home

+ -l --length _length of the task in minutes_

		todo.exe a task with a length -l 20

+ -s --set _set a default property_
	
	_Format =  PROPERTY:VALUE (ex: `folder:Actions`)_

		todo.exe set default folder -s folder:Inbox
		todo.ext set default context -s context:Home

	_If set all new tasks will go to the default folder or context_

+ -h --help _display this help screen_
 

Compiling From Source
-----------------------------------

This little project works with the Toodledo API Version 1.0. 
I will update it to work with 2.0 when Toodledo no longer supports 1.0.

This project also requires a few projects that are not provided:

[ToodleDo API Client](http://archive.msdn.microsoft.com/toodledo)

[.NET CommandLineParser](http://commandline.codeplex.com/)
