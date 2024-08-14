using System.Collections.Generic;
using UnityEngine;

public enum PenMode
{
    Pen,
    Paint
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]

public class Drawable : MonoBehaviour
{
    public static Color Pen_Color = Color.red; //�⺻ �÷�
    public static int Pen_Width = 4; //�⺻ �� �β�
    public static PenMode Pen_Mode = PenMode.Pen; //�� ���

    public delegate void Brush_Function(Vector2 world_position);
    // ���� Ŭ�� �� ȣ��Ǵ� �Լ�
    // �귯�� ���� ����� �Է�
    // Awake���� �⺻ ��� ����
    public Brush_Function current_brush;

    public LayerMask Drawing_Layers;

    public bool Reset_Canvas_On_Play = true;
    public Color Reset_color = new Color(0, 0, 0, 0);  // �⺻ ������ ����

    public static Drawable drawable;
    Sprite drawable_sprite;
    Texture2D drawable_texture;

    Vector2 previous_drag_position;
    Color[] clean_colors_array;
    Color32[] cur_colors;
    bool mouse_was_previously_held_down = false;
    bool no_drawing_on_current_drag = false;

    // �⺻ �귯��
    public void PenBrush(Vector2 world_point)
    {
        Vector2 pixel_pos = WorldToPixelCoordinates(world_point);

        cur_colors = drawable_texture.GetPixels32();

        if (previous_drag_position == Vector2.zero)
        {
            // �巡�׸� �ϴ� ���� ó���̶�� ���콺 ��ġ�� �귯���� ĥ��.
            MarkPixelsTocolor(pixel_pos, Pen_Width, Pen_Color);
        }
        else
        {
            // ������ ������Ʈ ��ġ�κ��� ���ٷ� ĥ��.
            colorBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Color);
        }
        ApplyMarkedPixelChanges();
        previous_drag_position = pixel_pos;
    }

    //�귯�� ����
    public void SetPenBrush()
    {
        current_brush = PenBrush;
    }

    void Update()
    {
        // ���� ���콺 Ŭ�� ��
        bool mouse_held_down = Input.GetMouseButton(0);
        if (mouse_held_down)
        {
            // ���콺 ��ǥ�� ���� ��ǥ�� ��ȯ
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
        // ���콺 ����
        else if (!mouse_held_down)
        {
            previous_drag_position = Vector2.zero;
            no_drawing_on_current_drag = false;
        }
        mouse_was_previously_held_down = mouse_held_down;
    }

    // ���������� �������� ���� ǥ��
    public void colorBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
    {
        // �Ÿ� ���
        float distance = Vector2.Distance(start_point, end_point);

        Vector2 cur_position = start_point;

        // ���� Ƚ�� ���
        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            cur_position = Vector2.Lerp(start_point, end_point, lerp);
            MarkPixelsTocolor(cur_position, width, color);
        }
    }

    public void MarkPixelsTocolor(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            // �̹����� �ٸ� �ʿ� �ȼ��� �׸��� �ʴ��� Ȯ��
            if (x >= (int)drawable_sprite.rect.width || x < 0)
                continue;

            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                if (y >= (int)drawable_sprite.rect.width || y < 0)
                    continue;

                MarkPixelToChange(x, y, color_of_pen);
            }
        }
    }
    public void MarkPixelToChange(int x, int y, Color color)
    {
        // ��ǥ ��ȯ
        int array_pos = y * (int)drawable_sprite.rect.width + x;

        // index ��ġ üũ
        if (array_pos > cur_colors.Length || x < 1)
            return;

        cur_colors[array_pos] = color;
    }
    public void ApplyMarkedPixelChanges()
    {
        drawable_texture.SetPixels32(cur_colors);
        drawable_texture.Apply();
    }

    public void FloodFill(Vector2 startPosition, Color fillcolor)
    {
        // ���� ��ġ�� �ȼ� ��ǥ�� ��ȯ
        Vector2 pixelPos = WorldToPixelCoordinates(startPosition);
        int x = (int)pixelPos.x;
        int y = (int)pixelPos.y;

        // ���� �ȼ����� ���� ���� ��������
        Color targetColor = drawable_texture.GetPixel(x, y);

        // ��� ������ ä�� ����� ������ ��ȯ
        if (targetColor == fillcolor)
            return;

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
        // ��ǥ�� �� �̹����� ���� ��ǥ�� ����
        Vector3 local_pos = transform.InverseTransformPoint(world_position);

        // �ȼ� ��ǥ�� ����
        float pixelWidth = drawable_sprite.rect.width;
        float pixelHeight = drawable_sprite.rect.height;
        float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

        // ���� ��ǥ ���
        float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
        float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

        // ���� ���콺 ��ġ�� ���� ����� �ȼ��� �ݿø�
        Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

        return pixel_pos;
    }

    //ĵ���� �ʱ�ȭ
    public void ResetCanvas()
    {
        drawable_texture.SetPixels(clean_colors_array);
        drawable_texture.Apply();
    }

    void Awake()
    {
        drawable = this;
        // �⺻ �귯�� ����
        current_brush = PenBrush;

        drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
        drawable_texture = drawable_sprite.texture;

        // �ȼ� �ʱ�ȭ
        clean_colors_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
        for (int x = 0; x < clean_colors_array.Length; x++)
            clean_colors_array[x] = Reset_color;

        // ĵ������ �ʱ� ���·� ����
        if (Reset_Canvas_On_Play)
            ResetCanvas();
    }
}