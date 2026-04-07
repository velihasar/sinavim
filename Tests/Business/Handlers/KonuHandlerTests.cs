
using Business.Handlers.Konus.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.Konus.Queries.GetKonuQuery;
using Entities.Concrete;
using static Business.Handlers.Konus.Queries.GetKonusQuery;
using static Business.Handlers.Konus.Commands.CreateKonuCommand;
using Business.Handlers.Konus.Commands;
using Business.Constants;
using static Business.Handlers.Konus.Commands.UpdateKonuCommand;
using static Business.Handlers.Konus.Commands.DeleteKonuCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class KonuHandlerTests
    {
        Mock<IKonuRepository> _konuRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _konuRepository = new Mock<IKonuRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task Konu_GetQuery_Success()
        {
            //Arrange
            var query = new GetKonuQuery();

            _konuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Konu, bool>>>())).ReturnsAsync(new Konu()
//propertyler buraya yazılacak
//{																		
//KonuId = 1,
//KonuName = "Test"
//}
);

            var handler = new GetKonuQueryHandler(_konuRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.KonuId.Should().Be(1);

        }

        [Test]
        public async Task Konu_GetQueries_Success()
        {
            //Arrange
            var query = new GetKonusQuery();

            _konuRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Konu, bool>>>()))
                        .ReturnsAsync(new List<Konu> { new Konu() { /*TODO:propertyler buraya yazılacak KonuId = 1, KonuName = "test"*/ } });

            var handler = new GetKonusQueryHandler(_konuRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Konu>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task Konu_CreateCommand_Success()
        {
            Konu rt = null;
            //Arrange
            var command = new CreateKonuCommand();
            //propertyler buraya yazılacak
            //command.KonuName = "deneme";

            _konuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Konu, bool>>>()))
                        .ReturnsAsync(rt);

            _konuRepository.Setup(x => x.Add(It.IsAny<Konu>())).Returns(new Konu());

            var handler = new CreateKonuCommandHandler(_konuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _konuRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Konu_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateKonuCommand();
            //propertyler buraya yazılacak 
            //command.KonuName = "test";

            _konuRepository.Setup(x => x.Query())
                                           .Returns(new List<Konu> { new Konu() { /*TODO:propertyler buraya yazılacak KonuId = 1, KonuName = "test"*/ } }.AsQueryable());

            _konuRepository.Setup(x => x.Add(It.IsAny<Konu>())).Returns(new Konu());

            var handler = new CreateKonuCommandHandler(_konuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Konu_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateKonuCommand();
            //command.KonuName = "test";

            _konuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Konu, bool>>>()))
                        .ReturnsAsync(new Konu() { /*TODO:propertyler buraya yazılacak KonuId = 1, KonuName = "deneme"*/ });

            _konuRepository.Setup(x => x.Update(It.IsAny<Konu>())).Returns(new Konu());

            var handler = new UpdateKonuCommandHandler(_konuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _konuRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Konu_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteKonuCommand();

            _konuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Konu, bool>>>()))
                        .ReturnsAsync(new Konu() { /*TODO:propertyler buraya yazılacak KonuId = 1, KonuName = "deneme"*/});

            _konuRepository.Setup(x => x.Delete(It.IsAny<Konu>()));

            var handler = new DeleteKonuCommandHandler(_konuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _konuRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

