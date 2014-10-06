using System.Linq.Expressions;
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
            // TODO: Add Init code.
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
        }

        [Ignore("Bug!")]
        [Test]
        public void TestSimpleValue()
        {
            var obj = new QJsonObject();
            obj.Insert("number", new QJsonValue(999.0));

            var array = new QJsonArray();
            for (var i = 0; i < 10; ++i)
                array.Append(new QJsonValue(i));

            var value = new QJsonValue(true);
            Assert.AreEqual(value.type, typeof(bool));
            Assert.AreEqual(value.ToDouble(), 0.0);
            Assert.AreEqual(value.ToString(), "");
            Assert.AreEqual(value.ToBool(), true);
            Assert.AreEqual(value.ToObject(), new QJsonObject());
            Assert.AreEqual(value.ToArray(), new QJsonArray());
            Assert.AreEqual(value.ToDouble(99.0), 99.0);
            Assert.AreEqual(value.ToString("test"), "test");
            Assert.AreEqual(value.ToObject(obj), obj);
            Assert.AreEqual(value.ToArray(array), array);

            value = new QJsonValue(999.0);
            Assert.AreEqual(value.type, QJsonValue.Type.Double);
            Assert.AreEqual(value.ToDouble(), 999.0);
            Assert.AreEqual(value.ToString(), "");
            Assert.AreEqual(value.ToBool(), false);
            Assert.AreEqual(value.ToBool(true), true);
            Assert.AreEqual(value.ToObject(), new QJsonObject());
            Assert.AreEqual(value.ToArray(), new QJsonArray());

            value = new QJsonValue(new QLatin1String("test"));
            Assert.AreEqual(value.ToDouble(), 0.0);
            Assert.AreEqual(value.ToString(), new QLatin1String("test"));
            Assert.AreEqual(value.ToBool(), false);
            Assert.AreEqual(value.ToObject(), new QJsonObject());
            Assert.AreEqual(value.ToArray(), new QJsonArray());
        }
    }
}