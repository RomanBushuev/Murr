

CREATE SEQUENCE murr_data.murr_sequence
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE murr_downloader.murr_sequence
    OWNER TO karma_admin;
	
create table murr_data.moex_attribute_types(
	attribute_type_id bigint not null,
	attribute_type_ident varchar(63) not null,
	constraint pk_attribute_type_id primary key (attribute_type_id)
);

ALTER TABLE murr_data.moex_attribute_types
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.moex_attribute_types
    IS 'Типы атрибутов';

create table murr_data.moex_fin_attributes(
	fin_attribute_id bigint NOT NULL DEFAULT nextval('murr_data.murr_sequence'::regclass),
	fin_ident varchar(63) not null,
	fin_title varchar(255) not null,
	fin_description varchar(1023) not null,
	attribute_type_id bigint not null,
	constraint pk_moex_fin_attributes_fin_attribute_id primary key(fin_attribute_id),
	constraint fk_moex_fin_attributes_moex_attribute_types foreign key(attribute_type_id) 
		references murr_data.moex_attribute_types (attribute_type_id) match simple
		ON UPDATE CASCADE
        ON DELETE CASCADE	
);
	
ALTER TABLE murr_data.moex_fin_attributes
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.moex_fin_attributes
    IS 'Финансовые атрибуты';
	
create table murr_data.moex_fin_instrument_types(
	fin_instrument_type_id bigint not null,
	fin_instrument_type_ident varchar(63) not null,
	constraint pk_moex_fin_instrument_types_fin_instrument_type_id primary key(fin_instrument_type_id)
);
	
ALTER TABLE murr_data.moex_fin_instrument_types
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.moex_fin_instrument_types
    IS 'Типы финансовых инструментов';
	
create table murr_data.moex_fin_instruments(
	fin_instrument_id bigint NOT NULL DEFAULT nextval('murr_data.murr_sequence'::regclass),
	fin_attribute_id bigint not null,
	fin_ident varchar(63) not null,
	fin_instrument_type_id bigint not null,
	constraint u_moex_fin_instruments_fi_fa_fiti unique (fin_attribute_id, fin_ident, fin_instrument_type_id),
	constraint pk_moex_fin_instruments_fin_instrument_id primary key(fin_instrument_id),
	constraint fk_moex_fin_instruments_fin_instrument_types_fin_attribute_id foreign key (fin_attribute_id)
		references murr_data.moex_fin_attributes (fin_attribute_id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
	constraint fk_moex_fi_fit_fin_instrument_type_id foreign key (fin_instrument_type_id)
		references murr_data.moex_fin_instrument_types (fin_instrument_type_id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

ALTER TABLE murr_data.moex_fin_instruments
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.moex_fin_instruments
    IS 'Финансовые инструменты';

create table murr_data.moex_fin_instrument_fin_attribute(
	moex_fin_instrument_fin_attribute_id bigint NOT NULL DEFAULT nextval('murr_data.murr_sequence'::regclass),
	fin_instrument_id bigint not null,
	fin_attribute_id bigint not null,
	constraint pk_moex_fi_fa_fin_instrument_fin_attribute_id primary key (moex_fin_instrument_fin_attribute_id),
	constraint fk_moex_fin_instrument_fin_attribute_moex_fin_instruments foreign key (fin_instrument_id)
		references murr_data.moex_fin_instruments (fin_instrument_id) match simple
		on update cascade
		on delete cascade,
	constraint fk_moex_fin_instrument_fin_attribute_fin_instrument_attributes foreign key (fin_attribute_id)
		references murr_data.moex_fin_attributes (fin_attribute_id) match simple
		on update cascade
		on delete cascade
);

ALTER TABLE murr_data.moex_fin_instrument_fin_attribute
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.moex_fin_instrument_fin_attribute
    IS 'Финансовые инструменты и атрибуты';

create table murr_data.moex_fin_numeric_value(
	fin_instrument_attribute_id bigint not null,
	fin_instrument_value numeric not null,
	fin_instrument_date_from timestamp without time zone NOT NULL,
	constraint u_moex_fin_numeric_value_fiai_fidf unique (fin_instrument_attribute_id, fin_instrument_date_from),
	constraint fk_moex_fin_numeric_value_moex_fin_instrument_fin_attribute foreign key  (fin_instrument_attribute_id)
		references murr_data.moex_fin_instrument_fin_attribute (moex_fin_instrument_fin_attribute_id) match simple
		on update cascade
		on delete cascade
);

ALTER TABLE murr_data.moex_fin_numeric_value
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.moex_fin_numeric_value
    IS 'Числовые значения финансовых инструментов';
	
create table murr_data.moex_fin_string_value(
	fin_instrument_attribute_id bigint not null,
	fin_instrument_value text not null,
	fin_instrument_date_from timestamp without time zone NOT NULL,
	constraint u_moex_fin_string_value_fiai_fidf unique (fin_instrument_attribute_id, fin_instrument_date_from),
	constraint fk_moex_fin_string_value_moex_fin_instrument_fin_attribute foreign key  (fin_instrument_attribute_id)
		references murr_data.moex_fin_instrument_fin_attribute (moex_fin_instrument_fin_attribute_id) match simple
		on update cascade
		on delete cascade
);

ALTER TABLE murr_data.moex_fin_string_value
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.moex_fin_string_value
    IS 'Строковые значения финансовых инструментов';
	
create table murr_data.moex_fin_date_value(
	fin_instrument_attribute_id bigint not null,
	fin_instrument_value timestamp without time zone not null,
	fin_instrument_date_from timestamp without time zone NOT NULL,
	constraint u_moex_fin_date_value_fiai_fidf unique (fin_instrument_attribute_id, fin_instrument_date_from),
	constraint fk_moex_fin_date_value_moex_fin_instrument_fin_attribute foreign key (fin_instrument_attribute_id)
		references murr_data.moex_fin_instrument_fin_attribute (moex_fin_instrument_fin_attribute_id) match simple
		on update cascade
		on delete cascade
);

ALTER TABLE murr_data.moex_fin_date_value
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.moex_fin_date_value
    IS 'Значения дат для финансовых инструментов';
	
	
create table murr_data.moex_time_series(
	fin_instrument_attribute_id bigint not null,
	fin_instrument_value numeric,
	fin_instrument_date timestamp without time zone NOT NULL,
	constraint u_moex_fin_time_series_fiai_fidf unique (fin_instrument_attribute_id, fin_instrument_date),
	constraint fk_fin_moex_time_series_moex_fin_instrument_fin_attribute foreign key (fin_instrument_attribute_id)
		references murr_data.moex_fin_instrument_fin_attribute (moex_fin_instrument_fin_attribute_id) match simple
		on update cascade
		on delete cascade
);

ALTER TABLE murr_data.moex_time_series
    OWNER to karma_admin;
COMMENT ON TABLE murr_data.moex_time_series
    IS 'Временная серия для финансовых инструментов';
	
insert into murr_data.moex_attribute_types
values(1, 'STRING'),
(2, 'DATETIME'),
(3, 'NUMERIC'),
(4, 'TIMESERIES');

insert into murr_data.moex_fin_attributes(fin_ident, fin_title, fin_description, attribute_type_id)
values('SHORTNAME', 'SHORTNAME', 'Краткое наименование', 1),
('ISIN', 'ISIN', 'ISIN', 1),
('LOW','LOW', 'Низшая цена', 4),
('HIGH', 'HIGH', 'Наивысшая цена', 4),
('CLOSE', 'CLOSE', 'Ставка', 4),
('OPEN', 'OPEN', 'Цена открытия', 4),
('VOLUME', 'VOLUME', 'Объем', 4),
('MATURITY_DATE', 'MATURITY_DATE', 'Дата погашения', 2),
('FACEVALUE', 'FACEVALUE', 'Номинал', 3),
('CURRENCY', 'CURRENCY', 'Валюта', 1),
('TRADED', 'TRADED', 'Наторговали объем на ставку', 4);

insert into murr_data.moex_fin_instrument_types(fin_instrument_type_id, fin_instrument_type_ident)
values(1, 'BOND'),
(2, 'SHARE');

CREATE OR REPLACE FUNCTION murr_data.insert_moex_fin_date_value(
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
	d_moex_fin_instrument_fin_attribute_id bigint;
	d_count bigint;
	d_value timestamp without time zone;
begin 
	select fin_attribute_id into d_fin_attribute_id 
	from murr_data.moex_fin_attributes
	where fin_ident = upper(in_fin_attribute);
	
	select moex_fin_instrument_fin_attribute_id into d_moex_fin_instrument_fin_attribute_id
	from murr_data.moex_fin_instrument_fin_attribute
	where fin_instrument_id = in_fin_id and fin_attribute_id = d_fin_attribute_id;
	
	if d_moex_fin_instrument_fin_attribute_id is null
	then
		insert into murr_data.moex_fin_instrument_fin_attribute(fin_instrument_id, fin_attribute_id)
		values(in_fin_id, d_fin_attribute_id)
		returning moex_fin_instrument_fin_attribute_id into d_moex_fin_instrument_fin_attribute_id;
	end if;
	
	select 1 into d_count from murr_data.moex_fin_date_value
	where fin_instrument_attribute_id = d_moex_fin_instrument_fin_attribute_id;
	
	if d_count is null
	then 
		insert into murr_data.moex_fin_date_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
		values(d_moex_fin_instrument_fin_attribute_id, in_value, in_date_from);
	else 
		select fin_instrument_value into d_value from 
		(
			select fin_instrument_value, rank() over (PARTITION BY fin_instrument_attribute_id ORDER BY fin_instrument_date_from DESC) as pos
			from murr_data.moex_fin_date_value
			where fin_instrument_attribute_id = d_moex_fin_instrument_fin_attribute_id 
				and fin_instrument_date_from <= in_date_from
		) as ss
		where pos = 1;
		
		if in_value != d_value
		then
			insert into murr_data.moex_fin_date_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
			values(d_moex_fin_instrument_fin_attribute_id, in_value, in_date_from)
			on conflict(fin_instrument_attribute_id, fin_instrument_date_from) do update
			set fin_instrument_value = in_value;		
		end if;
	end if;		
end
$BODY$;

ALTER FUNCTION murr_data.insert_moex_fin_date_value(bigint, character varying, timestamp without time zone, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_fin_date_value(bigint, character varying, timestamp without time zone, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_fin_date_value(bigint, character varying, timestamp without time zone, timestamp without time zone) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_moex_fin_date_value(bigint, character varying, timestamp without time zone, timestamp without time zone) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_moex_fin_date_value
    IS 'Значения финансовых инструментов(даты)';

--Вставка числовых значений
CREATE OR REPLACE FUNCTION murr_data.insert_moex_fin_numeric_value(
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
	d_moex_fin_instrument_fin_attribute_id bigint;
	d_count bigint;
	d_value numeric;
begin 
	--выборка финансового атрибута
	select fin_attribute_id into d_fin_attribute_id 
	from murr_data.moex_fin_attributes
	where fin_ident = upper(in_fin_attribute);
	
	--связка между инструментов и значением
	select moex_fin_instrument_fin_attribute_id into d_moex_fin_instrument_fin_attribute_id
	from murr_data.moex_fin_instrument_fin_attribute
	where fin_instrument_id = in_fin_id and fin_attribute_id = d_fin_attribute_id;
	
	if d_moex_fin_instrument_fin_attribute_id is null
	then
		insert into murr_data.moex_fin_instrument_fin_attribute(fin_instrument_id, fin_attribute_id)
		values(in_fin_id, d_fin_attribute_id)
		returning moex_fin_instrument_fin_attribute_id into d_moex_fin_instrument_fin_attribute_id;
	end if;
	
	select 1 into d_count from murr_data.moex_fin_numeric_value
	where fin_instrument_attribute_id = d_moex_fin_instrument_fin_attribute_id;
	
	--вставка значения
	if d_count is null
	then 
		--если значения не было, то вставляем
		insert into murr_data.moex_fin_numeric_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
		values(d_moex_fin_instrument_fin_attribute_id, in_value, in_date_from);
	else 
		--если значение есть
		select fin_instrument_value into d_value from 
		(
			select fin_instrument_value, rank() over (PARTITION BY fin_instrument_attribute_id ORDER BY fin_instrument_date_from DESC) as pos
			from murr_data.moex_fin_numeric_value
			where fin_instrument_attribute_id = d_moex_fin_instrument_fin_attribute_id 
				and fin_instrument_date_from <= in_date_from
		) as ss
		where pos = 1;
		
		--вставка значения, если значения расходятся
		if in_value != d_value
		then
			insert into murr_data.moex_fin_numeric_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
			values(d_moex_fin_instrument_fin_attribute_id, in_value, in_date_from)
			on conflict(fin_instrument_attribute_id, fin_instrument_date_from) do update
			set fin_instrument_value = in_value;		
		end if;
	end if;		
end
$BODY$;

ALTER FUNCTION murr_data.insert_moex_fin_numeric_value(bigint, character varying, numeric, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_fin_numeric_value(bigint, character varying, numeric, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_fin_numeric_value(bigint, character varying, numeric, timestamp without time zone) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_moex_fin_numeric_value(bigint, character varying, numeric, timestamp without time zone) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_moex_fin_numeric_value
    IS 'Значения финансовых инструментов (числовых)';


--Вставка числовых значений
CREATE OR REPLACE FUNCTION murr_data.insert_moex_fin_string_value(
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
	d_moex_fin_instrument_fin_attribute_id bigint;
	d_count bigint;
	d_value character varying;
begin 
	--выборка финансового атрибута
	select fin_attribute_id into d_fin_attribute_id 
	from murr_data.moex_fin_attributes
	where fin_ident = upper(in_fin_attribute);
	
	--связка между инструментов и значением
	select moex_fin_instrument_fin_attribute_id into d_moex_fin_instrument_fin_attribute_id
	from murr_data.moex_fin_instrument_fin_attribute
	where fin_instrument_id = in_fin_id and fin_attribute_id = d_fin_attribute_id;
	
	if d_moex_fin_instrument_fin_attribute_id is null
	then
		insert into murr_data.moex_fin_instrument_fin_attribute(fin_instrument_id, fin_attribute_id)
		values(in_fin_id, d_fin_attribute_id)
		returning moex_fin_instrument_fin_attribute_id into d_moex_fin_instrument_fin_attribute_id;
	end if;
	
	select 1 into d_count from murr_data.moex_fin_string_value
	where fin_instrument_attribute_id = d_moex_fin_instrument_fin_attribute_id;
	
	--вставка значения
	if d_count is null
	then 
		--если значения не было, то вставляем
		insert into murr_data.moex_fin_string_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
		values(d_moex_fin_instrument_fin_attribute_id, in_value, in_date_from);
	else 
		--если значение есть
		select fin_instrument_value into d_value from 
		(
			select fin_instrument_value, rank() over (PARTITION BY fin_instrument_attribute_id ORDER BY fin_instrument_date_from DESC) as pos
			from murr_data.moex_fin_string_value
			where fin_instrument_attribute_id = d_moex_fin_instrument_fin_attribute_id 
				and fin_instrument_date_from <= in_date_from
		) as ss
		where pos = 1;
		
		--вставка значения, если значения расходятся
		if in_value != d_value
		then
			insert into murr_data.moex_fin_string_value(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date_from)
			values(d_moex_fin_instrument_fin_attribute_id, in_value, in_date_from)
			on conflict(fin_instrument_attribute_id, fin_instrument_date_from) do update
			set fin_instrument_value = in_value;		
		end if;
	end if;		
end
$BODY$;

ALTER FUNCTION murr_data.insert_moex_fin_string_value(bigint, character varying, numeric, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_fin_string_value(bigint, character varying, numeric, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_fin_string_value(bigint, character varying, numeric, timestamp without time zone) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_moex_fin_string_value(bigint, character varying, numeric, timestamp without time zone) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_moex_fin_string_value
    IS 'Значения финансовых инструментов (строковых)';
	
	
--Вставка числовых значений
CREATE OR REPLACE FUNCTION murr_data.insert_moex_time_series(
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
	d_moex_fin_instrument_fin_attribute_id bigint;
begin 
	--выборка финансового атрибута
	select fin_attribute_id into d_fin_attribute_id 
	from murr_data.moex_fin_attributes
	where fin_ident = upper(in_fin_attribute);
	
	--связка между инструментов и значением
	select moex_fin_instrument_fin_attribute_id into d_moex_fin_instrument_fin_attribute_id
	from murr_data.moex_fin_instrument_fin_attribute
	where fin_instrument_id = in_fin_id and fin_attribute_id = d_fin_attribute_id;
	
	if d_moex_fin_instrument_fin_attribute_id is null
	then
		insert into murr_data.moex_fin_instrument_fin_attribute(fin_instrument_id, fin_attribute_id)
		values(in_fin_id, d_fin_attribute_id)
		returning moex_fin_instrument_fin_attribute_id into d_moex_fin_instrument_fin_attribute_id;
	end if;
	
	--вставка значений
	insert into murr_data.moex_time_series(fin_instrument_attribute_id, fin_instrument_value, fin_instrument_date)
			values(d_moex_fin_instrument_fin_attribute_id, in_value, in_date_from)
			on conflict(fin_instrument_attribute_id, fin_instrument_date) do update
			set fin_instrument_value = in_value;
end
$BODY$;

ALTER FUNCTION murr_data.insert_moex_time_series(bigint, character varying, numeric, timestamp without time zone)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_time_series(bigint, character varying, numeric, timestamp without time zone) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_time_series(bigint, character varying, numeric, timestamp without time zone) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_moex_time_series(bigint, character varying, numeric, timestamp without time zone) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_moex_time_series
    IS 'Значения временной серии для инструментов';
	

--Вставка числовых значений
CREATE OR REPLACE FUNCTION murr_data.insert_moex_fin_instrument(
	in_fin_attribute_id bigint,
	in_fin_ident character varying,
	in_fin_instrument_type_id bigint)
    RETURNS bigint
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE SECURITY DEFINER 
    
AS $BODY$
declare 
	d_fin_instrument_id bigint;
begin 
	select fin_instrument_id into d_fin_instrument_id 
	from murr_data.moex_fin_instruments
	where fin_ident = upper(in_fin_ident) 
		and fin_attribute_id = in_fin_attribute_id 
		and fin_instrument_type_id = in_fin_instrument_type_id;
	
	if d_fin_instrument_id is null 
	then
		insert into murr_data.moex_fin_instruments(fin_ident, fin_attribute_id, fin_instrument_type_id)
		values(upper(in_fin_ident), in_fin_attribute_id, in_fin_instrument_type_id)
		returning fin_instrument_id into d_fin_instrument_id;
	end if;
	
	return d_fin_instrument_id;
end
$BODY$;


ALTER FUNCTION murr_data.insert_moex_fin_instrument(bigint, character varying, bigint)
    OWNER TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_fin_instrument(bigint, character varying, bigint) TO karma_admin;

GRANT EXECUTE ON FUNCTION murr_data.insert_moex_fin_instrument(bigint, character varying, bigint) TO karma_saver;

REVOKE ALL ON FUNCTION murr_data.insert_moex_fin_instrument(bigint, character varying, bigint) FROM PUBLIC;

COMMENT ON FUNCTION murr_data.insert_moex_fin_instrument
    IS 'Формирование финансового инструмента';

