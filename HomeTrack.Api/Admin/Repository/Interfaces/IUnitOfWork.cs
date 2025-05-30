using HomeTrack.Application.Interface;

namespace HomeTrack.Application.Interfaces
{
  public interface IUnitOfWork : IDisposable
  {
    IPackageRepository Packages { get; }
    ISubscriptionRepository Subscriptions { get; }
    Task<int> CompleteAsync();
  }
}