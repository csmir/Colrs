namespace Colrs;

public readonly partial struct Colr
{
    /// <summary>
    ///     Takes the current red value and adds or removes the specified amount to it, returning a new <see cref="Colr"/> with the modified red value. The resulting value is clamped between 0 and 255.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    public Colr ShiftRed(int amount) 
        => new(ClampToByte(R, amount), G, B, A);

    /// <summary>
    ///     Takes the current red value and sets it to the specified value, returning a new <see cref="Colr"/> with the modified red value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    public Colr SetRed(byte value) 
        => new(value, G, B, A);

    /// <summary>
    ///     Takes the current green value and adds or removes the specified amount to it, returning a new <see cref="Colr"/> with the modified green value. The resulting value is clamped between 0 and 255.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    public Colr ShiftGreen(int amount) 
        => new(R, ClampToByte(G, amount), B, A);

    /// <summary>
    ///     Takes the current green value and sets it to the specified value, returning a new <see cref="Colr"/> with the modified green value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    public Colr SetGreen(byte value) 
        => new(R, value, B, A);

    /// <summary>
    ///     Takes the current blue value and adds or removes the specified amount to it, returning a new <see cref="Colr"/> with the modified blue value. The resulting value is clamped between 0 and 255.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    public Colr ShiftBlue(int amount) 
        => new(R, G, ClampToByte(B, amount), A);

    /// <summary>
    ///     Takes the current blue value and sets it to the specified value, returning a new <see cref="Colr"/> with the modified blue value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    public Colr SetBlue(byte value) 
        => new(R, G, value, A);

    /// <summary>
    ///     Takes the current alpha value and adds or removes the specified amount to it, returning a new <see cref="Colr"/> with the modified alpha value. The resulting value is clamped between 0 and 255.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    public Colr ShiftAlpha(int amount) 
        => new(R, G, B, ClampToByte(A, amount));

    /// <summary>
    ///     Takes the current alpha value and sets it to the specified value, returning a new <see cref="Colr"/> with the modified alpha value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    public Colr SetAlpha(byte value) 
        => new(R, G, B, value);

    /// <summary>
    ///     Shifts the hue of the color by the specified amount, returning a new <see cref="Colr"/> with the modified hue value. The resulting hue value is wrapped around the 0-360 degree range.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="amount"/> is less than -360 or more than 360.</exception>
    public Colr ShiftHue(float amount)
    {
        if (amount < -MAX_DEGREES || amount > MAX_DEGREES)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be between -360 and 360.");

        var hsla = GetHSLA();

        hsla.H = (hsla.H + amount) % MAX_DEGREES;

        if (hsla.H < 0)
            hsla.H += MAX_DEGREES;
        else if (hsla.H > MAX_DEGREES)
            hsla.H -= MAX_DEGREES;

        return new Colr(hsla.H, hsla.S, hsla.L, hsla.A);
    }

    /// <summary>
    ///     Takes the current hue value and sets it to the specified value, returning a new <see cref="Colr"/> with the modified hue value. The resulting hue value is wrapped around the 0-360 degree range.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is less than -360 or more than 360.</exception>
    public Colr SetHue(float value)
    {
        if (value < -MAX_DEGREES || value > MAX_DEGREES)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between -360 and 360.");

        var hsla = GetHSLA();

        hsla.H = value % MAX_DEGREES;

        if (hsla.H < 0)
            hsla.H += MAX_DEGREES;
        else if (hsla.H > MAX_DEGREES)
            hsla.H -= MAX_DEGREES;

        return new Colr(hsla.H, hsla.S, hsla.L, hsla.A);
    }

    /// <summary>
    ///     Takes the current saturation value and adds or removes the specified amount to it, returning a new <see cref="Colr"/> with the modified saturation value. The resulting saturation value is wrapped around the 0-1 range.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="amount"/> is less than -1 or more than 1.</exception>
    public Colr ShiftSaturation(float amount)
    {
        if (amount < -1f || amount > 1f)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be between -1 and 1.");

        var hsla = GetHSLA();
        hsla.S = (hsla.S + amount) % 1f;
        
        if (hsla.S < 0)
            hsla.S += 1f;
        else if (hsla.S > 1f)
            hsla.S -= 1f;

        return new Colr(hsla.H, hsla.S, hsla.L, hsla.A);
    }

    /// <summary>
    ///     Takes the current saturation value and sets it to the specified value, returning a new <see cref="Colr"/> with the modified saturation value. The resulting saturation value is wrapped around the 0-1 range.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is less than -1 or more than 1.</exception>
    public Colr SetSaturation(float value)
    {
        if (value < -1f || value > 1f)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between -1 and 1.");

        var hsla = GetHSLA();
        hsla.S = value % 1f;

        if (hsla.S < 0)
            hsla.S += 1f;
        else if (hsla.S > 1f)
            hsla.S -= 1f;

        return new Colr(hsla.H, hsla.S, hsla.L, hsla.A);
    }

    /// <summary>
    ///     Takes the current brightness value and adds or removes the specified amount to it, returning a new <see cref="Colr"/> with the modified lightness value. The resulting lightness value is wrapped around the 0-1 range.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="amount"/> is less than -1 or more than 1.</exception>
    public Colr ShiftBrightness(float amount)
    {
        if (amount < -1f || amount > 1f)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be between -1 and 1.");

        var hsla = GetHSLA();
        hsla.L = (hsla.L + amount) % 1f;

        if (hsla.L < 0)
            hsla.L += 1f;
        else if (hsla.L > 1f)
            hsla.L -= 1f;

        return new Colr(hsla.H, hsla.S, hsla.L, hsla.A);
    }

    /// <summary>
    ///     Takes the current brightness value and sets it to the specified value, returning a new <see cref="Colr"/> with the modified lightness value. The resulting lightness value is wrapped around the 0-1 range.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Colr"/> value with the included mutation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is less than -1 or more than 1.</exception>
    public Colr SetBrightness(float value)
    {
        if (value < -1f || value > 1f)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between -1 and 1.");

        var hsla = GetHSLA();
        hsla.L = value % 1f;

        if (hsla.L < 0)
            hsla.L += 1f;
        else if (hsla.L > 1f)
            hsla.L -= 1f;

        return new Colr(hsla.H, hsla.S, hsla.L, hsla.A);
    }

    private static byte ClampToByte(byte oldValue, int shift)
    {
        var newRed = oldValue + shift;

        byte newValue;

        if (newRed > byte.MaxValue)
            newValue = byte.MaxValue;
        else if (newRed < byte.MinValue)
            newValue = byte.MinValue;
        else
            newValue = (byte)newRed;

        return newValue;
    }
}
