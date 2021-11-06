CREATE SEQUENCE murr_data.murr_sequence
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE murr_downloader.murr_sequence
    OWNER TO karma_admin;

--Тип атрибута
create table murr_data.fin_attribute_types(
    fin_attribute_type_id bigint not null,
    fin_attribute_type_ident varchar(63) not null,
    constraint pk_fin_attribute_type_id primary key (fin_attribute_type_id)
);

ALTER TABLE murr_data.fin_attribute_types
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.fin_attribute_types
    IS 'Типы атрибутов';
    
    
--Площадки финансовых инструментов
create table murr_data.fin_data_sources(
    fin_data_source_id bigint,
    fin_data_source_ident varchar(63) not null,
    constraint pk_fin_data_sources_id primary key (fin_data_source_id)
);

ALTER TABLE murr_data.fin_data_sources
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.fin_data_sources
    IS 'Площадки финансовых инструментов';

create table murr_data.fin_attributes(
    fin_attribute_id bigint NOT NULL,
    fin_ident varchar(63) not null,
    fin_title varchar(255) not null,
    fin_description varchar(1023) not null,
    fin_attribute_type_id bigint not null,
    constraint pk_fin_attributes_fin_attribute_id primary key(fin_attribute_id),
    constraint fk_fin_attributes_fin_attribute_types foreign key(fin_attribute_type_id) 
        references murr_data.fin_attribute_types (fin_attribute_type_id) match simple
        ON UPDATE CASCADE
        ON DELETE CASCADE    
);
    
ALTER TABLE murr_data.fin_attributes
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.fin_attributes
    IS 'Финансовые атрибуты';
    
create table murr_data.fin_instruments(
    fin_instrument_id bigint NOT NULL DEFAULT nextval('murr_data.murr_sequence'::regclass),
    --Площадка 
    fin_data_source_id bigint not null,
    fin_ident varchar(63) not null,
    constraint u_fin_instruments_fi_fa_fiti unique (fin_data_source_id, fin_ident),
    constraint pk_fin_instruments_fin_instrument_id primary key(fin_instrument_id),
    constraint fk_fin_instruments_fin_data_source foreign key (fin_data_source_id)
        references murr_data.fin_data_sources (fin_data_source_id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

ALTER TABLE murr_data.fin_instruments
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.fin_instruments
    IS 'Финансовые инструменты';

create table murr_data.fin_instrument_fin_attribute(
    fin_instrument_fin_attribute_id bigint NOT NULL DEFAULT nextval('murr_data.murr_sequence'::regclass),
    fin_instrument_id bigint not null,
    fin_attribute_id bigint not null,
    constraint pk_fi_fa_fin_instrument_fin_attribute_id primary key (fin_instrument_fin_attribute_id),
    constraint fk_fin_instrument_fin_attribute_fin_instruments foreign key (fin_instrument_id)
        references murr_data.fin_instruments (fin_instrument_id) match simple
        on update cascade
        on delete cascade,
    constraint fk_fin_instrument_fin_attribute_fin_instrument_attributes foreign key (fin_attribute_id)
        references murr_data.fin_attributes (fin_attribute_id) match simple
        on update cascade
        on delete cascade
);

ALTER TABLE murr_data.fin_instrument_fin_attribute
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.fin_instrument_fin_attribute
    IS 'Финансовые инструменты и атрибуты';

create table murr_data.fin_numeric_value(
    fin_instrument_attribute_id bigint not null,
    fin_instrument_value numeric not null,
    fin_instrument_date_from timestamp without time zone NOT NULL,
    constraint u_fin_numeric_value_fiai_fidf unique (fin_instrument_attribute_id, fin_instrument_date_from),
    constraint fk_fin_numeric_value_fin_instrument_fin_attribute foreign key  (fin_instrument_attribute_id)
        references murr_data.fin_instrument_fin_attribute (fin_instrument_fin_attribute_id) match simple
        on update cascade
        on delete cascade
);

ALTER TABLE murr_data.fin_numeric_value
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.fin_numeric_value
    IS 'Числовые значения финансовых инструментов';
    
create table murr_data.fin_string_value(
    fin_instrument_attribute_id bigint not null,
    fin_instrument_value text not null,
    fin_instrument_date_from timestamp without time zone NOT NULL,
    constraint u_fin_string_value_fiai_fidf unique (fin_instrument_attribute_id, fin_instrument_date_from),
    constraint fk_fin_string_value_fin_instrument_fin_attribute foreign key  (fin_instrument_attribute_id)
        references murr_data.fin_instrument_fin_attribute (fin_instrument_fin_attribute_id) match simple
        on update cascade
        on delete cascade
);

ALTER TABLE murr_data.fin_string_value
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.fin_string_value
    IS 'Строковые значения финансовых инструментов';
    
create table murr_data.fin_date_value(
    fin_instrument_attribute_id bigint not null,
    fin_instrument_value timestamp without time zone not null,
    fin_instrument_date_from timestamp without time zone NOT NULL,
    constraint u_fin_date_value_fiai_fidf unique (fin_instrument_attribute_id, fin_instrument_date_from),
    constraint fk_fin_date_value_fin_instrument_fin_attribute foreign key (fin_instrument_attribute_id)
        references murr_data.fin_instrument_fin_attribute (fin_instrument_fin_attribute_id) match simple
        on update cascade
        on delete cascade
);

ALTER TABLE murr_data.fin_date_value
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.fin_date_value
    IS 'Значения дат для финансовых инструментов';
    
    
create table murr_data.time_series(
    fin_instrument_attribute_id bigint not null,
    fin_instrument_value numeric,
    fin_instrument_date timestamp without time zone NOT NULL,
    constraint u_fin_time_series_fiai_fidf unique (fin_instrument_attribute_id, fin_instrument_date),
    constraint fk_fin_time_series_fin_instrument_fin_attribute foreign key (fin_instrument_attribute_id)
        references murr_data.fin_instrument_fin_attribute (fin_instrument_fin_attribute_id) match simple
        on update cascade
        on delete cascade
);

ALTER TABLE murr_data.time_series
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.time_series
    IS 'Временная серия для финансовых инструментов';
    
insert into murr_data.fin_attribute_types
values(1, 'STRING'),
(2, 'DATETIME'),
(3, 'NUMERIC'),
(4, 'TIMESERIES');

insert into murr_data.fin_attributes(fin_attribute_id, fin_ident, fin_title, fin_description, fin_attribute_type_id)
values(1, 'SHORTNAME', 'SHORTNAME', 'Краткое наименование', 1),
(2, 'ISIN', 'ISIN', 'ISIN', 1),
(3, 'LOW','LOW', 'Низшая цена', 4),
(4, 'HIGH', 'HIGH', 'Наивысшая цена', 4),
(5, 'CLOSE', 'CLOSE', 'Ставка', 4),
(6, 'OPEN', 'OPEN', 'Цена открытия', 4),
(7, 'VOLUME', 'VOLUME', 'Объем', 4),
(8, 'MATURITY_DATE', 'MATURITY_DATE', 'Дата погашения', 2),
(9, 'FACEVALUE', 'FACEVALUE', 'Номинал', 3),
(10, 'CURRENCY', 'CURRENCY', 'Валюта', 1),
(11, 'TRADED', 'TRADED', 'Наторговали объем на ставку', 4),
(12, 'TYPE', 'TYPE', 'Тип инструмента', 1);

insert into murr_data.fin_data_sources(fin_data_source_id, fin_data_source_ident)
values(1, 'CBR');


CREATE OR REPLACE FUNCTION murr_data.insert_fin_date_value(
    in_fin_id bigint,
    in_fin_attribute character varying,
    in_value timestamp without time zone,
    in_date_from timestamp without time zone)
    RETURNS void
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE SECURITY DEFINER 
    
AS $BODY$
declare 
    d_fin_attribute_id bigint;
    d_fin_instrument_fin_attribute_id bigint;
    d_count bigint;
    d_value timestamp without time zone;
begin 
    select fin_attribute_id into d_fin_attribute_id 
    from murr_data.fin_attributes
    where fin_ident = upper(in_fin_attribute);
    
    select fin_instrument_fin_attribute_id into d_fin_instrument_fin_attribute_id
    from murr_data.fin_instrument_fin_attribute
    where fin_instrument_id = in_fin_id and fin_attribute_id = d_fin_attribute_id;
    
    if d_fin_instrument_fin_attribute_id is null
    then
        insert into murr_data.fin_instrument_fin_attribute(fin_instrument_id, fin_attribute_id)
        values(in_fin_id, d_fin_attribute_id)
        returning fin_instrument_fin_attribute_id into d_fin_instrument_fin_attribute_id;
    end if;
    
    select 1 into d_count from murr_data.fin_date_value
    where fin_instrument_attribute_id = d_fin_instrument_fin_attribute_id;
    
    if d_count is null
    then 
        insert into murr_data.fin_date_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
        values(d_fin_instrument_fin_attribute_id, in_value, in_date_from);
    else 
        select fin_instrument_value into d_value from 
        (
            select fin_instrument_value, rank() over (PARTITION BY fin_instrument_attribute_id ORDER BY fin_instrument_date_from DESC) as pos
            from murr_data.fin_date_value
            where fin_instrument_attribute_id = d_fin_instrument_fin_attribute_id 
                and fin_instrument_date_from <= in_date_from
        ) as ss
        where pos = 1;
        
        if in_value != d_value
        then
            insert into murr_data.fin_date_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
            values(d_fin_instrument_fin_attribute_id, in_value, in_date_from)
            on conflict(fin_instrument_attribute_id, fin_instrument_date_from) do update
            set fin_instrument_value = in_value;        
        end if;
    end if;        
end
$BODY$;

ALTER FUNCTION murr_data.insert_fin_date_value(bigint, character varying, timestamp without time zone, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_fin_date_value(bigint, character varying, timestamp without time zone, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_fin_date_value(bigint, character varying, timestamp without time zone, timestamp without time zone) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_fin_date_value(bigint, character varying, timestamp without time zone, timestamp without time zone) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_fin_date_value
    IS 'Значения финансовых инструментов(даты)';

--Вставка числовых значений
CREATE OR REPLACE FUNCTION murr_data.insert_fin_numeric_value(
    in_fin_id bigint,
    in_fin_attribute character varying,
    in_value numeric,
    in_date_from timestamp without time zone)
    RETURNS void
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE SECURITY DEFINER 
    
AS $BODY$
declare 
    d_fin_attribute_id bigint;
    d_fin_instrument_fin_attribute_id bigint;
    d_count bigint;
    d_value numeric;
begin 
    --выборка финансового атрибута
    select fin_attribute_id into d_fin_attribute_id 
    from murr_data.fin_attributes
    where fin_ident = upper(in_fin_attribute);
    
    --связка между инструментов и значением
    select fin_instrument_fin_attribute_id into d_fin_instrument_fin_attribute_id
    from murr_data.fin_instrument_fin_attribute
    where fin_instrument_id = in_fin_id and fin_attribute_id = d_fin_attribute_id;
    
    if d_fin_instrument_fin_attribute_id is null
    then
        insert into murr_data.fin_instrument_fin_attribute(fin_instrument_id, fin_attribute_id)
        values(in_fin_id, d_fin_attribute_id)
        returning fin_instrument_fin_attribute_id into d_fin_instrument_fin_attribute_id;
    end if;
    
    select 1 into d_count from murr_data.fin_numeric_value
    where fin_instrument_attribute_id = d_fin_instrument_fin_attribute_id;
    
    --вставка значения
    if d_count is null
    then 
        --если значения не было, то вставляем
        insert into murr_data.fin_numeric_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
        values(d_fin_instrument_fin_attribute_id, in_value, in_date_from);
    else 
        --если значение есть
        select fin_instrument_value into d_value from 
        (
            select fin_instrument_value, rank() over (PARTITION BY fin_instrument_attribute_id ORDER BY fin_instrument_date_from DESC) as pos
            from murr_data.fin_numeric_value
            where fin_instrument_attribute_id = d_fin_instrument_fin_attribute_id 
                and fin_instrument_date_from <= in_date_from
        ) as ss
        where pos = 1;
        
        --вставка значения, если значения расходятся
        if in_value != d_value
        then
            insert into murr_data.fin_numeric_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
            values(d_fin_instrument_fin_attribute_id, in_value, in_date_from)
            on conflict(fin_instrument_attribute_id, fin_instrument_date_from) do update
            set fin_instrument_value = in_value;        
        end if;
    end if;        
end
$BODY$;

ALTER FUNCTION murr_data.insert_fin_numeric_value(bigint, character varying, numeric, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_fin_numeric_value(bigint, character varying, numeric, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_fin_numeric_value(bigint, character varying, numeric, timestamp without time zone) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_fin_numeric_value(bigint, character varying, numeric, timestamp without time zone) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_fin_numeric_value
    IS 'Значения финансовых инструментов (числовых)';


--Вставка числовых значений
CREATE OR REPLACE FUNCTION murr_data.insert_fin_string_value(
    in_fin_id bigint,
    in_fin_attribute character varying,
    in_value character varying,
    in_date_from timestamp without time zone)
    RETURNS void
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE SECURITY DEFINER 
    
AS $BODY$
declare 
    d_fin_attribute_id bigint;
    d_fin_instrument_fin_attribute_id bigint;
    d_count bigint;
    d_value character varying;
begin 
    --выборка финансового атрибута
    select fin_attribute_id into d_fin_attribute_id 
    from murr_data.fin_attributes
    where fin_ident = upper(in_fin_attribute);
    
    --связка между инструментов и значением
    select fin_instrument_fin_attribute_id into d_fin_instrument_fin_attribute_id
    from murr_data.fin_instrument_fin_attribute
    where fin_instrument_id = in_fin_id and fin_attribute_id = d_fin_attribute_id;
    
    if d_fin_instrument_fin_attribute_id is null
    then
        insert into murr_data.fin_instrument_fin_attribute(fin_instrument_id, fin_attribute_id)
        values(in_fin_id, d_fin_attribute_id)
        returning fin_instrument_fin_attribute_id into d_fin_instrument_fin_attribute_id;
    end if;
    
    select 1 into d_count from murr_data.fin_string_value
    where fin_instrument_attribute_id = d_fin_instrument_fin_attribute_id;
    
    --вставка значения
    if d_count is null
    then 
        --если значения не было, то вставляем
        insert into murr_data.fin_string_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
        values(d_fin_instrument_fin_attribute_id, in_value, in_date_from);
    else 
        --если значение есть
        select fin_instrument_value into d_value from 
        (
            select fin_instrument_value, rank() over (PARTITION BY fin_instrument_attribute_id ORDER BY fin_instrument_date_from DESC) as pos
            from murr_data.fin_string_value
            where fin_instrument_attribute_id = d_fin_instrument_fin_attribute_id 
                and fin_instrument_date_from <= in_date_from
        ) as ss
        where pos = 1;
        
        --вставка значения, если значения расходятся
        if in_value != d_value
        then
            insert into murr_data.fin_string_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
            values(d_fin_instrument_fin_attribute_id, in_value, in_date_from)
            on conflict(fin_instrument_attribute_id, fin_instrument_date_from) do update
            set fin_instrument_value = in_value;        
        end if;
    end if;
end
$BODY$;

ALTER FUNCTION murr_data.insert_fin_string_value(bigint, character varying, character varying, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_fin_string_value(bigint, character varying, character varying, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_fin_string_value(bigint, character varying, character varying, timestamp without time zone) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_fin_string_value(bigint, character varying, character varying, timestamp without time zone) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_fin_string_value
    IS 'Значения финансовых инструментов (строковых)';
    
    
--Вставка числовых значений
CREATE OR REPLACE FUNCTION murr_data.insert_time_series(
    in_fin_id bigint,
    in_fin_attribute character varying,
    in_value numeric,
    in_date_from timestamp without time zone)
    RETURNS void
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE SECURITY DEFINER 
    
AS $BODY$
declare 
    d_fin_attribute_id bigint;
    d_fin_instrument_fin_attribute_id bigint;
begin 
    --выборка финансового атрибута
    select fin_attribute_id into d_fin_attribute_id 
    from murr_data.fin_attributes
    where fin_ident = upper(in_fin_attribute);
    
    --связка между инструментов и значением
    select fin_instrument_fin_attribute_id into d_fin_instrument_fin_attribute_id
    from murr_data.fin_instrument_fin_attribute
    where fin_instrument_id = in_fin_id and fin_attribute_id = d_fin_attribute_id;
    
    if d_fin_instrument_fin_attribute_id is null
    then
        insert into murr_data.fin_instrument_fin_attribute(fin_instrument_id, fin_attribute_id)
        values(in_fin_id, d_fin_attribute_id)
        returning fin_instrument_fin_attribute_id into d_fin_instrument_fin_attribute_id;
    end if;
    
    --вставка значений
    insert into murr_data.time_series(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date)
            values(d_fin_instrument_fin_attribute_id, in_value, in_date_from)
            on conflict(fin_instrument_attribute_id, fin_instrument_date) do update
            set fin_instrument_value = in_value;
end
$BODY$;

ALTER FUNCTION murr_data.insert_time_series(bigint, character varying, numeric, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_time_series(bigint, character varying, numeric, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_time_series(bigint, character varying, numeric, timestamp without time zone) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_time_series(bigint, character varying, numeric, timestamp without time zone) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_time_series
    IS 'Значения временной серии для инструментов';
    

--Вставка числовых значений
CREATE OR REPLACE FUNCTION murr_data.insert_fin_instrument(
    in_data_source_id bigint,
    in_fin_ident character varying)
    RETURNS bigint
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE SECURITY DEFINER 
    
AS $BODY$
declare 
    d_fin_instrument_id bigint;
begin 
    select fin_instrument_id into d_fin_instrument_id 
    from murr_data.fin_instruments
    where fin_ident = upper(in_fin_ident) 
        and fin_data_source_id = in_data_source_id;
    
    if d_fin_instrument_id is null 
    then
        insert into murr_data.fin_instruments(fin_ident, fin_data_source_id)
        values(upper(in_fin_ident), in_data_source_id)
        returning fin_instrument_id into d_fin_instrument_id;
    end if;
    
    return d_fin_instrument_id;
end
$BODY$;


ALTER FUNCTION murr_data.insert_fin_instrument(bigint, character varying)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_fin_instrument(bigint, character varying) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_fin_instrument(bigint, character varying) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_fin_instrument(bigint, character varying) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_fin_instrument
    IS 'Формирование финансового инструмента';

--Получение дат
create or replace function murr_data.get_dates(
	in_fin_id bigint,
	in_fin_attribute_id bigint)
	returns table(val timestamp without time zone,
		dat timestamp without time zone)
as $$
declare
begin
	return query
		select fin_instrument_value val, fin_instrument_date_from dat
		from murr_data.fin_instrument_fin_attribute fifa
			join murr_data.fin_date_value fdv on fifa.fin_instrument_fin_attribute_id = fdv.fin_instrument_attribute_id
		where fifa.fin_instrument_id = in_fin_id and fifa.fin_attribute_id = in_fin_attribute_id;
end
$$ language plpgsql
	security definer;
	
alter function murr_data.get_dates
	owner to karma_admin;
	
grant execute on function murr_data.get_dates(bigint, bigint) to karma_admin;

grant execute on function murr_data.get_dates(bigint, bigint) to karma_saver;

revoke all on function murr_data.get_dates(bigint, bigint) from public;

comment on function murr_data.get_dates
    is 'Получаем все историю по датам инструмента';

--Получение чисел
create or replace function murr_data.get_numerics(
	in_fin_id bigint,
	in_fin_attribute_id bigint)
	returns table(val numeric,
		dat timestamp without time zone)
as $$
declare
begin
	return query
		select fin_instrument_value val, fin_instrument_date_from dat
		from murr_data.fin_instrument_fin_attribute fifa
			join murr_data.fin_numeric_value fnv on fifa.fin_instrument_fin_attribute_id = fnv.fin_instrument_attribute_id
		where fifa.fin_instrument_id = in_fin_id and fifa.fin_attribute_id = in_fin_attribute_id;
end
$$ language plpgsql
	security definer;
	
alter function murr_data.get_numerics
	owner to karma_admin;
	
grant execute on function murr_data.get_numerics(bigint, bigint) to karma_admin;

grant execute on function murr_data.get_numerics(bigint, bigint) to karma_saver;

revoke all on function murr_data.get_numerics(bigint, bigint) from public;

comment on function murr_data.get_numerics
    is 'Получаем все историю по числам инструмента';

--Получение строк
create or replace function murr_data.get_strings(
	in_fin_id bigint,
	in_fin_attribute_id bigint)
	returns table(val text,
		dat timestamp without time zone)
as $$
declare
begin
	return query
		select fin_instrument_value val, fin_instrument_date_from dat
		from murr_data.fin_instrument_fin_attribute fifa
			join murr_data.fin_string_value fsv on fifa.fin_instrument_fin_attribute_id = fsv.fin_instrument_attribute_id
		where fifa.fin_instrument_id = in_fin_id and fifa.fin_attribute_id = in_fin_attribute_id;
end
$$ language plpgsql
	security definer;
	
alter function murr_data.get_strings
	owner to karma_admin;
	
grant execute on function murr_data.get_strings(bigint, bigint) to karma_admin;

grant execute on function murr_data.get_strings(bigint, bigint) to karma_saver;

revoke all on function murr_data.get_strings(bigint, bigint) from public;

comment on function murr_data.get_strings
    is 'Получаем все историю по строкам инструмента';


--Получение чисел
create or replace function murr_data.get_timeseries(
	in_fin_id bigint,
	in_fin_attribute_id bigint)
	returns table(val numeric,
		dat timestamp without time zone)
as $$
declare
begin
	return query
		select fin_instrument_value val, fin_instrument_date dat
		from murr_data.fin_instrument_fin_attribute fifa
			join murr_data.time_series ts on fifa.fin_instrument_fin_attribute_id = ts.fin_instrument_attribute_id
		where fifa.fin_instrument_id = in_fin_id and fifa.fin_attribute_id = in_fin_attribute_id;
end
$$ language plpgsql
	security definer;
	
alter function murr_data.get_timeseries
	owner to karma_admin;
	
grant execute on function murr_data.get_timeseries(bigint, bigint) to karma_admin;

grant execute on function murr_data.get_timeseries(bigint, bigint) to karma_saver;

revoke all on function murr_data.get_timeseries(bigint, bigint) from public;

comment on function murr_data.get_timeseries
    is 'Получаем все историю по временным сериям инструмента';
	
	
create or replace function murr_data.get_data_sources()
	returns table(fin_data_source_id bigint,
		fin_data_source_ident character varying)
as $$
declare
begin
	return query
		select fin_data_sources.fin_data_source_id, fin_data_sources.fin_data_source_ident
		from murr_data.fin_data_sources;
end
$$ language plpgsql
	security definer;
	
alter function murr_data.get_data_sources
	owner to karma_admin;
	
grant execute on function murr_data.get_data_sources() to karma_admin;

grant execute on function murr_data.get_data_sources() to karma_saver;

revoke all on function murr_data.get_data_sources() from public;

comment on function murr_data.get_data_sources
    is 'Получаем все источники данных';