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
	d_task_id bigint;
begin
	raise notice 'hello';
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
	
	raise notice 'bushuev';
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

	raise notice 'hello:%', d_task_tempate_id;

	--добавляем задачу к нам в шаблон
	insert into murr_downloader.tasks(task_template_id, task_created_time, task_status_id)
	values(d_task_tempate_id, now()::timestamp without time zone, 1)
	returning task_id into d_task_id;
	
	perform murr_downloader.insert_task_numeric(d_task_id, 'ATTEMPTIONS', 1);	
	
	return d_task_id;
end
$$;


ALTER FUNCTION murr_downloader.add_cbr_foreign_exchange(in_datetime timestamp without time zone) OWNER TO karma_admin;

--
-- Name: get_jobs(); Type: FUNCTION; Schema: murr_downloader; Owner: karma_admin
--

CREATE FUNCTION murr_downloader.get_jobs() RETURNS TABLE(task_id bigint, task_template_id bigint, task_status_id bigint)
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$	
declare
begin
	return query
		select tasks.task_id, tasks.task_template_id, tasks.task_status_id
		from murr_downloader.tasks;
end
$$;


ALTER FUNCTION murr_downloader.get_jobs() OWNER TO karma_admin;

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
-- Name: example_table; Type: TABLE; Schema: murr_downloader; Owner: karma_admin
--

CREATE TABLE murr_downloader.example_table (
    id integer,
    title character varying(255)
);


ALTER TABLE murr_downloader.example_table OWNER TO karma_admin;

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
    task_status_id bigint
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
-- Data for Name: counter; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.counter (id, key, value, expireat) FROM stdin;
1	stats:succeeded:2020-10-14	1	2020-11-14 20:38:24.60786
211	stats:succeeded	63	\N
6	stats:succeeded:2020-10-14	1	2020-11-14 20:52:51.139341
10	stats:succeeded:2020-10-14	1	2020-11-14 20:54:35.502018
13	stats:succeeded:2020-10-14	1	2020-11-14 20:55:05.823619
16	stats:succeeded:2020-10-14	1	2020-11-14 20:56:05.943824
19	stats:succeeded:2020-10-14	1	2020-11-14 20:57:05.984601
22	stats:succeeded:2020-10-14	1	2020-11-14 20:58:06.022581
25	stats:succeeded:2020-10-14	1	2020-11-14 20:59:06.085731
28	stats:succeeded:2020-10-14	1	2020-11-14 21:00:06.158047
31	stats:succeeded:2020-10-14	1	2020-11-14 21:01:06.232785
34	stats:succeeded:2020-10-14	1	2020-11-14 21:02:06.300878
37	stats:succeeded:2020-10-14	1	2020-11-14 21:03:06.353545
40	stats:succeeded:2020-10-14	1	2020-11-14 21:04:06.411628
43	stats:succeeded:2020-10-14	1	2020-11-14 21:05:06.478926
54	stats:succeeded:2020-10-15	1	2020-11-15 11:41:10.254379
57	stats:succeeded:2020-10-15	1	2020-11-15 11:42:10.28803
60	stats:succeeded:2020-10-15	1	2020-11-15 11:43:10.367281
63	stats:succeeded:2020-10-15	1	2020-11-15 11:44:10.394576
66	stats:succeeded:2020-10-15	1	2020-11-15 11:45:10.451485
69	stats:succeeded:2020-10-15	1	2020-11-15 11:46:10.57259
72	stats:succeeded:2020-10-15	1	2020-11-15 11:47:10.67057
75	stats:succeeded:2020-10-15	1	2020-11-15 11:48:10.734254
78	stats:succeeded:2020-10-15	1	2020-11-15 11:49:10.801412
81	stats:succeeded:2020-10-15	1	2020-11-15 11:50:10.838767
84	stats:succeeded:2020-10-15	1	2020-11-15 11:51:11.007825
87	stats:succeeded:2020-10-15	1	2020-11-15 11:52:11.136054
90	stats:succeeded:2020-10-15	1	2020-11-15 11:53:11.199241
93	stats:succeeded:2020-10-15	1	2020-11-15 11:54:11.258857
96	stats:succeeded:2020-10-15	1	2020-11-15 11:55:11.359607
99	stats:succeeded:2020-10-15	1	2020-11-15 11:56:12.128789
102	stats:succeeded:2020-10-15	1	2020-11-15 14:56:26.520709
105	stats:succeeded:2020-10-15	1	2020-11-15 15:41:24.296927
108	stats:succeeded:2020-10-15	1	2020-11-15 15:42:09.218871
111	stats:succeeded:2020-10-15	1	2020-11-15 15:43:09.325817
114	stats:succeeded:2020-10-15	1	2020-11-15 15:44:09.365421
117	stats:succeeded:2020-10-15	1	2020-11-15 15:45:09.516919
120	stats:succeeded:2020-10-15	1	2020-11-15 15:46:09.569207
123	stats:succeeded:2020-10-15	1	2020-11-15 15:47:09.681551
126	stats:succeeded:2020-10-15	1	2020-11-15 15:48:09.72665
129	stats:succeeded:2020-10-15	1	2020-11-15 15:49:09.926914
132	stats:succeeded:2020-10-15	1	2020-11-15 15:50:09.958742
135	stats:succeeded:2020-10-15	1	2020-11-15 15:51:10.268069
138	stats:succeeded:2020-10-15	1	2020-11-15 15:52:10.402786
141	stats:succeeded:2020-10-15	1	2020-11-15 15:53:10.465058
144	stats:succeeded:2020-10-15	1	2020-11-15 15:54:10.517046
147	stats:succeeded:2020-10-15	1	2020-11-15 15:55:10.629252
150	stats:succeeded:2020-10-15	1	2020-11-15 15:56:10.687585
153	stats:succeeded:2020-10-15	1	2020-11-15 15:57:10.737736
156	stats:succeeded:2020-10-15	1	2020-11-15 15:58:10.856812
159	stats:succeeded:2020-10-15	1	2020-11-15 15:59:10.913614
162	stats:succeeded:2020-10-15	1	2020-11-15 16:00:10.999219
165	stats:succeeded:2020-10-15	1	2020-11-15 16:01:11.074155
168	stats:succeeded:2020-10-15	1	2020-11-15 16:02:11.132219
171	stats:succeeded:2020-10-15	1	2020-11-15 16:03:11.208088
174	stats:succeeded:2020-10-15	1	2020-11-15 16:04:11.276542
177	stats:succeeded:2020-10-15	1	2020-11-15 16:05:11.680697
180	stats:succeeded:2020-10-15	1	2020-11-15 16:06:11.761764
183	stats:succeeded:2020-10-15	1	2020-11-15 16:07:11.826539
186	stats:succeeded:2020-10-15	1	2020-11-15 16:08:11.941666
189	stats:succeeded:2020-10-15	1	2020-11-15 16:09:11.963024
192	stats:succeeded:2020-10-15	1	2020-11-15 16:10:12.054782
195	stats:succeeded:2020-10-15	1	2020-11-15 16:11:12.106843
198	stats:succeeded:2020-10-15	1	2020-11-15 16:12:12.182575
\.


--
-- Data for Name: hash; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.hash (id, key, field, value, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: job; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.job (id, stateid, statename, invocationdata, arguments, createdat, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: jobparameter; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.jobparameter (id, jobid, name, value, updatecount) FROM stdin;
\.


--
-- Data for Name: jobqueue; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.jobqueue (id, jobid, queue, fetchedat, updatecount) FROM stdin;
\.


--
-- Data for Name: list; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.list (id, key, value, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: lock; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.lock (resource, updatecount, acquired) FROM stdin;
\.


--
-- Data for Name: schema; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.schema (version) FROM stdin;
12
\.


--
-- Data for Name: server; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.server (id, data, lastheartbeat, updatecount) FROM stdin;
desktop-tn2ken5:4140:01843ad5-3380-4300-a85b-ed9d3c70be4c	{"WorkerCount":20,"Queues":["default"],"StartedAt":"2020-10-15T11:40:23.2770485Z"}	2020-10-16 20:30:24.410871	0
\.


--
-- Data for Name: set; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.set (id, key, score, value, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: state; Type: TABLE DATA; Schema: hangfire; Owner: karma_admin
--

COPY hangfire.state (id, jobid, name, reason, createdat, data, updatecount) FROM stdin;
\.


--
-- Data for Name: example_table; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.example_table (id, title) FROM stdin;
1	roman
\.


--
-- Data for Name: folder_types; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.folder_types (folder_type_id, folder_type_title, folder_type_description) FROM stdin;
0	UNDEFINED	Неопределено
1	DOWNLOAD	Загрузчик
\.


--
-- Data for Name: folders; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.folders (folder_id, folder_root_id, folder_title, folder_type_id) FROM stdin;
25	\N	Загрузка валют	1
\.


--
-- Data for Name: service_attribute_date_strings; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.service_attribute_date_strings (service_service_attribute_id, service_service_attribute_date, service_service_attribute_title) FROM stdin;
23	2020-09-30 23:30:37.835176	bushuev2
23	2020-09-30 23:30:40.822627	bushuev3
23	2020-09-30 23:31:12.424145	bushuev1
23	2020-09-30 23:31:14.201586	bushuev2
23	2020-09-30 23:31:16.146409	bushuev3
23	2020-09-30 23:31:23.618032	bushuev1
23	2020-09-30 23:31:26.154162	bushuev3
23	2020-09-30 23:31:37.115381	bushuev3
24	2020-09-30 23:32:10.387493	error
24	2020-09-30 23:32:18.164109	error
23	2020-09-30 23:32:24.99925	bushuev3
23	2020-09-30 23:32:54.680101	bushuev3
24	2020-09-30 23:33:23.659797	error
\.


--
-- Data for Name: service_attribute_dates; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.service_attribute_dates (service_service_attribute_id, service_service_attribute_date) FROM stdin;
22	2020-09-29 00:00:00
\.


--
-- Data for Name: service_attribute_numerics; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.service_attribute_numerics (service_service_attribute_id, service_service_attribute_numeric) FROM stdin;
21	-1
\.


--
-- Data for Name: service_attribute_strings; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.service_attribute_strings (service_service_attribute_id, service_service_attribute_title) FROM stdin;
19	Bushuev8
\.


--
-- Data for Name: service_attributes; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.service_attributes (service_attribute_id, service_attribute_title, service_attribute_description) FROM stdin;
1	LAST_LIFE_DATE_TIME	Последнее время отклика сервиса
2	LAST_WORKING_DATE_TIME	Последнее время отклика сервиса, если он находится в рабочем статусе
3	CURRENT_TASK_ID	Текущая выполняема задача, если задачи нет, -1
4	SERVICE_LOG	Логирование состояния сервиса
5	ERROR_SERVICE_LOG	Ошибки сервиса
10	ALIAS	Псевдоним
\.


--
-- Data for Name: service_statuses; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.service_statuses (service_status_id, service_status_title, service_status_description) FROM stdin;
0	UNDEFINED	Неопределено
1	RUNNING	Запущен
2	STOPPING	Остановлен
\.


--
-- Data for Name: services; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.services (service_id, service_title, service_status_id) FROM stdin;
13	DOWNLOADER	0
\.


--
-- Data for Name: services_service_attributes; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.services_service_attributes (service_service_attribute_id, service_id, service_attribute_id) FROM stdin;
19	13	10
21	13	3
22	13	1
23	13	4
24	13	5
\.


--
-- Data for Name: task_attribute_date_strings; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.task_attribute_date_strings (task_task_attribute_id, task_task_attribute_date, task_task_attribute_title) FROM stdin;
\.


--
-- Data for Name: task_attribute_dates; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.task_attribute_dates (task_task_attribute_id, task_task_attribute_date) FROM stdin;
\.


--
-- Data for Name: task_attribute_numerics; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.task_attribute_numerics (task_task_attribute_id, task_task_attribute_numeric) FROM stdin;
\.


--
-- Data for Name: task_attribute_strings; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.task_attribute_strings (task_task_attribute_id, task_task_attribute_title) FROM stdin;
\.


--
-- Data for Name: task_attributes; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.task_attributes (task_attribute_id, task_attribute_title, task_attribute_description) FROM stdin;
6	ATTEMPTIONS	Кол-во запусков
7	ERROR_LOG	Ошибки задачи
8	INFO_LOG	Информационный лог
9	WARNING_LOG	Предупреждения
11	START_TASK	Время старта
12	END_TASK	Время окончания
\.


--
-- Data for Name: task_statuses; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.task_statuses (task_status_id, task_status_title, task_statuses_description) FROM stdin;
0	UNDEFINED	Неопределено
1	CREATING	В состоянии создания
2	CREATED	Создан
3	RUNNING	Запущен
4	DONE	Выполнен
5	ERROR	Закончен с ошибками
\.


--
-- Data for Name: task_templates; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.task_templates (task_template_id, task_template_title, task_template_created_time, task_template_folder_id, task_parameters, task_type_id) FROM stdin;
\.


--
-- Data for Name: task_types; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.task_types (task_type_id, task_type_title, task_type_description) FROM stdin;
0	UNDEFINED	Неопределено
1	DOWNLOAD CURRENCIES CBRF	Загрузка валюты из ЦБ
2	DOWNLOAD G2 CURVE CBRF	Загрузка кривой G2 из ЦБ
\.


--
-- Data for Name: tasks; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.tasks (task_id, task_template_id, task_created_time, task_status_id) FROM stdin;
27	26	2020-10-11 14:08:27.014837	1
\.


--
-- Data for Name: tasks_task_attributes; Type: TABLE DATA; Schema: murr_downloader; Owner: karma_admin
--

COPY murr_downloader.tasks_task_attributes (task_task_attribute_id, task_id, task_attribute_id) FROM stdin;
\.


--
-- Name: counter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: karma_admin
--

SELECT pg_catalog.setval('hangfire.counter_id_seq', 211, true);


--
-- Name: hash_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: karma_admin
--

SELECT pg_catalog.setval('hangfire.hash_id_seq', 32, true);


--
-- Name: job_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: karma_admin
--

SELECT pg_catalog.setval('hangfire.job_id_seq', 63, true);


--
-- Name: jobparameter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: karma_admin
--

SELECT pg_catalog.setval('hangfire.jobparameter_id_seq', 246, true);


--
-- Name: jobqueue_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: karma_admin
--

SELECT pg_catalog.setval('hangfire.jobqueue_id_seq', 63, true);


--
-- Name: list_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: karma_admin
--

SELECT pg_catalog.setval('hangfire.list_id_seq', 1, false);


--
-- Name: set_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: karma_admin
--

SELECT pg_catalog.setval('hangfire.set_id_seq', 4, true);


--
-- Name: state_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: karma_admin
--

SELECT pg_catalog.setval('hangfire.state_id_seq', 189, true);


--
-- Name: murr_sequence; Type: SEQUENCE SET; Schema: murr_downloader; Owner: karma_admin
--

SELECT pg_catalog.setval('murr_downloader.murr_sequence', 27, true);


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
-- Name: example_table example_table_title_key; Type: CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.example_table
    ADD CONSTRAINT example_table_title_key UNIQUE (title);


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
-- Name: task_templates fk_folders_task_template_folder_id; Type: FK CONSTRAINT; Schema: murr_downloader; Owner: karma_admin
--

ALTER TABLE ONLY murr_downloader.task_templates
    ADD CONSTRAINT fk_folders_task_template_folder_id FOREIGN KEY (task_template_folder_id) REFERENCES murr_downloader.folders(folder_id) ON UPDATE CASCADE ON DELETE CASCADE;


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
-- Name: FUNCTION get_jobs(); Type: ACL; Schema: murr_downloader; Owner: karma_admin
--

REVOKE ALL ON FUNCTION murr_downloader.get_jobs() FROM PUBLIC;
GRANT ALL ON FUNCTION murr_downloader.get_jobs() TO karma_downloader;


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

