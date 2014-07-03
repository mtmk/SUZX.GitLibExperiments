using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using EdgeJs;
using LibGit2Sharp;

namespace SUZX.GitLibExperiments
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run((Action)Start).Wait();
            Console.ReadLine();
            return;

            var tmpDir = new TmpDir();

            string init = Repository.Init(tmpDir.Path);

            var repository = new Repository(init);

            File.WriteAllText(Path.Combine(tmpDir.Path, "readme.txt"), "line1\nline2\nline3\n");
            repository.Index.Stage("readme.txt");
            repository.Commit("c1");

            foreach (var obj in repository.ObjectDatabase)
            {
                Console.WriteLine(obj);
                Console.WriteLine(obj.GetType());

                var commit = obj as Commit;
                var blob = obj as Blob;
                var tree = obj as Tree;
                if (commit != null)
                {
                    //commit.
                }
            }

            Console.WriteLine("[hit enter to exit]");
            Console.ReadLine();
        }

        public static async void Start()
        {
            Func<object, Task<object>> createHttpServer = Edge.Func(@"
                var http = require('http');

                return function (port, cb) {
                    var server = http.createServer(function (req, res) {
                        res.end('Hello, world! ' + new Date());
                    }).listen(port, cb);
                };");

            await createHttpServer(8080);

            Console.WriteLine("W: " + await new WebClient().DownloadStringTaskAsync("http://localhost:8080"));
        }
    }

    class TmpDir : IDisposable
    {
        private readonly bool _noclean;
        private readonly string _path;

        public TmpDir(bool noclean = false)
        {
            _noclean = noclean;
            _path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "suzx.gitlibexperiments", Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(_path);
        }

        public string Path
        {
            get { return _path; }
        }


        public void Dispose()
        {
            if (_noclean) return;
            if (!Directory.Exists(_path)) return;

            try
            {
                Directory.Delete(_path, true);
            }
            catch
            {
                Thread.Sleep(100);
                try
                {
                    Directory.Delete(_path, true);
                }
                catch
                {
                }
            }
        }

        ~TmpDir()
        {
            Dispose();
        }
    }
}
