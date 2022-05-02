$QAlgorithmBase="D:\\repo\\Murr\\Modules\\Murzik.AlgorithmService\\bin\\Debug\\net5.0\\*"
$QAlgorithm = "D:\\repo\\Murr\\TestRunningServices\\Algorithm"
$QAlgorithmSettings = "D:\\repo\\Murr\\PowerShellScripts\\TestSettings\\Algorithm\\*"
dotnet build D:\repo\Murr\Solutions\FullSolutions.sln
Stop-Service -Name "TestAlgorithm"
Remove-Item -Path $QAlgorithmBase -include *.pdb -Recurse
Get-ChildItem -Path $QAlgorithm -Recurse| Foreach-object {Remove-Item -Recurse -Path $_.FullName}
Copy-Item -Force -Recurse $QAlgorithmBase -Destination $QAlgorithm
Copy-Item -Force -Recurse $QAlgorithmSettings -Destination $QAlgorithm
Start-Service -Name "TestAlgorithm"