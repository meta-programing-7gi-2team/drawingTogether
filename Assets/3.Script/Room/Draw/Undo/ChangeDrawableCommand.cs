using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDrawableCommand : ICommand
{
    private Texture2D texture;
    private Texture2D previousTexture;
    private Texture2D newTexture;

    public ChangeDrawableCommand(Texture2D texture, Texture2D newTexture)
    {
        this.texture = texture;
        this.previousTexture = texture;
        this.newTexture = newTexture;
    }

    public void Execute()
    {
        texture = newTexture;
    }

    public void Undo()
    {
        texture = previousTexture;
    }
}
