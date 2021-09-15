using BlazorChess.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorChess.Services
{
    public class RefreshService : IRefreshService
    {
        public event Func<Task> RefreshRequested;
        public event Func<Board, Task> RefreshBoardRequested;
        private static readonly SemaphoreSlim refreshAsyncLock = new SemaphoreSlim(1,1);
        public async Task CallRequestRefresh()
        {
            await RefreshRequested.Invoke();
        }

        public async Task CallRefreshBoard(Board board)
        {
            await refreshAsyncLock.WaitAsync();
            try
            {
                await RefreshBoardRequested.Invoke(board);
            }
            finally
            {
                refreshAsyncLock.Release();
            }
        }


    }
}
