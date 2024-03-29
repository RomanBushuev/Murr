insert into murr_downloader.task_types(task_type_id, task_type_title, task_type_description)
values(10, 'DOWNLOAD MOEX AMORTIZATIONS', 'Загрузка купонов из Moex');

CREATE OR REPLACE FUNCTION murr_downloader.add_moex_amortizations()
    RETURNS bigint
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER     
AS $BODY$	
declare
	d_folder_id bigint;
	d_folder_title character varying(255) = 'MOEX_DOWNLOAD_SERVICES';
	d_folder_sub_id bigint;
	d_folder_sub_root character varying(255) = 'MOEX_AMORTIZATIONS';
	d_title character varying(255);
	d_task_parameters jsonb;
	d_task_tempate_id bigint;
	d_task_id bigint;
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
	d_folder_title = to_char(now(), 'YYYY_MM_DD');	
	select folder_id into d_folder_id
	from murr_downloader.folders
	where folder_title = d_folder_title and folder_type_id = 1 and folder_root_id = d_folder_sub_id;
	
	if d_folder_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_sub_id, d_folder_title, 1)
		returning folder_id into d_folder_id;
	end if;
	
	d_title = 'DOWNLOAD MOEX COUPONS' || '_' ||  to_char(nextval('murr_downloader.murr_sequence'::regclass), 'FM999999999999999999');
	d_task_parameters = ('{}')::jsonb;
	
	--создаем шаблон
	insert into murr_downloader.task_templates(task_template_title, 
		task_template_created_time, 
		task_template_folder_id, 
		task_parameters, 
		task_type_id) values(d_title, now()::timestamp without time zone, d_folder_id, d_task_parameters, 10)
	returning task_template_id into d_task_tempate_id;
		
	--добавляем задачу к нам в шаблон
	insert into murr_downloader.tasks(task_template_id, task_created_time, task_status_id, saver_template_id)
	values(d_task_tempate_id, now()::timestamp without time zone, 2, null)
	returning task_id into d_task_id;
	
	perform murr_downloader.insert_task_numeric(d_task_id, 'ATTEMPTIONS', 1);	
	
	return d_task_id;
end
$BODY$;

ALTER FUNCTION murr_downloader.add_moex_amortizations()
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_moex_amortizations() TO karma_admin;
GRANT EXECUTE ON FUNCTION murr_downloader.add_moex_amortizations() TO karma_downloader;
REVOKE ALL ON FUNCTION murr_downloader.add_moex_amortizations() FROM PUBLIC;

insert into murr_downloader.task_types(task_type_id, task_type_title, task_type_description)
values(11, 'DOWNLOAD MOEX OFFERS', 'Загрузка офферт из Moex');

CREATE OR REPLACE FUNCTION murr_downloader.add_moex_offers()
    RETURNS bigint
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE SECURITY DEFINER     
AS $BODY$	
declare
	d_folder_id bigint;
	d_folder_title character varying(255) = 'MOEX_DOWNLOAD_SERVICES';
	d_folder_sub_id bigint;
	d_folder_sub_root character varying(255) = 'MOEX_OFFERS';
	d_title character varying(255);
	d_task_parameters jsonb;
	d_task_tempate_id bigint;
	d_task_id bigint;
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
	d_folder_title = to_char(now(), 'YYYY_MM_DD');	
	select folder_id into d_folder_id
	from murr_downloader.folders
	where folder_title = d_folder_title and folder_type_id = 1 and folder_root_id = d_folder_sub_id;
	
	if d_folder_id is null 
	then 
		insert into murr_downloader.folders(folder_root_id, folder_title, folder_type_id)
		values(d_folder_sub_id, d_folder_title, 1)
		returning folder_id into d_folder_id;
	end if;
	
	d_title = 'DOWNLOAD MOEX COUPONS' || '_' ||  to_char(nextval('murr_downloader.murr_sequence'::regclass), 'FM999999999999999999');
	d_task_parameters = ('{}')::jsonb;
	
	--создаем шаблон
	insert into murr_downloader.task_templates(task_template_title, 
		task_template_created_time, 
		task_template_folder_id, 
		task_parameters, 
		task_type_id) values(d_title, now()::timestamp without time zone, d_folder_id, d_task_parameters, 11)
	returning task_template_id into d_task_tempate_id;
		
	--добавляем задачу к нам в шаблон
	insert into murr_downloader.tasks(task_template_id, task_created_time, task_status_id, saver_template_id)
	values(d_task_tempate_id, now()::timestamp without time zone, 2, null)
	returning task_id into d_task_id;
	
	perform murr_downloader.insert_task_numeric(d_task_id, 'ATTEMPTIONS', 1);	
	
	return d_task_id;
end
$BODY$;

ALTER FUNCTION murr_downloader.add_moex_offers()
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_moex_offers() TO karma_admin;
GRANT EXECUTE ON FUNCTION murr_downloader.add_moex_offers() TO karma_downloader;
REVOKE ALL ON FUNCTION murr_downloader.add_moex_offers() FROM PUBLIC;


