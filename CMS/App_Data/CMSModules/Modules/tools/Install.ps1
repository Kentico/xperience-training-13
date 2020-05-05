param($installPath, $toolsPath, $package, $project)
# Set build action of all module resx files to "Content"
$project.ProjectItems | where-object {$_.Name -eq "CMSResources"} | 
    foreach-object { $_.ProjectItems } | where-object {$_.Name -eq $package.id} | 
    foreach-object { $_.ProjectItems } | where-object {$_.Name -like "*.resx"} | 
    foreach-object {$_.Properties} | where-object {$_.Name -eq "BuildAction"} | foreach-object {$_.Value = [int]2}