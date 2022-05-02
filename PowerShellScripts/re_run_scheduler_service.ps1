$QSchedulerBase="D:\\repo\\Murr\\Modules\\Murzik.SchedulerService\\bin\\Debug\\net5.0\\*"
$QScheduler = "D:\\repo\\Murr\\PromRunningServices\\Scheduler"
$QSchedulerSettings = "D:\\repo\\Murr\\PowerShellScripts\\PromSettings\\Scheduler\\*"
dotnet build D:\repo\Murr\Solutions\FullSolutions.sln
Stop-Service -Name "PromScheduler"
Remove-Item -Path $QSchedulerBase -include *.pdb -Recurse
Get-ChildItem -Path $QScheduler -Recurse| Foreach-object {Remove-Item -Recurse -Path $_.FullName}
Copy-Item -Force -Recurse $QSchedulerBase -Destination $QScheduler
Copy-Item -Force -Recurse $QSchedulerSettings -Destination $QScheduler
Start-Service -Name "PromScheduler"