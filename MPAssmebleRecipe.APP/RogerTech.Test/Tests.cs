using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RogerTech.Test
{
    /*
     * NUnit 基本用法
     *
     * 1) 特性（Attributes）
     *   - [TestFixture]：测试类
     *   - [Test]：测试用例
     *   - [SetUp] / [TearDown]：每个 [Test] 前/后运行
     *   - [OneTimeSetUp] / [OneTimeTearDown]：整个测试类前/后运行一次
     *   - [TestCase(...)]：参数化用例（同一方法跑多组参数）
     *   - [TestCaseSource(nameof(Source))]：从集合提供多组参数
     *
     * 2) 断言（Assertions）
     *   - Assert.That(actual, Is.EqualTo(expected));
     *   - Assert.That(obj, Is.Not.Null);
     *   - Assert.That(list, Is.Not.Empty);
     *   - Assert.That(() => code, Throws.Exception.TypeOf<...>());
     *
     * 3) 推荐写法
     *   - Arrange（准备） / Act（执行） / Assert（断言）
     *   - 每个测试只验证 1 个核心行为，失败时定位更快
     */
    [TestFixture]
    public class Tests
    {

        private Calculator _calc;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // 整个 Tests 类开始前跑一次：适合初始化昂贵资源（数据库、临时目录等）
        }

        [SetUp]
        public void SetUp()
        {
            // 每个 Test 开始前都会跑：适合初始化“干净的对象”
            _calc = new Calculator();
        }

        [TearDown]
        public void TearDown()
        {
            // 每个 Test 结束后都会跑：适合清理临时文件、释放资源等
            _calc = null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // 整个 Tests 类结束后跑一次
        }

        [Test]
        public void Assert_Example_Should_Pass()
        {
            // Arrange
            int a = 1;
            int b = 2;

            // Act
            int sum = a + b;

            // Assert
            Assert.That(sum, Is.EqualTo(3));
            Assert.That(sum, Is.GreaterThan(0));
        }

        [Test]
        public void Exception_Example_Should_Throw()
        {
            // Act + Assert：验证抛出异常
            Assert.That(() => _calc.Divide(1, 0), Throws.TypeOf<DivideByZeroException>());
        }

        [TestCase(1, 2, 3)]
        [TestCase(-1, 1, 0)]
        [TestCase(0, 0, 0)]
        public void TestCase_Example_Add(int a, int b, int expected)
        {
            int actual = _calc.Add(a, b);
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static IEnumerable<TestCaseData> DivideCases()
        {
            yield return new TestCaseData(10, 2, 5);
            yield return new TestCaseData(9, 3, 3);
            yield return new TestCaseData(-8, 2, -4);
        }

        [TestCaseSource(nameof(DivideCases))]
        public void TestCaseSource_Example_Divide(int a, int b, int expected)
        {
            int actual = _calc.Divide(a, b);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void String_Asserts_Example()
        {
            string s = "NJJG[12].TorqueResult";
            Assert.That(s, Does.Contain("[12]"));
            Assert.That(s, Does.StartWith("NJJG"));
            Assert.That(s, Does.EndWith("TorqueResult"));
            Assert.That(s, Is.Not.Empty);
        }

        private sealed class Calculator
        {
            public int Add(int a, int b) => a + b;
            public int Divide(int a, int b) => a / b;
        }

    }
}
