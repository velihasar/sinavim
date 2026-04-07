
using Business.Handlers.KullaniciSinavs.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.KullaniciSinavs.Queries.GetKullaniciSinavQuery;
using Entities.Concrete;
using static Business.Handlers.KullaniciSinavs.Queries.GetKullaniciSinavsQuery;
using static Business.Handlers.KullaniciSinavs.Commands.CreateKullaniciSinavCommand;
using Business.Handlers.KullaniciSinavs.Commands;
using Business.Constants;
using static Business.Handlers.KullaniciSinavs.Commands.UpdateKullaniciSinavCommand;
using static Business.Handlers.KullaniciSinavs.Commands.DeleteKullaniciSinavCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class KullaniciSinavHandlerTests
    {
        Mock<IKullaniciSinavRepository> _kullaniciSinavRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _kullaniciSinavRepository = new Mock<IKullaniciSinavRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task KullaniciSinav_GetQuery_Success()
        {
            //Arrange
            var query = new GetKullaniciSinavQuery();

            _kullaniciSinavRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciSinav, bool>>>())).ReturnsAsync(new KullaniciSinav()
//propertyler buraya yazılacak
//{																		
//KullaniciSinavId = 1,
//KullaniciSinavName = "Test"
//}
);

            var handler = new GetKullaniciSinavQueryHandler(_kullaniciSinavRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.KullaniciSinavId.Should().Be(1);

        }

        [Test]
        public async Task KullaniciSinav_GetQueries_Success()
        {
            //Arrange
            var query = new GetKullaniciSinavsQuery();

            _kullaniciSinavRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<KullaniciSinav, bool>>>()))
                        .ReturnsAsync(new List<KullaniciSinav> { new KullaniciSinav() { /*TODO:propertyler buraya yazılacak KullaniciSinavId = 1, KullaniciSinavName = "test"*/ } });

            var handler = new GetKullaniciSinavsQueryHandler(_kullaniciSinavRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<KullaniciSinav>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task KullaniciSinav_CreateCommand_Success()
        {
            KullaniciSinav rt = null;
            //Arrange
            var command = new CreateKullaniciSinavCommand();
            //propertyler buraya yazılacak
            //command.KullaniciSinavName = "deneme";

            _kullaniciSinavRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciSinav, bool>>>()))
                        .ReturnsAsync(rt);

            _kullaniciSinavRepository.Setup(x => x.Add(It.IsAny<KullaniciSinav>())).Returns(new KullaniciSinav());

            var handler = new CreateKullaniciSinavCommandHandler(_kullaniciSinavRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciSinavRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task KullaniciSinav_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateKullaniciSinavCommand();
            //propertyler buraya yazılacak 
            //command.KullaniciSinavName = "test";

            _kullaniciSinavRepository.Setup(x => x.Query())
                                           .Returns(new List<KullaniciSinav> { new KullaniciSinav() { /*TODO:propertyler buraya yazılacak KullaniciSinavId = 1, KullaniciSinavName = "test"*/ } }.AsQueryable());

            _kullaniciSinavRepository.Setup(x => x.Add(It.IsAny<KullaniciSinav>())).Returns(new KullaniciSinav());

            var handler = new CreateKullaniciSinavCommandHandler(_kullaniciSinavRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task KullaniciSinav_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateKullaniciSinavCommand();
            //command.KullaniciSinavName = "test";

            _kullaniciSinavRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciSinav, bool>>>()))
                        .ReturnsAsync(new KullaniciSinav() { /*TODO:propertyler buraya yazılacak KullaniciSinavId = 1, KullaniciSinavName = "deneme"*/ });

            _kullaniciSinavRepository.Setup(x => x.Update(It.IsAny<KullaniciSinav>())).Returns(new KullaniciSinav());

            var handler = new UpdateKullaniciSinavCommandHandler(_kullaniciSinavRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciSinavRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task KullaniciSinav_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteKullaniciSinavCommand();

            _kullaniciSinavRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciSinav, bool>>>()))
                        .ReturnsAsync(new KullaniciSinav() { /*TODO:propertyler buraya yazılacak KullaniciSinavId = 1, KullaniciSinavName = "deneme"*/});

            _kullaniciSinavRepository.Setup(x => x.Delete(It.IsAny<KullaniciSinav>()));

            var handler = new DeleteKullaniciSinavCommandHandler(_kullaniciSinavRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciSinavRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

