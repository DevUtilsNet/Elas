param($installPath, $toolsPath, $package, $project)
 
  Write-Host "Execute Uninstall.ps1"

  # Need to load MSBuild assembly if it's not loaded yet.
  Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

  # Grab the loaded MSBuild project for the project
  $buildProject = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

  if (!$buildProject)
  {
    Write-Host "Manual Load"
    $manualLoad = $TRUE
    $project.Save()
    $buildProject = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.LoadProject($project.FullName)
  }

  # Find all the imports and targets added by this package.
  $itemsToRemove = @()

  # Allow many in case a past package was incorrectly uninstalled
  $itemsToRemove += $buildProject.Xml.Imports | Where-Object { $_.Project.EndsWith('ElasConfiguration.props') }
  
  # Remove the elements and save the project
  if ($itemsToRemove -and $itemsToRemove.length)
  {
    foreach ($itemToRemove in $itemsToRemove)
    {
        Write-Host "Remove import $($itemToRemove.Project)"
        $itemToRemove.Parent.RemoveChild($itemToRemove) | out-null
    }
     
    if($manualLoad)
    {
      $buildProject.Save()
      [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.UnloadProject($buildProject)
    }
    else
    {
      $project.Save()
    }
  }