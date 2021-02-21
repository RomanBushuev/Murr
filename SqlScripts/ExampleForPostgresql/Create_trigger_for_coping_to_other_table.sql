create table murr_downloader.person(id integer);

create table murr_downloader.person_temp(id integer);

alter table murr_downloader.person_temp
add constraint u_person_temp_id unique(id);

CREATE OR REPLACE FUNCTION murr_downloader.person_copy() RETURNS TRIGGER AS
$BODY$
BEGIN 
	--BEGIN
		INSERT INTO
			murr_downloader.person_temp(id)
			VALUES(new.id);
	--EXCEPTION WHEN OTHERS THEN
		--NULL;
	--END;
       RETURN new;
END;
$BODY$
language plpgsql;

CREATE TRIGGER trigger_person_copy
     AFTER INSERT ON murr_downloader.person
     FOR EACH ROW
     EXECUTE PROCEDURE murr_downloader.person_copy();
	 
insert into murr_downloader.person(id)
	 values(1);

--exception on constraint u_person_temp_id
insert into murr_downloader.person(id)
	 values(1);