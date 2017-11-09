call T5SuiteII\version.bat
devenv T5SuiteII.sln /Rebuild Release /project T5SuiteII

pushd T5SuiteII\Release\
"C:\md5sum.exe" T5SuiteII.msi >> T5SuiteII.md5
popd

mkdir z:\T5Suite2\%T5SuiteII.version%
xcopy T5SuiteII\version.bat z:\T5Suite2\%T5SuiteII.version%\
xcopy T5SuiteII\Release\T5SuiteII.msi z:\T5Suite2\%T5SuiteII.version%\
xcopy T5SuiteII\Release\T5SuiteII.md5 z:\T5Suite2\%T5SuiteII.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > z:\T5Suite2\version.xml
echo ^<t5suite2 version="%T5SuiteII.version%"/^> >> z:\T5Suite2\version.xml

echo ----------------------------------------------------
git changes
echo ----------------------------------------------------

git tag T5SuiteII_v%T5SuiteII.version%
git tag Owf.Controls.DigitalDisplayControl_v%Owf.Controls.DigitalDisplayControl.version%
git tag SuiteLauncher_v%SuiteLauncher.version%