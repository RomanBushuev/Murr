$QAlgorithmBase="C:\\Users\\homan\\Documents\\repo\\Murr\\Modules\\Murzik.AlgorithmService\\bin\\Debug\\net5.0\\*"
$QAlgorithm = "C:\\Users\\homan\\Documents\\repo\\Murr\\TestRunningServices\\Algorithm"
$QAlgorithmSettings = "C:\\Users\\homan\\Documents\\repo\\Murr\\PowerShellScripts\\TestSettings\\Algorithm\\*"
dotnet build C:\Users\homan\Documents\repo\Murr\Solutions\FullSolutions.sln
Stop-Service -Name "TestAlgorithm"
Remove-Item -Path $QAlgorithmBase -include *.pdb -Recurse
Get-ChildItem -Path $QAlgorithm -Recurse| Foreach-object {Remove-Item -Recurse -Path $_.FullName}
Copy-Item -Force -Recurse $QAlgorithmBase -Destination $QAlgorithm
Copy-Item -Force -Recurse $QAlgorithmSettings -Destination $QAlgorithm
Start-Service -Name "TestAlgorithm"