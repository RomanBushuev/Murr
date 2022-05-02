SET PGPATH=C:\"Program Files"\PostgreSQL\14\bin\
SET SVPATH=D:\repo\Murr\DatabaseBackups\
SET DBUSR=postgres
SET PGPASSWORD=roman

SET BACKUPFILE=karma_2022_03_05_00_21_20.dump
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d postgres -f disconnect_all_karma_test.sql
%PGPATH%dropdb.exe -h localhost -p 5432 -U %DBUSR% karma_test
%PGPATH%createdb.exe -h localhost -p 5432 -U %DBUSR% -T template0 karma_test
%PGPATH%pg_restore.exe -h localhost -p 5432 -U %DBUSR% -d karma_test %SVPATH%%BACKUPFILE%
timeout 5
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d karma_test -f replace_data_for_test.sql
pause