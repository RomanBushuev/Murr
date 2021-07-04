create or replace function murr_downloader.get_jobs()
	returns table(task_id bigint, 
	task_template_id bigint, 
	task_status_id bigint)
as $$	
declare
begin
	return query
		select tasks.task_id, tasks.task_template_id, tasks.task_status_id
		from murr_downloader.tasks;
end
$$ language plpgsql
	SECURITY DEFINER;

ALTER FUNCTION murr_downloader.get_jobs
    OWNER TO karma_admin;
	
	
GRANT EXECUTE ON FUNCTION murr_downloader.get_jobs() TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_jobs() TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_jobs() FROM PUBLIC;


alter table murr_downloader.folders
add column folder_type_id bigint not null

alter table murr_downloader.folders
add constraint fk_folders_folder_type_id foreign key(folder_type_id)
	references murr_downloader.folder_types (folder_type_id) MATCH SIMPLE