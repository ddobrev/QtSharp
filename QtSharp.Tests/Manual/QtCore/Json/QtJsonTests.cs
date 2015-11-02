using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.Manual.QtCore.Json
{
    [TestFixture]
    public class QtJsonTests
    {
        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public void Dispose()
        {
        }

        [Test]
        public void TestSimpleValue()
        {
            using (var obj = new QJsonObject())
            {
                obj.Insert("number", new QJsonValue(999.0));

                var array = new QJsonArray();
                for (var i = 0; i < 10; ++i)
                    array.Append(new QJsonValue(i));

                using (var value = new QJsonValue(true))
                {
                    Assert.AreEqual(value.type, QJsonValue.Type.Bool);
                    Assert.AreEqual(value.ToDouble(), 0.0);
                    Assert.AreEqual(value.ToString(), "");
                    Assert.AreEqual(value.ToBool(), true);
                    Assert.AreEqual(value.ToObject(), new QJsonObject());
                    Assert.AreEqual(value.ToArray(), new QJsonArray());
                    Assert.AreEqual(value.ToDouble(99.0), 99.0);
                    Assert.AreEqual(value.ToString("test"), "test");
                    Assert.AreEqual(value.ToObject(obj), obj);
                    Assert.AreEqual(value.ToArray(array), array);
                }
            }

            using (var value = new QJsonValue(999.0))
            {
                Assert.AreEqual(value.type, QJsonValue.Type.Double);
                Assert.AreEqual(value.ToDouble(), 999.0);
                Assert.AreEqual(value.ToString(), "");
                Assert.AreEqual(value.ToBool(), false);
                Assert.AreEqual(value.ToBool(true), true);
                Assert.AreEqual(value.ToObject(), new QJsonObject());
                Assert.AreEqual(value.ToArray(), new QJsonArray());
            }

            using (var value = new QJsonValue(new QLatin1String("test")))
            {
                Assert.AreEqual(value.ToDouble(), 0.0);
                Assert.AreEqual(value.ToString(), new QLatin1String("test"));
                Assert.AreEqual(value.ToBool(), false);
                Assert.AreEqual(value.ToObject(), new QJsonObject());
                Assert.AreEqual(value.ToArray(), new QJsonArray());
            }
        }
    }
}