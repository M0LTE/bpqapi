using bpqapi.Models;

namespace bpqapi.Services;

public class MailRepository(ILogger<MailRepository> logger)
{
    internal async Task<List<MailEntity>> GetMailItems(int[] ids)
    {
        var dbRows = await DbInfo.GetAsyncConnection().QueryAsync<DbMail>($"select * from mail where id IN ({string.Join(",", ids.Select(i => "?"))})", ids.Cast<object>().ToArray());

        var items = dbRows.Select(s => new MailEntity
        {
            Id = s.Id,
            State = s.State[0],
            Mid = s.Mid,
            Bid = s.Bid,
            DateTime = s.DateTime,
            Type = s.Type[0],
            From = s.From,
            To = s.To,
            Subject = s.Subject,
            Mbo = s.Mbo,
            ContentType = s.ContentType,
            ContentTransferEncoding = s.ContentTransferEncoding,
            Body = s.Body,
            Date = new MonthAndDay(s.DateTime.Month, s.DateTime.Day),
            Time = new TimeOnly(s.DateTime.Hour, s.DateTime.Minute, s.DateTime.Second),
            Read = s.Read
        }).ToList();

        logger.LogInformation("Loaded {0} mail items from db", items.Count);

        return items;
    }

    internal async Task SaveMailItems(List<MailEntity> items)
    {
        var connection = DbInfo.GetAsyncConnection();

        foreach (var item in items)
        {
            var dbItem = new DbMail
            {
                Id = item.Id,
                State = item.State.ToString()!,
                Mid = item.Mid,
                Bid = item.Bid,
                DateTime = item.DateTime!.Value,
                Type = item.Type.ToString(),
                From = item.From,
                To = item.To,
                Subject = item.Subject,
                Mbo = item.Mbo,
                ContentType = item.ContentType,
                ContentTransferEncoding = item.ContentTransferEncoding,
                Body = item.Body,
                Read = item.Read
            };

            await connection.InsertOrReplaceAsync(dbItem);
        }

        logger.LogInformation("Saved {0} mail items to db", items.Count);
    }
}
