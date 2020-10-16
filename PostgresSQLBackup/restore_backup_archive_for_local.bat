SET PGPATH=C:\"Program Files"\PostgreSQL\12\bin\
SET SVPATH=C:\Users\homan\Documents\repo\Murr\SqlScripts\MainDatabaseBackup\
SET DBUSR=postgres
SET PGPASSWORD=roman

SET BACKUPFILE=karma_2020_16_10_23_30_39.dump
%PGPATH%dropdb.exe -h localhost -p 5432 -U %DBUSR% karma_test
%PGPATH%createdb.exe -h localhost -p 5432 -U %DBUSR% -T template0 karma_test
%PGPATH%pg_restore.exe -h localhost -p 5432 -U %DBUSR% -d karma_test %SVPATH%%BACKUPFILE%