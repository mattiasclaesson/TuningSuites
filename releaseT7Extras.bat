call SetupT7Extras\version.bat
devenv T7.sln /Rebuild Release /project SetupT7Extras

pushd SetupT7Extras\Release\
"C:\Program Files (x86)\hashutils-1.3.0-redist\bin.x86-32\md5sum.exe" T7Extras.msi >> T7Extras.md5
popd

mkdir C:\Users\mattias\Delivery\T7Extras\%T7Extras.version%
xcopy SetupT7Extras\version.bat C:\Users\mattias\Delivery\T7Extras\%T7Extras.version%\
xcopy SetupT7Extras\Release\T7Extras.msi C:\Users\mattias\Delivery\T7Extras\%T7Extras.version%\
xcopy SetupT7Extras\Release\T7Extras.md5 C:\Users\mattias\Delivery\T7Extras\%T7Extras.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > C:\Users\mattias\Delivery\T7Extras\version.xml
echo ^<t7extras version="%T7Extras.version%"/^> >> C:\Users\mattias\Delivery\T7Extras\version.xml

git tag T7Extras_v%T7Extras.version%