using EventStore.Domain.Core;
using EventStore.Infrastructure.Misc;
using Newtonsoft.Json;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Store
{
    public class FileSnapshotStore: ISnapshotStore
    {
        private IKernel _kernel;
        private IFileManager _fileManager;

        public FileSnapshotStore(IKernel kernel, IFileManager fileManager)
        {
            _kernel = kernel;
            _fileManager = fileManager;
        }

        public SnapshotVersion LoadSnapshot()
        {
            _fileManager.EnsureDirectory(@"~\App_Data");
            _fileManager.EnsureDirectory(@"~\App_Data\snapshots");

            var files = _fileManager.GetFiles(@"~\App_Data\snapshots");

            var lastSnapshot = files.Select(i => GetSnapshotNumber(i)).OrderBy(i => i).LastOrDefault();
            var newVersion = new SnapshotVersion();

            if (lastSnapshot > SnapshotVersion.NoSnapshot)
            {
                newVersion = new SnapshotVersion { LastEventId = lastSnapshot };

                var serializer = new Newtonsoft.Json.JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.All,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                var projections = _kernel.GetAll(typeof(IProjection)).OfType<IProjection>().ToDictionary(i => i.Name);

                using (var stream = _fileManager.OpenFile(@"~\App_Data\snapshots\snapshot" + lastSnapshot + ".json"))
                {
                    try
                    {
                        var dataSet = (IEnumerable<ProjectionData>)serializer.Deserialize(stream, typeof(IEnumerable<ProjectionData>));

                        if (dataSet != null && dataSet.Count() == projections.Count)
                        {
                            foreach (var data in dataSet)
                            {
                                projections[data.Name].Data = data;
                            }
                        }
                        else
                        {
                            newVersion = new SnapshotVersion();
                        }
                    }
                    catch
                    {
                        newVersion = new SnapshotVersion();
                    }
                }
            }

            return newVersion;
        }

        private static long GetSnapshotNumber(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            var start ="snapshot".Length;
            var length = fileName.IndexOf('.') - start;
            long version = SnapshotVersion.NoSnapshot;

            if (!fileName.EndsWith(".json") || !Int64.TryParse(fileName.Substring(start, length), out version))
            {
                version = SnapshotVersion.NoSnapshot;
            }

            return version;
        }

        public void SaveSnapshot(SnapshotVersion version)
        {
            var projections = _kernel.GetAll(typeof(IProjection));
            var data = projections.AsParallel().OfType<IProjection>().Select(i => i.Data).ToArray();

            var serializer = new Newtonsoft.Json.JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.All,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            using (var stream = _fileManager.CreateFile(@"~\App_Data\snapshots\snapshot" + version.LastEventId + ".json"))
            {
                serializer.Serialize(stream, data);
            }
        }
    }
}
