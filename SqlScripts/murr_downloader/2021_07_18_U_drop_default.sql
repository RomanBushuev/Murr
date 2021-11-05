alter table murr_downloader.task_types
alter task_type_id drop default;

insert into murr_downloader.task_types(task_type_id, task_type_title, task_type_description)
values(8, 'SAVE CURRENCIES CBRF', 'Сохранение валюты ЦБ');

CREATE OR REPLACE FUNCTION murr_downloader.update_saver_template(
	in_saver_template_id bigint,
	in_saver_json jsonb)
    returns void
as $$	
declare 
begin
	update murr_downloader.saver_templates
	set saver_parameters = in_saver_json
	where saver_templates.saver_template_id = in_saver_template_id;
end
$$ language plpgsql
	SECURITY DEFINER;

ALTER FUNCTION murr_downloader.update_saver_template(bigint, jsonb)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.update_saver_template(bigint, jsonb) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.update_saver_template(bigint, jsonb) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.update_saver_template(bigint, jsonb) FROM PUBLIC;

alter table murr_downloader.services
add column service_version varchar(31);

alter table murr_downloader.services
add column service_health_check timestamp without time zone;


CREATE OR REPLACE FUNCTION murr_downloader.change_service_status(
	in_service_title character varying,
	in_new_service_status_id bigint)
    RETURNS void
AS $$
declare 
begin
	update murr_downloader.services
	set service_status_id = in_new_service_status_id
	where service_title = in_service_title;
end
$$ language plpgsql
	SECURITY DEFINER;

ALTER FUNCTION murr_downloader.change_service_status(character varying, bigint)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.change_service_status(character varying, bigint) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.change_service_status(character varying, bigint) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.change_service_status(character varying, bigint) FROM PUBLIC;

update murr_downloader.procedure_tasks
set procedure_params =  '{
    "in_datetime": "the day before yesterday"
}'::jsonb
where procedure_task_id = 1660;


CREATE OR REPLACE FUNCTION murr_downloader.insert_pipeline_tasks(
	in_start_task_id bigint,
	in_next_task_id bigint)
	returns void
as $$
declare
begin
	insert into murr_downloader.pipeline_tasks(start_task_id, next_task_id,
		created_date_time, update_date_time, is_done)
	values(in_start_task_id, in_next_task_id,
		now()::timestamp without time zone,
		now()::timestamp without time zone,
		null);
end
$$ language plpgsql
	SECURITY DEFINER;
	
ALTER FUNCTION murr_downloader.insert_pipeline_tasks(bigint, bigint)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_pipeline_tasks(bigint, bigint) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_pipeline_tasks(bigint, bigint) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.insert_pipeline_tasks(bigint, bigint) FROM PUBLIC;
	
	
CREATE OR REPLACE FUNCTION murr_downloader.tasks_copy()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
declare 
	f record;
	d_done_tasks bigint;
	d_all_tasks bigint;
BEGIN 
	raise notice 'Задача:%', new.task_id;
	for f in select next_task_id from murr_downloader.pipeline_tasks
		where start_task_id = new.task_id 
	loop 	
		select count(1) into d_all_tasks from murr_downloader.pipeline_tasks pt
		join murr_downloader.tasks t on pt.start_task_id = t.task_id
		where pt.next_task_id = f.next_task_id;
		
		select count(1) into d_done_tasks from murr_downloader.pipeline_tasks pt
		join murr_downloader.tasks t on pt.start_task_id = t.task_id
		where pt.next_task_id = f.next_task_id and task_status_id = 4;
		raise notice '%:всего задач:%:сделано задач:%', f.next_task_id, d_all_tasks, d_done_tasks;
		
		if d_all_tasks = d_done_tasks
		then
			update murr_downloader.tasks
			set task_status_id = 2
			where task_id = f.next_task_id;
		end if;
		
	end loop;
    RETURN new;
END;
$BODY$;

ALTER FUNCTION murr_downloader.tasks_copy()
    OWNER TO karma_admin;

drop function murr_downloader.get_task_template;

create or replace function murr_downloader.get_task_template(in_task_template_id bigint)
returns table(task_parameters jsonb, task_type_id bigint, task_template_folder_id bigint)
language 'plpgsql'
cost 100
volatile security definer 
rows 1000
as $body$
declare 
begin
return query 
	select task_templates.task_parameters, task_templates.task_type_id, task_templates.task_template_folder_id
	from murr_downloader.task_templates
	where task_templates.task_template_id = in_task_template_id;
end
$body$;
ALTER FUNCTION murr_downloader.get_task_template(bigint)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_template(bigint) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_template(bigint) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_task_template(bigint) FROM PUBLIC;

DROP FUNCTION murr_downloader.add_service;

CREATE OR REPLACE FUNCTION murr_downloader.add_service(
	in_service_name character varying,
	in_service_version character varying
	)
    RETURNS bigint
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE SECURITY DEFINER 
    
AS $BODY$
declare 
	d_service_id bigint;
begin
	select service_id into d_service_id
	from murr_downloader.services
	where service_title = in_service_name;
	
	if d_service_id is not null
	then
		update murr_downloader.services
		set service_version = in_service_version
		where service_id = d_service_id;
		return d_service_id;
	end if;	
	
	insert into murr_downloader.services(service_title, service_status_id, service_version)
	values(in_service_name, 1, in_service_version)
	returning service_id into d_service_id;
	return d_service_id;
end
$BODY$;

ALTER FUNCTION murr_downloader.add_service(character varying, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_service(character varying, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_service(character varying, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.add_service(character varying, character varying) FROM PUBLIC;

CREATE OR REPLACE FUNCTION murr_downloader.update_health_check(
	in_service_id bigint,
	in_service_time timestamp without time zone
	)
		returns void
as $$
declare
begin
	update murr_downloader.services
	set service_health_check = in_service_time
	where service_id = in_service_id;
end
$$ language plpgsql
	SECURITY DEFINER;
	
ALTER FUNCTION murr_downloader.update_health_check(bigint, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.update_health_check(bigint, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.update_health_check(bigint, timestamp without time zone) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.update_health_check(bigint, timestamp without time zone) FROM PUBLIC;
	

