namespace Colrs;

public readonly partial struct Colr
{
    const float
        REC_709_R = 0.2126f,
        REC_709_G = 0.7152f,
        REC_709_B = 0.0722f;

    const float
        BT_601_R = 0.299f,
        BT_601_G = 0.587f,
        BT_601_B = 0.114f;

    //const float
    //    REC_2020_R = 0.2627f,
    //    REC_2020_G = 0.6780f,
    //    REC_2020_B = 0.0593f;
    // Do we need REC.2020 coefficients?
    // If so, we can add them later.

    const float
        SRGBLINEAR_THRESHOLD = 0.04045f,
        GAMMA_2_2_THRESHOLD = 0.0031308f;

    const float
        LINEAR_UPPERFACTOR = 12.92f,
        LINEAR_INNERCURVE = 0.055f,
        LINEAR_LOWERFACTOR = 1.055f,
        LINEAR_GAMMACOEFFICIENT = 2.4f;

    const float
        CIE_LSTAR_THRESHOLD = 216f / 24389f,
        CIE_LSTAR_UPPERMUL = 24389f / 27f,
        CIE_LSTAR_CUBEROOT_FACTOR = 1f / 3f,
        CIE_LSTAR_OFFSET = 16f;

    const int
        ZCURVE_SHIFT12 = 00014000377,
        ZCURVE_SHIFT08 = 00014170017,
        ZCURVE_SHIFT04 = 00303030303,
        ZCURVE_SHIFT02 = 01111111111;

    const int 
        CFACTOR = 8,
        MAX_DEGREES = 360;
}
