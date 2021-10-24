﻿using AutoMapper;
using Murzik.DownloaderProvider.DbFunctions;
using Murzik.Entities;
using Murzik.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Murzik.DownloaderProvider
{
    public class ServiceActions : IServiceActions
    {
        private readonly string _connection;
        private IMapper _mapper;

        public ServiceActions(string connection,
            IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public void SetAttribute(string serviceName, string attribute, string text)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
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
                return KarmaDownloaderFunctions.GetServiceString(connection,
                    serviceName,
                    attribute.Trim().ToUpper());
            }
        }

        public decimal? GetNumber(string serviceName, string attribute)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                return KarmaDownloaderFunctions.GetServiceDecimal(connection,
                    serviceName,
                    attribute.Trim().ToUpper());
            }
        }

        public DateTime? GetDate(string serviceName, string attribute)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                return KarmaDownloaderFunctions.GetServiceDate(connection,
                    serviceName,
                    attribute.Trim().ToUpper());
            }
        }

        public IReadOnlyCollection<KarmaService> GetKarmaServices()
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                var result = KarmaDownloaderFunctions.DownloadKarmaServices(connection);
                return result.Select(z => _mapper.Map<KarmaService>(z)).ToList();
            }
        }
    }
}
