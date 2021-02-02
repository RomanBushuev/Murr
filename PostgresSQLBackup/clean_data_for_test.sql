update murr_downloader.default_saver_templates
set default_path = 'C:\Yandex\YandexDisk\Murr_test\Cbr\ForeignExchange'
where default_saver_template_id = 59;

update murr_downloader.default_saver_templates
set default_path = 'C:\Yandex\YandexDisk\Murr_test\Cbr\MosPrime'
where default_saver_template_id = 1162;

update murr_downloader.default_saver_templates
set default_path = 'C:\Yandex\YandexDisk\Murr_test\Cbr\Keyrate'
where default_saver_template_id = 1617;

update murr_downloader.default_saver_templates
set default_path = 'C:\Yandex\YandexDisk\Murr_test\Cbr\Ruonia'
where default_saver_template_id = 1618;

update murr_downloader.default_saver_templates
set default_path = 'C:\Yandex\YandexDisk\Murr_test\Cbr\Roisfix'
where default_saver_template_id = 1619;


delete from murr_downloader.services;
delete from murr_downloader.services_service_attributes;
delete from murr_downloader.service_attribute_date_strings;
delete from murr_downloader.service_attribute_dates;
delete from murr_downloader.service_attribute_numerics;
delete from murr_downloader.service_attribute_strings;

delete from murr_downloader.tasks;
delete from murr_downloader.tasks_task_attributes;
delete from murr_downloader.task_attribute_date_strings;
delete from murr_downloader.task_attribute_dates;
delete from murr_downloader.task_attribute_numerics;
delete from murr_downloader.task_attribute_strings;

delete from murr_downloader.procedure_tasks;
delete from murr_downloader.procedure_tasks_history;
delete from murr_downloader.saver_templates;
delete from murr_downloader.task_templates;