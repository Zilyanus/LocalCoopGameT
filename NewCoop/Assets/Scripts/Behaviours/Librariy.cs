using UnityEngine.Events;
using System.Threading.Tasks;

namespace UnityEngine
{
    public class Librariy : MonoBehaviour
    {
        public static void _WaitTask(UnityAction action = null, int second = 1)
        {
            if (second <= 0) return;
            Task.Delay(second);
            action();
        }
        public virtual void _AddVelocity(Rigidbody2D rb, float value)
        {
            rb.velocity += Vector2.up * 100 * value * Time.deltaTime;
        }
    }
    
}