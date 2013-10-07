@echo off
setlocal
set msbuildemitsolution=1
%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe build.proj /m:1 /v:m /flp:verbosity=diagnostic /p:IsDesktopBuild=true;SolutionRoot=..\solutions;OutDir=%temp%\TfsWorkbench.output\;FinalOutput=..\installers
pause