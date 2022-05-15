using System;
using System.Collections.Generic;

namespace Inheritance.Geometry.Visitor
{
    public interface IVisitor
    {
        Body Visit(Ball ball);
        Body Visit(RectangularCuboid cuboid);
        Body Visit(Cylinder cylinder);
        Body Visit(CompoundBody compoundBody);

    }

    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract Body Accept(IVisitor visitor);
    }

    public class Ball : Body
    {
        internal double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override Body Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class RectangularCuboid : Body
    {
        internal double SizeX { get; }
        internal double SizeY { get; }
        internal double SizeZ { get; }

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override Body Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class Cylinder : Body
    {
        internal double Radius { get; }
        internal double SizeZ { get; }

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position)
        {
            SizeZ = sizeZ;
            Radius = radius;
        }

        public override Body Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class CompoundBody : Body
    {
        internal IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override Body Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class BoundingBoxVisitor : IVisitor
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
            double minValueX;
            double minValueY;
            double minValueZ;
            if (minPoint.X <= boundingMinPoint.X)
                minValueX = minPoint.X;
            else
                minValueX = boundingMinPoint.X;
            if (minPoint.Y <= boundingMinPoint.Y)
                minValueY = minPoint.Y;
            else
                minValueY = boundingMinPoint.Y;
            if (minPoint.Z <= boundingMinPoint.Z)
                minValueZ = minPoint.Z;
            else
                minValueZ = boundingMinPoint.Z;
            boundingMinPoint = new Vector3(minValueX, minValueY, minValueZ);

            return boundingMinPoint;
        }

        private Vector3 GetBoundingMaxPoint(Vector3 boundingMaxPoint, Vector3 maxPoint)
        {
            double maxValueX;
            double maxValueY;
            double maxValueZ;
            if (maxPoint.X >= boundingMaxPoint.X)
                maxValueX = maxPoint.X;
            else
                maxValueX = boundingMaxPoint.X;
            if (maxPoint.Y >= boundingMaxPoint.Y)
                maxValueY = maxPoint.Y;
            else
                maxValueY = boundingMaxPoint.Y;
            if (maxPoint.Z >= boundingMaxPoint.Z)
                maxValueZ = maxPoint.Z;
            else
                maxValueZ = boundingMaxPoint.Z;
            boundingMaxPoint = new Vector3(maxValueX, maxValueY, maxValueZ);

            return boundingMaxPoint;
        }


        public Body Visit(Ball ball)
        {
            return new RectangularCuboid(ball.Position, ball.Radius * 2, ball.Radius * 2, ball.Radius * 2);
        }

        public Body Visit(Cylinder cylinder)
        {
            return new RectangularCuboid(cylinder.Position, cylinder.Radius * 2, cylinder.Radius * 2, cylinder.SizeZ);
        }

        public Body Visit(RectangularCuboid rectangularCuboid)
        {
            return rectangularCuboid;
        }

        private Vector3 GetMinPoint(RectangularCuboid currentRectangularCuboid)
        {
            return new Vector3(
                    currentRectangularCuboid.Position.X - currentRectangularCuboid.SizeX / 2,
                    currentRectangularCuboid.Position.Y - currentRectangularCuboid.SizeY / 2,
                    currentRectangularCuboid.Position.Z - currentRectangularCuboid.SizeZ / 2);
        }

        private Vector3 GetMaxPoint(RectangularCuboid currentRectangularCuboid)
        {
            return new Vector3(
                    currentRectangularCuboid.Position.X + currentRectangularCuboid.SizeX / 2,
                    currentRectangularCuboid.Position.Y + currentRectangularCuboid.SizeY / 2,
                    currentRectangularCuboid.Position.Z + currentRectangularCuboid.SizeZ / 2);
        }

        private Vector3 GetBoundingPoint(Vector3 boundingMinPoint, Vector3 boundingMaxPoint)
        {
            return new Vector3(
                    (boundingMaxPoint.X + boundingMinPoint.X) / 2,
                    (boundingMaxPoint.Y + boundingMinPoint.Y) / 2,
                    (boundingMaxPoint.Z + boundingMinPoint.Z) / 2
                    );
        }

        public Body Visit(CompoundBody compoundBody)
        {
            var boundingMinPoint = new Vector3(1000, 1000, 1000);
            var boundingMaxPoint = new Vector3(-1000, -1000, -1000);
            double boundingSizeX;
            double boundingSizeY;
            double boundingSizeZ;
            var parts = compoundBody.Parts;
            for (int i = 0; i < parts.Count; i++)
            {
                RectangularCuboid currentRectangularCuboid = parts[i].TryAcceptVisitor<RectangularCuboid>(new BoundingBoxVisitor());

                var minPoint = GetMinPoint(currentRectangularCuboid);
                var maxPoint = GetMaxPoint(currentRectangularCuboid);

                boundingMinPoint = GetBoundingMinPoint(boundingMinPoint, minPoint);
                boundingMaxPoint = GetBoundingMaxPoint(boundingMaxPoint, maxPoint);
            }
            boundingSizeX = GetBoundingSize(boundingMinPoint.X, boundingMaxPoint.X);
            boundingSizeY = GetBoundingSize(boundingMinPoint.Y, boundingMaxPoint.Y);
            boundingSizeZ = GetBoundingSize(boundingMinPoint.Z, boundingMaxPoint.Z);

            var boundingPoint = GetBoundingPoint(boundingMinPoint, boundingMaxPoint);

            return new RectangularCuboid(boundingPoint, boundingSizeX, boundingSizeY, boundingSizeZ);
        }
    }

    public class BoxifyVisitor : IVisitor
    {
        public Body Visit(Ball ball)
        {
            return new RectangularCuboid(ball.Position, ball.Radius * 2, ball.Radius * 2, ball.Radius * 2);
        }

        public Body Visit(Cylinder cylinder)
        {
            return new RectangularCuboid(cylinder.Position, cylinder.Radius * 2, cylinder.Radius * 2, cylinder.SizeZ);
        }

        public Body Visit(RectangularCuboid rectangularCuboid)
        {
            return rectangularCuboid;
        }

        public Body Visit(CompoundBody compoundBody)
        {
            var parts = compoundBody.Parts;
            List<Body> newCompoundBody = new List<Body> { };
            for (int i = 0; i < parts.Count; i++)
            {
                var currentRectangularCuboid = parts[i].TryAcceptVisitor<Body>(new BoxifyVisitor());
                newCompoundBody.Add(currentRectangularCuboid);
            }

            return new CompoundBody(newCompoundBody);
        }
    }
}