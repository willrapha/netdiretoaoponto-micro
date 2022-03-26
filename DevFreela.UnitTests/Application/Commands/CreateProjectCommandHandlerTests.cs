using DevFreela.Application.Commands.CreateProject;
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

namespace DevFreela.UnitTests.Application.Commands
{
    public class CreateProjectCommandHandlerTests
    {
        [Fact]
        public async Task InputDataIsOk_Executed_ReturnProjectId()
        {
            // Arrange
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var createProjectCommand = new CreateProjectCommand
            {
                Title = "Project Name",
                Description = "Project Description",
                TotalCost = 50000,
                IdClient = 1,
                IdFreelancer = 2
            };

            var createProjectCommandHandler = new CreateProjectCommandHandler(projectRepositoryMock.Object);

            // Act
            var id = await createProjectCommandHandler.Handle(createProjectCommand, new CancellationToken());

            // Assert
            // It.IsAny - pode ser qualquer instancia de Project
            Assert.True(id >= 0);
            projectRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>()), Times.Once);

        }
    }
}
