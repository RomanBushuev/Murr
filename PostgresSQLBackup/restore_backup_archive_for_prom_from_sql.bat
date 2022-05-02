SET PGPATH=C:\"Program Files"\PostgreSQL\14\bin\
SET SVPATH=D:\repo\Murr\SqlScripts\MainDatabaseBackup\
SET DBUSR=postgres
SET PGPASSWORD=roman

SET BACKUPFILE=karma_2022_12_02_10_56_42.sql
%PGPATH%dropdb.exe -h localhost -p 5432 -U %DBUSR% karma
%PGPATH%createdb.exe -h localhost -p 5432 -U %DBUSR% -T template0 karma
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d karma -f %SVPATH%%BACKUPFILE%
pause