using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum PenMode
{
    Pen,
    Paint
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]

public class Drawable : MonoBehaviour
{
    public static Color Pen_Color = Color.red; //기본 컬러
    public static int Pen_Width = 4; //기본 펜 두께
    public static PenMode Pen_Mode = PenMode.Pen; //펜 모드

    public delegate void Brush_Function(Vector2 world_position);
    // 왼쪽 클릭 시 호출되는 함수
    // 브러쉬 유형 변경시 입력
    // Awake에서 기본 기능 설정
    public Brush_Function current_brush;

    public LayerMask Drawing_Layers;

    public bool Reset_Canvas_On_Play = true;
    public Color Reset_color = new Color(0, 0, 0, 0);  // 기본 리셋은 투명

    public static Drawable drawable;
    Sprite drawable_sprite;
    Texture2D drawable_texture;

    Vector2 previous_drag_position;
    Color[] clean_colors_array;
    Color32[] cur_colors;
    bool mouse_was_previously_held_down = false;
    bool no_drawing_on_current_drag = false;

    // 기본 브러쉬
    public void PenBrush(Vector2 world_point)
    {
        Vector2 pixel_pos = WorldToPixelCoordinates(world_point);

        cur_colors = drawable_texture.GetPixels32();

        if (previous_drag_position == Vector2.zero)
        {
            // 드래그를 하는 것이 처음이라면 마우스 위치에 브러쉬를 칠함.
            MarkPixelsTocolor(pixel_pos, Pen_Width, Pen_Color);
        }
        else
        {
            // 마지막 업데이트 위치로부터 한줄로 칠함.
            colorBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Color);
        }
        ApplyMarkedPixelChanges();
        previous_drag_position = pixel_pos;
    }

    //브러쉬 설정
    public void SetPenBrush()
    {
        current_brush = PenBrush;
    }

    void Update()
    {
        // 왼쪽 마우스 클릭 시
        bool mouse_held_down = Input.GetMouseButton(0);
        if (mouse_held_down)
        {
            // 마우스 좌표를 월드 좌표로 반환
            Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Pen_Mode.Equals(PenMode.Paint))
            {
                FloodFill(mouse_world_position, Pen_Color);
            }
            else if (Pen_Mode.Equals(PenMode.Pen) && !no_drawing_on_current_drag)
            {
                Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
                if (hit != null && hit.transform != null)
                {
                    current_brush(mouse_world_position);
                }

                else
                {
                    previous_drag_position = Vector2.zero;
                    if (!mouse_was_previously_held_down)
                    {
                        no_drawing_on_current_drag = true;
                    }
                }
            }
        }
        // 마우스 해제
        else if (!mouse_held_down)
        {
            previous_drag_position = Vector2.zero;
            no_drawing_on_current_drag = false;
        }
        mouse_was_previously_held_down = mouse_held_down;
    }

    // Set the color of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is colored
    public void colorBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
    {
        // Get the distance from start to finish
        float distance = Vector2.Distance(start_point, end_point);
        Vector2 direction = (start_point - end_point).normalized;

        Vector2 cur_position = start_point;

        // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            cur_position = Vector2.Lerp(start_point, end_point, lerp);
            MarkPixelsTocolor(cur_position, width, color);
        }
    }

    public void MarkPixelsTocolor(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // Figure out how many pixels we need to color in each direction (x and y)
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        //int extra_radius = Mathf.Min(0, pen_thickness - 2);

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
            if (x >= (int)drawable_sprite.rect.width || x < 0)
                continue;

            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                MarkPixelToChange(x, y, color_of_pen);
            }
        }
    }
    public void MarkPixelToChange(int x, int y, Color color)
    {
        // Need to transform x and y coordinates to flat coordinates of array
        int array_pos = y * (int)drawable_sprite.rect.width + x;

        // Check if this is a valid position
        if (array_pos > cur_colors.Length || array_pos < 0)
            return;

        cur_colors[array_pos] = color;
    }
    public void ApplyMarkedPixelChanges()
    {
        drawable_texture.SetPixels32(cur_colors);
        drawable_texture.Apply();
    }


    // Directly colors pixels. This method is slower than using MarkPixelsTocolor then using ApplyMarkedPixelChanges
    // SetPixels32 is far faster than SetPixel
    // colors both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)
    public void colorPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // Figure out how many pixels we need to color in each direction (x and y)
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        //int extra_radius = Mathf.Min(0, pen_thickness - 2);

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                drawable_texture.SetPixel(x, y, color_of_pen);
            }
        }

        drawable_texture.Apply();
    }
    public void FloodFill(Vector2 startPosition, Color fillcolor)
    {
        // Convert start position to pixel coordinates
        Vector2 pixelPos = WorldToPixelCoordinates(startPosition);
        int x = (int)pixelPos.x;
        int y = (int)pixelPos.y;

        // Get the current color at the starting pixel
        Color targetColor = drawable_texture.GetPixel(x, y);

        // If the target color is the same as the fill color, return (nothing to fill)
        if (targetColor == fillcolor)
            return;

        // Perform flood fill
        Queue<Vector2> pixels = new Queue<Vector2>();
        pixels.Enqueue(new Vector2(x, y));

        while (pixels.Count > 0)
        {
            Vector2 currentPixel = pixels.Dequeue();
            int px = (int)currentPixel.x;
            int py = (int)currentPixel.y;

            if (px < 0 || px >= drawable_texture.width || py < 0 || py >= drawable_texture.height)
                continue;

            Color currentColor = drawable_texture.GetPixel(px, py);

            if (currentColor == targetColor)
            {
                drawable_texture.SetPixel(px, py, fillcolor);

                // Enqueue neighboring pixels
                pixels.Enqueue(new Vector2(px - 1, py));
                pixels.Enqueue(new Vector2(px + 1, py));
                pixels.Enqueue(new Vector2(px, py - 1));
                pixels.Enqueue(new Vector2(px, py + 1));
            }
        }

        drawable_texture.Apply();
    }

    public Vector2 WorldToPixelCoordinates(Vector2 world_position)
    {
        // Change coordinates to local coordinates of this image
        Vector3 local_pos = transform.InverseTransformPoint(world_position);

        // Change these to coordinates of pixels
        float pixelWidth = drawable_sprite.rect.width;
        float pixelHeight = drawable_sprite.rect.height;
        float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

        // Need to center our coordinates
        float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
        float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

        // Round current mouse position to nearest pixel
        Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

        return pixel_pos;
    }

    public void ResetCanvas()
    {
        drawable_texture.SetPixels(clean_colors_array);
        drawable_texture.Apply();
    }

    void Awake()
    {
        drawable = this;
        // 기본 브러쉬 설정
        current_brush = PenBrush;

        drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
        drawable_texture = drawable_sprite.texture;

        // 픽셀 초기화
        clean_colors_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
        for (int x = 0; x < clean_colors_array.Length; x++)
            clean_colors_array[x] = Reset_color;

        // 캔버스를 초기 상태로 리셋
        if (Reset_Canvas_On_Play)
            ResetCanvas();
    }
}