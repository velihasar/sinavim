
using Business.Handlers.DenemeSinavis.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.DenemeSinavis.Queries.GetDenemeSinaviQuery;
using Entities.Concrete;
using static Business.Handlers.DenemeSinavis.Queries.GetDenemeSinavisQuery;
using static Business.Handlers.DenemeSinavis.Commands.CreateDenemeSinaviCommand;
using Business.Handlers.DenemeSinavis.Commands;
using Business.Constants;
using static Business.Handlers.DenemeSinavis.Commands.UpdateDenemeSinaviCommand;
using static Business.Handlers.DenemeSinavis.Commands.DeleteDenemeSinaviCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class DenemeSinaviHandlerTests
    {
        Mock<IDenemeSinaviRepository> _denemeSinaviRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _denemeSinaviRepository = new Mock<IDenemeSinaviRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task DenemeSinavi_GetQuery_Success()
        {
            //Arrange
            var query = new GetDenemeSinaviQuery();

            _denemeSinaviRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavi, bool>>>())).ReturnsAsync(new DenemeSinavi()
//propertyler buraya yazılacak
//{																		
//DenemeSinaviId = 1,
//DenemeSinaviName = "Test"
//}
);

            var handler = new GetDenemeSinaviQueryHandler(_denemeSinaviRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.DenemeSinaviId.Should().Be(1);

        }

        [Test]
        public async Task DenemeSinavi_GetQueries_Success()
        {
            //Arrange
            var query = new GetDenemeSinavisQuery();

            _denemeSinaviRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<DenemeSinavi, bool>>>()))
                        .ReturnsAsync(new List<DenemeSinavi> { new DenemeSinavi() { /*TODO:propertyler buraya yazılacak DenemeSinaviId = 1, DenemeSinaviName = "test"*/ } });

            var handler = new GetDenemeSinavisQueryHandler(_denemeSinaviRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<DenemeSinavi>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task DenemeSinavi_CreateCommand_Success()
        {
            DenemeSinavi rt = null;
            //Arrange
            var command = new CreateDenemeSinaviCommand();
            //propertyler buraya yazılacak
            //command.DenemeSinaviName = "deneme";

            _denemeSinaviRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavi, bool>>>()))
                        .ReturnsAsync(rt);

            _denemeSinaviRepository.Setup(x => x.Add(It.IsAny<DenemeSinavi>())).Returns(new DenemeSinavi());

            var handler = new CreateDenemeSinaviCommandHandler(_denemeSinaviRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _denemeSinaviRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task DenemeSinavi_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateDenemeSinaviCommand();
            //command.DenemeSinaviName = "test";

            _denemeSinaviRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavi, bool>>>()))
                        .ReturnsAsync(new DenemeSinavi() { /*TODO:propertyler buraya yazılacak DenemeSinaviId = 1, DenemeSinaviName = "deneme"*/ });

            _denemeSinaviRepository.Setup(x => x.Update(It.IsAny<DenemeSinavi>())).Returns(new DenemeSinavi());

            var handler = new UpdateDenemeSinaviCommandHandler(_denemeSinaviRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _denemeSinaviRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task DenemeSinavi_DeleteCommand_WithoutHttpUser_ReturnsSessionError()
        {
            //Arrange — UserInfoExtensions.GetUserId() is 0 without HttpContext; handler buna göre hata döner.
            var command = new DeleteDenemeSinaviCommand { Id = 1 };

            _denemeSinaviRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavi, bool>>>()))
                        .ReturnsAsync(new DenemeSinavi { Id = 1, UserId = 1 });

            var sonucRepo = new Mock<IDenemeSinavSonucuRepository>();
            sonucRepo.Setup(x => x.Query()).Returns(new List<DenemeSinavSonucu>().AsQueryable());

            var handler = new DeleteDenemeSinaviCommandHandler(_denemeSinaviRepository.Object, sonucRepo.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Contain("Oturum");
        }
    }
}

