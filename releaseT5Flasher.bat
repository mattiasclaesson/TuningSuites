call T5CanFlash\version.bat
devenv T5CanFlasher.sln /Rebuild Release /project T5CanFlash

pushd T5CanFlash\Release\
"C:\Program Files (x86)\hashutils-1.3.0-redist\bin.x86-32\md5sum.exe" T5CanFlash.msi >> T5CanFlash.md5
popd

mkdir z:\T5CanFlasher\%T5CanFlash.version%
xcopy T5CanFlash\version.bat z:\T5CanFlasher\%T5CanFlash.version%\
xcopy T5CanFlash\Release\T5CanFlash.msi z:\T5CanFlasher\%T5CanFlash.version%\
xcopy T5CanFlash\Release\T5CanFlash.md5 z:\T5CanFlasher\%T5CanFlash.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > z:\T5CanFlasher\version.xml
echo ^<t5canflasher version="%T5CanFlash.version%"/^> >> z:\T5CanFlasher\version.xml

echo ----------------------------------------------------
git changes
echo ----------------------------------------------------

git tag T5CanFlasher_v%T5CanFlasher.version%
git tag T5CanLib_v%T5CanLib.version%
git tag SetupT5CanFlash_v%T5CanFlash.version%