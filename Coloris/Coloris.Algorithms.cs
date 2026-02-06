namespace Coloris;

public readonly partial struct Coloris
{
    // Gets CIE XYZ values from linear RGB channels, assuming sRGB color space and D65 illuminant.
    // Variables derived from the standard RGB to XYZ conversion matrix for sRGB with D65 white point
    // https://en.wikipedia.org/wiki/SRGB#Primaries
    // https://en.wikipedia.org/wiki/CIE_1931_color_space#From_RGB_to_CIE_XYZ
    // rL, gL, bL are in range [0,1].
    // Returns X, Y, Z in range [0,1].
    private static void CIEXYZ(float rL, float gL, float bL, out float x, out float y, out float z)
    {
        x = (rL * 0.4124564f) + (gL * 0.2126729f) + (bL * 0.0193339f);
        y = (rL * 0.2126729f) + (gL * 0.7151522f) + (bL * 0.0721750f);
        z = (rL * 0.0193339f) + (gL * 0.0721750f) + (bL * 0.9503041f);
    }

    // Gets gamma-corrected value from linear RGB channel.
    // C is in range [0,1].
    // Returns gamma-corrected value in range [0,1].
    private static float Gamma(float L)
    {
        if (L <= GAMMA_2_2_THRESHOLD)
            return L * LINEAR_UPPERFACTOR;

        return (LINEAR_LOWERFACTOR * (float)Math.Pow(L, 1 / LINEAR_GAMMACOEFFICIENT)) - LINEAR_INNERCURVE;
    }

    // Gets linearized value from sRGB channel.
    // C is in range [0,255]
    // Returns linear value in range [0,1]
    private static float VLinear(float C)
    {
        var v = C / 255f;

        if (v <= SRGBLINEAR_THRESHOLD)
            return v / LINEAR_UPPERFACTOR;

        return (float)Math.Pow((v + LINEAR_INNERCURVE) / LINEAR_LOWERFACTOR, LINEAR_GAMMACOEFFICIENT);
    }

    // Gets L* value from luminosity, assuming D65 illuminant and standard observer.
    // Y is in range [0,1].
    // Returns L* in range [0,100].
    private static float LStar(float Y)
    {
        if (Y <= CIE_LSTAR_THRESHOLD)
            return Y * CIE_LSTAR_UPPERMUL;

        return ((float)Math.Pow(Y, CIE_LSTAR_CUBEROOT_FACTOR) * 116f) - CIE_LSTAR_OFFSET;
    }

    // Splits bits of a byte into a 30-bit integer for Z-order curve calculation.
    // Only the lowest 10 bits are used, so input should be in range [0,255].
    private static int ZCurve(int a)
    {
        // split out the lowest 10 bits to lowest 30 bits
        a = (a | (a << 12)) & ZCURVE_SHIFT12;
        a = (a | (a << 08)) & ZCURVE_SHIFT08;
        a = (a | (a << 04)) & ZCURVE_SHIFT04;
        a = (a | (a << 02)) & ZCURVE_SHIFT02;

        return a;
    }

    // Rotates an angle by a certain degree amount, wrapping around at 360 degrees.
    // Returns the new angle in range [0,360].
    private static float Rotate(float angle, float degrees)
    {
        angle = (angle + degrees) % 360;

        if (angle < 0)
            angle += 360;

        return angle;
    }
}
