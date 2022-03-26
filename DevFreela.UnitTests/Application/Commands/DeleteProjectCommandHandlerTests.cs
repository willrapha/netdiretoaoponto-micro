using DevFreela.Application.Commands.DeleteProject;
using DevFreela.Core.Entities;
using DevFreela.Core.Enums;
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
    public class DeleteProjectCommandHandlerTests
    {
        [Fact]
        public async Task InputDataIsOk_Executed_ReturnUserId()
        {
            //Arrange
            var project = new Project("Project Name", "Project Description", 1, 2, 50000);

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock.Setup(x => x.GetByIdAsync(project.Id)).Returns(Task.FromResult(project));

            var deleteProjectCommand = new DeleteProjectCommand(project.Id);
            var deleteProjectCommandHandler = new DeleteProjectCommandHandler(projectRepositoryMock.Object);

            //Act
            project.Start();
            var unit = await deleteProjectCommandHandler.Handle(deleteProjectCommand, new CancellationToken());

            //Assert
            projectRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            Assert.Equal(ProjectStatusEnum.Cancelled, project.Status);
            projectRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
