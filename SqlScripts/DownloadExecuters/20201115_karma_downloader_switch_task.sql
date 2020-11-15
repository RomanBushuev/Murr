create or replace function murr_downloader.get_task(in_task_id bigint, 
	in_old_task_status bigint,
	in_new_task_status bigint)
returns bigint
language 'plpgsql'
cost 100
volatile security definer
as $BODY$
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
$BODY$;

ALTER FUNCTION murr_downloader.get_task(bigint, bigint, bigint) OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task(bigint, bigint, bigint) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_downloader.get_task(bigint, bigint, bigint) TO karma_downloader;

REVOKE ALL ON FUNCTION murr_downloader.get_task(bigint, bigint, bigint) FROM PUBLIC;


--get task_numeric

--get task_string

--get task_date

--get_task_date_string 