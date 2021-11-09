SET PGPATH=C:\"Program Files"\PostgreSQL\12\bin\
SET SVPATH=C:\Users\homan\Documents\repo\Murr\SqlScripts\MainDatabaseBackup\
SET DBUSR=postgres
SET PGPASSWORD=roman

SET BACKUPFILE=karma_2021_07_11_23_50_28.dump
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d postgres -f disconnect_all_karma_test.sql
%PGPATH%dropdb.exe -h localhost -p 5432 -U %DBUSR% karma_test
%PGPATH%createdb.exe -h localhost -p 5432 -U %DBUSR% -T template0 karma_test
%PGPATH%pg_restore.exe -h localhost -p 5432 -U %DBUSR% -d karma_test %SVPATH%%BACKUPFILE%
timeout 30
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d karma_test -f replace_data_for_test.sql
pause