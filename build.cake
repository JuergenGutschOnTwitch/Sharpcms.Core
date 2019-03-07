using System.Linq;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var branchname = Argument("branchname", "master");
var environment = branchname == "master" ? "Production" : "Development";

var buildnumber = Argument("buildnumber", "0");
var publishfolder = Argument("publishfolder", "./published/");

var build =  environment != "Production" ? String.Format("-build{0}", buildnumber) : "";
var version = "0.0.1";
var artifactName = $"Sharpcms.Core-{version}{build}.zip";

Task("CleanDirectory")
	.Does(() =>
	{
		CleanDirectory("./published/");
	});

Task("DotNetRestore")
	.IsDependentOn("CleanDirectory")
	.Does(() => {
		DotNetCoreRestore("./Sharpcms.Core.sln");
	});

Task("DotNetBuild")
	.IsDependentOn("DotNetRestore")
	.Does(() => 
	{	
		var settings = new DotNetCoreBuildSettings
		{
			Configuration = configuration,
			NoRestore = true
		};
		DotNetCoreBuild("./Sharpcms.Core.sln", settings);
	});

	
/*Task("DotNetTest")
	.IsDependentOn("DotNetBuild")
	.Does(() => 
	{
		var settings = new DotNetCoreTestSettings
		{
			Configuration = configuration,
			NoBuild = true
		};
		DotNetCoreTest("./Sharpcms.Core.Tests/", settings);
	});*/

Task("DotNetPublish")
	.IsDependentOn("DotNetBuild")
	.Does(() => 
	{	
		var settings = new DotNetCorePublishSettings
		{
			Configuration = configuration,
			OutputDirectory = "./published/",
			ArgumentCustomization = args => args
                .Append($"/p:Version={version}")
                .Append($"/p:FileVersion={version}")
                .Append($"/p:Assembly={version}")
                .Append($"/p:InformationalVersion={version}{build}"),
			NoRestore = true
		};
		DotNetCorePublish("./Sharpcms.Core/", settings);
	});

Task("PackApplication")
	.IsDependentOn("DotNetPublish")
	.Does(() => 
	{
		var target = $"./published/{artifactName}";
		Zip("./published/", target);
		Information($"Packed to {target}.");
	});

Task("PublishArtifact")
	.IsDependentOn("PackApplication")
	.Does(() => 
	{
		var target = $"./published/{artifactName}";
		CopyFile(target, System.IO.Path.Combine(publishfolder, artifactName));
		Information($"Published to {target}.");
	});

Task("Default")
	.IsDependentOn("PublishArtifact")
	.Does(() =>
	{
		Information($"You build of version '{version}{build}' is done!");
	});
	
RunTarget(target);