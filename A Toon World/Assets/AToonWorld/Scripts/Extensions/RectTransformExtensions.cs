 using UnityEngine;

 namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class RectTransformExtensions
    {
        public static void SetX(this RectTransform rt, float x)
        {
            rt.position = new Vector3(x, rt.position.y, rt.position.z);
        }
        
        public static void SetY(this RectTransform rt, float y)
        {
            rt.position = new Vector3(rt.position.x, y, rt.position.z);
        }

        public static void SetZ(this RectTransform rt, float z)
        {
            rt.position = new Vector3(rt.position.x, rt.position.y, z);
        }

        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }
    
        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }
    
        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }
    
        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
    }
}
