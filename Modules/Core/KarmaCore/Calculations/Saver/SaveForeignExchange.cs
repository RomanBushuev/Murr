using KarmaCore.BaseTypes;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;

namespace KarmaCore.Calculations.Saver
{
    public class SaveForeignExchange : Calculation
    {
        public const string RunDateTime = "RunDateTime";
        public const string Document = "Document";

        public override TaskTypes TaskTypes => TaskTypes.SaveForeignExchange;
        private ICbrXmlRepository _cbrXmlRepository;
        private IMarkerRepository _markerRepository;

        public SaveForeignExchange(ICbrXmlRepository cbrXmlRepository,
            IMarkerRepository markerRepository)
        {
            _cbrXmlRepository = cbrXmlRepository;
            _markerRepository = markerRepository;
        }

        public override void Run()
        {
            Notify($"Задача загрузки иностранных валют начата");
            //прочитать документ
            string file = _paramDescriptors.ConvertStr(Document);
            Notify($"Загружаем файл: {file}");
            var currencies = _cbrXmlRepository.GetCurrencies(file);
            Notify($"Файл загружен");
            Notify($"Сохранить значения {currencies.ValidDate} : {currencies.ValuteCursOnDates.Count}");
            //сохранить значения
            _markerRepository.Save(currencies);
            Notify($"Задача сохранения иностранных валют закончена");
        }

        public override IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            if (_paramDescriptors != null)
                return _paramDescriptors;
            else
            {
                _paramDescriptors = new List<ParamDescriptor>();
                _paramDescriptors.Add(new ParamDescriptor()
                {
                    Ident = RunDateTime,
                    Description = "Время для запуска",
                    ParamType = ParamType.DateTime,
                    Value = DateTime.Today.Date
                });
                _paramDescriptors.Add(new ParamDescriptor()
                {
                    Ident = Document,
                    Description = "Документ",
                    ParamType = ParamType.String,
                    Value = string.Empty
                });

                return _paramDescriptors;
            }
        }

        public override CalculationJson Serialize()
        {
            CalculationJson calculationJson = new CalculationJson();
            calculationJson.TaskType = (long)TaskTypes;
            calculationJson.JsonParameters = _paramDescriptors.SerializeJson();
            return calculationJson;
        }
    }
}
