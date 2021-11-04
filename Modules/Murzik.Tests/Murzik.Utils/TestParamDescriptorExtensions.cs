using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Utils;
using System;
using System.Collections.Generic;
using Xunit;

namespace Murzik.Tests.Murzik.Utils
{
    public class TestParamDescriptorExtensions
    {
        public readonly DateTime constDate = new DateTime(2021, 10, 04, 10, 0, 0);

        public List<ParamDescriptor> _params = new List<ParamDescriptor>()
        {
            new ParamDescriptor
            {
                Ident = "str",
                Description = "str",
                ParamType = ParamType.String,
                Value = "string"
            },
            new ParamDescriptor
            {
                Ident = "num",
                Description = "num",
                ParamType = ParamType.Decimal,
                Value = 10.0m
            },
            new ParamDescriptor
            {
                Ident = "dat",
                Description = "dat",
                ParamType = ParamType.DateTime,
                Value = new DateTime(2021, 10, 04, 10, 0, 0)
            }
        };


        [Fact]
        public void TestConvertNum()
        {
            Assert.Equal(10.0m, _params.ConvertNum("num"));
        }

        [Fact]
        public void TestConvertStr()
        {
            Assert.Equal("string", _params.ConvertStr("str"));
        }

        [Fact]
        public void TestConvertDat()
        {
            Assert.Equal(constDate, _params.ConvertDate("dat"));
        }

        [Fact]
        public void TestSerialize()
        {
            var json = _params.SerializeJson();
            var newParams = ParamDescriptorExtensions.DeserializeJson(json, _params);
            Assert.Equal(10.0m, newParams.ConvertNum("num"));
            Assert.Equal("string", newParams.ConvertStr("str"));
            Assert.Equal(constDate, newParams.ConvertDate("dat"));
        }

        [Fact]
        public void TesSetNum()
        {
            var json = _params.SerializeJson();
            var newParams = ParamDescriptorExtensions.DeserializeJson(json, _params);
            Assert.Equal(10.0m, newParams.ConvertNum("num"));
            newParams.SetNum("num", 11.0m);
            Assert.Equal(11.0m, newParams.ConvertNum("num"));
            Assert.Equal(10.0m, _params.ConvertNum("num"));
        }

        [Fact]
        public void TestSetDate()
        {
            var json = _params.SerializeJson();
            var newParams = ParamDescriptorExtensions.DeserializeJson(json, _params);
            Assert.Equal(constDate, newParams.ConvertDate("dat"));
            newParams.SetDat("dat", constDate.AddDays(1));
            Assert.Equal(constDate.AddDays(1), newParams.ConvertDate("dat"));
            Assert.Equal(constDate, _params.ConvertDate("dat"));
        }


        [Fact]
        public void TestSetStr()
        {
            var json = _params.SerializeJson();
            var newParams = ParamDescriptorExtensions.DeserializeJson(json, _params);
            Assert.Equal("string", newParams.ConvertStr("str"));
            newParams.SetStr("str", "roman");
            Assert.Equal("roman", newParams.ConvertStr("str"));
            Assert.Equal("string", _params.ConvertStr("str"));
        }
    }
}
