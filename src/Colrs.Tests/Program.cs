
using Colrs;
using System.Drawing;

var slightTransparentBlue = Color.Blue
    .ToColr()
    .SetAlpha(255)
    .ShiftHue(180)
    .ToColor();

Console.WriteLine(slightTransparentBlue);