using Godot;

namespace GA.GameProgramming
{
    public class Hit
    {
        public Vector2 Point;
        public Vector2 Normal;
        public Vector2 Penetration;
    }

    public static class CustomPhysics
    {
        public static Hit IntersectsCircle(Vector2 rectCenter, Vector2 rectExtents, Vector2 circleCenter, float radius)
        {
            Vector2 expandedExtents = rectExtents + new Vector2(radius, radius);

            Vector2 delta = circleCenter - rectCenter;

            float penetrationX = expandedExtents.X - Mathf.Abs(delta.X);
            float penetrationY = expandedExtents.Y - Mathf.Abs(delta.Y);

            if (penetrationX < 0 || penetrationY < 0)
                return null;

            Vector2 normal;
            Vector2 penetration;
            Vector2 collisionPoint;

            if (penetrationX < penetrationY)
            {
                float signX = Mathf.Sign(delta.X);
                normal = new Vector2(signX, 0);
                penetration = new Vector2(penetrationX * signX, 0);
                collisionPoint = new Vector2(rectCenter.X + rectExtents.X * signX, circleCenter.Y);
            }
            else
            {
                float signY = Mathf.Sign(delta.Y);
                normal = new Vector2(0, signY);
                penetration = new Vector2(0, penetrationY * signY);
                collisionPoint = new Vector2(circleCenter.X, rectCenter.Y + rectExtents.Y * signY);
            }

            return new Hit { Point = collisionPoint, Normal = normal, Penetration = penetration };
        }

        public static Vector2 Bounce(Vector2 direction, Vector2 normal)
        {
            Vector2 parallel = direction.Dot(normal) * normal;
            Vector2 perpendicular = direction - parallel;
            return perpendicular - parallel;
        }
    }
}