using System;
using System.Collections.Generic;
using System.Linq;

namespace Inheritance.Geometry.Virtual
{
    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract bool ContainsPoint(Vector3 point);

        public abstract RectangularCuboid GetBoundingBox();
    }

    public class Ball : Body
    {
        public override RectangularCuboid GetBoundingBox()
        {
            return new RectangularCuboid(Position, Radius * 2, Radius * 2, Radius * 2);
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var vector = point - Position;
            var length2 = vector.GetLength2();
            return length2 <= this.Radius * this.Radius;
        }

        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }
    }

    public class RectangularCuboid : Body
    {
        public override RectangularCuboid GetBoundingBox()
        {
            return new RectangularCuboid(Position, SizeX, SizeY, SizeZ);
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var minPoint = new Vector3(
                Position.X - this.SizeX / 2,
                Position.Y - this.SizeY / 2,
                Position.Z - this.SizeZ / 2);
            var maxPoint = new Vector3(
                Position.X + this.SizeX / 2,
                Position.Y + this.SizeY / 2,
                Position.Z + this.SizeZ / 2);

            return point >= minPoint && point <= maxPoint;
        }

        public double SizeX { get; }
        public double SizeY { get; }
        public double SizeZ { get; }

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }
    }

    public class Cylinder : Body
    {
        public override RectangularCuboid GetBoundingBox()
        {
            return new RectangularCuboid(Position, Radius * 2, Radius * 2, SizeZ);
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var vectorX = point.X - Position.X;
            var vectorY = point.Y - Position.Y;
            var length2 = vectorX * vectorX + vectorY * vectorY;
            var minZ = Position.Z - this.SizeZ / 2;
            var maxZ = minZ + this.SizeZ;

            return length2 <= this.Radius * this.Radius && point.Z >= minZ && point.Z <= maxZ;
        }

        public double SizeZ { get; }

        public double Radius { get; }

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position)
        {
            SizeZ = sizeZ;
            Radius = radius;
        }
    }


    public class CompoundBody : Body
    {
        private double GetBoundingSize(double boundingMinPoint, double boundingMaxPoint)
        {
            if (boundingMinPoint < 0 && boundingMaxPoint >= 0)
            {
                return Math.Abs(boundingMinPoint) + boundingMaxPoint;
            }
            else if (boundingMinPoint < 0 && boundingMaxPoint < 0)
            {
                return Math.Abs(boundingMaxPoint - boundingMinPoint);
            }
            else if (boundingMinPoint >= 0 && boundingMaxPoint >= 0)
            {
                return boundingMaxPoint - boundingMinPoint;
            }
            return 1;
        }

        private Vector3 GetBoundingMinPoint(Vector3 boundingMinPoint, Vector3 minPoint)
        {
            if (boundingMinPoint.X >= minPoint.X && boundingMinPoint.Y >= minPoint.Y && boundingMinPoint.Z >= minPoint.Z)
            {
                boundingMinPoint = minPoint;
            }
            else if (boundingMinPoint.X >= minPoint.X && boundingMinPoint.Y >= minPoint.Y)
            {
                boundingMinPoint = new Vector3(minPoint.X, minPoint.Y, boundingMinPoint.Z);
            }
            else if (boundingMinPoint.X >= minPoint.X && boundingMinPoint.Z >= minPoint.Z)
            {
                boundingMinPoint = new Vector3(minPoint.X, boundingMinPoint.Y, minPoint.Z);
            }
            else if (boundingMinPoint.Y >= minPoint.Y && boundingMinPoint.Z >= minPoint.Z)
            {
                boundingMinPoint = new Vector3(boundingMinPoint.X, minPoint.Y, minPoint.Z);
            }
            else if (boundingMinPoint.X >= minPoint.X)
            {
                boundingMinPoint = new Vector3(minPoint.X, boundingMinPoint.Y, boundingMinPoint.Z);
            }
            else if (boundingMinPoint.Y >= minPoint.Y)
            {
                boundingMinPoint = new Vector3(boundingMinPoint.X, minPoint.Y, boundingMinPoint.Z);
            }
            else if (boundingMinPoint.Z >= minPoint.Z)
            {
                boundingMinPoint = new Vector3(boundingMinPoint.X, boundingMinPoint.Y, minPoint.Z);
            }
            return boundingMinPoint;
        }

        private Vector3 GetBoundingMaxPoint(Vector3 boundingMaxPoint, Vector3 maxPoint)
        {
            if (boundingMaxPoint.X <= maxPoint.X && boundingMaxPoint.Y <= maxPoint.Y && boundingMaxPoint.Z <= maxPoint.Z)
            {
                boundingMaxPoint = maxPoint;
            }
            else if (boundingMaxPoint.X <= maxPoint.X && boundingMaxPoint.Y <= maxPoint.Y)
            {
                boundingMaxPoint = new Vector3(maxPoint.X, maxPoint.Y, boundingMaxPoint.Z);
            }
            else if (boundingMaxPoint.X <= maxPoint.X && boundingMaxPoint.Z <= maxPoint.Z)
            {
                boundingMaxPoint = new Vector3(maxPoint.X, boundingMaxPoint.Y, maxPoint.Z);
            }
            else if (boundingMaxPoint.Y <= maxPoint.Y && boundingMaxPoint.Z <= maxPoint.Z)
            {
                boundingMaxPoint = new Vector3(boundingMaxPoint.X, maxPoint.Y, maxPoint.Z);
            }
            else if (boundingMaxPoint.X <= maxPoint.X)
            {
                boundingMaxPoint = new Vector3(maxPoint.X, boundingMaxPoint.Y, boundingMaxPoint.Z);
            }
            else if (boundingMaxPoint.Y <= maxPoint.Y)
            {
                boundingMaxPoint = new Vector3(boundingMaxPoint.X, maxPoint.Y, boundingMaxPoint.Z);
            }
            else if (boundingMaxPoint.Z <= maxPoint.Z)
            {
                boundingMaxPoint = new Vector3(boundingMaxPoint.X, boundingMaxPoint.Y, maxPoint.Z);
            }
            return boundingMaxPoint;
        }


        public override RectangularCuboid GetBoundingBox()
        {
            var boundingMinPoint = new Vector3(1000,1000,1000);
            var boundingMaxPoint = new Vector3(-1000,-1000,-1000);
            double boundingSizeX;
            double boundingSizeY;
            double boundingSizeZ;
            for (int i = 0; i < Parts.Count; i++)
            {
                RectangularCuboid currentRectangularCuboid = Parts[i].GetBoundingBox();
                var minPoint = new Vector3(
                    currentRectangularCuboid.Position.X - currentRectangularCuboid.SizeX / 2,
                    currentRectangularCuboid.Position.Y - currentRectangularCuboid.SizeY / 2,
                    currentRectangularCuboid.Position.Z - currentRectangularCuboid.SizeZ / 2);
                var maxPoint = new Vector3(
                    currentRectangularCuboid.Position.X + currentRectangularCuboid.SizeX / 2,
                    currentRectangularCuboid.Position.Y + currentRectangularCuboid.SizeY / 2,
                    currentRectangularCuboid.Position.Z + currentRectangularCuboid.SizeZ / 2);

                boundingMinPoint = GetBoundingMinPoint(boundingMinPoint, minPoint);
                boundingMaxPoint = GetBoundingMaxPoint(boundingMaxPoint, maxPoint);
            }
            boundingSizeX = GetBoundingSize(boundingMinPoint.X, boundingMaxPoint.X);
            boundingSizeY = GetBoundingSize(boundingMinPoint.Y, boundingMaxPoint.Y);
            boundingSizeZ = GetBoundingSize(boundingMinPoint.Z, boundingMaxPoint.Z);

            var boundingPoint = 
                new Vector3(
                (boundingMaxPoint.X + boundingMinPoint.X) / 2, 
                (boundingMaxPoint.Y + boundingMinPoint.Y) / 2, 
                (boundingMaxPoint.Z + boundingMinPoint.Z) / 2
                );

            return new RectangularCuboid(boundingPoint, boundingSizeX, boundingSizeY, boundingSizeZ);
        }

        public override bool ContainsPoint(Vector3 point)
        {
            return this.Parts.Any(body => body.ContainsPoint(point));
        }

        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }
    }
}