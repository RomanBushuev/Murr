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
	

INSERT INTO murr_downloader.service_statuses(service_status_id, service_status_title, service_status_description)
values(3, 'FINISHED', 'Остановлен физически');

INSERT INTO murr_downloader.task_statuses(task_status_id, task_status_title, task_statuses_description)
values(6, 'CANCELLING', 'Отмена');

INSERT INTO murr_downloader.task_statuses(task_status_id, task_status_title, task_statuses_description)
values(7, 'CANCELED', 'Отменено');


CREATE OR REPLACE FUNCTION murr_downloader.insert_task(
	in_task_template_title character varying,
	in_task_template_folder_id bigint,
	in_task_parameters jsonb,
	in_task_type_id bigint,
	in_task_status_id bigint
	)
		returns bigint
as $$
declare
	d_task_tempate_id bigint;
	d_task_id bigint;
begin
	insert into murr_downloader.task_templates(task_template_title, 
		task_template_created_time, 
		task_template_folder_id, 
		task_parameters, 
		task_type_id) values(in_task_template_title, now()::timestamp without time zone, in_task_template_folder_id, in_task_parameters, in_task_type_id)
	returning task_template_id into d_task_tempate_id;
	
	insert into murr_downloader.tasks(task_template_id, task_created_time, task_status_id, saver_template_id)
	values(d_task_tempate_id, now()::timestamp without time zone, in_task_status_id, null)
	returning task_id into d_task_id;
	return d_task_id;
end
$$ language plpgsql
	SECURITY DEFINER;
	
ALTER FUNCTION murr_downloader.insert_task(character varying, bigint, jsonb, bigint, bigint)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task(character varying, bigint, jsonb, bigint, bigint) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task(character varying, bigint, jsonb, bigint, bigint) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.insert_task(character varying, bigint, jsonb, bigint, bigint) FROM PUBLIC;


CREATE OR REPLACE FUNCTION murr_downloader.add_cbr_foreign_exchange(
	in_datetime timestamp without time zone)
    RETURNS bigint
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER     
AS $BODY$	
declare
	d_folder_id bigint;
	d_folder_title character varying(255) = 'CBR_DOWNLOAD_SERVICES';
	d_folder_sub_id bigint;
	d_folder_sub_root character varying(255) = 'FOREIGN_EXCHANGE';
	d_service_service_attribute_id bigint = null;
	d_title character varying(255);
	d_hint text = null;
	d_task_parameters jsonb;
	d_task_tempate_id bigint;
	d_saver_template_id bigint;
	d_task_id bigint;
	d_xml_path character varying(2034);
begin
	--создать папку для загрузок 
	select folder_id into d_folder_id 
	from murr_downloader.folders 
	where folder_title = d_folder_title and folder_type_id = 1 and folder_root_id is null;
	
	if d_folder_id is null
	then
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(null, d_folder_title, 1)
		returning folder_id into d_folder_id;
	end if;
	
	--добавить папку 
	select folder_id into d_folder_sub_id
	from murr_downloader.folders
	where folder_title = d_folder_sub_root and folder_type_id = 1 and folder_root_id = d_folder_id;
	
	--добавить под папку 
	if d_folder_sub_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_id, d_folder_sub_root, 1)
		returning folder_id into d_folder_sub_id;
	end if;
	
	--добавить время в под папку
	d_folder_title = to_char(in_datetime, 'YYYY_MM_DD');
	
	select folder_id into d_folder_id
	from murr_downloader.folders
	where folder_title = d_folder_title and folder_type_id = 1 and folder_root_id = d_folder_sub_id;
	
	if d_folder_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_sub_id, d_folder_title, 1)
		returning folder_id into d_folder_id;
	end if;
	
	d_title = d_folder_title || '_' ||  to_char(nextval('murr_downloader.murr_sequence'::regclass), 'FM999999999999999999');
	
	d_task_parameters = ('{
    "RunDateTime": "' || to_char(in_datetime, 'YYYY-MM-DD') ||'"
	}')::jsonb;
	
	--создаем шаблон
	insert into murr_downloader.task_templates(task_template_title, 
		task_template_created_time, 
		task_template_folder_id, 
		task_parameters, 
		task_type_id) values(d_title, now()::timestamp without time zone, d_folder_id, d_task_parameters, 1)
	returning task_template_id into d_task_tempate_id;


	--создаем папку для шаблонов сохранения файлов
	select folder_id into d_folder_id 
	from murr_downloader.folders 
	where folder_title = d_folder_title and folder_type_id = 2 and folder_root_id is null;
	
	if d_folder_id is null
	then
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(null, d_folder_title, 2)
		returning folder_id into d_folder_id;
	end if;
	
	--добавить папку 
	select folder_id into d_folder_sub_id
	from murr_downloader.folders
	where folder_title = d_folder_sub_root and folder_type_id = 2 and folder_root_id = d_folder_id;
	
	--добавить под папку 
	if d_folder_sub_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_id, d_folder_sub_root, 2)
		returning folder_id into d_folder_sub_id;
	end if;
	
	--добавить время в под папку
	d_folder_title = to_char(in_datetime, 'YYYY_MM_DD');
	
	select folder_id into d_folder_id
	from murr_downloader.folders
	where folder_title = d_folder_title and folder_type_id = 2 and folder_root_id = d_folder_sub_id;
	
	if d_folder_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_sub_id, d_folder_title, 2)
		returning folder_id into d_folder_id;
	end if;
	
	select dst.default_path into d_xml_path from murr_downloader.task_types tt,
	murr_downloader.default_saver_templates dst
	where tt.task_type_id = dst.task_type_id;
	
	if d_xml_path is null
	then
		RAISE EXCEPTION 'Не найден путь по умолчанию';
	end if;
	raise notice 'First:%', d_xml_path;
	
	if SUBSTR(d_xml_path, LENGTH(d_xml_path), 1) <> '\'
	then 
		d_xml_path = d_xml_path || '\';
	end if;
	raise notice 'Second:%', d_xml_path;
	d_xml_path = d_xml_path || to_char(nextval('murr_downloader.murr_sequence'::regclass), 'FM999999999999999999') || '.xml';
	d_task_parameters = ('{
    "Path": ' || to_json(d_xml_path::text) ||'
	}')::jsonb;
	
	raise notice 'Third:%', d_task_parameters;
	insert into murr_downloader.saver_templates(saver_template_title,
		saver_template_created_time,
		saver_template_folder_id,
		saver_parameters,
		saver_type_id)
	values(d_title,
		now()::timestamp without time zone,
		d_folder_id,
		d_task_parameters,
		1)
	returning saver_template_id into d_saver_template_id;	
	
	--добавляем задачу к нам в шаблон
	insert into murr_downloader.tasks(task_template_id, task_created_time, task_status_id, saver_template_id)
	values(d_task_tempate_id, now()::timestamp without time zone, 2, d_saver_template_id)
	returning task_id into d_task_id;
	
	perform murr_downloader.insert_task_numeric(d_task_id, 'ATTEMPTIONS', 1);	
	
	return d_task_id;
end
$BODY$;


ALTER FUNCTION murr_downloader.add_cbr_foreign_exchange(timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_cbr_foreign_exchange(timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_cbr_foreign_exchange(timestamp without time zone) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.add_cbr_foreign_exchange(timestamp without time zone) FROM PUBLIC;


CREATE OR REPLACE FUNCTION murr_downloader.add_cbr_mosprime(
	in_datetime timestamp without time zone)
    RETURNS bigint
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE SECURITY DEFINER 
    
AS $BODY$	
declare
	d_folder_id bigint;
	d_folder_title character varying(255) = 'CBR_DOWNLOAD_SERVICES';
	d_folder_sub_id bigint;
	d_folder_sub_root character varying(255) = 'MOSPRIME';
	d_service_service_attribute_id bigint = null;
	d_title character varying(255);
	d_hint text = null;
	d_task_parameters jsonb;
	d_task_tempate_id bigint;
	d_saver_template_id bigint;
	d_task_id bigint;
	d_xml_path character varying(2034);
begin
	--создать папку для загрузок 
	select folder_id into d_folder_id 
	from murr_downloader.folders 
	where folder_title = d_folder_title and folder_type_id = 1 and folder_root_id is null;
	
	if d_folder_id is null
	then
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(null, d_folder_title, 1)
		returning folder_id into d_folder_id;
	end if;
	
	--добавить папку 
	select folder_id into d_folder_sub_id
	from murr_downloader.folders
	where folder_title = d_folder_sub_root and folder_type_id = 1 and folder_root_id = d_folder_id;
	
	--добавить под папку 
	if d_folder_sub_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_id, d_folder_sub_root, 1)
		returning folder_id into d_folder_sub_id;
	end if;
	
	--добавить время в под папку
	d_folder_title = to_char(in_datetime, 'YYYY_MM_DD');
	
	select folder_id into d_folder_id
	from murr_downloader.folders
	where folder_title = d_folder_title and folder_type_id = 1 and folder_root_id = d_folder_sub_id;
	
	if d_folder_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_sub_id, d_folder_title, 1)
		returning folder_id into d_folder_id;
	end if;
	
	d_title = d_folder_title || '_' ||  to_char(nextval('murr_downloader.murr_sequence'::regclass), 'FM999999999999999999');
	
	d_task_parameters = ('{
    "RunDateTime": "' || to_char(in_datetime, 'YYYY-MM-DD') ||'"
	}')::jsonb;
	
	--создаем шаблон
	insert into murr_downloader.task_templates(task_template_title, 
		task_template_created_time, 
		task_template_folder_id, 
		task_parameters, 
		task_type_id) values(d_title, now()::timestamp without time zone, d_folder_id, d_task_parameters, 3)
	returning task_template_id into d_task_tempate_id;

	--создаем папку для шаблонов сохранения файлов
	select folder_id into d_folder_id 
	from murr_downloader.folders 
	where folder_title = d_folder_title and folder_type_id = 2 and folder_root_id is null;
	
	if d_folder_id is null
	then
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(null, d_folder_title, 2)
		returning folder_id into d_folder_id;
	end if;
	
	--добавить папку 
	select folder_id into d_folder_sub_id
	from murr_downloader.folders
	where folder_title = d_folder_sub_root and folder_type_id = 2 and folder_root_id = d_folder_id;
	
	--добавить под папку 
	if d_folder_sub_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_id, d_folder_sub_root, 2)
		returning folder_id into d_folder_sub_id;
	end if;
	
	--добавить время в под папку
	d_folder_title = to_char(in_datetime, 'YYYY_MM_DD');
	
	select folder_id into d_folder_id
	from murr_downloader.folders
	where folder_title = d_folder_title and folder_type_id = 2 and folder_root_id = d_folder_sub_id;
	
	if d_folder_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_sub_id, d_folder_title, 2)
		returning folder_id into d_folder_id;
	end if;
	
	select dst.default_path into d_xml_path from murr_downloader.task_types tt,
	murr_downloader.default_saver_templates dst
	where tt.task_type_id = dst.task_type_id
		and tt.task_type_id = 3;
	
	if d_xml_path is null
	then
		RAISE EXCEPTION 'Не найден путь по умолчанию';
	end if;
	raise notice 'First:%', d_xml_path;
	
	if SUBSTR(d_xml_path, LENGTH(d_xml_path), 1) <> '\'
	then 
		d_xml_path = d_xml_path || '\';
	end if;
	raise notice 'Second:%', d_xml_path;
	d_xml_path = d_xml_path || to_char(nextval('murr_downloader.murr_sequence'::regclass), 'FM999999999999999999') || '.xml';
	d_task_parameters = ('{
    "Path": ' || to_json(d_xml_path::text) ||'
	}')::jsonb;
	
	raise notice 'Third:%', d_task_parameters;
	insert into murr_downloader.saver_templates(saver_template_title,
		saver_template_created_time,
		saver_template_folder_id,
		saver_parameters,
		saver_type_id)
	values(d_title,
		now()::timestamp without time zone,
		d_folder_id,
		d_task_parameters,
		1)
	returning saver_template_id into d_saver_template_id;	
	
	--добавляем задачу к нам в шаблон
	insert into murr_downloader.tasks(task_template_id, task_created_time, task_status_id, saver_template_id)
	values(d_task_tempate_id, now()::timestamp without time zone, 2, d_saver_template_id)
	returning task_id into d_task_id;
	
	perform murr_downloader.insert_task_numeric(d_task_id, 'ATTEMPTIONS', 1);	
	
	return d_task_id;
end
$BODY$;

ALTER FUNCTION murr_downloader.add_cbr_mosprime(timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_cbr_mosprime(timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_cbr_mosprime(timestamp without time zone) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.add_cbr_mosprime(timestamp without time zone) FROM PUBLIC;
