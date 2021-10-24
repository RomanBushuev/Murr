﻿using Murzik.Entities;
using System;
using System.Collections.Generic;

namespace Murzik.Interfaces
{
    public interface ITaskActions
    {
        bool IsAlive(long taskId);

        SaverJson GetSaverJson(long taskId);

        long RunJob(long taskId);

        long EndJob(long taskId);

        long ErrorJob(long taskId);

        IReadOnlyCollection<KarmaDownloadJob> GetKarmaDownloadJob();

        void SetAttribute(long taskId, string attribute, string text);

        void SetAttribute(long taskId, string attribute, decimal number);

        void SetAttribute(long taskId, string attribute, DateTime dateTime);

        void SetAttribute(long taskId, string attribute, DateTime dateTime, string text);

        string GetString(long taskId, string attribute);

        decimal? GetNumber(long taskId, string attribute);

        DateTime? GetDate(long taskId, string attribute);

        CalculationJson GetCalculationJson(long taskTemplateId);
    }
}