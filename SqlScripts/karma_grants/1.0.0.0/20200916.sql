--use postgres role
--main database
create database karma;

create user karma_admin;
alter user karma_admin with password 'karma_admin';

--karma_admin main admin in the system;
GRANT ALL PRIVILEGES ON DATABASE karma TO karma_admin;

--karma_downloader download data from different system
create user karma_downloader;
alter user karma_downloader with password 'karma_downloader';

--karma_saver save data from different system into the database
create user karma_saver;
alter user karma_saver with password 'karma_saver';

--karma_calculator calculate data for the system
create user karma_calculator;
alter user karma_calculator with password 'karma_calculator';

create schema murr_data;
create schema murr_downloader;
