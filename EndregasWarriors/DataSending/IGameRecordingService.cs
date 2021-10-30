using Retrofit;
using Retrofit.Methods;
using Retrofit.Parameters;


namespace UnityUXTesting.EndregasWarriors.DataSending
{
    public interface IGameRecordingService
    {
        [Multipart]
        [Headers("Content-Type: multipart/form-data")]
        [Post("/video")]
        UniRx.IObservable<string> PostPlayRun(
            [Part] MultipartBody body
        );
    }
}