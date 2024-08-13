using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingSettings : MonoBehaviour
{
    public static bool isCursorOverUI = false;

    public void SetMarkerColor(Color new_color)
    {
        Drawable.Pen_Color = new_color;
    }
    public void SetMarkerWidth(int new_width)
    {
        Drawable.Pen_Width = new_width;
    }

    //Ææ ¸ðµå ¼³Á¤
    public void SetPaint()
    {
        Drawable.Pen_Mode = PenMode.Paint;
    }
    public void SetPen()
    {
        Drawable.Pen_Mode = PenMode.Pen;
    }

    //Ææ ÄÃ·¯ ¼³Á¤
    public void SetMarkerBlack()
    {
        Color c = Color.black;
        SetMarkerColor(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerRed()
    {
        Color c = Color.red;
        SetMarkerColor(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerOrange()
    {
        Color c = new Color(1, 0.647f, 0);
        SetMarkerColor(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerYellow()
    {
        Color c = Color.yellow;
        SetMarkerColor(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerGreen()
    {
        Color c = Color.green;
        SetMarkerColor(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerBlue()
    {
        Color c = new Color(0f, 0.702f, 0.906f);
        SetMarkerColor(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerDarkBlue()
    {
        Color c = Color.blue;
        SetMarkerColor(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerMagenta()
    {
        Color c = Color.magenta;
        SetMarkerColor(c);
        Drawable.drawable.SetPenBrush();
    }
    public void SetMarkerWhite()
    {
        Color c = Color.white;
        SetMarkerColor(c);
        Drawable.drawable.SetPenBrush();
    }
}