
using System.Collections;
using System.Threading.Tasks;

namespace UnityUXTesting.EndregasWarriors.DataSending
{
    public interface IGameRecordingService
    {
         IEnumerator PostPlayThrough(string filePath);
    }
}