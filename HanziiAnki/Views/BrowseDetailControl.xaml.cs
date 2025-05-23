﻿using HanziiAnki.Core.Models;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace HanziiAnki.Views;

public sealed partial class BrowseDetailControl : UserControl
{
    public ExportedCard? ListDetailsMenuItem
    {
        get => GetValue(ListDetailsMenuItemProperty) as ExportedCard;
        set => SetValue(ListDetailsMenuItemProperty, value);
    }

    public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(ExportedCard), typeof(BrowseDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

    public BrowseDetailControl()
    {
        InitializeComponent();
    }

    private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BrowseDetailControl control)
        {
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
