
using Business.Handlers.Derses.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.Derses.Queries.GetDersQuery;
using Entities.Concrete;
using static Business.Handlers.Derses.Queries.GetDersesQuery;
using static Business.Handlers.Derses.Commands.CreateDersCommand;
using Business.Handlers.Derses.Commands;
using Business.Constants;
using static Business.Handlers.Derses.Commands.UpdateDersCommand;
using static Business.Handlers.Derses.Commands.DeleteDersCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class DersHandlerTests
    {
        Mock<IDersRepository> _dersRepository;
        Mock<ISinavBolumRepository> _sinavBolumRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _dersRepository = new Mock<IDersRepository>();
            _sinavBolumRepository = new Mock<ISinavBolumRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task Ders_GetQuery_Success()
        {
            //Arrange
            var query = new GetDersQuery();

            _dersRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Ders, bool>>>())).ReturnsAsync(new Ders()
//propertyler buraya yazılacak
//{																		
//DersId = 1,
//DersName = "Test"
//}
);

            var handler = new GetDersQueryHandler(_dersRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.DersId.Should().Be(1);

        }

        [Test]
        public async Task Ders_GetQueries_Success()
        {
            //Arrange
            var query = new GetDersesQuery();

            _dersRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Ders, bool>>>()))
                        .ReturnsAsync(new List<Ders> { new Ders() { /*TODO:propertyler buraya yazılacak DersId = 1, DersName = "test"*/ } });

            var handler = new GetDersesQueryHandler(_dersRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Ders>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task Ders_CreateCommand_Success()
        {
            Ders rt = null;
            //Arrange
            var command = new CreateDersCommand();
            //propertyler buraya yazılacak
            //command.DersName = "deneme";

            _dersRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Ders, bool>>>()))
                        .ReturnsAsync(rt);

            _dersRepository.Setup(x => x.Add(It.IsAny<Ders>())).Returns(new Ders());

            var handler = new CreateDersCommandHandler(_dersRepository.Object, _sinavBolumRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _dersRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Ders_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateDersCommand();
            //propertyler buraya yazılacak 
            //command.DersName = "test";

            _dersRepository.Setup(x => x.Query())
                                           .Returns(new List<Ders> { new Ders() { /*TODO:propertyler buraya yazılacak DersId = 1, DersName = "test"*/ } }.AsQueryable());

            _dersRepository.Setup(x => x.Add(It.IsAny<Ders>())).Returns(new Ders());

            var handler = new CreateDersCommandHandler(_dersRepository.Object, _sinavBolumRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Ders_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateDersCommand();
            //command.DersName = "test";

            _dersRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Ders, bool>>>()))
                        .ReturnsAsync(new Ders() { /*TODO:propertyler buraya yazılacak DersId = 1, DersName = "deneme"*/ });

            _dersRepository.Setup(x => x.Update(It.IsAny<Ders>())).Returns(new Ders());

            var handler = new UpdateDersCommandHandler(_dersRepository.Object, _sinavBolumRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _dersRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Ders_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteDersCommand();

            _dersRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Ders, bool>>>()))
                        .ReturnsAsync(new Ders() { /*TODO:propertyler buraya yazılacak DersId = 1, DersName = "deneme"*/});

            _dersRepository.Setup(x => x.Delete(It.IsAny<Ders>()));

            var handler = new DeleteDersCommandHandler(_dersRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _dersRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

