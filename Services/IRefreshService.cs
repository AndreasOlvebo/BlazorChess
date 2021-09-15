using BlazorChess.Models;
using System;
using System.Threading.Tasks;

namespace BlazorChess.Services
{
    public interface IRefreshService
    {
        event Func<Task> RefreshRequested;
        event Func<Board, Task> RefreshBoardRequested;
        Task CallRequestRefresh();
        Task CallRefreshBoard(Board board);
    }
}
