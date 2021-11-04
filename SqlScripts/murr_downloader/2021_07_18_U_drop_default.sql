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
