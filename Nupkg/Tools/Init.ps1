param($installPath, $toolsPath, $package)

$rootDir = (Get-Item $installPath).parent.parent.fullname
$deployTarget = "$rootDir\.Elas\"

$deploySource = join-path $installPath 'Tools/Elas'

if(!(test-path "$deployTarget\ElasConfiguration.props"))
{
	if (!(test-path $deployTarget)) {
		mkdir $deployTarget
	}

	# copy everything in there

	Copy-Item "$deploySource/ElasConfiguration.props" $deployTarget -Recurse -Force
}

# get the active solution
$solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])

# create a deploy solution folder if it doesn't exist

$deployFolder = $solution.Projects | where-object { $_.ProjectName -eq ".elas" } | select -first 1

if(!$deployFolder) {
	$deployFolder = $solution.AddSolutionFolder(".elas")
}

# add all our support deploy scripts to our Support solution folder

$folderItems = Get-Interface $deployFolder.ProjectItems ([EnvDTE.ProjectItems])

ls $deployTarget | foreach-object { 
	$folderItems.AddFromFile($_.FullName) > $null
} > $null