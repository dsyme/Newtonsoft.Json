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
using System.Collections.Generic;
using Newtonsoft.Json.Converters;

#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

#if HAVE_CUSTOM_ATTRIBUTES
using System.Reflection;
#endif

namespace Newtonsoft.Json.Tests
{
    [TestFixture]
    public class JsonDictionaryAttributeTests : TestFixtureBase
    {
        [Test]
        public void DefaultConstructor()
        {
            JsonDictionaryAttribute attribute = new JsonDictionaryAttribute();
            Assert.AreEqual(null, attribute.Id);
            Assert.AreEqual(null, attribute.Title);
            Assert.AreEqual(null, attribute.Description);
            Assert.AreEqual(null, attribute.ItemConverterType);
            Assert.AreEqual(null, attribute.ItemConverterParameters);
        }

        [Test]
        public void ConstructorWithId()
        {
            JsonDictionaryAttribute attribute = new JsonDictionaryAttribute("TestId");
            Assert.AreEqual("TestId", attribute.Id);
            Assert.AreEqual(null, attribute.Title);
            Assert.AreEqual(null, attribute.Description);
            Assert.AreEqual(null, attribute.ItemConverterType);
            Assert.AreEqual(null, attribute.ItemConverterParameters);
        }

        [Test]
        public void InheritsFromJsonContainerAttribute()
        {
            JsonDictionaryAttribute attribute = new JsonDictionaryAttribute();
            Assert.IsTrue(attribute is JsonContainerAttribute);
        }

        [Test]
        public void AttributeTargetsTest()
        {
#if HAVE_CUSTOM_ATTRIBUTES
            Type attributeType = typeof(JsonDictionaryAttribute);
            AttributeUsageAttribute usage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();
            
            Assert.IsNotNull(usage);
            Assert.AreEqual(AttributeTargets.Class | AttributeTargets.Interface, usage.ValidOn);
            Assert.AreEqual(false, usage.AllowMultiple);
#endif
        }

        [Test]
        public void SerializationBehaviorWithDictionary()
        {
            var testDict = new TestDictionary()
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            string json = JsonConvert.SerializeObject(testDict, Formatting.Indented);
            Assert.IsTrue(json.Contains("\"key1\": \"value1\""));
            Assert.IsTrue(json.Contains("\"key2\": \"value2\""));

            var deserialized = JsonConvert.DeserializeObject<TestDictionary>(json);
            Assert.AreEqual(2, deserialized.Count);
            Assert.AreEqual("value1", deserialized["key1"]);
            Assert.AreEqual("value2", deserialized["key2"]);
        }

        [Test]
        public void SerializationBehaviorWithIdAttribute()
        {
            var testDict = new TestDictionaryWithId()
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            string json = JsonConvert.SerializeObject(testDict, Formatting.Indented);
            Assert.IsTrue(json.Contains("\"key1\": \"value1\""));
            Assert.IsTrue(json.Contains("\"key2\": \"value2\""));

            var deserialized = JsonConvert.DeserializeObject<TestDictionaryWithId>(json);
            Assert.AreEqual(2, deserialized.Count);
            Assert.AreEqual("value1", deserialized["key1"]);
            Assert.AreEqual("value2", deserialized["key2"]);
        }

        [Test]
        public void PropertiesCanBeSetAndRead()
        {
            JsonDictionaryAttribute attribute = new JsonDictionaryAttribute();
            
            // Test inherited properties from JsonContainerAttribute
            attribute.Title = "Test Title";
            Assert.AreEqual("Test Title", attribute.Title);

            attribute.Description = "Test Description";
            Assert.AreEqual("Test Description", attribute.Description);

            attribute.ItemConverterType = typeof(StringEnumConverter);
            Assert.AreEqual(typeof(StringEnumConverter), attribute.ItemConverterType);
            
            attribute.ItemConverterParameters = new object[] { "param1", 123 };
            Assert.AreEqual(2, attribute.ItemConverterParameters.Length);
            Assert.AreEqual("param1", attribute.ItemConverterParameters[0]);
            Assert.AreEqual(123, attribute.ItemConverterParameters[1]);
        }

        [Test]
        public void ConstructorWithIdPreservesIdProperty()
        {
            JsonDictionaryAttribute attribute = new JsonDictionaryAttribute("MyId");
            
            // Ensure the Id property works correctly
            Assert.AreEqual("MyId", attribute.Id);
            
            // Ensure other properties are still settable
            attribute.Title = "Test Title";
            attribute.Description = "Test Description";
            
            Assert.AreEqual("MyId", attribute.Id);
            Assert.AreEqual("Test Title", attribute.Title);
            Assert.AreEqual("Test Description", attribute.Description);
        }

        [Test]
        public void AttributeAppliedToInterface()
        {
#if HAVE_CUSTOM_ATTRIBUTES
            // Test that the attribute can be applied to an interface
            Type interfaceType = typeof(ITestDictionary);
            var attribute = interfaceType.GetCustomAttribute<JsonDictionaryAttribute>();
            Assert.IsNotNull(attribute);
            Assert.AreEqual("InterfaceId", attribute.Id);
#endif
        }

        [Test]
        public void MultipleAttributesNotAllowed()
        {
#if HAVE_CUSTOM_ATTRIBUTES
            // Verify that AttributeUsage specifies AllowMultiple = false
            Type attributeType = typeof(JsonDictionaryAttribute);
            AttributeUsageAttribute usage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();
            
            Assert.IsFalse(usage.AllowMultiple);
#endif
        }

        // Test helper classes
        [JsonDictionary]
        public class TestDictionary : Dictionary<string, string>
        {
        }

        [JsonDictionary("SpecialId")]
        public class TestDictionaryWithId : Dictionary<string, string>
        {
        }

        [JsonDictionary("InterfaceId")]
        public interface ITestDictionary
        {
        }
    }
}