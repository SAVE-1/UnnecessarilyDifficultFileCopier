namespace Threads
{
    class FileCopier
    {
        private string source;
        private string target;

        private Queue<string> operationQueueForProgress = new Queue<string>();
        private Queue<string> operationQueueForReaderWriter = new Queue<string>();

        private bool _complete = false;

        public FileCopier(string source, string target)
        {
            this.source = source;
            this.target = target;
        }

        public void Start()
        {
#if DEV
            //// empty the target folder everytime, easier for development
            //Console.WriteLine($"Clearing {target} dir");
            //DirectoryInfo di = new DirectoryInfo(target);

            //foreach (FileInfo file in di.GetFiles())
            //{
            //    file.Delete();
            //}
#endif

            Thread reader = new Thread(new ThreadStart(Reader));
            reader.Start();

            Thread progress = new Thread(new ThreadStart(ProgressWriter));
            progress.Start();

            Thread copier = new Thread(new ThreadStart(Copier));
            copier.Start();


        }

        private void ProgressWriter()
        {
            Console.WriteLine("ProgressWriter-thread starts");
            while (_complete == false)
            {
                if (operationQueueForProgress.Count > 0)
                {
                    string s = "";
                    lock (operationQueueForProgress)
                    {
                        s = operationQueueForProgress.Dequeue();
                    }
                    Console.WriteLine(s);
                }
            }
        }

        private void Reader()
        {
            Console.WriteLine("ReaderWriter-thread starts");

            string[] files = Directory.GetFiles(source);

            foreach (string file in files)
            {
                lock (operationQueueForProgress)
                {
                    operationQueueForProgress.Enqueue(file);
                }

                lock (operationQueueForReaderWriter)
                {
                    operationQueueForReaderWriter.Enqueue(file);
                }
            }

            Console.WriteLine(operationQueueForProgress.Count);
            _complete = true;
        }

        private void Copier()
        {
            while (_complete == false)
            {
                if (operationQueueForReaderWriter.Count > 0)
                {
                    lock (operationQueueForReaderWriter)
                    {
                        string val = operationQueueForReaderWriter.Dequeue();

                        Console.WriteLine(val);

                        File.Copy(val, Path.Combine(target, Path.GetFileName(val)), true);
                    }
                }
            }
        }
    }
}
