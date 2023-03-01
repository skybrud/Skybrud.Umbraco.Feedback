@echo off
dotnet build src/Skybrud.Umbraco.Feedback --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget