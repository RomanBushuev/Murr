REVOKE ALL ON SCHEMA murr_downloader FROM PUBLIC;
GRANT USAGE ON SCHEMA murr_downloader  TO karma_downloader;

--ACCESS TABLES
REVOKE ALL ON ALL TABLES IN SCHEMA murr_downloader FROM PUBLIC;
GRANT USAGE ON SCHEMA murr_downloader TO karma_downloader;

REVOKE ALL ON ALL TABLES IN SCHEMA murr_downloader FROM karma_downloader ;