SET PGPATH=C:\"Program Files"\PostgreSQL\14\bin\
SET SVPATH=C:\Users\homan\YandexDisk\backups\
SET DBUSR=karma_admin
SET DBDUMP=.dump
SET DBDUMPSQL=.sql
SET PGPASSWORD=karma_admin
SET GITPATH=D:\repo\Murr\DatabaseBackups\

For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c_%%a_%%b)
SET BACKUPFILE=karma%mydate%_%date:~-4,4%_%date:~-10,2%_%date:~-7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%
set BACKUPFILE=%BACKUPFILE: =0%
%PGPATH%pg_dump.exe -h localhost -p 5432 -U %DBUSR% -Fc -d karma >%SVPATH%%BACKUPFILE%%DBDUMP%
%PGPATH%pg_dump.exe -h localhost -p 5432 -U %DBUSR% -d karma >%SVPATH%%BACKUPFILE%%DBDUMPSQL%
%PGPATH%pg_dump.exe -h localhost -p 5432 -U %DBUSR% -Fc -d karma >%GITPATH%%BACKUPFILE%%DBDUMP%
%PGPATH%pg_dump.exe -h localhost -p 5432 -U %DBUSR% -d karma >%GITPATH%%BACKUPFILE%%DBDUMPSQL%
pause