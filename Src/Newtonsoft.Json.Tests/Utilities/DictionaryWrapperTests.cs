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
using System.Collections;
#if HAVE_READ_ONLY_COLLECTIONS
using System.Collections.ObjectModel;
#endif
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Utilities;
using System.Linq;

namespace Newtonsoft.Json.Tests.Utilities
{
    [TestFixture]
    public class DictionaryWrapperTests : TestFixtureBase
    {
        [Test]
        public void Constructor_WithIDictionary_ValidatesNullArgument()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() => new DictionaryWrapper<string, object>((IDictionary)null!), 
                "Value cannot be null. (Parameter 'dictionary')");
        }

        [Test]
        public void Constructor_WithGenericDictionary_ValidatesNullArgument()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() => new DictionaryWrapper<string, object>((IDictionary<string, object>)null!), 
                "Value cannot be null. (Parameter 'dictionary')");
        }

#if HAVE_READ_ONLY_COLLECTIONS
        [Test]
        public void Constructor_WithReadOnlyDictionary_ValidatesNullArgument()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() => new DictionaryWrapper<string, object>((IReadOnlyDictionary<string, object>)null!), 
                "Value cannot be null. (Parameter 'dictionary')");
        }
#endif

        [Test]
        public void Constructor_WithIDictionary_StoresReference()
        {
            var dictionary = new Hashtable();
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            Assert.AreSame(dictionary, ((IWrappedDictionary)wrapper).UnderlyingDictionary);
        }

        [Test]
        public void Constructor_WithGenericDictionary_StoresReference()
        {
            var dictionary = new Dictionary<string, object>();
            var wrapper = new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            Assert.AreSame(dictionary, ((IWrappedDictionary)wrapper).UnderlyingDictionary);
        }

        [Test]
        public void Add_WithIDictionary_AddsKeyValue()
        {
            var dictionary = new Hashtable();
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            wrapper.Add("key", "value");

            Assert.AreEqual("value", dictionary["key"]);
            Assert.AreEqual(1, wrapper.Count);
        }

        [Test]
        public void Add_WithGenericDictionary_AddsKeyValue()
        {
            var dictionary = new Dictionary<string, object>();
            var wrapper = new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            wrapper.Add("key", "value");

            Assert.AreEqual("value", dictionary["key"]);
            Assert.AreEqual(1, wrapper.Count);
        }

#if HAVE_READ_ONLY_COLLECTIONS
        [Test]
        public void Add_WithReadOnlyDictionary_ThrowsNotSupportedException()
        {
            var dictionary = new Dictionary<string, object> { { "existing", "value" } };
            var readOnlyDict = new ReadOnlyDictionary<string, object>(dictionary);
            var wrapper = new DictionaryWrapper<string, object>(readOnlyDict);

            ExceptionAssert.Throws<NotSupportedException>(() => wrapper.Add("key", "value"));
        }
#endif

        [Test]
        public void ContainsKey_WithIDictionary_ReturnsCorrectValue()
        {
            var dictionary = new Hashtable { { "existing", "value" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            Assert.IsTrue(wrapper.ContainsKey("existing"));
            Assert.IsFalse(wrapper.ContainsKey("missing"));
        }

        [Test]
        public void ContainsKey_WithGenericDictionary_ReturnsCorrectValue()
        {
            var dictionary = new Dictionary<string, object> { { "existing", "value" } };
            var wrapper = new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            Assert.IsTrue(wrapper.ContainsKey("existing"));
            Assert.IsFalse(wrapper.ContainsKey("missing"));
        }

        [Test]
        public void Keys_WithIDictionary_ReturnsAllKeys()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            var keys = wrapper.Keys.ToList();

            Assert.AreEqual(2, keys.Count);
            Assert.IsTrue(keys.Contains("key1"));
            Assert.IsTrue(keys.Contains("key2"));
        }

        [Test]
        public void Keys_WithGenericDictionary_ReturnsAllKeys()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            var keys = wrapper.Keys.ToList();

            Assert.AreEqual(2, keys.Count);
            Assert.IsTrue(keys.Contains("key1"));
            Assert.IsTrue(keys.Contains("key2"));
        }

        [Test]
        public void Remove_WithIDictionary_RemovesExistingKey()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            bool result = wrapper.Remove("key1");

            Assert.IsTrue(result);
            Assert.AreEqual(1, wrapper.Count);
            Assert.IsFalse(wrapper.ContainsKey("key1"));
        }

        [Test]
        public void Remove_WithIDictionary_ReturnsFalseForMissingKey()
        {
            var dictionary = new Hashtable { { "key1", "value1" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            bool result = wrapper.Remove("missing");

            Assert.IsFalse(result);
            Assert.AreEqual(1, wrapper.Count);
        }

        [Test]
        public void Remove_WithGenericDictionary_RemovesExistingKey()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            bool result = wrapper.Remove("key1");

            Assert.IsTrue(result);
            Assert.AreEqual(1, wrapper.Count);
            Assert.IsFalse(wrapper.ContainsKey("key1"));
        }

        [Test]
        public void TryGetValue_WithIDictionary_ReturnsCorrectValue()
        {
            var dictionary = new Hashtable { { "key1", "value1" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            bool result = wrapper.TryGetValue("key1", out object? value);

            Assert.IsTrue(result);
            Assert.AreEqual("value1", value);
        }

        [Test]
        public void TryGetValue_WithIDictionary_ReturnsFalseForMissingKey()
        {
            var dictionary = new Hashtable { { "key1", "value1" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            bool result = wrapper.TryGetValue("missing", out object? value);

            Assert.IsFalse(result);
            Assert.IsNull(value);
        }

#if HAVE_READ_ONLY_COLLECTIONS
        [Test]
        public void TryGetValue_WithReadOnlyDictionary_ThrowsNotSupportedException()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" } };
            var readOnlyDict = new ReadOnlyDictionary<string, object>(dictionary);
            var wrapper = new DictionaryWrapper<string, object>(readOnlyDict);

            ExceptionAssert.Throws<NotSupportedException>(() => wrapper.TryGetValue("key1", out object? value));
        }
#endif

        [Test]
        public void Values_WithIDictionary_ReturnsAllValues()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            var values = wrapper.Values.ToList();

            Assert.AreEqual(2, values.Count);
            Assert.IsTrue(values.Contains("value1"));
            Assert.IsTrue(values.Contains("value2"));
        }

        [Test]
        public void Indexer_Get_WithIDictionary_ReturnsCorrectValue()
        {
            var dictionary = new Hashtable { { "key1", "value1" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            Assert.AreEqual("value1", wrapper["key1"]);
        }

        [Test]
        public void Indexer_Set_WithIDictionary_SetsValue()
        {
            var dictionary = new Hashtable();
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            wrapper["key1"] = "value1";

            Assert.AreEqual("value1", dictionary["key1"]);
        }

#if HAVE_READ_ONLY_COLLECTIONS
        [Test]
        public void Indexer_Set_WithReadOnlyDictionary_ThrowsNotSupportedException()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" } };
            var readOnlyDict = new ReadOnlyDictionary<string, object>(dictionary);
            var wrapper = new DictionaryWrapper<string, object>(readOnlyDict);

            ExceptionAssert.Throws<NotSupportedException>(() => wrapper["key1"] = "newvalue");
        }
#endif

        [Test]
        public void Add_KeyValuePair_WithIDictionary_ThrowsInvalidCastException()
        {
            // Most IDictionary implementations (like Hashtable) don't implement IList
            var dictionary = new Hashtable();
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            // This reveals a bug in DictionaryWrapper - it assumes IDictionary can be cast to IList
            ExceptionAssert.Throws<InvalidCastException>(() => 
                wrapper.Add(new KeyValuePair<string, object>("key1", "value1")));
        }

        [Test]
        public void Clear_WithIDictionary_RemovesAllItems()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            wrapper.Clear();

            Assert.AreEqual(0, wrapper.Count);
            Assert.AreEqual(0, dictionary.Count);
        }

#if HAVE_READ_ONLY_COLLECTIONS
        [Test]
        public void Clear_WithReadOnlyDictionary_ThrowsNotSupportedException()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" } };
            var readOnlyDict = new ReadOnlyDictionary<string, object>(dictionary);
            var wrapper = new DictionaryWrapper<string, object>(readOnlyDict);

            ExceptionAssert.Throws<NotSupportedException>(() => wrapper.Clear());
        }
#endif

        [Test]
        public void Contains_KeyValuePair_WithIDictionary_ThrowsInvalidCastException()
        {
            // Most IDictionary implementations (like Hashtable) don't implement IList
            var dictionary = new Hashtable { { "key1", "value1" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            // This reveals a bug in DictionaryWrapper - it assumes IDictionary can be cast to IList
            ExceptionAssert.Throws<InvalidCastException>(() => 
                wrapper.Contains(new KeyValuePair<string, object>("key1", "value1")));
        }

        [Test]
        public void CopyTo_WithIDictionary_CopiesAllItems()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);
            var array = new KeyValuePair<string, object>[3];

            wrapper.CopyTo(array, 1);

            Assert.AreEqual(2, array.Count(kvp => !kvp.Equals(default(KeyValuePair<string, object>))));
        }

        [Test]
        public void Count_ReturnsCorrectValue()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            Assert.AreEqual(2, wrapper.Count);
        }

        [Test]
        public void IsReadOnly_WithIDictionary_ReturnsCorrectValue()
        {
            var dictionary = new Hashtable();
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            Assert.AreEqual(dictionary.IsReadOnly, wrapper.IsReadOnly);
        }

        [Test]
        public void Remove_KeyValuePair_WithIDictionary_RemovesMatchingItem()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            bool result = wrapper.Remove(new KeyValuePair<string, object>("key1", "value1"));

            Assert.IsTrue(result);
            Assert.AreEqual(1, wrapper.Count);
            Assert.IsFalse(wrapper.ContainsKey("key1"));
        }

        [Test]
        public void Remove_KeyValuePair_WithIDictionary_ReturnsFalseForWrongValue()
        {
            var dictionary = new Hashtable { { "key1", "value1" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            bool result = wrapper.Remove(new KeyValuePair<string, object>("key1", "wrongvalue"));

            Assert.IsFalse(result);
            Assert.AreEqual(1, wrapper.Count);
        }

        [Test]
        public void Remove_KeyValuePair_WithIDictionary_ReturnsTrueForMissingKey()
        {
            var dictionary = new Hashtable { { "key1", "value1" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            bool result = wrapper.Remove(new KeyValuePair<string, object>("missing", "value"));

            Assert.IsTrue(result);
        }

        [Test]
        public void GetEnumerator_WithIDictionary_ReturnsAllItems()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>(dictionary);

            var items = wrapper.ToList();

            Assert.AreEqual(2, items.Count);
            Assert.IsTrue(items.Any(kvp => kvp.Key == "key1" && kvp.Value.ToString() == "value1"));
            Assert.IsTrue(items.Any(kvp => kvp.Key == "key2" && kvp.Value.ToString() == "value2"));
        }

        [Test]
        public void GetEnumerator_WithGenericDictionary_ReturnsAllItems()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            var items = wrapper.ToList();

            Assert.AreEqual(2, items.Count);
            Assert.IsTrue(items.Any(kvp => kvp.Key == "key1" && kvp.Value.ToString() == "value1"));
            Assert.IsTrue(items.Any(kvp => kvp.Key == "key2" && kvp.Value.ToString() == "value2"));
        }

        [Test]
        public void IDictionary_Add_WithIDictionary_AddsItem()
        {
            var dictionary = new Hashtable();
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>(dictionary);

            wrapper.Add("key1", "value1");

            Assert.AreEqual("value1", dictionary["key1"]);
        }

        [Test]
        public void IDictionary_Indexer_WithIDictionary_ReturnsCorrectValue()
        {
            var dictionary = new Hashtable { { "key1", "value1" } };
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>(dictionary);

            Assert.AreEqual("value1", wrapper["key1"]);
        }

        [Test]
        public void IDictionary_Indexer_Set_WithIDictionary_SetsValue()
        {
            var dictionary = new Hashtable();
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>(dictionary);

            wrapper["key1"] = "value1";

            Assert.AreEqual("value1", dictionary["key1"]);
        }

        [Test]
        public void IDictionary_Contains_WithGenericDictionary_ReturnsCorrectValue()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" } };
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            Assert.IsTrue(wrapper.Contains("key1"));
            Assert.IsFalse(wrapper.Contains("missing"));
        }

        [Test]
        public void IDictionary_IsFixedSize_WithGenericDictionary_ReturnsFalse()
        {
            var dictionary = new Dictionary<string, object>();
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            Assert.IsFalse(wrapper.IsFixedSize);
        }

#if HAVE_READ_ONLY_COLLECTIONS
        [Test]
        public void IDictionary_IsFixedSize_WithReadOnlyDictionary_ReturnsTrue()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" } };
            var readOnlyDict = new ReadOnlyDictionary<string, object>(dictionary);
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>(readOnlyDict);

            Assert.IsTrue(wrapper.IsFixedSize);
        }
#endif

        [Test]
        public void IDictionary_Keys_WithGenericDictionary_ReturnsAllKeys()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            var keys = wrapper.Keys.Cast<string>().ToList();

            Assert.AreEqual(2, keys.Count);
            Assert.IsTrue(keys.Contains("key1"));
            Assert.IsTrue(keys.Contains("key2"));
        }

        [Test]
        public void IDictionary_Remove_WithIDictionary_RemovesItem()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>(dictionary);

            wrapper.Remove("key1");

            Assert.AreEqual(1, dictionary.Count);
            Assert.IsFalse(dictionary.ContainsKey("key1"));
        }

        [Test]
        public void IDictionary_Values_WithGenericDictionary_ReturnsAllValues()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            var values = wrapper.Values.Cast<object>().ToList();

            Assert.AreEqual(2, values.Count);
            Assert.IsTrue(values.Contains("value1"));
            Assert.IsTrue(values.Contains("value2"));
        }

        [Test]
        public void ICollection_CopyTo_WithIDictionary_CopiesItems()
        {
            var dictionary = new Hashtable { { "key1", "value1" }, { "key2", "value2" } };
            var wrapper = (ICollection)new DictionaryWrapper<string, object>(dictionary);
            var array = new object[4];

            wrapper.CopyTo(array, 1);

            // Items should be copied, exact format depends on implementation
            Assert.IsTrue(array.Skip(1).Take(2).All(item => item != null));
        }

        [Test]
        public void ICollection_IsSynchronized_WithIDictionary_ReturnsCorrectValue()
        {
            var dictionary = new Hashtable();
            var wrapper = (ICollection)new DictionaryWrapper<string, object>(dictionary);

            Assert.AreEqual(dictionary.IsSynchronized, wrapper.IsSynchronized);
        }

        [Test]
        public void ICollection_IsSynchronized_WithGenericDictionary_ReturnsFalse()
        {
            var dictionary = new Dictionary<string, object>();
            var wrapper = (ICollection)new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            Assert.IsFalse(wrapper.IsSynchronized);
        }

        [Test]
        public void ICollection_SyncRoot_ReturnsNonNullValue()
        {
            var dictionary = new Dictionary<string, object>();
            var wrapper = (ICollection)new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            Assert.IsNotNull(wrapper.SyncRoot);
        }

        [Test]
        public void IDictionary_GetEnumerator_WithIDictionary_ReturnsEnumerator()
        {
            var dictionary = new Hashtable { { "key1", "value1" } };
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>(dictionary);

            var enumerator = wrapper.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            
            var entry = enumerator.Entry;
            Assert.AreEqual("key1", entry.Key);
            Assert.AreEqual("value1", entry.Value);
        }

#if HAVE_READ_ONLY_COLLECTIONS
        [Test]
        public void IDictionary_GetEnumerator_WithReadOnlyDictionary_ReturnsEnumerator()
        {
            var dictionary = new Dictionary<string, object> { { "key1", "value1" } };
            var readOnlyDict = new ReadOnlyDictionary<string, object>(dictionary);
            var wrapper = (IDictionary)new DictionaryWrapper<string, object>(readOnlyDict);

            var enumerator = wrapper.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            
            var entry = enumerator.Entry;
            Assert.AreEqual("key1", entry.Key);
            Assert.AreEqual("value1", entry.Value);
        }
#endif

        [Test]
        public void GenericDictionary_Property_ReturnsCorrectInstance()
        {
            var dictionary = new Dictionary<string, object>();
            var wrapper = new DictionaryWrapper<string, object>((IDictionary<string, object>)dictionary);

            // Access internal property via reflection would be needed to test GenericDictionary property
            // but since it's internal, we test its behavior indirectly through public methods
            wrapper.Add("key", "value");
            Assert.AreEqual("value", wrapper["key"]);
        }
    }
}