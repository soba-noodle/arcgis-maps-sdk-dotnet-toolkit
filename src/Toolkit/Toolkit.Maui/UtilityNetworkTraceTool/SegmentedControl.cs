﻿// /*******************************************************************************
//  * Copyright 2012-2018 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/
using System;
using System.Linq;
using Microsoft.Maui.Layouts;
using Rectangle = Microsoft.Maui.Controls.Shapes.Rectangle;
using Rectangle2 = Microsoft.Maui.Graphics.Rect;

namespace Esri.ArcGISRuntime.Toolkit.Maui.Primitives;

/// <summary>
/// Internal control used by the <see cref="UtilityNetworkTraceTool"/>.
/// </summary>
public class SegmentedControl : GraphicsView
{
    private class SegmentDrawable : IDrawable
    {
        public bool IsHovered { get; set; }
        public int SelectedIndex { get; set; }
        public int CornerRadius { get; set; } = 8;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float segmentWidth = dirtyRect.Width / 4;

            // Draw background
            canvas.StrokeColor = Color.FromRgba(0, 0, 0, 0);
            if (IsHovered)
            {
                canvas.FillColor = Color.FromRgb(234, 234, 234);
            }
            else
            {
                canvas.FillColor = Color.FromRgb(243, 243, 243);
            }
            canvas.FillRoundedRectangle(0, 0, dirtyRect.Width, dirtyRect.Height, CornerRadius);

            // Draw selection background
            if (IsHovered)
            {
                canvas.FillColor = Color.FromRgb(0, 98, 155);
            }
            else
            {
                canvas.FillColor = Color.FromRgb(0, 123, 194);
            }
            
            canvas.FillRoundedRectangle(segmentWidth * SelectedIndex, 0, segmentWidth, 30, CornerRadius);

            // Draw unselected labels
            for(int x = 0; x < 5; x++)
            {
                if (x == SelectedIndex)
                {
                    canvas.FontColor = Color.FromRgb(255, 255, 255);
                    
                }
                else
                {
                    canvas.FontColor = Color.FromRgb(21, 21, 21);
                }

                switch (x)
                {
                    case 0:
                        canvas.DrawString("Select", new RectF(0, 0, segmentWidth, 30), HorizontalAlignment.Center, VerticalAlignment.Center);
                        break;
                    case 1:
                        canvas.DrawString("Configure", new RectF(segmentWidth, 0, segmentWidth, 30), HorizontalAlignment.Center, VerticalAlignment.Center);
                        break;
                    case 2:
                        canvas.DrawString("Run", new RectF(segmentWidth * 2, 0, segmentWidth, 30), HorizontalAlignment.Center, VerticalAlignment.Center);
                        break;
                    case 3:
                        canvas.DrawString("View", new RectF(segmentWidth * 3, 0, segmentWidth, 30), HorizontalAlignment.Center, VerticalAlignment.Center);
                        break;
                }
            }
        }
    }

    

    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentedControl"/> class.
    /// </summary>
    public SegmentedControl()
    {
        this.Drawable = new SegmentDrawable();
        this.MoveHoverInteraction += SegmentedControl_MoveHoverInteraction;
        this.EndHoverInteraction += SegmentedControl_EndHoverInteraction;
        this.StartInteraction += SegmentedControl_StartInteraction;
        this.BackgroundColor = Colors.Transparent;
    }

    private void SegmentedControl_StartInteraction(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Any())
        {
            var xCoord = e.Touches[0].X;
            var segmentWidth = Width / 4;
            var segmentPosition = xCoord / segmentWidth;
            SelectedSegmentIndex = (int)Math.Round(segmentPosition, 0, MidpointRounding.ToZero);
            (Drawable as SegmentDrawable)!.SelectedIndex = SelectedSegmentIndex;
            Invalidate();
        }
    }

    private void SegmentedControl_EndHoverInteraction(object? sender, EventArgs e)
    {
        (this.Drawable as SegmentDrawable)!.IsHovered = false;
        this.Invalidate();
    }

    private void SegmentedControl_MoveHoverInteraction(object? sender, TouchEventArgs e)
    {
        (this.Drawable as SegmentDrawable)!.IsHovered = true;
        this.Invalidate();
    }

    /// <summary>
    /// Gets or sets the index of the currently selected segment.
    /// </summary>
    public int SelectedSegmentIndex
    {
        get => (int)GetValue(SelectedSegmentIndexProperty);
        set => SetValue(SelectedSegmentIndexProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="SelectedSegmentIndex"/> bindable property.
    /// </summary>
    public static readonly BindableProperty SelectedSegmentIndexProperty = BindableProperty.Create(nameof(SelectedSegmentIndex), typeof(int), typeof(SegmentedControl), defaultValue: 0, propertyChanged: OnSelectionChanged);

    private static void OnSelectionChanged(BindableObject sender, object oldValue, object newValue)
    {
        ((sender as SegmentedControl)!.Drawable as SegmentDrawable)!.SelectedIndex = (int)newValue;
        (sender as SegmentedControl)?.Invalidate();
    }
}
