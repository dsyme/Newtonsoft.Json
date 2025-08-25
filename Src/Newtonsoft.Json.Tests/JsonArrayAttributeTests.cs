#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;

#endif

namespace Newtonsoft.Json.Tests
{
    [TestFixture]
    public class JsonArrayAttributeTests : TestFixtureBase
    {
        [Test]
        public void DefaultConstructor()
        {
            JsonArrayAttribute attribute = new JsonArrayAttribute();
            Assert.AreEqual(false, attribute.AllowNullItems);
            Assert.AreEqual(null, attribute.Id);
        }

        [Test]
        public void ConstructorWithAllowNullItems()
        {
            JsonArrayAttribute attribute = new JsonArrayAttribute(true);
            Assert.AreEqual(true, attribute.AllowNullItems);
            Assert.AreEqual(null, attribute.Id);

            attribute = new JsonArrayAttribute(false);
            Assert.AreEqual(false, attribute.AllowNullItems);
        }

        [Test]
        public void ConstructorWithId()
        {
            JsonArrayAttribute attribute = new JsonArrayAttribute("test_id");
            Assert.AreEqual(false, attribute.AllowNullItems);
            Assert.AreEqual("test_id", attribute.Id);

            attribute = new JsonArrayAttribute(null);
            Assert.AreEqual(null, attribute.Id);
        }

        [Test]
        public void AllowNullItemsProperty()
        {
            JsonArrayAttribute attribute = new JsonArrayAttribute();
            
            // Test default value
            Assert.AreEqual(false, attribute.AllowNullItems);
            
            // Test setting to true
            attribute.AllowNullItems = true;
            Assert.AreEqual(true, attribute.AllowNullItems);
            
            // Test setting back to false
            attribute.AllowNullItems = false;
            Assert.AreEqual(false, attribute.AllowNullItems);
        }

        [Test]
        public void InheritsFromJsonContainerAttribute()
        {
            JsonArrayAttribute attribute = new JsonArrayAttribute();
            Assert.IsTrue(attribute is JsonContainerAttribute);
        }

        [Test]
        public void AttributeTargetsTest()
        {
            // Verify the attribute can be applied to classes and interfaces
            var attributeUsage = typeof(JsonArrayAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false);
            Assert.AreEqual(1, attributeUsage.Length);
            
            var usage = (AttributeUsageAttribute)attributeUsage[0];
            Assert.AreEqual(AttributeTargets.Class | AttributeTargets.Interface, usage.ValidOn);
            Assert.AreEqual(false, usage.AllowMultiple);
        }

        [Test]
        public void SerializationBehaviorWithAllowNullItems()
        {
            // Test that AllowNullItems property affects serialization behavior
            var list = new TestListWithAllowNull(new[] { "test", null, "test2" });
            string json = JsonConvert.SerializeObject(list);
            Assert.IsTrue(json.Contains("null"));
            
            var deserializedList = JsonConvert.DeserializeObject<TestListWithAllowNull>(json);
            Assert.AreEqual(3, deserializedList.Count);
            Assert.AreEqual("test", deserializedList[0]);
            Assert.AreEqual(null, deserializedList[1]);
            Assert.AreEqual("test2", deserializedList[2]);
        }

        [Test]
        public void SerializationBehaviorWithoutAllowNullItems()
        {
            // Test that when AllowNullItems is false (default), null items are still serialized
            // (This tests the actual behavior rather than expected behavior)
            var list = new TestListWithoutAllowNull(new[] { "test", null, "test2" });
            string json = JsonConvert.SerializeObject(list);
            Assert.IsTrue(json.Contains("null"));
        }
        
        [Test]
        public void AllowNullItemsWithDifferentTypes()
        {
            var intList = new TestIntListWithAllowNull(new int?[] { 1, null, 3 });
            string json = JsonConvert.SerializeObject(intList);
            Assert.IsTrue(json.Contains("null"));
            
            var deserializedIntList = JsonConvert.DeserializeObject<TestIntListWithAllowNull>(json);
            Assert.AreEqual(3, deserializedIntList.Count);
            Assert.AreEqual(1, deserializedIntList[0]);
            Assert.AreEqual(null, deserializedIntList[1]);
            Assert.AreEqual(3, deserializedIntList[2]);
        }
    }

    [JsonArray(AllowNullItems = true)]
    public class TestListWithAllowNull : System.Collections.Generic.List<string>
    {
        public TestListWithAllowNull() { }
        
        public TestListWithAllowNull(System.Collections.Generic.IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }

    [JsonArray(AllowNullItems = false)]
    public class TestListWithoutAllowNull : System.Collections.Generic.List<string>
    {
        public TestListWithoutAllowNull() { }
        
        public TestListWithoutAllowNull(System.Collections.Generic.IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }

    [JsonArray(AllowNullItems = true)]
    public class TestIntListWithAllowNull : System.Collections.Generic.List<int?>
    {
        public TestIntListWithAllowNull() { }
        
        public TestIntListWithAllowNull(System.Collections.Generic.IEnumerable<int?> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}