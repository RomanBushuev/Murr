create table murr_downloader.procedure_tasks
(
	procedure_task_id bigint not null DEFAULT nextval('murr_downloader.murr_sequence'::regclass),
	procedure_title character varying(255) COLLATE pg_catalog."default" NOT NULL,
	procedure_is_use boolean not null,
	procedure_params jsonb not null,
	procedure_template character varying(255) COLLATE pg_catalog."default" NOT NULL,
	procedure_last_run timestamp without time zone,
	procedure_next_run timestamp without time zone
);

ALTER TABLE murr_downloader.procedure_tasks
    OWNER to karma_admin;
COMMENT ON TABLE murr_downloader.procedure_tasks
    IS 'Запуск процедур по требованию';

create or replace function murr_downloader.get_procedure_tasks()
	returns table (procedure_task_id bigint,
				 procedure_title character varying(255),
				 procedure_is_use boolean,
				 procedure_params jsonb,
				 procedure_template character varying(255),
				 procedure_last_run timestamp without time zone,
				 procedure_next_run timestamp without time zone)
as $$
declare 
begin 
	return query 
		select procedure_tasks.procedure_task_id, procedure_tasks.procedure_title, procedure_tasks.procedure_is_use, 
			procedure_tasks.procedure_params, procedure_tasks.procedure_template, 
			procedure_tasks.procedure_last_run, procedure_tasks.procedure_next_run
		from murr_downloader.procedure_tasks;
end
$$ language plpgsql
	security definer;

alter function murr_downloader.get_procedure_tasks
	owner to karma_admin;

grant execute on function murr_downloader.get_procedure_tasks() to karma_admin;
grant execute on function murr_downloader.get_procedure_tasks() to karma_downloader;
revoke all on function murr_downloader.get_procedure_tasks() from public;

--функция для вставки процедуры
create or replace function murr_downloader.insert_procedure_task(in_procedure_title character varying(255),
				 in_procedure_is_use boolean,
				 in_procedure_params jsonb,
				 in_procedure_template character varying(255),
				 in_procedure_last_run timestamp without time zone,
				 in_procedure_next_run timestamp without time zone)
returns bigint
as $$
declare
	d_procedure_task_id bigint;
begin
	insert into murr_downloader.procedure_tasks(procedure_title, procedure_is_use, procedure_params,
		procedure_template, procedure_last_run, procedure_next_run)
	values(in_procedure_title, in_procedure_is_use, in_procedure_params, in_procedure_template, in_procedure_last_run, in_procedure_next_run)
	returning procedure_task_id into d_procedure_task_id;
	return d_procedure_task_id;
end
$$ language plpgsql
	security definer;

alter function murr_downloader.insert_procedure_task(character varying(255),boolean,jsonb,character varying(255),timestamp without time zone,timestamp without time zone)
	owner to karma_admin;

grant execute on function murr_downloader.insert_procedure_task(character varying(255),boolean,jsonb,character varying(255),timestamp without time zone,timestamp without time zone) to karma_admin;
grant execute on function murr_downloader.insert_procedure_task(character varying(255),boolean,jsonb,character varying(255),timestamp without time zone,timestamp without time zone) to karma_downloader;
revoke all on function murr_downloader.insert_procedure_task(character varying(255),boolean,jsonb,character varying(255),timestamp without time zone,timestamp without time zone) from public;

--constraints
alter table murr_downloader.procedure_tasks
add constraint pk_procedure_tasks_procedure_task_id PRIMARY KEY (procedure_task_id);


create table murr_downloader.procedure_tasks_history
(
	procedure_task_id bigint not null,
	procedure_last_run timestamp without time zone,
	procedure_next_run timestamp without time zone,
	CONSTRAINT fk_procedure_tasks_procedure_task_id_procedure_task_id FOREIGN KEY (procedure_task_id)
        REFERENCES murr_downloader.procedure_tasks (procedure_task_id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE	
);

create or replace function murr_downloader.change_procedure_task(in_procedure_task_id bigint, 
	in_procedure_last_run timestamp without time zone,
	in_procedure_next_run timestamp without time zone)
returns void
as $$
declare
	d_procedure_task_id bigint;
begin
	insert into murr_downloader.procedure_tasks_history(procedure_task_id, procedure_last_run, procedure_next_run)
	values(in_procedure_task_id, in_procedure_last_run, in_procedure_next_run);
	
	update murr_downloader.procedure_tasks
	set procedure_last_run = in_procedure_last_run, procedure_next_run = in_procedure_next_run
	where procedure_task_id = in_procedure_task_id;
end
$$ language plpgsql
	security definer;
alter function murr_downloader.change_procedure_task(bigint, timestamp without time zone, timestamp without time zone)
	owner to karma_admin;

grant execute on function murr_downloader.change_procedure_task(bigint, timestamp without time zone, timestamp without time zone) to karma_admin;
grant execute on function murr_downloader.change_procedure_task(bigint, timestamp without time zone, timestamp without time zone) to karma_downloader;
revoke all on function murr_downloader.change_procedure_task(bigint, timestamp without time zone, timestamp without time zone) from public;





