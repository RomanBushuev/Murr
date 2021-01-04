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
	select service_id into d_service_id
	from murr_downloader.services
	where service_title = in_service_name;
	
	if d_service_id is not null
	then
		return d_service_id;
	end if;	
	
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

create or replace function murr_downloader.get_services()
returns table(service_id bigint, service_title character varying, service_status_id bigint)
language 'plpgsql'
cost 100
volatile security definer 
rows 1000
as $body$
declare 
begin
	 return query
		 select services.service_id,services.service_title, services.service_status_id 
		 from murr_downloader.services;
end
$body$;
ALTER FUNCTION murr_downloader.get_services()
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_services() TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_services() TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_services() FROM PUBLIC;

