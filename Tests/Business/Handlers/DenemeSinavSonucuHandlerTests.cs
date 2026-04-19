
using Business.Handlers.DenemeSinavSonucus.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.DenemeSinavSonucus.Queries.GetDenemeSinavSonucuQuery;
using Entities.Concrete;
using static Business.Handlers.DenemeSinavSonucus.Queries.GetDenemeSinavSonucusQuery;
using static Business.Handlers.DenemeSinavSonucus.Commands.CreateDenemeSinavSonucuCommand;
using Business.Handlers.DenemeSinavSonucus.Commands;
using Business.Constants;
using static Business.Handlers.DenemeSinavSonucus.Commands.UpdateDenemeSinavSonucuCommand;
using static Business.Handlers.DenemeSinavSonucus.Commands.DeleteDenemeSinavSonucuCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class DenemeSinavSonucuHandlerTests
    {
        Mock<IDenemeSinavSonucuRepository> _denemeSinavSonucuRepository;
        Mock<IDenemeSinaviRepository> _denemeSinaviRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _denemeSinavSonucuRepository = new Mock<IDenemeSinavSonucuRepository>();
            _denemeSinaviRepository = new Mock<IDenemeSinaviRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task DenemeSinavSonucu_GetQuery_Success()
        {
            //Arrange
            var query = new GetDenemeSinavSonucuQuery();

            _denemeSinavSonucuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavSonucu, bool>>>())).ReturnsAsync(new DenemeSinavSonucu()
//propertyler buraya yazılacak
//{																		
//DenemeSinavSonucuId = 1,
//DenemeSinavSonucuName = "Test"
//}
);

            var handler = new GetDenemeSinavSonucuQueryHandler(_denemeSinavSonucuRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.DenemeSinavSonucuId.Should().Be(1);

        }

        [Test]
        public async Task DenemeSinavSonucu_GetQueries_Success()
        {
            //Arrange
            var query = new GetDenemeSinavSonucusQuery();

            _denemeSinavSonucuRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<DenemeSinavSonucu, bool>>>()))
                        .ReturnsAsync(new List<DenemeSinavSonucu> { new DenemeSinavSonucu() { /*TODO:propertyler buraya yazılacak DenemeSinavSonucuId = 1, DenemeSinavSonucuName = "test"*/ } });

            var handler = new GetDenemeSinavSonucusQueryHandler(_denemeSinavSonucuRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<DenemeSinavSonucu>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task DenemeSinavSonucu_CreateCommand_Success()
        {
            DenemeSinavSonucu rt = null;
            //Arrange
            var command = new CreateDenemeSinavSonucuCommand();
            //propertyler buraya yazılacak
            //command.DenemeSinavSonucuName = "deneme";

            _denemeSinavSonucuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavSonucu, bool>>>()))
                        .ReturnsAsync(rt);

            _denemeSinavSonucuRepository.Setup(x => x.Add(It.IsAny<DenemeSinavSonucu>())).Returns(new DenemeSinavSonucu());

            var handler = new CreateDenemeSinavSonucuCommandHandler(_denemeSinavSonucuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _denemeSinavSonucuRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task DenemeSinavSonucu_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateDenemeSinavSonucuCommand();
            //propertyler buraya yazılacak 
            //command.DenemeSinavSonucuName = "test";

            _denemeSinavSonucuRepository.Setup(x => x.Query())
                                           .Returns(new List<DenemeSinavSonucu> { new DenemeSinavSonucu() { /*TODO:propertyler buraya yazılacak DenemeSinavSonucuId = 1, DenemeSinavSonucuName = "test"*/ } }.AsQueryable());

            _denemeSinavSonucuRepository.Setup(x => x.Add(It.IsAny<DenemeSinavSonucu>())).Returns(new DenemeSinavSonucu());

            var handler = new CreateDenemeSinavSonucuCommandHandler(_denemeSinavSonucuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task DenemeSinavSonucu_UpdateCommand_WithoutHttpUser_ReturnsSessionError()
        {
            //Arrange — oturum yokken GetUserId() = 0
            var command = new UpdateDenemeSinavSonucuCommand
            {
                Id = 1,
                DenemeSinaviId = 1,
                DersId = 1,
            };

            _denemeSinavSonucuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavSonucu, bool>>>()))
                        .ReturnsAsync(new DenemeSinavSonucu { Id = 1, DenemeSinaviId = 1, DersId = 1 });

            _denemeSinaviRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavi, bool>>>()))
                .ReturnsAsync(new DenemeSinavi { Id = 1, UserId = 1 });

            _denemeSinavSonucuRepository.Setup(x => x.Update(It.IsAny<DenemeSinavSonucu>())).Returns(new DenemeSinavSonucu());

            var handler = new UpdateDenemeSinavSonucuCommandHandler(
                _denemeSinavSonucuRepository.Object,
                _denemeSinaviRepository.Object,
                _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Contain("yetkiniz");
        }

        [Test]
        public async Task DenemeSinavSonucu_DeleteCommand_WithoutHttpUser_ReturnsAuthError()
        {
            //Arrange
            var command = new DeleteDenemeSinavSonucuCommand { Id = 1 };

            _denemeSinavSonucuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavSonucu, bool>>>()))
                        .ReturnsAsync(new DenemeSinavSonucu { Id = 1, DenemeSinaviId = 1 });

            _denemeSinaviRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<DenemeSinavi, bool>>>()))
                .ReturnsAsync(new DenemeSinavi { Id = 1, UserId = 1 });

            var handler = new DeleteDenemeSinavSonucuCommandHandler(
                _denemeSinavSonucuRepository.Object,
                _denemeSinaviRepository.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Contain("yetkiniz");
        }
    }
}

