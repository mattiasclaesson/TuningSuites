call T5CanFlash\version.bat
devenv T5CanFlasher.sln /Rebuild Release /project T5CanFlash

pushd T5CanFlash\Release\
"C:\md5sum.exe" T5CanFlash.msi >> T5CanFlash.md5
popd

mkdir z:\T5CanFlasher\%T5CanFlash.version%
xcopy T5CanFlash\version.bat z:\T5CanFlasher\%T5CanFlash.version%\
xcopy T5CanFlash\Release\T5CanFlash.msi z:\T5CanFlasher\%T5CanFlash.version%\
xcopy T5CanFlash\Release\T5CanFlash.md5 z:\T5CanFlasher\%T5CanFlash.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > z:\T5CanFlasher\version.xml
echo ^<t5canflasher version="%T5CanFlash.version%"/^> >> z:\T5CanFlasher\version.xml

git tag T5CanFlasher_v%T5CanFlasher.version%
git tag T5CanLib_v%T5CanLib.version%
git tag SetupT5CanFlash_v%T5CanFlash.version%

git push --tags

gh release create T5CanFlash_v%T5CanFlasher.version% --generate-notes --verify-tag
gh release upload T5CanFlash_v%T5CanFlasher.version% T5CanFlash\Release\T5CanFlash.msi
gh release upload T5CanFlash_v%T5CanFlasher.version% T5CanFlash\Release\T5CanFlash.md5