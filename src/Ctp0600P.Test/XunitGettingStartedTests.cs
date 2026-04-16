namespace Ctp0600P.Test;

/// <summary>
/// 这个工程主要用于“客户端/VM 的纯逻辑测试”：
///     1. WPF 页面/UI 不适合直接单测（需要 UI 线程、控件实例、渲染环境）
///     2. ViewModel 里的业务逻辑（字符串处理、状态机、下一步选择、校验规则）适合单测
/// 写 VM 测试的步骤：
///     1. 只 new ViewModel（把依赖用 fake/mock 或者传 null!，前提是不会触发依赖调用）
///     2. 调用一个方法（或设置某个属性触发逻辑）
///     3. Assert：验证状态字段（例如 CurrentScrewNo、CurDoingScrew、列表状态）
/// </summary>
public class XunitGettingStartedTests
{

    [Fact]
    public void Test1()
    {
        Assert.True(true);
    }
    
}
