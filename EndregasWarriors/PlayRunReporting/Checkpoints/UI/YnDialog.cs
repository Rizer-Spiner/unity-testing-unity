using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUXTesting.EndregasWarriors.PlayRunReporting.Checkpoints.UI
{
    public class YnDialog : MonoBehaviour
    {
        public enum STATUS
        {
            WAITING,
            NO,
            YES
        }

        private STATUS _status = STATUS.WAITING;

        public STATUS status
        {
            get { return _status; }
            private set { _status = value; }
        }

        public void setTitle(string title)
        {
            TMP_Text text = GetComponent<TMP_Text>();
            text.text = title;
        }

        // GUI callbacks
        public void YesClicked()
        {
            status = STATUS.YES;
        }

        public void NoClicked()
        {
            status = STATUS.NO;
        }
    }
}