SET PGPATH=C:\"Program Files"\PostgreSQL\14\bin\
SET SVPATH=D:\repo\Murr\DatabaseBackups\
SET DBUSR=postgres
SET PGPASSWORD=roman

SET BACKUPFILE=karma_2022_03_05_00_45_04.dump
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d postgres -f disconnect_all_karma_test.sql
%PGPATH%dropdb.exe -h localhost -p 5432 -U %DBUSR% karma
%PGPATH%createdb.exe -h localhost -p 5432 -U %DBUSR% -T template0 karma
%PGPATH%pg_restore.exe -h localhost -p 5432 -U %DBUSR% -d karma %SVPATH%%BACKUPFILE%
pause