@echo off
dotnet build src/Skybrud.Umbraco.Feedback --configuration Release /t:rebuild /t:pack -p:Configuration=Release -p:BuildTools=1 -p:PackageOutputPath=../../releases/nuget