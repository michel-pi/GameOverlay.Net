using System;
using System.Globalization;
using System.Runtime.CompilerServices;

using SharpDX;
using SharpDX.Mathematics.Interop;

/*
 * Original source and license: https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/Matrix3x2.cs
 * Docs:
 * https://docs.microsoft.com/en-us/windows/win32/direct2d/direct2d-transforms-overview
 * https://docs.microsoft.com/en-us/windows/win32/api/d2d1helper/nl-d2d1helper-matrix3x2f
 */

namespace GameOverlay.Drawing
{
	/// <summary>
	/// Represents a 3x2 matrix which is used to apply transformations on a render target and geometry.
	/// </summary>
	public struct TransformationMatrix
	{
		/// <summary>
		/// Gets an empty matrix.
		/// </summary>
		public static readonly TransformationMatrix Empty = new TransformationMatrix();

		/// <summary>
		/// Gets the identity matrix.
		/// </summary>
		/// <value>The identity matrix.</value>
		public static readonly TransformationMatrix Identity = new TransformationMatrix(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);

		private RawMatrix3x2 _matrix;

		/// <summary>
		/// Element (1,1)
		/// </summary>
		public float M11
		{
			get => _matrix.M11;
			set => _matrix.M11 = value;
		}

		/// <summary>
		/// Element (1,2)
		/// </summary>
		public float M12
		{
			get => _matrix.M12;
			set => _matrix.M12 = value;
		}

		/// <summary>
		/// Element (2,1)
		/// </summary>
		public float M21
		{
			get => _matrix.M21;
			set => _matrix.M21 = value;
		}

		/// <summary>
		/// Element (2,2)
		/// </summary>
		public float M22
		{
			get => _matrix.M22;
			set => _matrix.M22 = value;
		}

		/// <summary>
		/// Element (3,1)
		/// </summary>
		public float M31
		{
			get => _matrix.M31;
			set => _matrix.M31 = value;
		}

		/// <summary>
		/// Element (3,2)
		/// </summary>
		public float M32
		{
			get => _matrix.M32;
			set => _matrix.M32 = value;
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the matrix component, depending on the index.</value>
		/// <param name="index">The zero-based index of the component to access.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 5].</exception>
		public float this[int index]
		{
			get
			{
				switch(index)
				{
					case 0:
						return _matrix.M11;
					case 1:
						return _matrix.M12;
					case 2:
						return _matrix.M21;
					case 3:
						return _matrix.M22;
					case 4:
						return _matrix.M31;
					case 5:
						return _matrix.M32;
					default:
							throw new ArgumentOutOfRangeException(nameof(index));
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						_matrix.M11 = value;
						break;
					case 1:
						_matrix.M12 = value;
						break;
					case 2:
						_matrix.M21 = value;
						break;
					case 3:
						_matrix.M22 = value;
						break;
					case 4:
						_matrix.M31 = value;
						break;
					case 5:
						_matrix.M32 = value;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(index));
				}
			}
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the matrix component, depending on the index.</value>
		/// <param name="row">The row of the matrix to access.</param>
		/// <param name="column">The column of the matrix to access.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>is out of the range [0, 3].</exception>
		public float this[int row, int column]
		{
			get
			{
				if (row < 0 || row > 2) throw new ArgumentOutOfRangeException(nameof(row));
				if (column < 0 || column > 1) throw new ArgumentOutOfRangeException(nameof(column));

				return this[(row * 2) + column];
			}
			set
			{
				if (row < 0 || row > 2) throw new ArgumentOutOfRangeException(nameof(row));
				if (column < 0 || column > 1) throw new ArgumentOutOfRangeException(nameof(column));

				this[(row * 2) + column] = value;
			}
		}

		/// <summary>
		/// Gets or sets the translation of the matrix; that is M31 and M32.
		/// </summary>
		public Point TranslationVector
		{
			get { return new Point(M31, M32); }
			set { M31 = value.X; M32 = value.Y; }
		}

		/// <summary>
		/// Gets or sets the scale of the matrix; that is M11 and M22.
		/// </summary>
		public Point ScaleVector
		{
			get { return new Point(M11, M22); }
			set { M11 = value.X; M22 = value.Y; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is an identity matrix.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is an identity matrix; otherwise, <c>false</c>.
		/// </value>
		public bool IsIdentity
		{
			get { return Equals(Identity); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransformationMatrix"/> struct.
		/// </summary>
		/// <param name="value">The value that will be assigned to all components.</param>
		public TransformationMatrix(float value)
		{
			_matrix = new RawMatrix3x2(value, value, value, value, value, value);
		}

		/// <summary>
		/// Initializes a new TransformationMatrix using the given RawMatrix3x2.
		/// </summary>
		/// <param name="matrix"></param>
		public TransformationMatrix(RawMatrix3x2 matrix)
		{
			_matrix = matrix;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransformationMatrix"/> struct.
		/// </summary>
		/// <param name="m11">The value to assign at row 1 column 1 of the matrix.</param>
		/// <param name="m12">The value to assign at row 1 column 2 of the matrix.</param>
		/// <param name="m21">The value to assign at row 2 column 1 of the matrix.</param>
		/// <param name="m22">The value to assign at row 2 column 2 of the matrix.</param>
		/// <param name="m31">The value to assign at row 3 column 1 of the matrix.</param>
		/// <param name="m32">The value to assign at row 3 column 2 of the matrix.</param>
		public TransformationMatrix(float m11, float m12, float m21, float m22, float m31, float m32)
		{
			_matrix = new RawMatrix3x2(m11, m12, m21, m22, m31, m32);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransformationMatrix"/> struct.
		/// </summary>
		/// <param name="values">The values to assign to the components of the matrix. This must be an array with six elements.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than six elements.</exception>
		public TransformationMatrix(float[] values)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));
			if (values.Length != 6) throw new ArgumentOutOfRangeException(nameof(values));

			_matrix = new RawMatrix3x2(values[0], values[1], values[2], values[3], values[4], values[5]);
		}

		/// <summary>
		/// Creates an array containing the elements of the matrix.
		/// </summary>
		/// <returns>A six-element array containing the components of the matrix.</returns>
		public float[] ToArray()
		{
			return new float[] { M11, M12, M21, M22, M31, M32 };
		}

		/// <summary>
		/// Determines the sum of two matrices.
		/// </summary>
		/// <param name="left">The first matrix to add.</param>
		/// <param name="right">The second matrix to add.</param>
		public static TransformationMatrix Add(ref TransformationMatrix left, ref TransformationMatrix right)
		{
			return new TransformationMatrix(
				left.M11 + right.M11,
				left.M12 + right.M12,
				left.M21 + right.M21,
				left.M22 + right.M22,
				left.M31 + right.M31,
				left.M32 + right.M32);
		}

		/// <summary>
		/// Determines the difference between two matrices.
		/// </summary>
		/// <param name="left">The first matrix to subtract.</param>
		/// <param name="right">The second matrix to subtract.</param>
		public static TransformationMatrix Subtract(ref TransformationMatrix left, ref TransformationMatrix right)
		{
			return new TransformationMatrix(
				left.M11 - right.M11,
				left.M12 - right.M12,
				left.M21 - right.M21,
				left.M22 - right.M22,
				left.M31 - right.M31,
				left.M32 - right.M32);
		}

		/// <summary>
		/// Scales a matrix by the given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		public static TransformationMatrix Multiply(ref TransformationMatrix left, ref TransformationMatrix right)
		{
			return new TransformationMatrix(
				(left.M11 * right.M11) + (left.M12 * right.M21),
				(left.M11 * right.M12) + (left.M12 * right.M22),
				(left.M21 * right.M11) + (left.M22 * right.M21),
				(left.M21 * right.M12) + (left.M22 * right.M22),
				(left.M31 * right.M11) + (left.M32 * right.M21) + right.M31,
				(left.M31 * right.M12) + (left.M32 * right.M22) + right.M32);
		}

		/// <summary>
		/// Scales a matrix by the given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		public static TransformationMatrix Multiply(ref TransformationMatrix left, float right)
		{
			return new TransformationMatrix(
				left.M11 * right,
				left.M12 * right,
				left.M21 * right,
				left.M22 * right,
				left.M31 * right,
				left.M32 * right);
		}

		/// <summary>
		/// Scales a matrix by the given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		public static TransformationMatrix Divide(ref TransformationMatrix left, float right)
		{
			var inv = 1.0f / right;

			return new TransformationMatrix(
				left.M11 * inv,
				left.M12 * inv,
				left.M21 * inv,
				left.M22 * inv,
				left.M31 * inv,
				left.M32 * inv);
		}

		/// <summary>
		/// Scales a matrix by the given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		public static TransformationMatrix Divide(ref TransformationMatrix left, ref TransformationMatrix right)
		{
			return new TransformationMatrix(
				left.M11 / right.M11,
				left.M12 / right.M12,
				left.M21 / right.M21,
				left.M22 / right.M22,
				left.M31 / right.M31,
				left.M32 / right.M32);
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="value">The matrix to be negated.</param>
		public static TransformationMatrix Negate(ref TransformationMatrix value)
		{
			return new TransformationMatrix(
				-value.M11,
				-value.M12,
				-value.M21,
				-value.M22,
				-value.M31,
				-value.M32);
		}

		/// <summary>
		/// Adds two matrices.
		/// </summary>
		/// <param name="left">The first matrix to add.</param>
		/// <param name="right">The second matrix to add.</param>
		/// <returns>The sum of the two matrices.</returns>
		public static TransformationMatrix operator +(TransformationMatrix left, TransformationMatrix right)
		{
			return Add(ref left, ref right);
		}

		/// <summary>
		/// Subtracts two matrices.
		/// </summary>
		/// <param name="left">The first matrix to subtract.</param>
		/// <param name="right">The second matrix to subtract.</param>
		/// <returns>The difference between the two matrices.</returns>
		public static TransformationMatrix operator -(TransformationMatrix left, TransformationMatrix right)
		{
			return Subtract(ref left, ref right);
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="value">The matrix to negate.</param>
		/// <returns>The negated matrix.</returns>
		public static TransformationMatrix operator -(TransformationMatrix value)
		{
			return Negate(ref value);
		}

		/// <summary>
		/// Scales a matrix by a given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		/// <returns>The scaled matrix.</returns>
		public static TransformationMatrix operator *(TransformationMatrix left, TransformationMatrix right)
		{
			return Multiply(ref left, ref right);
		}

		/// <summary>
		/// Scales a matrix by a given value.
		/// </summary>
		/// <param name="left">The amount by which to scale.</param>
		/// <param name="right">The matrix to scale.</param>
		/// <returns>The scaled matrix.</returns>
		public static TransformationMatrix operator *(TransformationMatrix left, float right)
		{
			return Multiply(ref left, right);
		}

		/// <summary>
		/// Divides two matrices.
		/// </summary>
		/// <param name="left">The first matrix to divide.</param>
		/// <param name="right">The second matrix to divide.</param>
		/// <returns>The quotient of the two matrices.</returns>
		public static TransformationMatrix operator /(TransformationMatrix left, TransformationMatrix right)
		{
			return Divide(ref left, ref right);
		}

		/// <summary>
		/// Scales a matrix by a given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		/// <returns>The scaled matrix.</returns>
		public static TransformationMatrix operator /(TransformationMatrix left, float right)
		{
			return Divide(ref left, right);
		}

		/// <summary>
		/// Performs a linear interpolation between two matrices.
		/// </summary>
		/// <param name="start">Start matrix.</param>
		/// <param name="end">End matrix.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		/// <remarks>
		/// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
		/// </remarks>
		public static TransformationMatrix Lerp(ref TransformationMatrix start, ref TransformationMatrix end, float amount)
		{
			return new TransformationMatrix(
				MathUtil.Lerp(start.M11, end.M11, amount),
				MathUtil.Lerp(start.M12, end.M12, amount),
				MathUtil.Lerp(start.M21, end.M21, amount),
				MathUtil.Lerp(start.M22, end.M22, amount),
				MathUtil.Lerp(start.M31, end.M31, amount),
				MathUtil.Lerp(start.M32, end.M32, amount));
		}

		/// <summary>
		/// Performs a cubic interpolation between two matrices.
		/// </summary>
		/// <param name="start">Start matrix.</param>
		/// <param name="end">End matrix.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		public static TransformationMatrix SmoothStep(ref TransformationMatrix start, ref TransformationMatrix end, float amount)
		{
			amount = MathUtil.SmoothStep(amount);
			return Lerp(ref start, ref end, amount);
		}

		/// <summary>
		/// Creates a matrix that scales along the x-axis and y-axis.
		/// </summary>
		/// <param name="scale">Scaling factor for both axes.</param>
		public static TransformationMatrix Scaling(ref Point scale)
		{
			return Scaling(scale.X, scale.Y);
		}

		/// <summary>
		/// Creates a matrix that scales along the x-axis and y-axis.
		/// </summary>
		/// <param name="scale">Scaling factor for both axes.</param>
		/// <returns>The created scaling matrix.</returns>
		public static TransformationMatrix Scaling(Point scale)
		{
			return Scaling(ref scale);
		}

		/// <summary>
		/// Creates a matrix that scales along the x-axis and y-axis.
		/// </summary>
		/// <param name="x">Scaling factor that is applied along the x-axis.</param>
		/// <param name="y">Scaling factor that is applied along the y-axis.</param>
		public static TransformationMatrix Scaling(float x, float y)
		{
			var result = Identity;
			result.M11 = x;
			result.M22 = y;
			return result;
		}

		/// <summary>
		/// Creates a matrix that uniformly scales along both axes.
		/// </summary>
		/// <param name="scale">The uniform scale that is applied along both axes.</param>
		public static TransformationMatrix Scaling(float scale)
		{
			var result = Identity;
			result.M11 = result.M22 = scale;
			return result;
		}

		/// <summary>
		/// Creates a matrix that is scaling from a specified center.
		/// </summary>
		/// <param name="x">Scaling factor that is applied along the x-axis.</param>
		/// <param name="y">Scaling factor that is applied along the y-axis.</param>
		/// <param name="center">The center of the scaling.</param>
		/// <returns>The created scaling matrix.</returns>
		public static TransformationMatrix Scaling(float x, float y, Point center)
		{
			return new TransformationMatrix
			{
				M11 = x,
				M12 = 0.0f,
				M21 = 0.0f,
				M22 = y,

				M31 = center.X - (x * center.X),
				M32 = center.Y - (y * center.Y)
			};
		}

		/// <summary>
		/// Calculates the determinant of this matrix.
		/// </summary>
		/// <returns>Result of the determinant.</returns>
		public float Determinant()
		{
			return (M11 * M22) - (M12 * M21);
		}

		/// <summary>
		/// Creates a matrix that rotates.
		/// </summary>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis.</param>
		public static TransformationMatrix Rotation(float angle)
		{
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);

			var result = Identity;
			result.M11 = cos;
			result.M12 = sin;
			result.M21 = -sin;
			result.M22 = cos;

			return result;
		}

		/// <summary>
		/// Creates a matrix that rotates about a specified center.
		/// </summary>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis.</param>
		/// <param name="center">The center of the rotation.</param>
		public static TransformationMatrix Rotation(float angle, Point center)
		{
			return Translation(new Point(-center.X, -center.Y)) * Rotation(angle) * Translation(center);
		}

		/// <summary>
		/// Creates a transformation matrix.
		/// </summary>
		/// <param name="xScale">Scaling factor that is applied along the x-axis.</param>
		/// <param name="yScale">Scaling factor that is applied along the y-axis.</param>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis.</param>
		/// <param name="xOffset">X-coordinate offset.</param>
		/// <param name="yOffset">Y-coordinate offset.</param>
		public static TransformationMatrix Transformation(float xScale, float yScale, float angle, float xOffset, float yOffset)
		{
			return Scaling(xScale, yScale) * Rotation(angle) * Translation(xOffset, yOffset);
		}

		/// <summary>
		/// Creates a translation matrix using the specified offsets.
		/// </summary>
		/// <param name="value">The offset for both coordinate planes.</param>
		public static TransformationMatrix Translation(ref Point value)
		{
			return Translation(value.X, value.Y);
		}

		/// <summary>
		/// Creates a translation matrix using the specified offsets.
		/// </summary>
		/// <param name="value">The offset for both coordinate planes.</param>
		/// <returns>The created translation matrix.</returns>
		public static TransformationMatrix Translation(Point value)
		{
			return Translation(ref value);
		}

		/// <summary>
		/// Creates a translation matrix using the specified offsets.
		/// </summary>
		/// <param name="x">X-coordinate offset.</param>
		/// <param name="y">Y-coordinate offset.</param>
		public static TransformationMatrix Translation(float x, float y)
		{
			var result = Identity;
			result.M31 = x;
			result.M32 = y;

			return result;
		}

		/// <summary>
		/// Transforms a vector by this matrix.
		/// </summary>
		/// <param name="matrix">The matrix to use as a transformation matrix.</param>
		/// <param name="point">The original vector to apply the transformation.</param>
		/// <returns>The result of the transformation for the input vector.</returns>
		public static Point TransformPoint(TransformationMatrix matrix, Point point)
		{
			Point result;
			result.X = (point.X * matrix.M11) + (point.Y * matrix.M21) + matrix.M31;
			result.Y = (point.X * matrix.M12) + (point.Y * matrix.M22) + matrix.M32;
			return result;
		}

		/// <summary>
		/// Transforms a vector by this matrix.
		/// </summary>
		/// <param name="matrix">The matrix to use as a transformation matrix.</param>
		/// <param name="point">The original vector to apply the transformation.</param>
		/// <returns></returns>
		public static Point TransformPoint(ref TransformationMatrix matrix, ref Point point)
		{
			Point localResult;
			localResult.X = (point.X * matrix.M11) + (point.Y * matrix.M21) + matrix.M31;
			localResult.Y = (point.X * matrix.M12) + (point.Y * matrix.M22) + matrix.M32;
			return localResult;
		}

		/// <summary>
		/// Calculates the inverse of this matrix instance.
		/// </summary>
		public void Invert()
		{
			InvertHelper(ref this, out this);
		}

		/// <summary>
		/// Calculates the inverse of the specified matrix.
		/// </summary>
		/// <param name="value">The matrix whose inverse is to be calculated.</param>
		/// <param name="result">When the method completes, contains the inverse of the specified matrix.</param>
		private static void InvertHelper(ref TransformationMatrix value, out TransformationMatrix result)
		{
			float determinant = value.Determinant();

			if (MathUtil.IsZero(determinant))
			{
				result = Identity;
				return;
			}

			float invdet = 1.0f / determinant;
			float _offsetX = value.M31;
			float _offsetY = value.M32;

			result = new TransformationMatrix(
				value.M22 * invdet,
				-value.M12 * invdet,
				-value.M21 * invdet,
				value.M11 * invdet,
				((value.M21 * _offsetY) - (_offsetX * value.M22)) * invdet,
				((_offsetX * value.M12) - (value.M11 * _offsetY)) * invdet);
		}

		/// <summary>
		/// Calculates the inverse of the specified matrix.
		/// </summary>
		/// <param name="value">The matrix whose inverse is to be calculated.</param>
		/// <returns>the inverse of the specified matrix.</returns>
		public static TransformationMatrix Invert(TransformationMatrix value)
		{
			return Invert(ref value);
		}

		/// <summary>
		/// Creates a skew matrix.
		/// </summary>
		/// <param name="angleX">Angle of skew along the X-axis in radians.</param>
		/// <param name="angleY">Angle of skew along the Y-axis in radians.</param>
		public static TransformationMatrix Skew(float angleX, float angleY)
		{
			var result = Identity;
			result.M12 = (float)Math.Tan(angleX);
			result.M21 = (float)Math.Tan(angleY);
			return result;
		}

		/// <summary>
		/// Calculates the inverse of the specified matrix.
		/// </summary>
		/// <param name="value">The matrix whose inverse is to be calculated.</param>
		public static TransformationMatrix Invert(ref TransformationMatrix value)
		{
			float determinant = value.Determinant();

			if (MathUtil.IsZero(determinant))
			{
				return Identity;
			}

			float invdet = 1.0f / determinant;
			float _offsetX = value.M31;
			float _offsetY = value.M32;

			return new TransformationMatrix(
				value.M22 * invdet,
				-value.M12 * invdet,
				-value.M21 * invdet,
				value.M11 * invdet,
				((value.M21 * _offsetY) - (_offsetX * value.M22)) * invdet,
				((_offsetX * value.M12) - (value.M11 * _offsetY)) * invdet);
		}

		/// <summary>
		/// Tests for equality between two objects.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public static bool operator ==(TransformationMatrix left, TransformationMatrix right)
		{
			return left.Equals(ref right);
		}

		/// <summary>
		/// Tests for inequality between two objects.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public static bool operator !=(TransformationMatrix left, TransformationMatrix right)
		{
			return !left.Equals(ref right);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "[M11:{0} M12:{1}] [M21:{2} M22:{3}] [M31:{4} M32:{5}]",
				M11, M12, M21, M22, M31, M32);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public string ToString(string format)
		{
			if (format == null)
				return ToString();

			return string.Format(format, CultureInfo.CurrentCulture, "[M11:{0} M12:{1}] [M21:{2} M22:{3}] [M31:{4} M32:{5}]",
				M11.ToString(format, CultureInfo.CurrentCulture), M12.ToString(format, CultureInfo.CurrentCulture),
				M21.ToString(format, CultureInfo.CurrentCulture), M22.ToString(format, CultureInfo.CurrentCulture),
				M31.ToString(format, CultureInfo.CurrentCulture), M32.ToString(format, CultureInfo.CurrentCulture));
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, "[M11:{0} M12:{1}] [M21:{2} M22:{3}] [M31:{4} M32:{5}]",
				M11.ToString(formatProvider), M12.ToString(formatProvider),
				M21.ToString(formatProvider), M22.ToString(formatProvider),
				M31.ToString(formatProvider), M32.ToString(formatProvider));
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
				return ToString(formatProvider);

			return string.Format(format, formatProvider, "[M11:{0} M12:{1}] [M21:{2} M22:{3}] [M31:{4} M32:{5}]",
				M11.ToString(format, formatProvider), M12.ToString(format, formatProvider),
				M21.ToString(format, formatProvider), M22.ToString(format, formatProvider),
				M31.ToString(format, formatProvider), M32.ToString(format, formatProvider));
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = M11.GetHashCode();
				hashCode = (hashCode * 397) ^ M12.GetHashCode();
				hashCode = (hashCode * 397) ^ M21.GetHashCode();
				hashCode = (hashCode * 397) ^ M22.GetHashCode();
				hashCode = (hashCode * 397) ^ M31.GetHashCode();
				hashCode = (hashCode * 397) ^ M32.GetHashCode();
				return hashCode;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="Matrix3x2"/> is equal to this instance.
		/// </summary>
		/// <param name="other">The <see cref="Matrix3x2"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="Matrix3x2"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public bool Equals(ref TransformationMatrix other)
		{
			return MathUtil.NearEqual(other.M11, M11) &&
				MathUtil.NearEqual(other.M12, M12) &&
				MathUtil.NearEqual(other.M21, M21) &&
				MathUtil.NearEqual(other.M22, M22) &&
				MathUtil.NearEqual(other.M31, M31) &&
				MathUtil.NearEqual(other.M32, M32);
		}
		/// <summary>
		/// Determines whether the specified <see cref="Matrix3x2"/> is equal to this instance.
		/// </summary>
		/// <param name="other">The <see cref="Matrix3x2"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="Matrix3x2"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public bool Equals(TransformationMatrix other)
		{
			return Equals(ref other);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="value">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object value)
		{
			if (!(value is TransformationMatrix))
				return false;

			var strongValue = (TransformationMatrix)value;
			return Equals(ref strongValue);
		}

		/// <summary>
		/// Converts the given TransformationMatrix to a RawMatrix3x2.
		/// </summary>
		/// <param name="value">The TransformationMatrix to convert.</param>
		public static implicit operator RawMatrix3x2(TransformationMatrix value)
		{
			return value._matrix;
		}

		/// <summary>
		/// Converts the given RawMatrix3x2 to a TransformationMatrix.
		/// </summary>
		/// <param name="value">The RawMatrix3x2 to convert.</param>
		public static explicit operator TransformationMatrix(RawMatrix3x2 value)
		{
			return new TransformationMatrix(value);
		}
	}
}
