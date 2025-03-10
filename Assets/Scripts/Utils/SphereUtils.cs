using Unity.Mathematics;
using UnityEngine;

namespace Utils
{
    public static class SphereUtils
    {
        public static Vector3 GetPointOnSphere(float radius, float theta, float phi)
        {
            var x = radius * math.sin(phi) * math.cos(theta);
            var y = radius * math.sin(phi) * math.sin(theta);
            var z = radius * math.cos(phi);

            // Notice that we need to set y as z here, because unity's up vector is y not z
            return new Vector3(x, z, y);
        }
    }
}