using DownloaderProvider.DatabaseEntities;
using DownloaderProvider.DbFunctions;
using KarmaCore.Entities;
using KarmaCore.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DownloaderProvider
{
    public class ServiceActions : IServiceActions
    {
        private readonly string _connection;

        public ServiceActions(string connection)
        {
            _connection = connection;
        }

        public void SetAttribute(string serviceName, string attribute, string text)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                KarmaDownloaderFunctions.InsertServiceString(connection,
                    serviceName,
                    attribute.Trim().ToUpper(),
                    text);
            }
        }

        public void SetAttribute(string serviceName, string attribute, decimal number)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                KarmaDownloaderFunctions.InsertServiceNumeric(connection,
                    serviceName,
                    attribute.Trim().ToUpper(),
                    number);
            }
        }

        public void SetAttribute(string serviceName, string attribute, DateTime date)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                KarmaDownloaderFunctions.InsertServiceDate(connection,
                    serviceName,
                    attribute.Trim().ToUpper(),
                    date);
            }
        }


        public void SetAttribute(string serviceName, string attribute, DateTime date, string text)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                KarmaDownloaderFunctions.InsertServiceDateString(connection,
                    serviceName,
                    attribute.Trim().ToUpper(),
                    date, 
                    text);
            }
        }

        public string GetString(string serviceName, string attribute)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                return KarmaDownloaderFunctions.GetServiceString(connection,
                    serviceName,
                    attribute.Trim().ToUpper());
            }
        }

        public decimal? GetNumber(string serviceName, string attribute)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                return KarmaDownloaderFunctions.GetServiceDecimal(connection,
                    serviceName,
                    attribute.Trim().ToUpper());
            }
        }

        public DateTime? GetDate(string serviceName, string attribute)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                return KarmaDownloaderFunctions.GetServiceDate(connection,
                    serviceName,
                    attribute.Trim().ToUpper());
            }
        }

        public List<KarmaService> GetKarmaServices()
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                var result = KarmaDownloaderFunctions.DownloadKarmaServices(connection);
                return ConverterDto.ConvertDto<DbKarmaService, KarmaService>(result).ToList();
            }
        }

        public long Initialize(string service)
        {
            using (var connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                var result = KarmaDownloaderFunctions.CreateService(connection, service);
                return result;
            }
        }
    }
}
