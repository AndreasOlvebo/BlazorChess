using System;
using System.Threading.Tasks;

namespace BlazorChess.Services
{
    public class DefeatedPiecesRefreshService : IRefreshService
    {
        public event Func<Task> RefreshRequested;

        public async Task CallRequestRefresh()
        {
            await RefreshRequested.Invoke();
            
        }
    }
}
