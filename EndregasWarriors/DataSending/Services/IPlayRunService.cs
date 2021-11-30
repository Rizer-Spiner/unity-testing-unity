using System.Collections;
using UnityUXTesting.EndregasWarriors.Common.Model;

namespace UnityUXTesting.EndregasWarriors.DataSending
{
    public interface IPlayRunService
    {
        IEnumerator PostPlayRunReport(PlayRunReport report);
    }
}