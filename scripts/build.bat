@echo off
chcp 65001

set current_dir=%~dp0
set sln_path=%current_dir%..\src\JimJiangApp\JimJiangApp.sln
set proj_path=%current_dir%..\src\JimJiangApp\JimJiangApp\JimJiangApp.csproj
set ui_output_dir=%current_dir%..\output
msbuild -restore %sln_path% /p:Configuration=Release /p:Platform="x64"
msbuild %proj_path% /p:Configuration=Release /p:Platform="x64" /p:OutputPath=%ui_output_dir% /p:DefineConstants=%app_type% /p:DefineConstants=AcerAssist /p:GenerateAppxPackageOnBuild=true