using bpqapi.Models;
using bpqapi.Services;

namespace bpqapi.Controllers;

public class MailService(BpqUiService bpqUiService, BpqNativeApiService bpqNativeApiService, MailRepository mailRepository)
{
    public async Task<List<MailEntity>> GetMail(string user, string password, int[] ids)
    {
        var mailFromDb = await mailRepository.GetMailItems(ids);
        var itemsFromBpqUi = await bpqUiService.GetWebmailItems(user, password, ids.Except(mailFromDb.Select(s => s.Id)).ToArray());

        if (itemsFromBpqUi.Count != 0)
        {
            var token = await bpqNativeApiService.RequestMailToken(user, password);
            var nativeResponse = await bpqNativeApiService.GetMessagesV1(token.AccessToken);

            foreach (var item in itemsFromBpqUi)
            {
                var nativeItem = nativeResponse.Messages.Single(m => m.Id == item.Id);
                item.DateTime = DateTime.UnixEpoch.AddSeconds(nativeItem.Received);
            }
            
            await mailRepository.SaveMailItems(itemsFromBpqUi);
        }

        return itemsFromBpqUi.Concat(mailFromDb).ToList();
    }

    internal async Task<bool> SetReadState(int id, bool value)
    {
        var item = (await mailRepository.GetMailItems([id])).SingleOrDefault();

        if (item == null)
        {
            return false;
        }

        item.Read = value;

        await mailRepository.SaveMailItems([item]);
        return true;
    }
}
