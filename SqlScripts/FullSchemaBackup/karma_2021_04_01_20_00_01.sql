--
-- PostgreSQL database dump
--

-- Dumped from database version 12.3
-- Dumped by pg_dump version 12.3

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: hangfire; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA hangfire;


ALTER SCHEMA hangfire OWNER TO postgres;

--
-- Name: murr_data; Type: SCHEMA; Schema: -; Owner: karma_admin
--

CREATE SCHEMA murr_data;


ALTER SCHEMA murr_data OWNER TO karma_admin;

--
-- Name: murr_downloader; Type: SCHEMA; Schema: -; Owner: karma_admin
--

CREATE SCHEMA murr_downloader;


ALTER SCHEMA murr_downloader OWNER TO karma_admin;

--
-- Name: add_cbr_foreign_exchange(timestamp without time zone); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.add_cbr_foreign_exchange(in_datetime timestamp without time zone) RETURNS bigint
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$	
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
	values(d_task_tempate_id, now()::timestamp without time zone, 2, d_saver_template_id)
	returning task_id into d_task_id;
	
	perform murr_downloader.insert_task_numeric(d_task_id, 'ATTEMPTIONS', 1);	
	
	return d_task_id;
end
$$;


ALTER FUNCTION murr_downloader.add_cbr_foreign_exchange(in_datetime timestamp without time zone) OWNER TO karma_admin;

--
-- Name: add_service(character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.add_service(in_service_name character varying) RETURNS bigint
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare 
	d_service_id bigint;
begin
	insert into murr_downloader.services(service_title, service_status_id)
	values(in_service_name, 0)
	returning service_id into d_service_id;
	return d_service_id;
end
$$;


ALTER FUNCTION murr_downloader.add_service(in_service_name character varying) OWNER TO karma_admin;

--
-- Name: change_procedure_task(bigint, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.change_procedure_task(in_procedure_task_id bigint, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare
	d_procedure_task_id bigint;
begin
	insert into murr_downloader.procedure_tasks_history(procedure_task_id, procedure_last_run, procedure_next_run)
	values(in_procedure_task_id, in_procedure_last_run, in_procedure_next_run);
	
	update murr_downloader.procedure_tasks
	set procedure_last_run = in_procedure_last_run, procedure_next_run = in_procedure_next_run
	where procedure_task_id = in_procedure_task_id;
end
$$;


ALTER FUNCTION murr_downloader.change_procedure_task(in_procedure_task_id bigint, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone) OWNER TO karma_admin;

--
-- Name: change_task_status(bigint, bigint, bigint); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.change_task_status(in_task_id bigint, in_old_task_status bigint, in_new_task_status bigint) RETURNS bigint
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare 
	d_amount bigint;
begin
	update murr_downloader.tasks
	set task_status_id = in_new_task_status
	where task_status_id = in_old_task_status and task_id = in_task_id;
	
	GET DIAGNOSTICS d_amount = ROW_COUNT;

        if d_amount > 0 
		then
           return 1;
		else
			return 0;
        end if;
	
	return d_amount;
end
$$;


ALTER FUNCTION murr_downloader.change_task_status(in_task_id bigint, in_old_task_status bigint, in_new_task_status bigint) OWNER TO karma_admin;

--
-- Name: get_inner_procedures(); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_inner_procedures() RETURNS TABLE(procedure_schema character varying, procedure_name_second character varying, procedure_name character varying, external_language character varying, pn character varying, pm character varying, data_type character varying)
    LANGUAGE plpgsql
    AS $$
declare 
begin 
	return query 
	select (proc.specific_schema::character varying) as procedure_schema,
       (proc.specific_name::character varying) as procedure_name_second,
       (proc.routine_name::character varying) as procedure_name,
       (proc.external_language::character varying) as external_language,
       (args.parameter_name::character varying) as pn,
       (args.parameter_mode::character varying) as pm,
       (args.data_type::character varying) as data_type
	from information_schema.routines proc
	left join information_schema.parameters args
          on proc.specific_schema = args.specific_schema
          and proc.specific_name = args.specific_name
	where proc.routine_schema in ('murr_downloader')
	order by procedure_schema,
         procedure_name_second,
         procedure_name,
         args.ordinal_position;
end
$$;


ALTER FUNCTION murr_downloader.get_inner_procedures() OWNER TO karma_admin;

--
-- Name: get_jobs(); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_jobs() RETURNS TABLE(task_id bigint, task_template_id bigint, task_status_id bigint, saver_template_id bigint)
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$	
declare
begin
	return query
		select tasks.task_id, tasks.task_template_id, tasks.task_status_id, tasks.saver_template_id
		from murr_downloader.tasks;
end
$$;


ALTER FUNCTION murr_downloader.get_jobs() OWNER TO karma_admin;

--
-- Name: get_procedure_tasks(); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_procedure_tasks() RETURNS TABLE(procedure_task_id bigint, procedure_title character varying, procedure_is_use boolean, procedure_params jsonb, procedure_template character varying, procedure_last_run timestamp without time zone, procedure_next_run timestamp without time zone)
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare 
begin 
	return query 
		select procedure_tasks.procedure_task_id, procedure_tasks.procedure_title, procedure_tasks.procedure_is_use, 
			procedure_tasks.procedure_params, procedure_tasks.procedure_template, 
			procedure_tasks.procedure_last_run, procedure_tasks.procedure_next_run
		from murr_downloader.procedure_tasks;
end
$$;


ALTER FUNCTION murr_downloader.get_procedure_tasks() OWNER TO karma_admin;

--
-- Name: get_service_date(character varying, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_service_date(in_service_name character varying, in_attribute character varying) RETURNS timestamp without time zone
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare 
	d_title timestamp without time zone;
begin 
	select sad.service_service_attribute_date into d_title
		from murr_downloader.services s,
		murr_downloader.service_attributes sa,
		murr_downloader.services_service_attributes ssa,
		murr_downloader.service_attribute_dates sad
		where s.service_id = ssa.service_id and 
			ssa.service_attribute_id = sa.service_attribute_id and
			sad.service_service_attribute_id = ssa.service_service_attribute_id and
			s.service_title = in_service_name and
			sa.service_attribute_title = in_attribute;
	return d_title;
end 
$$;


ALTER FUNCTION murr_downloader.get_service_date(in_service_name character varying, in_attribute character varying) OWNER TO karma_admin;

--
-- Name: get_service_numeric(character varying, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_service_numeric(in_service_name character varying, in_attribute character varying) RETURNS numeric
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare 
	d_title numeric;
begin 
	select sad.service_service_attribute_numeric into d_title
		from murr_downloader.services s,
		murr_downloader.service_attributes sa,
		murr_downloader.services_service_attributes ssa,
		murr_downloader.service_attribute_numerics sad
		where s.service_id = ssa.service_id and 
			ssa.service_attribute_id = sa.service_attribute_id and
			sad.service_service_attribute_id = ssa.service_service_attribute_id and
			s.service_title = in_service_name and
			sa.service_attribute_title = in_attribute;
	return d_title;
end 
$$;


ALTER FUNCTION murr_downloader.get_service_numeric(in_service_name character varying, in_attribute character varying) OWNER TO karma_admin;

--
-- Name: get_service_string(character varying, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_service_string(in_service_name character varying, in_attribute character varying) RETURNS character varying
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare 
	d_title character varying;
begin 
	select sad.service_service_attribute_title into d_title
		from murr_downloader.services s,
		murr_downloader.service_attributes sa,
		murr_downloader.services_service_attributes ssa,
		murr_downloader.service_attribute_strings sad
		where s.service_id = ssa.service_id and 
			ssa.service_attribute_id = sa.service_attribute_id and
			sad.service_service_attribute_id = ssa.service_service_attribute_id and
			s.service_title = in_service_name and
			sa.service_attribute_title = in_attribute;
	return d_title;
end 
$$;


ALTER FUNCTION murr_downloader.get_service_string(in_service_name character varying, in_attribute character varying) OWNER TO karma_admin;

--
-- Name: get_task_date(bigint, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_task_date(in_task_id bigint, in_attribute character varying) RETURNS timestamp without time zone
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare 
	d_title timestamp without time zone;
begin 
	select tad.task_task_attribute_date into d_title
	from murr_downloader.tasks t,
	murr_downloader.task_attributes ta,
	murr_downloader.tasks_task_attributes tta,
	murr_downloader.task_attribute_dates tad
	where t.task_id = tta.task_id and
	ta.task_attribute_id = tta.task_attribute_id and
	tta.task_task_attribute_id = tad.task_task_attribute_id and
	t.task_id = in_task_id and
	ta.task_attribute_title = in_attribute;
	return d_title;
end 
$$;


ALTER FUNCTION murr_downloader.get_task_date(in_task_id bigint, in_attribute character varying) OWNER TO karma_admin;

--
-- Name: get_task_numeric(bigint, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_task_numeric(in_task_id bigint, in_attribute character varying) RETURNS character varying
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare 
	d_title numeric;
begin 
	select tan.task_task_attribute_numeric into d_title
	from murr_downloader.tasks t,
	murr_downloader.task_attributes ta,
	murr_downloader.tasks_task_attributes tta,
	murr_downloader.task_attribute_numerics tan
	where t.task_id = tta.task_id and
	ta.task_attribute_id = tta.task_attribute_id and
	tta.task_task_attribute_id = tan.task_task_attribute_id and
	t.task_id = in_task_id and
	ta.task_attribute_title = in_attribute;
	return d_title;
end 
$$;


ALTER FUNCTION murr_downloader.get_task_numeric(in_task_id bigint, in_attribute character varying) OWNER TO karma_admin;

--
-- Name: get_task_string(bigint, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_task_string(in_task_id bigint, in_attribute character varying) RETURNS character varying
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare 
	d_title character varying;
begin 
	select tas.task_task_attribute_title into d_title
	from murr_downloader.tasks t,
	murr_downloader.task_attributes ta,
	murr_downloader.tasks_task_attributes tta,
	murr_downloader.task_attribute_strings tas
	where t.task_id = tta.task_id and
	ta.task_attribute_id = tta.task_attribute_id and
	tta.task_task_attribute_id = tas.task_task_attribute_id and
	t.task_id = in_task_id and
	ta.task_attribute_title = in_attribute;
	return d_title;
end 
$$;


ALTER FUNCTION murr_downloader.get_task_string(in_task_id bigint, in_attribute character varying) OWNER TO karma_admin;

--
-- Name: insert_procedure_task(character varying, boolean, jsonb, character varying, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.insert_procedure_task(in_procedure_title character varying, in_procedure_is_use boolean, in_procedure_params jsonb, in_procedure_template character varying, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone) RETURNS bigint
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
declare
	d_procedure_task_id bigint;
begin
	insert into murr_downloader.procedure_tasks(procedure_title, procedure_is_use, procedure_params,
		procedure_template, procedure_last_run, procedure_next_run)
	values(in_procedure_title, in_procedure_is_use, in_procedure_params, in_procedure_template, in_procedure_last_run, in_procedure_next_run)
	returning procedure_task_id into d_procedure_task_id;
	return d_procedure_task_id;
end
$$;


ALTER FUNCTION murr_downloader.insert_procedure_task(in_procedure_title character varying, in_procedure_is_use boolean, in_procedure_params jsonb, in_procedure_template character varying, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone) OWNER TO karma_admin;

--
-- Name: insert_service_date(character varying, character varying, timestamp without time zone); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.insert_service_date(in_service_name character varying, in_service_attribute character varying, in_service_value timestamp without time zone) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$	
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
$$;


ALTER FUNCTION murr_downloader.insert_service_date(in_service_name character varying, in_service_attribute character varying, in_service_value timestamp without time zone) OWNER TO karma_admin;

--
-- Name: insert_service_date_string(character varying, character varying, timestamp without time zone, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.insert_service_date_string(in_service_name character varying, in_service_attribute character varying, in_service_date timestamp without time zone, in_service_value character varying) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$	
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
$$;


ALTER FUNCTION murr_downloader.insert_service_date_string(in_service_name character varying, in_service_attribute character varying, in_service_date timestamp without time zone, in_service_value character varying) OWNER TO karma_admin;

--
-- Name: insert_service_numeric(character varying, character varying, numeric); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.insert_service_numeric(in_service_name character varying, in_service_attribute character varying, in_service_value numeric) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$	
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
$$;


ALTER FUNCTION murr_downloader.insert_service_numeric(in_service_name character varying, in_service_attribute character varying, in_service_value numeric) OWNER TO karma_admin;

--
-- Name: insert_service_string(character varying, character varying, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.insert_service_string(in_service_name character varying, in_service_attribute character varying, in_service_value character varying) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$	
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
$$;


ALTER FUNCTION murr_downloader.insert_service_string(in_service_name character varying, in_service_attribute character varying, in_service_value character varying) OWNER TO karma_admin;

--
-- Name: insert_task_date(bigint, character varying, timestamp without time zone); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.insert_task_date(in_task_id bigint, in_task_attribute character varying, in_task_value timestamp without time zone) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
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
$$;


ALTER FUNCTION murr_downloader.insert_task_date(in_task_id bigint, in_task_attribute character varying, in_task_value timestamp without time zone) OWNER TO karma_admin;

--
-- Name: insert_task_date_string(bigint, character varying, timestamp without time zone, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.insert_task_date_string(in_task_id bigint, in_task_attribute character varying, in_task_date timestamp without time zone, in_task_value character varying) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
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
$$;


ALTER FUNCTION murr_downloader.insert_task_date_string(in_task_id bigint, in_task_attribute character varying, in_task_date timestamp without time zone, in_task_value character varying) OWNER TO karma_admin;

--
-- Name: insert_task_numeric(bigint, character varying, numeric); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.insert_task_numeric(in_task_id bigint, in_task_attribute character varying, in_task_value numeric) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
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
$$;


ALTER FUNCTION murr_downloader.insert_task_numeric(in_task_id bigint, in_task_attribute character varying, in_task_value numeric) OWNER TO karma_admin;

--
-- Name: insert_task_string(bigint, character varying, character varying); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.insert_task_string(in_task_id bigint, in_task_attribute character varying, in_task_value character varying) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
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
$$;


ALTER FUNCTION murr_downloader.insert_task_string(in_task_id bigint, in_task_attribute character varying, in_task_value character varying) OWNER TO karma_admin;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: counter; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.counter (
    id bigint NOT NULL,
    key text NOT NULL,
    value bigint NOT NULL,
    expireat timestamp without time zone
);


ALTER TABLE hangfire.counter OWNER TO karma_admin;

--
-- Name: counter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: karma_admin
--

CREATE SEQUENCE hangfire.counter_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE hangfire.counter_id_seq OWNER TO karma_admin;

--
-- Name: counter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: karma_admin
--

ALTER SEQUENCE hangfire.counter_id_seq OWNED BY hangfire.counter.id;


--
-- Name: hash; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.hash (
    id bigint NOT NULL,
    key text NOT NULL,
    field text NOT NULL,
    value text,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.hash OWNER TO karma_admin;

--
-- Name: hash_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: karma_admin
--

CREATE SEQUENCE hangfire.hash_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE hangfire.hash_id_seq OWNER TO karma_admin;

--
-- Name: hash_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: karma_admin
--

ALTER SEQUENCE hangfire.hash_id_seq OWNED BY hangfire.hash.id;


--
-- Name: job; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.job (
    id bigint NOT NULL,
    stateid bigint,
    statename text,
    invocationdata text NOT NULL,
    arguments text NOT NULL,
    createdat timestamp without time zone NOT NULL,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.job OWNER TO karma_admin;

--
-- Name: job_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: karma_admin
--

CREATE SEQUENCE hangfire.job_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE hangfire.job_id_seq OWNER TO karma_admin;

--
-- Name: job_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: karma_admin
--

ALTER SEQUENCE hangfire.job_id_seq OWNED BY hangfire.job.id;


--
-- Name: jobparameter; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.jobparameter (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    value text,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.jobparameter OWNER TO karma_admin;

--
-- Name: jobparameter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: karma_admin
--

CREATE SEQUENCE hangfire.jobparameter_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE hangfire.jobparameter_id_seq OWNER TO karma_admin;

--
-- Name: jobparameter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: karma_admin
--

ALTER SEQUENCE hangfire.jobparameter_id_seq OWNED BY hangfire.jobparameter.id;


--
-- Name: jobqueue; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.jobqueue (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    queue text NOT NULL,
    fetchedat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.jobqueue OWNER TO karma_admin;

--
-- Name: jobqueue_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: karma_admin
--

CREATE SEQUENCE hangfire.jobqueue_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE hangfire.jobqueue_id_seq OWNER TO karma_admin;

--
-- Name: jobqueue_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: karma_admin
--

ALTER SEQUENCE hangfire.jobqueue_id_seq OWNED BY hangfire.jobqueue.id;


--
-- Name: list; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.list (
    id bigint NOT NULL,
    key text NOT NULL,
    value text,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.list OWNER TO karma_admin;

--
-- Name: list_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: karma_admin
--

CREATE SEQUENCE hangfire.list_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE hangfire.list_id_seq OWNER TO karma_admin;

--
-- Name: list_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: karma_admin
--

ALTER SEQUENCE hangfire.list_id_seq OWNED BY hangfire.list.id;


--
-- Name: lock; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.lock (
    resource text NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL,
    acquired timestamp without time zone
);


ALTER TABLE hangfire.lock OWNER TO karma_admin;

--
-- Name: schema; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.schema (
    version integer NOT NULL
);


ALTER TABLE hangfire.schema OWNER TO karma_admin;

--
-- Name: server; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.server (
    id text NOT NULL,
    data text,
    lastheartbeat timestamp without time zone NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.server OWNER TO karma_admin;

--
-- Name: set; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.set (
    id bigint NOT NULL,
    key text NOT NULL,
    score double precision NOT NULL,
    value text NOT NULL,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.set OWNER TO karma_admin;

--
-- Name: set_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: karma_admin
--

CREATE SEQUENCE hangfire.set_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE hangfire.set_id_seq OWNER TO karma_admin;

--
-- Name: set_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: karma_admin
--

ALTER SEQUENCE hangfire.set_id_seq OWNED BY hangfire.set.id;


--
-- Name: state; Type: TABLE; Schema: hangfire; Owner: karma_admin
--

CREATE TABLE hangfire.state (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    reason text,
    createdat timestamp without time zone NOT NULL,
    data text,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.state OWNER TO karma_admin;

--
-- Name: state_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: karma_admin
--

CREATE SEQUENCE hangfire.state_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE hangfire.state_id_seq OWNER TO karma_admin;

--
-- Name: state_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: karma_admin
--

ALTER SEQUENCE hangfire.state_id_seq OWNED BY hangfire.state.id;


--
-- Name: murr_sequence; Type: SEQUENCE; Schema: murr_downloader; Owner: karma_admin
--

CREATE SEQUENCE murr_downloader.murr_sequence
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE murr_downloader.murr_sequence OWNER TO karma_admin;

--
-- Name: default_saver_templates; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.default_saver_templates (
    default_saver_template_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    task_type_id bigint NOT NULL,
    default_path character varying(2047) NOT NULL
);


ALTER TABLE murr_downloader.default_saver_templates OWNER TO karma_admin;

--
-- Name: TABLE default_saver_templates; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.default_saver_templates IS 'Пути по умолчанию к задачам загрузки';


--
-- Name: folder_types; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.folder_types (
    folder_type_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    folder_type_title character varying(255) NOT NULL,
    folder_type_description character varying(1023) NOT NULL
);


ALTER TABLE murr_downloader.folder_types OWNER TO karma_admin;

--
-- Name: TABLE folder_types; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.folder_types IS 'Типы папок';


--
-- Name: folders; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.folders (
    folder_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    folder_root_id bigint,
    folder_title character varying(255),
    folder_type_id bigint NOT NULL
);


ALTER TABLE murr_downloader.folders OWNER TO karma_admin;

--
-- Name: TABLE folders; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.folders IS 'Папки';


--
-- Name: procedure_tasks; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.procedure_tasks (
    procedure_task_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    procedure_title character varying(255) NOT NULL,
    procedure_is_use boolean NOT NULL,
    procedure_params jsonb NOT NULL,
    procedure_template character varying(255) NOT NULL,
    procedure_last_run timestamp without time zone,
    procedure_next_run timestamp without time zone
);


ALTER TABLE murr_downloader.procedure_tasks OWNER TO karma_admin;

--
-- Name: TABLE procedure_tasks; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.procedure_tasks IS 'Запуск процедур по требованию';


--
-- Name: procedure_tasks_history; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.procedure_tasks_history (
    procedure_task_id bigint NOT NULL,
    procedure_last_run timestamp without time zone,
    procedure_next_run timestamp without time zone
);


ALTER TABLE murr_downloader.procedure_tasks_history OWNER TO karma_admin;

--
-- Name: saver_templates; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.saver_templates (
    saver_template_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    saver_template_title character varying(255) NOT NULL,
    saver_template_created_time timestamp without time zone NOT NULL,
    saver_template_folder_id bigint NOT NULL,
    saver_parameters jsonb NOT NULL,
    saver_type_id bigint NOT NULL
);


ALTER TABLE murr_downloader.saver_templates OWNER TO karma_admin;

--
-- Name: TABLE saver_templates; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.saver_templates IS 'Шаблоны для сохранений';


--
-- Name: saver_types; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.saver_types (
    saver_type_id bigint NOT NULL,
    saver_type_title character varying(255) NOT NULL,
    saver_type_description character varying(1023) NOT NULL
);


ALTER TABLE murr_downloader.saver_types OWNER TO karma_admin;

--
-- Name: TABLE saver_types; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.saver_types IS 'Типы сохранений';


--
-- Name: service_attribute_date_strings; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.service_attribute_date_strings (
    service_service_attribute_id bigint NOT NULL,
    service_service_attribute_date timestamp without time zone NOT NULL,
    service_service_attribute_title character varying(255) NOT NULL
);


ALTER TABLE murr_downloader.service_attribute_date_strings OWNER TO karma_admin;

--
-- Name: TABLE service_attribute_date_strings; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.service_attribute_date_strings IS 'Значения для даты и времени для сервисов';


--
-- Name: service_attribute_dates; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.service_attribute_dates (
    service_service_attribute_id bigint NOT NULL,
    service_service_attribute_date timestamp without time zone NOT NULL
);


ALTER TABLE murr_downloader.service_attribute_dates OWNER TO karma_admin;

--
-- Name: TABLE service_attribute_dates; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.service_attribute_dates IS 'Значения времени для сервисов';


--
-- Name: service_attribute_numerics; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.service_attribute_numerics (
    service_service_attribute_id bigint NOT NULL,
    service_service_attribute_numeric numeric NOT NULL
);


ALTER TABLE murr_downloader.service_attribute_numerics OWNER TO karma_admin;

--
-- Name: TABLE service_attribute_numerics; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.service_attribute_numerics IS 'Числовые значения для сервисов';


--
-- Name: service_attribute_strings; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.service_attribute_strings (
    service_service_attribute_id bigint NOT NULL,
    service_service_attribute_title character varying(255) NOT NULL
);


ALTER TABLE murr_downloader.service_attribute_strings OWNER TO karma_admin;

--
-- Name: TABLE service_attribute_strings; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.service_attribute_strings IS 'Строковые значения для сервисов';


--
-- Name: service_attributes; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.service_attributes (
    service_attribute_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    service_attribute_title character varying(255) NOT NULL,
    service_attribute_description character varying(1023) NOT NULL
);


ALTER TABLE murr_downloader.service_attributes OWNER TO karma_admin;

--
-- Name: TABLE service_attributes; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.service_attributes IS 'Атрибуты для сервисов';


--
-- Name: service_statuses; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.service_statuses (
    service_status_id bigint NOT NULL,
    service_status_title character varying(255) NOT NULL,
    service_status_description character varying(1023) NOT NULL
);


ALTER TABLE murr_downloader.service_statuses OWNER TO karma_admin;

--
-- Name: TABLE service_statuses; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.service_statuses IS 'Статусы сервисов';


--
-- Name: services; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.services (
    service_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    service_title character varying(255) NOT NULL,
    service_status_id bigint NOT NULL
);


ALTER TABLE murr_downloader.services OWNER TO karma_admin;

--
-- Name: TABLE services; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.services IS 'Сервисы';


--
-- Name: services_service_attributes; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.services_service_attributes (
    service_service_attribute_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    service_id bigint,
    service_attribute_id bigint
);


ALTER TABLE murr_downloader.services_service_attributes OWNER TO karma_admin;

--
-- Name: TABLE services_service_attributes; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.services_service_attributes IS 'Таблица для генерирования уникального ключа для сервисов и атрибутов для сервисов';


--
-- Name: services_dates; Type: VIEW; Schema: murr_downloader; Owner: karma_admin
--

CREATE VIEW murr_downloader.services_dates AS
 SELECT s.service_id,
    s.service_title,
    sa.service_attribute_title,
    sad.service_service_attribute_date
   FROM murr_downloader.services s,
    murr_downloader.service_attributes sa,
    murr_downloader.services_service_attributes ssa,
    murr_downloader.service_attribute_dates sad
  WHERE ((s.service_id = ssa.service_id) AND (ssa.service_attribute_id = sa.service_attribute_id) AND (sad.service_service_attribute_id = ssa.service_service_attribute_id));


ALTER TABLE murr_downloader.services_dates OWNER TO karma_admin;

--
-- Name: services_logs; Type: VIEW; Schema: murr_downloader; Owner: karma_admin
--

CREATE VIEW murr_downloader.services_logs AS
 SELECT s.service_id,
    s.service_title,
    sa.service_attribute_title,
    sad.service_service_attribute_date AS date,
    sad.service_service_attribute_title AS title
   FROM murr_downloader.services s,
    murr_downloader.service_attributes sa,
    murr_downloader.services_service_attributes ssa,
    murr_downloader.service_attribute_date_strings sad
  WHERE ((s.service_id = ssa.service_id) AND (ssa.service_attribute_id = sa.service_attribute_id) AND (sad.service_service_attribute_id = ssa.service_service_attribute_id));


ALTER TABLE murr_downloader.services_logs OWNER TO karma_admin;

--
-- Name: services_numerics; Type: VIEW; Schema: murr_downloader; Owner: karma_admin
--

CREATE VIEW murr_downloader.services_numerics AS
 SELECT s.service_id,
    s.service_title,
    sa.service_attribute_title,
    sad.service_service_attribute_numeric
   FROM murr_downloader.services s,
    murr_downloader.service_attributes sa,
    murr_downloader.services_service_attributes ssa,
    murr_downloader.service_attribute_numerics sad
  WHERE ((s.service_id = ssa.service_id) AND (ssa.service_attribute_id = sa.service_attribute_id) AND (sad.service_service_attribute_id = ssa.service_service_attribute_id));


ALTER TABLE murr_downloader.services_numerics OWNER TO karma_admin;

--
-- Name: services_strings; Type: VIEW; Schema: murr_downloader; Owner: karma_admin
--

CREATE VIEW murr_downloader.services_strings AS
 SELECT s.service_id,
    s.service_title,
    sa.service_attribute_title,
    sad.service_service_attribute_title
   FROM murr_downloader.services s,
    murr_downloader.service_attributes sa,
    murr_downloader.services_service_attributes ssa,
    murr_downloader.service_attribute_strings sad
  WHERE ((s.service_id = ssa.service_id) AND (ssa.service_attribute_id = sa.service_attribute_id) AND (sad.service_service_attribute_id = ssa.service_service_attribute_id));


ALTER TABLE murr_downloader.services_strings OWNER TO karma_admin;

--
-- Name: task_attribute_date_strings; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.task_attribute_date_strings (
    task_task_attribute_id bigint NOT NULL,
    task_task_attribute_date timestamp without time zone NOT NULL,
    task_task_attribute_title character varying(255) NOT NULL
);


ALTER TABLE murr_downloader.task_attribute_date_strings OWNER TO karma_admin;

--
-- Name: TABLE task_attribute_date_strings; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.task_attribute_date_strings IS 'Значения для даты и времени для задач';


--
-- Name: task_attribute_dates; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.task_attribute_dates (
    task_task_attribute_id bigint NOT NULL,
    task_task_attribute_date timestamp without time zone NOT NULL
);


ALTER TABLE murr_downloader.task_attribute_dates OWNER TO karma_admin;

--
-- Name: TABLE task_attribute_dates; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.task_attribute_dates IS 'Значения времени для задач';


--
-- Name: task_attribute_numerics; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.task_attribute_numerics (
    task_task_attribute_id bigint NOT NULL,
    task_task_attribute_numeric numeric NOT NULL
);


ALTER TABLE murr_downloader.task_attribute_numerics OWNER TO karma_admin;

--
-- Name: TABLE task_attribute_numerics; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.task_attribute_numerics IS 'Числовые значения для задач';


--
-- Name: task_attribute_strings; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.task_attribute_strings (
    task_task_attribute_id bigint NOT NULL,
    task_task_attribute_title character varying(255) NOT NULL
);


ALTER TABLE murr_downloader.task_attribute_strings OWNER TO karma_admin;

--
-- Name: TABLE task_attribute_strings; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.task_attribute_strings IS 'Строковые значения для задач';


--
-- Name: task_attributes; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.task_attributes (
    task_attribute_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    task_attribute_title character varying(255) NOT NULL,
    task_attribute_description character varying(1023) NOT NULL
);


ALTER TABLE murr_downloader.task_attributes OWNER TO karma_admin;

--
-- Name: TABLE task_attributes; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.task_attributes IS 'Атрибуты для задач';


--
-- Name: task_statuses; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.task_statuses (
    task_status_id bigint NOT NULL,
    task_status_title character varying(255) NOT NULL,
    task_statuses_description character varying(1023) NOT NULL
);


ALTER TABLE murr_downloader.task_statuses OWNER TO karma_admin;

--
-- Name: TABLE task_statuses; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.task_statuses IS 'Статусы для задач';


--
-- Name: task_templates; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.task_templates (
    task_template_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass),
    task_template_title character varying(255) NOT NULL,
    task_template_created_time timestamp without time zone NOT NULL,
    task_template_folder_id bigint NOT NULL,
    task_parameters jsonb NOT NULL,
    task_type_id bigint NOT NULL
);


ALTER TABLE murr_downloader.task_templates OWNER TO karma_admin;

--
-- Name: TABLE task_templates; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.task_templates IS 'Шаблоны задач';


--
-- Name: task_types; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.task_types (
    task_type_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    task_type_title character varying(255) NOT NULL,
    task_type_description character varying(1023) NOT NULL
);


ALTER TABLE murr_downloader.task_types OWNER TO karma_admin;

--
-- Name: TABLE task_types; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.task_types IS 'Типы задач';


--
-- Name: tasks; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.tasks (
    task_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    task_template_id bigint,
    task_created_time timestamp without time zone NOT NULL,
    task_status_id bigint,
    saver_template_id bigint
);


ALTER TABLE murr_downloader.tasks OWNER TO karma_admin;

--
-- Name: TABLE tasks; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.tasks IS 'Задачи, которые необходимо выполнить';


--
-- Name: tasks_task_attributes; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.tasks_task_attributes (
    task_task_attribute_id bigint DEFAULT nextval('murr_downloader.murr_sequence'::regclass) NOT NULL,
    task_id bigint,
    task_attribute_id bigint
);


ALTER TABLE murr_downloader.tasks_task_attributes OWNER TO karma_admin;

--
-- Name: TABLE tasks_task_attributes; Type: COMMENT; Schema: murr_downloader; Owner: karma_admin
--

COMMENT ON TABLE murr_downloader.tasks_task_attributes IS 'Таблица для генерирования уникального ключа для задач и атрибутов для задач';


--
-- Name: tasks_dates; Type: VIEW; Schema: murr_downloader; Owner: karma_admin
--

CREATE VIEW murr_downloader.tasks_dates AS
 SELECT t.task_id,
    ta.task_attribute_title,
    tad.task_task_attribute_date
   FROM murr_downloader.tasks t,
    murr_downloader.task_attributes ta,
    murr_downloader.tasks_task_attributes tta,
    murr_downloader.task_attribute_dates tad
  WHERE ((t.task_id = tta.task_id) AND (ta.task_attribute_id = tta.task_attribute_id) AND (tta.task_task_attribute_id = tad.task_task_attribute_id));


ALTER TABLE murr_downloader.tasks_dates OWNER TO karma_admin;

--
-- Name: tasks_numerics; Type: VIEW; Schema: murr_downloader; Owner: karma_admin
--

CREATE VIEW murr_downloader.tasks_numerics AS
 SELECT t.task_id,
    ta.task_attribute_title,
    tan.task_task_attribute_numeric
   FROM murr_downloader.tasks t,
    murr_downloader.task_attributes ta,
    murr_downloader.tasks_task_attributes tta,
    murr_downloader.task_attribute_numerics tan
  WHERE ((t.task_id = tta.task_id) AND (ta.task_attribute_id = tta.task_attribute_id) AND (tta.task_task_attribute_id = tan.task_task_attribute_id));


ALTER TABLE murr_downloader.tasks_numerics OWNER TO karma_admin;

--
-- Name: tasks_strings; Type: VIEW; Schema: murr_downloader; Owner: karma_admin
--

CREATE VIEW murr_downloader.tasks_strings AS
 SELECT t.task_id,
    ta.task_attribute_title,
    tas.task_task_attribute_title
   FROM murr_downloader.tasks t,
    murr_downloader.task_attributes ta,
    murr_downloader.tasks_task_attributes tta,
    murr_downloader.task_attribute_strings tas
  WHERE ((t.task_id = tta.task_id) AND (ta.task_attribute_id = tta.task_attribute_id) AND (tta.task_task_attribute_id = tas.task_task_attribute_id));


ALTER TABLE murr_downloader.tasks_strings OWNER TO karma_admin;

--
-- Name: counter id; Type: DEFAULT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.counter ALTER COLUMN id SET DEFAULT nextval('hangfire.counter_id_seq'::regclass);


--
-- Name: hash id; Type: DEFAULT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.hash ALTER COLUMN id SET DEFAULT nextval('hangfire.hash_id_seq'::regclass);


--
-- Name: job id; Type: DEFAULT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.job ALTER COLUMN id SET DEFAULT nextval('hangfire.job_id_seq'::regclass);


--
-- Name: jobparameter id; Type: DEFAULT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.jobparameter ALTER COLUMN id SET DEFAULT nextval('hangfire.jobparameter_id_seq'::regclass);


--
-- Name: jobqueue id; Type: DEFAULT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.jobqueue ALTER COLUMN id SET DEFAULT nextval('hangfire.jobqueue_id_seq'::regclass);


--
-- Name: list id; Type: DEFAULT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.list ALTER COLUMN id SET DEFAULT nextval('hangfire.list_id_seq'::regclass);


--
-- Name: set id; Type: DEFAULT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.set ALTER COLUMN id SET DEFAULT nextval('hangfire.set_id_seq'::regclass);


--
-- Name: state id; Type: DEFAULT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.state ALTER COLUMN id SET DEFAULT nextval('hangfire.state_id_seq'::regclass);


--
-- Name: counter counter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.counter
    ADD CONSTRAINT counter_pkey PRIMARY KEY (id);


--
-- Name: hash hash_key_field_key; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_key_field_key UNIQUE (key, field);


--
-- Name: hash hash_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_pkey PRIMARY KEY (id);


--
-- Name: job job_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);


--
-- Name: jobparameter jobparameter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_pkey PRIMARY KEY (id);


--
-- Name: jobqueue jobqueue_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.jobqueue
    ADD CONSTRAINT jobqueue_pkey PRIMARY KEY (id);


--
-- Name: list list_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.list
    ADD CONSTRAINT list_pkey PRIMARY KEY (id);


--
-- Name: lock lock_resource_key; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.lock
    ADD CONSTRAINT lock_resource_key UNIQUE (resource);


--
-- Name: schema schema_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.schema
    ADD CONSTRAINT schema_pkey PRIMARY KEY (version);


--
-- Name: server server_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.server
    ADD CONSTRAINT server_pkey PRIMARY KEY (id);


--
-- Name: set set_key_value_key; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_key_value_key UNIQUE (key, value);


--
-- Name: set set_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_pkey PRIMARY KEY (id);


--
-- Name: state state_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_pkey PRIMARY KEY (id);


--
-- Name: default_saver_templates pk_defalult_saver_templates; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.default_saver_templates
    ADD CONSTRAINT pk_defalult_saver_templates PRIMARY KEY (default_saver_template_id);


--
-- Name: folder_types pk_folder_types_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.folder_types
    ADD CONSTRAINT pk_folder_types_id PRIMARY KEY (folder_type_id);


--
-- Name: folders pk_folders_folder_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.folders
    ADD CONSTRAINT pk_folders_folder_id PRIMARY KEY (folder_id);


--
-- Name: procedure_tasks pk_procedure_tasks_procedure_task_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.procedure_tasks
    ADD CONSTRAINT pk_procedure_tasks_procedure_task_id PRIMARY KEY (procedure_task_id);


--
-- Name: saver_templates pk_saver_templates; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.saver_templates
    ADD CONSTRAINT pk_saver_templates PRIMARY KEY (saver_template_id);


--
-- Name: saver_types pk_saver_types; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.saver_types
    ADD CONSTRAINT pk_saver_types PRIMARY KEY (saver_type_id);


--
-- Name: service_attributes pk_service_attributes_service_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attributes
    ADD CONSTRAINT pk_service_attributes_service_attribute_id PRIMARY KEY (service_attribute_id);


--
-- Name: service_statuses pk_service_statuses_service_status_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_statuses
    ADD CONSTRAINT pk_service_statuses_service_status_id PRIMARY KEY (service_status_id);


--
-- Name: services_service_attributes pk_services_service_attributes_service_service_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.services_service_attributes
    ADD CONSTRAINT pk_services_service_attributes_service_service_attribute_id PRIMARY KEY (service_service_attribute_id);


--
-- Name: services pk_services_service_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.services
    ADD CONSTRAINT pk_services_service_id PRIMARY KEY (service_id);


--
-- Name: task_attributes pk_task_attributes_task_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_attributes
    ADD CONSTRAINT pk_task_attributes_task_attribute_id PRIMARY KEY (task_attribute_id);


--
-- Name: task_statuses pk_task_statuses_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_statuses
    ADD CONSTRAINT pk_task_statuses_id PRIMARY KEY (task_status_id);


--
-- Name: task_types pk_task_types_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_types
    ADD CONSTRAINT pk_task_types_id PRIMARY KEY (task_type_id);


--
-- Name: tasks_task_attributes pk_tasks_task_attributes_task_task_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.tasks_task_attributes
    ADD CONSTRAINT pk_tasks_task_attributes_task_task_attribute_id PRIMARY KEY (task_task_attribute_id);


--
-- Name: tasks pk_tasks_task_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.tasks
    ADD CONSTRAINT pk_tasks_task_id PRIMARY KEY (task_id);


--
-- Name: folder_types u_folder_types_title; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.folder_types
    ADD CONSTRAINT u_folder_types_title UNIQUE (folder_type_title);


--
-- Name: saver_types u_saver_types_title; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.saver_types
    ADD CONSTRAINT u_saver_types_title UNIQUE (saver_type_title);


--
-- Name: service_attribute_date_strings u_service_attribute_date_strings_id_string; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attribute_date_strings
    ADD CONSTRAINT u_service_attribute_date_strings_id_string UNIQUE (service_service_attribute_id, service_service_attribute_date);


--
-- Name: service_attribute_date_strings u_service_attribute_date_strings_service_attribute_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attribute_date_strings
    ADD CONSTRAINT u_service_attribute_date_strings_service_attribute_attribute_id UNIQUE (service_service_attribute_id, service_service_attribute_date);


--
-- Name: service_attribute_dates u_service_attribute_dates_service_attribute_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attribute_dates
    ADD CONSTRAINT u_service_attribute_dates_service_attribute_attribute_id UNIQUE (service_service_attribute_id);


--
-- Name: service_attribute_numerics u_service_attribute_numerics_service_attribute_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attribute_numerics
    ADD CONSTRAINT u_service_attribute_numerics_service_attribute_attribute_id UNIQUE (service_service_attribute_id);


--
-- Name: service_attribute_strings u_service_attribute_strings_service_attribute_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attribute_strings
    ADD CONSTRAINT u_service_attribute_strings_service_attribute_attribute_id UNIQUE (service_service_attribute_id);


--
-- Name: service_attributes u_service_attributes_service_attribute_title; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attributes
    ADD CONSTRAINT u_service_attributes_service_attribute_title UNIQUE (service_attribute_title);


--
-- Name: services_service_attributes u_services_service_attributes_service_id_service_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.services_service_attributes
    ADD CONSTRAINT u_services_service_attributes_service_id_service_attribute_id UNIQUE (service_id, service_attribute_id);


--
-- Name: services u_services_service_title; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.services
    ADD CONSTRAINT u_services_service_title UNIQUE (service_title);


--
-- Name: task_attribute_dates u_task_attribute_dates_task_task_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_attribute_dates
    ADD CONSTRAINT u_task_attribute_dates_task_task_attribute_id UNIQUE (task_task_attribute_id);


--
-- Name: task_attribute_numerics u_task_attribute_numerics_task_task_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_attribute_numerics
    ADD CONSTRAINT u_task_attribute_numerics_task_task_attribute_id UNIQUE (task_task_attribute_id);


--
-- Name: task_attribute_strings u_task_attribute_strings_task_task_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_attribute_strings
    ADD CONSTRAINT u_task_attribute_strings_task_task_attribute_id UNIQUE (task_task_attribute_id);


--
-- Name: task_attributes u_task_attributes_task_attribute_title; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_attributes
    ADD CONSTRAINT u_task_attributes_task_attribute_title UNIQUE (task_attribute_title);


--
-- Name: task_statuses u_task_statuses_title; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_statuses
    ADD CONSTRAINT u_task_statuses_title UNIQUE (task_status_title);


--
-- Name: task_types u_task_types_title; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_types
    ADD CONSTRAINT u_task_types_title UNIQUE (task_type_title);


--
-- Name: tasks_task_attributes u_tasks_task_attributes_task_id_task_attribute_id; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.tasks_task_attributes
    ADD CONSTRAINT u_tasks_task_attributes_task_id_task_attribute_id UNIQUE (task_id, task_attribute_id);


--
-- Name: ix_hangfire_counter_expireat; Type: INDEX; Schema: hangfire; Owner: karma_admin
--

CREATE INDEX ix_hangfire_counter_expireat ON hangfire.counter USING btree (expireat);


--
-- Name: ix_hangfire_counter_key; Type: INDEX; Schema: hangfire; Owner: karma_admin
--

CREATE INDEX ix_hangfire_counter_key ON hangfire.counter USING btree (key);


--
-- Name: ix_hangfire_job_statename; Type: INDEX; Schema: hangfire; Owner: karma_admin
--

CREATE INDEX ix_hangfire_job_statename ON hangfire.job USING btree (statename);


--
-- Name: ix_hangfire_jobparameter_jobidandname; Type: INDEX; Schema: hangfire; Owner: karma_admin
--

CREATE INDEX ix_hangfire_jobparameter_jobidandname ON hangfire.jobparameter USING btree (jobid, name);


--
-- Name: ix_hangfire_jobqueue_jobidandqueue; Type: INDEX; Schema: hangfire; Owner: karma_admin
--

CREATE INDEX ix_hangfire_jobqueue_jobidandqueue ON hangfire.jobqueue USING btree (jobid, queue);


--
-- Name: ix_hangfire_jobqueue_queueandfetchedat; Type: INDEX; Schema: hangfire; Owner: karma_admin
--

CREATE INDEX ix_hangfire_jobqueue_queueandfetchedat ON hangfire.jobqueue USING btree (queue, fetchedat);


--
-- Name: ix_hangfire_state_jobid; Type: INDEX; Schema: hangfire; Owner: karma_admin
--

CREATE INDEX ix_hangfire_state_jobid ON hangfire.state USING btree (jobid);


--
-- Name: jobparameter jobparameter_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: state state_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: karma_admin
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: folders fk_folders_folder_id_folder_root_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.folders
    ADD CONSTRAINT fk_folders_folder_id_folder_root_id FOREIGN KEY (folder_root_id) REFERENCES murr_downloader.folders(folder_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: saver_templates fk_folders_saver_template_folder_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.saver_templates
    ADD CONSTRAINT fk_folders_saver_template_folder_id FOREIGN KEY (saver_template_folder_id) REFERENCES murr_downloader.folders(folder_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: task_templates fk_folders_task_template_folder_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_templates
    ADD CONSTRAINT fk_folders_task_template_folder_id FOREIGN KEY (task_template_folder_id) REFERENCES murr_downloader.folders(folder_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: procedure_tasks_history fk_procedure_tasks_procedure_task_id_procedure_task_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.procedure_tasks_history
    ADD CONSTRAINT fk_procedure_tasks_procedure_task_id_procedure_task_id FOREIGN KEY (procedure_task_id) REFERENCES murr_downloader.procedure_tasks(procedure_task_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: saver_templates fk_saver_types_saver_type_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.saver_templates
    ADD CONSTRAINT fk_saver_types_saver_type_id FOREIGN KEY (saver_type_id) REFERENCES murr_downloader.saver_types(saver_type_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: service_attribute_dates fk_service_attribute_dates_service_service_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attribute_dates
    ADD CONSTRAINT fk_service_attribute_dates_service_service_attribute_id FOREIGN KEY (service_service_attribute_id) REFERENCES murr_downloader.services_service_attributes(service_service_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: service_attribute_numerics fk_service_attribute_numerics_service_service_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attribute_numerics
    ADD CONSTRAINT fk_service_attribute_numerics_service_service_attribute_id FOREIGN KEY (service_service_attribute_id) REFERENCES murr_downloader.services_service_attributes(service_service_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: service_attribute_strings fk_service_attribute_strings_service_service_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attribute_strings
    ADD CONSTRAINT fk_service_attribute_strings_service_service_attribute_id FOREIGN KEY (service_service_attribute_id) REFERENCES murr_downloader.services_service_attributes(service_service_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: services_service_attributes fk_services_service_attributes_service_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.services_service_attributes
    ADD CONSTRAINT fk_services_service_attributes_service_attribute_id FOREIGN KEY (service_attribute_id) REFERENCES murr_downloader.service_attributes(service_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: services_service_attributes fk_services_service_attributes_service_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.services_service_attributes
    ADD CONSTRAINT fk_services_service_attributes_service_id FOREIGN KEY (service_id) REFERENCES murr_downloader.services(service_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: services fk_services_service_statuses_service_status_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.services
    ADD CONSTRAINT fk_services_service_statuses_service_status_id FOREIGN KEY (service_status_id) REFERENCES murr_downloader.service_statuses(service_status_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: task_attribute_date_strings fk_task_attribute_date_strings_task_task_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_attribute_date_strings
    ADD CONSTRAINT fk_task_attribute_date_strings_task_task_attribute_id FOREIGN KEY (task_task_attribute_id) REFERENCES murr_downloader.tasks_task_attributes(task_task_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: service_attribute_date_strings fk_task_attribute_date_strings_task_task_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.service_attribute_date_strings
    ADD CONSTRAINT fk_task_attribute_date_strings_task_task_attribute_id FOREIGN KEY (service_service_attribute_id) REFERENCES murr_downloader.services_service_attributes(service_service_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: task_attribute_dates fk_task_attribute_dates_task_task_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_attribute_dates
    ADD CONSTRAINT fk_task_attribute_dates_task_task_attribute_id FOREIGN KEY (task_task_attribute_id) REFERENCES murr_downloader.tasks_task_attributes(task_task_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: task_attribute_numerics fk_task_attribute_numerics_task_task_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_attribute_numerics
    ADD CONSTRAINT fk_task_attribute_numerics_task_task_attribute_id FOREIGN KEY (task_task_attribute_id) REFERENCES murr_downloader.tasks_task_attributes(task_task_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: task_attribute_strings fk_task_attribute_strings_task_task_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_attribute_strings
    ADD CONSTRAINT fk_task_attribute_strings_task_task_attribute_id FOREIGN KEY (task_task_attribute_id) REFERENCES murr_downloader.tasks_task_attributes(task_task_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: tasks fk_task_statuses_tasks_status_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.tasks
    ADD CONSTRAINT fk_task_statuses_tasks_status_id FOREIGN KEY (task_status_id) REFERENCES murr_downloader.task_statuses(task_status_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: task_templates fk_task_types_task_templates_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_templates
    ADD CONSTRAINT fk_task_types_task_templates_id FOREIGN KEY (task_type_id) REFERENCES murr_downloader.task_types(task_type_id);


--
-- Name: default_saver_templates fk_task_types_task_type_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.default_saver_templates
    ADD CONSTRAINT fk_task_types_task_type_id FOREIGN KEY (task_type_id) REFERENCES murr_downloader.task_types(task_type_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: tasks fk_tasks_saver_template_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.tasks
    ADD CONSTRAINT fk_tasks_saver_template_id FOREIGN KEY (saver_template_id) REFERENCES murr_downloader.saver_templates(saver_template_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: tasks_task_attributes fk_tasks_task_attributes_task_attribute_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.tasks_task_attributes
    ADD CONSTRAINT fk_tasks_task_attributes_task_attribute_id FOREIGN KEY (task_attribute_id) REFERENCES murr_downloader.task_attributes(task_attribute_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: tasks_task_attributes fk_tasks_task_attributes_task_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.tasks_task_attributes
    ADD CONSTRAINT fk_tasks_task_attributes_task_id FOREIGN KEY (task_id) REFERENCES murr_downloader.tasks(task_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: SCHEMA hangfire; Type: ACL; Schema: -; Owner: postgres
--

GRANT USAGE ON SCHEMA hangfire TO karma_admin;


--
-- Name: SCHEMA murr_downloader; Type: ACL; Schema: -; Owner: karma_admin
--

GRANT USAGE ON SCHEMA murr_downloader TO karma_downloader;


--
-- Name: FUNCTION add_cbr_foreign_exchange(in_datetime timestamp without time zone); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.add_cbr_foreign_exchange(in_datetime timestamp without time zone) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.add_cbr_foreign_exchange(in_datetime timestamp without time zone) TO karma_downloader;


--
-- Name: FUNCTION add_service(in_service_name character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.add_service(in_service_name character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.add_service(in_service_name character varying) TO karma_downloader;


--
-- Name: FUNCTION change_procedure_task(in_procedure_task_id bigint, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.change_procedure_task(in_procedure_task_id bigint, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.change_procedure_task(in_procedure_task_id bigint, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone) TO karma_downloader;


--
-- Name: FUNCTION change_task_status(in_task_id bigint, in_old_task_status bigint, in_new_task_status bigint); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.change_task_status(in_task_id bigint, in_old_task_status bigint, in_new_task_status bigint) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.change_task_status(in_task_id bigint, in_old_task_status bigint, in_new_task_status bigint) TO karma_downloader;


--
-- Name: FUNCTION get_jobs(); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.get_jobs() FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.get_jobs() TO karma_downloader;


--
-- Name: FUNCTION get_procedure_tasks(); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.get_procedure_tasks() FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.get_procedure_tasks() TO karma_downloader;


--
-- Name: FUNCTION get_service_date(in_service_name character varying, in_attribute character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.get_service_date(in_service_name character varying, in_attribute character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.get_service_date(in_service_name character varying, in_attribute character varying) TO karma_downloader;


--
-- Name: FUNCTION get_service_numeric(in_service_name character varying, in_attribute character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.get_service_numeric(in_service_name character varying, in_attribute character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.get_service_numeric(in_service_name character varying, in_attribute character varying) TO karma_downloader;


--
-- Name: FUNCTION get_service_string(in_service_name character varying, in_attribute character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.get_service_string(in_service_name character varying, in_attribute character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.get_service_string(in_service_name character varying, in_attribute character varying) TO karma_downloader;


--
-- Name: FUNCTION get_task_date(in_task_id bigint, in_attribute character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.get_task_date(in_task_id bigint, in_attribute character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.get_task_date(in_task_id bigint, in_attribute character varying) TO karma_downloader;


--
-- Name: FUNCTION get_task_numeric(in_task_id bigint, in_attribute character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.get_task_numeric(in_task_id bigint, in_attribute character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.get_task_numeric(in_task_id bigint, in_attribute character varying) TO karma_downloader;


--
-- Name: FUNCTION get_task_string(in_task_id bigint, in_attribute character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.get_task_string(in_task_id bigint, in_attribute character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.get_task_string(in_task_id bigint, in_attribute character varying) TO karma_downloader;


--
-- Name: FUNCTION insert_procedure_task(in_procedure_title character varying, in_procedure_is_use boolean, in_procedure_params jsonb, in_procedure_template character varying, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.insert_procedure_task(in_procedure_title character varying, in_procedure_is_use boolean, in_procedure_params jsonb, in_procedure_template character varying, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.insert_procedure_task(in_procedure_title character varying, in_procedure_is_use boolean, in_procedure_params jsonb, in_procedure_template character varying, in_procedure_last_run timestamp without time zone, in_procedure_next_run timestamp without time zone) TO karma_downloader;


--
-- Name: FUNCTION insert_service_date(in_service_name character varying, in_service_attribute character varying, in_service_value timestamp without time zone); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.insert_service_date(in_service_name character varying, in_service_attribute character varying, in_service_value timestamp without time zone) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.insert_service_date(in_service_name character varying, in_service_attribute character varying, in_service_value timestamp without time zone) TO karma_downloader;


--
-- Name: FUNCTION insert_service_date_string(in_service_name character varying, in_service_attribute character varying, in_service_date timestamp without time zone, in_service_value character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.insert_service_date_string(in_service_name character varying, in_service_attribute character varying, in_service_date timestamp without time zone, in_service_value character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.insert_service_date_string(in_service_name character varying, in_service_attribute character varying, in_service_date timestamp without time zone, in_service_value character varying) TO karma_downloader;


--
-- Name: FUNCTION insert_service_numeric(in_service_name character varying, in_service_attribute character varying, in_service_value numeric); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.insert_service_numeric(in_service_name character varying, in_service_attribute character varying, in_service_value numeric) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.insert_service_numeric(in_service_name character varying, in_service_attribute character varying, in_service_value numeric) TO karma_downloader;


--
-- Name: FUNCTION insert_service_string(in_service_name character varying, in_service_attribute character varying, in_service_value character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.insert_service_string(in_service_name character varying, in_service_attribute character varying, in_service_value character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.insert_service_string(in_service_name character varying, in_service_attribute character varying, in_service_value character varying) TO karma_downloader;


--
-- Name: FUNCTION insert_task_date(in_task_id bigint, in_task_attribute character varying, in_task_value timestamp without time zone); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.insert_task_date(in_task_id bigint, in_task_attribute character varying, in_task_value timestamp without time zone) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.insert_task_date(in_task_id bigint, in_task_attribute character varying, in_task_value timestamp without time zone) TO karma_downloader;


--
-- Name: FUNCTION insert_task_date_string(in_task_id bigint, in_task_attribute character varying, in_task_date timestamp without time zone, in_task_value character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.insert_task_date_string(in_task_id bigint, in_task_attribute character varying, in_task_date timestamp without time zone, in_task_value character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.insert_task_date_string(in_task_id bigint, in_task_attribute character varying, in_task_date timestamp without time zone, in_task_value character varying) TO karma_downloader;


--
-- Name: FUNCTION insert_task_numeric(in_task_id bigint, in_task_attribute character varying, in_task_value numeric); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.insert_task_numeric(in_task_id bigint, in_task_attribute character varying, in_task_value numeric) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.insert_task_numeric(in_task_id bigint, in_task_attribute character varying, in_task_value numeric) TO karma_downloader;


--
-- Name: FUNCTION insert_task_string(in_task_id bigint, in_task_attribute character varying, in_task_value character varying); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.insert_task_string(in_task_id bigint, in_task_attribute character varying, in_task_value character varying) FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.insert_task_string(in_task_id bigint, in_task_attribute character varying, in_task_value character varying) TO karma_downloader;


--
-- PostgreSQL database dump complete
--

