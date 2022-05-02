New-Service -Name "HangBushuev" -BinaryPathName C:\Users\homan\source\repos\WindowsService1\WindowsService1\bin\Debug\WindowsService1.exe
#удаление
Stop-Service 'PromScheduler'; Get-CimInstance -ClassName Win32_Service -Filter "Name='PromScheduler'" | Remove-CimInstance

powershell -executionpolicy bypass -File .\re_run_test_algorithm_service.ps1

New-Service -Name "TestAlgorithm" -BinaryPathName D:\repo\Murr\TestRunningServices\Algorithm\Murzik.AlgorithmService.exe
New-Service -Name "TestScheduler" -BinaryPathName D:\repo\Murr\TestRunningServices\Scheduler\Murzik.SchedulerService.exe

New-Service -Name "HangBushuev" -BinaryPathName C:\Users\homan\source\repos\WindowsService1\WindowsService1\bin\Debug\WindowsService1.exe
