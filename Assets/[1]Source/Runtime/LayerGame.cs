using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixeye.Actors;

public class LayerGame : Layer<LayerGame>
{
    protected override void Setup()
    {
        Add<ProcessorUpdate>();
        Add<ProcessorMazeGenerator>();

        // Get<ProcessorMazeGenerator>().Generate();
        Debug.Log("setuped");
    }
}
