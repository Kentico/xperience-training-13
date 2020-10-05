You have installed or updated the module's files in the Kentico Xperience project. If you are using a web application project, you need to build the Kentico Xperience solution.

To finish the installation or update, open your Kentico Xperience application in a browser. During the processing of the first request after the module installation or 
update, the system automatically imports database objects from the module installation package to the Kentico Xperience database.

You can verify that the module was installed or updated successfully in the Event log application - check that the log contains no errors and the following event:

Source = ModuleInstaller, Event code = MODULEINSTALLED
 -or-
Source = ModuleInstaller, Event code = MODULEUPDATED