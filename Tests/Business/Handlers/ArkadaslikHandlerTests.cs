
using Business.Handlers.Arkadasliks.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.Arkadasliks.Queries.GetArkadaslikQuery;
using Entities.Concrete;
using static Business.Handlers.Arkadasliks.Queries.GetArkadasliksQuery;
using static Business.Handlers.Arkadasliks.Commands.CreateArkadaslikCommand;
using Business.Handlers.Arkadasliks.Commands;
using Business.Constants;
using static Business.Handlers.Arkadasliks.Commands.UpdateArkadaslikCommand;
using static Business.Handlers.Arkadasliks.Commands.DeleteArkadaslikCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class ArkadaslikHandlerTests
    {
        Mock<IArkadaslikRepository> _arkadaslikRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _arkadaslikRepository = new Mock<IArkadaslikRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task Arkadaslik_GetQuery_Success()
        {
            //Arrange
            var query = new GetArkadaslikQuery();

            _arkadaslikRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Arkadaslik, bool>>>())).ReturnsAsync(new Arkadaslik()
//propertyler buraya yazılacak
//{																		
//ArkadaslikId = 1,
//ArkadaslikName = "Test"
//}
);

            var handler = new GetArkadaslikQueryHandler(_arkadaslikRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.ArkadaslikId.Should().Be(1);

        }

        [Test]
        public async Task Arkadaslik_GetQueries_Success()
        {
            //Arrange
            var query = new GetArkadasliksQuery();

            _arkadaslikRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Arkadaslik, bool>>>()))
                        .ReturnsAsync(new List<Arkadaslik> { new Arkadaslik() { /*TODO:propertyler buraya yazılacak ArkadaslikId = 1, ArkadaslikName = "test"*/ } });

            var handler = new GetArkadasliksQueryHandler(_arkadaslikRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Arkadaslik>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task Arkadaslik_CreateCommand_Success()
        {
            Arkadaslik rt = null;
            //Arrange
            var command = new CreateArkadaslikCommand();
            //propertyler buraya yazılacak
            //command.ArkadaslikName = "deneme";

            _arkadaslikRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Arkadaslik, bool>>>()))
                        .ReturnsAsync(rt);

            _arkadaslikRepository.Setup(x => x.Add(It.IsAny<Arkadaslik>())).Returns(new Arkadaslik());

            var handler = new CreateArkadaslikCommandHandler(_arkadaslikRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _arkadaslikRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Arkadaslik_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateArkadaslikCommand();
            //propertyler buraya yazılacak 
            //command.ArkadaslikName = "test";

            _arkadaslikRepository.Setup(x => x.Query())
                                           .Returns(new List<Arkadaslik> { new Arkadaslik() { /*TODO:propertyler buraya yazılacak ArkadaslikId = 1, ArkadaslikName = "test"*/ } }.AsQueryable());

            _arkadaslikRepository.Setup(x => x.Add(It.IsAny<Arkadaslik>())).Returns(new Arkadaslik());

            var handler = new CreateArkadaslikCommandHandler(_arkadaslikRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Arkadaslik_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateArkadaslikCommand();
            //command.ArkadaslikName = "test";

            _arkadaslikRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Arkadaslik, bool>>>()))
                        .ReturnsAsync(new Arkadaslik() { /*TODO:propertyler buraya yazılacak ArkadaslikId = 1, ArkadaslikName = "deneme"*/ });

            _arkadaslikRepository.Setup(x => x.Update(It.IsAny<Arkadaslik>())).Returns(new Arkadaslik());

            var handler = new UpdateArkadaslikCommandHandler(_arkadaslikRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _arkadaslikRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Arkadaslik_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteArkadaslikCommand();

            _arkadaslikRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Arkadaslik, bool>>>()))
                        .ReturnsAsync(new Arkadaslik() { /*TODO:propertyler buraya yazılacak ArkadaslikId = 1, ArkadaslikName = "deneme"*/});

            _arkadaslikRepository.Setup(x => x.Delete(It.IsAny<Arkadaslik>()));

            var handler = new DeleteArkadaslikCommandHandler(_arkadaslikRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _arkadaslikRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

