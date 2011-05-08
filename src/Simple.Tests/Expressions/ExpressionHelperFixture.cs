﻿using System;
using System.Linq.Expressions;
using NUnit.Framework;
using SharpTestsEx;
using System.Collections.Generic;

namespace Simple.Tests.Expressions
{
    [TestFixture]
    public class ExpressionHelperFixture
    {
        class A
        {
            public B BProp { get; set; }
        }

        class B
        {
            public C CProp { get; set; }
            public int Diff { get; set; }
        }

        class C
        {
            public int IntProp { get; set; }
            public D DProp { get; set; }
            public IList<D> DList { get; set; }
        }

        class D
        {
            public D(int a) { Value = a; }
            public int Value { get; set; }
            public int GetValue() { return this.Value; }
        }

     

        [Test]
        public void TestSimplePropertyName()
        {
            var propName = ExpressionHelper.GetMemberName((A a) => a.BProp);
            propName.Should().Be("BProp");
        }

        [Test]
        public void TestNestedPropertyName()
        {
            var propName = ExpressionHelper.GetMemberName((A a) => a.BProp.CProp.IntProp);
            propName.Should().Be("BProp.CProp.IntProp");
        }

        

        [Test]
        public void TestSimplePropertySet()
        {
            Expression<Func<A, B>> lambda = x => x.BProp;

            A a = new A();
            ExpressionHelper.SetValue(lambda, a, new B() { Diff = 42 });


            a.BProp.Should().Not.Be.Null();
            a.BProp.Diff.Should().Be(42);
        }

        [Test]
        public void GetMemberPathReturnPropertyNames()
        {
            Expression<Func<A, int>> lambda = x => x.BProp.CProp.DProp.Value;
            lambda.GetMemberPath().Should().Have.SameSequenceAs("BProp", "CProp", "DProp", "Value");
        }



        [Test]
        public void GetMemberListWithoutMembersReturnAnEmptyList()
        {
            Expression<Func<A, A>> lambda = x => x;
            lambda.GetMemberList().Should().Be.Empty();
        }

        [Test]
        public void GetMemberListWithNullExpressionShouldReturnEmptyList()
        {
            ExpressionHelper.GetMemberList(null).Should().Be.Empty();
        }

        [Test]
        public void TestSimplePropertySetUsingIConvertible()
        {
            Expression<Func<A, int>> lambda = x => x.BProp.CProp.IntProp;

            A a = new A();
            ExpressionHelper.SetValue(lambda, a, "42");

            a.BProp.CProp.IntProp.Should().Be(42);
        }

        [Test]
        public void TestSimplePropertySetUsingNonConvertibleTypeFails()
        {
            Expression<Func<A, int>> lambda = x => x.BProp.CProp.IntProp;

            A a = new A();
            a.Executing(x=> ExpressionHelper.SetValue(lambda, x, new C()))
                .Throws<ArgumentException>();
        }

        [Test]
        public void TestSimplePropertySetList()
        {
            Expression<Func<A, IList<D>>> lambda = x => x.BProp.CProp.DList;

            A a = new A();
            ExpressionHelper.SetValue(lambda, a, new[] { new D(1), new D(2) });

            a.BProp.CProp.DList.Should().Not.Be.Null();
        }


        [Test, ExpectedException(typeof(NotSupportedException))]
        public void FailSettingMethod()
        {
            Expression<Func<A, int>> lambda = x => x.BProp.CProp.DProp.GetValue();

            A a = new A();

            ExpressionHelper.SetValue(lambda, a, 50);
        }

        [Test]
        public void TestSimplePropertySetNewInstance()
        {
            Expression<Func<A, B>> lambda = x => x.BProp;

            A a = new A();
            ExpressionHelper.SetValue(lambda, a, new B());


            a.BProp.Should().Not.Be.Null();
            a.BProp.Diff.Should().Be(0);
        }

        [Test]
        public void TestNestedPropertySet()
        {
            Expression<Func<A, int>> lambda = x => x.BProp.CProp.IntProp;

            A a = new A();
            ExpressionHelper.SetValue(lambda, a, 42);

            a.BProp.Should().Not.Be.Null();
            a.BProp.Diff.Should().Be(0);
            a.BProp.CProp.Should().Not.Be.Null();
            a.BProp.CProp.IntProp.Should().Be(42);
        }

        [Test, ExpectedException(typeof(MissingMethodException))]
        public void TestNoDefaultConstructorSet()
        {
            Expression<Func<A, int>> lambda = x => x.BProp.CProp.DProp.Value;

            A a = new A();
            ExpressionHelper.SetValue(lambda, a, 42);

        }

        [Test]
        public void TestGetPropertyFromString()
        {
            var type = typeof(A);
            var str = "BProp.CProp.DProp";

            var prop = str.GetMember(type);

            Assert.That(prop.Name, Is.EqualTo("DProp"));
            Assert.That(prop.DeclaringType, Is.EqualTo(typeof(C)));
            Assert.That(prop.Type, Is.EqualTo(typeof(D)));
        }

        [Test]
        public void TestGetPropertyFromStringArray()
        {
            var type = typeof(A);
            var str = new[] { "BProp", "CProp", "DProp" };

            var prop = str.GetMember(type);

            Assert.That(prop.Name, Is.EqualTo("DProp"));
            Assert.That(prop.DeclaringType, Is.EqualTo(typeof(C)));
            Assert.That(prop.Type, Is.EqualTo(typeof(D)));
        }

        [Test]
        public void TestGetPropertyFromStringAndGenericType()
        {
            var str = "BProp.CProp.DProp";

            var prop = str.GetMember<A>();

            Assert.That(prop.Name, Is.EqualTo("DProp"));
            Assert.That(prop.DeclaringType, Is.EqualTo(typeof(C)));
            Assert.That(prop.Type, Is.EqualTo(typeof(D)));
        }

        [Test]
        public void TestGetPropertyFromStringArrayAndGenericType()
        {
            var str = new[] { "BProp", "CProp", "DProp" };

            var prop = str.GetMember<A>();

            Assert.That(prop.Name, Is.EqualTo("DProp"));
            Assert.That(prop.DeclaringType, Is.EqualTo(typeof(C)));
            Assert.That(prop.Type, Is.EqualTo(typeof(D)));
        }

        [Test]
        public void TestGetPropertyFromEmptyString()
        {
            var str = "";

            var prop = str.GetMember<A>();

            Assert.That(prop, Is.Null);
        }

        [Test]
        public void TestTryGetNonExistingPropertyFromString()
        {
            var str = "KProp";

            Assert.Throws<ArgumentException>(() =>
            {
                str.GetMember<A>();
            });
        }


        [Test]
        public void TestGetPropertyExpressionFromString()
        {
            var str = "BProp.CProp.DProp";

            var expr = Expression.Parameter(typeof(A), "x");
            var prop = str.GetMemberExpression(expr);

            prop.ToString().Should().Be("x.BProp.CProp.DProp");
        }

        [Test]
        public void TestGetPropertyExpressionFromStringArray()
        {
            var str = new[] { "BProp", "CProp", "DProp" };

            var expr = Expression.Parameter(typeof(A), "x");
            var prop = str.GetMemberExpression(expr);

            prop.ToString().Should().Be("x.BProp.CProp.DProp");

        }

        [Test]
        public void TestGetPropertyLambdaWithObjectReturnTypeFromStringArray()
        {
            var str = new[] { "BProp", "CProp", "DProp" };

            var prop = str.GetMemberExpression<A>();

            prop.ToString().Should().Be("x => x.BProp.CProp.DProp");

        }

        [Test]
        public void TestGetPropertyLambdaWithDReturnTypeFromStringArray()
        {
            var str = new[] { "BProp", "CProp", "DProp" };

            var prop = str.GetMemberExpression<A, D>();

            prop.ToString().Should().Be("x => x.BProp.CProp.DProp");

        }

        [Test]
        public void TestGetPropertyLambdaWithObjectReturnTypeFromString()
        {
            var str = "BProp.CProp.DProp";

            var prop = str.GetMemberExpression<A>();

            prop.ToString().Should().Be("x => x.BProp.CProp.DProp");

        }

        [Test]
        public void TestGetPropertyLambdaWithObjectReturnTypeFromStringUsingIntProperty()
        {
            var str = "BProp.CProp.DProp.Value";

            var prop = str.GetMemberExpression<A>();

            prop.ToString().Should().Be("x => Convert(x.BProp.CProp.DProp.Value)");

        }


        [Test]
        public void TestGetPropertyLambdaWithDReturnTypeFromString()
        {
            var str = "BProp.CProp.DProp";

            var prop = str.GetMemberExpression<A, D>();

            prop.ToString().Should().Be("x => x.BProp.CProp.DProp");

        }


        [Test]
        public void TestGetPropertyExpressionFromEmptyString()
        {
            var str = "";
            var expr = Expression.Parameter(typeof(A), "x");
            var prop = str.GetMemberExpression(expr);

            prop.ToString().Should().Be("x");
        }

        [Test]
        public void TestTryGetNonExistingPropertyExpressionFromString()
        {
            var str = "KProp";
            var expr = Expression.Parameter(typeof(A), "x");

            Assert.Throws<ArgumentException>(() =>
            {
                str.GetMemberExpression(expr);
            });
        }
    }

}
