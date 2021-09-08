using System;
using System.Threading.Tasks;

namespace BlazorChess.Services
{
    public interface IRefreshService
    {
        event Func<Task> RefreshRequested;
        Task CallRequestRefresh();
    }
}
