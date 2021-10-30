using Retrofit;
using Retrofit.Methods;
using Retrofit.Parameters;


namespace UnityUXTesting.EndregasWarriors.DataSending
{
    public interface IGameRecordingService
    {
        [Multipart]
        [Post("/video")]
        UniRx.IObservable<string> PostPlayRun(
            [Part] MultipartBody body
        );
    }
}