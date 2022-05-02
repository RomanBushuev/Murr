$QSchedulerBase="D:\\repo\\Murr\\Modules\\Murzik.SchedulerService\\bin\\Debug\\net5.0\\*"
$QScheduler = "D:\\repo\\Murr\\TestRunningServices\\Scheduler"
$QSchedulerSettings = "D:\\repo\\Murr\\PowerShellScripts\\TestSettings\\Scheduler\\*"
dotnet build D:\repo\Murr\Solutions\FullSolutions.sln
Stop-Service -Name "TestScheduler"
Remove-Item -Path $QSchedulerBase -include *.pdb -Recurse
Get-ChildItem -Path $QScheduler -Recurse| Foreach-object {Remove-Item -Recurse -Path $_.FullName}
Copy-Item -Force -Recurse $QSchedulerBase -Destination $QScheduler
Copy-Item -Force -Recurse $QSchedulerSettings -Destination $QScheduler
Start-Service -Name "TestScheduler"