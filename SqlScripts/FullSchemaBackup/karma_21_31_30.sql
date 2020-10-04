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

SET default_tablespace = '';

SET default_table_access_method = heap;

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
    folder_title character varying(255)
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
-- Name: SCHEMA murr_downloader; Type: ACL; Schema: -; Owner: karma_admin
--

GRANT USAGE ON SCHEMA murr_downloader TO karma_downloader;


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
-- PostgreSQL database dump complete
--

