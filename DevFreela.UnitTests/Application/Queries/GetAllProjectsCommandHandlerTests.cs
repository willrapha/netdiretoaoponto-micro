using DevFreela.Application.Queries.GetAllProjects;
using DevFreela.Core.Entities;
using DevFreela.Core.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DevFreela.UnitTests.Application.Queries
{
    public class GetAllProjectsCommandHandlerTests
    {
        [Fact]
        // Given / When / Then
        // Dado que ThreeProjectsExist
        // Quando Executed
        // Retorna ReturnThreeProjectViewModels
        public async Task ThreeProjectsExist_Executed_ReturnThreeProjectViewModelsAsync()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project("Nome Do Teste 1", "Descricao De Teste 1", 1, 2, 1000),
                new Project("Nome Do Teste 2", "Descricao De Teste 2", 1, 2, 2000),
                new Project("Nome Do Teste 3", "Descricao De Teste 3", 1, 2, 3000)
            };

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(projects));

            var getAllProjectsQuery = new GetAllProjectsQuery("");
            var getAllProjectsQueryHandler = new GetAllProjectsQueryHandler(projectRepositoryMock.Object);

            // Act
            var projectViewModelList = await getAllProjectsQueryHandler.Handle(getAllProjectsQuery, new CancellationToken());

            // Assert
            Assert.NotNull(projectViewModelList);
            Assert.NotEmpty(projectViewModelList);
            Assert.Equal(projects.Count, projectViewModelList.Count);

            projectRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }
    }
}
