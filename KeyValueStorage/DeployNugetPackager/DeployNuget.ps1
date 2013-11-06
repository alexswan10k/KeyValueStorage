param([String]$TargetDirectoryConfig)

try
{
	$thisFolderDI = (New-Object system.IO.DirectoryInfo $PWD)

	foreach($thisFolderNuSpecFile in $thisFolderDI.GetFiles("*.nuspec"))
	{
		[Reflection.Assembly]::LoadWithPartialName("System.Xml.Linq")
		$xDocNuspecFile = [System.Xml.Linq.XDocument]::Load($thisFolderNuSpecFile.FullName)



		$xDocDeployConfigFile = [System.Xml.Linq.XDocument]::Load($thisFolderDI.FullName+"\"+ "DeployNuget.config")

		foreach($item in $xDocNuspecFile.Descendants())
		{
			if($item.Name.LocalName -eq "title")
			{
				$packageName = $item.Value
			}	
			if($item.Name.LocalName -eq "version")
			{
				$version = $item.Value
			}
		}

		#Set our props from config
		foreach($item in $xDocDeployConfigFile.Descendants())
		{
			if($item.Name.LocalName -eq $TargetDirectoryConfig)
			{
				$targetDir = $item.Value
			}
		}


		$targetDI = New-Object system.IO.DirectoryInfo $targetDir


		#iterate parents to find the nuget folder in the solution
		$tempIttDir = (New-Object system.IO.DirectoryInfo $thisFolderDI.FullName)
		$loopCount = 0
		while($tempIttDir.GetDirectories(".nuget").Length -eq 0)
		{
			if($loopCount -ge 10)
			{
				throw New-Object [System.Exception] ".nuget exe not found"
			}
		
			$tempIttDir = $tempIttDir.Parent

			$loopCount= $loopCount+1
		}

		$nugetExeDir = $tempIttDir.GetDirectories(".nuget")[0].FullName + "\NuGet.exe"



		$thisFolderNuSpecFileName = $thisFolderNuSpecFile.Name
		$nugetStagingDir = $thisFolderDI.Parent.FullName + "\NugetPackageStaging"

		$packageStagingDI = New-Object System.IO.DirectoryInfo($nugetStagingDir)
		if($packageStagingDI.Exists)
		{
			Remove-Item -Recurse -Force $packageStagingDI.FullName
		}

		$packageStagingDI.Create()


		#$packageStagingLibNet4DI = New-Object System.IO.DirectoryInfo($nugetStagingDir + "\lib\net40\")
		#if(-not $packageStagingLibNet4DI.Exists)
		#{
		#	$packageStagingLibNet4DI.Create()
		#}

		#Copy the nuspec file
		$thisFolderNuSpecFile.CopyTo($packageStagingDI.FullName+ "\" + $thisFolderNuSpecFile.Name, 1)

		#Create the nupkg file
		& $nugetExeDir pack $thisFolderNuSpecFileName -OutputDirectory $packageStagingDI -Verbose -Build
		#& $nugetExeDir push C:\NugetPackages\

		#Copy the binaries we are interested in
		foreach($item in $xDocNuspecFile.Descendants("file"))
		{
			$itemSrc = New-Object System.IO.FileInfo (Resolve-Path $item.Attribute("src").Value)	
			$itemTarPath = $nugetStagingDir + "\" + $item.Attribute("target").Value
			
			$itemTarFileInfo = New-Object System.IO.FileInfo $itemTarPath
			if(-not $itemTarFileInfo.Directory.Exists)
			{
				$itemTarFileInfo.Directory.Create()
			}
			
			$itemSrc.CopyTo($itemTarPath, 1)
		}

		#directoryinfo does not have an easy copy method! hack...
		$roboSrc = $packageStagingDI.FullName
		$roboTgt = $targetDir +"\"+ $packageName + "_" + $version

		robocopy $roboSrc $roboTgt /MIR
	}
}
catch [Exception]
{
	throw $_.Exception
}
