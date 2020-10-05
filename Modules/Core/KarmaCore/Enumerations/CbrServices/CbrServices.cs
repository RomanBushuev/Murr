using KarmaCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Enumerations.CbrServices
{
    public enum CbrServices
    {
        [MurrCbrAttribute("Неопределенный")]
        Undefined = 0,
        [MurrCbrAttribute("Загрузка валют")]
        ForeignExchange = 1
    }
}
