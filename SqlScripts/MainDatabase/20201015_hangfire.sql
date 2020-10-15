GRANT USAGE ON SCHEMA hangfire TO karma_admin;

ALTER TABLE hangfire.counter OWNER to karma_admin;

ALTER TABLE hangfire.hash OWNER to karma_admin;
	
ALTER TABLE hangfire.job OWNER to karma_admin;
	
ALTER TABLE hangfire.jobparameter OWNER to karma_admin;
	
ALTER TABLE hangfire.jobqueue OWNER to karma_admin;
	
ALTER TABLE hangfire.list OWNER to karma_admin;
	
ALTER TABLE hangfire.lock OWNER to karma_admin;
	
ALTER TABLE hangfire.schema OWNER to karma_admin;
	
ALTER TABLE hangfire.server OWNER to karma_admin;
	
ALTER TABLE hangfire.set OWNER to karma_admin;
	
ALTER TABLE hangfire.state OWNER to karma_admin;
	
ALTER SEQUENCE hangfire.counter_id_seq OWNER TO karma_admin;
	
ALTER SEQUENCE hangfire.hash_id_seq OWNER TO karma_admin;
	
ALTER SEQUENCE hangfire.job_id_seq OWNER TO karma_admin;
	
ALTER SEQUENCE hangfire.jobparameter_id_seq OWNER TO karma_admin;
	
ALTER SEQUENCE hangfire.jobqueue_id_seq OWNER TO karma_admin;
	
ALTER SEQUENCE hangfire.list_id_seq OWNER TO karma_admin;
	
ALTER SEQUENCE hangfire.set_id_seq OWNER TO karma_admin;
	
ALTER SEQUENCE hangfire.state_id_seq OWNER TO karma_admin;