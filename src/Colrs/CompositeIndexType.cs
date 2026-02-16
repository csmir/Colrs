namespace Colrs;

/// <summary>
///     Generates an independent composite index based on the color's active space (sRGB value).
/// </summary>
/// <remarks>
///     These indexes can be used to sort colors by. 
///     Algorithms that back these composite indexes are specifically designed to produce accurate sort results.
/// </remarks>
public enum CompositeIndexType
{
    /// <summary>
    ///     Produces a hue-first composite index that smoothens out across a single dimension.
    /// </summary>
    HLV1D = 0,

    /// <summary>
    ///     Produces a hue-first composite index that retains a sharp boundary for cutting off at multiple dimensions.
    /// </summary>
    HLV2D = 1,

    /// <summary>
    ///     Produces an inverted hue-first composite index that smoothens out across a single dimension.
    /// </summary>
    HLV1DInverted = 2,

    /// <summary>
    ///     Produces an inverted hue-first composite index that retains a sharp boundary for cutting off at multiple dimensions.
    /// </summary>
    HLV2DInverted = 3,

    /// <summary>
    ///     Produces a depth-first composite index that transitions across any dimension.
    /// </summary>
    HSV = 4,
}
