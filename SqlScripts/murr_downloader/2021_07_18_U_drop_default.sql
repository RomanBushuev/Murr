alter table murr_downloader.task_types
alter task_type_id drop default;

insert into murr_downloader.task_types(task_type_id, task_type_title, task_type_description)
values(8, 'SAVE CURRENCIES CBRF', 'Сохранение валюты ЦБ');