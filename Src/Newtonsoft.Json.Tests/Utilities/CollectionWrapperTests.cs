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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Utilities;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Tests.Utilities
{
    [TestFixture]
    public class CollectionWrapperTests : TestFixtureBase
    {
        private class TestList : List<string>
        {
            public TestList()
            {
            }

            public TestList(IEnumerable<string> collection) : base(collection)
            {
            }
        }

        private class TestArrayList : ArrayList
        {
            public TestArrayList()
            {
            }

            public TestArrayList(ICollection c) : base(c)
            {
            }
        }

        [Test]
        public void ConstructorWithIList_ValidList_SetsUnderlyingList()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            Assert.AreEqual(arrayList, wrapper.UnderlyingCollection);
        }

        [Test]
        public void ConstructorWithIList_NullList_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() => new CollectionWrapper<string>((IList)null!), "Value cannot be null. (Parameter 'list')");
        }

        [Test]
        public void ConstructorWithICollection_ValidCollection_SetsUnderlyingCollection()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            Assert.AreEqual(list, wrapper.UnderlyingCollection);
        }

        [Test]
        public void ConstructorWithICollection_NullCollection_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() => new CollectionWrapper<string>((ICollection<string>)null!), "Value cannot be null. (Parameter 'list')");
        }

        [Test]
        public void ConstructorWithIList_GenericListAsIList_WrapsAsGenericCollection()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((IList)list);

            wrapper.Add("d");
            Assert.AreEqual(4, wrapper.Count);
            Assert.AreEqual("d", list[3]);
        }

        [Test]
        public void Add_WithGenericCollection_AddsToUnderlyingCollection()
        {
            var list = new List<string> { "a", "b" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            wrapper.Add("c");

            Assert.AreEqual(3, wrapper.Count);
            Assert.AreEqual("c", list[2]);
        }

        [Test]
        public void Add_WithNonGenericList_AddsToUnderlyingList()
        {
            var arrayList = new ArrayList { "a", "b" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            wrapper.Add("c");

            Assert.AreEqual(3, wrapper.Count);
            Assert.AreEqual("c", arrayList[2]);
        }

        [Test]
        public void Clear_WithGenericCollection_ClearsUnderlyingCollection()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            wrapper.Clear();

            Assert.AreEqual(0, wrapper.Count);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void Clear_WithNonGenericList_ClearsUnderlyingList()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            wrapper.Clear();

            Assert.AreEqual(0, wrapper.Count);
            Assert.AreEqual(0, arrayList.Count);
        }

        [Test]
        public void Contains_WithGenericCollection_ChecksUnderlyingCollection()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            Assert.IsTrue(wrapper.Contains("b"));
            Assert.IsFalse(wrapper.Contains("d"));
        }

        [Test]
        public void Contains_WithNonGenericList_ChecksUnderlyingList()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            Assert.IsTrue(wrapper.Contains("b"));
            Assert.IsFalse(wrapper.Contains("d"));
        }

        [Test]
        public void CopyTo_WithGenericCollection_CopiesToArray()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var array = new string[5];

            wrapper.CopyTo(array, 1);

            Assert.AreEqual(null, array[0]);
            Assert.AreEqual("a", array[1]);
            Assert.AreEqual("b", array[2]);
            Assert.AreEqual("c", array[3]);
            Assert.AreEqual(null, array[4]);
        }

        [Test]
        public void CopyTo_WithNonGenericList_CopiesToArray()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var array = new string[5];

            wrapper.CopyTo(array, 1);

            Assert.AreEqual(null, array[0]);
            Assert.AreEqual("a", array[1]);
            Assert.AreEqual("b", array[2]);
            Assert.AreEqual("c", array[3]);
            Assert.AreEqual(null, array[4]);
        }

        [Test]
        public void Count_WithGenericCollection_ReturnsCorrectCount()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            Assert.AreEqual(3, wrapper.Count);

            list.Add("d");
            Assert.AreEqual(4, wrapper.Count);
        }

        [Test]
        public void Count_WithNonGenericList_ReturnsCorrectCount()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            Assert.AreEqual(3, wrapper.Count);

            arrayList.Add("d");
            Assert.AreEqual(4, wrapper.Count);
        }

        [Test]
        public void IsReadOnly_WithGenericCollection_ReturnsUnderlyingIsReadOnly()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            Assert.IsFalse(wrapper.IsReadOnly);
        }

        [Test]
        public void IsReadOnly_WithNonGenericList_ReturnsUnderlyingIsReadOnly()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            Assert.AreEqual(arrayList.IsReadOnly, wrapper.IsReadOnly);
        }

        [Test]
        public void Remove_WithGenericCollection_RemovesFromUnderlyingCollection()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            bool removed = wrapper.Remove("b");

            Assert.IsTrue(removed);
            Assert.AreEqual(2, wrapper.Count);
            Assert.IsFalse(list.Contains("b"));
        }

        [Test]
        public void Remove_WithGenericCollection_ItemNotExists_ReturnsFalse()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            bool removed = wrapper.Remove("d");

            Assert.IsFalse(removed);
            Assert.AreEqual(3, wrapper.Count);
        }

        [Test]
        public void Remove_WithNonGenericList_RemovesFromUnderlyingList()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            bool removed = wrapper.Remove("b");

            Assert.IsTrue(removed);
            Assert.AreEqual(2, wrapper.Count);
            Assert.IsFalse(arrayList.Contains("b"));
        }

        [Test]
        public void Remove_WithNonGenericList_ItemNotExists_ReturnsFalse()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            bool removed = wrapper.Remove("d");

            Assert.IsFalse(removed);
            Assert.AreEqual(3, wrapper.Count);
        }

        [Test]
        public void GetEnumerator_WithGenericCollection_ReturnsCorrectEnumerator()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            var result = wrapper.ToList();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
        }

        [Test]
        public void GetEnumerator_WithNonGenericList_ReturnsCorrectEnumerator()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            var result = wrapper.ToList();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
        }

        [Test]
        public void NonGenericGetEnumerator_WithGenericCollection_ReturnsCorrectEnumerator()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var enumerable = (IEnumerable)wrapper;

            var result = new List<object>();
            foreach (object item in enumerable)
            {
                result.Add(item);
            }

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
        }

        [Test]
        public void NonGenericGetEnumerator_WithNonGenericList_ReturnsCorrectEnumerator()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var enumerable = (IEnumerable)wrapper;

            var result = new List<object>();
            foreach (object item in enumerable)
            {
                result.Add(item);
            }

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
        }

        [Test]
        public void IList_Add_ValidValue_AddsToCollection()
        {
            var list = new List<string> { "a", "b" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            int index = ilist.Add("c");

            Assert.AreEqual(2, index);
            Assert.AreEqual(3, wrapper.Count);
            Assert.AreEqual("c", list[2]);
        }

        [Test]
        public void IList_Add_IncompatibleType_ThrowsArgumentException()
        {
            var list = new List<string> { "a", "b" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            ExceptionAssert.Throws<ArgumentException>(() => ilist.Add(42), "The value '42' is not of type 'System.String' and cannot be used in this generic collection. (Parameter 'value')");
        }

        [Test]
        public void IList_Contains_ValidValue_ReturnsTrue()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            Assert.IsTrue(ilist.Contains("b"));
            Assert.IsFalse(ilist.Contains("d"));
        }

        [Test]
        public void IList_Contains_IncompatibleType_ReturnsFalse()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            Assert.IsFalse(ilist.Contains(42));
        }

        [Test]
        public void IList_IndexOf_WithGenericCollection_ThrowsInvalidOperationException()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            ExceptionAssert.Throws<InvalidOperationException>(() => ilist.IndexOf("b"), "Wrapped ICollection<T> does not support IndexOf.");
        }

        [Test]
        public void IList_IndexOf_WithNonGenericList_ValidValue_ReturnsCorrectIndex()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var ilist = (IList)wrapper;

            Assert.AreEqual(1, ilist.IndexOf("b"));
            Assert.AreEqual(-1, ilist.IndexOf("d"));
        }

        [Test]
        public void IList_IndexOf_WithNonGenericList_IncompatibleType_ReturnsMinusOne()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var ilist = (IList)wrapper;

            Assert.AreEqual(-1, ilist.IndexOf(42));
        }

        [Test]
        public void IList_RemoveAt_WithGenericCollection_ThrowsInvalidOperationException()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            ExceptionAssert.Throws<InvalidOperationException>(() => ilist.RemoveAt(1), "Wrapped ICollection<T> does not support RemoveAt.");
        }

        [Test]
        public void IList_RemoveAt_WithNonGenericList_RemovesAtIndex()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var ilist = (IList)wrapper;

            ilist.RemoveAt(1);

            Assert.AreEqual(2, wrapper.Count);
            Assert.AreEqual("a", arrayList[0]);
            Assert.AreEqual("c", arrayList[1]);
        }

        [Test]
        public void IList_Insert_WithGenericCollection_ThrowsInvalidOperationException()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            ExceptionAssert.Throws<InvalidOperationException>(() => ilist.Insert(1, "x"), "Wrapped ICollection<T> does not support Insert.");
        }

        [Test]
        public void IList_Insert_WithNonGenericList_ValidValue_InsertsAtIndex()
        {
            var arrayList = new ArrayList { "a", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var ilist = (IList)wrapper;

            ilist.Insert(1, "b");

            Assert.AreEqual(3, wrapper.Count);
            Assert.AreEqual("a", arrayList[0]);
            Assert.AreEqual("b", arrayList[1]);
            Assert.AreEqual("c", arrayList[2]);
        }

        [Test]
        public void IList_Insert_WithNonGenericList_IncompatibleType_ThrowsArgumentException()
        {
            var arrayList = new ArrayList { "a", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var ilist = (IList)wrapper;

            ExceptionAssert.Throws<ArgumentException>(() => ilist.Insert(1, 42), "The value '42' is not of type 'System.String' and cannot be used in this generic collection. (Parameter 'value')");
        }

        [Test]
        public void IList_IsFixedSize_WithGenericCollection_ReturnsIsReadOnly()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            Assert.IsFalse(ilist.IsFixedSize);
        }

        [Test]
        public void IList_IsFixedSize_WithNonGenericList_ReturnsUnderlyingIsFixedSize()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var ilist = (IList)wrapper;

            Assert.AreEqual(arrayList.IsFixedSize, ilist.IsFixedSize);
        }

        [Test]
        public void IList_Remove_ValidValue_RemovesFromCollection()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            ilist.Remove("b");

            Assert.AreEqual(2, wrapper.Count);
            Assert.IsFalse(list.Contains("b"));
        }

        [Test]
        public void IList_Remove_IncompatibleType_DoesNotRemove()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            ilist.Remove(42);

            Assert.AreEqual(3, wrapper.Count);
        }

        [Test]
        public void IList_Indexer_Get_WithGenericCollection_ThrowsInvalidOperationException()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            ExceptionAssert.Throws<InvalidOperationException>(() => { var value = ilist[1]; }, "Wrapped ICollection<T> does not support indexer.");
        }

        [Test]
        public void IList_Indexer_Set_WithGenericCollection_ThrowsInvalidOperationException()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var ilist = (IList)wrapper;

            ExceptionAssert.Throws<InvalidOperationException>(() => ilist[1] = "x", "Wrapped ICollection<T> does not support indexer.");
        }

        [Test]
        public void IList_Indexer_Get_WithNonGenericList_ReturnsCorrectValue()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var ilist = (IList)wrapper;

            Assert.AreEqual("b", ilist[1]);
        }

        [Test]
        public void IList_Indexer_Set_WithNonGenericList_ValidValue_SetsValue()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var ilist = (IList)wrapper;

            ilist[1] = "x";

            Assert.AreEqual("x", arrayList[1]);
        }

        [Test]
        public void IList_Indexer_Set_WithNonGenericList_IncompatibleType_ThrowsArgumentException()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);
            var ilist = (IList)wrapper;

            ExceptionAssert.Throws<ArgumentException>(() => ilist[1] = 42, "The value '42' is not of type 'System.String' and cannot be used in this generic collection. (Parameter 'value')");
        }

        [Test]
        public void ICollection_CopyTo_CopiesElementsToArray()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var icollection = (ICollection)wrapper;
            var array = new string[5];

            icollection.CopyTo(array, 1);

            Assert.AreEqual(null, array[0]);
            Assert.AreEqual("a", array[1]);
            Assert.AreEqual("b", array[2]);
            Assert.AreEqual("c", array[3]);
            Assert.AreEqual(null, array[4]);
        }

        [Test]
        public void ICollection_IsSynchronized_ReturnsFalse()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var icollection = (ICollection)wrapper;

            Assert.IsFalse(icollection.IsSynchronized);
        }

        [Test]
        public void ICollection_SyncRoot_ReturnsNonNullObject()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);
            var icollection = (ICollection)wrapper;

            Assert.IsNotNull(icollection.SyncRoot);
            
            // Should return the same object on subsequent calls
            Assert.AreSame(icollection.SyncRoot, icollection.SyncRoot);
        }

        [Test]
        public void UnderlyingCollection_WithGenericCollection_ReturnsGenericCollection()
        {
            var list = new List<string> { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>((ICollection<string>)list);

            Assert.AreSame(list, wrapper.UnderlyingCollection);
        }

        [Test]
        public void UnderlyingCollection_WithNonGenericList_ReturnsList()
        {
            var arrayList = new ArrayList { "a", "b", "c" };
            var wrapper = new CollectionWrapper<string>(arrayList);

            Assert.AreSame(arrayList, wrapper.UnderlyingCollection);
        }

        [Test]
        public void ValueType_Compatibility_WithNullableTypes()
        {
            var list = new List<int?> { 1, null, 3 };
            var wrapper = new CollectionWrapper<int?>((ICollection<int?>)list);
            var ilist = (IList)wrapper;

            Assert.IsTrue(ilist.Contains(null));
            Assert.IsTrue(ilist.Contains(1));
            Assert.IsFalse(ilist.Contains("string"));

            ilist.Add(null);
            Assert.AreEqual(4, wrapper.Count);
        }

        [Test]
        public void ValueType_Compatibility_WithValueTypes()
        {
            var list = new List<int> { 1, 2, 3 };
            var wrapper = new CollectionWrapper<int>((ICollection<int>)list);
            var ilist = (IList)wrapper;

            Assert.IsTrue(ilist.Contains(2));
            Assert.IsFalse(ilist.Contains(null));
            Assert.IsFalse(ilist.Contains("string"));
        }
    }
}