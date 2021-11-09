$QSchedulerBase="C:\\Users\\homan\\Documents\\repo\\Murr\\Modules\\Murzik.SchedulerService\\bin\\Debug\\net5.0\\*"
$QScheduler = "C:\\Users\\homan\\Documents\\repo\\Murr\\PromRunningServices\\Scheduler"
$QSchedulerSettings = "C:\\Users\\homan\\Documents\\repo\\Murr\\PowerShellScripts\\PromSettings\\Scheduler\\*"
dotnet build C:\Users\homan\Documents\repo\Murr\Solutions\FullSolutions.sln
Stop-Service -Name "ProdScheduler"
Remove-Item -Path $QSchedulerBase -include *.pdb -Recurse
Get-ChildItem -Path $QScheduler -Recurse| Foreach-object {Remove-Item -Recurse -Path $_.FullName}
Copy-Item -Force -Recurse $QSchedulerBase -Destination $QScheduler
Copy-Item -Force -Recurse $QSchedulerSettings -Destination $QScheduler
Start-Service -Name "ProdScheduler"