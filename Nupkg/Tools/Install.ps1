param($installPath, $toolsPath, $package, $project)

  function AsMSBuildProject($prj)
  {
    $ret = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($prj.FullName) | Select-Object -First 1

    if(!$ret)
    {
      $ret = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.LoadProject($prj.FullName)
    }
    return $ret
  }

  function AddImportStatement($prj, $targetsPath, $location)
  {
    $prj=AsMSBuildProject($prj)

		Write-Host "`$targetsPath $targetsPath"
    Write-Host "`$location $location"

    [NuGet.MSBuildProjectUtility]::AddImportStatement($prj, $targetsPath, $location)
  }

  Write-Host "Execute Install.ps1"

  $rootDir = (Get-Item $installPath).parent.parent.fullname
  $elasConfiguration = "$rootDir\.Elas\ElasConfiguration.props"
	
  # This is the MSBuild targets file to add
  $targetsFile = [System.IO.Path]::Combine($rootDir, $elasConfiguration)

  # Need to load MSBuild assembly if it's not loaded yet.
  # Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

  # Grab the loaded MSBuild project for the project
  # $buildProject = $project #[Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1


  # if (!$buildProject)
  # {
    # Write-Host "Manual Load"
    # $manualLoad = $TRUE
    # $project.Save()
    # $buildProject = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.LoadProject($project.FullName)
    # $buildProject = $project
  # }

  # Make the path to the targets file relative.
  $projectUri = new-object Uri($project.FullName, [System.UriKind]::Absolute)
  $targetUri = new-object Uri($targetsFile, [System.UriKind]::Absolute)
  $relativePath = [System.Uri]::UnescapeDataString($projectUri.MakeRelativeUri($targetUri).ToString()).Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

	Write-Host "`$relativePath $relativePath"
  AddImportStatement $project $relativePath "Bottom"
  #Write-Host "`$package.GetType() $($package.GetType())"
  #$project.AddImport($relativePath, [NuGet.ProjectImportLocation]::Bottom)
  # Add the import with a condition, to allow the project to load without the targets present.
  # $import = $buildProject.Xml.AddImport($relativePath)
  # $import.Condition = "Exists('$relativePath')"


  # if($manualLoad)
  # {
    # $buildProject.Save()
    # [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.UnloadProject($buildProject)
  # }
  # else
  # {
    # $project.Save()
  # }