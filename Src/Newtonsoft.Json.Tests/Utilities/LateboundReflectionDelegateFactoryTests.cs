using System;
using System.Reflection;
using Newtonsoft.Json.Utilities;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;

#endif

namespace Newtonsoft.Json.Tests.Utilities
{
    public class OutAndRefTestClass
    {
        public string Input { get; set; }
        public bool B1 { get; set; }
        public bool B2 { get; set; }

        public OutAndRefTestClass(ref string value)
        {
            Input = value;
            value = "Output";
        }

        public OutAndRefTestClass(ref string value, out bool b1)
            : this(ref value)
        {
            b1 = true;
            B1 = true;
        }

        public OutAndRefTestClass(ref string value, ref bool b1, ref bool b2)
            : this(ref value)
        {
            B1 = b1;
            B2 = b2;
        }
    }

    public class InTestClass
    {
        public string Value { get; }
        public bool B1 { get; }

        public InTestClass(in string value)
        {
            Value = value;
        }

        public InTestClass(in string value, in bool b1)
            : this(in value)
        {
            B1 = b1;
        }
    }

    [TestFixture]
    public class LateboundReflectionDelegateFactoryTests : TestFixtureBase
    {
        [Test]
        public void ConstructorWithInString()
        {
            ConstructorInfo constructor = TestReflectionUtils.GetConstructors(typeof(InTestClass)).Single(c => c.GetParameters().Count() == 1);

            var creator = LateBoundReflectionDelegateFactory.Instance.CreateParameterizedConstructor(constructor);

            object[] args = new object[] { "Value" };
            InTestClass o = (InTestClass)creator(args);
            Assert.IsNotNull(o);
            Assert.AreEqual("Value", o.Value);
        }

        [Test]
        public void ConstructorWithInStringAndBool()
        {
            ConstructorInfo constructor = TestReflectionUtils.GetConstructors(typeof(InTestClass)).Single(c => c.GetParameters().Count() == 2);

            var creator = LateBoundReflectionDelegateFactory.Instance.CreateParameterizedConstructor(constructor);

            object[] args = new object[] { "Value", true };
            InTestClass o = (InTestClass)creator(args);
            Assert.IsNotNull(o);
            Assert.AreEqual("Value", o.Value);
            Assert.AreEqual(true, o.B1);
        }

        [Test]
        public void ConstructorWithRefString()
        {
            ConstructorInfo constructor = TestReflectionUtils.GetConstructors(typeof(OutAndRefTestClass)).Single(c => c.GetParameters().Count() == 1);

            var creator = LateBoundReflectionDelegateFactory.Instance.CreateParameterizedConstructor(constructor);

            object[] args = new object[] { "Input" };
            OutAndRefTestClass o = (OutAndRefTestClass)creator(args);
            Assert.IsNotNull(o);
            Assert.AreEqual("Input", o.Input);
        }

        [Test]
        public void ConstructorWithRefStringAndOutBool()
        {
            ConstructorInfo constructor = TestReflectionUtils.GetConstructors(typeof(OutAndRefTestClass)).Single(c => c.GetParameters().Count() == 2);

            var creator = LateBoundReflectionDelegateFactory.Instance.CreateParameterizedConstructor(constructor);

            object[] args = new object[] { "Input", null };
            OutAndRefTestClass o = (OutAndRefTestClass)creator(args);
            Assert.IsNotNull(o);
            Assert.AreEqual("Input", o.Input);
        }

        [Test]
        public void ConstructorWithRefStringAndRefBoolAndRefBool()
        {
            ConstructorInfo constructor = TestReflectionUtils.GetConstructors(typeof(OutAndRefTestClass)).Single(c => c.GetParameters().Count() == 3);

            var creator = LateBoundReflectionDelegateFactory.Instance.CreateParameterizedConstructor(constructor);

            object[] args = new object[] { "Input", true, null };
            OutAndRefTestClass o = (OutAndRefTestClass)creator(args);
            Assert.IsNotNull(o);
            Assert.AreEqual("Input", o.Input);
            Assert.AreEqual(true, o.B1);
            Assert.AreEqual(false, o.B2);
        }

        [Test]
        public void CreateParameterizedConstructor_NullMethod_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                LateBoundReflectionDelegateFactory.Instance.CreateParameterizedConstructor(null);
            });
        }

        [Test]
        public void CreateParameterizedConstructor_StaticMethod_InvokesCorrectly()
        {
            MethodInfo method = typeof(DateTime).GetMethod("Parse", new[] { typeof(string) });

            var creator = LateBoundReflectionDelegateFactory.Instance.CreateParameterizedConstructor(method);

            object[] args = new object[] { "2023-01-01" };
            DateTime result = (DateTime)creator(args);
            Assert.AreEqual(new DateTime(2023, 1, 1), result);
        }

        [Test]
        public void CreateMethodCall_Constructor_InvokesCorrectly()
        {
            ConstructorInfo constructor = typeof(DateTime).GetConstructor(new[] { typeof(int), typeof(int), typeof(int) });

            var methodCall = LateBoundReflectionDelegateFactory.Instance.CreateMethodCall<object>(constructor);

            object[] args = new object[] { 2023, 1, 1 };
            DateTime result = (DateTime)methodCall(null, args);
            Assert.AreEqual(new DateTime(2023, 1, 1), result);
        }

        [Test]
        public void CreateMethodCall_InstanceMethod_InvokesCorrectly()
        {
            MethodInfo method = typeof(string).GetMethod("ToUpper", Type.EmptyTypes);

            var methodCall = LateBoundReflectionDelegateFactory.Instance.CreateMethodCall<string>(method);

            string result = (string)methodCall("hello", new object[0]);
            Assert.AreEqual("HELLO", result);
        }

        [Test]
        public void CreateMethodCall_StaticMethod_InvokesCorrectly()
        {
            MethodInfo method = typeof(Math).GetMethod("Abs", new[] { typeof(int) });

            var methodCall = LateBoundReflectionDelegateFactory.Instance.CreateMethodCall<object>(method);

            object[] args = new object[] { -5 };
            int result = (int)methodCall(null, args);
            Assert.AreEqual(5, result);
        }

        [Test]
        public void CreateMethodCall_NullMethod_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                LateBoundReflectionDelegateFactory.Instance.CreateMethodCall<object>(null);
            });
        }

        [Test]
        public void CreateDefaultConstructor_ValueType_CreatesCorrectly()
        {
            var constructor = LateBoundReflectionDelegateFactory.Instance.CreateDefaultConstructor<MyStruct>(typeof(MyStruct));

            MyStruct result = constructor();
            Assert.AreEqual(0, result.IntProperty);
        }

        [Test]
        public void CreateDefaultConstructor_ReferenceType_CreatesCorrectly()
        {
            var constructor = LateBoundReflectionDelegateFactory.Instance.CreateDefaultConstructor<object>(typeof(object));

            object result = constructor();
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(object), result.GetType());
        }

        [Test]
        public void CreateDefaultConstructor_NullType_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                LateBoundReflectionDelegateFactory.Instance.CreateDefaultConstructor<object>(null);
            });
        }

        [Test]
        public void CreateDefaultConstructor_NoDefaultConstructor_ThrowsInvalidOperationException()
        {
            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                LateBoundReflectionDelegateFactory.Instance.CreateDefaultConstructor<OutAndRefTestClass>(typeof(OutAndRefTestClass));
            });
        }

        [Test]
        public void CreateGet_PropertyInfo_GetsValue()
        {
            PropertyInfo property = typeof(MyStruct).GetProperty("IntProperty");
            var getter = LateBoundReflectionDelegateFactory.Instance.CreateGet<MyStruct>(property);

            MyStruct instance = new MyStruct { IntProperty = 42 };
            object result = getter(instance);
            Assert.AreEqual(42, result);
        }

        [Test]
        public void CreateGet_PropertyInfo_NullProperty_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                LateBoundReflectionDelegateFactory.Instance.CreateGet<MyStruct>((PropertyInfo)null);
            });
        }

        [Test]
        public void CreateGet_FieldInfo_GetsValue()
        {
            FieldInfo field = typeof(MyStruct).GetField("_intProperty", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var getter = LateBoundReflectionDelegateFactory.Instance.CreateGet<MyStruct>(field);

            MyStruct instance = new MyStruct { IntProperty = 42 };
            object result = getter(instance);
            Assert.AreEqual(42, result);
        }

        [Test]
        public void CreateGet_FieldInfo_NullField_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                LateBoundReflectionDelegateFactory.Instance.CreateGet<MyStruct>((FieldInfo)null);
            });
        }

        [Test]
        public void CreateSet_FieldInfo_SetsValue()
        {
            FieldInfo field = typeof(TestClassForGetSet).GetField("IntField");
            var setter = LateBoundReflectionDelegateFactory.Instance.CreateSet<TestClassForGetSet>(field);

            TestClassForGetSet instance = new TestClassForGetSet();
            setter(instance, 42);
            Assert.AreEqual(42, instance.IntField);
        }

        [Test]
        public void CreateSet_FieldInfo_NullField_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                LateBoundReflectionDelegateFactory.Instance.CreateSet<MyStruct>((FieldInfo)null);
            });
        }

        [Test]
        public void CreateSet_PropertyInfo_SetsValue()
        {
            PropertyInfo property = typeof(TestClassForGetSet).GetProperty("StringProperty");
            var setter = LateBoundReflectionDelegateFactory.Instance.CreateSet<TestClassForGetSet>(property);

            TestClassForGetSet instance = new TestClassForGetSet();
            setter(instance, "hello");
            Assert.AreEqual("hello", instance.StringProperty);
        }

        [Test]
        public void CreateSet_PropertyInfo_NullProperty_ThrowsArgumentNullException()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                LateBoundReflectionDelegateFactory.Instance.CreateSet<MyStruct>((PropertyInfo)null);
            });
        }

        [Test]
        public void Instance_ReturnsSameInstance()
        {
            var instance1 = LateBoundReflectionDelegateFactory.Instance;
            var instance2 = LateBoundReflectionDelegateFactory.Instance;

            Assert.AreSame(instance1, instance2);
        }
    }

    public class TestClassForGetSet
    {
        public string StringProperty { get; set; }
        public int IntField;
        private readonly bool _readOnlyProperty;

        public bool ReadOnlyProperty => _readOnlyProperty;

        public TestClassForGetSet(bool readOnlyValue = false)
        {
            _readOnlyProperty = readOnlyValue;
            StringProperty = "";
            IntField = 0;
        }
    }

    public struct MyStruct
    {
        private int _intProperty;

        public int IntProperty
        {
            get { return _intProperty; }
            set { _intProperty = value; }
        }
    }
}