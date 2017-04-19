call SetupT8Extras\version.bat
devenv T8.sln /Rebuild Release /project SetupT8Extras

pushd SetupT8Extras\Release\
"C:\Program Files (x86)\hashutils-1.3.0-redist\bin.x86-32\md5sum.exe" T8Extras.msi >> T8Extras.md5
popd

mkdir C:\Users\mattias\Delivery\T8Extras\%T8Extras.version%
xcopy SetupT8Extras\version.bat C:\Users\mattias\Delivery\T8Extras\%T8Extras.version%\
xcopy SetupT8Extras\Release\T8Extras.msi C:\Users\mattias\Delivery\T8Extras\%T8Extras.version%\
xcopy SetupT8Extras\Release\T8Extras.md5 C:\Users\mattias\Delivery\T8Extras\%T8Extras.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > C:\Users\mattias\Delivery\T8Extras\version.xml
echo ^<t8extras version="%T8Extras.version%"/^> >> C:\Users\mattias\Delivery\T8Extras\version.xml

git tag T8Extras_v%T8Extras.version%