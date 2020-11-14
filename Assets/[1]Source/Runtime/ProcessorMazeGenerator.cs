using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixeye.Actors;

public class ProcessorMazeGenerator : Processor
{
    public ProcessorMazeGenerator()
    {
        Generate();
    }

    public void Generate()
    {
        debug.log("Generated maze");
    }
}
