using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public enum MoveAllowed
    {
        No,
        Yes,
        IfEmpty,
        IfAttack
    }
}
