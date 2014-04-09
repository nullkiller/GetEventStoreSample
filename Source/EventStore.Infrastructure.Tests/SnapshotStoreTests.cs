using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using EventStore.Infrastructure.Store;
using EventStore.ReadModel;
using EventStore.Tests.Arrange;
using Ninject;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using EventStore.Domain;

namespace EventStore.Infrastructure.Tests
{
    public class SnapshotStoreTests
    {
        [Fact]
        public void snapshot_store_should_save_and_load_snapshot()
        {
            var kernel = Substitute.For<IKernel>();

            var cacheData = ArrangeCacheData();
            var entities = cacheData.Data;
            var projection1 = ArrangeProjection("entities", cacheData);

            var viewData = ArrangeViewData();
            var users = viewData.Data;
            var projection2 = ArrangeProjection("users", viewData);

            kernel.GetAll(typeof(IProjection)).ReturnsForAnyArgs(new IProjection[] { projection1, projection2 });
            var data = kernel.GetAll(typeof(IProjection));

            var fileManager = new InMemoryFileManager();
            var snapshotStore = new FileSnapshotStore(kernel, fileManager);

            var version = new SnapshotVersion(5);
            snapshotStore.SaveSnapshot(version);

            fileManager.Data.Length.Should().BeGreaterThan(0);

            cacheData.Data = null;
            viewData.Data = null;

            var loadedVersion = snapshotStore.LoadSnapshot();

            cacheData.Data.As<IEnumerable<IAggregate>>().Should().BeEquivalentTo(entities);
            var user = cacheData.Data.As<IEnumerable<IAggregate>>().OfType<User>().First();
            FakeUser.AssertUserCreated(user);

            viewData.Data.As<IEnumerable<UserDto>>().Should().BeEquivalentTo(users);
        }

        [Fact]
        public void snapshot_store_should_handle_no_snapshots()
        {
            var kernel = ArrangeKernel();

            var fileManager = new InMemoryFileManager();
            var snapshotStore = new FileSnapshotStore(kernel, fileManager); 

            var loadedVersion = snapshotStore.LoadSnapshot();

            loadedVersion.LastEventId.Should().Be(SnapshotVersion.NoSnapshot);
        }

        [Fact]
        public void snapshot_store_should_skip_invalid_file_names()
        {
            var kernel = ArrangeKernel();

            var fileManager = new InMemoryFileManager();
            var snapshotStore = new FileSnapshotStore(kernel, fileManager);

            fileManager.FileName = @"App_Data\snapshots\test.bin";

            var loadedVersion = snapshotStore.LoadSnapshot();

            loadedVersion.LastEventId.Should().Be(SnapshotVersion.NoSnapshot);
        }

        [Fact]
        public void snapshot_store_should_skip_invalid_file_format()
        {
            var kernel = ArrangeKernel();

            var fileManager = new InMemoryFileManager();
            var snapshotStore = new FileSnapshotStore(kernel, fileManager);

            fileManager.FileName = @"App_Data\snapshots\snapshot1.json";

            var loadedVersion = snapshotStore.LoadSnapshot();

            loadedVersion.LastEventId.Should().Be(SnapshotVersion.NoSnapshot);
        }

        [Fact]
        public void snapshot_store_should_skip_snapshot_with_not_all_data()
        {
            var kernel = Substitute.For<IKernel>();

            var cacheData = ArrangeCacheData();
            var projection1 = ArrangeProjection("entities", cacheData);

            var viewData = ArrangeViewData();
            var projection2 = ArrangeProjection("users", viewData);

            var list = new List<IProjection>(){ projection1 };

            kernel.GetAll(typeof(IProjection)).ReturnsForAnyArgs(list);
            var data = kernel.GetAll(typeof(IProjection));

            var fileManager = new InMemoryFileManager();
            var snapshotStore = new FileSnapshotStore(kernel, fileManager);

            var version = new SnapshotVersion(5);
            snapshotStore.SaveSnapshot(version);

            list.Add(projection2);

            var loadedVersion = snapshotStore.LoadSnapshot();

            loadedVersion.LastEventId.Should().Be(SnapshotVersion.NoSnapshot);
        }

        private IKernel ArrangeKernel()
        {
            var kernel = Substitute.For<IKernel>();

            var cacheData = ArrangeCacheData();
            var projection1 = ArrangeProjection("entities", cacheData);

            var viewData = ArrangeViewData();
            var projection2 = ArrangeProjection("users", viewData);

            kernel.GetAll(typeof(IProjection)).ReturnsForAnyArgs(new IProjection[] { projection1, projection2 });
            var data = kernel.GetAll(typeof(IProjection));
            return kernel;
        }

        private ProjectionData<IEnumerable<UserDto>> ArrangeViewData()
        {
            var data = new UserDto[] { FakeUser.ArrangeUserDto(), FakeUser.ArrangeUserDto(1) };
            return new ProjectionData<IEnumerable<UserDto>> { Data = data };
        }

        private static ProjectionData<IEnumerable<IAggregate>> ArrangeCacheData()
        {
            var user = FakeUser.ArrangeUser();
            var competence = FakeCompetence.ArrangeCompetence();
            var employee = FakeEmployee.ArrangeEmployee(3);

            var data = new ProjectionData<IEnumerable<IAggregate>> { Data = new IAggregate[] { user, competence, employee } };
            return data;
        }

        private static IProjection ArrangeProjection<T>(string name, ProjectionData<T> data)
        {
            var projection1 = Substitute.For<IProjection>();
            
            projection1.Name.Returns(name);
            projection1.Data = Arg.Do<ProjectionData<T>>(i => data.Data = i.Data);
            projection1.Data.Returns(data);

            data.Name = name;

            return projection1;
        }
    }
}
