namespace Coloris;

public readonly partial struct Coloris
{
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

    private double GetHSVIndex()
    {
        GetHSV(out var h, out var s, out var v);

        h /= 360f;
        v /= 255f;

        // combine the HSV values into a single value for sorting
        return (h * CFACTOR) + (s * CFACTOR) + (v * CFACTOR);
    }
}
