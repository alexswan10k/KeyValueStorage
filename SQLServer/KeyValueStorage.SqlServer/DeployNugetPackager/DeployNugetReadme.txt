DeployNuget is a package designed to help you quickly build proprietary packages to be deployed to a local or remote file system.
In order to use packages from DeployNuget simply add the base directory of the nuget packages folder to visual studio. 

There are a number of components requried for DeployNuget to operate:

(--NOTE -- you MUST compile your project in release mode before upgrading binaries referenced in the nuspec file --)
--DeployLocal.bat - Deploys your nuget package to the local directory with a single click
--DeployServer.bat - Deploys your nuget package to the remote repository with a single click

--.nuget\Nuget.exe and appropriate configuration

--DeployNuget.ps1 - The library script. Do not execute this directly (you will need to Set-ExecutionPolicy RemoteSigned)

--DeployNuget.Config - All global variables for DeployNuget are stored here.
YOU WILL NEED TO CREATE THIS FILE AFTER INSTALLING THIS PACKAGE FOR THE FIRST TIME
------------Example configuration-----------
<?xml version="1.0" encoding="utf-8"?>
<!-- Rename this file to DeployNuget.config and set your required properties. These will be picked up by the DeployNuget powershell script -->
<DeployNuget>
	<TargetNugetShareBaseDirectory>\\MyServer\NugetPackages</TargetNugetShareBaseDirectory>
	<TargetNugetLocalBaseDirectory>c:\NugetPackages</TargetNugetLocalBaseDirectory>
</DeployNuget>
--------------------------------------------

--SomeConfig.nuspec - This contains all target data for the deployment. A list of files to deploy can be set here specifically.

Your nuspec file is where you will set the package name along with version and other details.
Note - Incrementing the version will automatically show as an update for all clients which are referencing the package.
YOU WILL NEED TO CREATE ONE OR MORE INSTANCES OF THIS FILE AFTER INSTALLING THIS PACKAGE FOR THE FIRST TIME
------------Example configuration-----------
<?xml version="1.0" encoding="utf-8" ?>
<!-- Rename this to myPackageName.nuspec. All .nuspec files will be iterated over by the deploy nuget script allowing a single project to produce multiple output packages-->
<package xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <metadata xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
    <id>Business.Fake</id>
    <version>0.0.0.1</version>
    <title>YourProjectTitleHere</title>
    <authors>Metal10k</authors>
    <summary></summary>
    <description>Your description here</description>
    <language>en-us</language>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <dependencies>
	<!-- Define any package dependencies here -->
    </dependencies>
  </metadata>
  <files>
 		<file src="..\bin\Release\MyBinary.dll" target="lib\net40\MyBinary.dll" />
		<file src="..\bin\Release\MyBinary.xml" target="lib\net40\MyBinary.xml" />
		<file src="..\SomeArbitraryContent.xml" target="\Content\SomeArbitraryContent.xml" />
  </files>
</package>
--------------------------------------------

IncrementVersion.ps1 - Increments the version of your nuspec file by x.x.x.x+1 so updates will be noticed by your client. Note - this file may be read-only if it is not checked out


DeployNuget itself is deployed via DeployNuget!
