call T5SuiteII\version.bat
devenv T5SuiteII.sln /Rebuild Release /project T5SuiteII
mkdir C:\users\mattias\Dropbox\public\T5Suite2\%T5SuiteII.version%
xcopy T5SuiteII\version.bat C:\users\mattias\Dropbox\public\T5Suite2\%T5SuiteII.version%\
xcopy T5SuiteII\Release\T5SuiteII.msi C:\users\mattias\Dropbox\public\T5Suite2\%T5SuiteII.version%\

git tag T5SuiteII_v%T5SuiteII.version%