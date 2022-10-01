insert into murr_data.fin_data_sources(fin_data_source_id, fin_data_source_ident)
values(2, 'MOEX');

insert into murr_data.fin_attributes(fin_attribute_id, fin_ident, fin_title, fin_description, fin_attribute_type_id)
values(14, 'ISSUE_DATE', 'ISSUE_DATE', 'Дата выпуска', 2),
(15, 'ISSUE_SIZE', 'ISSUE_SIZE', 'Размер выпуска', 3),
(16, 'REGNUMBER', 'REGNUMBER', 'Регистрационный номер', 1);

insert into murr_data.fin_attributes(fin_attribute_id, fin_ident, fin_title, fin_description, fin_attribute_type_id)
values(17, 'VALUE_RUB', 'VALUE_RUB', 'Объем сделок в рублях', 4),
(18, 'NUMTRADES', 'NUMTRADES', 'Количество сделок', 4),
(19, 'WAPRICE', 'WAPRICE', 'Средневзвешенная цена', 4);