using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Drawing;

namespace MapLib
{
  /// <summary>
  /// Using a .Net System.Drawing.Point: and adding some methods
  /// </summary>
  public static class Point_Extension
  {
    /*
     System.Drawing.Point:
        Point(Int32)            Initializes a new instance of the Point struct using coordinates specified by an integer value.
        Point(Int32, Int32)     Initializes a new instance of the Point struct with the specified coordinates.
        Point(Size)             Initializes a new instance of the Point struct from a Size.

        Empty                   Represents a Point that has X and Y values set to zero.

        IsEmpty                 Gets a value indicating whether this Point is empty.
        X                       Gets or sets the x-coordinate of this Point.
        Y                       Gets or sets the y-coordinate of this Point.

        Add(Point, Size)        Adds the specified Size to the specified Point.
        Ceiling(PointF)         Converts the specified PointF to a Point by rounding the values of the PointF to the next higher integer values.
        Equals(Object)          Specifies whether this point instance contains the same coordinates as the specified object.
        Equals(Point)           Specifies whether this point instance contains the same coordinates as another point.
        GetHashCode()           Returns a hash code for this Point.
        Offset(Int32, Int32)    Translates this Point by the specified amount.
        Offset(Point)           Translates this Point by the specified Point.
        Round(PointF)           Converts the specified PointF to a Point object by rounding the PointF values to the nearest integer.
        Subtract(Point, Size)   Returns the result of subtracting specified Size from the specified Point.
        ToString()              Converts this Point to a human-readable string.
        Truncate(PointF)        Converts the specified PointF to a Point by truncating the values of the PointF.

        Addition(Point, Size)   Translates a Point by a given Size.
        Equality(Point, Point)  Compares two Point objects. The result specifies whether the values of the X and Y properties of the two Point objects are equal.
        Explicit(Point to Size) Converts the specified Point structure to a Size structure.
        Implicit(Point to PointF) Converts the specified Point structure to a PointF structure.
        Inequality(Point, Point) Compares two Point objects. The result specifies whether the values of the X or Y properties of the two Point objects are unequal.
        Subtraction(Point, Size) Translates a Point by the negative of a given Size.

      Extension:
        ToSize                   Returns A Size item from this Point values
        OffsetNegative(Point)    Translates this Point by the inverse of the specified Point.

     */
    // public static explicit operator Size( this Point, Point p ) -- not possible

    /// <summary>
    /// Returns A Size item from this Point values
    /// </summary>
    /// <returns>A Size</returns>
    public static Size ToSize( this Point _p ) => new Size( _p.X, _p.Y );
    /// <summary>
    /// Translates this Point by the inverse of the specified Point.
    /// </summary>
    public static void OffsetNegative(this Point _p, Point p )=> _p.Offset( -p.X, -p.Y );
    /// <summary>
    /// Returns a Size streching from this point and an End Point
    /// </summary>
    public static Size ToSize( this Point _p, Point _pend ) => new Size( _pend.X - _p.X, _pend.Y - _p.Y );
    /// <summary>
    /// Returns the vector distance from this point to an end point
    /// </summary>
    public static float Distance (this Point _p, Point _pend )=> (float)_p.ToSize( _pend ).Length( );


    /// <summary>
    /// Converts a Point to a SizeF
    /// </summary>
    public static SizeF ToSizeF( this Point _p ) => new SizeF( _p.X, _p.Y );
    /// <summary>
    /// Converts a PointF to a SizeF
    /// </summary>
    public static SizeF ToSizeF( this PointF _p ) => new SizeF( _p.X, _p.Y );

    /// <summary>
    /// Returns the Multiplicative inverse value
    /// </summary>
    public static PointF MInverse( this PointF _p ) => new PointF( 1f / _p.X, 1f / _p.Y );
    /// <summary>
    /// Returns the Multiplicative inverse value
    /// </summary>
    public static PointF MInverse( this Point _p ) => new PointF( 1f / _p.X, 1f / _p.Y );


  }
}

