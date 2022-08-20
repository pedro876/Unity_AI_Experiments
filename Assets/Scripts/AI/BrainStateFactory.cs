using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class BrainStateFactory
{
    public static Brain CreateSimpleBrain(int iterationsPerSecond = 60)
    {
        BrainState state = new BrainState(1, 1, 2, iterationsPerSecond);
        state.positions[1, 1] = -1f; //output cell down
        //state.positions[1, 0] = -1f; //output cell right
        Brain brain = new Brain(state);
        return brain;
    }
}

