using KarmaCore.Entities;
using System;
using System.Collections.Generic;

namespace KarmaCore.Interfaces
{
    public interface IServiceActions
    {
        void SetAttribute(string serviceName, string attribute, string text);

        void SetAttribute(string serviceName, string attribute, decimal number);

        void SetAttribute(string serviceName, string attribute, DateTime date);

        void SetAttribute(string serviceName, string attribute, DateTime date, string text);

        string GetString(string serviceName, string attribute);

        decimal? GetNumber(string serviceName, string attribute);

        DateTime? GetDate(string serviceName, string attribute);

        List<KarmaService> GetKarmaServices();

        long Initialize(string service);
    }
}
