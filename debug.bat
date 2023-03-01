@echo off
dotnet build src/Skybrud.Umbraco.Feedback --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=c:/nuget