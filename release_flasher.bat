call T5CanFlash\version.bat
devenv T5CanFlasher.sln /Rebuild Release /project T5CanFlasher

pushd T5CanFlash\Release\
"C:\Program Files (x86)\hashutils-1.3.0-redist\bin.x86-32\md5sum.exe" T5CanFlasher.msi >> T5CanFlasher.md5
popd

mkdir C:\users\mattias\Dropbox\public\T5CanFlash\%T5CanFlasher.version%
xcopy T5CanFlash\version.bat C:\users\mattias\Dropbox\public\T5CanFlasher\%T5CanFlasher.version%\
xcopy T5CanFlash\Release\T5CanFlasher.msi C:\users\mattias\Dropbox\public\T5CanFlasher\%T5CanFlasher.version%\
xcopy T5CanFlash\Release\T5CanFlasher.md5 C:\users\mattias\Dropbox\public\T5CanFlasher\%T5CanFlasher.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > C:\users\mattias\Dropbox\public\T5CanFlasher\version.xml
echo ^<t5canflasher version="%T5CanFlasher.version%"/^> >> C:\users\mattias\Dropbox\public\T5CanFlasher\version.xml
git tag T5CanFlasher_v%T5CanFlasher.version%
git tag T5CanLib_v%T5CanLib.version%
git tag SetupT5CanFlash_v%T5CanFlash.version%