using KarmaCore.BaseTypes;
using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Interfaces
{
    /// <summary>
    /// Рыночный провайдер данных
    /// </summary>
    public interface IMarketProvider
    {
        /// <summary>
        /// Получение даты
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="scalarAttribute"></param>
        /// <returns></returns>
        ScalarDate GetScalarDate(string ident, ScalarAttribute scalarAttribute);

        /// <summary>
        /// Получение строки
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="scalarAttribute"></param>
        /// <returns></returns>
        ScalarStr GetScalarStr(string ident, ScalarAttribute scalarAttribute);

        /// <summary>
        /// Получение числа
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="scalarAttribute"></param>
        /// <returns></returns>
        ScalarNum GetScalarNum(string ident, ScalarAttribute scalarAttribute);

        /// <summary>
        /// Получение перечисления
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="scalarAttribute"></param>
        /// <returns></returns>
        ScalarEnum GetScalarEnum(string ident, ScalarAttribute scalarAttribute);
    }
}
