using System.Drawing;

namespace Colrs;

public readonly partial struct Colr
{
    /// <summary>
    ///     Creates a new <see cref="Colr"/> from the specified red, green, blue, and alpha values.
    /// </summary>
    /// <param name="r">The red channel to create this color from.</param>
    /// <param name="g">The green channel to create this color from.</param>
    /// <param name="b">The blue channel to create this color from.</param>
    /// <param name="a">The alpha channel to create this color from. This parameter is optional and defaults to 255 (fully opaque) if not provided.</param>
    /// <returns>A new <see cref="Colr"/> value from the provided values.</returns>
    public static Colr FromRGB(byte r, byte g, byte b, byte a = byte.MaxValue)
        => new(r, g, b, a);

    /// <summary>
    ///     Creates a new <see cref="Colr"/> from the specified hue, saturation, and value (brightness) components.
    /// </summary>
    /// <param name="h">The hue to create this color from, in a range between 0 and 360 degrees.</param>
    /// <param name="s">The saturation to create this color from, in a range between 0 and 1.</param>
    /// <param name="v">The value (brightness) to create this color from, in a range between 0 and 1.</param>
    /// <returns>A new <see cref="Colr"/> value from the provided values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the provided values are less than, or more than the accepted range.</exception>
    public static Colr FromHSV(float h, float s, float v)
    {
        if (h < 0f || h > MAX_DEGREES)
            throw new ArgumentOutOfRangeException(nameof(h), "Hue value must be between 0 and 360.");
        if (s < 0f || s > 1f)
            throw new ArgumentOutOfRangeException(nameof(s), "Saturation value must be between 0 and 1.");
        if (v < 0f || v > 1f)
            throw new ArgumentOutOfRangeException(nameof(v), "Value (brightness) must be between 0 and 1.");

        return new(h, s, v);
    }

    /// <summary>
    ///     Creates a new <see cref="Colr"/> from the specified hue, saturation, lightness, and alpha components.
    /// </summary>
    /// <param name="h">The hue to create this color from, in a range between 0 and 360 degrees.</param>
    /// <param name="s">The saturation to create this color from, in a range between 0 and 1.</param>
    /// <param name="l">The lightness to create this color from, in a range between 0 and 1.</param>
    /// <param name="a">The alpha to create this color from, in a range between 0 and 1. This parameter is optional and defaults to 1 (fully opaque) if not provided.</param>
    /// <returns>A new <see cref="Colr"/> value from the provided values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the provided values are less than, or more than the accepted range.</exception>
    public static Colr FromHSL(float h, float s, float l, float a = 1f)
    {
        if (h < 0f || h > MAX_DEGREES)
            throw new ArgumentOutOfRangeException(nameof(h), "Hue value must be between 0 and 360.");
        if (s < 0f || s > 1f)
            throw new ArgumentOutOfRangeException(nameof(s), "Saturation value must be between 0 and 1.");
        if (l < 0f || l > 1f)
            throw new ArgumentOutOfRangeException(nameof(l), "Lightness value must be between 0 and 1.");
        if (a < 0f || a > 1f)
            throw new ArgumentOutOfRangeException(nameof(a), "Alpha value must be between 0 and 1.");

        return new(h, s, l, a);
    }

    /// <summary>
    ///     Creates a new <see cref="Colr"/> with random red, green, and blue values, and an alpha value of 255 (fully opaque).
    /// </summary>
    /// <returns>A new <see cref="Colr"/> with random R, G and B channels.</returns>
    public static Colr FromRandom()
        => new((byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), byte.MaxValue);

    /// <summary>
    ///     Creates a new <see cref="Colr"/> from a System.Drawing.Color struct. The alpha, red, green, and blue channels of the provided color are used to create the new <see cref="Colr"/> value.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to create a new <see cref="Colr"/> from.</param>
    /// <returns>A new <see cref="Colr"/> value from the provided value.</returns>
    public static Colr FromSystem(Color color)
        => new(color.R, color.G, color.B, color.A);
}
