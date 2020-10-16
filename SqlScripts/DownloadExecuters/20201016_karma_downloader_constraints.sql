alter table murr_downloader.task_attribute_dates
add CONSTRAINT u_task_attribute_dates_task_task_attribute_id UNIQUE (task_task_attribute_id);

alter table murr_downloader.task_attribute_numerics
add constraint u_task_attribute_numerics_task_task_attribute_id unique(task_task_attribute_id);

alter table murr_downloader.task_attribute_strings
add constraint u_task_attribute_strings_task_task_attribute_id unique(task_task_attribute_id);


create or replace function murr_downloader.insert_task_date(
	in_task_id bigint,
	in_task_attribute character varying,
	in_task_value timestamp without time zone)
	returns void 
	language 'plpgsql'
	
	cost 100
	volatile security definer

as $BODY$
declare 
	d_task_attribute_id bigint;
	d_task_task_attribute_id bigint;
begin 
	select task_attribute_id into d_task_attribute_id 
	from murr_downloader.task_attributes
	where task_attribute_title = upper(in_task_attribute);
	
	select task_task_attribute_id into d_task_task_attribute_id
	from murr_downloader.tasks_task_attributes
	where task_id = in_task_id and task_attribute_id = d_task_attribute_id;
	
	if d_task_task_attribute_id is null
	then
		insert into murr_downloader.tasks_task_attributes(task_id, task_attribute_id)
		values(in_task_id, d_task_attribute_id)
		returning task_task_attribute_id into d_task_task_attribute_id;
	end if;
	
	insert into murr_downloader.task_attribute_dates(task_task_attribute_id, task_task_attribute_date)
	values(d_task_task_attribute_id, in_task_value)
	on conflict(task_task_attribute_id) do update
	set task_task_attribute_date = in_task_value;
end
$BODY$;

ALTER FUNCTION murr_downloader.insert_task_date(bigint, character varying, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task_date(bigint, character varying, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task_date(bigint, character varying, timestamp without time zone) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.insert_task_date(bigint, character varying, timestamp without time zone) FROM PUBLIC;


create or replace function murr_downloader.insert_task_string(
	in_task_id bigint,
	in_task_attribute character varying,
	in_task_value character varying)
	returns void 
	language 'plpgsql'
	
	cost 100
	volatile security definer

as $BODY$
declare 
	d_task_attribute_id bigint;
	d_task_task_attribute_id bigint;
begin 
	select task_attribute_id into d_task_attribute_id 
	from murr_downloader.task_attributes
	where task_attribute_title = upper(in_task_attribute);
	
	select task_task_attribute_id into d_task_task_attribute_id
	from murr_downloader.tasks_task_attributes
	where task_id = in_task_id and task_attribute_id = d_task_attribute_id;
	
	if d_task_task_attribute_id is null
	then
		insert into murr_downloader.tasks_task_attributes(task_id, task_attribute_id)
		values(in_task_id, d_task_attribute_id)
		returning task_task_attribute_id into d_task_task_attribute_id;
	end if;
	
	insert into murr_downloader.task_attribute_strings(task_task_attribute_id, task_task_attribute_title)
	values(d_task_task_attribute_id, in_task_value)
	on conflict(task_task_attribute_id) do update
	set task_task_attribute_title = in_task_value;
end
$BODY$;

ALTER FUNCTION murr_downloader.insert_task_string(bigint, character varying, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task_string(bigint, character varying, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task_string(bigint, character varying, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.insert_task_string(bigint, character varying, character varying) FROM PUBLIC;

create or replace function murr_downloader.insert_task_numeric(
	in_task_id bigint,
	in_task_attribute character varying,
	in_task_value numeric)
	returns void 
	language 'plpgsql'
	
	cost 100
	volatile security definer

as $BODY$
declare 
	d_task_attribute_id bigint;
	d_task_task_attribute_id bigint;
begin 
	select task_attribute_id into d_task_attribute_id 
	from murr_downloader.task_attributes
	where task_attribute_title = upper(in_task_attribute);
	
	select task_task_attribute_id into d_task_task_attribute_id
	from murr_downloader.tasks_task_attributes
	where task_id = in_task_id and task_attribute_id = d_task_attribute_id;
	
	if d_task_task_attribute_id is null
	then
		insert into murr_downloader.tasks_task_attributes(task_id, task_attribute_id)
		values(in_task_id, d_task_attribute_id)
		returning task_task_attribute_id into d_task_task_attribute_id;
	end if;
	
	insert into murr_downloader.task_attribute_numerics(task_task_attribute_id, task_task_attribute_numeric)
	values(d_task_task_attribute_id, in_task_value)
	on conflict(task_task_attribute_id) do update
	set task_task_attribute_numeric = in_task_value;
end
$BODY$;

ALTER FUNCTION murr_downloader.insert_task_numeric(bigint, character varying, numeric)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task_numeric(bigint, character varying, numeric) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task_numeric(bigint, character varying, numeric) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.insert_task_numeric(bigint, character varying, numeric) FROM PUBLIC;



create or replace function murr_downloader.insert_task_date_string(
	in_task_id bigint,
	in_task_attribute character varying,
	in_task_date timestamp without time zone,
	in_task_value character varying)
	returns void 
	language 'plpgsql'
	
	cost 100
	volatile security definer

as $BODY$
declare 
	d_task_attribute_id bigint;
	d_task_task_attribute_id bigint;
begin 
	select task_attribute_id into d_task_attribute_id 
	from murr_downloader.task_attributes
	where task_attribute_title = upper(in_task_attribute);
	
	select task_task_attribute_id into d_task_task_attribute_id
	from murr_downloader.tasks_task_attributes
	where task_id = in_task_id and task_attribute_id = d_task_attribute_id;
	
	if d_task_task_attribute_id is null
	then
		insert into murr_downloader.tasks_task_attributes(task_id, task_attribute_id)
		values(in_task_id, d_task_attribute_id)
		returning task_task_attribute_id into d_task_task_attribute_id;
	end if;
	
	
	insert into murr_downloader.task_attribute_date_strings(task_task_attribute_id,
		task_task_attribute_date,
		task_task_attribute_title)
		values(d_task_task_attribute_id, in_task_date, in_task_value);
		
	DELETE FROM murr_downloader.task_attribute_date_strings
		WHERE task_task_attribute_id = d_task_task_attribute_id
			and	task_task_attribute_date IN (
			SELECT task_task_attribute_date
				FROM murr_downloader.task_attribute_date_strings
				where task_task_attribute_id = d_task_task_attribute_id
				ORDER BY task_task_attribute_date desc
				offset 10);	
				
end
$BODY$;

ALTER FUNCTION murr_downloader.insert_task_date_string(bigint, character varying, timestamp without time zone, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task_date_string(bigint, character varying, timestamp without time zone, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.insert_task_date_string(bigint, character varying, timestamp without time zone, character varying) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.insert_task_date_string(bigint, character varying, timestamp without time zone, character varying) FROM PUBLIC;
