using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain.Core
{
    public interface IProjection
    {
        string Name { get; }
        ProjectionData Data { get; set; }
    }

    public class ProjectionData
    {
        public string Name { get; set; }
    }

    public class ProjectionData<T> : ProjectionData
    {
        public T Data { get; set; }
    }
}
