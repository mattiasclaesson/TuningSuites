call T5SuiteII\version.bat
devenv T5SuiteII.sln /Rebuild Release /project T5SuiteII

pushd T5SuiteII\Release\
"C:\Program Files (x86)\hashutils-1.3.0-redist\bin.x86-32\md5sum.exe" T5SuiteII.msi >> T5SuiteII.md5
popd

mkdir C:\users\mattias\Dropbox\public\T5Suite2\%T5SuiteII.version%
xcopy T5SuiteII\version.bat C:\users\mattias\Dropbox\public\T5Suite2\%T5SuiteII.version%\
xcopy T5SuiteII\Release\T5SuiteII.msi C:\users\mattias\Dropbox\public\T5Suite2\%T5SuiteII.version%\
xcopy T5SuiteII\Release\T5SuiteII.md5 C:\users\mattias\Dropbox\public\T5Suite2\%T5SuiteII.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > C:\users\mattias\Dropbox\public\T5Suite2\version.xml
echo ^<t5suite2 version="%T5SuiteII.version%"/^> >> C:\users\mattias\Dropbox\public\T5Suite2\version.xml
git tag T5SuiteII_v%T5SuiteII.version%