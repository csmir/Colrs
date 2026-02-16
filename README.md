# 🎨 Colrs

Let's be real. `System.Drawing.Color` is a bit of a mess. It includes legacy garbage, bad inline performance and does not compute well.

Colrs is designed to do away with the issues and provide a modern, efficient and accurate color library for .NET. 
It is built on top of the latest C# features (E.G. functional APIs) and is designed to be easy to use and understand.

Let's break down the runtime implementation, and see where Colrs improves:

- [Size](#-size)
- [Performance](#-performance)
- [Mutability](#-mutability)
- [Color Spaces](#-color-spaces)
- [Usability](#-usability)
- [Color Science](#-color-science)

## 🔢 Size

Size matters (take that as you will). `System.Drawing.Color` contains the following:

```cs
// 8 bytes (nint pointer)
private string? name;

// 8 bytes
private long value;

// 2 bytes
private short knownColor;

// 2 bytes
private short state;
```

The struct is 20 bytes in size. Because of 8 byte alignment (most of the time), it is padded to 24 bytes. 
This means that every time you create a color, **you are allocating 24 bytes of memory**.

`Colr` contains only the following:

```cs
public readonly byte B;

public readonly byte G;

public readonly byte R;

public readonly byte A;
```

It doesn't need a genius to understand the stark contrast in size. 
`Colr` only uses 4 bytes. It is also padded to 8 bytes in most cases, but that still means **`System.Drawing.Color` is 3 times larger**.

> The remaining 4 bytes currently unused can be used for future optimizations, such as storing the color space or other metadata.

## ⚡ Performance

When working with colors abundantly, it's unpleasant to have to deal with unnecessary overhead.

`System.Drawing.Color` deconstructs the underlying value in `Value` (regrettably also private) each time you access the R, G, B or A properties. 
This means that **every time you access a color component, you are performing a bitwise operation** to extract the value.

```cs
public byte R => unchecked((byte)(Value >> ARGBRedShift));

public byte G => unchecked((byte)(Value >> ARGBGreenShift));

public byte B => unchecked((byte)(Value >> ARGBBlueShift));

public byte A => unchecked((byte)(Value >> ARGBAlphaShift));
```

To make it even worse, this same logic is repeated for every method call. 
For example, if you call `GetBrightness()`, it will deconstruct the value to get the R, G and B components, and then perform the brightness calculation.

`Colr` on the other hand, stores the color components directly in the struct as demonstrated in the previous section.

This means that accessing the R, G, B or A properties is a simple memory access, with no additional logic required.

> The `Value` property in `Colr` is stored as an aligned value alongside A, R, G and B. 
> It can be accessed without any additional overhead as a the 4 individual bytes aligned.

## 🖊️ Mutability

To change a color in the current `System.Drawing.Color` implementation, you have to create a new color with the desired values.
You decide what those values are. Referencing the old ones takes more time and is more error prone. Documentation on color values is also scarce.

Of course because both implementations are `readonly struct`, you cannot change the values of the color components directly.
Still, because of low-no overhead of Colrs, you can easily create new colors with modified components using the `SetX` and `ShiftX` methods:

```cs
var c = Colr.FromRGB(255, 0, 0) // Red
  .SetGreen(255)	// Green
  .SetBlue(255)		// Blue
  .SetAlpha(128);	// Alpha
```

This is a much more intuitive and efficient way to create new colors based on existing ones.

## 🛰️ Color Spaces

We cannot expect the runtime to be responsible for giving us scientific and accurate color conversion semantics and color space support.

There are many color spaces out there, and each one has its own use cases and applications.

Luckily Colrs is not part of the runtime, and supports a wide range of color spaces, including but not limited to:

- `RGB(A)`
- `HSL(A)`
- `HSV`
- `CIE-Lab`
- `CIE-XYZ`

More color formats are being added all the time, and the library is designed to be easily extensible to support new color spaces in the future.

## 📰 Usability

We can't directly compare the usability of the two libraries, as it is subjective and depends on the use case.

Colrs design however intends to replace `System.Drawing.Color`. To actually do such a thing, it needs to be as easy to use as possible.

It inherits some of the design principles from its runtime counterpart. 
It has the `Colr.ToColor` and `Color.ToColr` and implicit conversions between the two types. 
It also has a similar API for creating colors, such as `Colr.FromRGB`, `Colr.FromHSL` etc.

## 🔍 Color science

Where the counterpart has none, Colrs has a wide range of API's for color science. 
The API is well documented and refers all scientific calculations to their original sources.

Various of those sources include:

- [Euclidian Distance](https://en.wikipedia.org/wiki/Color_difference#sRGB)
- [Delta E](https://en.wikipedia.org/wiki/Color_difference#CIE76)
- [Color Primaries](https://en.wikipedia.org/wiki/SRGB#Primaries)
- [CIE-XYZ Conversion](https://en.wikipedia.org/wiki/CIE_1931_color_space#From_RGB_to_CIE_XYZ)

... and others, all of which are implemented in a way that is efficient and accurate, while also being easy to use.

Several papers and published works are also referenced. 
Rec, CIE, W3C, BT and other standards are all taken into account when designing the API and implementing the color science calculations.