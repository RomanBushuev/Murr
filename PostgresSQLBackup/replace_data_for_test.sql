update murr_downloader.default_saver_templates
set default_path = 'C:\Users\homan\YandexDisk\Murr_test\Cbr\ForeignExchange'
where default_saver_template_id = 59;

update murr_downloader.default_saver_templates
set default_path = 'C:\Users\homan\YandexDisk\Murr_test\Cbr\MosPrime'
where default_saver_template_id = 1162;

update murr_downloader.default_saver_templates
set default_path = 'C:\Users\homan\YandexDisk\Murr_test\Cbr\Keyrate'
where default_saver_template_id = 1617;

update murr_downloader.default_saver_templates
set default_path = 'C:\Users\homan\YandexDisk\Murr_test\Cbr\Ruonia'
where default_saver_template_id = 1618;

update murr_downloader.default_saver_templates
set default_path = 'C:\Users\homan\YandexDisk\Murr_test\Cbr\Ruonia'
where default_saver_template_id = 1618;

update murr_downloader.default_saver_templates
set default_path = 'C:\Users\homan\YandexDisk\Murr_test\Moex\Shares'
where default_saver_template_id = 2476;

update murr_downloader.default_saver_templates
set default_path = 'C:\Users\homan\YandexDisk\Murr_test\Moex\Bonds'
where default_saver_template_id = 2477;

insert into murr_data.fin_data_sources(fin_data_source_id, fin_data_source_ident)
values(0, 'UNDEFINED');