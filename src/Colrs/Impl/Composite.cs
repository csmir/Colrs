namespace Colrs;

public readonly partial struct Colr
{
    // Combines hue, luminosity, and brightness into a single value for sorting. Hue is weighted most heavily, followed by luminosity and then brightness.
    // If smooth is true, the luminosity and brightness values are inverted for odd hue values to create a smoother gradient when sorting.
    private double GetHLVIndex(bool smooth)
    {
        var lum = GetLuminosity();
        GetHSV(out var h, out _, out var v);

        h *= CFACTOR;
        lum *= CFACTOR;
        v *= CFACTOR;

        if (smooth && h % 2 is 1)
        {
            v = CFACTOR - v;
            lum = CFACTOR - lum;
        }

        return h + lum + v;
    }

    // Combines inverted hue, luminosity, and brightness into a single value for sorting. Inverted hue is weighted most heavily, followed by luminosity and then brightness.
    private double GetHLVInvertedIndex(bool smooth)
    {
        var lum = GetLuminosity();
        var hue = 1 - Rotate(GetHue(), 180) / 360;
        var brightness = GetBrightness();

        var h2 = hue * CFACTOR;
        var v2 = brightness * CFACTOR;

        if (smooth)
        {
            if (h2 % 2 is 0)
                v2 = CFACTOR - v2;
            else
                lum = CFACTOR - lum;
        }

        return h2 + v2 + lum;
    }

    // Combines hue, saturation, and value into a single value for sorting. Hue is weighted most heavily, followed by saturation and then value.
    private double GetHSVIndex()
    {
        GetHSV(out var h, out var s, out var v);

        h /= 360f;
        v /= 255f;

        // combine the HSV values into a single value for sorting
        return (h * CFACTOR) + (s * CFACTOR) + (v * CFACTOR);
    }
}
