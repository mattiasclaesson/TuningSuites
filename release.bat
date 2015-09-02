call SetupT7Suite\version.bat
devenv T7.sln /Rebuild Release /project SetupT7Suite

pushd SetupT7Suite\Release\
"C:\Program Files (x86)\hashutils-1.3.0-redist\bin.x86-32\md5sum.exe" T7Suite.msi >> T7Suite.md5
"C:\Program Files\7-Zip\7z.exe" a -tzip T7Suite.zip T7Suite.* setup.exe
popd

mkdir C:\users\mattias\Dropbox\public\T7Suite\%T7.version%
xcopy SetupT7Suite\version.bat C:\users\mattias\Dropbox\public\T7Suite\%T7.version%\
xcopy SetupT7Suite\Release\T7Suite.msi C:\users\mattias\Dropbox\public\T7Suite\%T7.version%\
xcopy SetupT7Suite\Release\T7Suite.md5 C:\users\mattias\Dropbox\public\T7Suite\%T7.version%\
xcopy SetupT7Suite\Release\T7Suite.zip C:\users\mattias\Dropbox\public\T7Suite\%T7.version%\
xcopy SetupT7Suite\Release\setup.exe C:\users\mattias\Dropbox\public\T7Suite\%T7.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > C:\users\mattias\Dropbox\public\T7Suite\version.xml
echo ^<t7suitepro version="%T7.version%"/^> >> C:\users\mattias\Dropbox\public\T7Suite\version.xml

echo ----------------------------------------------------
git changes
echo ----------------------------------------------------

git tag T7suite_v%T7.version%
git tag SetupT7suite_v%SetupT7Suite.version%