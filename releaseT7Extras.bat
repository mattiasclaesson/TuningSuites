call SetupT7Extras\version.bat
devenv T7.sln /Rebuild Release /project SetupT7Extras

pushd SetupT7Extras\Release\
"C:\md5sum.exe" T7Extras.msi >> T7Extras.md5
popd

mkdir z:\T7Extras\%T7Extras.version%
xcopy SetupT7Extras\version.bat z:\T7Extras\%T7Extras.version%\
xcopy SetupT7Extras\Release\T7Extras.msi z:\T7Extras\%T7Extras.version%\
xcopy SetupT7Extras\Release\T7Extras.md5 z:\T7Extras\%T7Extras.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > z:\T7Extras\version.xml
echo ^<t7extras version="%T7Extras.version%"/^> >> z:\T7Extras\version.xml

git tag T7Extras_v%T7Extras.version%