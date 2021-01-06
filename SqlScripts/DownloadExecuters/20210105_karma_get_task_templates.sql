create or replace function murr_downloader.get_task_template(in_task_template_id bigint)
returns table(task_parameters jsonb, task_type_id bigint)
language 'plpgsql'
cost 100
volatile security definer 
rows 1000
as $body$
declare 
begin
return query 
	select task_templates.task_parameters, task_templates.task_type_id
	from murr_downloader.task_templates
	where task_templates.task_template_id = in_task_template_id;
end
$body$;
ALTER FUNCTION murr_downloader.get_task_template(bigint)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_template(bigint) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task_template(bigint) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_task_template(bigint) FROM PUBLIC;

