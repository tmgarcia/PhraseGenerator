using System.Collections;
using PhraseGenerator.PhraseGeneration;

namespace PhraseGenerator
{
    //Generating the phrases takes some time, so this is a simple class to run the phrase population on a separate thread and then give the resulting string through a callback
    public class PhraseGenerationJob
    {
        private static int _nextId = 0;
        private int _seed;
        private bool _isDone;
        private object _handle;
        private System.Threading.Thread _thread;
        private string generatedPhrase;
        private System.Action<string,int> callback;
        private bool _isCanceled;
        public int Id { get; private set; }

        public PhraseGenerationJob()
        {
            this.Id = ++_nextId;
        }

        public bool IsDone
        {
            get
            {
                bool temp;
                lock (_handle)
                {
                    temp = _isDone;
                }
                return temp;
            }
            set
            {
                lock (_handle)
                {
                    _isDone = value;
                }
            }
        }
        public bool IsCancled
        {
            get
            {
                bool temp;
                lock (_handle)
                {
                    temp = _isCanceled;
                }
                return temp;
            }
            set
            {
                lock (_handle)
                {
                    _isCanceled = value;
                }
            }
        }
        public void Start(int seed, TemplateCluster cluster, System.Action<string,int> callback)
        {
            _seed = seed;
            _isDone = false;
            _handle = new object();
            _thread = null;
            _isCanceled = false;


            this.callback = callback;
            _thread = new System.Threading.Thread(GeneratePhrase);
            _thread.Start(cluster);
        }
        public void Abort()
        {
            _thread.Abort();
            IsCancled = true;
        }
        public void GeneratePhrase(object data)
        {
            TemplateCluster cluster = data as TemplateCluster;
            generatedPhrase = cluster.Populate(_seed, true);
            IsDone = true;
        }
        public void OnFinished()
        {
            string temp = generatedPhrase;
            callback.Invoke(temp, _seed);
        }

        public bool Update()
        {
            if (IsDone)
            {
                OnFinished();
                return true;
            }
            return IsCancled;
        }
    }
}