$QSchedulerBase="C:\\Users\\homan\\Documents\\repo\\Murr\\Modules\\Murzik.SchedulerService\\bin\\Debug\\net5.0\\*"
$QScheduler = "C:\\Users\\homan\\Documents\\repo\\Murr\\TestRunningServices\\Scheduler"
$QSchedulerSettings = "C:\\Users\\homan\\Documents\\repo\\Murr\\PowerShellScripts\\TestSettings\\Scheduler\\*"
Stop-Service -Name "TestScheduler"
Start-Sleep -s 10
Remove-Item -Path $QSchedulerBase -include *.pdb -Recurse
Get-ChildItem -Path $QScheduler -Recurse| Foreach-object {Remove-Item -Recurse -Path $_.FullName}
Copy-Item -Force -Recurse $QSchedulerBase -Destination $QScheduler
Copy-Item -Force -Recurse $QSchedulerSettings -Destination $QScheduler
Start-Sleep -s 10
Start-Service -Name "TestScheduler"