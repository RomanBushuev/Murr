create index idx_fin_attribute_types_id on murr_data.fin_attribute_types(fin_attribute_type_id);
create index idx_fin_attributes_id on murr_data.fin_attributes(fin_attribute_id);
create index idx_fin_data_sources_id on murr_data.fin_data_sources(fin_data_source_id);
create index idx_fin_date_value_id on murr_data.fin_date_value(fin_instrument_attribute_id, fin_instrument_date_from);
create index idx_fin_instrument_fin_attribute_fin_attr_id on murr_data.fin_instrument_fin_attribute(fin_instrument_id, fin_attribute_id);
create index idx_fin_instruments_fin_id on murr_data.fin_instruments(fin_instrument_id);
create index idx_fin_instruments_fin_ds_id on murr_data.fin_instruments(fin_instrument_id, fin_data_source_id);
create index idx_fin_numeric_value_id_from on murr_data.fin_numeric_value(fin_instrument_attribute_id, fin_instrument_date_from);
create index idx_fin_string_value_id_from on murr_data.fin_string_value(fin_instrument_attribute_id, fin_instrument_date_from);
create index idx_time_series_id_date on murr_data.time_series(fin_instrument_attribute_id, fin_instrument_date);