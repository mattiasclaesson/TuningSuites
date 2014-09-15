call SetupT7Suite\version.bat
devenv T7.sln /Rebuild Release /project SetupT7Suite
mkdir C:\users\mattias\Dropbox\public\T7Suite\%T7.version%
xcopy SetupT7Suite\version.bat C:\users\mattias\Dropbox\public\T7Suite\%T7.version%\
xcopy SetupT7Suite\Release\T7Suite.msi C:\users\mattias\Dropbox\public\T7Suite\%T7.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > C:\users\mattias\Dropbox\public\T7Suite\version.xml
echo ^<t7suitepro version="%T7.version%"/^> >> C:\users\mattias\Dropbox\public\T7Suite\version.xml

git tag T7suite_v%T7.version%
git tag SetupT7suite_v%SetupT7Suite.version%