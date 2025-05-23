using Utya.Shared.Models;

namespace Utya.Services;

public interface IClickProcessor : IHostedService
{
    void EnqueueClick(Guid linkId, ClickData data);
}