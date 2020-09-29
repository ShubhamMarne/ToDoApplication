# ToDoApplication
Application used to manage TODO tasks and send notification email as reminder to users.

# Requirement : 
1. Desktop Utility for managing TODO tasks.
2. Windows service for sending notification email for TODO tasks.

1. TODODesktopUtlity : 
Used to create, update TODO task as per user.
2. TODOHelperLibrary : 
Contains generic code used by desktop utlity and windows service.
3. Windows Service :
Used to send notification email for TODO task as per notification time provided by user.

# Prerequisites : 
Execute database script present at \..\Database_Script\todo.sql

# Steps to configure Windows Service :
1. NOTE : Build Solution in Release mode.
Open WindowsMailService.exe.config file located at \..\TODODesktopUtility\WindowsMailService\bin\Release
	a. Add Username and Password at line #7
		e.g. <add key="connectionString" value="Server=localhost;Database=todo;Uid=root;Pwd=root;" />
	b. Add From email at line #8
		e.g. <add key="FromMail" value="test@gmail.com"/>
	c. Add Password for From email at line #9
		e.g. <add key="Password" value="PASSWORD@123"/>
	d. Update host if want to change. Recommended to use smtp.gmail.com
		e.g <add key="Host" value="smtp.gmail.com"/>
2. 
Install WindowsMailService windows service locally.
	a. Goto following path \..\TODODesktopUtility\WindowsMailService\bin\Release and execute command : 
		Open Command Prompt as Administrator.
		WindowsMailService.exe install
3.
Start WindowsMailService windows service using following command : 
	WindowsMailService.exe start
4.
Stop WindowsMailService windows service using following command : 
	WindowsMailService.exe stop
5.
Uninstall/Remove WindowsMailService windows service using following command : 
	WindowsMailService.exe uninstall

# Steps to configure Desktop Utility :
1. NOTE : Build Solution in Release mode.
Open TODODesktopUtility.exe.config file located in \..\TODODesktopUtility\TODODesktopUtility\bin\Release\
	a. Add Username and Password at line #10
		e.g. <add key="connectionString" value="Server=localhost;Database=todo;Uid=root;Pwd=root;" />
2.
Execute TODODesktopUtility.exe file located at \..\TODODesktopUtility\TODODesktopUtility\bin\Release\
	a. Create new user and login into the application to create TODO Task.
