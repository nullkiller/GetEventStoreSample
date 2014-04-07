using EventStore.Domain;
using EventStore.Domain.Core;
using EventStore.Messages.Employee;
using EventStore.Web.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.GenerateData
{
    class Program
    {
        static readonly Random r = new Random();

        static void Main(string[] args)
        {
            var repository = Bootstrapper.Instance.Get<IRepository>();
            var identityGenerator = Bootstrapper.Instance.Get<IIdentityGenerator>();

            var cometencies = Enumerable.Range(1, 100).Select(i => Competence.Create("C" + i, identityGenerator)).ToList();

            cometencies.ForEach(i => repository.Save(i, Guid.NewGuid()));

            for (var j = 0; j < 10000; j++)
            {
                var employee = Employee.Create("E" + j, identityGenerator);

                var set = Generate(cometencies, 4);
                employee.AddCompetences(set);

                repository.Save(employee, Guid.NewGuid());

                set = Generate(cometencies, 2);
                employee.AddCompetences(set);

                repository.Save(employee, Guid.NewGuid());

                set = Generate(cometencies, 4);
                employee.AddCompetences(set);

                repository.Save(employee, Guid.NewGuid());
            }
        }

        private static IEnumerable<CompetenceDocument> Generate(List<Competence> cometencies, int p)
        {
            return Enumerable.Range(1, p).Select(i => cometencies[r.Next(0, cometencies.Count - 1)]).Select(i => new CompetenceDocument { CompetenceId = i.Id });
        }
    }
}
