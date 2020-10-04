echo off
SET PGPATH=C:\"Program Files"\PostgreSQL\12\bin\
SET SVPATH=C:\Yandex\YandexDisk\backups\
SET DBUSR=karma_admin
SET DBDUMP=.sql
SET PGPASSWORD=karma_admin
SET GITPATH=C:\Users\homan\Documents\repo\Murr\SqlScripts\FullSchemaBackup\

For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c_%%a_%%b)
SET BACKUPFILE=karma%mydate%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%
%PGPATH%pg_dump.exe -h localhost -p 5432 -U %DBUSR% -s karma >%SVPATH%%BACKUPFILE%%DBDUMP%
%PGPATH%pg_dump.exe -h localhost -p 5432 -U %DBUSR% -s karma >%GITPATH%%BACKUPFILE%%DBDUMP%