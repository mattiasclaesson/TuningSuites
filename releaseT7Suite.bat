call SetupT7Suite\version.bat
devenv T7.sln /Rebuild Release /project SetupT7Suite

pushd SetupT7Suite\Release\
"C:\md5sum.exe" T7Suite.msi >> T7Suite.md5
"C:\Program Files\7-Zip\7z.exe" a -tzip T7Suite.zip T7Suite.* setup.exe
popd

mkdir z:\T7Suite\%T7.version%
xcopy SetupT7Suite\version.bat z:\T7Suite\%T7.version%\
xcopy SetupT7Suite\Release\T7Suite.msi z:\T7Suite\%T7.version%\
xcopy SetupT7Suite\Release\T7Suite.md5 z:\T7Suite\%T7.version%\
xcopy SetupT7Suite\Release\T7Suite.zip z:\T7Suite\%T7.version%\
xcopy SetupT7Suite\Release\setup.exe z:\T7Suite\%T7.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > z:\T7Suite\version.xml
echo ^<t7suitepro version="%T7.version%"/^> >> z:\T7Suite\version.xml

echo ----------------------------------------------------
git changes
echo ----------------------------------------------------

git tag T7suite_v%T7.version%
git tag SetupT7suite_v%SetupT7Suite.version%