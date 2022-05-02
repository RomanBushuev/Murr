create or replace function murr_downloader.get_inner_procedures()
	returns table(procedure_schema character varying,
		procedure_name_second character varying,
		procedure_name character varying,
		external_language character varying,
		pn character varying,
		pm character varying,
		data_type character varying)
as $$
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
$$ language plpgsql;

alter function murr_downloader.get_inner_procedures
	owner to karma_admin;