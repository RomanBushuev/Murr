SET PGPATH=C:\"Program Files"\PostgreSQL\12\bin\
SET SVPATH=C:\Users\homan\Documents\repo\Murr\SqlScripts\MainDatabaseBackup\
SET DBUSR=postgres
SET PGPASSWORD=roman

SET BACKUPFILE=karma_2022_12_02_10_56_42.sql
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d postgres -f disconnect_all_karma_test.sql
%PGPATH%dropdb.exe -h localhost -p 5432 -U %DBUSR% karma_test
%PGPATH%createdb.exe -h localhost -p 5432 -U %DBUSR% -T template0 karma_test
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d karma_test -f %SVPATH%%BACKUPFILE%
timeout 30
%PGPATH%psql.exe -h localhost -p 5432 -U %DBUSR% -d karma_test -f replace_data_for_test.sql
pause