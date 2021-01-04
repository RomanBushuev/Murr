using System;
using System.Collections.Generic;
using KarmaCore.Entities;

namespace KarmaCore.Interfaces
{
    public interface ITaskActions
    {
        long RunJob(long taskId);

        long EndJob(long taskId);

        long ErrorJob(long taskId);

        List<KarmaDownloadJob> GetKarmaDownloadJob();

        void SetAttribute(long taskId, string attribute, string text);

        void SetAttribute(long taskId, string attribute, decimal number);

        void SetAttribute(long taskId, string attribute, DateTime dateTime);

        void SetAttribute(long taskId, string attribute, DateTime dateTime, string text);

        string GetString(long taskId, string attribute);

        decimal? GetNumber(long taskId, string attribute);

        DateTime? GetDate(long taskId, string attribute);
    }
}
