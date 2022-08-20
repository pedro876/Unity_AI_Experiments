using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BrainImageGenerator
{
    const int NODE_SIZE_PIXELS = 10;
    const int EDGE_DIR_SIZE_PIXELS = 5;
    const int EDGE_THICKNESS_PIXELS = 3;
    const int BRAIN_TO_IMAGE_SPACE = 50;
    static Color EDGE_COLOR = Color.red;
    static Color NODE_COLOR = Color.blue;
    static Color BACKGROUND_COLOR = Color.black;

    public float displacementX = 0f;
    public float displacementY = 0f;
    public float zoom = 1f;

    private Color[] image;
    private int width;
    private int height;
    private float resMultiplier;
    private BrainState state;

    public BrainImageGenerator(BrainState state, int width, int height, float resMultiplier)
    {
        this.state = state;
        this.width = width;
        this.height = height;
        this.resMultiplier = resMultiplier;
        image = new Color[width* height];
    }

    public Color[] GenerateImage(out double generationSeconds)
    {
        DateTime now = DateTime.Now;
        FillBackground();
        DrawEdges();
        DrawNodes();
        generationSeconds = (DateTime.Now - now).TotalSeconds;
        return image;
    }

    private void FillBackground()
    {
        for (int i = 0; i < image.Length; i++)
            image[i] = BACKGROUND_COLOR;
    }

    private void DrawEdges()
    {
        for(int i = 0; i < state.totalSize; i++)
        {
            (int srcX, int srcY) = Transform(state.positions[i, 0], state.positions[i, 1]);

            for (int j = 0; j < state.totalSize; j++)
            {
                if (state.adjacencies[i, j])
                {
                    (int dstX, int dstY) = Transform(state.positions[j, 0], state.positions[j, 1]);

                    DrawLine(srcX, dstX, srcY, dstY, ref EDGE_COLOR);

                    float dirX = srcX - dstX;
                    float dirY = srcY - dstY;
                    float dist = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                    dirX /= dist;
                    dirY /= dist;
                    dirX *= NODE_SIZE_PIXELS * zoom;
                    dirY *= NODE_SIZE_PIXELS * zoom;

                    DrawCircle((int)(dstX + dirX+0.5f), (int)(dstY + dirY+0.5f), (int)(EDGE_DIR_SIZE_PIXELS * zoom), ref EDGE_COLOR);
                }
            }
        }
    }

    private void DrawNodes()
    {
        for (int i = 0; i < state.totalSize; i++)
        {
            (int x, int y) = Transform(state.positions[i, 0], state.positions[i, 1]);
            int radius = (int)(NODE_SIZE_PIXELS * zoom);
            if (i < state.inputSize)
                DrawDiamond(x, y, radius * 2, ref NODE_COLOR);
            else if (i < state.inputSize + state.outputSize)
                DrawSquare(x, y, radius * 2, ref NODE_COLOR);
            else
                DrawCircle(x, y, radius, ref NODE_COLOR);

        }
    }

    private void DrawSquare(int x, int y, int width, ref Color color)
    {
        width /= 2;
        for (int i = x - width; i < x + width; i++)
        {
            for (int j = y - width; j < y + width; j++)
            {
                PaintAt(i, j, ref color);
            }
        }
    }

    private void DrawDiamond(int x, int y, int width, ref Color color)
    {
        width /= 2;
        for (int i = x - width; i < x + width; i++)
        {
            for (int j = y - width; j < y + width; j++)
            {
                float fixedI = i + 0.5f - x;
                float fixedJ = j + 0.5f - y;
                if (Math.Abs(fixedI) +Math.Abs(fixedJ) < width)
                    PaintAt(i, j, ref color);
            }
        }
    }

    private void DrawCircle(int x, int y, int radius, ref Color color)
    {
        for(int i = x - radius; i < x + radius; i++)
        {
            for(int j = y - radius; j < y + radius; j++)
            {
                float fixedI = i + 0.5f - x;
                float fixedJ = j + 0.5f - y;
                bool inside = Math.Sqrt(fixedI * fixedI + fixedJ * fixedJ) <= radius;
                if (inside)
                {
                    PaintAt(i, j, ref color);
                }
            }
        }
    }

    private void DrawLine(int startX, int endX, int startY, int endY, ref Color color)
    {
        float distX = endX - startX;
        float distY = endY - startY;
        int times = (int)((Math.Sqrt(distX * distX + distY * distY)+0.5f)*1.5f);
        float increment = 1f / times;
        float currentX, currentY;
        int thickness = (int)Math.Max(1,EDGE_THICKNESS_PIXELS * zoom);
        for(int thicknessX = -thickness/2; thicknessX <= thickness/2; thicknessX++)
        {
            for (int thicknessY = -thickness/2; thicknessY <= thickness/2; thicknessY++)
            {
                float weight = 0f;
                for (int i = 0; i < times; i++)
                {
                    currentX = endX * weight + startX * (1f - weight) + 0.5f;
                    currentY = endY * weight + startY * (1f - weight) + 0.5f;
                    PaintAt((int)currentX + thicknessX, (int)currentY + thicknessY, ref color);
                    weight += increment;
                }
            }
        }
    }

    private void PaintAt(int x, int y, ref Color color)
    {
        int fixedX = x + width / 2;
        int fixedY = y + height / 2;
        if (fixedX < 0) return;
        if (fixedX >= width) return;
        if (fixedY < 0) return;
        if (fixedY >= height) return;
        image[fixedX + fixedY * width] = color;
    }

    private (int x, int y) Transform(float x, float y)
    {
        int transformedX = Mathf.RoundToInt((x * zoom + displacementX) * BRAIN_TO_IMAGE_SPACE);
        int transformedY = Mathf.RoundToInt((y * zoom + displacementY) * BRAIN_TO_IMAGE_SPACE);
        return (transformedX, transformedY);
    }
}

