using System.Threading.Tasks;

namespace ExtractR.Mailer
{
    public interface IMailSender
    {
        Task<bool> TrySendSimpleMail(string message, string subject, string from, string to, bool isFeatureRequest, string userName = null);
    }
}