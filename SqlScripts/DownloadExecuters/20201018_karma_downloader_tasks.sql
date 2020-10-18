create table murr_downloader.saver_types 
(
	saver_type_id bigint not null,
	saver_type_title character varying(255) NOT NULL,
	saver_type_description character varying(1023) not null,
	constraint pk_saver_types primary key (saver_type_id),
	constraint u_saver_types_title unique(saver_type_title)
);

alter table murr_downloader.saver_types owner to karma_admin;
comment on table murr_downloader.saver_types is 'Типы сохранений';

insert into murr_downloader.saver_types(saver_type_id,
	saver_type_title,
	saver_type_description)
values(1, 'XML_FILE_SAVER', 'Сохранение XML файлов');

insert into murr_downloader.folder_types(folder_type_id,
	folder_type_title,
	folder_type_description)
values(2, 'XML_FILE_SAVER', 'Сохранение XML файлов'); 

create table murr_downloader.saver_templates
(
	saver_template_id bigint not null DEFAULT nextval('murr_downloader.murr_sequence'::regclass),
	saver_template_title character varying(255) not null,
	saver_template_created_time timestamp without time zone not null,
	saver_template_folder_id bigint not null,
	saver_parameters jsonb not null,
	saver_type_id bigint not null,
	constraint pk_saver_templates primary key(saver_template_id),
	constraint fk_folders_saver_template_folder_id foreign key(saver_template_folder_id)
		references murr_downloader.folders(folder_id) match simple 
		on update cascade
		on delete cascade,
	constraint fk_saver_types_saver_type_id foreign key (saver_type_id)
		references murr_downloader.saver_types(saver_type_id) match simple
		on update cascade
		on delete cascade
);

alter table murr_downloader.saver_templates owner to karma_admin;
comment on table murr_downloader.saver_templates is 'Шаблоны для сохранений';

alter table murr_downloader.tasks 
add saver_template_id bigint;

alter table murr_downloader.tasks
add constraint fk_tasks_saver_template_id foreign key (saver_template_id)
	references murr_downloader.saver_templates(saver_template_id) match simple	
	on update cascade 
	on delete cascade;
	
create table murr_downloader.defalult_saver_templates
(
	default_saver_template_id bigint not null DEFAULT nextval('murr_downloader.murr_sequence'::regclass),
	task_type_id bigint not null,
	default_path character varying(2047) not null,
	constraint pk_defalult_saver_templates primary key(default_saver_template_id),
	constraint fk_task_types_task_type_id foreign key(task_type_id)
		references murr_downloader.task_types(task_type_id) match simple
		on update cascade
		on delete cascade
);

alter table murr_downloader.defalult_saver_templates owner to karma_admin;
comment on table murr_downloader.defalult_saver_templates is 'Пути по умолчанию к задачам загрузки';

insert into murr_downloader.defalult_saver_templates(task_type_id, default_path)
values(1, 'C:\Yandex\YandexDisk\Murr\Cbr\ForeignExchange');