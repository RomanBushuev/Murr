insert into murr_downloader.service_attributes

insert into murr_downloader.service_attributes(service_attribute_title, service_attribute_description)
values('LAST_LIFE_DATE_TIME', 'Последнее время отклика сервиса'),
('LAST_WORKING_DATE_TIME', 'Последнее время отклика сервиса, если он находится в рабочем статусе'),
('CURRENT_TASK_ID', 'Текущая выполняема задача, если задачи нет, -1'),
('SERVICE_LOG', 'Логирование состояния сервиса'),
('ERROR_SERVICE_LOG', 'Ошибки сервиса');

insert into murr_downloader.task_attributes(task_attribute_title, task_attribute_description)
values('ATTEMPTIONS', 'Кол-во запусков'),
('ERROR_LOG', 'Ошибки задачи'),
('INFO_LOG', 'Информационный лог'),
('WARNING_LOG', 'Предупреждения');

insert into murr_downloader.service_attributes(service_attribute_title, service_attribute_description)
values('ALIAS', 'Псевдоним');

insert into murr_downloader.task_attributes(task_attribute_title, task_attribute_description)
values('START_TASK', 'Время старта'),
('END_TASK', 'Время окончания');

--добавить информацию
insert into murr_downloader.services(service_title, service_status_id)
values ('DOWNLOADER', 0);

--если его не будет, то конфликты не будут видны 
alter table murr_downloader.service_attribute_strings
add constraint u_service_attribute_strings_service_attribute_attribute_id unique(service_service_attribute_id);

create or replace function murr_downloader.insert_service_string(in_service_name varchar(255),
	in_service_attribute varchar(255),
	in_service_value varchar(255))
	returns void
as $$	
declare
	d_service_id bigint = null;
	d_service_attriubte_id bigint = null;
	d_service_service_attribute_id bigint = null;
	d_hint text = null;
begin
	raise notice 'Start function insert_service_string';
	--find service_id
	select service_id into d_service_id from murr_downloader.services
	where murr_downloader.services.service_title = in_service_name;
	
	d_hint:= format('Please check service_name:%s', in_service_name);
	if d_service_id is null
	then 
	  RAISE EXCEPTION 'service_id --> null'
      USING HINT = d_hint;
	end if;	
	
	--find service_attribute_id
	select service_attribute_id into d_service_attriubte_id from murr_downloader.service_attributes
	where murr_downloader.service_attributes.service_attribute_title = in_service_attribute;
	
	d_hint:= format('Please check service_attribute_title:%s', in_service_attribute);
	if d_service_attriubte_id is null
	then 
		raise exception 'service_attribute_id --> null'
		using hint = d_hint;
	end if;
	
	--find service_service_attribute_id
	select service_service_attribute_id into d_service_service_attribute_id from murr_downloader.services_service_attributes
	where murr_downloader.services_service_attributes.service_id = d_service_id  
		and murr_downloader.services_service_attributes.service_attribute_id = d_service_attriubte_id;
	
	if d_service_service_attribute_id is null
	then 
		insert into murr_downloader.services_service_attributes(service_id, service_attribute_id)
		values(d_service_id, d_service_attriubte_id)
		returning service_service_attribute_id into d_service_service_attribute_id;
	end if;
	
	raise notice 'service_service_attribute_id: %', d_service_service_attribute_id;
	
	insert into murr_downloader.service_attribute_strings(service_service_attribute_id, service_service_attribute_title)
	values(d_service_service_attribute_id, in_service_value)
	on conflict (service_service_attribute_id) do update 
	set service_service_attribute_title = in_service_value;		
end
$$ language plpgsql
	SECURITY DEFINER;

ALTER FUNCTION murr_downloader.insert_service_string
    OWNER TO karma_admin;

