using Amigos;
using System;

public class IncImpl : IInc
{
    int x = 0;

    public IncImpl()
    {
        x = 0;
    }
    public int Inc()
    {
        x++;
        return x;
    }
}