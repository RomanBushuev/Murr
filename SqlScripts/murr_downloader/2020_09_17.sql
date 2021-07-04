CREATE SEQUENCE murr_downloader.murr_sequence 
AS BIGINT 
INCREMENT 1
MINVALUE 1;

--В статусах вставка происходит только человеком
create table murr_downloader.task_statuses
(
	task_status_id bigint,
	task_status_title varchar(255) not null,
	task_statuses_description varchar(1023) not null,
	constraint pk_task_statuses_id primary key(task_status_id),
	constraint u_task_statuses_title unique(task_status_title)
);
comment on table murr_downloader.task_statuses is 'Статусы для задач';

alter table murr_downloader.task_statuses OWNER TO karma_admin;
grant select on murr_downloader.task_statuses to karma_downloader;

insert into murr_downloader.task_statuses(task_status_id,
	task_status_title,
	task_statuses_description)
values(0, 'UNDEFINED', 'Неопределено'),
(1, 'CREATING', 'В состоянии создания'),
(2, 'CREATED', 'Создан'),
(3, 'RUNNING', 'Запущен'),
(4, 'DONE', 'Выполнен'),
(5, 'ERROR', 'Закончен с ошибками');

create table murr_downloader.task_types
(
	task_type_id bigint default nextval('murr_downloader.murr_sequence'),
	task_type_title varchar(255) not null,
	task_type_description varchar(1023) not null,
	constraint pk_task_types_id primary key (task_type_id),
	constraint u_task_types_title unique (task_type_title)
);
comment on table murr_downloader.task_types is 'Типы задач';

alter table murr_downloader.task_types OWNER TO karma_admin;
grant select on murr_downloader.task_types to karma_downloader;

insert into murr_downloader.task_types(task_type_id, 
	task_type_title,
	task_type_description)
values(0, 'UNDEFINED', 'Неопределено'),
(1, 'DOWNLOAD CURRENCIES CBRF', 'Загрузка валюты из ЦБ'),
(2, 'DOWNLOAD G2 CURVE CBRF', 'Загрузка кривой G2 из ЦБ');

create table murr_downloader.folder_types
(
	folder_type_id bigint default nextval('murr_downloader.murr_sequence'),
	folder_type_title varchar(255) not null,
	folder_type_description varchar(1023) not null,
	constraint pk_folder_types_id primary key (folder_type_id),
	constraint u_folder_types_title unique(folder_type_title)
);
comment on table murr_downloader.folder_types is 'Типы папок';

alter table murr_downloader.folder_types OWNER TO karma_admin;
grant select on murr_downloader.folder_types to karma_downloader;

insert into murr_downloader.folder_types(folder_type_id,
	folder_type_title,
	folder_type_description)
values(0 , 'UNDEFINED', 'Неопределено'),
(1, 'DOWNLOAD', 'Загрузчик');

create table murr_downloader.folders
(
	folder_id bigint default nextval('murr_downloader.murr_sequence'),
	folder_root_id bigint null,
	folder_title varchar(255),
	constraint pk_folders_folder_id primary key (folder_id),
	constraint fk_folders_folder_id_folder_root_id foreign key(folder_root_id)
		references murr_downloader.folders (folder_id) 
		on delete cascade
		on update cascade
);
comment on table murr_downloader.folders is 'Папки';

alter table murr_downloader.folders OWNER TO karma_admin;
grant select on murr_downloader.folders to karma_downloader;

--пул задач
create table murr_downloader.task_templates
(
	task_template_id bigint default nextval('murr_downloader.murr_sequence'),
	task_template_title varchar(255) not null,
	task_template_created_time timestamp without time zone not null,
	task_template_folder_id bigint not null,
	task_parameters jsonb not null,
	task_type_id bigint not null,
	constraint fk_folders_task_template_folder_id foreign key(task_template_folder_id)
		references murr_downloader.folders(folder_id)
		on delete cascade
		on update cascade,
	constraint fk_task_types_task_templates_id foreign key(task_type_id)
		references murr_downloader.task_types(task_type_id)
);
comment on table murr_downloader.task_templates is 'Шаблоны задач';

alter table murr_downloader.task_templates OWNER TO karma_admin;
grant select on murr_downloader.task_templates to karma_downloader;

create table murr_downloader.tasks
(
	task_id bigint default nextval('murr_downloader.murr_sequence'),
	task_template_id bigint,
	task_created_time timestamp without time zone not null,
	task_status_id bigint,
	constraint fk_task_statuses_tasks_status_id foreign key(task_status_id)
		references murr_downloader.task_statuses(task_status_id)
		on delete cascade
		on update cascade,
	constraint pk_tasks_task_id primary key(task_id)
);
comment on table murr_downloader.tasks is 'Задачи, которые необходимо выполнить';

alter table murr_downloader.tasks OWNER TO karma_admin;
grant select on murr_downloader.tasks to karma_downloader;

--атриубуты для задач
create table murr_downloader.task_attributes
(
	task_attribute_id bigint default nextval('murr_downloader.murr_sequence'),
	task_attribute_title varchar(255) not null,
	task_attribute_description varchar(1023) not null,
	constraint u_task_attributes_task_attribute_title unique(task_attribute_title),
	constraint pk_task_attributes_task_attribute_id primary key(task_attribute_id)
);
comment on table murr_downloader.task_attributes is 'Атрибуты для задач';

alter table murr_downloader.task_attributes OWNER TO karma_admin;
grant select on murr_downloader.task_attributes to karma_downloader;

create table murr_downloader.tasks_task_attributes
(
	task_task_attribute_id bigint default nextval('murr_downloader.murr_sequence'),
	task_id bigint,
	task_attribute_id bigint,
	constraint fk_tasks_task_attributes_task_id foreign key(task_id)
		references murr_downloader.tasks(task_id)
		on delete cascade
		on update cascade,
	constraint fk_tasks_task_attributes_task_attribute_id foreign key(task_attribute_id)
		references murr_downloader.task_attributes(task_attribute_id)
		on delete cascade
		on update cascade,
	constraint pk_tasks_task_attributes_task_task_attribute_id primary key(task_task_attribute_id),
	constraint u_tasks_task_attributes_task_id_task_attribute_id unique(task_id, task_attribute_id)
);
comment on table murr_downloader.tasks_task_attributes is 'Таблица для генерирования уникального ключа для задач и атрибутов для задач';

alter table murr_downloader.tasks_task_attributes OWNER TO karma_admin;
grant select on murr_downloader.tasks_task_attributes to karma_downloader;

create table murr_downloader.task_attribute_strings
(
	task_task_attribute_id bigint not null,
	task_task_attribute_title varchar(255) not null,
	constraint fk_task_attribute_strings_task_task_attribute_id foreign key (task_task_attribute_id)
		references murr_downloader.tasks_task_attributes(task_task_attribute_id)
		on delete cascade
		on update cascade
);
comment on table murr_downloader.task_attribute_strings is 'Строковые значения для задач';

alter table murr_downloader.task_attribute_strings OWNER TO karma_admin;
grant select on murr_downloader.task_attribute_strings to karma_downloader;

create table murr_downloader.task_attribute_numerics 
(
	task_task_attribute_id bigint not null,
	task_task_attribute_numeric numeric not null,
	constraint fk_task_attribute_numerics_task_task_attribute_id foreign key (task_task_attribute_id)
		references murr_downloader.tasks_task_attributes(task_task_attribute_id)
		on delete cascade
		on update cascade
);
comment on table murr_downloader.task_attribute_numerics is 'Числовые значения для задач';

alter table murr_downloader.task_attribute_numerics OWNER TO karma_admin;
grant select on murr_downloader.task_attribute_numerics to karma_downloader;

create table murr_downloader.task_attribute_dates 
(
	task_task_attribute_id bigint not null,
	task_task_attribute_date timestamp without time zone not null,
	constraint fk_task_attribute_dates_task_task_attribute_id foreign key (task_task_attribute_id)
		references murr_downloader.tasks_task_attributes(task_task_attribute_id)
		on delete cascade
		on update cascade
);
comment on table murr_downloader.task_attribute_dates is 'Значения времени для задач';

alter table murr_downloader.task_attribute_dates OWNER TO karma_admin;
grant select on murr_downloader.task_attribute_dates to karma_downloader;

create table murr_downloader.task_attribute_date_strings
(
	task_task_attribute_id bigint not null,
	task_task_attribute_date timestamp without time zone not null,
	task_task_attribute_title varchar(255) not null,
	constraint fk_task_attribute_date_strings_task_task_attribute_id foreign key (task_task_attribute_id)
		references murr_downloader.tasks_task_attributes(task_task_attribute_id)
		on delete cascade
		on update cascade
);
comment on table murr_downloader.task_attribute_date_strings is 'Значения для даты и времени для задач';

alter table murr_downloader.task_attribute_date_strings OWNER TO karma_admin;
grant select on murr_downloader.task_attribute_date_strings to karma_downloader;

--сервисы 
create table murr_downloader.service_statuses
(
	service_status_id bigint,
	service_status_title varchar(255) not null,
	service_status_description varchar(1023) not null,
	constraint pk_service_statuses_service_status_id primary key(service_status_id)
);
comment on table murr_downloader.service_statuses is 'Статусы сервисов';

insert into murr_downloader.service_statuses(service_status_id,
	service_status_title,
	service_status_description)
values(0, 'UNDEFINED', 'Неопределено'),
(1, 'RUNNING', 'Запущен'),
(2, 'STOPPING', 'Остановлен');

alter table murr_downloader.service_statuses OWNER TO karma_admin;
grant select on murr_downloader.service_statuses to karma_downloader;

create table murr_downloader.services
(
	service_id bigint default nextval('murr_downloader.murr_sequence'),
	service_title varchar(255) not null,
	service_status_id bigint not null,
	constraint u_services_service_title unique(service_title),
	constraint pk_services_service_id primary key(service_id),
	constraint fk_services_service_statuses_service_status_id foreign key(service_status_id)
		references murr_downloader.service_statuses(service_status_id)
		on delete cascade
		on update cascade
);
comment on table murr_downloader.services is 'Сервисы';

alter table murr_downloader.services OWNER TO karma_admin;
grant select on murr_downloader.services to karma_downloader;


--атрибуты для сервисов 
create table murr_downloader.service_attributes
(
	service_attribute_id bigint default nextval('murr_downloader.murr_sequence') not null,
	service_attribute_title varchar(255) not null,
	service_attribute_description varchar(1023) not null,
	constraint u_service_attributes_service_attribute_title unique(service_attribute_title),
	constraint pk_service_attributes_service_attribute_id primary key(service_attribute_id)
);
comment on table murr_downloader.service_attributes is 'Атрибуты для сервисов';

alter table murr_downloader.service_attributes OWNER TO karma_admin;
grant select on murr_downloader.service_attributes to karma_downloader;


create table murr_downloader.services_service_attributes
(
	service_service_attribute_id bigint default nextval('murr_downloader.murr_sequence'),
	service_id bigint,
	service_attribute_id bigint,
	constraint fk_services_service_attributes_service_id foreign key(service_id)
		references murr_downloader.services(service_id)
		on delete cascade
		on update cascade,
	constraint fk_services_service_attributes_service_attribute_id foreign key(service_attribute_id)
		references murr_downloader.service_attributes(service_attribute_id)
		on delete cascade
		on update cascade,
	constraint pk_services_service_attributes_service_service_attribute_id primary key(service_service_attribute_id),
	constraint u_services_service_attributes_service_id_service_attribute_id unique(service_id, service_attribute_id)
);
comment on table murr_downloader.services_service_attributes is 'Таблица для генерирования уникального ключа для сервисов и атрибутов для сервисов';

--строки
--время
--числа
--логи
create table murr_downloader.service_attribute_strings
(
	service_service_attribute_id bigint not null,
	service_service_attribute_title varchar(255) not null,
	constraint fk_service_attribute_strings_service_service_attribute_id foreign key (service_service_attribute_id)
		references murr_downloader.services_service_attributes(service_service_attribute_id)
		on delete cascade
		on update cascade
);
comment on table murr_downloader.service_attribute_strings is 'Строковые значения для сервисов';

alter table murr_downloader.service_attribute_strings OWNER TO karma_admin;
grant select on murr_downloader.service_attribute_strings to karma_downloader;

create table murr_downloader.service_attribute_numerics 
(
	service_service_attribute_id bigint not null,
	service_service_attribute_numeric numeric not null,
	constraint fk_service_attribute_numerics_service_service_attribute_id foreign key (service_service_attribute_id)
		references murr_downloader.services_service_attributes(service_service_attribute_id)
		on delete cascade
		on update cascade
);
comment on table murr_downloader.service_attribute_numerics is 'Числовые значения для сервисов';

alter table murr_downloader.service_attribute_numerics OWNER TO karma_admin;
grant select on murr_downloader.service_attribute_numerics to karma_downloader;

create table murr_downloader.service_attribute_dates 
(
	service_service_attribute_id bigint not null,
	service_service_attribute_date timestamp without time zone not null,
	constraint fk_service_attribute_dates_service_service_attribute_id foreign key (service_service_attribute_id)
		references murr_downloader.services_service_attributes(service_service_attribute_id)
		on delete cascade
		on update cascade
);
comment on table murr_downloader.service_attribute_dates is 'Значения времени для сервисов';

alter table murr_downloader.service_attribute_dates OWNER TO karma_admin;
grant select on murr_downloader.service_attribute_dates to karma_downloader;

create table murr_downloader.service_attribute_date_strings
(
	service_service_attribute_id bigint not null,
	service_service_attribute_date timestamp without time zone not null,
	service_service_attribute_title varchar(255) not null,
	constraint fk_task_attribute_date_strings_task_task_attribute_id foreign key (service_service_attribute_id)
		references murr_downloader.services_service_attributes(service_service_attribute_id)
		on delete cascade
		on update cascade
);
comment on table murr_downloader.service_attribute_date_strings is 'Значения для даты и времени для сервисов';

alter table murr_downloader.service_attribute_date_strings OWNER TO karma_admin;
grant select on murr_downloader.service_attribute_date_strings to karma_downloader;


