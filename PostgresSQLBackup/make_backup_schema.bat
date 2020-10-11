echo off

SET PGPATH=C:\"Program Files"\PostgreSQL\12\bin\
SET SVPATH=C:\Yandex\YandexDisk\backups\
SET DBUSR=karma_admin
SET DBDUMP=.sql
SET PGPASSWORD=karma_admin
SET GITPATH=C:\Users\homan\Documents\repo\Murr\SqlScripts\FullSchemaBackup\

For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c_%%a_%%b)
echo %mydate%
SET BACKUPFILE=karma%mydate%_%date:~-4,4%_%date:~-10,2%_%date:~-7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%
set BACKUPFILE=%BACKUPFILE: =0%
%PGPATH%pg_dump.exe -h localhost -p 5432 -U %DBUSR% -s karma >%SVPATH%%BACKUPFILE%%DBDUMP%
%PGPATH%pg_dump.exe -h localhost -p 5432 -U %DBUSR% -s karma >%GITPATH%%BACKUPFILE%%DBDUMP%
