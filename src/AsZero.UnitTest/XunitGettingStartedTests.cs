namespace AsZero.UnitTest;

/// <summary>
/// xUnit关键概念：
///     [Fact]：无参数测试用例（最常用）
///     [Theory]：参数化测试用例（同一逻辑跑多组输入）
///         常用数据源：[InlineData] / [MemberData] / [ClassData]
///     Assert.*：断言，表示“期望是什么”
///     Arrange -> Act -> Assert：常用结构（准备数据 -> 执行 -> 断言）
/// </summary>
public class XunitGettingStartedTests
{

    [Fact]
    public void Fact_example_basic_assertions()
    {
        // 1：Arrange 准备数据
        var input = "hello";

        // 2：Act 执行
        var upper = input.ToUpperInvariant();

        // 3：Assert 断言
        Assert.Equal("HELLO", upper);
        Assert.StartsWith("HE", upper);
        Assert.NotEqual("hello", upper);
    }

    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(10, 20, 30)]
    [InlineData(-5, 6, 1)]
    public void Theory_example_parameterized(int a, int b, int expected)
    {
        var actual = a + b;
        Assert.Equal(expected, actual);
    }

    [Fact(Skip = "演示如何跳过测试：把 Skip 去掉即可执行")]
    public void Skipped_test_example()
    {
        Assert.True(false);
    }

    [Fact]
    public async Task Async_test_example()
    {
        await Task.Delay(10);
        Assert.True(true);
    }

    [Fact]
    public void Assert_examples()
    {
        Assert.True(1 < 2);
        Assert.False(string.IsNullOrWhiteSpace("x"));
        Assert.NotNull(new object());
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3 }, 1)]
    [InlineData(new[] { 5, 6, 7 }, 5)]
    public void Theory_example_pick_first(int[] source, int expectedFirst)
    {
        var first = source[0];
        Assert.Equal(expectedFirst, first);
    }
    
    
    public static TheoryData<string, bool> EmailCases => new()
    {
        { "a@b.com", true },
        { "a@", false },
        { "", false }
    };

    [Theory]
    [MemberData(nameof(EmailCases))]
    public void MemberData_example(string email, bool expected)
    {
        var actual = LooksLikeEmail(email);
        Assert.Equal(expected, actual);
    }

    private static bool LooksLikeEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var at = value.IndexOf('@', StringComparison.Ordinal);
        return at > 0 && at < value.Length - 1;
    }
}

