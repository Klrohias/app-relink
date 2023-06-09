﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AppRelink.Converters;

[ValueConversion(typeof(bool), typeof(string))]
public class BooleanToStringConverter : IValueConverter
{
    public string ValOnFalse { get; set; } = "";
    public string ValOnTrue { get; set; } = "";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (bool) value ? ValOnTrue : ValOnFalse;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        ValOnTrue.Equals(value);
}