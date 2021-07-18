SET PGPATH=C:\"Program Files"\PostgreSQL\12\bin\
SET SVPATH=C:\Users\homan\Documents\repo\Murr\SqlScripts\MainDatabaseBackup\
SET DBUSR=postgres
SET PGPASSWORD=roman

SET BACKUPFILE=karma_2021_18_07_13_54_58.dump
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d postgres -f disconnect_all_karma_test.sql
%PGPATH%dropdb.exe -h localhost -p 5432 -U %DBUSR% karma_test
%PGPATH%createdb.exe -h localhost -p 5432 -U %DBUSR% -T template0 karma_test
%PGPATH%pg_restore.exe -h localhost -p 5432 -U %DBUSR% -d karma_test %SVPATH%%BACKUPFILE%
pause