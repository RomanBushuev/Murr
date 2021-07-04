--если его не будет, то конфликты не будут видны 
alter table murr_downloader.service_attribute_numerics
add constraint u_service_attribute_numerics_service_attribute_attribute_id unique(service_service_attribute_id);

create or replace function murr_downloader.insert_service_numeric(in_service_name varchar(255),
	in_service_attribute varchar(255),
	in_service_value numeric)
	returns void
as $$	
declare
	d_service_id bigint = null;
	d_service_attriubte_id bigint = null;
	d_service_service_attribute_id bigint = null;
	d_hint text = null;
begin
	raise notice 'Start function insert_service_numeric';
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
	
	insert into murr_downloader.service_attribute_numerics(service_service_attribute_id, service_service_attribute_numeric)
	values(d_service_service_attribute_id, in_service_value)
	on conflict (service_service_attribute_id) do update 
	set service_service_attribute_numeric = in_service_value;		
end
$$ language plpgsql
	SECURITY DEFINER;

ALTER FUNCTION murr_downloader.insert_service_numeric
    OWNER TO karma_admin;
	
	
GRANT EXECUTE ON FUNCTION murr_downloader.insert_service_numeric(character varying, character varying, numeric) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_service_numeric(character varying, character varying, numeric) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.insert_service_numeric(character varying, character varying, numeric) FROM PUBLIC;


alter table murr_downloader.service_attribute_dates
add constraint u_service_attribute_dates_service_attribute_attribute_id unique(service_service_attribute_id);

create or replace function murr_downloader.insert_service_date(in_service_name varchar(255),
	in_service_attribute varchar(255),
	in_service_value timestamp without time zone)
	returns void
as $$	
declare
	d_service_id bigint = null;
	d_service_attriubte_id bigint = null;
	d_service_service_attribute_id bigint = null;
	d_hint text = null;
begin
	raise notice 'Start function insert_service_date';
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
	
	insert into murr_downloader.service_attribute_dates(service_service_attribute_id, service_service_attribute_date)
	values(d_service_service_attribute_id, in_service_value)
	on conflict (service_service_attribute_id) do update 
	set service_service_attribute_date = in_service_value;		
end
$$ language plpgsql
	SECURITY DEFINER;

ALTER FUNCTION murr_downloader.insert_service_date
    OWNER TO karma_admin;
	
	
GRANT EXECUTE ON FUNCTION murr_downloader.insert_service_date(character varying, character varying, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_service_date(character varying, character varying, timestamp without time zone) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.insert_service_date(character varying, character varying, timestamp without time zone) FROM PUBLIC;


--логирование 
alter table murr_downloader.service_attribute_date_strings
add constraint u_service_attribute_date_strings_id_string
unique(service_service_attribute_id, service_service_attribute_date);


create or replace function murr_downloader.insert_service_date_string(in_service_name varchar(255),
	in_service_attribute varchar(255),
	in_service_date timestamp without time zone,
	in_service_value varchar(255))
	returns void
as $$	
declare
	d_service_id bigint = null;
	d_service_attriubte_id bigint = null;
	d_service_service_attribute_id bigint = null;
	d_hint text = null;
begin
	raise notice 'Start function insert_service_date';
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
	
	insert into murr_downloader.service_attribute_date_strings(service_service_attribute_id,
		service_service_attribute_date,
		service_service_attribute_title)
		values(d_service_service_attribute_id, in_service_date, in_service_value);
		
	DELETE FROM murr_downloader.service_attribute_date_strings
		WHERE service_service_attribute_id = d_service_service_attribute_id
			and	service_service_attribute_date IN (
			SELECT service_service_attribute_date
				FROM murr_downloader.service_attribute_date_strings
				where service_service_attribute_id = d_service_service_attribute_id
				ORDER BY service_service_attribute_date desc
				offset 10);	
end
$$ language plpgsql
	SECURITY DEFINER;

ALTER FUNCTION murr_downloader.insert_service_date_string
    OWNER TO karma_admin;
	
GRANT EXECUTE ON FUNCTION murr_downloader.insert_service_date_string(character varying, character varying, timestamp without time zone, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_service_date_string(character varying, character varying, timestamp without time zone, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.insert_service_date_string(character varying, character varying, timestamp without time zone, character varying) FROM PUBLIC;