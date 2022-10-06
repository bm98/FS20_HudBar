using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Drawing;

namespace bm98_Map
{
  /// <summary>
  /// Using a .Net System.Drawing.Point: and adding some methods
  /// </summary>
  internal static class Size_Extension
  {
    /*
     System.Drawing.Size:
        Size(Int32, Int32)      Initializes a new instance of the Size structure from the specified dimensions.
        Size(Point)             Initializes a new instance of the Size structure from the specified Point structure.

        Empty                   Gets a Size structure that has a Height and Width value of 0.

        Height                  Gets or sets the vertical component of this Size structure.
        IsEmpty                 Tests whether this Size structure has width and height of 0.
        Width                   Gets or sets the horizontal component of this Size structure.

        Add(Size, Size)         Adds the width and height of one Size structure to the width and height of another Size structure.
        Ceiling(SizeF)          Converts the specified SizeF structure to a Size structure by rounding the values of the Size structure to the next higher integer values.
        Equals(Object)          Tests to see whether the specified object is a Size structure with the same dimensions as this Size structure.
        Equals(Size)            Indicates whether the current object is equal to another object of the same type.
        GetHashCode()           Returns a hash code for this Size structure.
        Round(SizeF)            Converts the specified SizeF structure to a Size structure by rounding the values of the SizeF structure to the nearest integer values.
        Subtract(Size, Size)    Subtracts the width and height of one Size structure from the width and height of another Size structure.
        ToString()              Creates a human-readable string that represents this Size structure.
        Truncate(SizeF)         Converts the specified SizeF structure to a Size structure by truncating the values of the SizeF structure to the next lower integer values.

        Addition(Size, Size)    Adds the width and height of one Size structure to the width and height of another Size structure.
        Equality(Size, Size)    Tests whether two Size structures are equal.
        Explicit(Size to Point) Converts the specified Size structure to a Point structure.
        Implicit(Size to SizeF) Converts the specified Size structure to a SizeF structure.
        Inequality(Size, Size)  Tests whether two Size structures are different.
        Subtraction(Size, Size) Subtracts the width and height of one Size structure from the width and height of another Size structure.

      Extension:
        ToPoint                 Returns a Point item from the Size Values
        Multiply(Int32)         Scale the Size with a factor
        Multiply(Single)        Scale the Size with a factor
        Length                  Length (as Vector length)
     */
    /// <summary>
    /// Returns a Point item from the Size Values
    /// </summary>
    /// <returns>A Point</returns>
    public static Point ToPoint( this Size _s ) => new Point( _s.Width, _s.Height );
    /// <summary>
    /// Returns a Point item from the Size Values
    /// </summary>
    /// <returns>A Point</returns>
    public static PointF ToPointF( this Size _s ) => new PointF( _s.Width, _s.Height );
    /// <summary>
    /// Returns a PointF item from the SizeF Values
    /// </summary>
    /// <returns>A Point</returns>
    public static PointF ToPointF( this SizeF _s ) => new PointF( _s.Width, _s.Height );

    /// <summary>
    /// Returns this scaled with the given factor
    /// </summary>
    public static Size Multiply( this Size _s, Int32 f ) { return new Size( _s.Width * f, _s.Height * f ); }
    /// <summary>
    /// Returns this scaled with the given factor
    /// </summary>
    public static Size Multiply( this Size _s, Single f ) { return new Size( (int)(_s.Width * f), (int)(_s.Height * f) ); }
    /// <summary>
    /// Length of this Size item (Vector Size)
    /// </summary>
    public static float Length( this Size _s ) => (float)Math.Sqrt( _s.Width * _s.Width + _s.Height * _s.Height );
    /// <summary>
    /// Length of this Size item (Vector Size)
    /// </summary>
    public static float Length( this SizeF _s ) => (float)Math.Sqrt( _s.Width * _s.Width + _s.Height * _s.Height );
  }

}