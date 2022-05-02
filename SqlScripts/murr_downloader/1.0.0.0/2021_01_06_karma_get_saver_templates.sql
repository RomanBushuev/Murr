create or replace function murr_downloader.get_saver_template(in_saver_template_id bigint)
returns table(saver_parameters jsonb, saver_type_id bigint)
language 'plpgsql'
cost 100
volatile security definer 
rows 1000
as $body$
declare 
begin
return query 
	select saver_templates.saver_parameters, saver_templates.saver_type_id
	from murr_downloader.saver_templates
	where saver_templates.saver_template_id = in_saver_template_id;
end
$body$;
ALTER FUNCTION murr_downloader.get_saver_template(bigint)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_saver_template(bigint) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_saver_template(bigint) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_saver_template(bigint) FROM PUBLIC;

CREATE OR REPLACE FUNCTION murr_downloader.add_service(
	in_service_name character varying)
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
		return d_service_id;
	end if;	
	
	insert into murr_downloader.services(service_title, service_status_id)
	values(in_service_name, 1)
	returning service_id into d_service_id;
	return d_service_id;
end
$BODY$;

ALTER FUNCTION murr_downloader.add_service(character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_service(character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.add_service(character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.add_service(character varying) FROM PUBLIC;

