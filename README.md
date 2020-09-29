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
NOTE : Build Solution in Release mode.
1.
Open WindowsMailService.exe.config file located at \..\TODODesktopUtility\WindowsMailService\bin\Release
a. Add Username and Password at line #7
e.g. <add key="connectionString" value="Server=localhost;Database=todo;Uid=root;Pwd=root;" />

