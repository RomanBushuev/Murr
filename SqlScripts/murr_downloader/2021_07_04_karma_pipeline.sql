create table murr_downloader.pipeline_tasks
(
	pipeline_task_id bigint NOT NULL DEFAULT nextval('murr_downloader.murr_sequence'::regclass),
	start_task_id bigint not null, 
	next_task_id bigint,
	created_date_time timestamp without time zone,
	update_date_time timestamp without time zone,
	constraint pk_pipeline_task_id primary key(pipeline_task_id, start_task_id),
	constraint fk_tasks_pipeline_tasks_start_task_id FOREIGN KEY (start_task_id)
		REFERENCES murr_downloader.tasks (task_id) MATCH SIMPLE
		ON UPDATE CASCADE
        ON DELETE CASCADE,
	constraint fk_tasks_pipeline_tasks_next_task_id FOREIGN KEY (next_task_id)
		REFERENCES murr_downloader.tasks (task_id) MATCH SIMPLE
		ON UPDATE CASCADE
        ON DELETE CASCADE
);

ALTER TABLE murr_downloader.pipeline_tasks
    OWNER to karma_admin;
COMMENT ON TABLE murr_downloader.pipeline_tasks
    IS 'Задачи для формирования pipeline';


CREATE OR REPLACE FUNCTION murr_downloader.tasks_copy() RETURNS TRIGGER AS
$BODY$
declare 
	d_pipeline_id bigint;
BEGIN 
	--BEGIN
	if(new.task_status_id = 4) then 
	
		select pipeline_task_id into d_pipeline_id
		from murr_downloader.pipeline_tasks
		where new.task_id = next_task_id;

		if d_pipeline_id is null 
		then
			INSERT INTO murr_downloader.pipeline_tasks(start_task_id, next_task_id, created_date_time, update_date_time)
			VALUES(new.task_id, null, CURRENT_TIMESTAMP::timestamp without time zone, CURRENT_TIMESTAMP::timestamp without time zone);
		else 
			INSERT INTO murr_downloader.pipeline_tasks(pipeline_task_id, start_task_id, next_task_id, created_date_time, update_date_time)
			VALUES(d_pipeline_id, new.task_id, null, CURRENT_TIMESTAMP::timestamp without time zone, CURRENT_TIMESTAMP::timestamp without time zone);
		end if;
	end if;
	--EXCEPTION WHEN OTHERS THEN
		--NULL;
	--END;
       RETURN new;
END;
$BODY$
language plpgsql;

CREATE TRIGGER trigger_murr_copy
    AFTER UPDATE OF task_status_id ON murr_downloader.tasks
    FOR EACH ROW
    EXECUTE PROCEDURE murr_downloader.tasks_copy();

CREATE OR REPLACE FUNCTION murr_data.insert_next_task_id_in_pipeline(
	in_pipeline_id bigint,
	in_new_task

