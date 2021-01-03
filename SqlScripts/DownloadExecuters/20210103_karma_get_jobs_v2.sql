drop function murr_downloader.get_jobs;

CREATE OR REPLACE FUNCTION murr_downloader.get_jobs(
	)
    RETURNS TABLE(task_id bigint, task_template_id bigint, task_status_id bigint, saver_template_id bigint) 
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE SECURITY DEFINER 
    ROWS 1000
    
AS $BODY$	
declare
begin
	return query
		select tasks.task_id, tasks.task_template_id, tasks.task_status_id, tasks.saver_template_id
		from murr_downloader.tasks;
end
$BODY$;

ALTER FUNCTION murr_downloader.get_jobs()
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_jobs() TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_jobs() TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_jobs() FROM PUBLIC;

--views
create view murr_downloader.services_dates
as 
select s.service_id, s.service_title, sa.service_attribute_title, sad.service_service_attribute_date
from murr_downloader.services s,
murr_downloader.service_attributes sa,
murr_downloader.services_service_attributes ssa,
murr_downloader.service_attribute_dates sad
where s.service_id = ssa.service_id and 
	ssa.service_attribute_id = sa.service_attribute_id and
	sad.service_service_attribute_id = ssa.service_service_attribute_id;
	
	
create view murr_downloader.services_strings
as 
select s.service_id, s.service_title, sa.service_attribute_title, sad.service_service_attribute_title
from murr_downloader.services s,
murr_downloader.service_attributes sa,
murr_downloader.services_service_attributes ssa,
murr_downloader.service_attribute_strings sad
where s.service_id = ssa.service_id and 
	ssa.service_attribute_id = sa.service_attribute_id and
	sad.service_service_attribute_id = ssa.service_service_attribute_id;

	
create view murr_downloader.services_numerics
as 
select s.service_id, s.service_title, sa.service_attribute_title, sad.service_service_attribute_numeric
from murr_downloader.services s,
murr_downloader.service_attributes sa,
murr_downloader.services_service_attributes ssa,
murr_downloader.service_attribute_numerics sad
where s.service_id = ssa.service_id and 
	ssa.service_attribute_id = sa.service_attribute_id and
	sad.service_service_attribute_id = ssa.service_service_attribute_id;
	

create view murr_downloader.services_logs
as 
select s.service_id, s.service_title, sa.service_attribute_title, sad.service_service_attribute_date date, sad.service_service_attribute_title title
from murr_downloader.services s,
murr_downloader.service_attributes sa,
murr_downloader.services_service_attributes ssa,
murr_downloader.service_attribute_date_strings sad
where s.service_id = ssa.service_id and 
	ssa.service_attribute_id = sa.service_attribute_id and
	sad.service_service_attribute_id = ssa.service_service_attribute_id;
	
--Получение значение для сервисов
create or replace function murr_downloader.get_service_string (
	in_service_name character varying,
	in_attribute character varying)
	RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER 
as $BODY$
declare 
	d_title character varying;
begin 
	select sad.service_service_attribute_title into d_title
		from murr_downloader.services s,
		murr_downloader.service_attributes sa,
		murr_downloader.services_service_attributes ssa,
		murr_downloader.service_attribute_strings sad
		where s.service_id = ssa.service_id and 
			ssa.service_attribute_id = sa.service_attribute_id and
			sad.service_service_attribute_id = ssa.service_service_attribute_id and
			s.service_title = in_service_name and
			sa.service_attribute_title = in_attribute;
	return d_title;
end 
$BODY$;

ALTER FUNCTION murr_downloader.get_service_string(character varying, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_service_string(character varying, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_service_string(character varying, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_service_string(character varying, character varying) FROM PUBLIC;

--Получение числового значения для сервисов
create or replace function murr_downloader.get_service_numeric(
	in_service_name character varying,
	in_attribute character varying)
	RETURNS numeric
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER 
as $BODY$
declare 
	d_title numeric;
begin 
	select sad.service_service_attribute_numeric into d_title
		from murr_downloader.services s,
		murr_downloader.service_attributes sa,
		murr_downloader.services_service_attributes ssa,
		murr_downloader.service_attribute_numerics sad
		where s.service_id = ssa.service_id and 
			ssa.service_attribute_id = sa.service_attribute_id and
			sad.service_service_attribute_id = ssa.service_service_attribute_id and
			s.service_title = in_service_name and
			sa.service_attribute_title = in_attribute;
	return d_title;
end 
$BODY$;

ALTER FUNCTION murr_downloader.get_service_numeric(character varying, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_service_numeric(character varying, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_service_numeric(character varying, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_service_numeric(character varying, character varying) FROM PUBLIC;

--Получение значение дат для сервисов
create or replace function murr_downloader.get_service_date(
	in_service_name character varying,
	in_attribute character varying)
	RETURNS timestamp without time zone
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER 
as $BODY$
declare 
	d_title timestamp without time zone;
begin 
	select sad.service_service_attribute_date into d_title
		from murr_downloader.services s,
		murr_downloader.service_attributes sa,
		murr_downloader.services_service_attributes ssa,
		murr_downloader.service_attribute_dates sad
		where s.service_id = ssa.service_id and 
			ssa.service_attribute_id = sa.service_attribute_id and
			sad.service_service_attribute_id = ssa.service_service_attribute_id and
			s.service_title = in_service_name and
			sa.service_attribute_title = in_attribute;
	return d_title;
end 
$BODY$;

ALTER FUNCTION murr_downloader.get_service_date(character varying, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_service_date(character varying, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_service_date(character varying, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_service_date(character varying, character varying) FROM PUBLIC;

--Получение значение для задач
create view murr_downloader.tasks_strings
as 
select t.task_id, ta.task_attribute_title, tas.task_task_attribute_title
from murr_downloader.tasks t,
murr_downloader.task_attributes ta,
murr_downloader.tasks_task_attributes tta,
murr_downloader.task_attribute_strings tas
where t.task_id = tta.task_id and
ta.task_attribute_id = tta.task_attribute_id and
tta.task_task_attribute_id = tas.task_task_attribute_id;

create view murr_downloader.tasks_numerics
as 
select t.task_id, ta.task_attribute_title, tan.task_task_attribute_numeric
from murr_downloader.tasks t,
murr_downloader.task_attributes ta,
murr_downloader.tasks_task_attributes tta,
murr_downloader.task_attribute_numerics tan
where t.task_id = tta.task_id and
ta.task_attribute_id = tta.task_attribute_id and
tta.task_task_attribute_id = tan.task_task_attribute_id;

create view murr_downloader.tasks_dates
as 
select t.task_id, ta.task_attribute_title, tad.task_task_attribute_date
from murr_downloader.tasks t,
murr_downloader.task_attributes ta,
murr_downloader.tasks_task_attributes tta,
murr_downloader.task_attribute_dates tad
where t.task_id = tta.task_id and
ta.task_attribute_id = tta.task_attribute_id and
tta.task_task_attribute_id = tad.task_task_attribute_id;
	
	
create or replace function murr_downloader.get_task_string (
	in_task_id bigint,
	in_attribute character varying)
	RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER 
as $BODY$
declare 
	d_title character varying;
begin 
	select tas.task_task_attribute_title into d_title
	from murr_downloader.tasks t,
	murr_downloader.task_attributes ta,
	murr_downloader.tasks_task_attributes tta,
	murr_downloader.task_attribute_strings tas
	where t.task_id = tta.task_id and
	ta.task_attribute_id = tta.task_attribute_id and
	tta.task_task_attribute_id = tas.task_task_attribute_id and
	t.task_id = in_task_id and
	ta.task_attribute_title = in_attribute;
	return d_title;
end 
$BODY$;

ALTER FUNCTION murr_downloader.get_task_string(bigint, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_string(bigint, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_string(bigint, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_task_string(bigint, character varying) FROM PUBLIC;


create or replace function murr_downloader.get_task_numeric (
	in_task_id bigint,
	in_attribute character varying)
	RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER 
as $BODY$
declare 
	d_title numeric;
begin 
	select tan.task_task_attribute_numeric into d_title
	from murr_downloader.tasks t,
	murr_downloader.task_attributes ta,
	murr_downloader.tasks_task_attributes tta,
	murr_downloader.task_attribute_numerics tan
	where t.task_id = tta.task_id and
	ta.task_attribute_id = tta.task_attribute_id and
	tta.task_task_attribute_id = tan.task_task_attribute_id and
	t.task_id = in_task_id and
	ta.task_attribute_title = in_attribute;
	return d_title;
end 
$BODY$;

ALTER FUNCTION murr_downloader.get_task_numeric(bigint, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_numeric(bigint, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_numeric(bigint, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_task_numeric(bigint, character varying) FROM PUBLIC;

	
create or replace function murr_downloader.get_task_date(
	in_task_id bigint,
	in_attribute character varying)
	RETURNS timestamp without time zone
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER 
as $BODY$
declare 
	d_title timestamp without time zone;
begin 
	select tad.task_task_attribute_date into d_title
	from murr_downloader.tasks t,
	murr_downloader.task_attributes ta,
	murr_downloader.tasks_task_attributes tta,
	murr_downloader.task_attribute_dates tad
	where t.task_id = tta.task_id and
	ta.task_attribute_id = tta.task_attribute_id and
	tta.task_task_attribute_id = tad.task_task_attribute_id and
	t.task_id = in_task_id and
	ta.task_attribute_title = in_attribute;
	return d_title;
end 
$BODY$;

ALTER FUNCTION murr_downloader.get_task_date(bigint, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_date(bigint, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_date(bigint, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_task_date(bigint, character varying) FROM PUBLIC;


create or replace function murr_downloader.add_service(
	in_service_name character varying)
	RETURNS bigint
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER 
as $BODY$
declare 
	d_service_id bigint;
begin
	insert into murr_downloader.services(service_title, service_status_id)
	values(in_service_name, 0)
	returning service_id into d_service_id;
	return d_service_id;
end
$BODY$;
ALTER FUNCTION murr_downloader.add_service(character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_service(character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_service(character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.add_service(character varying) FROM PUBLIC;

