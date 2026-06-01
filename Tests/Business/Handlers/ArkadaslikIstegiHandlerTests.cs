
using Business.Handlers.ArkadaslikIstegis.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.ArkadaslikIstegis.Queries.GetArkadaslikIstegiQuery;
using Entities.Concrete;
using static Business.Handlers.ArkadaslikIstegis.Queries.GetArkadaslikIstegisQuery;
using static Business.Handlers.ArkadaslikIstegis.Commands.CreateArkadaslikIstegiCommand;
using Business.Handlers.ArkadaslikIstegis.Commands;
using Business.Constants;
using static Business.Handlers.ArkadaslikIstegis.Commands.UpdateArkadaslikIstegiCommand;
using static Business.Handlers.ArkadaslikIstegis.Commands.DeleteArkadaslikIstegiCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class ArkadaslikIstegiHandlerTests
    {
        Mock<IArkadaslikIstegiRepository> _arkadaslikIstegiRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _arkadaslikIstegiRepository = new Mock<IArkadaslikIstegiRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task ArkadaslikIstegi_GetQuery_Success()
        {
            //Arrange
            var query = new GetArkadaslikIstegiQuery();

            _arkadaslikIstegiRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ArkadaslikIstegi, bool>>>())).ReturnsAsync(new ArkadaslikIstegi()
//propertyler buraya yazılacak
//{																		
//ArkadaslikIstegiId = 1,
//ArkadaslikIstegiName = "Test"
//}
);

            var handler = new GetArkadaslikIstegiQueryHandler(_arkadaslikIstegiRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.ArkadaslikIstegiId.Should().Be(1);

        }

        [Test]
        public async Task ArkadaslikIstegi_GetQueries_Success()
        {
            //Arrange
            var query = new GetArkadaslikIstegisQuery();

            _arkadaslikIstegiRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<ArkadaslikIstegi, bool>>>()))
                        .ReturnsAsync(new List<ArkadaslikIstegi> { new ArkadaslikIstegi() { /*TODO:propertyler buraya yazılacak ArkadaslikIstegiId = 1, ArkadaslikIstegiName = "test"*/ } });

            var handler = new GetArkadaslikIstegisQueryHandler(_arkadaslikIstegiRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<ArkadaslikIstegi>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task ArkadaslikIstegi_CreateCommand_Success()
        {
            ArkadaslikIstegi rt = null;
            //Arrange
            var command = new CreateArkadaslikIstegiCommand();
            //propertyler buraya yazılacak
            //command.ArkadaslikIstegiName = "deneme";

            _arkadaslikIstegiRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ArkadaslikIstegi, bool>>>()))
                        .ReturnsAsync(rt);

            _arkadaslikIstegiRepository.Setup(x => x.Add(It.IsAny<ArkadaslikIstegi>())).Returns(new ArkadaslikIstegi());

            var handler = new CreateArkadaslikIstegiCommandHandler(_arkadaslikIstegiRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _arkadaslikIstegiRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task ArkadaslikIstegi_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateArkadaslikIstegiCommand();
            //propertyler buraya yazılacak 
            //command.ArkadaslikIstegiName = "test";

            _arkadaslikIstegiRepository.Setup(x => x.Query())
                                           .Returns(new List<ArkadaslikIstegi> { new ArkadaslikIstegi() { /*TODO:propertyler buraya yazılacak ArkadaslikIstegiId = 1, ArkadaslikIstegiName = "test"*/ } }.AsQueryable());

            _arkadaslikIstegiRepository.Setup(x => x.Add(It.IsAny<ArkadaslikIstegi>())).Returns(new ArkadaslikIstegi());

            var handler = new CreateArkadaslikIstegiCommandHandler(_arkadaslikIstegiRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task ArkadaslikIstegi_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateArkadaslikIstegiCommand();
            //command.ArkadaslikIstegiName = "test";

            _arkadaslikIstegiRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ArkadaslikIstegi, bool>>>()))
                        .ReturnsAsync(new ArkadaslikIstegi() { /*TODO:propertyler buraya yazılacak ArkadaslikIstegiId = 1, ArkadaslikIstegiName = "deneme"*/ });

            _arkadaslikIstegiRepository.Setup(x => x.Update(It.IsAny<ArkadaslikIstegi>())).Returns(new ArkadaslikIstegi());

            var handler = new UpdateArkadaslikIstegiCommandHandler(_arkadaslikIstegiRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _arkadaslikIstegiRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task ArkadaslikIstegi_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteArkadaslikIstegiCommand();

            _arkadaslikIstegiRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ArkadaslikIstegi, bool>>>()))
                        .ReturnsAsync(new ArkadaslikIstegi() { /*TODO:propertyler buraya yazılacak ArkadaslikIstegiId = 1, ArkadaslikIstegiName = "deneme"*/});

            _arkadaslikIstegiRepository.Setup(x => x.Delete(It.IsAny<ArkadaslikIstegi>()));

            var handler = new DeleteArkadaslikIstegiCommandHandler(_arkadaslikIstegiRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _arkadaslikIstegiRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

