
using Business.Handlers.Sinavs.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.Sinavs.Queries.GetSinavQuery;
using Entities.Concrete;
using static Business.Handlers.Sinavs.Queries.GetSinavsQuery;
using static Business.Handlers.Sinavs.Commands.CreateSinavCommand;
using Business.Handlers.Sinavs.Commands;
using Business.Constants;
using static Business.Handlers.Sinavs.Commands.UpdateSinavCommand;
using static Business.Handlers.Sinavs.Commands.DeleteSinavCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class SinavHandlerTests
    {
        Mock<ISinavRepository> _sinavRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _sinavRepository = new Mock<ISinavRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task Sinav_GetQuery_Success()
        {
            //Arrange
            var query = new GetSinavQuery();

            _sinavRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Sinav, bool>>>())).ReturnsAsync(new Sinav()
//propertyler buraya yazılacak
//{																		
//SinavId = 1,
//SinavName = "Test"
//}
);

            var handler = new GetSinavQueryHandler(_sinavRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.SinavId.Should().Be(1);

        }

        [Test]
        public async Task Sinav_GetQueries_Success()
        {
            //Arrange
            var query = new GetSinavsQuery();

            _sinavRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Sinav, bool>>>()))
                        .ReturnsAsync(new List<Sinav> { new Sinav() { /*TODO:propertyler buraya yazılacak SinavId = 1, SinavName = "test"*/ } });

            var handler = new GetSinavsQueryHandler(_sinavRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Sinav>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task Sinav_CreateCommand_Success()
        {
            Sinav rt = null;
            //Arrange
            var command = new CreateSinavCommand();
            //propertyler buraya yazılacak
            //command.SinavName = "deneme";

            _sinavRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Sinav, bool>>>()))
                        .ReturnsAsync(rt);

            _sinavRepository.Setup(x => x.Add(It.IsAny<Sinav>())).Returns(new Sinav());

            var handler = new CreateSinavCommandHandler(_sinavRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _sinavRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Sinav_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateSinavCommand();
            //propertyler buraya yazılacak 
            //command.SinavName = "test";

            _sinavRepository.Setup(x => x.Query())
                                           .Returns(new List<Sinav> { new Sinav() { /*TODO:propertyler buraya yazılacak SinavId = 1, SinavName = "test"*/ } }.AsQueryable());

            _sinavRepository.Setup(x => x.Add(It.IsAny<Sinav>())).Returns(new Sinav());

            var handler = new CreateSinavCommandHandler(_sinavRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Sinav_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateSinavCommand();
            //command.SinavName = "test";

            _sinavRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Sinav, bool>>>()))
                        .ReturnsAsync(new Sinav() { /*TODO:propertyler buraya yazılacak SinavId = 1, SinavName = "deneme"*/ });

            _sinavRepository.Setup(x => x.Update(It.IsAny<Sinav>())).Returns(new Sinav());

            var handler = new UpdateSinavCommandHandler(_sinavRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _sinavRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Sinav_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteSinavCommand();

            _sinavRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Sinav, bool>>>()))
                        .ReturnsAsync(new Sinav() { /*TODO:propertyler buraya yazılacak SinavId = 1, SinavName = "deneme"*/});

            _sinavRepository.Setup(x => x.Delete(It.IsAny<Sinav>()));

            var handler = new DeleteSinavCommandHandler(_sinavRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _sinavRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

