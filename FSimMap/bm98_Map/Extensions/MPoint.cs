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
  internal static class Point_Extension
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
    /// Returns A Size item from this Point values
    /// </summary>
    /// <returns>A Size</returns>
    public static SizeF ToSizeF( this PointF _p ) => new SizeF( _p.X, _p.Y );

    /// <summary>
    /// Translates this Point by the inverse of the specified Point.
    /// </summary>
    public static void OffsetNegative( this Point _p, Point p ) => _p.Offset( -p.X, -p.Y );

    /// <summary>
    /// Returns a Size streching from this point and an End Point
    /// </summary>
    public static Size ToSize( this Point _p, Point _pend ) => new Size( _pend.X - _p.X, _pend.Y - _p.Y );
    /// <summary>
    /// Returns a Size streching from this point and an End Point
    /// </summary>
    public static SizeF ToSizeF( this PointF _p, PointF _pend ) => new SizeF( _pend.X - _p.X, _pend.Y - _p.Y );

    /// <summary>
    /// Returns the vector distance from this point to an end point
    /// </summary>
    public static float Distance( this Point _p, Point _pend ) => (float)_p.ToSize( _pend ).Length( );
    /// <summary>
    /// Returns the vector distance from this point to an end point
    /// </summary>
    public static float Distance( this PointF _p, PointF _pend ) => _p.ToSizeF( _pend ).Length( );



    /// <summary>
    /// Returns a Point containing the result of the Addition from this Point + other Point
    /// </summary>
    public static Point Add( this Point _p, Point _other ) => new Point( _p.X + _other.X, _p.Y + _other.Y );

    /// <summary>
    /// Add the other Point to this Point
    /// </summary>
    public static void Plus( this ref Point _p, Point _other ) { _p.X += _other.X; _p.Y += _other.Y; }


    /// <summary>
    /// Returns a PointF containing the result of the Addition from this PointF + other PointF
    /// </summary>
    public static PointF Add( this PointF _p, PointF _other ) => new PointF( _p.X + _other.X, _p.Y + _other.Y );

    /// <summary>
    /// Add the other PointF to this PointF
    /// </summary>
    public static void Plus( this ref PointF _p, PointF _other ) { _p.X += _other.X; _p.Y += _other.Y; }
    /// <summary>
    /// Add the other Point to this PointF
    /// </summary>
    public static void Plus( this ref PointF _p, Point _other ) { _p.X += _other.X; _p.Y += _other.Y; }



    /// <summary>
    /// Returns a Point containing the result of the Subtraction from this Point - other Point
    /// </summary>
    public static Point Subtract( this Point _p, Point _other ) => new Point( _p.X - _other.X, _p.Y - _other.Y );

    /// <summary>
    /// Subtract the other Point from this Point
    /// </summary>
    public static void Minus( this ref Point _p, Point _other ) { _p.X -= _other.X; _p.Y -= _other.Y; }

    /// <summary>
    /// Returns a PointF containing the result of the Subtraction from this PointF - other PointF
    /// </summary>
    public static PointF Subtract( this PointF _p, PointF _other ) => new PointF( _p.X - _other.X, _p.Y - _other.Y );

    /// <summary>
    /// Subtract the other PointF from this PointF
    /// </summary>
    public static void Minus( this ref PointF _p, PointF _other ) { _p.X -= _other.X; _p.Y -= _other.Y; }
    /// <summary>
    /// Subtract the other Point from this PointF
    /// </summary>
    public static void Minus( this ref PointF _p, Point _other ) { _p.X -= _other.X; _p.Y -= _other.Y; }


    /// <summary>
    /// Multiply this Point with a factor (rounds)
    /// </summary>
    public static void Multiply( this ref Point _p, float factor ) { _p.X = (int)(_p.X * factor); _p.Y = (int)(_p.Y * factor); }
    /// <summary>
    /// Multiply this PointF with a factor
    /// </summary>
    public static void Multiply( this ref PointF _p, float factor ) { _p.X *= factor; _p.Y *= factor; }
  }
}

