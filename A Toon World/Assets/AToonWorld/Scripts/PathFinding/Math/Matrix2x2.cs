using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding.Math
{
    public struct Matrix2x2
    {
        private float _a00, _a01, _a10, _a11;
        public Matrix2x2(float a00, float a01, float a10, float a11)
        {
            (_a00, _a01) = (a00, a01);
            (_a10, _a11) = (a10, a11);
        }

        public static Matrix2x2 FromRows(Vector2 firstRow, Vector2 secondRow)
            => new Matrix2x2(firstRow.x, firstRow.y, secondRow.x, secondRow.y);

        public static Matrix2x2 FromColumns(Vector2 firstColumn, Vector2 secondColumn)
            => new Matrix2x2(firstColumn.x, secondColumn.x, firstColumn.y, secondColumn.y);

        public static Matrix2x2 Zero => new Matrix2x2(0, 0, 0, 0);



        public float this[int row, int col] => this[2 * row  + col];
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return _a00;
                    case 1:
                        return _a01;
                    case 2:
                        return _a10;
                    case 3:
                        return _a11;
                    default:
                        throw new InvalidOperationException($"Index {index} is not valid for a 2X2 matrix");
                }
            }
        }

        public float Determinant() => this[1, 1] * this[0, 0] - this[0, 1] * this[1, 0];


        public bool TryInvert(out Matrix2x2 inverse)
        {
            var det = Determinant();
            inverse = det == 0 ? Zero : new Matrix2x2(this[1, 1], -this[0, 1], -this[1, 0], this[0, 0]) * (1/det);
            return det != 0;
        }


        public static Matrix2x2 operator *(float number, Matrix2x2 matrix)
            => new Matrix2x2(number * matrix[0, 0], number * matrix[1, 0], number * matrix[1, 0], number * matrix[1, 1]);

        public static Matrix2x2 operator *(Matrix2x2 matrix, float number)
            => new Matrix2x2(number * matrix[0, 0], number * matrix[1, 0], number * matrix[1, 0], number * matrix[1, 1]);

        public static Matrix2x2 operator + (Matrix2x2 a, Matrix2x2 b)
            => new Matrix2x2(a[0, 0] + b[0, 0], a[0, 1] + b[0, 1], a[1, 0] + b[1, 0], a[1, 1] + b[1, 1]);



        public static Vector2 operator * (Matrix2x2 matrix, Vector2 columnVector)
        {
            Vector2 firstRow = new Vector2(matrix[0, 0], matrix[0, 1]);
            Vector2 secondRow = new Vector2(matrix[1, 0], matrix[1, 1]);
            return new Vector2(Vector2.Dot(firstRow, columnVector), Vector2.Dot(secondRow, columnVector));
        }        
    }
}
