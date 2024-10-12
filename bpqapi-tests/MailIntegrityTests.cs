using System.Text.Json;

namespace bpqapi_tests;

public class MailIntegrityTests
{
    [Fact]
    public void Test()
    {
        var json = File.ReadAllText("testdata/response_1728752234250.json");
        var entity = JsonSerializer.Deserialize<MailItem>(json);
        File.WriteAllText("mailbody.txt", entity.body);
    }

    private readonly record struct MailItem
    {
        public string body { get; init; }
    }
}
