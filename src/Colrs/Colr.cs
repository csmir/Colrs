using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Supports F# and VB.NET. Some APIs are marked as CLSCompliant(false) due to unsigned types, but the main API is CLS compliant.
[assembly: CLSCompliant(true)]

namespace Colrs;

/// <summary>
///     This type directly equates the memory layout of System.Drawing.Color.
/// </summary>
/// <remarks>
///     The reason for this type existing is because the overhead of creating a new System.Drawing.Color is absurd. It quite possibly is the worst performing struct in the entire .NET framework.
///     Rant aside, <see cref="Colr.ToColor"/> directly transforms the memory of a <see cref="Colr"/> to a <see cref="Color"/> by using <see cref="Unsafe.As{TFrom, TTo}(ref TFrom)"/> over this struct.
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
internal struct SystemColor
{
    /// <summary>
    ///     string? System.Drawing.Color.name. Because its a string, it can be set to 0 in an nint to fit in the same memory.
    /// </summary>
    [FieldOffset(0)]
    public nint Name;

    /// <summary>
    ///     long? System.Drawing.Color.value. This is the value that has equality to Colr.Value.
    /// </summary>
    [FieldOffset(8)]
    public long Value;

    /// <summary>
    ///     short System.Drawing.Color.knownColor, but it can be set to 0 in a short to fit in the same memory.
    /// </summary>
    [FieldOffset(16)]
    public short KnownColor;

    /// <summary>
    ///     short System.Drawing.Color.state. This is the value that has to be set to 0x0002 for the Color to be valid when using the Value field.
    /// </summary>
    [FieldOffset(18)]
    public short State;
}

/// <summary>
///     An sRGB color representation that uses a 32-bit unsigned integer to store the RGBA (red, green, blue, alpha) colour channels.
/// </summary>
/// <remarks>
///     Create a new Colr from ARGB uint representation of the color. Other color formats are accepted using static FromX methods.
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
[DebuggerDisplay("R = {R}, G = {G}, B = {B}, A = {A}")]
public readonly partial struct Colr :
    IEquatable<Colr>,
    IEquatable<Color>,
    ICloneable
{
    /// <summary>
    ///     The 32-bit unsigned integer representation of the colour.
    /// </summary>
    [FieldOffset(0)]
    [CLSCompliant(false)]
    public readonly uint Value;

    /// <summary>
    ///     The B (blue) colour channel.
    /// </summary>
    /// <remarks>
    ///     A value of 0 results in no blue being present in the colour, while a value of 255 results in full blue intensity.
    /// </remarks>
    [FieldOffset(0)]
    public readonly byte B;

    /// <summary>
    ///     The G (green) colour channel.
    /// </summary>
    /// <remarks>
    ///     A value of 0 results in no green being present in the colour, while a value of 255 results in full green intensity.
    /// </remarks>
    [FieldOffset(1)]
    public readonly byte G;

    /// <summary>
    ///     The R (red) colour channel.
    /// </summary>
    /// <remarks>
    ///     A value of 0 results in no red being present in the colour, while a value of 255 results in full red intensity.
    /// </remarks>
    [FieldOffset(2)]
    public readonly byte R;

    /// <summary>
    ///     The A (alpha) colour channel, which controls the opacity of the colour. 
    /// </summary>
    /// <remarks>
    ///     A value of 0 is fully transparent, while a value of 255 is fully opaque.
    /// </remarks>
    [FieldOffset(3)]
    public readonly byte A;

    /// <summary>
    ///     Creates a new <see cref="Colr"/> value based on the provided 32-bit sRGB (A) representation.
    /// </summary>
    /// <param name="argb">A 32 bit representation of RGBA.</param>
    [CLSCompliant(false)]
    public Colr(uint argb)
        => Value = argb;

    #region Internal Constructors

    private Colr(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }

    private Colr(float h, float s, float v)
    {
        A = byte.MaxValue;

        var hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
        var f = h / 60 - Math.Floor(h / 60);

        v *= byte.MaxValue;

        var b = Convert.ToByte(v);
        var p = Convert.ToByte(v * (1 - s));
        var q = Convert.ToByte(v * (1 - f * s));
        var t = Convert.ToByte(v * (1 - (1 - f) * s));

        switch (hi)
        {
            case 0:
                R = b; G = t; B = p;
                break;
            case 1:
                R = q; G = b; B = p;
                break;
            case 2:
                R = p; G = b; B = t;
                break;
            case 3:
                R = p; G = q; B = b;
                break;
            case 4:
                R = t; G = p; B = b;
                break;
            default:
                R = b; G = p; B = q;
                break;
        }
    }

    private Colr(float h, float s, float l, float a = 1f)
    {
        A = (byte)(a * byte.MaxValue);

        var c = (1 - Math.Abs(2 * l - 1)) * s;
        var x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        var m = l - c / 2;

        byte r, g, b;

        if (h < 60)
        {
            r = (byte)(c * byte.MaxValue);
            g = (byte)(x * byte.MaxValue);
            b = 0;
        }
        else if (h < 120)
        {
            r = (byte)(x * byte.MaxValue);
            g = (byte)(c * byte.MaxValue);
            b = 0;
        }
        else if (h < 180)
        {
            r = 0;
            g = (byte)(c * byte.MaxValue);
            b = (byte)(x * byte.MaxValue);
        }
        else if (h < 240)
        {
            r = 0;
            g = (byte)(x * byte.MaxValue);
            b = (byte)(c * byte.MaxValue);
        }
        else if (h < 300)
        {
            r = (byte)(x * byte.MaxValue);
            g = 0;
            b = (byte)(c * byte.MaxValue);
        }
        else
        {
            r = (byte)(c * byte.MaxValue);
            g = 0;
            b = (byte)(x * byte.MaxValue);
        }

        R = (byte)(r + m * byte.MaxValue);
        G = (byte)(g + m * byte.MaxValue);
        B = (byte)(b + m * byte.MaxValue);
    }

    #endregion

    /// <summary>
    ///     Gets the luminosity of the color according to the Rec. 709 standard, 
    ///     implementing its Luma coefficients over linearized RGB values. (V-linear algorithm)
    /// </summary>
    /// <returns>True luminosity in accordance to Rec. 709 coefficients over V-linear.</returns>
    public float GetLuminosity()
        => (REC_709_R * VLinear(R))
         + (REC_709_G * VLinear(G))
         + (REC_709_B * VLinear(B));

    /// <summary>
    ///     Gets the Rec. 709 relative luminance for the current color, 
    ///     applying the Luma coefficient without linearization.
    /// </summary>
    /// <returns>Relative luminance in accordance to Rec. 709 coefficients.</returns>
    public float GetRelativeLuminance()
        => (REC_709_R * R)
         + (REC_709_G * G)
         + (REC_709_B * B);

    /// <summary>
    ///     Gets the perceived lightness of the color according to the CIE L* color space. 
    ///     This takes the Rec. 709 luminosity and converts it to L*.
    /// </summary>
    /// <returns>Perceived lightness in accordance to Rec. 709 luminosity to L*.</returns>
    public float GetPerceivedLightness()
        => LStar(GetLuminosity());

    /// <summary>
    ///     Gets the perceived brightness of the current color according to the HSP color model using the BT.601 coefficients.
    /// </summary>
    /// <returns>Perceived brightness in accordance to BT.601 coefficients.</returns>
    public float GetPerceivedBrightness()
        => (float)Math.Sqrt(
            BT_601_R * Math.Pow(R, 2) +
            BT_601_G * Math.Pow(G, 2) +
            BT_601_B * Math.Pow(B, 2)
        );

    /// <summary>
    ///     Gets the wavelength of the color based on its hue, 
    ///     mapping the hue range (0-360) to a wavelength range (400-700 nm) in the visible spectrum.
    /// </summary>
    /// <returns>The combined wavelength of the color in the visible spectrum.</returns>
    public float GetCombinedWavelength()
        => 400 / 270 * GetHue();

    /// <summary>
    ///     Gets the gamma-corrected (TRC) luminance of the color using BT.601 coefficients.
    /// </summary>
    /// <returns>The gamma-corrected luminance of the color.</returns>
    public float GetTransferCurve()
        => BT_601_R * Gamma(R)
         + BT_601_G * Gamma(G)
         + BT_601_B * Gamma(B);

    /// <summary>
    ///     Gets a Z-order value for the color by interleaving the bits of the RGB channels, 
    ///     effectively creating a 30-bit integer that can be used for spatial sorting of colors in a 3D RGB space.
    /// </summary>
    /// <returns>The Z-order value for this color.</returns>
    public int GetZValue()
        => ZCurve(R)
         + (ZCurve(G) << 1)
         + (ZCurve(B) << 2);

    /// <summary>
    ///     Gets the hue of the color between 0 and 360 degrees.
    /// </summary>
    /// <returns>The hue of the current color.</returns>
    public float GetHue()
    {
        if (IsRGBEquals())
            return 0f;

        GetMinMax(out var min, out var max);

        float delta = max - min;
        float hue;

        int r, g, b;

        r = R;
        g = G;
        b = B;

        if (r == max)
            hue = (g - b) / delta;
        else if (g == max)
            hue = (b - r) / delta + 2f;
        else
            hue = (r - g) / delta + 4f;

        hue *= 60f;
        if (hue < 0f)
            hue += 360f;

        return hue;
    }

    /// <summary>
    ///     Gets the HSL accepted saturation of the color as a percentile between 0 and 1.
    /// </summary>
    /// <remarks>
    ///     Intensity is represented between 0% (grayscale) and 100% (full color).
    /// </remarks>
    /// <returns>The saturation of the current color.</returns>
    public float GetSaturation()
    {
        if (IsRGBEquals())
            return 0f;

        GetMinMax(out var min, out var max);

        var div = max + min;

        if (div > byte.MaxValue)
            div = byte.MaxValue * 2 - max - min;

        return (max - min) / (float)div;
    }

    /// <summary>
    ///     Gets the HSL accepted brightness (lightness) of the color as a percentile between 0 and 1.
    /// </summary>
    /// <remarks>
    ///     Brightness is represented between 0% (black) and 100% (white), where 50% is normal.
    /// </remarks>
    /// <returns>The brightness of the current color.</returns>
    public float GetBrightness()
    {
        GetMinMax(out var min, out var max);

        return (max + min) / (byte.MaxValue * 2f);
    }

    /// <summary>
    ///     Gets the contrast ratio between this color and another color according to the WCAG guidelines.
    /// </summary>
    /// <param name="o">The color to compare to to define the contrast.</param>
    /// <returns>The contrast ratio between the two colors, where a higher value indicates greater contrast.</returns>
    public double GetContrastRatio(Colr o)
    {
        var l1 = GetRelativeLuminance();

        var l2 = o.GetRelativeLuminance();

        return (Math.Max(l1, l2) + 0.05) / (Math.Min(l1, l2) + 0.05);
    }

    /// <summary>
    ///     Gets the Euclidean distance between this color and another color in RGB space, 
    ///     providing a simple measure of how different the two colors are based on their red, green, and blue channel values.
    /// </summary>
    /// <param name="o">The other color to calculate the Euclidian distance from.</param>
    /// <returns>The Euclidean distance between the two colors in RGB space, where a higher value indicates greater difference.</returns>
    public double GetEuclidian(Colr o)
    {
        var deltaR = R - o.R;
        var deltaG = G - o.G;
        var deltaB = B - o.B;

        // get euclidean distance between the two colors in RGB space
        // https://en.wikipedia.org/wiki/Color_difference#sRGB

        return Math.Sqrt(Math.Pow(deltaR, 2) + Math.Pow(deltaG, 2) + Math.Pow(deltaB, 2));
    }

    /// <summary>
    ///     Gets the CIE Delta E 1976 color difference between this color and another color by converting both colors to the CIE-LAB color space and calculating the Euclidean distance between their L*, a*, and b* values, 
    ///     providing a more perceptually accurate measure of color difference that accounts for human visual sensitivity to different colors.
    /// </summary>
    /// <param name="o">The other color to calculate deltaE from.</param>
    /// <returns>The CIE Delta E 1976 color difference between the two colors, where a higher value indicates greater perceptual difference.</returns>
    public double GetDeltaE(Colr o)
    {
        // https://stackoverflow.com/questions/9018016/how-to-compare-two-colors-for-similarity-difference
        // use CIE-LAB color space for better perceptual distance measurement

        GetLAB(out var l1, out var a1, out var b1);
        o.GetLAB(out var l2, out var a2, out var b2);

        // get deltaE between the two colors using CIE76 formula
        // https://en.wikipedia.org/wiki/Color_difference#CIE76

        var deltaL = l1 - l2;
        var deltaA = a1 - a2;
        var deltaB = b1 - b2;

        return Math.Sqrt(Math.Pow(deltaL, 2) + Math.Pow(deltaA, 2) + Math.Pow(deltaB, 2));
    }

    /// <summary>
    ///     Gets the complementary color by rotating the hue by 180 degrees in the HSV color space while keeping the saturation and value (brightness) the same.
    /// </summary>
    /// <remarks>
    ///     This produces a color that is opposite on the color wheel and provides maximum contrast to the original color.
    /// </remarks>
    /// <returns>A new <see cref="Colr"/> value that is the complementary value of the current color.</returns>
    public Colr GetComplementaryColor()
    {
        GetHSV(out var h, out var s, out var v);

        var shiftH = Rotate(h, 180);

        return new(shiftH, s, v);
    }

    /// <summary>
    ///     Gets the gamma corrected color by applying a gamma function over R, G, B while retaining the alpha channel.
    /// </summary>
    /// <returns>A new <see cref="Colr"/> value that is the gamma-corrected value of the current color.</returns>
    public Colr GetGammaCorrectedColor()
    {
        return new(
            (byte)(Math.Clamp(Gamma(R), 0, 1) * byte.MaxValue),
            (byte)(Math.Clamp(Gamma(G), 0, 1) * byte.MaxValue),
            (byte)(Math.Clamp(Gamma(B), 0, 1) * byte.MaxValue),
            A
        );
    }

    /// <summary>
    ///     Gets a composite index based on the provided index type for algorithmic sorting.
    /// </summary>
    /// <param name="indexType">The type of index to generate for this value.</param>
    /// <returns>A value representing a floating point (composite) index for the current value produced according to <paramref name="indexType"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided type is not a named value of <see cref="CompositeIndexType"/>.</exception>
    public double GetCompositeIndex(CompositeIndexType indexType)
    {
        return indexType switch
        {
            CompositeIndexType.HLV1D or CompositeIndexType.HLV2D
                => GetHLVIndex(indexType is CompositeIndexType.HLV1D),
            CompositeIndexType.HLV1DInverted or CompositeIndexType.HLV2DInverted
                => GetHLVInvertedIndex(indexType is CompositeIndexType.HLV1DInverted),
            CompositeIndexType.HSV
                => GetHSVIndex(),
            _ => throw new ArgumentException("Invalid index type.", nameof(indexType)),
        };
    }

    /// <summary>
    ///     Gets the HSV color space representation of the current value as H, S, V.
    /// </summary>
    /// <returns>A <see cref="ValueTuple{T1, T2, T3}"/> containing H, S, V.</returns>
    public (float H, float S, float V) GetHSV()
    {
        GetHSV(out var h, out var s, out var v);

        return (h, s, v);
    }

    /// <summary>
    ///     Gets the CIE-XYZ color space representation of the current value as X, Y, Z.
    /// </summary>
    /// <returns>A <see cref="ValueTuple{T1, T2, T3}"/> containing X, Y, Z.</returns>
    public (float X, float Y, float Z) GetXYZ()
    {
        GetXYZ(out var x, out var y, out var z);

        return (x, y, z);
    }

    /// <summary>
    ///     Gets the CIE-LAb color space representation of the current value as L*, A*, B*.
    /// </summary>
    /// <returns>A <see cref="ValueTuple{T1, T2, T3}"/> containing L, A, B.</returns>
    public (float L, float A, float B) GetLAB()
    {
        GetLAB(out var l, out var a, out var b);

        return (l, a, b);
    }

    /// <summary>
    ///     Gets the HSL color space representation of the current value as H, S, L.
    /// </summary>
    /// <returns>A <see cref="ValueTuple{T1, T2, T3}"/> containing H, S, L.</returns>
    public (float H, float S, float L) GetHSL()
        => (GetHue(), GetSaturation(), GetBrightness());

    /// <summary>
    ///     Gets the HSLA color space representation of the current value as H, S, L, A.
    /// </summary>
    /// <returns>A <see cref="ValueTuple{T1, T2, T3, T4}"/> containing H, S, L, A.</returns>
    public (float H, float S, float L, float A) GetHSLA()
        => (GetHue(), GetSaturation(), GetBrightness(), A / 255f);

    /// <summary>
    ///     Gets the minimum value among the RGB channels of the color.
    /// </summary>
    /// <returns>The smallest value in the set of R, G, B in this color.</returns>
    public int Min()
    {
        GetMinMax(out var min, out _);

        return min;
    }

    /// <summary>
    ///     Gets the maximum value among the RGB channels of the color. 
    /// </summary>
    /// <returns>The largest value in the set of R, G, B in this color.</returns>
    public int Max()
    {
        GetMinMax(out _, out var max);

        return max;
    }

    /// <summary>
    ///     Creates a <see cref="Color"/> from the current <see cref="Colr"/> instance.
    /// </summary>
    /// <returns>A new <see cref="Color"/> created from the sRGB value of this <see cref="Colr"/>.</returns>
    public Color ToColor()
    {
        var systemColor = new SystemColor
        {
            Value = Value,
            State = 0x0002 // Refer to System.Drawing.Color.StateARGBValueValid.
        };

        return Unsafe.As<SystemColor, Color>(ref systemColor);
    }

    /// <summary>
    ///     Checks equality to another object.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns><see langword="true"/> if the other object's value equals the current value; otherwise <see langword="false"/>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Colr other && Value == other.Value;

    /// <summary>
    ///     Checks equality to another <see cref="Colr"/> value by comparing their inner <see cref="Value"/>.
    /// </summary>
    /// <param name="other">The value to check equality for.</param>
    /// <returns><see langword="true"/> if the other value equals the current value; otherwise <see langword="false"/>.</returns>
    public bool Equals(Colr other)
        => other.Value == Value;

    /// <summary>
    ///     Checks equality to another <see cref="Color"/> value by comparing the R, G, B and A channels.
    /// </summary>
    /// <param name="other"></param>
    /// <returns><see langword="true"/> if the other value equals the current value; otherwise <see langword="false"/>.</returns>
    public bool Equals(Color other)
        => other.R == R && other.G == G && other.B == B && other.A == A;

    /// <summary>
    ///     Gets the hash code for the current value by returning the hash code of the inner <see cref="Value"/>.
    /// </summary>
    /// <returns>A hash code for the current value.</returns>
    public override int GetHashCode()
        => Value.GetHashCode();

    /// <summary>
    ///     Gets a web-format string representation of the current value in sRGB (A) color space.
    /// </summary>
    /// <returns>A string representing web-format sRGB (A) color space.</returns>
    public override string ToString()
        => ToString(ColorFormat.RGBA);

    /// <summary>
    ///     Gets a web-format string representation of the current value in the chosen format.
    /// </summary>
    /// <param name="format">The target format for the current value.</param>
    /// <returns>A string representing web-format of the current value.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided format is not a named value of <see cref="ColorFormat"/>.</exception>
    public string ToString(ColorFormat format)
    {
        switch (format)
        {
            case ColorFormat.RGB:
                return $"rgb({R}, {G}, {B})";
            case ColorFormat.RGBA:
                return $"rgba({R}, {G}, {B}, {A})";
            case ColorFormat.HSL:
                {
                    var (h, s, l) = GetHSL();

                    return $"hsl({h}, {s}, {l})";
                }
            case ColorFormat.HSLA:
                {
                    var (h, s, l, a) = GetHSLA();

                    return $"hsla({h}, {s}, {l}, {a}";
                }
            case ColorFormat.HSV:
                {
                    var (h, s, v) = GetHSV();

                    return $"hsv({h}, {s}, {v})";
                }
            case ColorFormat.CIEXYZ:
                {
                    var (x, y, z) = GetXYZ();

                    return $"xyz({x}, {y}, {z})";
                }
            case ColorFormat.CIELAB:
                {
                    var (l, a, b) = GetLAB();

                    return $"lab({l}, {a}, {b})";
                }
            case ColorFormat.HEX:
                {
                    return $"#{GetOrderedRGBA():X8}";
                }
            default:
                throw new ArgumentException("Invalid color format", nameof(format));
        }
    }

    object ICloneable.Clone()
        => new Colr(Value);

    #region Optimization

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GetMinMax(out int min, out int max)
    {
        if (R > G)
        {
            max = R;
            min = G;
        }
        else
        {
            max = G;
            min = R;
        }

        if (B > max)
            max = B;
        else if (B < min)
            min = B;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsRGBEquals()
        => R == G && G == B;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GetXYZ(out float x, out float y, out float z)
        => CIEXYZ(VLinear(R), VLinear(G), VLinear(B), out x, out y, out z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GetLAB(out float l, out float a, out float b)
    {
        GetXYZ(out var x, out var y, out var z);

        var xS = LStar(x);
        var yS = LStar(y);
        var zS = LStar(z);

        l = yS;
        a = 500f * (xS - yS);
        b = 200f * (yS - zS);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GetHSV(out float h, out float s, out float v)
    {
        GetMinMax(out var min, out var max);

        h = GetHue();
        s = (max == 0) ? 0f : 1f - (1f * min / max);
        v = max / 255f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint GetOrderedRGBA()
    {
        var r = (uint)R;
        var g = (uint)G << 8;
        var b = (uint)B << 16;
        var a = (uint)A << 24;

        return r | g | b | a;
    }

    #endregion

    #region Operators

    /// <summary>
    ///     Compares two <see cref="Colr"/> values for equality by comparing their inner <see cref="Value"/>.
    /// </summary>
    public static bool operator ==(Colr left, Colr right)
        => left.Equals(right);

    /// <summary>
    ///     Compares two <see cref="Colr"/> values for non-equality by comparing their inner <see cref="Value"/>.
    /// </summary>
    public static bool operator !=(Colr left, Colr right)
        => !left.Equals(right);

    /// <summary>
    ///     Converts a <see cref="Colr"/> value to a 32-bit unsigned integer representation of RGBA.
    /// </summary>
    [CLSCompliant(false)]
    public static implicit operator uint(Colr color)
        => (uint)(color.R | (color.G << 8) | (color.B << 16) | (color.A << 24));

    /// <summary>
    ///     Converts a 32-bit unsigned integer representation of RGBA to a <see cref="Colr"/> value by interpreting the least significant byte as the A (alpha) channel, followed by the B (blue), G (green), and R (red) channels in that order.
    /// </summary>
    [CLSCompliant(false)]
    public static implicit operator Colr(uint rgba)
        => new(rgba);

    #endregion
}

/// <summary>
///     Provides extension methods for the Colr structure to enhance its functionality.
/// </summary>
public static class ColrExtensions
{
    private readonly static Func<object?, object> _getValueInvoker = typeof(Color)
        .GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance)!
        .GetValue!;

    /// <summary>
    ///     Converts the specified Color structure to a Colr instance.
    /// </summary>
    /// <param name="color">The Color structure to convert.</param>
    /// <returns>A Colr instance that represents the specified Color.</returns>
    public static Colr ToColr(this Color color)
    {
        var uintColorValue = unchecked((uint)(long)_getValueInvoker(color));

        return new Colr(uintColorValue);
    }
}