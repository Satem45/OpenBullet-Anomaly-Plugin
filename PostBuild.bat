set mypath=%cd%
md %mypath%\PureLib\bin\Release\netstandard2.0\Anomaly
move %mypath%\PureLib\bin\Release\netstandard2.0\*.* %mypath%\PureLib\bin\Release\netstandard2.0\Anomaly
move %mypath%\PureLib\bin\Release\netstandard2.0\Anomaly\Anomaly.dll %mypath%\PureLib\bin\Release\netstandard2.0
exit