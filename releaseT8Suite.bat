call SetupT8SuitePro\version.bat
devenv T8.sln /Rebuild Release /project SetupT8Suite

pushd SetupT8SuitePro\Release\
"C:\md5sum.exe" T8Suite.msi >> T8Suite.md5
"C:\Program Files\7-Zip\7z.exe" a -tzip T8Suite.zip T8Suite.* setup.exe
popd

mkdir z:\T8Suite\%T8.version%
xcopy SetupT8SuitePro\version.bat z:\T8Suite\%T8.version%\
xcopy SetupT8SuitePro\Release\T8Suite.msi z:\T8Suite\%T8.version%\
xcopy SetupT8SuitePro\Release\T8Suite.md5 z:\T8Suite\%T8.version%\
xcopy SetupT8SuitePro\Release\T8Suite.zip z:\T8Suite\%T8.version%\
xcopy SetupT8SuitePro\Release\setup.exe z:\T8Suite\%T8.version%\

echo ^<?xml version="1.0" encoding="utf-8"?^>  > z:\T8Suite\version.xml
echo ^<t8suitepro version="%T8.version%"/^> >> z:\T8Suite\version.xml

git tag T8suite_v%T8.version%
git tag SetupT8suite_v%SetupT8Suite.version%

git push --tags

gh release create T8suite_v%T8.version% --generate-notes --verify-tag
gh release upload T8suite_v%T8.version% SetupT8SuitePro\Release\T8Suite.zip
gh release upload T8suite_v%T8.version% SetupT8SuitePro\Release\T8Suite.msi
gh release upload T8suite_v%T8.version% SetupT8SuitePro\Release\T8Suite.md5