Dim Args
Set Args = WScript.Arguments

If Args.Count = 3 Then
    Dim ProgramFiles, IISExpress, LocalDB, SqlInstance, Site, URL   

    'Load arguments
    SqlInstance = Args(0)
    Site = Args(1)
    URL = Args(2)
	  
	'Init system objects
    Set WshShell = CreateObject("WScript.Shell")
    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    
    'Init paths to assemblies. Program files system variable must be expanded before calling any method with IISExpress/LocalDB as an argument.
    ProgramFiles = WshShell.ExpandEnvironmentStrings("%PROGRAMFILES%")
    IISExpress = ProgramFiles & "\IIS Express\iisexpress.exe"
    LocalDB = "C:\Program Files\Microsoft SQL Server\130\Tools\Binn\SqlLocalDB.exe"

    'Start LocalDB using cmd.exe
    If Len(SqlInstance) > 1 And FileSystem.FileExists(LocalDB) Then
        WshShell.Run "%COMSPEC% /C """ & LocalDB & """ start " & SqlInstance, 0
    End If

    'Start IISExpress using cmd.exe
    If Len(Site) > 1 And FileSystem.FileExists(IISExpress) Then
        WshShell.Run "%COMSPEC% /C """ & IISExpress & """ /site:" & Site, 0
    End If

    'Open URL in browser
    If Len(URL) > 1 Then
        WshShell.Run URL, 0
    End If

    Set WshShell = Nothing
End If
