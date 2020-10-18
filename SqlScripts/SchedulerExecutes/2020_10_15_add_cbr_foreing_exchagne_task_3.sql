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
    "RunDateTime": "' || to_char(now(), 'YYYY-MM-DD') ||'"
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
	values(d_task_tempate_id, now()::timestamp without time zone, 1, d_saver_template_id)
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


ALTER TABLE murr_downloader.defalult_saver_templates
RENAME TO default_saver_templates;