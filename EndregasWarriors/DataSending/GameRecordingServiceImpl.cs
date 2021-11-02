using Retrofit;
using UniRx;

namespace UnityUXTesting.EndregasWarriors.DataSending
{
    public class GameRecordingServiceImpl : IGameRecordingService
    {
        public IObservable<string> PostPlayRun(MultipartBody body, string contentType)
        {
            throw new System.NotImplementedException();
        }
    }
}